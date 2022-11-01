#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace $ext_safeprojectname$
{
    /// <summary>
    /// 未使用标准服务的存根
    /// </summary>
    public partial class AppStub : DefaultStub
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
        protected override async Task OnStartup()
        {
			// 打开本地sqlite库
            AtLocal.OpenDb();

            // 初次运行，显示用户协议和隐私政策对话框
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PolicyDlg().ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            ShowRoot(typeof(typeof(Home));
        }
    }
}