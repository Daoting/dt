﻿#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 使用搬运工标准服务的存根
    /// </summary>
    //public partial class AppStub : LobStub
    //{
    //    public AppStub()
    //    {
    //        Title = "搬运工";
    //        SvcUrl = "https://x13382a571.oicp.vip/sample";
    //        InitFixedMenus();
    //    }

    //    protected override void ConfigureServices(IServiceCollection p_svcs)
    //    {
    //        base.ConfigureServices(p_svcs);
    //        p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
    //        p_svcs.AddTransient<IReceiveShare, ReceiveShare>();
    //    }

    //    protected override async Task OnStartup()
    //    {
    //        await StartRun();
    //    }

    //    /// <summary>
    //    /// 设置主页的固定菜单项
    //    /// </summary>
    //    void InitFixedMenus()
    //    {
    //        Lob.FixedMenus = new List<OmMenu>
    //        {
    //            new OmMenu(
    //                ID: 1110,
    //                Name: "通讯录",
    //                Icon: "留言",
    //                ViewName: "通讯录"),

    //            new OmMenu(
    //                ID: 3000,
    //                Name: "任务",
    //                Icon: "双绞线",
    //                ViewName: "任务",
    //                SvcName: "cm:UserRelated.GetMenuTip"),

    //            new OmMenu(
    //                ID: 4000,
    //                Name: "文件",
    //                Icon: "文件夹",
    //                ViewName: "文件"),

    //            new OmMenu(
    //                ID: 5000,
    //                Name: "发布",
    //                Icon: "公告",
    //                ViewName: "发布"),

    //            new OmMenu(
    //                ID: 1,
    //                Name: "样例",
    //                Icon: "词典",
    //                ViewName: "样例"),
    //        };
    //    }
    //}
}