#if WASM
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
using Windows.Devices.Enumeration;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.FileProperties;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    class CameraCapture
    {
        public async Task<FileData> TakePhoto(CapturePhotoOptions p_options)
        {
            if (!await IsCameraAvailable())
                return null;

            var options = p_options ?? new CapturePhotoOptions();
            var capture = new CameraCaptureUI();
            capture.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            capture.PhotoSettings.MaxResolution = (options.VideoQuality == 1) ? CameraCaptureUIMaxPhotoResolution.HighestAvailable : CameraCaptureUIMaxPhotoResolution.SmallVga;
            // we can only disable cropping if resolution is set to max
            if (capture.PhotoSettings.MaxResolution == CameraCaptureUIMaxPhotoResolution.HighestAvailable)
                capture.PhotoSettings.AllowCropping = options.AllowCropping;

            var result = await capture.CaptureFileAsync(CameraCaptureUIMode.Photo);
            if (result == null)
                return null;

            // 将文件移动到CachePath
            await result.MoveAsync(await StorageFolder.GetFolderFromPathAsync(Kit.CachePath), Kit.NewGuid + ".jpg");

            var fd = new FileData(result.Path, result.Name, (await result.GetBasicPropertiesAsync()).Size);
            var prop = await result.Properties.GetImagePropertiesAsync();
            fd.Desc = $"{prop.Width} x {prop.Height} (jpg)";

            // 生成缩略图
            if (prop.Width > FileData.ThumbSize || prop.Height > FileData.ThumbSize)
            {
                fd.ThumbPath = Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                using (var fs = File.Create(fd.ThumbPath))
                {
                    // 默认根据DPI调整缩略图大小
                    var fl = await result.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, FileData.ThumbSize, ThumbnailOptions.ResizeThumbnail);
                    await fl.AsStreamForRead().CopyToAsync(fs);
                }
            }
            return fd;
        }

        public async Task<FileData> TakeVideo(CaptureVideoOptions p_options)
        {
            if (!await IsCameraAvailable())
                return null;

            var options = p_options ?? new CaptureVideoOptions();
            var capture = new CameraCaptureUI();
            capture.VideoSettings.Format = CameraCaptureUIVideoFormat.Mp4;
            capture.VideoSettings.MaxResolution = (options.VideoQuality == 1) ? CameraCaptureUIMaxVideoResolution.HighDefinition : CameraCaptureUIMaxVideoResolution.LowDefinition;
            capture.VideoSettings.AllowTrimming = options.AllowCropping;
            if (capture.VideoSettings.AllowTrimming)
                capture.VideoSettings.MaxDurationInSeconds = (float)options.DesiredLength.TotalSeconds;

            var result = await capture.CaptureFileAsync(CameraCaptureUIMode.Video);
            if (result == null)
                return null;

            // 将文件移动到CachePath
            await result.MoveAsync(await StorageFolder.GetFolderFromPathAsync(Kit.CachePath), Kit.NewGuid + ".mp4");

            var fd = new FileData(result.Path, result.Name, (await result.GetBasicPropertiesAsync()).Size);
            var prop = await result.Properties.GetVideoPropertiesAsync();
            fd.Desc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(prop.Duration.Ticks), prop.Width, prop.Height);

            // 生成缩略图
            fd.ThumbPath = Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
            using (var fs = File.Create(fd.ThumbPath))
            {
                // 默认根据DPI调整缩略图大小
                var fl = await result.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.SingleItem, FileData.ThumbSize, ThumbnailOptions.ResizeThumbnail);
                await fl.AsStreamForRead().CopyToAsync(fs);
            }
            return fd;
        }

        async Task<bool> IsCameraAvailable()
        {
            try
            {
                var info = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture).AsTask().ConfigureAwait(false);
                foreach (var device in info)
                {
                    if (device.IsEnabled)
                        return true;
                }
            }
            catch { }

            Kit.Warn("无摄像头设备");
            return false;
        }
    }
}
#endif