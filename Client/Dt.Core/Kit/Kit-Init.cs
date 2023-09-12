﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using System.Text;
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
        /// 系统初始化：日志、全局事件、本地目录、状态库
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

            // 异常处理，参见 https://github.com/Daoting/dt/issues/1
            AttachUnhandledException();

            // 创建本地文件存放目录
            // 使用 StorageFolder 替换 Directory 是因为 wasm 中可以等待 IDBFS 初始化完毕！！！
            // 否则用 Directory 每次都创建新目录！
            var localFolder = ApplicationData.Current.LocalFolder;
            await localFolder.CreateFolderAsync(".doc", CreationCollisionOption.OpenIfExists);
            await localFolder.CreateFolderAsync(".data", CreationCollisionOption.OpenIfExists);
            await localFolder.CreateFolderAsync(".log", CreationCollisionOption.OpenIfExists);
            //if (!Directory.Exists(CachePath))
            //    Directory.CreateDirectory(CachePath);
            //if (!Directory.Exists(DataPath))
            //    Directory.CreateDirectory(DataPath);

            // SQLite3
            // 默认引用顺序：Microsoft.Data.Sqlite -> SQLitePCLRaw.bundle_e_sqlite3 -> SQLitePCLRaw.provider.e_sqlite3
            // 内部默认初始化调用 SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3());
            // 所以最终引用的库名为 e_sqlite3
            // iOS上即可绑定到系统 sqlite(早期)，也可绑定到 e_sqlite3(现在)
            // 但wasm中引用包Uno.sqlite-wasm中的库名为 sqlite3，需要另外引用包 SQLitePCLRaw.provider.sqlite3，并且设置provider
            // 否则默认privider为 SQLite3Provider_e_sqlite3，因找不到e_sqlite3出错！
#if WASM
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_sqlite3());
#endif
            // GBK编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            IsInited = true;
            Debug("Kit.Init完毕");
        }

        /// <summary>
        /// app已初始化
        /// </summary>
        internal static bool IsInited { get; private set; }

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
#if WIN

        static void AttachUnhandledException()
        {
            // For WinUI 3:
            //
            // * Exceptions on background threads are caught by AppDomain.CurrentDomain.UnhandledException,
            //   not by Microsoft.UI.Xaml.Application.Current.UnhandledException
            //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/5221
            //
            // * Exceptions caught by Microsoft.UI.Xaml.Application.Current.UnhandledException have details removed,
            //   but that can be worked around by saved by trapping first chance exceptions
            //   See: https://github.com/microsoft/microsoft-ui-xaml/issues/7160
            //
            //  目前问题：UI主线程异步异常造成崩溃、后台未处理异常不能提醒，V1.2 preview2解决

            // V1.2 已完美解决
            Application.Current.UnhandledException += (s, e) =>
            {
                e.Handled = true;
                OnUnhandledException(e.Exception);
            };
        }

#elif ANDROID

        static void AttachUnhandledException()
        {
            // For Android:
            // All exceptions will flow through Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser,
            // and NOT through AppDomain.CurrentDomain.UnhandledException

            Android.Runtime.AndroidEnvironment.UnhandledExceptionRaiser += (s, e) =>
            {
                e.Handled = true;
                OnUnhandledException(e.Exception);
            };
        }

#elif IOS

        static void AttachUnhandledException()
        {
            // .net7.0 崩溃已治愈
            // 1. 在Main函数中try catch，延用xamarin中 RunLoop 的方法
            // 2. 处理以下两事件，否则 "调试时不崩溃，正式运行时崩溃"
            // UI主线程异步方法中抛异常，不再崩溃

            ObjCRuntime.Runtime.MarshalManagedException += (s, e) => e.ExceptionMode = ObjCRuntime.MarshalManagedExceptionMode.UnwindNativeCode;
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e.ExceptionObject as Exception);
        }

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

#elif WASM

        static void AttachUnhandledException()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(e.ExceptionObject as Exception);
        }

#endif

        static void OnUnhandledException(Exception p_ex)
        {
            try
            {
                // 不处理已知异常，已在抛出异常前警告(Throw类)，不输出日志
                if (!(p_ex is KnownException) && !(p_ex.InnerException is KnownException))
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
                        ShowLogBox();
                        notify.Close();
                    };
                    Notify(notify);

                    // ServerException日志已输出
                    if (p_ex is not ServerException)
                        Log.Error(p_ex, title);
                }
            }
            catch { }
        }
        #endregion
    }
}