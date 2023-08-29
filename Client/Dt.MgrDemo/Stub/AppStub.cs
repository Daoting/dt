#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.MgrDemo
{
    /// <summary>
    /// 使用搬运工标准服务的存根
    /// </summary>
    public class AppStub : LobStub
    {
        public AppStub()
        {
            Title = "搬运工";

            SvcUrl = "http://10.10.1.16/dt-cm";
            SvcUrlOptions = new List<string>
            {
                "https://x13382a571.oicp.vip/demo",
                "http://58.240.201.154:25435/lob",
                "http://10.10.1.16/cosm-mysql",
                "http://10.10.1.16/cosm-orcl",
                "http://10.10.1.16/cosm-ms",
                "http://10.10.1.16/cosm-pg",
                "http://10.10.1.16/dt-cm"
            };
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
            var suc = await Kit.LoginByCookie();
            Kit.ShowRoot(suc ? LobViews.主页 : LobViews.登录页);
        }
    }
}