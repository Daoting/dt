#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using Java.Security;
using Java.Util.Concurrent;
using Javax.Net.Ssl;
using Square.OkHttp3;
using Square.OkIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Android版文件上传
    /// </summary>
    public static class Uploader
    {
        static readonly OkHttpClient _client = new OkHttpClient();
        // 取消上传的令牌
        static CancellationTokenSource _tokenSource;

        static Uploader()
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

            var bodyBuilder = new MultipartBody.Builder().SetType(MultipartBody.Form);

            if (!string.IsNullOrEmpty(p_fixedvolume))
            {
                // 固定上传路径放在最前
                bodyBuilder.AddFormDataPart("fixedvolume", p_fixedvolume);
            }

            foreach (var uf in p_uploadFiles)
            {
                Java.IO.File file = new Java.IO.File(uf.FilePath);
                RequestBody rb = RequestBody.Create(file, MediaType.Parse("application/octet-stream"));
                // 包一层实现进度
                ProgressRequestBody progress = new ProgressRequestBody(rb, ((IUploadUI)uf.UploadUI).UploadProgress);
                bodyBuilder.AddFormDataPart(uf.Desc, uf.FileName, progress);

                // 含缩略图
                if (!string.IsNullOrEmpty(uf.ThumbPath))
                {
                    var thumbFile = new Java.IO.File(uf.ThumbPath);
                    var thumb = RequestBody.Create(thumbFile, MediaType.Parse("application/octet-stream"));
                    bodyBuilder.AddFormDataPart("thumbnail", "thumbnail.jpg", thumb);
                }
            }
            RequestBody body = bodyBuilder.Build();

            var request = new Request.Builder()
                .Post(body)
                .Url($"{Kit.Stub.ServerUrl.TrimEnd('/')}/fsm/.u");
            if (Kit.IsLogon)
                request.Header("uid", Kit.UserID.ToString());

            var call = _client.NewCall(request.Build());
            p_tokenSource.Token.Register(() => Task.Run(() => call.Cancel()));
            _tokenSource = p_tokenSource;

            Response resp;
            try
            {
                resp = await call.EnqueueAsync().ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
            finally
            {
                _tokenSource = null;
            }

            var result = resp.Body().Bytes();
            if (result == null || result.Length == 0)
                return null;
            return ParseResult(result);
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

        static List<string> ParseResult(byte[] p_data)
        {
            // Utf8JsonReader不能用在异步方法内！
            var reader = new Utf8JsonReader(p_data);
            reader.Read();
            return JsonRpcSerializer.Deserialize(ref reader) as List<string>;
        }
    }

    public class ProgressRequestBody : RequestBody
    {
        RequestBody _body;
        ProgressDelegate _progressListener;

        public ProgressRequestBody(RequestBody p_body, ProgressDelegate p_progressListener)
        {
            _body = p_body;
            _progressListener = p_progressListener;
        }

        public override MediaType ContentType()
        {
            return _body.ContentType();
        }

        public override long ContentLength()
        {
            return _body.ContentLength();
        }

        public override void WriteTo(IBufferedSink p_sink)
        {
            // 需要另一个代理类来获取写入的长度
            ExForwardingSink forwardingSink = new ExForwardingSink(p_sink, _progressListener, ContentLength());
            // 转一下
            IBufferedSink bufferedSink = OkIO.Buffer(forwardingSink);
            // 写数据
            _body.WriteTo(bufferedSink);
            // 刷新写入
            bufferedSink.Flush();
        }
    }

    public class ExForwardingSink : ForwardingSink
    {
        ProgressDelegate _progressListener;
        long _totalLength;
        long _currentLength;

        public ExForwardingSink(ISink p_sink, ProgressDelegate p_progressListener, long p_totalLength)
            : base(p_sink)
        {
            _progressListener = p_progressListener;
            _totalLength = p_totalLength;
        }

        public override void Write(OkBuffer p_source, long p_byteCount)
        {
            _currentLength += p_byteCount;
            // 回调进度
            _progressListener?.Invoke(p_byteCount, _currentLength, _totalLength);
            base.Write(p_source, p_byteCount);
        }
    }
}
#endif