#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.Mgr;
using Dt.Base;
using Dt.Core.Model;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 存根
    /// </summary>
    public class AppStub : DefaultStub
    {
        public AppStub()
        {
            Title = "搬运工";
            InitCmUrl("http://10.10.1.16/dt-cm");
            LogSetting.FileEnabled = true;
        }

        /// <summary>
        /// 注入全局服务
        /// </summary>
        /// <param name="p_svcs"></param>
        protected override void ConfigureServices(IServiceCollection p_svcs)
        {
            p_svcs.AddTransient<IFixedMenus, FixedMenusDemo>();
            p_svcs.AddTransient<IBackgroundJob, BackgroundJob>();
        }

        /// <summary>
        /// 初始化完毕，系统启动
        /// </summary>
        protected override async Task OnStartup()
        {
            // 初次运行，显示用户协议、隐私政策、向导
            if (AtState.GetCookie("FirstRun") == "")
            {
                await new PrivacyDlg("lob/DtAgreement.html", "lob/DtPrivacy.html").ShowAsync();
                AtState.SaveCookie("FirstRun", "0");
            }

            // 1. 默认启动
            //await StartRun();

            // 2. 自定义启动
            await StartRun(typeof(Sample.SamplesMain), false);
        }

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        protected override void OnReceiveShare(ShareInfo p_info)
        {
            Kit.OpenWin(typeof(ReceiveShareWin), "接收分享", Icons.分享, p_info);
        }

        #region 自动生成
        protected override void Init()
        {
            // 视图名称与窗口类型的映射字典，主要菜单项用
            ViewTypes = new Dictionary<string, Type>
            {
                { "通讯录", typeof(Dt.Base.Chat.ChatHome) },
                { "报表", typeof(Dt.Mgr.ReportView) },
                { "流程设计", typeof(Dt.Mgr.Workflow.WorkflowMgr) },
                { "任务", typeof(Dt.Mgr.Workflow.TasksView) },
                { "发布", typeof(Dt.Mgr.Publish.PublishView) },
                { "发布管理", typeof(Dt.Mgr.Publish.PublishMgr) },
                { "基础选项", typeof(Dt.Mgr.Model.BaseOption) },
                { "菜单管理", typeof(Dt.Mgr.Model.MenuWin) },
                { "我的设置", typeof(Dt.Mgr.Model.MyParamsSetting) },
                { "参数定义", typeof(Dt.Mgr.Model.UserParamsWin) },
                { "基础权限", typeof(Dt.Mgr.Model.PrvWin) },
                { "报表设计", typeof(Dt.Mgr.Model.RptWin) },
                { "系统角色", typeof(Dt.Mgr.Model.RoleWin) },
                { "用户账号", typeof(Dt.Mgr.Model.UserAccountWin) },
                { "文件", typeof(Dt.Mgr.File.FileHome) },
                { "样例", typeof(Dt.Sample.SamplesMain) },
                { "ShoppingWin", typeof(Dt.Sample.ModuleView.OneToMany1.ShoppingWin) },
            };

            // 处理服务器推送的类型字典
            PushHandlers = new Dictionary<string, Type>
            {
                { "syspushapi", typeof(Dt.Base.SysPushApi) },
                { "webrtcapi", typeof(Dt.Base.Chat.WebRtcApi) },
            };

            // 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
            SqliteDb = new Dictionary<string, SqliteTblsInfo>
            {
                {
                    "state",
                    new SqliteTblsInfo
                    {
                        Version = "047ebd4f0ef4957193958ba8aff3966b",
                        Tables = new List<Type>
                        {
                            typeof(Dt.Core.Model.ClientCookie),
                            typeof(Dt.Core.Model.DataVersion),
                            typeof(Dt.Core.Model.UserParams),
                            typeof(Dt.Core.Model.UserPrivilege),
                            typeof(Dt.Base.Docking.DockLayout),
                            typeof(Dt.Base.ModuleView.SearchHistory),
                            typeof(Dt.Base.FormView.CellLastVal),
                            typeof(Dt.Base.Chat.ChatMember),
                            typeof(Dt.Base.Chat.Letter),
                            typeof(Dt.Mgr.MenuFav),
                            typeof(Dt.Mgr.UserMenu),
                            typeof(Dt.Mgr.File.ReadFileHistory),
                        }
                    }
                },
            };
        }
        #endregion
    }
}