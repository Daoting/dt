#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2017-12-06 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
#endregion


namespace Infras.Demo.Droid
{
    [Activity(
        MainLauncher = true,
        // 横竖屏模式改变、屏幕大小变化、键盘可用性改变时不重新启动activity
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
    {
    }

    [Application(LargeHeap = true, HardwareAccelerated = true)]
    public class Application : Microsoft.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new Infras.Demo.App(), javaReference, transfer)
        {
        }
    }
}