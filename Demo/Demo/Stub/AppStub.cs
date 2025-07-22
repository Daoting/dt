#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024/5/20 10:39:08 创建
******************************************************************************/
#endregion

#region 引用命名
using Demo.Base;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Demo
{
    /// <summary>
    /// 使用搬运工标准服务的存根
    /// </summary>
    public class AppStub : LobStub
    {
        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            base.ConfigureServices(p_svcs);
            p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
            p_svcs.AddTransient<IReceiveShare, ReceiveShare>();
            p_svcs.AddTransient<ITaskbar, Taskbar>();
            //p_svcs.AddTransient<ITaskbar, MyTaskbar>();
            //p_svcs.AddSingleton<ILogSetting, LogSetting>();
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

            LoginDs.Login += Gs.OnLogin;
            // 已登录过先自动登录，未登录或登录失败时显示登录页
            var suc = await Kit.LoginByCookie();
            Kit.ShowRoot(suc ? LobViews.主页 : LobViews.登录页);
        }

        protected override async void OnInitFailed(Exception p_ex)
        {
            // 服务器连接失败后主页设为控件样例
            Kit.ShowRoot("控件样例");
            
            if (await CookieX.Get("InitFailed") != "0")
            {
                if (!await Kit.Confirm("服务器连接失败，只显示控件样例！\r\n下次失败是否继续提醒？", "提醒"))
                    await CookieX.Save("InitFailed", "0");
            }
        }
    }
}