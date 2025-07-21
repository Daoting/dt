#if WIN
#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppLifecycle;
using Microsoft.Windows.AppNotifications;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 默认存根
    /// </summary>
    public partial class DefaultStub : Stub
    {
        public DefaultStub()
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }
        
        public override async Task OnLaunched(LaunchActivatedEventArgs p_args)
        {
            try
            {
                // 当用户点击Toast通知时，注册当前应用接收通知，在 NotificationInvoked 中处理通知，不注册会另外启动新应用实例！
                // Package.appxmanifest中定义的Executable切记不加路径，否则无法在应用完全关闭时点击Toast启动应用！
                // 初次运行时报 “未发现元素” 异常，无影响
                AppNotificationManager nm = AppNotificationManager.Default;
                nm.NotificationInvoked += OnNotificationInvoked;
                nm.Register();
            }
            catch { }
            
            var actArgs = AppInstance.GetCurrent().GetActivatedEventArgs();
            if (actArgs.Kind != ExtendedActivationKind.AppNotification)
            {
                // 正常启动
                await Launch();
            }
            else
            {
                // Toast启动
                var args = (AppNotificationActivatedEventArgs)actArgs.Data;
                var dq = DispatcherQueue.GetForCurrentThread();
                if (dq != null)
                {
                    // 应用完全关闭时点击Toast启动应用
                    dq.TryEnqueue(async () => await Launch(args.Argument));
                }
                else
                {
                    await Launch(args.Argument);
                }
            }
        }

        void OnNotificationInvoked(AppNotificationManager sender, AppNotificationActivatedEventArgs args)
        {
            // 应用已打开接收Toast通知
            Kit.RunAsync(() => _ = Launch(args.Argument));
        }
        
        void OnProcessExit(object sender, EventArgs e)
        {
            try
            {
                AppNotificationManager.Default.Unregister();
            }
            catch { }
        }
    }
}
#endif