#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System.IO;
using Windows.Storage.AccessCache;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class FileData
    {
        public FileData(string p_filePath, string p_fileName)
        {
            FilePath = p_filePath;
            FileName = p_fileName;
        }

        /// <summary>
        /// 文件名，不包含路径，UWP存放安全访问路径
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// 完整路径
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
#if ANDROID
            if (IOUtil.IsMediaStore(FilePath))
            {
                var contentUri = Android.Net.Uri.Parse(FilePath);
                return Android.App.Application.Context.ContentResolver.OpenInputStream(contentUri);
            }
            return System.IO.File.OpenRead(FilePath);
#elif UWP
            return StorageApplicationPermissions.FutureAccessList.GetFileAsync(FilePath).GetResults().OpenStreamForReadAsync().Result;
#else
            return new FileStream(FilePath, FileMode.Open, FileAccess.Read);
#endif
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        public byte[] GetBytes()
        {
            using (var stream = GetStream())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}