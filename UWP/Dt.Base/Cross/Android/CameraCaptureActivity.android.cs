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
using Android.Graphics;
using Android.Hardware;
using Android.Media;
using Android.OS;
using Android.Provider;
using Dt.Core;
using Dt.Core.Rpc;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Xamarin.Essentials;
#endregion

namespace Dt.Base
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
    public class CameraCaptureActivity : Activity
    {
        /// <summary>
        /// 是否为拍照
        /// </summary>
        public const string ExtraIsPhoto = "EXTRA_IsPhoto";

        /// <summary>
        /// 是否用前置摄像头
        /// </summary>
        public const string ExtraFront = "EXTRA_Front";
        const string _huaweiManufacturer = "Huawei";
        Context _context;
        string _path;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _context = Application.Context;
            bool isPhoto = Intent.GetBooleanExtra(ExtraIsPhoto, false);
            Intent intent = new Intent(isPhoto ? MediaStore.ActionImageCapture : MediaStore.ActionVideoCapture);
            try
            {
                if (!isPhoto)
                {
                    var isPixel = false;
                    try
                    {
                        var name = Settings.System.GetString(_context.ContentResolver, "device_name");
                        isPixel = name.Contains("Pixel") || name.Contains("pixel");
                    }
                    catch (Exception)
                    { }

                    int seconds = Intent.GetIntExtra(MediaStore.ExtraDurationLimit, 0);
                    if (seconds != 0 && !isPixel)
                        intent.PutExtra(MediaStore.ExtraDurationLimit, seconds);

                    long size = Intent.GetLongExtra(MediaStore.ExtraSizeLimit, 0);
                    if (size != 0)
                        intent.PutExtra(MediaStore.ExtraSizeLimit, size);
                }

                var quality = Intent.GetIntExtra(MediaStore.ExtraVideoQuality, 1);
                intent.PutExtra(MediaStore.ExtraVideoQuality, quality);

                // 启用前/后摄像头
                if (Intent.GetBooleanExtra(ExtraFront, false))
                {
                    intent.UseFrontCamera();
                }
                else
                {
                    intent.UseBackCamera();
                }

                // 照片或视频保存的文件
                _path = System.IO.Path.Combine(Kit.CachePath, Kit.NewGuid + (isPhoto ? ".jpg" : ".mp4"));
                var file = new Java.IO.File(_path);
                file.CreateNewFile();

                try
                {
                    // 将访问受限的 file:// URI 转化为可以授权共享的 content:// URI
                    // 已将Kit.CachePath共享，配置见：AndroidManifest.xml 和 Resources\xml\file_path.xml
                    var uri = FileProvider.GetUriForFile(this, _context.PackageName + ".fileprovider", file);
                    GrantUriPermissionsForIntent(intent, uri);
                    intent.AddFlags(ActivityFlags.GrantReadUriPermission);
                    intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
                    intent.PutExtra(MediaStore.ExtraOutput, uri);
                }
                catch (Java.Lang.IllegalArgumentException iae)
                {
                    // Using a Huawei device on pre-N. Increased likelihood of failure...
                    if (_huaweiManufacturer.Equals(Build.Manufacturer, StringComparison.CurrentCultureIgnoreCase) && (int)Build.VERSION.SdkInt < 24)
                    {
                        intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Unable to get file location, check and set manifest with file provider. Exception: {iae}");
                        throw new ArgumentException("Unable to get file location. This most likely means that the file provider information is not set in your Android Manifest file. Please check documentation on how to set this up in your project.", iae);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unable to get file location, check and set manifest with file provider. Exception: {ex}");
                    throw new ArgumentException("Unable to get file location. This most likely means that the file provider information is not set in your Android Manifest file. Please check documentation on how to set this up in your project.", ex);
                }

                StartActivityForResult(intent, 1337);
            }
            catch (Exception)
            {
                OnCaptured(null);
                //must finish here because an exception has occured else blank screen
                Finish();
            }
            finally
            {
                if (intent != null)
                    intent.Dispose();
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode != Result.Ok)
            {
                if (!string.IsNullOrEmpty(_path) && File.Exists(_path))
                {
                    try
                    {
                        File.Delete(_path);
                    }
                    catch { }
                }
                OnCaptured(null);
                Finish();
                return;
            }

            try
            {
                FileData fd = new FileData(_path, System.IO.Path.GetFileName(_path), (ulong)new Java.IO.File(_path).Length());
                string ext = fd.Ext;

                // 生成文件描述和缩略图
                if (ext == ".jpg")
                {
                    BitmapFactory.Options options = new BitmapFactory.Options();
                    // 只解析图片大小，不加载内容
                    options.InJustDecodeBounds = true;
                    BitmapFactory.DecodeFile(_path, options);
                    fd.Desc = $"{options.OutWidth} x {options.OutHeight} ({ext.TrimStart('.')})";

                    int maxSize = Math.Max(options.OutWidth, options.OutHeight);
                    if (maxSize > FileData.ThumbSize)
                    {
                        // 直接按缩放比例加载
                        options.InJustDecodeBounds = false;
                        options.InSampleSize = maxSize / FileData.ThumbSize;
                        // v29 弃用
                        //options.InPurgeable = true;
                        Bitmap bmp = BitmapFactory.DecodeFile(_path, options);

                        fd.ThumbPath = System.IO.Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                        using (var fs = System.IO.File.Create(fd.ThumbPath))
                        {
                            await bmp.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
                        }
                        bmp.Recycle();
                    }
                }
                else if (ext == ".mp4")
                {
                    Android.Media.MediaMetadataRetriever media = new Android.Media.MediaMetadataRetriever();
                    try
                    {
                        await media.SetDataSourceAsync(_path);
                        string dur = media.ExtractMetadata(Android.Media.MetadataKey.Duration);
                        string width = media.ExtractMetadata(Android.Media.MetadataKey.VideoWidth);
                        string height = media.ExtractMetadata(Android.Media.MetadataKey.VideoHeight);
                        fd.Desc = string.Format("{0:HH:mm:ss} ({1} x {2})", new DateTime(long.Parse(dur) * 10000), width, height);
                    }
                    catch { }
                    finally
                    {
                        media.Release();
                    }

                    // 帧缩略图
                    var bmp = await ThumbnailUtils.CreateVideoThumbnailAsync(_path, ThumbnailKind.MiniKind);
                    fd.ThumbPath = System.IO.Path.Combine(Kit.CachePath, Kit.NewGuid + "-t.jpg");
                    using (var fs = System.IO.File.Create(fd.ThumbPath))
                    {
                        await bmp.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Jpeg, 100, fs);
                    }
                    bmp.Recycle();
                }
                OnCaptured(fd);
            }
            catch
            {
                OnCaptured(null);
            }
            finally
            {
                Finish();
            }
        }

        void GrantUriPermissionsForIntent(Intent intent, Android.Net.Uri uri)
        {
            var resInfoList = PackageManager.QueryIntentActivities(intent, PackageInfoFlags.MatchDefaultOnly);
            foreach (var resolveInfo in resInfoList)
            {
                var packageName = resolveInfo.ActivityInfo.PackageName;
                GrantUriPermission(packageName, uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            }
        }

        internal static event EventHandler<FileData> Captured;

        static void OnCaptured(FileData args)
        {
            Captured?.Invoke(null, args);
        }
    }

    static class IntentExtraExtensions
    {
        const string extraFrontPre25 = "android.intent.extras.CAMERA_FACING";
        const string extraFrontPost25 = "android.intent.extras.LENS_FACING_FRONT";
        const string extraBackPost25 = "android.intent.extras.LENS_FACING_BACK";
        const string extraUserFront = "android.intent.extra.USE_FRONT_CAMERA";

        public static void UseFrontCamera(this Intent intent)
        {
            // Android before API 25 (7.1)
            intent.PutExtra(extraFrontPre25, (int)CameraFacing.Front);

            // Android API 25 and up
            intent.PutExtra(extraFrontPost25, 1);

            intent.PutExtra(extraUserFront, true);
        }

        public static void UseBackCamera(this Intent intent)
        {
            // Android before API 25 (7.1)
            intent.PutExtra(extraFrontPre25, (int)CameraFacing.Back);

            // Android API 25 and up
            intent.PutExtra(extraBackPost25, 1);

            intent.PutExtra(extraUserFront, false);
        }
    }
}
#endif