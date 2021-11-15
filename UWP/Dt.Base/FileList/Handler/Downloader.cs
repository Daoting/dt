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
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
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
#elif WASM
        readonly static HttpClient _client = new HttpClient();
#else
        readonly static HttpClient _client = new HttpClient(new NativeMessageHandler());
#endif

        /// <summary>
        /// 下载文件，将下载内容写入目标流
        /// </summary>
        /// <param name="p_info">下载描述</param>
        /// <param name="p_token"></param>
        /// <returns>是否成功</returns>
        public static async Task<bool> GetFile(DownloadInfo p_info, CancellationToken p_token)
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

        /// <summary>
        /// 下载文件并缓存到本地
        /// </summary>
        /// <param name="p_path">要下载的文件路径，以原有文件名缓存到本地</param>
        /// <returns>false 下载失败，缓存文件已删除</returns>
        public static async Task<bool> GetAndCacheFile(string p_path)
        {
            if (string.IsNullOrEmpty(p_path))
                return false;

            // 路径肯定含/
            int index = p_path.LastIndexOf('/');
            if (index <= 0)
                return false;

            string path = Path.Combine(Kit.CachePath, p_path.Substring(index + 1));
            FileStream stream = File.Create(path);
            DownloadInfo info = new DownloadInfo
            {
                Path = p_path,
                TgtStream = stream,
            };

            bool suc = false;
            try
            {
                suc = await GetFile(info, CancellationToken.None);
            }
            finally
            {
                stream.Close();
            }

            if (!suc)
            {
                // 未成功，删除缓存文件，避免打开时出错
                try
                {
                    // mono中 FileInfo 的 Exists 状态不同步！
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                catch { }
            }
            return suc;
        }

        /// <summary>
        /// 下载图片，不在本地缓存文件，直接返回BitmapImage对象
        /// </summary>
        /// <param name="p_path">要下载的文件路径</param>
        /// <returns></returns>
        public static async Task<BitmapImage> GetImage(string p_path)
        {
            if (string.IsNullOrEmpty(p_path))
                return null;

            // 路径肯定含/
            int index = p_path.LastIndexOf('/');
            if (index <= 0)
                return null;

            MemoryStream stream = new MemoryStream();
            DownloadInfo info = new DownloadInfo
            {
                Path = p_path,
                TgtStream = stream,
            };

            bool suc = false;
            try
            {
                suc = await GetFile(info, CancellationToken.None);
                if (suc)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    BitmapImage bmp = new BitmapImage();
#if UWP
                    var randomStream = new InMemoryRandomAccessStream();
                    var outputStream = randomStream.GetOutputStreamAt(0);
                    await RandomAccessStream.CopyAsync(stream.AsInputStream(), outputStream);
                    await bmp.SetSourceAsync(randomStream);
#else
                    await bmp.SetSourceAsync(stream);
#endif
                    return bmp;
                }
            }
            finally
            {
                stream.Close();
            }
            return null;
        }

        static HttpRequestMessage CreateRequestMessage(string p_act)
        {
            // 使用http2协议Post方法，路径相同连续 Get 时无效！
            return new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Version = new Version(2, 0),
                RequestUri = new Uri($"{Kit.Stub.ServerUrl}/fsm/.d/{p_act}"),
                // 无内容时 okhttp 异常
                Content = new StringContent("a")
            };
        }
    }
}