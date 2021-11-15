#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-08-21 创建
******************************************************************************/
#endregion

#region 引用命名
using Java.IO;
using Java.Security;
using Java.Util.Concurrent;
using Javax.Net.Ssl;
using Square.OkHttp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Core.Rpc
{
    /// <summary>
    /// android中使用OkHttp3实现Http2通信
    /// 参考：https://github.com/alexrainman/ModernHttpClient
    /// </summary>
    public class NativeMessageHandler : HttpClientHandler
    {
        OkHttpClient _client = new OkHttpClient();
        readonly Dictionary<HttpRequestMessage, WeakReference> _registeredProgressCallbacks = new Dictionary<HttpRequestMessage, WeakReference>();
        readonly CacheControl _noCacheCacheControl = new CacheControl.Builder().NoCache().Build();

        public NativeMessageHandler()
        {
            var clientBuilder = _client.NewBuilder();

            // tls
            var tlsSpecBuilder = new ConnectionSpec.Builder(ConnectionSpec.ModernTls).TlsVersions(new[] { TlsVersion.Tls12, TlsVersion.Tls13 });
            var tlsSpec = tlsSpecBuilder.Build();
            var specs = new List<ConnectionSpec>() { tlsSpec, ConnectionSpec.Cleartext };
            clientBuilder.ConnectionSpecs(specs);

            // 始终有Http11避免PROTOCOL_ERROR
            clientBuilder.Protocols(new[] { Protocol.Http11, Protocol.Http2 });

            // 信任所有服务器证书，支持自签名证书
            var sslContext = SSLContext.GetInstance("TLS");
            var trustManager = new CustomX509TrustManager();
            sslContext.Init(null, new ITrustManager[] { trustManager }, new SecureRandom());
            // Create an ssl socket factory with our all-trusting manager
            var sslSocketFactory = sslContext.SocketFactory;
            clientBuilder.SslSocketFactory(sslSocketFactory, trustManager);

            // 读始终不超时，配合服务器推送
            clientBuilder.ReadTimeout(0, TimeUnit.Milliseconds);
            clientBuilder.WriteTimeout(0, TimeUnit.Milliseconds);
            clientBuilder.CallTimeout(0, TimeUnit.Milliseconds);

            // Hostname始终有效
            clientBuilder.HostnameVerifier((name, ssl) => true);
            _client = clientBuilder.Build();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var java_uri = request.RequestUri.GetComponents(UriComponents.AbsoluteUri, UriFormat.UriEscaped);
            var url = new Java.Net.URL(java_uri);

            var body = default(RequestBody);
            if (request.Content != null)
            {
                var bytes = await request.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                var contentType = "text/plain";
                if (request.Content.Headers.ContentType != null)
                {
                    contentType = string.Join(" ", request.Content.Headers.GetValues("Content-Type"));
                }
                body = RequestBody.Create(bytes, MediaType.Parse(contentType));
            }

            var requestBuilder = new Request.Builder()
                .Method(request.Method.Method.ToUpperInvariant(), body)
                .Url(url);
            requestBuilder.CacheControl(_noCacheCacheControl);

            foreach (var kvp in request.Headers)
            {
                requestBuilder.AddHeader(kvp.Key, String.Join(",", kvp.Value));
            }

            cancellationToken.ThrowIfCancellationRequested();

            var rq = requestBuilder.Build();
            var call = _client.NewCall(rq);

            // NB: Even closing a socket must be done off the UI thread. Cray!
            cancellationToken.Register(() => Task.Run(() => call.Cancel()));

            var resp = default(Response);
            try
            {
                resp = await call.EnqueueAsync().ConfigureAwait(false);

                // 重定向
                //var newReq = resp.Request();
                //var newUri = newReq == null ? null : newReq.Url().Uri();
                //request.RequestUri = new Uri(newUri.ToString());
                //if (throwOnCaptiveNetwork && newUri != null)
                //{
                //    if (url.Host != newUri.Host)
                //    {
                //        throw new CaptiveNetworkException(new Uri(java_uri), new Uri(newUri.ToString()));
                //    }
                //}
            }
            catch (IOException ex)
            {
                if (ex.Message.ToLowerInvariant().Contains("canceled"))
                {
                    throw new System.OperationCanceledException();
                }

                // Calling HttpClient methods should throw .Net Exception when fail #5
                throw new HttpRequestException(ex.Message, ex);
            }

            var respBody = resp.Body();

            cancellationToken.ThrowIfCancellationRequested();

            var ret = new HttpResponseMessage((HttpStatusCode)resp.Code());
            ret.RequestMessage = request;
            ret.ReasonPhrase = resp.Message();

            // ReasonPhrase is empty under HTTPS #8
            if (string.IsNullOrEmpty(ret.ReasonPhrase))
            {
                try
                {
                    ret.ReasonPhrase = ((ReasonPhrases)resp.Code()).ToString().Replace('_', ' ');
                }
#pragma warning disable 0168
                catch (Exception ex)
                {
                    ret.ReasonPhrase = "Unassigned";
                }
#pragma warning restore 0168
            }

            if (respBody != null)
            {
                var content = new ProgressStreamContent(respBody.ByteStream(), CancellationToken.None);
                content.Progress = GetAndRemoveCallbackFromRegister(request);
                ret.Content = content;
            }
            else
            {
                ret.Content = new ByteArrayContent(new byte[0]);
            }

            // 响应头
            var respHeaders = resp.Headers();
            foreach (var k in respHeaders.Names())
            {
                ret.Headers.TryAddWithoutValidation(k, respHeaders.Get(k));
                ret.Content.Headers.TryAddWithoutValidation(k, respHeaders.Get(k));
            }

            return ret;
        }

        public void RegisterForProgress(HttpRequestMessage request, ProgressDelegate callback)
        {
            if (callback == null && _registeredProgressCallbacks.ContainsKey(request))
            {
                _registeredProgressCallbacks.Remove(request);
                return;
            }

            _registeredProgressCallbacks[request] = new WeakReference(callback);
        }

        ProgressDelegate GetAndRemoveCallbackFromRegister(HttpRequestMessage request)
        {
            ProgressDelegate emptyDelegate = delegate { };

            lock (_registeredProgressCallbacks)
            {
                if (!_registeredProgressCallbacks.ContainsKey(request)) return emptyDelegate;

                var weakRef = _registeredProgressCallbacks[request];
                if (weakRef == null) return emptyDelegate;

                var callback = weakRef.Target as ProgressDelegate;
                if (callback == null) return emptyDelegate;

                _registeredProgressCallbacks.Remove(request);
                return callback;
            }
        }
    }

    public static class AwaitableOkHttp
    {
        public static Task<Response> EnqueueAsync(this ICall This)
        {
            var cb = new OkTaskCallback();
            This.Enqueue(cb);
            return cb.Task;
        }

        class OkTaskCallback : Java.Lang.Object, Square.OkHttp3.ICallback
        {
            readonly TaskCompletionSource<Response> tcs = new TaskCompletionSource<Response>();
            public Task<Response> Task { get { return tcs.Task; } }

            public void OnFailure(ICall p0, IOException p1)
            {
                System.Diagnostics.Debug.WriteLine(p1.Message);
                tcs.TrySetException(p1);
            }

            public void OnResponse(ICall p0, Response p1)
            {
                tcs.TrySetResult(p1);
            }
        }
    }

    public enum ReasonPhrases
    {
        // 1xx: Informational - Request received, continuing process

        Continue = 100,
        Switching_Protocols = 101,
        Processing = 102,
        Early_Hints = 103,

        // 104-199   Unassigned

        // 2xx: Success - The action was successfully received, understood, and accepted

        OK = 200,
        Created = 201,
        Accepted = 202,
        Non_Authoritative_Information = 203,
        No_Content = 204,
        Reset_Content = 205,
        Partial_Content = 206,
        Multi_Status = 207,
        Already_Reported = 208,

        // 209-225 Unassigned

        IM_Used = 226,

        // 227-299   Unassigned

        // 3xx: Redirection - Further action must be taken in order to complete the request

        Multiple_Choices = 300,
        Moved_Permanently = 301,
        Found = 302,
        See_Other = 303,
        Not_Modified = 304,
        Use_Proxy = 305,
        Unused = 306,
        Temporary_Redirect = 307,
        Permanent_Redirect = 308,

        // 309-399  Unassigned

        // 4xx: Client Error - The request contains bad syntax or cannot be fulfilled

        Bad_Request = 400,
        Unauthorized = 401,
        Payment_Required = 402,
        Forbidden = 403,
        Not_Found = 404,
        Method_Not_Allowed = 405,
        Not_Acceptable = 406,
        Proxy_Authentication_Required = 407,
        Request_Timeout = 408,
        Conflict = 409,
        Gone = 410,
        Length_Required = 411,
        Precondition_Failed = 412,
        Payload_Too_Large = 413,
        URI_Too_Long = 414,
        Unsupported_Media_Type = 415,
        Range_Not_Satisfiable = 416,
        Expectation_Failed = 417,

        // 418-420  Unassigned

        Misdirected_Request = 421,
        Unprocessable_Entity = 422,
        Locked = 423,
        Failed_Dependency = 424,
        //Unassigned = 425,
        Upgrade_Required = 426,
        //Unassigned = 427,
        Precondition_Required = 428,
        Too_Many_Requests = 429,
        //Unassigned = 430,
        Request_Header_Fields_Too_Large = 431,

        // 432-450 Unassigned

        Unavailable_For_Legal_Reasons = 451,

        // 452-499 Unassigned

        Internal_Server_Error = 500,
        Not_Implemented = 501,
        Bad_Gateway = 502,
        Service_Unavailable = 503,
        Gateway_Timeout = 504,
        HTTP_Version_Not_Supported = 505,
        Variant_Also_Negotiates = 506,
        Insufficient_Storage = 507,
        Loop_Detected = 508,
        //Unassigned  509
        Not_Extended = 510,
        Network_Authentication_Required = 511

        // 512-599  Unassigned
    }

    class CustomX509TrustManager : Java.Lang.Object, IX509TrustManager
    {
        public void CheckClientTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
        {
        }

        public void CheckServerTrusted(Java.Security.Cert.X509Certificate[] chain, string authType)
        {
        }

        Java.Security.Cert.X509Certificate[] IX509TrustManager.GetAcceptedIssuers()
        {
            return new Java.Security.Cert.X509Certificate[0];
        }
    }
}
#endif