#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
#if IOS
using UIKit;
using Foundation;
#endif
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public abstract partial class Stub
    {
        public Stub()
        {
            Inst = this;
            var svcs = new ServiceCollection();
            svcs.AddSingleton<ITypeAlias, DefTypeAlias>();
            ConfigureServices(svcs);
            Kit.Inject(svcs.BuildServiceProvider());
        }
        
        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected virtual void ConfigureServices(IServiceCollection p_svcs) { }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected virtual Task OnStartup() { return Task.CompletedTask; }

        /// <summary>
        /// Application.OnLaunched
        /// </summary>
        /// <param name="p_args"></param>
        public abstract Task OnLaunched(LaunchActivatedEventArgs p_args);

        /// <summary>
        /// 内部访问存根实例
        /// </summary>
        internal static Stub Inst { get; private set; }
        
#if IOS
        public virtual void OpenUrl(UIApplication p_app, Foundation.NSUrl p_url, Foundation.NSDictionary p_options) { }

        public virtual void ReceivedLocalNotification(UIApplication p_app, UILocalNotification p_notification) { }
#endif
    }
}