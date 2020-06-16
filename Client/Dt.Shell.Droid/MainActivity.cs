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
        // 不再需要计算状态栏高度
        // styles.xml 中已设置不占用顶部状态栏和底部导航栏：windowTranslucentStatus, windowTranslucentNavigation

        //protected override void OnCreate(Bundle bundle)
        //{
        //    base.OnCreate(bundle);

        //    // 状态栏高度
        //    int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
        //    if (resourceId > 0)
        //        Windows.UI.Xaml.Application.Current.Resources["StatusBarHeight"] = (int)(Resources.GetDimensionPixelSize(resourceId) / Resources.DisplayMetrics.Density);
        //}
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

