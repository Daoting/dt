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
using Newtonsoft.Json;
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

        /// <summary>
        /// 执行上传
        /// </summary>
        /// <param name="p_uploadFiles"></param>
        /// <param name="p_token"></param>
        /// <returns></returns>
        public new static async Task<List<string>> Handle(List<IUploadFile> p_uploadFiles, CancellationToken p_token)
        {
            if (p_uploadFiles == null || p_uploadFiles.Count == 0)
                return null;

            using (await _locker.LockAsync())
            {
                try
                {
                    return await _uploader.Upload(p_uploadFiles, p_token);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "上传异常");
                    return null;
                }
            }
        }
        #endregion

        readonly NSUrlSession _session;
        readonly List<IosUploadFile> _uploadFiles;
        string _boundary;
        CancellationToken _cancelToken;
        TaskCompletionSource<List<string>> _result;
        NSMutableData _dataResponse;

        private Uploader()
        {
            var config = NSUrlSessionConfiguration.DefaultSessionConfiguration;
            config.TLSMinimumSupportedProtocol = SslProtocol.Tls_1_2;
            config.AllowsCellularAccess = true;
            _session = NSUrlSession.FromConfiguration(config, (INSUrlSessionDelegate)this, NSOperationQueue.MainQueue);
            _uploadFiles = new List<IosUploadFile>();
        }

        async Task<List<string>> Upload(List<IUploadFile> p_uploadFiles, CancellationToken p_token)
        {
            _cancelToken = p_token;
            _boundary = AtKit.NewID;
            _dataResponse = NSMutableData.Create();

            _cancelToken.ThrowIfCancellationRequested();
            _result = new TaskCompletionSource<List<string>>();
            _cancelToken.Register(() => _result.TrySetCanceled());

            string path = await SaveToFile(p_uploadFiles);

            var request = new NSMutableUrlRequest(NSUrl.FromString($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/fsm/.u"));
            request.HttpMethod = "POST";
            request["Content-Type"] = "multipart/form-data; boundary=" + _boundary;

            var uploadTask = _session.CreateUploadTask(request, new NSUrl(path, false));
            uploadTask.Resume();

            return await _result.Task.ConfigureAwait(false);
        }

        Task<string> SaveToFile(List<IUploadFile> p_uploadFiles)
        {
            return Task.Run(() =>
            {
                _uploadFiles.Clear();
                var multiPartPath = Path.Combine(AtSys.DocPath, AtKit.NewID);
                using (var fs = new FileStream(multiPartPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    // UTF8.GetBytes的结果只一字节10，诡异！
                    byte[] line = new byte[] { 13, 10 }; // Encoding.UTF8.GetBytes("\r\n");
                    foreach (var uf in p_uploadFiles)
                    {
                        // section头
                        byte[] data = Encoding.UTF8.GetBytes(string.Format(_sectionHeader, _boundary, uf.File.Desc, uf.File.FileName));
                        fs.Write(data, 0, data.Length);

                        // 内容
                        data = File.ReadAllBytes(uf.File.FilePath);
                        fs.Write(data, 0, data.Length);

                        // 结束行
                        fs.Write(line, 0, line.Length);

                        // 含缩略图
                        if (uf.File.ThumbStream != null)
                        {
                            // section头
                            data = Encoding.UTF8.GetBytes(string.Format(_sectionHeader, _boundary, "thumbnail", "thumbnail.jpg"));
                            fs.Write(data, 0, data.Length);

                            // 内容
                            uf.File.ThumbStream.Seek(0, SeekOrigin.Begin);
                            data = new byte[uf.File.ThumbStream.Length];
                            uf.File.ThumbStream.Read(data, 0, data.Length);
                            fs.Write(data, 0, data.Length);

                            // 结束行
                            fs.Write(line, 0, line.Length);
                        }

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
                    uf.File.UploadProgress(size, size, size);
                    startPos = uf.EndPosition;
                }
                else
                {
                    // 上传部分，
                    long len = totalBytesSent - startPos;
                    uf.File.UploadProgress(len, len, size);
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

            string msg = NSString.FromData(_dataResponse, NSStringEncoding.UTF8);
            using (var sr = new StringReader(msg))
            using (var reader = new JsonTextReader(sr))
            {
                reader.Read();
                var ls = JsonRpcSerializer.Deserialize(reader) as List<string>;
                _result.SetResult(ls);
            }
        }
    }

    class IosUploadFile
    {
        public IUploadFile File { get; set; }

        public long EndPosition { get; set; }

        public bool IsCompleted { get; set; }
    }
}
#endif