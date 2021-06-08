#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-06-08 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 拍照选项
    /// </summary>
    public class CapturePhotoOptions
    {
        /// <summary>
        /// 是否默认使用前置摄像头
        /// </summary>
        public bool UseFrontCamera { get; set; }

        /// <summary>
        /// 视频/图片品质，1高品质，0普通
        /// </summary>
        public int VideoQuality { get; set; } = 1;

        /// <summary>
        /// 是否允许编辑视频或照片
        /// Photo: UWP cropping can only be disabled on full size
        /// Video: UWP trimming when disabled won't allow time limit to be set
        /// </summary>
        public bool AllowCropping { get; set; }
    }

    /// <summary>
    /// 录像选项
    /// </summary>
    public class CaptureVideoOptions : CapturePhotoOptions
    {
        /// <summary>
        /// 视频录制时间限制
        /// </summary>
        public TimeSpan DesiredLength { get; set; }

        /// <summary>
        /// 视频文件大小限制
        /// 只 Android 有效
        /// Eg. 1000000 = 1MB
        /// </summary>
        public long DesiredSize { get; set; }
    }
}