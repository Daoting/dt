#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-04
******************************************************************************/
#endregion

#region 引用命名
using System;
using Dt.Base;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Infras.Demo
{
    sealed partial class App : Microsoft.UI.Xaml.Application
    {
        public App()
        {
            InitializeComponent();

#if !WIN && !WASM
            global::Uno.UI.FeatureConfiguration.Font.DefaultTextFontFamily = "ms-appx:///Assets/Fonts/HarmonySans.ttf#HarmonyOS Sans";
#endif
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            Window win;
#if WIN
            win = new Window();
#else
            win = Microsoft.UI.Xaml.Window.Current;
#endif
            
            var rootFrame = win.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
#if IOS
                rootFrame.Padding = new Thickness(0, (int)UIKit.UIApplication.SharedApplication.StatusBarFrame.Height, 0, 0);
#elif ANDROID
                var res = Android.App.Application.Context.Resources;
                int resourceId = res.GetIdentifier("status_bar_height", "dimen", "android");
                if (resourceId > 0)
                {
                    var StatusBarHeight = (int)(res.GetDimensionPixelSize(resourceId) / res.DisplayMetrics.Density);
                    rootFrame.Padding = new Thickness(0, StatusBarHeight, 0, 0);
                }
#endif
                win.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(HomePage));
            }
            win.Activate();
            ExcelKit.MainWin = win;
        }
    }
}