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
using Android.Runtime;
using Android.Views;
using Android.OS;
using Android.Widget;
using Dt.Base;
using System;
#endregion

namespace Demo
{
    // 接收分享的内容
    [IntentFilter(
        new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeTypes = new[] { "image/*", "text/plain", "video/*", "audio/*", "*/*" })]

    [Activity(
        MainLauncher = true,
        // 横竖屏模式改变、屏幕大小变化、键盘可用性等所有改变时不重新启动activity
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : BaseAppActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            global::AndroidX.Core.SplashScreen.SplashScreen.InstallSplashScreen(this);

            base.OnCreate(savedInstanceState);
        }
    }

    [Application(
        Label = "@string/ApplicationName",
        Icon = "@mipmap/icon",
        LargeHeap = true,
        HardwareAccelerated = true,
        Theme = "@style/Theme.App.Starting"
    )]
    public class MainApplication : Microsoft.UI.Xaml.NativeApplication
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
        }
    }
}