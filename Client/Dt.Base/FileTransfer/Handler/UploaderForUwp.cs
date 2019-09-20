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
using Newtonsoft.Json;
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
    /// 
    /// </summary>
    public static class Uploader
    {
        static readonly AsyncLocker _locker = new AsyncLocker();
        readonly static HttpClient _client = new HttpClient(new HttpClientHandler
        {
            // 验证时服务端证书始终有效！
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });


        public static async Task<List<string>> Handle(List<IUploadFile> p_uploadFiles, CancellationToken p_token)
        {
            if (p_uploadFiles == null || p_uploadFiles.Count == 0)
                return null;

            // 排队避免服务器压力
            string result;
            using (await _locker.LockAsync())
            using (var request = CreateRequestMessage())
            using (var content = new MultipartFormDataContent())
            {
                foreach (var up in p_uploadFiles)
                {
                    if (up == null || up.File == null)
                        continue;

                    var streamContent = new ProgressStreamContent((await up.File.OpenReadAsync()).AsStream(), CancellationToken.None);
                    streamContent.Progress = up.UploadProgress;
                    content.Add(streamContent, up.File.DisplayName, up.File.Name);
                }
                request.Content = content;

                try
                {
                    using (var response = await _client.SendAsync(request, p_token).ConfigureAwait(false))
                    {
                        result = await response.Content.ReadAsStringAsync();
                    }
                }
                catch
                {
                    return null;
                }
            }

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
            get { return _locker.IsLocked; }
        }

        static HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/fsm/.u")
            };
        }
    }
}