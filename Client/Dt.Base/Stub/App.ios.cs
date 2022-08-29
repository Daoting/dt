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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using UIKit;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认存根
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        public DefaultStub()
        {
            UnoKit.Init();
        }

        public override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _ = Launch(p_args.Arguments);
        }

        public override void OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            var doc = new UIDocument(p_url);
            string path = doc.FileUrl?.Path;
            if (!string.IsNullOrEmpty(path))
                _ = Launch(null, new ShareInfo(path));
        }

        public override void FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // 设置 Background Fetch 最小时间间隔，10-15分钟不定
            application.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);
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
                    _ = Launch(val);
                }
            }

            // 桌面图标的提醒数字
            UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
        }
    }
}
#endif