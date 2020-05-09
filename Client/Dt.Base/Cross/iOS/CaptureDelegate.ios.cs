#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2019-09-29 创建
******************************************************************************/
#endregion

#region 引用命名
using AVFoundation;
using CoreGraphics;
using CoreImage;
using Dt.Core;
using Foundation;
using ImageIO;
using MobileCoreServices;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using NSAction = System.Action;
#endregion

namespace Dt.Base
{
	/// <summary>
	/// 参加 https://github.com/jamesmontemagno/MediaPlugin
	/// </summary>
	class CaptureDelegate : UIImagePickerControllerDelegate
	{
		readonly TaskCompletionSource<FileData> _tcs = new TaskCompletionSource<FileData>();

		public Task<FileData> Task => _tcs.Task;

		public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
		{
			FileData fd = null;
			switch ((NSString)info[UIImagePickerController.MediaType])
			{
				case "public.image":
					fd = GetPictureMediaFile(info);
					break;

				case "public.movie":
					fd = GetMovieMediaFile(info);
					break;

				default:
					throw new NotSupportedException();
			}
			UIApplication.SharedApplication.SetStatusBarStyle(CameraCapture.StatusBarStyle, false);
			Dismiss(picker, () => _tcs.TrySetResult(fd));
		}

		public override void Canceled(UIImagePickerController picker)
		{
			UIApplication.SharedApplication.SetStatusBarStyle(CameraCapture.StatusBarStyle, false);
			Dismiss(picker, () => _tcs.SetResult(null));
		}

        #region 图片
        FileData GetPictureMediaFile(NSDictionary info)
		{
			var image = (UIImage)info[UIImagePickerController.EditedImage] ?? (UIImage)info[UIImagePickerController.OriginalImage];
			if (image == null)
				return null;

			var fileExt = ((info[UIImagePickerController.ReferenceUrl] as NSUrl)?.PathExtension == "PNG") ? "png" : "jpg";
			var path = System.IO.Path.Combine(AtLocal.CachePath, $"{AtKit.NewID}.{fileExt}");

			NSDictionary meta = null;
			try
			{
				meta = info[UIImagePickerController.MediaMetadata] as NSDictionary;
				if (meta != null && meta.ContainsKey(ImageIO.CGImageProperties.Orientation))
				{
					var newMeta = new NSMutableDictionary();
					newMeta.SetValuesForKeysWithDictionary(meta);
					var newTiffDict = new NSMutableDictionary();
					newTiffDict.SetValuesForKeysWithDictionary(meta[ImageIO.CGImageProperties.TIFFDictionary] as NSDictionary);
					newTiffDict.SetValueForKey(meta[ImageIO.CGImageProperties.Orientation], ImageIO.CGImageProperties.TIFFOrientation);
					newMeta[ImageIO.CGImageProperties.TIFFDictionary] = newTiffDict;

					meta = newMeta;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unable to get metadata: {ex}");
			}

			var savedImage = false;
			if (meta != null)
				savedImage = SaveImageWithMetadata(image, meta, path, fileExt);

			if (!savedImage)
			{
				var imageData = fileExt == "png" ? image.AsPNG() : image.AsJPEG();
				if (imageData == null)
					throw new NullReferenceException("Unable to convert image to jpeg, please ensure file exists or lower quality level");

				imageData.Save(path, true);
				imageData.Dispose();
			}

			var fd = new FileData(path, System.IO.Path.GetFileName(path), (ulong)new FileInfo(path).Length);
			fd.Desc = $"{image.CGImage.Width} x {image.CGImage.Height} ({fileExt})";
			fd.ThumbPath = Path.Combine(AtLocal.CachePath, AtKit.NewID + "-t.jpg");
			return fd;
		}

		static bool SaveImageWithMetadata(UIImage image, NSDictionary meta, string path, string fileExt)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
				return SaveImageWithMetadataiOS13(image, meta, path, fileExt);

			try
			{
				var imageData = fileExt == "png" ? image.AsPNG() : image.AsJPEG();
				if (imageData == null)
					throw new NullReferenceException("Unable to convert image to jpeg, please ensure file exists or lower quality level");

				var dataProvider = new CGDataProvider(imageData);
				var cgImageFromJpeg = CGImage.FromJPEG(dataProvider, null, false, CGColorRenderingIntent.Default);
				var imageWithExif = new NSMutableData();
				var destination = CGImageDestination.Create(imageWithExif, UTType.JPEG, 1);
				var cgImageMetadata = new CGMutableImageMetadata();
				var destinationOptions = new CGImageDestinationOptions();

				if (meta.ContainsKey(ImageIO.CGImageProperties.Orientation))
					destinationOptions.Dictionary[ImageIO.CGImageProperties.Orientation] = meta[ImageIO.CGImageProperties.Orientation];

				if (meta.ContainsKey(ImageIO.CGImageProperties.DPIWidth))
					destinationOptions.Dictionary[ImageIO.CGImageProperties.DPIWidth] = meta[ImageIO.CGImageProperties.DPIWidth];

				if (meta.ContainsKey(ImageIO.CGImageProperties.DPIHeight))
					destinationOptions.Dictionary[ImageIO.CGImageProperties.DPIHeight] = meta[ImageIO.CGImageProperties.DPIHeight];

				if (meta.ContainsKey(ImageIO.CGImageProperties.ExifDictionary))
					destinationOptions.ExifDictionary = new CGImagePropertiesExif(meta[ImageIO.CGImageProperties.ExifDictionary] as NSDictionary);

				if (meta.ContainsKey(ImageIO.CGImageProperties.TIFFDictionary))
				{
					var existingTiffDict = meta[ImageIO.CGImageProperties.TIFFDictionary] as NSDictionary;
					if (existingTiffDict != null)
					{
						var newTiffDict = new NSMutableDictionary();
						newTiffDict.SetValuesForKeysWithDictionary(existingTiffDict);
						newTiffDict.SetValueForKey(meta[ImageIO.CGImageProperties.Orientation], ImageIO.CGImageProperties.TIFFOrientation);
						destinationOptions.TiffDictionary = new CGImagePropertiesTiff(newTiffDict);
					}
				}

				if (meta.ContainsKey(ImageIO.CGImageProperties.GPSDictionary))
					destinationOptions.GpsDictionary = new CGImagePropertiesGps(meta[ImageIO.CGImageProperties.GPSDictionary] as NSDictionary);

				if (meta.ContainsKey(ImageIO.CGImageProperties.JFIFDictionary))
					destinationOptions.JfifDictionary = new CGImagePropertiesJfif(meta[ImageIO.CGImageProperties.JFIFDictionary] as NSDictionary);

				if (meta.ContainsKey(ImageIO.CGImageProperties.IPTCDictionary))
					destinationOptions.IptcDictionary = new CGImagePropertiesIptc(meta[ImageIO.CGImageProperties.IPTCDictionary] as NSDictionary);
				
				destination.AddImageAndMetadata(cgImageFromJpeg, cgImageMetadata, destinationOptions);
				var success = destination.Close();
				if (success)
				{
					var saved = imageWithExif.Save(path, true, out var error);
					if (error != null)
						Debug.WriteLine($"Unable to save exif data: {error.ToString()}");

					imageWithExif.Dispose();
					imageWithExif = null;
				}

				return success;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unable to save image with metadata: {ex}");
			}

			return false;
		}

		static bool SaveImageWithMetadataiOS13(UIImage image, NSDictionary meta, string path, string fileExt)
		{
			try
			{
				var imageData = fileExt == "png" ? image.AsPNG() : image.AsJPEG();
				if (imageData == null)
					throw new NullReferenceException("Unable to convert image to jpeg, please ensure file exists or lower quality level");

				// Copy over meta data
				using (var ciImage = CIImage.FromData(imageData))
				using (var newImageSource = ciImage.CreateBySettingProperties(meta))
				using (var ciContext = new CIContext())
				{
					if (fileExt == "png")
						return ciContext.WritePngRepresentation(newImageSource, NSUrl.FromFilename(path), CIFormat.ARGB8, CGColorSpace.CreateSrgb(), new NSDictionary(), out var error2);

					return ciContext.WriteJpegRepresentation(newImageSource, NSUrl.FromFilename(path), CGColorSpace.CreateSrgb(), new NSDictionary(), out var error);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Unable to save image with metadata: {ex}");
			}
			return false;
		}
        #endregion

        #region 录像
        FileData GetMovieMediaFile(NSDictionary info)
		{
			var url = info[UIImagePickerController.MediaURL] as NSUrl;
			if (url == null)
				return null;

			var path = System.IO.Path.Combine(AtLocal.CachePath, AtKit.NewID + ".mp4");
			try
			{
				File.Move(url.Path, path);
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"Unable to move file, trying to copy. {ex.Message}");
				try
				{
					File.Copy(url.Path, path);
					File.Delete(url.Path);
				}
				catch (Exception)
				{
					Debug.WriteLine($"Unable to copy/delete file, will be left around :( {ex.Message}");
				}
			}

			return new FileData(path, System.IO.Path.GetFileName(path), (ulong)new FileInfo(path).Length);
		}
        #endregion

        void Dismiss(UINavigationController picker, NSAction onDismiss)
		{
			picker.DismissViewController(true, onDismiss);
		}
	}
}
#endif
