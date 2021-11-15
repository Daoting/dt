#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2015-08-04
******************************************************************************/
#endregion

#region 引用命名
using AndroidX.Work;
using Dt.Base;
using Dt.Core;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
#endregion

namespace Dt.Shell
{
    sealed partial class App : Application
    {
        string _params;

        public App()
        {
            InitializeComponent();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs p_args)
        {
            if (string.IsNullOrEmpty(_params))
            {
                await Startup.Launch<Stub>(p_args.Arguments, null);

                // 注册后台服务，后台Worker每15分钟运行一次，系统要求最短间隔15分钟！
                var workRequest = PeriodicWorkRequest.Builder.From<Droid.PluginWorker>(TimeSpan.FromMinutes(15)).Build();
                // 设为Replace时每次启动都运行后台服务，方便调试！
                WorkManager.GetInstance(Android.App.Application.Context).EnqueueUniquePeriodicWork("PluginWorker", ExistingPeriodicWorkPolicy.Replace, workRequest);
            }
            else
            {
                await Startup.Launch<Stub>(_params, null);
                _params = null;
            }
        }

        public async void ReceiveShare(ShareInfo p_shareInfo)
        {
            await Startup.Launch<Stub>(null, p_shareInfo);
        }

        public void ToastStart(string p_params)
        {
            // 点击通知栏启动
            if (Kit.Stub != null)
            {
                // 非null表示app已启动过，不会再调用 OnLaunched
                _ = Startup.Launch<Stub>(p_params, null);
            }
            else
            {
                // 未启动，记录参数提供给 OnLaunched
                _params = p_params;
            }
        }
    }
}