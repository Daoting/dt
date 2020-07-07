using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;

namespace App.Droid
{
    [Activity(
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
    )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        // styles.xml 中已设置不占用顶部状态栏和底部导航栏，windowTranslucentStatus windowTranslucentNavigation
    }


    [global::Android.App.ApplicationAttribute(LargeHeap = true, HardwareAccelerated = true)]
    public class Application : Windows.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(new Dt.Shell.App(), javaReference, transfer)
        {
        }
    }
}

