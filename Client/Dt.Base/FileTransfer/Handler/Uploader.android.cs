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
using Newtonsoft.Json;
using Square.OkHttp3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
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

        public static async Task<List<string>> Handle(List<IUploadFile> p_uploadFiles, CancellationToken p_token)
        {
            if (p_uploadFiles == null || p_uploadFiles.Count == 0)
                return null;

            var bodyBuilder = new MultipartBody.Builder().SetType(MultipartBody.Form);
            foreach (var uf in p_uploadFiles)
            {
                Java.IO.File file = new Java.IO.File(uf.File.Path);
                RequestBody rb = RequestBody.Create(MediaType.Parse("application/octet-stream"), file);
                bodyBuilder.AddFormDataPart(uf.File.FileType, uf.File.Name, rb);
            }
            RequestBody body = bodyBuilder.Build();

            var request = new Request.Builder()
                .Post(body)
                .Url($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/fsm/.u")
                .AddHeader("uid", AtUser.ID)
                .Build();
            var call = _client.NewCall(request);
            p_token.Register(() => Task.Run(() => call.Cancel()));

            Response resp;
            try
            {
                resp = await call.EnqueueAsync().ConfigureAwait(false);
            }
            catch
            {
                return null;
            }

            string result = resp.Body().String();
            if (string.IsNullOrEmpty(result))
                return null;

            using (var sr = new StringReader(result))
            using (var reader = new JsonTextReader(sr))
            {
                reader.Read();
                return JsonRpcSerializer.Deserialize(reader) as List<string>;
            }
        }

        /// <summary>
        /// 获取当前是否已锁定文件传输
        /// </summary>
        public static bool IsLocked
        {
            get { return false; }
        }
    }
}
#endif