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
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
#endregion

namespace Infras.Demo
{
    [Activity(
        MainLauncher = true,
        // 横竖屏模式改变、屏幕大小变化等所有改变都不重新启动activity
        ConfigurationChanges = global::Uno.UI.ActivityHelper.AllConfigChanges,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustNothing | SoftInput.StateHidden
    )]
    public class MainActivity : Microsoft.UI.Xaml.ApplicationActivity
    {
    }

    [Application(
       Label = "@string/ApplicationName",
       Icon = "@mipmap/icon",
       LargeHeap = true,
       HardwareAccelerated = true,
       Theme = "@style/AppTheme"
   )]
    public class Application : Microsoft.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new Infras.Demo.App(), javaReference, transfer)
        {
        }
    }
}

