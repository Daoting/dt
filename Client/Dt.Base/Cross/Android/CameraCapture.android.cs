#if ANDROID
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Provider;
using Dt.Core;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    class CameraCapture
    {
        TaskCompletionSource<FileData> _tcs;

        public async Task<FileData> TakePhoto(CapturePhotoOptions p_options)
        {
            if (await IsCameraAvailable())
                return await TakeMedia(true, p_options ?? new CapturePhotoOptions());
            return null;
        }

        public async Task<FileData> TakeVideo(CaptureVideoOptions p_options)
        {
            if (await IsCameraAvailable())
                return await TakeMedia(false, p_options ?? new CaptureVideoOptions());
            return null;
        }

        Task<FileData> TakeMedia(bool p_isPhoto, CapturePhotoOptions p_options)
        {
            var ntcs = new TaskCompletionSource<FileData>();
            var previousTcs = Interlocked.Exchange(ref _tcs, ntcs);
            if (previousTcs != null)
                previousTcs.TrySetResult(null);

            try
            {
                var intent = new Intent(Application.Context, typeof(CameraCaptureActivity));
                intent.PutExtra(CameraCaptureActivity.ExtraIsPhoto, p_isPhoto);
                intent.PutExtra(CameraCaptureActivity.ExtraFront, p_options.UseFrontCamera);
                intent.PutExtra(MediaStore.ExtraVideoQuality, p_options.VideoQuality);
                if (p_options is CaptureVideoOptions vo)
                {
                    // 一定要转成int，否正无法按int获取值！
                    intent.PutExtra(MediaStore.ExtraDurationLimit, (int)vo.DesiredLength.TotalSeconds);
                    if (vo.DesiredSize > 0)
                        intent.PutExtra(MediaStore.ExtraSizeLimit, vo.DesiredSize);
                }
                intent.SetFlags(ActivityFlags.NewTask);
                
                Application.Context.StartActivity(intent);
                EventHandler<FileData> handler = null;
                handler = (s, e) =>
                {
                    var tcs = Interlocked.Exchange(ref _tcs, null);
                    CameraCaptureActivity.Captured -= handler;
                    tcs?.SetResult(e);
                };
                CameraCaptureActivity.Captured += handler;
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
                _tcs.SetException(ex);
            }

            return _tcs.Task;
        }

        async Task<bool> IsCameraAvailable()
        {
            if (!Platform.AppContext.PackageManager.HasSystemFeature(PackageManager.FeatureCamera))
            {
                Kit.Warn("无摄像头设备");
                return false;
            }

            var hasPer = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (hasPer != PermissionStatus.Granted)
                hasPer = await Permissions.RequestAsync<Permissions.Camera>();
            if (hasPer != PermissionStatus.Granted)
            {
                Kit.Warn("摄像头未授权！");
                return false;
            }

            return true;
        }
    }
}
#endif