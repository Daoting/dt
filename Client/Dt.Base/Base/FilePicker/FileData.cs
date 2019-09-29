#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// 缩略图宽或高的最大尺寸
        /// </summary>
        public const int ThumbSize = 180;

        public FileData(string p_filePath, string p_fileName, ulong p_size)
        {
            FilePath = p_filePath;
            FileName = p_fileName;
            Size = p_size;
        }

        /// <summary>
        /// 文件名，包含扩展名
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 完整路径，UWP为安全访问ID
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public ulong Size { get; }

        /// <summary>
        /// 文件名，不包括扩展名
        /// </summary>
        public string DisplayName
        {
            get
            {
                int index;
                if (!string.IsNullOrEmpty(FileName) && (index = FileName.LastIndexOf('.')) != -1)
                    return FileName.Substring(0, index);
                return FileName;
            }
        }

        /// <summary>
        /// 文件扩展名，以 . 开头
        /// </summary>
        public string Ext
        {
            get
            {
                int index;
                if (!string.IsNullOrEmpty(FileName) && (index = FileName.LastIndexOf('.')) != -1)
                    return FileName.Substring(index).ToLower();
                return "";
            }
        }

        /// <summary>
        /// 文件描述
        /// </summary>
        public string Desc { get; private set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThumbPath { get; private set; }

        /// <summary>
        /// 缩略图上传流
        /// </summary>
        public Stream ThumbStream { get; private set; }

#if UWP
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> GetStream()
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                var file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FilePath);
                return await file.OpenStreamForReadAsync();
            }
            return null;
        }
#elif ANDROID
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetStream()
        {
            if (IOUtil.IsMediaStore(FilePath))
            {
                var contentUri = Android.Net.Uri.Parse(FilePath);
                return Task.FromResult(Android.App.Application.Context.ContentResolver.OpenInputStream(contentUri));
            }
            return Task.FromResult((Stream)System.IO.File.OpenRead(FilePath));
        }
#elif IOS
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetStream()
        {
            return Task.FromResult((Stream)new FileStream(FilePath, FileMode.Open, FileAccess.Read));
        }
#endif

        /// <summary>
        /// 获取文件内容
        /// </summary>
        public async Task<byte[]> GetBytes()
        {
            using (var stream = await GetStream())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 上传前：生成文件描述和缩略图
        /// </summary>
        internal async Task InitUpload()
        {
            string ext = Ext;
            if (string.IsNullOrEmpty(ext))
            {
                Desc = "未知文件";
            }
            else if (FileFilter.UwpImage.Contains(ext))
            {
                try
                {
                    using (Image image = Image.Load(await GetStream()))
                    {
                        Desc = $"{image.Width} x {image.Height} ({ext.TrimStart('.')})";

                        // 图片大小超过缩略图时生成
                        if (image.Width > ThumbSize || image.Height > ThumbSize)
                        {
                            if (image.Width > image.Height)
                                image.Mutate(x => x.Resize(ThumbSize, 0));
                            else
                                image.Mutate(x => x.Resize(0, ThumbSize));

                            ThumbStream = new MemoryStream();
                            image.SaveAsJpeg(ThumbStream);

                            // 保存到文件
                            ThumbStream.Seek(0, SeekOrigin.Begin);
                            ThumbPath = Path.Combine(AtSys.DocPath, AtKit.NewID + "-t.jpg");
                            using (var fs = File.Create(ThumbPath))
                            {
                                await ThumbStream.CopyToAsync(fs);
                                await fs.FlushAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "生成缩略图异常");
                }
            }
            else if (FileFilter.UwpVideo.Contains(ext))
            {
#if UWP
                var sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FilePath);
                var prop = await sf.Properties.GetVideoPropertiesAsync();
                Desc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(prop.Duration.Ticks), prop.Width, prop.Height);
                // 默认根据DPI调整缩略图大小
                ThumbStream = (await sf.GetThumbnailAsync(ThumbnailMode.SingleItem, ThumbSize, ThumbnailOptions.ResizeThumbnail)).AsStreamForRead();

                // 保存到文件
                ThumbPath = Path.Combine(AtSys.DocPath, AtKit.NewID + "-t.jpg");
                using (var fs = File.Create(ThumbPath))
                {
                    await ThumbStream.CopyToAsync(fs);
                }
#elif ANDROID
                Android.Media.MediaMetadataRetriever media = new Android.Media.MediaMetadataRetriever();
                try
                {
                    await media.SetDataSourceAsync(FilePath);
                    string dur = media.ExtractMetadata(Android.Media.MetadataKey.Duration);
                    var bmp = media.GetFrameAtTime(1, Android.Media.Option.ClosestSync);
                    Desc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(long.Parse(dur) * 10000), bmp.Width, bmp.Height);

                    // 将帧输出到流
                    MemoryStream tempStream = new MemoryStream();
                    await bmp.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Png, 100, tempStream);

                    // 缩略图
                    try
                    {
                        tempStream.Seek(0, SeekOrigin.Begin);
                        using (Image image = Image.Load(tempStream))
                        {
                            if (image.Width > image.Height)
                                image.Mutate(x => x.Resize(ThumbSize, 0));
                            else
                                image.Mutate(x => x.Resize(0, ThumbSize));

                            ThumbStream = new MemoryStream();
                            image.SaveAsJpeg(ThumbStream);

                            // 保存到文件
                            ThumbStream.Seek(0, SeekOrigin.Begin);
                            ThumbPath = Path.Combine(AtSys.DocPath, AtKit.NewID + "-t.jpg");
                            using (var fs = File.Create(ThumbPath))
                            {
                                await ThumbStream.CopyToAsync(fs);
                                await fs.FlushAsync();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "生成缩略图异常");
                    }
                }
                catch { }
                finally
                {
                    media.Release();
                }
#elif IOS

#endif
            }
            else if (FileFilter.UwpAudio.Contains(ext))
            {
#if UWP
                var sf = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FilePath);
                var prop = await sf.Properties.GetMusicPropertiesAsync();
                Desc = string.Format("{0:mm:ss}", new DateTime(prop.Duration.Ticks));
#elif ANDROID
                Android.Media.MediaMetadataRetriever media = new Android.Media.MediaMetadataRetriever();
                try
                {
                    await media.SetDataSourceAsync(FilePath);
                    string dur = media.ExtractMetadata(Android.Media.MetadataKey.Duration);
                    Desc = string.Format("{0:mm:ss}", new DateTime(long.Parse(dur) * 10000));
                }
                catch { }
                finally
                {
                    media.Release();
                }
#elif IOS

#endif
            }
            else
            {
                Desc = $"{ext.TrimStart('.')}文件";
            }
        }

        internal void Close()
        {
            if (ThumbStream != null)
            {
                ThumbStream.Close();
                ThumbStream = null;
            }
        }
    }
}