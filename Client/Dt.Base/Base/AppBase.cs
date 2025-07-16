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
#if !WIN
using Microsoft.Extensions.Logging;
#endif
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
            /* 异常处理，参见 https://github.com/Daoting/dt/issues/1
            未处理异常发生的位置有4种：
            主线程同步方法、主线程异步方法、Task内部同步方法、Task内部异步方法
            
            对于以上4种未处理异常：
            1. WinAppSdk V1.2 都能触发未处理异常事件，已完美解决崩溃问题，v1.7主线程异步异常会崩溃
            2. Skia渲染时都不触发未处理异常事件，被uno 或 .net内部拦截处理，但都不会崩溃！调试时输出错误日志
            
            KnownException是业务异常，阻止业务继续时通过Throw类抛出，为了能统一显示警告信息，只能在抛出KnownException异常前显示！

            总结：所有平台都不会因为异常而崩溃，对于不是通过Throw类抛出的异常，非WinAppSdk无法给出警告提示！
            */
#if WIN
            UnhandledException += OnUnhandledException;
#elif DEBUG
            // 影响性能，正式版不启用
            InitExceptionLog();
#endif
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

            // Skia渲染时HarmonyOS Sans字体作为默认字体，开源字体无版权问题，在构造方法设置对wasm无效！
            // uno通过 HarmonySans.ttf.manifest 获取粗体、斜体等样式，wasm无需在css中设置字体
            Uno.UI.FeatureConfiguration.Font.DefaultTextFontFamily = "ms-appx:///Assets/Fonts/HarmonySans.ttf";
#endif
            _stub.OnLaunched(p_args);
        }

        #region 异常处理
        static void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            // 不处理已知异常，已在抛出异常前警告(Throw类)，不输出日志
            if (e.Exception is KnownException || e.Exception.InnerException is KnownException)
                return;

            string title = "";
            try
            {
                if (e.Exception is ServerException se)
                    title = se.Title;
                else
                    title = $"未处理异常：{e.Exception.GetType().FullName}";

                // 警告、保存日志
                var notify = new NotifyInfo
                {
                    NotifyType = NotifyType.Warning,
                    Message = title,
                    Delay = 5,
                    Link = "查看详细",
                };
                notify.LinkCallback = (e) =>
                {
                    Kit.ShowLogBox();
                    Kit.CloseNotify(notify);
                };
                Kit.Notify(notify);
            }
            catch { }
            finally
            {
                // ServerException日志已输出
                if (e.Exception is not ServerException)
                    Log.Error(e.Exception, title);
            }
        }

#if !WIN
        static void InitExceptionLog()
        {
            var factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
                builder.AddConsole();
#else
                builder.AddConsole();
#endif

                builder.SetMinimumLevel(LogLevel.Warning);

                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);
            });

            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;

#if HAS_UNO
            global::Uno.UI.Adapter.Microsoft.Extensions.Logging.LoggingAdapter.Initialize();
#endif
        }
#endif
        #endregion

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