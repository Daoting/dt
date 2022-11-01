﻿#region 文件描述
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
    public partial class AppStub : LobStub
    {
        public AppStub()
        {
            Title = "搬运工";

            // ip不能为localhost，确保android ios虚拟机能够访问
            SvcUrl = "https://x13382a571.oicp.vip/sample";
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

        protected override async Task OnStartup()
        {
            AtLocal.OpenDb();

            // 初次运行，显示用户协议和隐私政策对话框
            if (AtLocal.GetDict("FirstRun") == "")
            {
                await new PolicyDlg().ShowAsync();
                AtLocal.SaveDict("FirstRun", "0");
            }

            await StartRun();
        }

        /// <summary>
        /// 设置主页的固定菜单项
        /// </summary>
        void InitFixedMenus()
        {
            Lob.FixedMenus = new List<OmMenu>
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