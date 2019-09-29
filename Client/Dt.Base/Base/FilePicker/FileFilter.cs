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

        /// <summary>
        /// ios图片格式
        /// </summary>
        public static readonly string[] IOSImage = new string[]
        {
#if IOS
            MobileCoreServices.UTType.Image
#endif
        };

        /// <summary>
        /// uwp视频格式
        /// </summary>
        public static readonly string[] UwpVideo = new string[] { ".mp4", ".wmv" };

        /// <summary>
        /// android视频格式
        /// </summary>
        public static readonly string[] AndroidVideo = new string[] { "video/*", };

        /// <summary>
        /// ios视频格式
        /// </summary>
        public static readonly string[] IOSVideo = new string[]
        {
#if IOS
            MobileCoreServices.UTType.Video
#endif
        };

        /// <summary>
        /// uwp音频格式
        /// </summary>
        public static readonly string[] UwpAudio = new string[] { ".m4a", ".mp3", ".wav", ".wma" };

        /// <summary>
        /// android音频格式
        /// </summary>
        public static readonly string[] AndroidAudio = new string[] { "audio/*" };

        /// <summary>
        /// ios音频格式
        /// </summary>
        public static readonly string[] IOSAudio = new string[]
        {
#if IOS
            MobileCoreServices.UTType.Audio
#endif
        };

        /// <summary>
        /// uwp媒体文件格式
        /// </summary>
        public static readonly string[] UwpMedia = new string[] { ".png", ".jpg", ".bmp", ".gif", ".ico", ".tif", ".mp4", ".wmv", ".m4a", ".mp3", ".wav", ".wma" };

        /// <summary>
        /// android媒体文件格式
        /// </summary>
        public static readonly string[] AndroidMedia = new string[] { "image/*", "video/*", "audio/*" };

        /// <summary>
        /// ios媒体文件格式
        /// </summary>
        public static readonly string[] IOSMedia = new string[]
        {
#if IOS
            MobileCoreServices.UTType.Image,
            MobileCoreServices.UTType.Video,
            MobileCoreServices.UTType.Audio
#endif
        };
    }
}