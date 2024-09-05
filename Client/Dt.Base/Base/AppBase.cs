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
        
#if IOS
        public override bool OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options)
        {
            _stub.OpenUrl(p_app, p_url, p_options);
            return true;
        }

        public override void DidEnterBackground(UIApplication application)
        {
            BgJob.OnEnterBackground();
        }

        public override void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification)
        {
            _stub.ReceivedLocalNotification(p_app, p_notification);
        }
#endif
    }
}