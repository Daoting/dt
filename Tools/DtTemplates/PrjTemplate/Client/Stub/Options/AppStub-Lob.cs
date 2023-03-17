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
    /// 使用搬运工标准服务的存根
    /// </summary>
    public class AppStub : LobStub
    {
        public AppStub()
        {
            Title = "搬运工";

            // 先启动$ext_safeprojectname$.Svc服务，localhost只能win wasm访问，请将localhost换成android ios虚拟机能够访问的IP
            SvcUrl = "https://localhost:1234";
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            base.ConfigureServices(p_svcs);
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
            if (await CookieX.Get("FirstRun") != "0")
            {
                await new PolicyDlg().ShowAsync();
                await CookieX.Save("FirstRun", "0");
            }

            // 已登录过先自动登录，未登录或登录失败时显示登录页
            var suc = await LobKit.LoginByCookie();
            Kit.ShowRoot(suc ? LobViews.主页 : LobViews.登录页);
        }
    }
}