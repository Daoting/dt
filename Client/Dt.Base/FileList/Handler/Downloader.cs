﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
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
    /// 文件下载，三平台合一
    /// </summary>
    public static class Downloader
    {
        static readonly AsyncLocker _locker = new AsyncLocker();

#if UWP
        readonly static HttpClient _client = new HttpClient(new HttpClientHandler
        {
            // 验证时服务端证书始终有效！
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        });
#else
        readonly static HttpClient _client = new HttpClient(new NativeMessageHandler());
#endif

        /// <summary>
        /// 执行下载
        /// </summary>
        /// <param name="p_info">下载描述</param>
        /// <param name="p_token"></param>
        /// <returns>是否成功</returns>
        public static async Task<bool> Handle(DownloadInfo p_info, CancellationToken p_token)
        {
            if (p_info == null || string.IsNullOrEmpty(p_info.Path) || p_info.TgtStream == null)
                return false;

            HttpResponseMessage response = null;
            using (await _locker.LockAsync())
            using (var request = CreateRequestMessage(p_info.Path))
            {
                try
                {
                    response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, p_token).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    p_info.Error = "已取消下载！";
                    return false;
                }
                catch (Exception ex)
                {
                    p_info.Error = "😢下载过程中出错！" + ex.Message;
                    return false;
                }
            }

            // 下载失败
            if (response.Headers.TryGetValues("error", out var vals))
            {
                p_info.Error = WebUtility.UrlDecode(vals.First());
                return false;
            }

            // 文件长度
            long total;
            if (!response.Content.Headers.TryGetValues("Content-Length", out var lgh) || !long.TryParse(lgh.First(), out total))
            {
                p_info.Error = "😢待下载的文件长度未知，下载失败！";
                return false;
            }

            try
            {
                using (var inputStream = await response.Content.ReadAsStreamAsync())
                {
                    int read;
                    int readTotal = 0;
                    byte[] data = new byte[81920];
                    while ((read = await inputStream.ReadAsync(data, 0, data.Length, p_token).ConfigureAwait(false)) > 0)
                    {
                        await p_info.TgtStream.WriteAsync(data, 0, read);
                        readTotal += read;
                        p_info.Progress?.Invoke(read, readTotal, total);
                    }
                    await p_info.TgtStream.FlushAsync();
                }
            }
            catch (TaskCanceledException)
            {
                p_info.Error = "已取消下载！";
                return false;
            }
            catch (Exception ex)
            {
                p_info.Error = "😢下载过程中出错！" + ex.Message;
                return false;
            }
            return true;
        }

        static HttpRequestMessage CreateRequestMessage(string p_act)
        {
            // 使用http2协议Post方法，路径相同连续 Get 时无效！
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"{AtSys.Stub.ServerUrl.TrimEnd('/')}/fsm/.d/{p_act}"),
                // 无内容时 okhttp 异常
                Content = new StringContent("a")
            };
        }
    }
}