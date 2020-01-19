using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using System;

namespace App.Droid
{
    [Activity(
        Icon = "@drawable/icon",
        Label = "@string/AppName",
        Theme = "@style/AppTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize,
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden
    )]
    public class MainActivity : Windows.UI.Xaml.ApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // 状态栏高度
            int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
                Windows.UI.Xaml.Application.Current.Resources["StatusBarHeight"] = (int)(Resources.GetDimensionPixelSize(resourceId) / Resources.DisplayMetrics.Density);
        }
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

