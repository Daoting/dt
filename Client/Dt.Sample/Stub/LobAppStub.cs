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

namespace Dt.Sample
{
    /// <summary>
    /// 使用搬运工标准服务的存根
    /// </summary>
    public class LobAppStub : LobStub
    {
        public LobAppStub()
        {
            Title = "搬运工";
            SvcUrl = "http://10.10.1.2/sample";
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
            AtLocal.OpenDb();
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PolicyDlg().ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            // 已登录过先自动登录，未登录或登录失败时显示登录页
            string phone = AtState.GetCookie("LoginPhone");
            string pwd = AtState.GetCookie("LoginPwd");
            if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
            {
                var result = await AtCm.LoginByPwd<LoginResult>(phone, pwd);
                if (result.IsSuc)
                {
                    await LobKit.AfterLogin(result);
                    Kit.ShowRoot(LobViews.主页);
                    return;
                }
            }
            Kit.ShowRoot(LobViews.登录页);
        }

        /// <summary>
        /// 设置主页的固定菜单项
        /// </summary>
        void InitFixedMenus()
        {
            LobKit.FixedMenus = new List<OmMenu>
            {
                new OmMenu(
                    ID: 1110,
                    Name: "通讯录",
                    Icon: "留言",
                    ViewName: "通讯录"),

                new OmMenu(
                    ID: 3000,
                    Name: "任务",
                    Icon: "双绞线",
                    ViewName: "任务",
                    SvcName: "cm:UserRelated.GetMenuTip"),

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

                new OmMenu(
                    ID: 1,
                    Name: "样例",
                    Icon: "词典",
                    ViewName: "样例"),
            };
        }
    }
}