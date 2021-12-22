#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
#endregion

namespace $ext_safeprojectname$
{
    // 接收分享的内容
    //[IntentFilter(
    //    new[] { Intent.ActionSend },
    //    Categories = new[] { Intent.CategoryDefault },
    //    DataMimeTypes = new[] { "image/*", "text/plain", "video/*", "audio/*", "*/*" })]

    [Activity(
        MainLauncher = true,
        // 横竖屏模式改变、屏幕大小变化、键盘可用性改变时不重新启动activity
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden,
        // 控制软键盘
        WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.StateHidden)]
    public class MainActivity : BaseAppActivity
    {
        // styles.xml 中已设置不占用顶部状态栏和底部导航栏，windowTranslucentStatus windowTranslucentNavigation
    }

    [Application(LargeHeap = true, HardwareAccelerated = true)]
    public class Application : Microsoft.UI.Xaml.NativeApplication
    {
        public Application(IntPtr javaReference, JniHandleOwnership transfer)
            : base(() => new App(), javaReference, transfer)
        {
        }
    }
}