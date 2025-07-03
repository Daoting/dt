#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-12-27
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Microsoft.UI.Xaml;
using SkiaSharp;
using Windows.Storage;
#if IOS
using UIKit;
using Foundation;
#endif
#endregion

namespace Dt.Base
{
    /// <summary>
    /// Application基类
    /// </summary>
    public abstract class AppBase : Application
    {
        readonly Stub _stub;

        public AppBase()
        {
            _stub = NewStub();
        }

        protected abstract Stub NewStub();

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
#if !WIN
            // 非WinAppSdk平台统一Skia渲染：

            // Skia渲染时默认true，和WinUI一致Frame不保存旧页面。为提高返回时的性能，设置为false
            // https://platform.uno/docs/articles/controls/Frame.html
            Uno.UI.FeatureConfiguration.Frame.UseWinUIBehavior = false;

            // Skia渲染时HarmonyOS Sans字体作为默认字体，开源字体无版权问题，在构造方法设置对wasm无效
            Uno.UI.FeatureConfiguration.Font.DefaultTextFontFamily = "ms-appx:///Assets/Fonts/HarmonySans.ttf";
#endif
            
            _stub.OnLaunched(p_args);
        }

        /// <summary>
        /// 处理wpf的DispatcherUnhandledException事件
        /// </summary>
        /// <param name="ex"></param>
        public void OnUnhandledException(Exception ex)
        {
            Kit.OnUnhandledException(ex);
        }

        //#if IOS
        //        public override bool OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        //        {
        //            _stub.OpenUrl(p_app, p_url, p_options);
        //            return true;
        //        }

        //        public override void DidEnterBackground(UIApplication application)
        //        {
        //            BgJob.OnEnterBackground();
        //        }

        //        public override void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification)
        //        {
        //            _stub.ReceivedLocalNotification(p_app, p_notification);
        //        }
        //#endif
    }
}