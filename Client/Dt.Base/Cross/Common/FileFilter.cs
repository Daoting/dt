#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-25 创建
******************************************************************************/
#endregion

#region 引用命名
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 常用文件类型
    /// </summary>
    public static class FileFilter
    {
        /// <summary>
        /// uwp图片格式
        /// </summary>
        public static readonly string[] UwpImage = new string[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".ico", ".tif" };

        /// <summary>
        /// android图片格式
        /// </summary>
        public static readonly string[] AndroidImage = new string[] { "image/*" };

#if IOS
        /// <summary>
        /// ios图片格式
        /// </summary>
        public static readonly Photos.PHAssetMediaType[] IOSImage = new Photos.PHAssetMediaType[]
        {
            Photos.PHAssetMediaType.Image
        };
#endif

        /// <summary>
        /// uwp视频格式
        /// </summary>
        public static readonly string[] UwpVideo = new string[] { ".mp4", ".wmv", ".mov" };

        /// <summary>
        /// android视频格式
        /// </summary>
        public static readonly string[] AndroidVideo = new string[] { "video/*", };

#if IOS
        /// <summary>
        /// ios视频格式
        /// </summary>
        public static readonly Photos.PHAssetMediaType[] IOSVideo = new Photos.PHAssetMediaType[]
        {
            Photos.PHAssetMediaType.Video
        };
#endif

        /// <summary>
        /// uwp音频格式
        /// </summary>
        public static readonly string[] UwpAudio = new string[] { ".m4a", ".mp3", ".wav", ".wma" };

        /// <summary>
        /// android音频格式
        /// </summary>
        public static readonly string[] AndroidAudio = new string[] { "audio/*" };

#if IOS
        /// <summary>
        /// ios音频格式
        /// </summary>
        public static readonly Photos.PHAssetMediaType[] IOSAudio = new Photos.PHAssetMediaType[]
        {
            Photos.PHAssetMediaType.Audio
        };
#endif

        /// <summary>
        /// uwp媒体文件格式
        /// </summary>
        public static readonly string[] UwpMedia = new string[] { ".png", ".jpg", ".bmp", ".gif", ".ico", ".tif", ".mp4", ".wmv", ".m4a", ".mp3", ".wav", ".wma" };

        /// <summary>
        /// android媒体文件格式
        /// </summary>
        public static readonly string[] AndroidMedia = new string[] { "image/*", "video/*", "audio/*" };

#if IOS
        /// <summary>
        /// ios媒体文件格式
        /// </summary>
        public static readonly Photos.PHAssetMediaType[] IOSMedia = new Photos.PHAssetMediaType[]
        {
            Photos.PHAssetMediaType.Image,
            Photos.PHAssetMediaType.Video,
            Photos.PHAssetMediaType.Audio
        };
#endif
    }
}