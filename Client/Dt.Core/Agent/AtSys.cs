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
using System.IO;
using System.Threading.Tasks;
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

        #region 路径
        /* 在uno保存文件时只支持以下路径，形如：
         * LocalFolder
         * uwp：C:\Users\hdt\AppData\Local\Packages\4e169f82-ed49-494f-8c23-7dab11228222_dm57000t4aqw0\LocalState
         * android：/data/user/0/App.Droid/files
         * 
         * RoamingFolder
         * android：/data/user/0/App.Droid/files/.config
         * 
         * SharedLocalFolder
         * android：/data/user/0/App.Droid/files/.local/share
         * 
         * TemporaryFolder 缓存路径，app关闭时删除，不可用于保存文件！
         */

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
        /// 本地文件存放路径
        /// uwp：C:\Users\...\LocalState\.doc
        /// android：/data/user/0/App.Droid/files/.doc
        /// </summary>
        public static string DocPath { get; } =
#if UWP
            Path.Combine(ApplicationData.Current.LocalFolder.Path, ".doc");
#else
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".doc");
#endif

        /// <summary>
        /// 清空本地文件
        /// </summary>
        public static void ClearDoc()
        {
            try
            {
                if (Directory.Exists(DocPath))
                    Directory.Delete(DocPath, true);
                Directory.CreateDirectory(DocPath);
            }
            catch { }
        }

        /// <summary>
        /// 获取存放在.doc路径的本地文件(仅uwp可用)
        /// </summary>
        /// <param name="p_fileName"></param>
        /// <returns></returns>
        public static async Task<StorageFile> GetUwpDocFile(string p_fileName)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(DocPath);
            return await folder.TryGetItemAsync(p_fileName) as StorageFile;
        }

        /// <summary>
        /// 删除存放在.doc路径的本地文件
        /// </summary>
        /// <param name="p_fileName">文件名</param>
        public static void DeleteDocFile(string p_fileName)
        {
            try
            {
                File.Delete(Path.Combine(DocPath, p_fileName));
            }
            catch { }
        }
        #endregion

        #region 登录注销
        /// <summary>
        /// 显示登录页面
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

            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            SQLitePCL.Batteries_V2.Init();
            // 打开状态库
            AtLocal.OpenStateDb();
            // 创建本地文件存放目录
            if (!Directory.Exists(DocPath))
                Directory.CreateDirectory(DocPath);
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
                await Stub.OnShutDown();
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
                    Log.Error("未处理异常：" + e.Message);
                    // 显示异常错误对话框
                    AtKit.RunAsync(async () =>
                    {
                        MessageDialog dlg = new MessageDialog(e.Message, "程序异常");
                        dlg.Commands.Add(new UICommand("终止", new UICommandInvokedHandler(async (cmd) =>
                        {
                            try
                            {
                                // 外部系统关闭处理
                                await Stub.OnShutDown();
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