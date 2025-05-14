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
        // 横竖屏模式改变、屏幕大小变化、键盘可用性改变时不重新启动activity
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : BaseAppActivity
    {
    }

    [Application(LargeHeap = true, HardwareAccelerated = true)]
    public class MainApplication : Microsoft.UI.Xaml.NativeApplication
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
        }
    }
}