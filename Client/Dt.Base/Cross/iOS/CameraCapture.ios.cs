#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Windows.Storage.AccessCache;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    class CameraCapture
    {
        bool _isCameraAvailable = UIImagePickerController.IsCameraDeviceAvailable(UIKit.UIImagePickerControllerCameraDevice.Front)
            | UIImagePickerController.IsCameraDeviceAvailable(UIKit.UIImagePickerControllerCameraDevice.Rear);

        public async Task<FileData> TakePhoto(CapturePhotoOptions p_options)
        {
            if (_isCameraAvailable)
            {
                AtKit.Warn("无摄像头设备");
                return null;
            }

            return null;
        }

        public async Task<FileData> TakeVideo(CaptureVideoOptions p_options)
        {
            if (_isCameraAvailable)
            {
                AtKit.Warn("无摄像头设备");
                return null;
            }
            return null;
        }
    }
}
#endif