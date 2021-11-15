#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Work;
using Dt.Base;
using Dt.Core;
using System;
#endregion

namespace Dt.Shell.Droid
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
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        // styles.xml 中已设置不占用顶部状态栏和底部导航栏，windowTranslucentStatus windowTranslucentNavigation

        // 启动调用顺序：Application.OnCreate -> MainActivity.MainActivity -> App.OnLaunched

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            var it = Intent;
            switch (it.Action)
            {
                case Intent.ActionMain:
                    // 普通启动，不传递参数
                    // 注册后台服务放在 App.OnLaunched，初始化结束后再注册！
                    break;

                case Intent.ActionSend:
                    // 接收分享内容
                    if (it.Type != null)
                        ReceiveShare();
                    break;

                case BgJob.ActionToast:
                    // 点击通知栏后，接收传递参数
                    var startInfo = it.GetStringExtra(BgJob.ActionToast);
                    if (!string.IsNullOrEmpty(startInfo))
                        ((Dt.Shell.App)Windows.UI.Xaml.Application.Current).ToastStart(startInfo);
                    break;
            }
        }

        void ReceiveShare()
        {
            var it = Intent;
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

    [Application(LargeHeap = true, HardwareAccelerated = true)]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new Dt.Shell.App(), javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            // 确保 Permissions.RequestAsync 调用时正常
            Xamarin.Essentials.Platform.Init(this);
        }
    }

    public class PluginWorker : Worker
    {
        public PluginWorker(Context context, WorkerParameters workerParameters)
            : base(context, workerParameters)
        {
        }

        public override Result DoWork()
        {
            try
            {
                BgJob.DoWork(new Stub(), typeof(MainActivity)).Wait();
            }
            catch { }
            return Result.InvokeSuccess();
        }
    }
}