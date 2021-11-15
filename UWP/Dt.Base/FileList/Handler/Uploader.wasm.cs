#if WASM
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
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using System.Text;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// UWP版文件上传
    /// </summary>
    public static class Uploader
    {
        static readonly AsyncLocker _locker = new AsyncLocker();
        readonly static HttpClient _client = new HttpClient(new HttpClientHandler
        {
            // 验证时服务端证书始终有效！
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
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

            // 排队避免服务器压力
            byte[] result;
            using (await _locker.LockAsync())
            using (var request = CreateRequestMessage())
            using (var content = new MultipartFormDataContent())
            {
                if (!string.IsNullOrEmpty(p_fixedvolume))
                {
                    // 固定上传路径放在最前
                    content.Add(new StringContent(p_fixedvolume, Encoding.UTF8), "fixedvolume");
                }

                foreach (var file in p_uploadFiles)
                {
                    // 带进度的流内容
                    var streamContent = new ProgressStreamContent(await file.GetStream(), CancellationToken.None);
                    streamContent.Progress = ((IUploadUI)file.UploadUI).UploadProgress;
                    content.Add(streamContent, file.Desc, file.FileName);

                    // 含缩略图
                    if (!string.IsNullOrEmpty(file.ThumbPath))
                    {
                        var sf = await StorageFile.GetFileFromPathAsync(file.ThumbPath);
                        var thumb = new StreamContent(await sf.OpenStreamForReadAsync());
                        content.Add(thumb, "thumbnail", "thumbnail.jpg");
                    }
                }
                request.Content = content;
                _tokenSource = p_tokenSource;

                try
                {
                    using (var response = await _client.SendAsync(request, p_tokenSource.Token).ConfigureAwait(false))
                    {
                        result = await response.Content.ReadAsByteArrayAsync();
                        // response.StatusCode：412 表示不存在固定卷；415 无Boundary
                    }
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

        static HttpRequestMessage CreateRequestMessage()
        {
            // 使用http2协议Post方法
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"{Kit.Stub.ServerUrl.TrimEnd('/')}/fsm/.u")
            };

            if (Kit.IsLogon)
                req.Headers.Add("uid", Kit.UserID.ToString());
            return req;
        }
    }
}
#endif