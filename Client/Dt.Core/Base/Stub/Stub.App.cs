#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 重写Application方法
    /// </summary>
    public abstract partial class Stub
    {
        /// <summary>
        /// Application.OnLaunched
        /// </summary>
        /// <param name="p_args"></param>
        public abstract void OnLaunched(LaunchActivatedEventArgs p_args);


#if IOS
        public virtual void OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options) { }

        public virtual void FinishedLaunching(UIApplication p_app, NSDictionary p_options) { }

        public virtual void PerformFetch(UIApplication p_app, Action<UIBackgroundFetchResult> p_completionHandler) { }

        public virtual void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification) { }
#endif
    }
}