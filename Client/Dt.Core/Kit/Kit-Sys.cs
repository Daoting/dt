#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统相关管理类
    /// </summary>
    public partial class Kit
    {
        #region 系统基础
        /// <summary>
        /// 获取系统是否采用手机的UI模式
        /// </summary>
        public static bool IsPhoneUI { get; internal set; }

        /// <summary>
        /// 获取宿主操作系统类型
        /// </summary>
        public static HostOS HostOS
        {
            get
            {
#if UWP
                return HostOS.Windows;
#elif IOS
                return HostOS.iOS;
#elif ANDROID
                return HostOS.Android;
#elif WASM
                return GetHostOS();
#endif
            }
        }

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
        /// 获取系统存根
        /// </summary>
        public static IStub Stub { get; internal set; }

        /// <summary>
        /// 显示监视窗口
        /// </summary>
        public static Action ShowTraceBox => Callback.ShowTraceBox;

        /// <summary>
        /// 系统回调
        /// </summary>
        internal static ICallback Callback { get; private set; }

        /// <summary>
        /// 获取设置是否监控Rpc调用结果，TraceBox中控制输出
        /// </summary>
        internal static bool TraceRpc { get; set; }
        #endregion

        #region 当前时间
        /// <summary>
        /// 获取服务器端当前时间，根据时差计算所得
        /// </summary>
        public static DateTime Now
        {
            get { return DateTime.Now + _timeSpan; }
        }

        static TimeSpan _timeSpan;
        /// <summary>
        /// 同步服务器时间
        /// </summary>
        /// <param name="p_serverTime"></param>
        internal static void SyncTime(DateTime p_serverTime)
        {
            // 与服务器时差
            _timeSpan = p_serverTime - DateTime.Now;
        }
        #endregion

        #region 本地文件
        /* 在uno保存文件时只支持以下路径，形如：
         * LocalFolder
         * uwp：C:\Users\hdt\AppData\Local\Packages\4e169f82-ed49-494f-8c23-7dab11228222_dm57000t4aqw0\LocalState
         * android：/data/user/0/App.Droid/files
         * ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
         * wasm：/local
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
        /// 本地文件的根路径
        /// uwp：C:\Users\...\LocalState
        /// android：/data/user/0/App.Droid/files
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data
        /// wasm：/local
        /// </summary>
        public static string RootPath
        {
            get { return ApplicationData.Current.LocalFolder.Path; }
        }

        /// <summary>
        /// 本地缓存文件的存放路径
        /// uwp：C:\Users\...\LocalState\.doc
        /// android：/data/user/0/App.Droid/files/.doc
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data/.doc
        /// wasm：/local/.doc
        /// </summary>
        public static string CachePath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, ".doc");

        /// <summary>
        /// 本地sqlite数据文件的存放路径
        /// uwp：C:\Users\...\LocalState\.data
        /// android：/data/user/0/App.Droid/files/.data
        /// ios：/Users/usrname/Library/Developer/CoreSimulator/Devices/xxx/data/Containers/Data/Application/yyy/Library/Data/.data
        /// wasm：/local/.doc
        /// </summary>
        public static string DataPath { get; } = Path.Combine(ApplicationData.Current.LocalFolder.Path, ".data");

        /// <summary>
        /// 清空所有存放在.doc路径的缓存文件
        /// </summary>
        public static void ClearCacheFiles()
        {
            try
            {
                if (Directory.Exists(CachePath))
                    Directory.Delete(CachePath, true);
                Directory.CreateDirectory(CachePath);
            }
            catch { }
        }

        /// <summary>
        /// 删除存放在.doc路径的本地文件
        /// </summary>
        /// <param name="p_fileName">文件名</param>
        public static void DeleteCacheFile(string p_fileName)
        {
            try
            {
                File.Delete(Path.Combine(CachePath, p_fileName));
            }
            catch { }
        }
        #endregion

        #region 登录注销
        /// <summary>
        /// 显示登录页面，参数：是否为弹出式
        /// </summary>
        public static Action<bool> Login => Callback.Login;

        /// <summary>
        /// 注销后重新登录
        /// </summary>
        public static Action Logout => Callback.Logout;
        #endregion

        #region 平台启动
        /// <summary>
        /// 系统初始化
        /// </summary>
        /// <param name="p_stub">系统存根</param>
        /// <param name="p_callback"></param>
        internal static void Startup(IStub p_stub, ICallback p_callback)
        {
            Stub = p_stub;
            Callback = p_callback;
            if (Stub.SerializeTypes != null)
                SerializeTypeAlias.Merge(Stub.SerializeTypes);

            var app = Application.Current;
            app.Suspending += OnSuspending;
            app.Resuming += OnResuming;

            // 异常处理
#if UWP
            app.UnhandledException += OnUwpUnhandledException;
#elif ANDROID
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#elif IOS
            // 在iOS项目的Main函数处理
#elif WASM
            //TaskScheduler.UnobservedTaskException += (s, e) => OnUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e.ExceptionObject as Exception);
#endif

            // 创建本地文件存放目录
            if (!Directory.Exists(CachePath))
                Directory.CreateDirectory(CachePath);
            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);

#if WASM
            // .net5.0 只能引用 SQLite3Provider_sqlite3，DllImport("sqlite3")
            // 默认为 SQLite3Provider_e_sqlite3 引用时出错！
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#else
            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            // 内部调用 SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            SQLitePCL.Batteries_V2.Init();
#endif

            // 打开状态库
            AtState.OpenDb();
        }
        #endregion

        #region App事件方法
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
                await Callback.OnSuspending();
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
                Callback.OnResuming();
                Stub.OnResuming();
            }
            catch { }
        }
        #endregion

        #region 异常处理
        public static void OnUnhandledException(Exception p_ex)
        {
            try
            {
                KnownException kex;
                if ((kex = p_ex as KnownException) != null || (kex = p_ex.InnerException as KnownException) != null)
                {
                    // 只警告，不保存日志
                    Kit.Warn(kex.Message);
                }
                else
                {
                    string title;
                    string msg;
                    if (p_ex is ServerException se)
                    {
                        title = se.Title;
                        msg = se.Message;
                    }
                    else
                    {
                        title = "未处理异常";
                        msg = $"异常消息：{p_ex.Message}\r\n堆栈信息：{p_ex.StackTrace}";
                    }

                    // 警告、输出监视、保存日志
                    var notify = new NotifyInfo
                    {
                        NotifyType = NotifyType.Warning,
                        Message = title,
                        DelaySeconds = 5,
                        Link = "查看详细",
                    };
                    notify.LinkCallback = (e) =>
                    {
                        ShowTraceBox();
                        notify.Close();
                    };
                    SysVisual.NotifyList.Add(notify);
                    Kit.Trace(TraceOutType.UnhandledException, title, msg);
                    Log.Error(msg);
                }
            }
            catch { }
        }

        static void OnUwpUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            OnUnhandledException(e.Exception);
        }

#if ANDROID
        static void OnAndroidUnhandledException(object sender, Android.Runtime.RaiseThrowableEventArgs e)
        {
            e.Handled = true;
            OnUnhandledException(e.Exception);
        }
#endif
        #endregion
    }
}