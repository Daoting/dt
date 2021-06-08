#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 选择文件的文件信息
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// 缩略图宽或高的最大尺寸
        /// </summary>
        public const int ThumbSize = 360;

        public FileData(string p_filePath, string p_fileName, ulong p_size)
        {
            FilePath = p_filePath;
            FileName = p_fileName;
            Size = p_size;
            string ext = Ext;
            Desc = string.IsNullOrEmpty(ext) ? "" : $"{ext.TrimStart('.')}文件";
        }

        /// <summary>
        /// 文件名，包含扩展名
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 完整路径，UWP时若不在Kit.CachePath则为安全访问ID
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
        public string Desc { get; set; }

        /// <summary>
        /// 缩略图路径
        /// </summary>
        public string ThumbPath { get; set; }

        /// <summary>
        /// 文件上传过程的UI
        /// </summary>
        public object UploadUI { get; set; }

#if UWP
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public async Task<Stream> GetStream()
        {
            if (string.IsNullOrEmpty(FilePath))
                return null;

            StorageFile file;
            // 安全访问ID
            if (FilePath.StartsWith("{"))
                file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(FilePath);
            else
                file = await StorageFile.GetFileFromPathAsync(FilePath);
            return await file.OpenStreamForReadAsync();
        }
#elif ANDROID
        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public Task<Stream> GetStream()
        {
            if (FilePath.StartsWith("content"))
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
#elif WASM
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
        /// 删除临时缩略图文件，上传成功的已改名无需删除
        /// </summary>
        internal void DeleteThumbnail()
        {
            if (!string.IsNullOrEmpty(ThumbPath) && File.Exists(ThumbPath))
            {
                try
                {
                    File.Delete(ThumbPath);
                }
                catch { }
            }
        }
    }
}