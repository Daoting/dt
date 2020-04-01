#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 为支持Xamarin将实现部分转移到IPlatform
* 日志: 2017-12-29 改名
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统相关管理类
    /// </summary>
    public static class AtSys
    {
        #region 成员变量
        static TimeSpan _timeSpan;
        #endregion

        #region 系统信息
        /// <summary>
        /// 获取平台种类
        /// </summary>
        public static TargetSystem System
        {
            get
            {
#if UWP
                return TargetSystem.Windows;
#elif IOS
                return TargetSystem.iOS;
#elif ANDROID
                return TargetSystem.Android;
#elif WASM
                return TargetSystem.Web;
#endif
            }
        }

        /// <summary>
        /// 获取系统是否采用手机的UI模式
        /// </summary>
#if IOS || ANDROID
        public static bool IsPhoneUI { get { return true; } }
#else
        public static bool IsPhoneUI { get; internal set; }
#endif

        /// <summary>
        /// 获取系统是否为触摸模式
        /// </summary>
        public static bool IsTouchMode
        {
            get
            {
#if UWP
                return UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// 获取服务器端当前时间，根据时差计算所得
        /// </summary>
        public static DateTime Now
        {
            get { return DateTime.Now + _timeSpan; }
        }

        /// <summary>
        /// 获取系统存根
        /// </summary>
        public static IStub Stub { get; private set; }

        /// <summary>
        /// 获取设置是否监控Rpc调用结果，TraceBox中控制输出
        /// </summary>
        internal static bool TraceRpc { get; set; }
        #endregion

        #region 登录注销
        /// <summary>
        /// 显示登录页面，参数：是否为弹出式
        /// </summary>
        public static Action<bool> Login { get; internal set; }

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        public static Action Logout { get; internal set; }
        #endregion

        #region 平台方法
        /// <summary>
        /// 系统初始化
        /// </summary>
        /// <param name="p_stub">系统存根</param>
        public static void Startup(IStub p_stub)
        {
            Stub = p_stub;
            if (Stub.SerializeTypes != null)
                SerializeTypeAlias.Merge(Stub.SerializeTypes);

            Application app = Application.Current;
            app.Suspending += OnSuspending;
            app.Resuming += OnResuming;
            app.UnhandledException += OnUnhandledException;

            // 打开状态库
            AtLocal.OpenStateDb();
        }

        /// <summary>
        /// 同步服务器时间
        /// </summary>
        /// <param name="p_cfg"></param>
        internal static void SyncTime(DateTime p_serverTime)
        {
            // 与服务器时差
            _timeSpan = p_serverTime - DateTime.Now;
        }
        #endregion

        #region App事件方法
        /// <summary>
        /// 挂起时的回调
        /// </summary>
        internal static Func<Task> Suspending { get; set; }

        /// <summary>
        /// 恢复时的回调
        /// </summary>
        internal static Action Resuming { get; set; }

        /// <summary>
        /// 三平台都能正常触发！必须耗时小！
        /// Running -> Suspended    手机或PC平板模式下不占据屏幕时触发
        /// 此时不知道应用程序将被终止还是可恢复
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        static async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                await Suspending();
                await Stub.OnSuspending();
            }
            catch { }
            deferral.Complete();
        }

        /// <summary>
        /// 三平台都能正常触发！
        /// Suspended -> Running    手机或PC平板模式下再次占据屏幕时触发
        /// 执行恢复状态、恢复会话等操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnResuming(object sender, object e)
        {
            try
            {
                Resuming();
                Stub.OnResuming();
            }
            catch { }
        }

        /// <summary>
        /// 未处理异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            try
            {
                FriendlyException friendlyEx = e.Exception as FriendlyException;
                if (friendlyEx != null)
                {
                    // 输出到监视窗口
                    string msg = friendlyEx.Message;
                    string title;
                    int index = msg.IndexOf("\r\n");
                    if (index > 0)
                        title = msg.Substring(0, index);
                    else
                        title = msg.Length > 20 ? msg.Substring(0, 20) : msg;
                    AtKit.Trace(TraceOutType.Normal, title, msg);
                    // 显示Assert警告对话框
                    AtKit.RunAsync(async () =>
                    {
                        await new MessageDialog(msg, "警告").ShowAsync();
                    });
                }
                else
                {
                    // 输出到监视窗口
                    string msg = e.Message;
                    string title;
                    int index = msg.IndexOf("\r\n");
                    if (index > 0)
                        title = msg.Substring(0, index);
                    else
                        title = msg.Length > 20 ? msg.Substring(0, 20) : msg;
                    AtKit.Trace(TraceOutType.UnhandledException, title, e.Message);
                    // 保存日志
                    Log.Error("未处理异常：" + e.Message);
                    // 显示异常错误对话框
                    AtKit.RunAsync(async () =>
                    {
                        MessageDialog dlg = new MessageDialog(e.Message, "程序异常");
                        dlg.Commands.Add(new UICommand("终止", new UICommandInvokedHandler(async (cmd) =>
                        {
                            try
                            {
                                await Suspending();
                                await Stub.OnSuspending();
                            }
                            catch { }

                            // 调用Exit直接关闭时不走Suspending事件
                            // 以此方式关闭应用时，系统会将此视为应用崩溃，变态！！！
                            Application.Current.Exit();
                        })));
                        dlg.Commands.Add(new UICommand("继续", null));
                        dlg.DefaultCommandIndex = 1;
                        dlg.CancelCommandIndex = 1;
                        await dlg.ShowAsync();
                    });
                }
            }
            catch (Exception ex)
            {
                AtKit.Trace(TraceOutType.UnhandledException, "未处理异常", ex.Message);
            }
            finally
            {
                e.Handled = true;
            }
        }
        #endregion
    }
}