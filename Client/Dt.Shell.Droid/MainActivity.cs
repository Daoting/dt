using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Dt.Core;
using Dt.Base;

namespace App.Droid
{
    // 接收分享的内容
    [IntentFilter(
        new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeTypes = new[] { "image/*", "text/plain", "video/*", "audio/*", "*/*" })]
    [Activity(
        MainLauncher = true,
        // 横竖屏模式改变、屏幕大小变化、键盘可用性改变时不重新启动activity
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden),]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        // styles.xml 中已设置不占用顶部状态栏和底部导航栏，windowTranslucentStatus windowTranslucentNavigation

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // 确保 Permissions.RequestAsync 调用时正常
            Xamarin.Essentials.Platform.Init(this, bundle);

            // 接收分享内容
            var it = Intent;
            if (it.Action == Intent.ActionSend && it.Type != null)
            {
                ShareInfo info = new ShareInfo();
                string tp = it.Type;

                if (tp.StartsWith("text/"))
                {
                    info.DataType = ShareDataType.Text;
                    info.Content = it.GetStringExtra(Intent.ExtraText);
                }
                else
                {
                    if (tp.StartsWith("image/"))
                    {
                        info.DataType = ShareDataType.Image;
                    }
                    else if (tp.StartsWith("video/"))
                    {
                        info.DataType = ShareDataType.Video;
                    }
                    else if (tp.StartsWith("audio/"))
                    {
                        info.DataType = ShareDataType.Audio;
                    }
                    else
                    {
                        info.DataType = ShareDataType.File;
                    }

                    Android.Net.Uri uri = (Android.Net.Uri)it.GetParcelableExtra(Intent.ExtraStream);
                    var path = IOUtil.GetPath(Android.App.Application.Context, uri);
                    // 通过FileProvider分享时无法获取路径但可读取文件内容，参见ShareInfo.GetStream
                    if (string.IsNullOrEmpty(path))
                        path = uri.ToString();
                    info.FilePath = path;
                }
                ((Dt.Shell.App)Windows.UI.Xaml.Application.Current).ReceiveShare(info);
            }
        }
    }


    [global::Android.App.ApplicationAttribute(LargeHeap = true, HardwareAccelerated = true)]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new Dt.Shell.App(), javaReference, transfer)
        {
        }
    }
}

