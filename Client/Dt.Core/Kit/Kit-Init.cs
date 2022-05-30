#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 系统初始化
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// 系统初始化
        /// </summary>
        internal static async Task Init()
        {
            // 初始化日志
            Serilogger.Init();

#if !WIN
            // WinUI已移除事件，其他平台咋？
            var app = Application.Current;
            app.Suspending += OnSuspending;
            app.Resuming += OnResuming;
#endif

            // 异常处理
#if WIN
            Application.Current.UnhandledException += OnUwpUnhandledException;
#elif ANDROID
            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
#elif IOS
            // 在iOS项目的Main函数处理
#elif WASM
            //TaskScheduler.UnobservedTaskException += (s, e) => OnUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e.ExceptionObject as Exception);
#endif

            // 创建本地文件存放目录
            // 使用 StorageFolder 替换 Directory 是因为 wasm 中可以等待 IDBFS 初始化完毕！！！
            // 否则用 Directory 每次都创建新目录！
            var localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.CreateFolderAsync(".doc", CreationCollisionOption.OpenIfExists);
            await localFolder.CreateFolderAsync(".data", CreationCollisionOption.OpenIfExists);
            //if (!Directory.Exists(CachePath))
            //    Directory.CreateDirectory(CachePath);
            //if (!Directory.Exists(DataPath))
            //    Directory.CreateDirectory(DataPath);

#if WASM
            // .net5.0 只能引用 SQLite3Provider_sqlite3，DllImport("sqlite3")
            // 默认为 SQLite3Provider_e_sqlite3 引用时出错！
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#else
            // 升级.net6.0后使用 Microsoft.Data.Sqlite 的默认初始化
            // 初始化不同平台的包绑定！V2支持类型和属性的绑定
            // 内部调用 SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            //SQLitePCL.Batteries_V2.Init();
#endif

            // 打开状态库
            AtState.OpenDb();

            // 创建窗口及整个系统可视树
            SysVisual.Init();
            Log.Debug("初始化日志、目录、状态库、可视树");
        }

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
                await Stub.Inst.OnSuspending();
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
                Stub.Inst.OnResuming();
            }
            catch { }
        }
        #endregion

        #region 异常处理
        static void OnUnhandledException(Exception p_ex)
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
                    if (p_ex is ServerException se)
                    {
                        title = se.Title;
                    }
                    else
                    {
                        title = $"未处理异常：{p_ex.GetType().FullName}";
                    }

                    // 警告、保存日志
                    var notify = new NotifyInfo
                    {
                        NotifyType = NotifyType.Warning,
                        Message = title,
                        Delay = 5,
                        Link = "查看详细",
                    };
                    notify.LinkCallback = (e) =>
                    {
                        ShowTraceBox();
                        notify.Close();
                    };
                    Notify(notify);
                    Log.Error(p_ex, title);
                }
            }
            catch { }
        }

        static void OnUwpUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            OnUnhandledException(e.Exception);
        }

#if IOS
        public static void OnIOSUnhandledException(Exception ex)
        {
            OnUnhandledException(ex);
            RunLoop();
        }

        /// <summary>
        /// 原创方法，防止异常时闪退，碰巧好使
        /// 网上未找到处理方法，已测试的方法有：
        /// ObjCRuntime.Runtime.MarshalManagedException += OnIOSUnhandledException;
        /// AppDomain.CurrentDomain.UnhandledException
        /// NSSetUncaughtExceptionHandler signal
        /// Mono.Runtime.RemoveSignalHandlers
        /// </summary>
        static void RunLoop()
        {
            var loop = CoreFoundation.CFRunLoop.Current;
            bool hasException;
            while (true)
            {
                try
                {
                    loop.RunInMode(CoreFoundation.CFRunLoop.ModeDefault, 0.001, false);
                }
                catch (Exception ex)
                {
                    hasException = true;
                    OnUnhandledException(ex);
                    break;
                }
            }

            if (hasException)
                RunLoop();
        }
#endif

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