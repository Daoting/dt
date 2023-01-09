#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.UIDemo
{
    /// <summary>
    /// 未使用标准服务的存根
    /// </summary>
    public class AppStub : DefaultStub
    {
        public AppStub()
        {
            Title = "搬运工";
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            base.ConfigureServices(p_svcs);
            p_svcs.AddSingleton<IRpcConfig, RpcConfig>();
            p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
            p_svcs.AddTransient<IReceiveShare, ReceiveShare>();
            //p_svcs.AddSingleton<ILogSetting, LogSetting>();
            //p_svcs.AddTransient<ITheme, CustomTheme>();
        }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        /// <returns></returns>
        protected override async Task OnStartup()
        {
            // 初次运行，显示用户协议和隐私政策对话框
            AtLocal.OpenDb();
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PolicyDlg().ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            Kit.ShowRoot(typeof(DemoMain));
        }
    }
}