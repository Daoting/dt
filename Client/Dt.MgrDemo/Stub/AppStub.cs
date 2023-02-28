#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Mgr;
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
            //SvcUrl = "http://10.10.1.2/sample";
            //SvcUrl = "https://x13382a571.oicp.vip/sample";
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
            var suc = await LoginDs.LoginByCookie();
            Kit.ShowRoot(suc ? LobViews.主页 : LobViews.登录页);
        }

        /// <summary>
        /// 设置主页的固定菜单项
        /// </summary>
        void InitFixedMenus()
        {
            MenuDs.FixedMenus = new List<OmMenu>
            {
                new OmMenu(
                    ID: 1,
                    Name: "控件样例",
                    Icon: "词典",
                    ViewName: "控件样例"),

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