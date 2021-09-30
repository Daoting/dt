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
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
#if __IOS__
using UIKit;
#endif
#endregion

namespace Dt.Shell
{
    sealed partial class App : Application
    {
        string _params;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            string args = string.IsNullOrEmpty(_params) ? p_args.Arguments : _params;
            _ = Startup.Launch<Stub>(args, null);
            _params = null;
        }

#if UWP
        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs p_args)
        {
            var info = new ShareInfo();
            await info.Init(p_args.ShareOperation);
            await Startup.Launch<Stub>(null, info);
        }
#elif __IOS__
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
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // 注册本地通知
                var ns = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null);
                application.RegisterUserNotificationSettings(ns);
            }
            return base.FinishedLaunching(application, launchOptions);
        }

        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
            // 点击通知自定义启动，app完全退出后点击通知启动时不调用此方法！！！
            if (notification.UserInfo.ContainsKey(NSObject.FromObject(BgJob.ToastStart)))
            {
                var val = notification.UserInfo[NSObject.FromObject(BgJob.ToastStart)].ToString();
                if (!string.IsNullOrEmpty(val))
                {
                    if (Kit.Stub != null)
                    {
                        // 非null表示app已启动过，不会再调用 OnLaunched
                        _ = Startup.Launch<Stub>(val, null);
                    }
                    else
                    {
                        // 未启动，记录参数提供给 OnLaunched
                        _params = val;
                    }
                }
            }

            // 桌面图标的提醒数字
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }

        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            base.PerformFetch(application, completionHandler);
            Kit.Toast("标题", DateTime.Now.ToString(), new AutoStartInfo { WinType = typeof(Sample.LvHome).AssemblyQualifiedName, Title = "列表" });
            completionHandler(UIBackgroundFetchResult.NewData);
        }
#elif ANDROID
        public async void ReceiveShare(ShareInfo p_shareInfo)
        {
            await Startup.Launch<Stub>(null, p_shareInfo);
        }

        public void ToastStart(string p_params)
        {
            if (Kit.Stub != null)
            {
                // 非null表示app已启动过，不会再调用 OnLaunched
                _ = Startup.Launch<Stub>(p_params, null);
            }
            else
            {
                // 未启动，记录参数提供给 OnLaunched
                _params = p_params;
            }
        }
#endif
    }
}