#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 为支持Xamarin将实现部分转移到IPlatform
* 日志: 2017-12-29 改名
******************************************************************************/
#endregion

#region 引用命名
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;
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
        static readonly Dictionary<string, MethodInfo> _serviceMethods = new Dictionary<string, MethodInfo>();
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
            if (!Stub.IsLocalMode)
                BaseRpc.Init(Stub.ServerUrl);

            Application app = Application.Current;
            app.Suspending += OnSuspending;
            app.Resuming += OnResuming;
            app.UnhandledException += OnUnhandledException;

            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            SQLitePCL.Batteries_V2.Init();
            // 打开状态库
            AtLocal.OpenStateDb();
        }

        /// <summary>
        /// 关闭系统
        /// </summary>
        public static async void ShutDown()
        {
            try
            {
                // 外部系统关闭处理
                await Stub.ShutDown();
            }
            catch { }

            // 调用Exit直接关闭时不走Suspending事件
            // 以此方式关闭应用时，系统会将此视为应用崩溃，变态！！！
            Application.Current.Exit();
        }
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
        /// 全局注销方法
        /// </summary>
        public static Action Logout { get; internal set; }

        /// <summary>
        /// 本地sqlite文件路径
        /// </summary>
        public static string LocalDbPath
        {
            get
            {
#if UWP
                return ApplicationData.Current.LocalFolder.Path;
#else
                return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
#endif
            }
        }

        /// <summary>
        /// 获取设置是否监控Rpc调用结果，TraceBox中控制输出
        /// </summary>
        internal static bool TraceRpc { get; set; }
        #endregion

        #region 存根
        /// <summary>
        /// 获取视图类型
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回类型</returns>
        public static Type GetViewType(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && Stub.ViewTypes.TryGetValue(p_typeName, out tp))
                return tp;
            return null;
        }

        /// <summary>
        /// 是否存在指定视图
        /// </summary>
        /// <param name="p_viewName">视图名称</param>
        /// <returns></returns>
        public static bool IsExistView(string p_viewName)
        {
            return Stub.ViewTypes.ContainsKey(p_viewName);
        }

        /// <summary>
        /// 获取客户端服务方法
        /// </summary>
        /// <param name="p_name"></param>
        /// <returns></returns>
        public static MethodInfo GetServiceMethod(string p_name)
        {
            if (string.IsNullOrEmpty(p_name))
                return null;

            MethodInfo mi;
            if (_serviceMethods.TryGetValue(p_name, out mi))
                return mi;

            Type tp;
            string[] arr = p_name.Split('.');
            if (arr.Length == 2 && Stub.ServiceTypes.TryGetValue(arr[0], out tp))
            {
                mi = tp.GetMethod(arr[1]);
                if (mi != null)
                    _serviceMethods[p_name] = mi;
                return mi;
            }
            return null;
        }

        /// <summary>
        /// 创建流程表单实例
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回实例对象</returns>
        public static object CreateWfForm(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && Stub.FormTypes.TryGetValue(p_typeName, out tp))
                return Activator.CreateInstance(tp);
            return null;
        }

        /// <summary>
        /// 创建FormGrid自定义查找实例对象
        /// </summary>
        /// <param name="p_typeName">类型名称</param>
        /// <returns>返回实例对象</returns>
        public static object CreateWfSheet(string p_typeName)
        {
            Type tp;
            if (!string.IsNullOrEmpty(p_typeName) && Stub.SheetTypes.TryGetValue(p_typeName, out tp))
                return Activator.CreateInstance(tp);
            return null;
        }

        /// <summary>
        /// 获取已加载程序的描述信息
        /// </summary>
        /// <returns>描述信息字符串</returns>
        public static string GetAssemblyInfo()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("▶ 视图类型（{0}）\r\n", Stub.ViewTypes.Count);
            foreach (string item in Stub.ViewTypes.Keys)
            {
                sb.AppendLine(item);
            }
            sb.AppendLine();

            sb.AppendFormat("▶ 流程表单（{0}）\r\n", Stub.FormTypes.Count);
            foreach (string item in Stub.FormTypes.Keys)
            {
                sb.AppendLine(item);
            }
            sb.AppendLine();

            sb.AppendFormat("▶ 流程列表（{0}）\r\n", Stub.SheetTypes.Count);
            foreach (string item in Stub.SheetTypes.Keys)
            {
                sb.AppendLine(item);
            }
            sb.AppendLine();

            sb.AppendFormat("▶ 服务类型（{0}）\r\n", Stub.ServiceTypes.Count);
            foreach (string item in Stub.ServiceTypes.Keys)
            {
                sb.AppendLine(item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取状态库版本号，和本地不同时自动更新
        /// </summary>
        public static string StateDbVer
        {
            get { return Stub.StateDbVer; }
        }

        /// <summary>
        /// 获取状态库表类型
        /// </summary>
        public static Dictionary<string, Type> StateTbls
        {
            get { return Stub.StateTbls; }
        }
        #endregion

        #region 内部方法
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
        /// Running -> Suspended    手机或PC平板模式下不占据屏幕时触发
        /// 此时不知道应用程序将被终止还是可恢复
        /// 执行保存应用程序状态、注销等操作
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起的请求的详细信息。</param>
        static async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            try
            {
                // 外部系统关闭处理
                await Stub.ShutDown();
            }
            catch { }

            // 保存状态，10586版本后发现不用处理挂起恢复了！真的？
            //await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        /// <summary>
        /// Suspended -> Running    手机或PC平板模式下再次占据屏幕时触发
        /// 执行恢复状态、恢复会话等操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnResuming(object sender, object e)
        {
            if (AtUser.IsLogon)
            {
                // 挂起前为已登录状态时恢复会话记录

                // 获取离线私信
                //await AtIM.LoadOfflines();
            }

            // 恢复状态，不需要了？
            //await SuspensionManager.RestoreAsync();
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
                    AtLocal.SaveLog("程序异常", e.Message);
                    // 显示异常错误对话框
                    AtKit.RunAsync(async () =>
                    {
                        MessageDialog dlg = new MessageDialog(e.Message, "程序异常");
                        dlg.Commands.Add(new UICommand("终止", new UICommandInvokedHandler((cmd) => ShutDown())));
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