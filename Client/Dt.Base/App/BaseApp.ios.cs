#if IOS
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-12-14 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core;
using Foundation;
using System;
using UIKit;
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认的Application行为
    /// </summary>
    public abstract class BaseApp : Application
    {
        /// <summary>
        /// 存根
        /// </summary>
        protected Stub _stub;

        public BaseApp()
        {
#if DEBUG
            // 初始化uno平台全局日志
            UnoKit.InitializeLogging();
#endif
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _ = Startup.Launch(_stub, p_args.Arguments);
        }

        public override bool OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            var doc = new UIDocument(p_url);
            string path = doc.FileUrl?.Path;
            if (!string.IsNullOrEmpty(path))
                _ = Startup.Launch(_stub, null, new ShareInfo(path));

            return true;
        }

        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // 设置 Background Fetch 最小时间间隔，10-15分钟不定
            application.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            if (application.CurrentUserNotificationSettings.Types == UIUserNotificationType.None)
            {
                // 注册本地通知
                var ns = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                application.RegisterUserNotificationSettings(ns);
            }
            return base.FinishedLaunching(application, launchOptions);
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            try
            {
                // 该方法耗时必须在30秒内！
                BgJob.Run().Wait();
            }
            catch { }

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // 点击本地通知自定义启动
            if (notification.UserInfo.ContainsKey(NSObject.FromObject(BgJob.ToastStart)))
            {
                var val = notification.UserInfo[NSObject.FromObject(BgJob.ToastStart)].ToString();
                if (!string.IsNullOrEmpty(val))
                {
                    // app完全退出后点击通知启动时不调用此方法！！！
                    // 因此 OnLaunched 和 ReceivedLocalNotification 方法只调用一个！
                    _ = Startup.Launch(_stub, val);
                }
            }

            // 桌面图标的提醒数字
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}
#endif