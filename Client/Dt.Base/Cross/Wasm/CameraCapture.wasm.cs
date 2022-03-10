#if WASM
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
#endregion

namespace Dt.Base
{
    class CameraCapture
    {
        public Task<FileData> TakePhoto(CapturePhotoOptions p_options)
        {
            return Task.FromResult<FileData>(null);
        }

        public Task<FileData> TakeVideo(CaptureVideoOptions p_options)
        {
            return Task.FromResult<FileData>(null);
        }
    }
}
#endif