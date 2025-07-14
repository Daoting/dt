#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System.Text;
using Windows.ApplicationModel;
using Windows.Storage;
#endregion

namespace Dt.Core
{
    /// <summary>
    /// 初始化
    /// </summary>
    public partial class Kit
    {
        /// <summary>
        /// Kit初始化：提取注入的服务、日志、全局事件
        /// </summary>
        /// <param name="p_svcProvider">注入服务</param>
        internal static void Init(IServiceProvider p_svcProvider)
        {
            _svcProvider = p_svcProvider;
            _typeAlias = _svcProvider.GetRequiredService<ITypeAlias>();
            _ui = _svcProvider.GetRequiredService<IUICallback>();
            _user = _svcProvider.GetService<IUserCallback>();

            // 从后台任务启动
            if (Application.Current == null)
                return;

            // 初始化日志
            Serilogger.Init();

            /* 异常处理，参见 https://github.com/Daoting/dt/issues/1
            未处理异常发生的位置有4种：
            主线程同步方法、主线程异步方法、Task内部同步方法、Task内部异步方法
            
            对于以上4种未处理异常：
            1. WinAppSdk V1.2 都能触发未处理异常事件，已完美解决崩溃问题，v1.7主线程异步异常会崩溃
            2. Skia渲染时都不触发未处理异常事件，被uno 或 .net内部拦截处理，但都不会崩溃！
            
            KnownException是业务异常，阻止业务继续时通过Throw类抛出，为了能统一显示警告信息，只能在抛出KnownException异常前显示！

            总结：所有平台都不会因为异常而崩溃，对于不是通过Throw类抛出的异常，非WinAppSdk无法给出警告提示！
            */
#if WIN
            Application.Current.UnhandledException += (s, e) =>
            {
                e.Handled = true;
                OnUnhandledException(e.Exception);
            };
#endif

            Debug("构造Stub，注入服务");
        }

        /// <summary>
        /// 启动前的准备，Application.OnLaunched中调用
        /// </summary>
        /// <returns></returns>
        internal static async Task OnLaunch()
        {
            await At.InitConfig();
            
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

            // GBK编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region 异常处理
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
                        CloseNotify(notify);
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
                var svc = GetService<IAppEvent>();
                if (svc != null)
                    await svc.OnSuspending();
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
                var svc = GetService<IAppEvent>();
                if (svc != null)
                    svc.OnResuming();
            }
            catch { }
        }
        #endregion

        /// <summary>
        /// 依赖注入的全局服务对象提供者
        /// </summary>
        static IServiceProvider _svcProvider;
        static ITypeAlias _typeAlias;
        static IUICallback _ui;
        static IUserCallback _user;
    }
}