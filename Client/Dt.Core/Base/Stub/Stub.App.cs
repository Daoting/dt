#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
#if IOS
using UIKit;
using Foundation;
#endif
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

        /// <summary>
        /// 切换Stub新实例重启
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Task Reboot<T>()
            where T : Stub
        {
            Inst = null;
            var stub = Activator.CreateInstance<T>();
            return stub.OnReboot();
        }

        /// <summary>
        /// Stub新实例启动
        /// </summary>
        /// <returns></returns>
        protected abstract Task OnReboot();

#if IOS
        public virtual void OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options) { }

        public virtual void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification) { }
#endif
    }
}