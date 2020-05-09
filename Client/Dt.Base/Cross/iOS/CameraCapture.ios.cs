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
using System;
using System.Linq;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Essentials;
using System.Threading;
using Foundation;
#endregion

namespace Dt.Base
{
    class CameraCapture
    {
        public async Task<FileData> TakePhoto(CapturePhotoOptions p_options)
        {
            if (await IsCameraAvailable())
                return await TakeMedia(true, p_options);
            return null;
        }

        public async Task<FileData> TakeVideo(CaptureVideoOptions p_options)
        {
			if (await IsCameraAvailable())
				return await TakeMedia(false, p_options);
			return null;
		}

		/// <summary>
		/// Color of the status bar
		/// </summary>
		public static UIStatusBarStyle StatusBarStyle { get; set; }

		Task<FileData> TakeMedia(bool p_isPhoto, CapturePhotoOptions p_options)
		{
			StatusBarStyle = UIApplication.SharedApplication.StatusBarStyle;
			var viewController = GetHostViewController();

			var ndelegate = new CaptureDelegate();
			var od = Interlocked.CompareExchange(ref _pickerDelegate, ndelegate, null);
			if (od != null)
				throw new InvalidOperationException("同一时间只可激活一次");

			var view = SetupController(ndelegate, p_isPhoto, p_options);
			viewController.PresentViewController(view, true, null);

			return ndelegate.Task.ContinueWith(t =>
			{
				Dismiss(view);
				return t.Result;
			});
		}

		static CaptureController SetupController(CaptureDelegate p_delegate, bool p_isPhoto, CapturePhotoOptions p_options)
		{
			var picker = new CaptureController(p_delegate);
			picker.MediaTypes = new[] { p_isPhoto ? "public.image" : "public.movie" };
			picker.SourceType = UIImagePickerControllerSourceType.Camera;

			var options = p_options ?? new CapturePhotoOptions();
			picker.CameraDevice = options.UseFrontCamera ? UIImagePickerControllerCameraDevice.Front : UIImagePickerControllerCameraDevice.Rear;
			picker.AllowsEditing = options.AllowCropping;

			if (p_isPhoto)
			{
				picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Photo;
			}
			else if (p_options is CaptureVideoOptions voptions)
			{
				picker.CameraCaptureMode = UIImagePickerControllerCameraCaptureMode.Video;
				picker.VideoQuality = (voptions.VideoQuality == 0) ? UIImagePickerControllerQualityType.Low : UIImagePickerControllerQualityType.High;
				picker.VideoMaximumDuration = voptions.DesiredLength.TotalSeconds;
			}
			return picker;
		}

		UIImagePickerControllerDelegate _pickerDelegate;
		void Dismiss(UIViewController picker)
		{
			try
			{
				picker?.Dispose();
			}
			catch { }

			GC.Collect(GC.MaxGeneration, GCCollectionMode.Default);
			Interlocked.Exchange(ref _pickerDelegate, null);
		}

		static UIViewController GetHostViewController()
		{
			UIViewController viewController = null;
			var window = UIApplication.SharedApplication.KeyWindow;
			if (window == null)
				throw new InvalidOperationException("There's no current active window");

			if (window.WindowLevel == UIWindowLevel.Normal)
				viewController = window.RootViewController;

			if (viewController == null)
			{
				window = UIApplication.SharedApplication.Windows.OrderByDescending(w => w.WindowLevel).FirstOrDefault(w => w.RootViewController != null && w.WindowLevel == UIWindowLevel.Normal);
				if (window == null)
					throw new InvalidOperationException("Could not find current view controller");
				else
					viewController = window.RootViewController;
			}

			while (viewController.PresentedViewController != null)
				viewController = viewController.PresentedViewController;

			return viewController;
		}

		async Task<bool> IsCameraAvailable()
        {
            bool isCameraAvailable = UIImagePickerController.IsCameraDeviceAvailable(UIKit.UIImagePickerControllerCameraDevice.Front)
                                      | UIImagePickerController.IsCameraDeviceAvailable(UIKit.UIImagePickerControllerCameraDevice.Rear);
            if (!isCameraAvailable)
            {
                AtKit.Warn("无摄像头设备");
                return false;
            }

            var hasPer = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (hasPer != PermissionStatus.Granted)
                hasPer = await Permissions.RequestAsync<Permissions.Camera>();
            if (hasPer != PermissionStatus.Granted)
            {
                AtKit.Warn("摄像头未授权！");
                return false;
            }

            return true;
        }
    }
}
#endif