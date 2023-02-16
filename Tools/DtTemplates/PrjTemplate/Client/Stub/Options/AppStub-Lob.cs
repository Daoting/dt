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

            // 先启动$ext_safeprojectname$.Svc服务，localhost只能win wasm访问，若确保android ios虚拟机能够访问请使用IP
            SvcUrl = "https://localhost:1234";
            InitFixedMenus();
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
            var suc = await LoginDs.Me.LoginByCookie();
            Kit.ShowRoot(suc ? LobViews.主页 : LobViews.登录页);
        }

        /// <summary>
        /// 设置主页的固定菜单项
        /// </summary>
        void InitFixedMenus()
        {
            LobKit.FixedMenus = new List<OmMenu>
            {
                new OmMenu(
                    ID: 6000,
                    Name: "主窗",
                    Icon: "搬运工",
                    ViewName: "主窗"),

                new OmMenu(
                    ID: 1110,
                    Name: "通讯录",
                    Icon: "留言",
                    ViewName: "通讯录"),

                new OmMenu(
                    ID: 3000,
                    Name: "任务",
                    Icon: "双绞线",
                    ViewName: "任务"),

                new OmMenu(
                    ID: 4000,
                    Name: "文件",
                    Icon: "文件夹",
                    ViewName: "文件"),

                new OmMenu(
                    ID: 5000,
                    Name: "发布",
                    Icon: "公告",
                    ViewName: "发布"),
            };
        }
    }
}