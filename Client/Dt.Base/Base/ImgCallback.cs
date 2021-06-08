#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-03-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 加载图片工具类
    /// </summary>
    internal partial class DefaultCallback : ICallback
    {
#if WASM
        /// <summary>
        /// 加载文件服务的图片，支持路径 或 FileList中json格式
        /// </summary>
        /// <param name="p_path">路径或FileList中json格式</param>
        /// <param name="p_img"></param>
        public Task LoadImage(string p_path, Image p_img)
        {
            if (string.IsNullOrEmpty(p_path))
                return Task.CompletedTask;

            // 按照FileList中json格式获取路径，如：
            // [["photo/E3/18/58108158862553088.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-03-09 16:21"]]
            if (p_path.StartsWith("["))
            {
                int i = p_path.IndexOf("\",");
                if (i <= 3)
                    return Task.CompletedTask;
                p_path = p_path.Substring(3, i - 3);
            }

            // 图片无缓存
            p_img.Source = new BitmapImage(new Uri($"{Kit.Stub.ServerUrl}/fsm/{p_path}"));
            return Task.CompletedTask;
        }
#else
        readonly AsyncLocker _locker = new AsyncLocker();

        /// <summary>
        /// 加载文件服务的图片，优先加载缓存，支持路径 或 FileList中json格式
        /// </summary>
        /// <param name="p_path">路径或FileList中json格式</param>
        /// <param name="p_img"></param>
        public async Task LoadImage(string p_path, Image p_img = null)
        {
            // 加载过程：
            // 1. 本地.doc目录是否存在
            // 2. 不存在从文件服务下载文件，缓存到本地.doc目录
            // 3. 下载不成功删除缓存文件
            // 4. 下载成功，加载本地图片
            // 
            if (string.IsNullOrEmpty(p_path))
                return;

            // 按照FileList中json格式获取路径，如：
            // [["photo/E3/18/58108158862553088.jpg","未标题-2","300 x 300 (.jpg)",49179,"daoting","2020-03-09 16:21"]]
            if (p_path.StartsWith("["))
            {
                int i = p_path.IndexOf("\",");
                if (i <= 3)
                    return;
                p_path = p_path.Substring(3, i - 3);
            }

            // 文件服务的路径肯定含/
            int index = p_path.LastIndexOf('/');
            if (index <= 0)
                return;

            // 减轻并发下载时服务端的压力，避免异步下载、显示同一图片时异常
            using (await _locker.LockAsync())
            {
                string fileName = p_path.Substring(index + 1);
                string path = System.IO.Path.Combine(Kit.CachePath, fileName);
                if (!System.IO.File.Exists(path))
                {
                    if (!await Downloader.GetAndCacheFile(p_path))
                        return;
                }

                var bmp = await GetLocalImage(fileName);
                if (p_img != null)
                    p_img.Source = bmp;
            }
        }
#endif

        /// <summary>
        /// 获取存放在.doc路径的本地图片
        /// </summary>
        /// <param name="p_fileName">文件名</param>
        /// <returns></returns>
        static async Task<BitmapImage> GetLocalImage(string p_fileName)
        {
            string path = Path.Combine(Kit.CachePath, p_fileName);
            if (!File.Exists(path))
                return null;
            
            BitmapImage bmp = new BitmapImage();
#if UWP
            StorageFile sf = await StorageFile.GetFileFromPathAsync(path);
            if (sf != null)
            {
                try
                {
                    using (var stream = await sf.OpenAsync(FileAccessMode.Read))
                    {
                        await bmp.SetSourceAsync(stream);
                    }
                }
                catch { }
            }
#elif ANDROID
            using (var stream = System.IO.File.OpenRead(path))
            {
                await bmp.SetSourceAsync(stream);
            }
#elif IOS
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                await bmp.SetSourceAsync(stream);
            }
#elif WASM
            // 不支持本地文件
            await Task.CompletedTask;
#endif
            return bmp;
        }
    }
}
