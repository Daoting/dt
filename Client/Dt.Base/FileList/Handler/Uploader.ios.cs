#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Foundation;
using System.Text.Json;
using Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// iOS版文件上传
    /// </summary>
    public class Uploader : NSUrlSessionDataDelegate, INSUrlSessionDelegate
    {
        #region 静态内容
        static readonly Uploader _uploader = new Uploader();
        static readonly AsyncLocker _locker = new AsyncLocker();
        const string _sectionHeader = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
        // 取消上传的令牌
        static CancellationTokenSource _tokenSource;

        /// <summary>
        /// 执行上传
        /// </summary>
        /// <param name="p_uploadFiles">待上传文件</param>
        /// <param name="p_fixedvolume">要上传的固定卷名，null表示上传到普通卷</param>
        /// <param name="p_tokenSource">取消上传的令牌，不负责释放</param>
        /// <returns></returns>
        public static async Task<List<string>> Send(IList<FileData> p_uploadFiles, string p_fixedvolume, CancellationTokenSource p_tokenSource)
        {
            // 列表内容不可为null
            if (p_uploadFiles == null
                || p_uploadFiles.Count == 0
                || p_uploadFiles.Contains(null))
                return null;

            using (await _locker.LockAsync())
            {
                try
                {
                    _tokenSource = p_tokenSource;
                    return await _uploader.Upload(p_uploadFiles, p_fixedvolume, p_tokenSource.Token);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "上传异常");
                    return null;
                }
                finally
                {
                    _tokenSource = null;
                }
            }
        }

        /// <summary>
        /// 取消上传
        /// </summary>
        internal static void Cancel()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
                _tokenSource = null;
            }
        }
        #endregion

        readonly NSUrlSession _session;
        readonly List<IosUploadFile> _uploadFiles;
        string _boundary;
        CancellationToken _cancelToken;
        TaskCompletionSource<List<string>> _result;
        NSMutableData _dataResponse;
        string _tempFile;

        Uploader()
        {
            var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            config.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_2;
            config.AllowsCellularAccess = true;
            _session = NSUrlSession.FromConfiguration(config, (INSUrlSessionDelegate)this, NSOperationQueue.MainQueue);
            _uploadFiles = new List<IosUploadFile>();
        }

        async Task<List<string>> Upload(IList<FileData> p_uploadFiles, string p_fixedvolume, CancellationToken p_token)
        {
            _cancelToken = p_token;
            _boundary = Kit.NewGuid;
            _dataResponse = NSMutableData.Create();

            _cancelToken.ThrowIfCancellationRequested();
            _result = new TaskCompletionSource<List<string>>();
            _cancelToken.Register(() => _result.TrySetCanceled());

            _tempFile = await SaveToFile(p_uploadFiles, p_fixedvolume);

            var request = new NSMutableUrlRequest(NSUrl.FromString($"{Kit.Stub.ServerUrl.TrimEnd('/')}/fsm/.u"));
            request.HttpMethod = "POST";
            request["Content-Type"] = "multipart/form-data; boundary=" + _boundary;
            if (Kit.IsLogon)
                request.Headers["uid"] = (NSString)Kit.UserID.ToString();

            var uploadTask = _session.CreateUploadTask(request, new NSUrl(_tempFile, false));
            uploadTask.Resume();

            return await _result.Task.ConfigureAwait(false);
        }

        /// <summary>
        /// 将所有要上传的文件按照 multipart/form-data 格式合并成一个文件，上传结束时删除
        /// </summary>
        /// <param name="p_uploadFiles">待上传文件</param>
        /// <param name="p_fixedvolume">要上传的固定卷名，null表示上传到普通卷</param>
        /// <returns></returns>
        Task<string> SaveToFile(IList<FileData> p_uploadFiles, string p_fixedvolume)
        {
            return Task.Run(() =>
            {
                _uploadFiles.Clear();
                var multiPartPath = Path.Combine(Kit.CachePath, Kit.NewGuid);
                using (var fs = new FileStream(multiPartPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    // UTF8.GetBytes的结果只一字节10，诡异！
                    byte[] line = new byte[] { 13, 10 }; // Encoding.UTF8.GetBytes("\r\n");

                    if (!string.IsNullOrEmpty(p_fixedvolume))
                    {
                        // 固定上传路径放在最前
                        byte[] data = Encoding.UTF8.GetBytes($"--{_boundary}\r\nContent-Disposition: form-data; name=\"fixedvolume\"\r\n\r\n{p_fixedvolume}\r\n");
                        fs.Write(data, 0, data.Length);
                    }

                    foreach (var uf in p_uploadFiles)
                    {
                        // section头
                        byte[] data = Encoding.UTF8.GetBytes(string.Format(_sectionHeader, _boundary, uf.Desc, uf.FileName));
                        fs.Write(data, 0, data.Length);

                        // 内容
                        data = File.ReadAllBytes(uf.FilePath);
                        fs.Write(data, 0, data.Length);

                        // 结束行
                        fs.Write(line, 0, line.Length);

                        // 含缩略图
                        if (!string.IsNullOrEmpty(uf.ThumbPath))
                        {
                            // section头
                            data = Encoding.UTF8.GetBytes(string.Format(_sectionHeader, _boundary, "thumbnail", "thumbnail.jpg"));
                            fs.Write(data, 0, data.Length);

                            // 内容
                            data = File.ReadAllBytes(uf.ThumbPath);
                            fs.Write(data, 0, data.Length);

                            // 结束行
                            fs.Write(line, 0, line.Length);
                        }

                        // 记录位置为上传进度用
                        _uploadFiles.Add(new IosUploadFile { File = uf, EndPosition = fs.Position, });
                    }

                    // 结束标志
                    var end = Encoding.UTF8.GetBytes($"\r\n--{_boundary}--\r\n");
                    fs.Write(end, 0, end.Length);
                    fs.Flush();
                }
                return multiPartPath;
            });
        }

        /* 以下重写方法按调用次序 */

        public override void DidSendBodyData(NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
        {
            // 上传进度，已合并成一个文件，上传过程中可能出现跨文件完毕
            long startPos = 0;
            foreach (var uf in _uploadFiles)
            {
                if (uf.IsCompleted)
                {
                    startPos = uf.EndPosition;
                    continue;
                }

                // 未开始
                if (startPos > totalBytesSent)
                    break;

                // 当前文件大小
                long size = uf.EndPosition - startPos;
                if (uf.EndPosition <= totalBytesSent)
                {
                    // 上传完毕
                    uf.IsCompleted = true;
                    // 可能会跨文件
                    ((IUploadUI)uf.File.UploadUI)?.UploadProgress?.Invoke(size, size, size);
                    startPos = uf.EndPosition;
                }
                else
                {
                    // 上传部分，
                    long len = totalBytesSent - startPos;
                    ((IUploadUI)uf.File.UploadUI)?.UploadProgress?.Invoke(len, len, size);
                    break;
                }
            }
        }

        public override void DidReceiveChallenge(NSUrlSession session, NSUrlSessionTask task, NSUrlAuthenticationChallenge challenge, Action<NSUrlSessionAuthChallengeDisposition, NSUrlCredential> completionHandler)
        {
            // 信任所有服务器证书，支持自签名证书
            if (challenge.ProtectionSpace.AuthenticationMethod == NSUrlProtectionSpace.AuthenticationMethodServerTrust)
                completionHandler(NSUrlSessionAuthChallengeDisposition.UseCredential, NSUrlCredential.FromTrust(challenge.ProtectionSpace.ServerSecTrust));
            else
                completionHandler(NSUrlSessionAuthChallengeDisposition.PerformDefaultHandling, challenge.ProposedCredential);
        }

        public override void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
        {
            try
            {
                if (_cancelToken.IsCancellationRequested)
                    dataTask.Cancel();
            }
            catch (Exception ex)
            {
                _result.TrySetException(ex);
            }
            completionHandler(NSUrlSessionResponseDisposition.Allow);
        }

        public override void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData byteData)
        {
            // 接收时可能调用多次
            _dataResponse.AppendData(byteData);
        }

        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            // 删除临时文件
            if (File.Exists(_tempFile))
            {
                try
                {
                    File.Delete(_tempFile);
                }
                catch { }
            }

            var resp = task.Response as NSHttpUrlResponse;
            if (error != null
                || resp == null
                || (resp.StatusCode != 200 && resp.StatusCode != 201))
            {
                // 暂时简化错误提示
                string ex = $"{(resp == null ? "" : "状态码：" + resp.StatusCode.ToString() + "\r\n")}{(error == null ? "" : error.Description)}";
                _result.TrySetException(new Exception(ex));
                return;
            }

            var data = _dataResponse.ToArray();
            if (data != null && data.Length > 0)
            {
                var reader = new Utf8JsonReader(data);
                reader.Read();
                var ls = JsonRpcSerializer.Deserialize(ref reader) as List<string>;
                _result.SetResult(ls);
            }
            else
            {
                _result.SetResult(null);
            }
        }
    }

    class IosUploadFile
    {
        public FileData File { get; set; }

        public long EndPosition { get; set; }

        public bool IsCompleted { get; set; }
    }
}
#endif