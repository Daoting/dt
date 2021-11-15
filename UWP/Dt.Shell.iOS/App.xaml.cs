#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-04
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using Foundation;
using System;
using UIKit;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Shell
{
    sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _ = Startup.Launch<Stub>(p_args.Arguments, null);
        }

        public override bool OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            var doc = new UIDocument(p_url);
            string path = doc.FileUrl?.Path;
            if (!string.IsNullOrEmpty(path))
                _ = Startup.Launch<Stub>(null, new ShareInfo(path));

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

        public async override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            Console.WriteLine("启动后台任务");
            try
            {
                var iOSTaskId = UIApplication.SharedApplication.BeginBackgroundTask(() => { Console.WriteLine("后台运行超时"); });
                await BgJob.Run(new Stub());
                UIApplication.SharedApplication.EndBackgroundTask(iOSTaskId);
            }
            catch
            { }

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
                    _ = Startup.Launch<Stub>(val, null);
                }
            }

            // 桌面图标的提醒数字
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}