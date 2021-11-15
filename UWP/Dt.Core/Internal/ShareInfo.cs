#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-12-10 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.DataTransfer.ShareTarget;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 接收分享的内容描述
    /// </summary>
    public class ShareInfo
    {
        /// <summary>
        /// 分享内容的类型
        /// </summary>
        public ShareDataType DataType { get; set; }

        /// <summary>
        /// 文本内容，非文件内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// 分享的文件名，包括扩展名
        /// </summary>
        public string FileName
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
#if UWP
                    int index = FilePath.LastIndexOf('\\');
#else
                    int index = FilePath.LastIndexOf('/');
#endif
                    if (index > -1)
                        return FilePath.Substring(index + 1);
                }
                return "";
            }
        }

        /// <summary>
        /// 分享文件的扩展名，最前面.
        /// </summary>
        public string FileExt
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath))
                {
                    int index = FilePath.LastIndexOf('.');
                    if (index > -1)
                        return FilePath.Substring(index);
                }
                return "";
            }
        }

        /// <summary>
        /// 文件大小
        /// </summary>
        public ulong FileLength
        {
            get
            {
#if UWP
                return _length;
#else
                return (ulong)(new FileInfo(FilePath)).Length;
#endif
            }
        }

        /// <summary>
        /// 文件流
        /// </summary>
        /// <returns></returns>
        public Stream GetStream()
        {
#if UWP
            return _sf?.OpenStreamForReadAsync().Result;
#elif ANDROID
            if (FilePath.StartsWith("content://"))
                return Android.App.Application.Context.ContentResolver.OpenInputStream(Android.Net.Uri.Parse(FilePath));
            return System.IO.File.OpenRead(FilePath);
#elif IOS
            return new FileStream(FilePath, FileMode.Open, FileAccess.Read);
#elif WASM
            return null;
#endif
        }

        /// <summary>
        /// 分享结束
        /// </summary>
        public void ShareCompleted()
        {
#if UWP
            if (_shareOperation != null)
            {
                _shareOperation.ReportCompleted();
                _shareOperation = null;
            }
#elif WASM
            
#else
            Application.Current.Exit();
#endif
        }


#if UWP
        ShareOperation _shareOperation;
        StorageFile _sf;
        ulong _length;

        public async Task Init(ShareOperation p_shareOperation)
        {
            _shareOperation = p_shareOperation;
            var data = _shareOperation.Data;
            if (data.Contains(StandardDataFormats.StorageItems))
            {
                var files = await data.GetStorageItemsAsync();
                if (files != null && files.Count > 0 && files[0] is StorageFile sf)
                {
                    _sf = sf;
                    FilePath = _sf.Path;
                    _length = (await _sf.GetBasicPropertiesAsync()).Size;
                    if (_image.Contains(_sf.FileType))
                        DataType = ShareDataType.Image;
                    else if (_video.Contains(_sf.FileType))
                        DataType = ShareDataType.Video;
                    else if (_audio.Contains(_sf.FileType))
                        DataType = ShareDataType.Audio;
                    else
                        DataType = ShareDataType.File;
                }
            }
            else if (_shareOperation.Data.Contains(StandardDataFormats.WebLink))
            {
                DataType = ShareDataType.Text;
                Content = (await data.GetWebLinkAsync()).AbsoluteUri;
            }
            else if (_shareOperation.Data.Contains(StandardDataFormats.Text))
            {
                DataType = ShareDataType.Text;
                Content = await data.GetTextAsync();
            }
        }
#elif IOS
        public ShareInfo(string p_path)
        {
            FilePath = p_path;
            var ext = FileExt;
            if (_image.Contains(ext))
                DataType = ShareDataType.Image;
            else if (_video.Contains(ext))
                DataType = ShareDataType.Video;
            else if (_audio.Contains(ext))
                DataType = ShareDataType.Audio;
            else
                DataType = ShareDataType.File;
        }
#endif

        static readonly string[] _image = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico", ".tif" };
        static readonly string[] _video = new string[] { ".mp4", ".wmv", ".mov" };
        static readonly string[] _audio = new string[] { ".m4a", ".mp3", ".wav", ".wma" };
    }

    /// <summary>
    /// 分享内容的类型
    /// </summary>
    public enum ShareDataType
    {
        Text,

        Image,

        Video,

        Audio,

        File
    }
}
