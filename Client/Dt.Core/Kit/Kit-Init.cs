﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: AtSys合并到Kit
* 日志: 2021-06-08 改名
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog.Extensions.ElapsedTime;
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
        /// 提取注入的服务，Stub构造方法中调用
        /// </summary>
        /// <param name="p_svcProvider">注入服务</param>
        internal static void Inject(IServiceProvider p_svcProvider)
        {
            _svcProvider = p_svcProvider;
            _typeAlias = _svcProvider.GetRequiredService<ITypeAlias>();
            _ui = _svcProvider.GetRequiredService<IUICallback>();
            _user = _svcProvider.GetService<IUserCallback>();
            At.InitConfig();
            Trace("注入服务");
        }

        /// <summary>
        /// 正常启动结束，最后的准备，Application.OnLaunched中调用
        /// </summary>
        /// <returns></returns>
        internal static async Task OnLaunched()
        {
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
            Log.Logger.WithElapsed<Kit>(GlobalConfig.Path);
            TraceTick("启动耗时");
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