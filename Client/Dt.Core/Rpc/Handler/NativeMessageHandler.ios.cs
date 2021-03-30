#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Foundation;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// iOS中使用NSUrlSession实现Http2通信
    /// 参考：https://github.com/alexrainman/ModernHttpClient
    /// </summary>
    public class NativeMessageHandler : HttpClientHandler
    {
        readonly NSUrlSession _session;

        readonly Dictionary<NSUrlSessionTask, InflightOperation> inflightRequests =
            new Dictionary<NSUrlSessionTask, InflightOperation>();

        readonly Dictionary<HttpRequestMessage, ProgressDelegate> registeredProgressCallbacks =
            new Dictionary<HttpRequestMessage, ProgressDelegate>();

        public NativeMessageHandler()
        {
            var configuration = NSUrlSessionConfiguration.DefaultSessionConfiguration;

            // 两个数据包之间的时间大于该时间则认为超时，默认60秒
            // 为减少服务器推送模式时的重连次数，增大超时限制
            configuration.TimeoutIntervalForRequest = 300;

            // System.Net.ServicePointManager.SecurityProtocol provides a mechanism for specifying supported protocol types
            // for System.Net. Since iOS only provides an API for a minimum and maximum protocol we are not able to port
            // this configuration directly and instead use the specified minimum value when one is specified.
            configuration.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_2;

            var urlSessionDelegate = new DataTaskDelegate(this);
            _session = NSUrlSession.FromConfiguration(configuration, (INSUrlSessionDelegate)urlSessionDelegate, null);
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var headers = request.Headers as IEnumerable<KeyValuePair<string, IEnumerable<string>>>;
            var ms = new MemoryStream();

            if (request.Content != null)
            {
                await request.Content.CopyToAsync(ms).ConfigureAwait(false);
                headers = headers.Union(request.Content.Headers).ToArray();
            }

            var rq = new NSMutableUrlRequest()
            {
                AllowsCellularAccess = true,
                Body = NSData.FromArray(ms.ToArray()),
                CachePolicy = NSUrlRequestCachePolicy.ReloadIgnoringCacheData,
                Headers = headers.Aggregate(new NSMutableDictionary(), (acc, x) =>
                {
                    acc.Add(new NSString(x.Key), new NSString(string.Join(",", x.Value)));
                    return acc;
                }),
                HttpMethod = request.Method.ToString().ToUpperInvariant(),
                Url = NSUrl.FromString(request.RequestUri.AbsoluteUri),
            };

            var op = _session.CreateDataTask(rq);

            cancellationToken.ThrowIfCancellationRequested();

            var ret = new TaskCompletionSource<HttpResponseMessage>();
            cancellationToken.Register(() => ret.TrySetCanceled());

            lock (inflightRequests)
            {
                inflightRequests[op] = new InflightOperation()
                {
                    FutureResponse = ret,
                    Request = request,
                    Progress = getAndRemoveCallbackFromRegister(request),
                    ResponseBody = new ByteArrayListStream(),
                    CancellationToken = cancellationToken,
                };
            }

            op.Resume();
            return await ret.Task.ConfigureAwait(false);
        }

        public void RegisterForProgress(HttpRequestMessage request, ProgressDelegate callback)
        {
            if (callback == null && registeredProgressCallbacks.ContainsKey(request))
            {
                registeredProgressCallbacks.Remove(request);
                return;
            }

            registeredProgressCallbacks[request] = callback;
        }

        ProgressDelegate getAndRemoveCallbackFromRegister(HttpRequestMessage request)
        {
            ProgressDelegate emptyDelegate = delegate { };

            lock (registeredProgressCallbacks)
            {
                if (!registeredProgressCallbacks.ContainsKey(request)) return emptyDelegate;

                var callback = registeredProgressCallbacks[request];
                registeredProgressCallbacks.Remove(request);
                return callback;
            }
        }

        class DataTaskDelegate : NSUrlSessionDataDelegate, INSUrlSessionDelegate
        {
            NativeMessageHandler nativeHandler { get; set; }

            public DataTaskDelegate(NativeMessageHandler handler)
            {
                this.nativeHandler = handler;
            }

            public override void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
            {
                var data = getResponseForTask(dataTask);

                try
                {
                    if (data.CancellationToken.IsCancellationRequested)
                    {
                        dataTask.Cancel();
                    }

                    var resp = (NSHttpUrlResponse)response;

                    var content = new CancellableStreamContent(data.ResponseBody, () =>
                    {
                        if (!data.IsCompleted)
                        {
                            dataTask.Cancel();
                        }
                        data.IsCompleted = true;

                        data.ResponseBody.SetException(new OperationCanceledException());
                    })
                    {
                        Progress = data.Progress
                    };

                    // NB: The double cast is because of a Xamarin compiler bug
                    int status = (int)resp.StatusCode;
                    var ret = new HttpResponseMessage((HttpStatusCode)status)
                    {
                        Content = content,
                        RequestMessage = data.Request,
                    };
                    ret.RequestMessage.RequestUri = new Uri(resp.Url.AbsoluteString);

                    foreach (var v in resp.AllHeaderFields)
                    {
                        // NB: Cocoa trolling us so hard by giving us back dummy
                        // dictionary entries
                        if (v.Key == null || v.Value == null) continue;

                        ret.Headers.TryAddWithoutValidation(v.Key.ToString(), v.Value.ToString());
                        ret.Content.Headers.TryAddWithoutValidation(v.Key.ToString(), v.Value.ToString());
                    }

                    data.FutureResponse.TrySetResult(ret);
                }
                catch (Exception ex)
                {
                    data.FutureResponse.TrySetException(ex);
                }

                completionHandler(NSUrlSessionResponseDisposition.Allow);
            }

            public override void WillCacheResponse(NSUrlSession session, NSUrlSessionDataTask dataTask,
                NSCachedUrlResponse proposedResponse, Action<NSCachedUrlResponse> completionHandler)
            {
                completionHandler(null);
            }

            public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
            {
                var data = getResponseForTask(task);
                data.IsCompleted = true;

                if (error != null)
                {
                    var ex = CreateExceptionForNSError(error);

                    // Pass the exception to the response
                    data.FutureResponse.TrySetException(ex);
                    data.ResponseBody.SetException(ex);
                    return;
                }

                data.ResponseBody.Complete();

                lock (nativeHandler.inflightRequests)
                {
                    nativeHandler.inflightRequests.Remove(task);
                }
            }

            public override void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData byteData)
            {
                var data = getResponseForTask(dataTask);
                var bytes = byteData.ToArray();

                // NB: If we're cancelled, we still might have one more chunk 
                // of data that attempts to be delivered
                if (data.IsCompleted) return;

                data.ResponseBody.AddByteArray(bytes);
            }

            InflightOperation getResponseForTask(NSUrlSessionTask task)
            {
                lock (nativeHandler.inflightRequests)
                {
                    return nativeHandler.inflightRequests[task];
                }
            }

            public override void DidReceiveChallenge(NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
            {
                // 信任所有服务器证书，支持自签名证书
                if (challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrust)
                {
                    completionHandler(NSUrlSessionAuthChallengeDisposition.UseCredential, NSUrlCredential.FromTrust(challenge.ProtectionSpace.ServerSecTrust));
                    return;
                }

                if (challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodNTLM)
                {
                    NetworkCredential credentialsToUse;

                    if (nativeHandler.Credentials != null)
                    {
                        if (nativeHandler.Credentials is NetworkCredential)
                        {
                            credentialsToUse = (NetworkCredential)nativeHandler.Credentials;
                        }
                        else
                        {
                            var uri = this.getResponseForTask(task).Request.RequestUri;
                            credentialsToUse = nativeHandler.Credentials.GetCredential(uri, "NTLM");
                        }
                        var credential = new NSUrlCredential(credentialsToUse.UserName, credentialsToUse.Password, NSUrlCredentialPersistence.ForSession);
                        completionHandler(NSUrlSessionAuthChallengeDisposition.UseCredential, credential);
                    }
                    return;
                }

                completionHandler(NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
            }

            public override void WillPerformHttpRedirection(NSUrlSession session, NSUrlSessionTask task, NSHttpUrlResponse response, NSUrlRequest newRequest, Action<NSUrlRequest> completionHandler)
            {
                NSUrlRequest nextRequest = (nativeHandler.AllowAutoRedirect ? newRequest : null);
                completionHandler(nextRequest);
            }

            Exception CreateExceptionForNSError(NSError error)
            {
                var ret = default(Exception);
                var webExceptionStatus = WebExceptionStatus.UnknownError;

                var innerException = new NSErrorException(error);

                if (error.Domain == NSError.NSUrlErrorDomain)
                {
                    // Convert the error code into an enumeration (this is future
                    // proof, rather than just casting integer)
                    NSUrlErrorExtended urlError;
                    if (!Enum.TryParse<NSUrlErrorExtended>(error.Code.ToString(), out urlError)) urlError = NSUrlErrorExtended.Unknown;

                    // Parse the enum into a web exception status or exception. Some
                    // of these values don't necessarily translate completely to
                    // what WebExceptionStatus supports, so made some best guesses
                    // here.  For your reading pleasure, compare these:
                    //
                    // Apple docs: https://developer.apple.com/library/mac/documentation/Cocoa/Reference/Foundation/Miscellaneous/Foundation_Constants/index.html#//apple_ref/doc/constant_group/URL_Loading_System_Error_Codes
                    // .NET docs: http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus(v=vs.110).aspx
                    switch (urlError)
                    {
                        case NSUrlErrorExtended.Cancelled:
                        case NSUrlErrorExtended.UserCancelledAuthentication:
                            // No more processing is required so just return.
                            var message = error.LocalizedDescription;
                            return new OperationCanceledException(message, innerException);
                        case NSUrlErrorExtended.BadURL:
                        case NSUrlErrorExtended.UnsupportedURL:
                        case NSUrlErrorExtended.CannotConnectToHost:
                        case NSUrlErrorExtended.ResourceUnavailable:
                        case NSUrlErrorExtended.NotConnectedToInternet:
                        case NSUrlErrorExtended.UserAuthenticationRequired:
                        case NSUrlErrorExtended.InternationalRoamingOff:
                        case NSUrlErrorExtended.CallIsActive:
                        case NSUrlErrorExtended.DataNotAllowed:
                            webExceptionStatus = WebExceptionStatus.ConnectFailure;
                            break;
                        case NSUrlErrorExtended.TimedOut:
                            webExceptionStatus = WebExceptionStatus.Timeout;
                            break;
                        case NSUrlErrorExtended.CannotFindHost:
                        case NSUrlErrorExtended.DNSLookupFailed:
                            webExceptionStatus = WebExceptionStatus.NameResolutionFailure;
                            break;
                        case NSUrlErrorExtended.DataLengthExceedsMaximum:
                            webExceptionStatus = WebExceptionStatus.MessageLengthLimitExceeded;
                            break;
                        case NSUrlErrorExtended.NetworkConnectionLost:
                            webExceptionStatus = WebExceptionStatus.ConnectionClosed;
                            break;
                        case NSUrlErrorExtended.HTTPTooManyRedirects:
                        case NSUrlErrorExtended.RedirectToNonExistentLocation:
                            webExceptionStatus = WebExceptionStatus.ProtocolError;
                            break;
                        case NSUrlErrorExtended.RequestBodyStreamExhausted:
                            webExceptionStatus = WebExceptionStatus.SendFailure;
                            break;
                        case NSUrlErrorExtended.BadServerResponse:
                        case NSUrlErrorExtended.ZeroByteResource:
                        case NSUrlErrorExtended.CannotDecodeRawData:
                        case NSUrlErrorExtended.CannotDecodeContentData:
                        case NSUrlErrorExtended.CannotParseResponse:
                        case NSUrlErrorExtended.FileDoesNotExist:
                        case NSUrlErrorExtended.FileIsDirectory:
                        case NSUrlErrorExtended.NoPermissionsToReadFile:
                        case NSUrlErrorExtended.CannotLoadFromNetwork:
                        case NSUrlErrorExtended.CannotCreateFile:
                        case NSUrlErrorExtended.CannotOpenFile:
                        case NSUrlErrorExtended.CannotCloseFile:
                        case NSUrlErrorExtended.CannotWriteToFile:
                        case NSUrlErrorExtended.CannotRemoveFile:
                        case NSUrlErrorExtended.CannotMoveFile:
                        case NSUrlErrorExtended.DownloadDecodingFailedMidStream:
                        case NSUrlErrorExtended.DownloadDecodingFailedToComplete:
                            webExceptionStatus = WebExceptionStatus.ReceiveFailure;
                            break;
                        case NSUrlErrorExtended.SecureConnectionFailed:
                            webExceptionStatus = WebExceptionStatus.SecureChannelFailure;
                            break;
                        case NSUrlErrorExtended.ServerCertificateHasBadDate:
                        case NSUrlErrorExtended.ServerCertificateHasUnknownRoot:
                        case NSUrlErrorExtended.ServerCertificateNotYetValid:
                        case NSUrlErrorExtended.ServerCertificateUntrusted:
                        case NSUrlErrorExtended.ClientCertificateRejected:
                        case NSUrlErrorExtended.ClientCertificateRequired:
                            webExceptionStatus = WebExceptionStatus.TrustFailure;
                            break;
                    }

                    goto done;
                }

                if (error.Domain == CFNetworkError.ErrorDomain)
                {
                    // Convert the error code into an enumeration (this is future
                    // proof, rather than just casting integer)
                    CFNetworkErrors networkError;
                    if (!Enum.TryParse<CFNetworkErrors>(error.Code.ToString(), out networkError))
                    {
                        networkError = CFNetworkErrors.CFHostErrorUnknown;
                    }

                    // Parse the enum into a web exception status or exception. Some
                    // of these values don't necessarily translate completely to
                    // what WebExceptionStatus supports, so made some best guesses
                    // here.  For your reading pleasure, compare these:
                    //
                    // Apple docs: https://developer.apple.com/library/ios/documentation/Networking/Reference/CFNetworkErrors/#//apple_ref/c/tdef/CFNetworkErrors
                    // .NET docs: http://msdn.microsoft.com/en-us/library/system.net.webexceptionstatus(v=vs.110).aspx
                    switch (networkError)
                    {
                        case CFNetworkErrors.CFURLErrorCancelled:
                        case CFNetworkErrors.CFURLErrorUserCancelledAuthentication:
                        case CFNetworkErrors.CFNetServiceErrorCancel:
                            // No more processing is required so just return.
                            var message = error.LocalizedDescription;
                            return new OperationCanceledException(message, innerException);
                        case CFNetworkErrors.CFSOCKS5ErrorBadCredentials:
                        case CFNetworkErrors.CFSOCKS5ErrorUnsupportedNegotiationMethod:
                        case CFNetworkErrors.CFSOCKS5ErrorNoAcceptableMethod:
                        case CFNetworkErrors.CFErrorHttpAuthenticationTypeUnsupported:
                        case CFNetworkErrors.CFErrorHttpBadCredentials:
                        case CFNetworkErrors.CFErrorHttpBadURL:
                        case CFNetworkErrors.CFURLErrorBadURL:
                        case CFNetworkErrors.CFURLErrorUnsupportedURL:
                        case CFNetworkErrors.CFURLErrorCannotConnectToHost:
                        case CFNetworkErrors.CFURLErrorResourceUnavailable:
                        case CFNetworkErrors.CFURLErrorNotConnectedToInternet:
                        case CFNetworkErrors.CFURLErrorUserAuthenticationRequired:
                        case CFNetworkErrors.CFURLErrorInternationalRoamingOff:
                        case CFNetworkErrors.CFURLErrorCallIsActive:
                        case CFNetworkErrors.CFURLErrorDataNotAllowed:
                            webExceptionStatus = WebExceptionStatus.ConnectFailure;
                            break;
                        case CFNetworkErrors.CFURLErrorTimedOut:
                        case CFNetworkErrors.CFNetServiceErrorTimeout:
                            webExceptionStatus = WebExceptionStatus.Timeout;
                            break;
                        case CFNetworkErrors.CFHostErrorHostNotFound:
                        case CFNetworkErrors.CFURLErrorCannotFindHost:
                        case CFNetworkErrors.CFURLErrorDNSLookupFailed:
                        case CFNetworkErrors.CFNetServiceErrorDNSServiceFailure:
                            webExceptionStatus = WebExceptionStatus.NameResolutionFailure;
                            break;
                        case CFNetworkErrors.CFURLErrorDataLengthExceedsMaximum:
                            webExceptionStatus = WebExceptionStatus.MessageLengthLimitExceeded;
                            break;
                        case CFNetworkErrors.CFErrorHttpConnectionLost:
                        case CFNetworkErrors.CFURLErrorNetworkConnectionLost:
                            webExceptionStatus = WebExceptionStatus.ConnectionClosed;
                            break;
                        case CFNetworkErrors.CFErrorHttpRedirectionLoopDetected:
                        case CFNetworkErrors.CFURLErrorHTTPTooManyRedirects:
                        case CFNetworkErrors.CFURLErrorRedirectToNonExistentLocation:
                            webExceptionStatus = WebExceptionStatus.ProtocolError;
                            break;
                        case CFNetworkErrors.CFSOCKSErrorUnknownClientVersion:
                        case CFNetworkErrors.CFSOCKSErrorUnsupportedServerVersion:
                        case CFNetworkErrors.CFErrorHttpParseFailure:
                        case CFNetworkErrors.CFURLErrorRequestBodyStreamExhausted:
                            webExceptionStatus = WebExceptionStatus.SendFailure;
                            break;
                        case CFNetworkErrors.CFSOCKS4ErrorRequestFailed:
                        case CFNetworkErrors.CFSOCKS4ErrorIdentdFailed:
                        case CFNetworkErrors.CFSOCKS4ErrorIdConflict:
                        case CFNetworkErrors.CFSOCKS4ErrorUnknownStatusCode:
                        case CFNetworkErrors.CFSOCKS5ErrorBadState:
                        case CFNetworkErrors.CFSOCKS5ErrorBadResponseAddr:
                        case CFNetworkErrors.CFURLErrorBadServerResponse:
                        case CFNetworkErrors.CFURLErrorZeroByteResource:
                        case CFNetworkErrors.CFURLErrorCannotDecodeRawData:
                        case CFNetworkErrors.CFURLErrorCannotDecodeContentData:
                        case CFNetworkErrors.CFURLErrorCannotParseResponse:
                        case CFNetworkErrors.CFURLErrorFileDoesNotExist:
                        case CFNetworkErrors.CFURLErrorFileIsDirectory:
                        case CFNetworkErrors.CFURLErrorNoPermissionsToReadFile:
                        case CFNetworkErrors.CFURLErrorCannotLoadFromNetwork:
                        case CFNetworkErrors.CFURLErrorCannotCreateFile:
                        case CFNetworkErrors.CFURLErrorCannotOpenFile:
                        case CFNetworkErrors.CFURLErrorCannotCloseFile:
                        case CFNetworkErrors.CFURLErrorCannotWriteToFile:
                        case CFNetworkErrors.CFURLErrorCannotRemoveFile:
                        case CFNetworkErrors.CFURLErrorCannotMoveFile:
                        case CFNetworkErrors.CFURLErrorDownloadDecodingFailedMidStream:
                        case CFNetworkErrors.CFURLErrorDownloadDecodingFailedToComplete:
                        case CFNetworkErrors.CFHTTPCookieCannotParseCookieFile:
                        case CFNetworkErrors.CFNetServiceErrorUnknown:
                        case CFNetworkErrors.CFNetServiceErrorCollision:
                        case CFNetworkErrors.CFNetServiceErrorNotFound:
                        case CFNetworkErrors.CFNetServiceErrorInProgress:
                        case CFNetworkErrors.CFNetServiceErrorBadArgument:
                        case CFNetworkErrors.CFNetServiceErrorInvalid:
                            webExceptionStatus = WebExceptionStatus.ReceiveFailure;
                            break;
                        case CFNetworkErrors.CFURLErrorServerCertificateHasBadDate:
                        case CFNetworkErrors.CFURLErrorServerCertificateUntrusted:
                        case CFNetworkErrors.CFURLErrorServerCertificateHasUnknownRoot:
                        case CFNetworkErrors.CFURLErrorServerCertificateNotYetValid:
                        case CFNetworkErrors.CFURLErrorClientCertificateRejected:
                        case CFNetworkErrors.CFURLErrorClientCertificateRequired:
                            webExceptionStatus = WebExceptionStatus.TrustFailure;
                            break;
                        case CFNetworkErrors.CFURLErrorSecureConnectionFailed:
                            webExceptionStatus = WebExceptionStatus.SecureChannelFailure;
                            break;
                        case CFNetworkErrors.CFErrorHttpProxyConnectionFailure:
                        case CFNetworkErrors.CFErrorHttpBadProxyCredentials:
                        case CFNetworkErrors.CFErrorPACFileError:
                        case CFNetworkErrors.CFErrorPACFileAuth:
                        case CFNetworkErrors.CFErrorHttpsProxyConnectionFailure:
                        case CFNetworkErrors.CFStreamErrorHttpsProxyFailureUnexpectedResponseToConnectMethod:
                            webExceptionStatus = WebExceptionStatus.RequestProhibitedByProxy;
                            break;
                    }

                    goto done;
                }

            done:

                // Always create a WebException so that it can be handled by the client.
                ret = new WebException(error.LocalizedDescription, innerException, webExceptionStatus, response: null);
                return ret;
            }
        }
    }

    class InflightOperation
    {
        public HttpRequestMessage Request { get; set; }
        public TaskCompletionSource<HttpResponseMessage> FutureResponse { get; set; }
        public ProgressDelegate Progress { get; set; }
        public ByteArrayListStream ResponseBody { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public bool IsCompleted { get; set; }
    }

    class ByteArrayListStream : Stream
    {
        Exception exception;
        IDisposable lockRelease;
        readonly AsyncLock readStreamLock;
        readonly List<byte[]> bytes = new List<byte[]>();

        bool isCompleted;
        long maxLength = 0;
        long position = 0;
        int offsetInCurrentBuffer = 0;

        public ByteArrayListStream()
        {
            // Initially we have nothing to read so Reads should be parked
            readStreamLock = AsyncLock.CreateLocked(out lockRelease);
        }

        public override bool CanRead { get { return true; } }
        public override bool CanWrite { get { return false; } }
        public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }
        public override void WriteByte(byte value) { throw new NotSupportedException(); }
        public override bool CanSeek { get { return false; } }
        public override bool CanTimeout { get { return false; } }
        public override void SetLength(long value) { throw new NotSupportedException(); }
        public override void Flush() { }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get { return position; }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override long Length
        {
            get
            {
                return maxLength;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.ReadAsync(buffer, offset, count).Result;
        }

        /* OMG THIS CODE IS COMPLICATED
         *
         * Here's the core idea. We want to create a ReadAsync function that
         * reads from our list of byte arrays **until it gets to the end of
         * our current list**.
         *
         * If we're not there yet, we keep returning data, serializing access
         * to the underlying position pointer (i.e. we definitely don't want
         * people concurrently moving position along). If we try to read past
         * the end, we return the section of data we could read and complete
         * it.
         *
         * Here's where the tricky part comes in. If we're not Completed (i.e.
         * the caller still wants to add more byte arrays in the future) and
         * we're at the end of the current stream, we want to *block* the read
         * (not blocking, but async blocking whatever you know what I mean),
         * until somebody adds another byte[] to chew through, or if someone
         * rewinds the position.
         *
         * If we *are* completed, we should return zero to simply complete the
         * read, signalling we're at the end of the stream */
        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
        retry:
            int bytesRead = 0;
            int buffersToRemove = 0;

            if (isCompleted && position == maxLength)
            {
                return 0;
            }

            if (exception != null) throw exception;

            using (await readStreamLock.LockAsync().ConfigureAwait(false))
            {
                lock (bytes)
                {
                    foreach (var buf in bytes)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        if (exception != null) throw exception;

                        int toCopy = Math.Min(count, buf.Length - offsetInCurrentBuffer);
                        Array.ConstrainedCopy(buf, offsetInCurrentBuffer, buffer, offset, toCopy);

                        count -= toCopy;
                        offset += toCopy;
                        bytesRead += toCopy;

                        offsetInCurrentBuffer += toCopy;

                        if (offsetInCurrentBuffer >= buf.Length)
                        {
                            offsetInCurrentBuffer = 0;
                            buffersToRemove++;
                        }

                        if (count <= 0) break;
                    }

                    // Remove buffers that we read in this operation
                    bytes.RemoveRange(0, buffersToRemove);

                    position += bytesRead;
                }
            }

            // If we're at the end of the stream and it's not done, prepare
            // the next read to park itself unless AddByteArray or Complete 
            // posts
            if (position >= maxLength && !isCompleted)
            {
                lockRelease = await readStreamLock.LockAsync().ConfigureAwait(false);
            }

            if (bytesRead == 0 && !isCompleted)
            {
                // NB: There are certain race conditions where we somehow acquire
                // the lock yet are at the end of the stream, and we're not completed
                // yet. We should try again so that we can get stuck in the lock.
                goto retry;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                Interlocked.Exchange(ref lockRelease, EmptyDisposable.Instance).Dispose();
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (exception != null)
            {
                Interlocked.Exchange(ref lockRelease, EmptyDisposable.Instance).Dispose();
                throw exception;
            }

            if (isCompleted && position < maxLength)
            {
                // NB: This solves a rare deadlock 
                //
                // 1. ReadAsync called (waiting for lock release)
                // 2. AddByteArray called (release lock)
                // 3. AddByteArray called (release lock)
                // 4. Complete called (release lock the last time)
                // 5. ReadAsync called (lock released at this point, the method completed successfully) 
                // 6. ReadAsync called (deadlock on LockAsync(), because the lock is block, and there is no way to release it)
                // 
                // Current condition forces the lock to be released in the end of 5th point

                Interlocked.Exchange(ref lockRelease, EmptyDisposable.Instance).Dispose();
            }

            return bytesRead;
        }

        public void AddByteArray(byte[] arrayToAdd)
        {
            if (exception != null) throw exception;
            if (isCompleted) throw new InvalidOperationException("Can't add byte arrays once Complete() is called");

            lock (bytes)
            {
                maxLength += arrayToAdd.Length;
                bytes.Add(arrayToAdd);
                //Console.WriteLine("Added a new byte array, {0}: max = {1}", arrayToAdd.Length, maxLength);
            }

            Interlocked.Exchange(ref lockRelease, EmptyDisposable.Instance).Dispose();
        }

        public void Complete()
        {
            isCompleted = true;
            Interlocked.Exchange(ref lockRelease, EmptyDisposable.Instance).Dispose();
        }

        public void SetException(Exception ex)
        {
            exception = ex;
            Complete();
        }
    }

    sealed class CancellableStreamContent : ProgressStreamContent
    {
        Action onDispose;

        public CancellableStreamContent(Stream source, Action onDispose) : base(source, CancellationToken.None)
        {
            this.onDispose = onDispose;
        }

        protected override void Dispose(bool disposing)
        {
            Interlocked.Exchange(ref onDispose, null)?.Invoke();

            // EVIL HAX: We have to let at least one ReadAsync of the underlying
            // stream fail with OperationCancelledException before we can dispose
            // the base, or else the exception coming out of the ReadAsync will
            // be an ObjectDisposedException from an internal MemoryStream. This isn't
            // the Ideal way to fix this, but #yolo.
            Task.Run(() => base.Dispose(disposing));
        }
    }

    sealed class EmptyDisposable : IDisposable
    {
        static readonly IDisposable instance = new EmptyDisposable();
        public static IDisposable Instance { get { return instance; } }

        EmptyDisposable() { }
        public void Dispose() { }
    }

    public sealed class AsyncLock
    {
        readonly SemaphoreSlim m_semaphore;
        readonly Task<IDisposable> m_releaser;

        public static AsyncLock CreateLocked(out IDisposable releaser)
        {
            var asyncLock = new AsyncLock(true);
            releaser = asyncLock.m_releaser.Result;
            return asyncLock;
        }

        AsyncLock(bool isLocked)
        {
            m_semaphore = new SemaphoreSlim(isLocked ? 0 : 1, 1);
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                m_releaser :
                wait.ContinueWith((_, state) => (IDisposable)state,
                    m_releaser.Result, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        }

        sealed class Releaser : IDisposable
        {
            readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }
    }

    public enum NSUrlErrorExtended
    {
        Unknown = -1,
        Cancelled = -999,
        BadURL = -1000,
        TimedOut = -1001,
        UnsupportedURL = -1002,
        CannotFindHost = -1003,
        CannotConnectToHost = -1004,
        NetworkConnectionLost = -1005,
        DNSLookupFailed = -1006,
        HTTPTooManyRedirects = -1007,
        ResourceUnavailable = -1008,
        NotConnectedToInternet = -1009,
        RedirectToNonExistentLocation = -1010,
        BadServerResponse = -1011,
        UserCancelledAuthentication = -1012,
        UserAuthenticationRequired = -1013,
        ZeroByteResource = -1014,
        CannotDecodeRawData = -1015,
        CannotDecodeContentData = -1016,
        CannotParseResponse = -1017,

        // NEW
        InternationalRoamingOff = -1018,
        CallIsActive = -1019,
        DataNotAllowed = -1020,
        RequestBodyStreamExhausted = -1021,

        // SSL errors
        SecureConnectionFailed = -1200,
        ServerCertificateHasBadDate = -1201,
        ServerCertificateUntrusted = -1202,
        ServerCertificateHasUnknownRoot = -1203,
        ServerCertificateNotYetValid = -1204,
        ClientCertificateRejected = -1205,

        // NEW
        ClientCertificateRequired = -1206,

        CannotLoadFromNetwork = -2000,

        // Downoad and file I/O errors
        CannotCreateFile = -3000,
        CannotOpenFile = -3001,
        CannotCloseFile = -3002,
        CannotWriteToFile = -3003,
        CannotRemoveFile = -3004,
        CannotMoveFile = -3005,
        DownloadDecodingFailedMidStream = -3006,
        DownloadDecodingFailedToComplete = -3007,

        FileDoesNotExist = -1100,
        FileIsDirectory = -1101,
        NoPermissionsToReadFile = -1102,
        DataLengthExceedsMaximum = -1103,

        BackgroundSessionRequiresSharedContainer = -995,
        BackgroundSessionInUseByAnotherProcess = -996,
        BackgroundSessionWasDisconnected = -997
    }

    public static class CFNetworkError
    {
        public static NSString ErrorDomain { get { return new NSString("kCFErrorDomainCFNetwork"); } }
    }

    // From Apple reference docs:
    // https://developer.apple.com/library/ios/documentation/Networking/Reference/CFNetworkErrors/#//apple_ref/c/tdef/CFNetworkErrors
    public enum CFNetworkErrors
    {
        CFHostErrorHostNotFound = 1,
        CFHostErrorUnknown = 2,

        // SOCKS errors
        CFSOCKSErrorUnknownClientVersion = 100,
        CFSOCKSErrorUnsupportedServerVersion = 101,

        // SOCKS4-specific errors
        CFSOCKS4ErrorRequestFailed = 110,
        CFSOCKS4ErrorIdentdFailed = 111,
        CFSOCKS4ErrorIdConflict = 112,
        CFSOCKS4ErrorUnknownStatusCode = 113,

        // SOCKS5-specific errors
        CFSOCKS5ErrorBadState = 120,
        CFSOCKS5ErrorBadResponseAddr = 121,
        CFSOCKS5ErrorBadCredentials = 122,
        CFSOCKS5ErrorUnsupportedNegotiationMethod = 123,
        CFSOCKS5ErrorNoAcceptableMethod = 124,

        // FTP errors
        CFFTPErrorUnexpectedStatusCode = 200,

        // HTTP errors
        CFErrorHttpAuthenticationTypeUnsupported = 300,
        CFErrorHttpBadCredentials = 301,
        CFErrorHttpConnectionLost = 302,
        CFErrorHttpParseFailure = 303,
        CFErrorHttpRedirectionLoopDetected = 304,
        CFErrorHttpBadURL = 305,
        CFErrorHttpProxyConnectionFailure = 306,
        CFErrorHttpBadProxyCredentials = 307,
        CFErrorPACFileError = 308,
        CFErrorPACFileAuth = 309,
        CFErrorHttpsProxyConnectionFailure = 310,
        CFStreamErrorHttpsProxyFailureUnexpectedResponseToConnectMethod = 311,

        // CFURL and CFURLConnection Errors
        CFURLErrorUnknown = -998,
        CFURLErrorCancelled = -999,
        CFURLErrorBadURL = -1000,
        CFURLErrorTimedOut = -1001,
        CFURLErrorUnsupportedURL = -1002,
        CFURLErrorCannotFindHost = -1003,
        CFURLErrorCannotConnectToHost = -1004,
        CFURLErrorNetworkConnectionLost = -1005,
        CFURLErrorDNSLookupFailed = -1006,
        CFURLErrorHTTPTooManyRedirects = -1007,
        CFURLErrorResourceUnavailable = -1008,
        CFURLErrorNotConnectedToInternet = -1009,
        CFURLErrorRedirectToNonExistentLocation = -1010,
        CFURLErrorBadServerResponse = -1011,
        CFURLErrorUserCancelledAuthentication = -1012,
        CFURLErrorUserAuthenticationRequired = -1013,
        CFURLErrorZeroByteResource = -1014,
        CFURLErrorCannotDecodeRawData = -1015,
        CFURLErrorCannotDecodeContentData = -1016,
        CFURLErrorCannotParseResponse = -1017,
        CFURLErrorInternationalRoamingOff = -1018,
        CFURLErrorCallIsActive = -1019,
        CFURLErrorDataNotAllowed = -1020,
        CFURLErrorRequestBodyStreamExhausted = -1021,
        CFURLErrorFileDoesNotExist = -1100,
        CFURLErrorFileIsDirectory = -1101,
        CFURLErrorNoPermissionsToReadFile = -1102,
        CFURLErrorDataLengthExceedsMaximum = -1103,

        // SSL errors
        CFURLErrorSecureConnectionFailed = -1200,
        CFURLErrorServerCertificateHasBadDate = -1201,
        CFURLErrorServerCertificateUntrusted = -1202,
        CFURLErrorServerCertificateHasUnknownRoot = -1203,
        CFURLErrorServerCertificateNotYetValid = -1204,
        CFURLErrorClientCertificateRejected = -1205,
        CFURLErrorClientCertificateRequired = -1206,

        CFURLErrorCannotLoadFromNetwork = -2000,

        // Download and file I/O errors
        CFURLErrorCannotCreateFile = -3000,
        CFURLErrorCannotOpenFile = -3001,
        CFURLErrorCannotCloseFile = -3002,
        CFURLErrorCannotWriteToFile = -3003,
        CFURLErrorCannotRemoveFile = -3004,
        CFURLErrorCannotMoveFile = -3005,
        CFURLErrorDownloadDecodingFailedMidStream = -3006,
        CFURLErrorDownloadDecodingFailedToComplete = -3007,

        // Cookie errors
        CFHTTPCookieCannotParseCookieFile = -4000,

        // Errors originating from CFNetServices
        CFNetServiceErrorUnknown = -72000,
        CFNetServiceErrorCollision = -72001,
        CFNetServiceErrorNotFound = -72002,
        CFNetServiceErrorInProgress = -72003,
        CFNetServiceErrorBadArgument = -72004,
        CFNetServiceErrorCancel = -72005,
        CFNetServiceErrorInvalid = -72006,
        CFNetServiceErrorTimeout = -72007,
        CFNetServiceErrorDNSServiceFailure = -73000,
    }
}
#endif