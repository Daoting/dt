﻿#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#if IOS
using UIKit;
using Foundation;
#endif
#endregion

namespace $ext_safeprojectname$
{
    public partial class App : Application
    {
        readonly Stub _stub;

        public App()
        {
            InitializeComponent();
            _stub = new AppStub();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            _stub.OnLaunched(p_args);
        }

#if IOS
        public override bool OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            _stub.OpenUrl(p_app, p_url, p_options);
            return true;
        }

        public override bool FinishedLaunching(UIApplication p_app, NSDictionary p_options)
        {
            _stub.FinishedLaunching(p_app, p_options);
            return base.FinishedLaunching(p_app, p_options);
        }

        public override void PerformFetch(UIApplication p_app, Action<UIBackgroundFetchResult> p_completionHandler)
        {
            _stub.PerformFetch(p_app, p_completionHandler);
        }

        public override void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification)
        {
            _stub.ReceivedLocalNotification(p_app, p_notification);
        }
#endif
    }
}