#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2016-02-18
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using Dt.Core.Model;
using Dt.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Sample
{
    /// <summary>
    /// 存根
    /// </summary>
    public class AppStub : Stub
    {
        public AppStub()
        {
            //ServerUrl = "https://10.10.1.16/fz";
            ServerUrl = "http://mapp.wicp.net/fz";
            Title = "搬运工";
            EnableBgTask = true;

            MenuKit.FixedMenus = new List<OmMenu>
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
                    SvcName: "cm"),

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

        /// <summary>
        /// 系统启动
        /// </summary>
        public override async Task OnStartup()
        {
            // 初次运行，显示用户协议、隐私政策、向导
            if (AtState.GetCookie("FirstRun") == "")
            {
                await new PrivacyDlg("lob/DtAgreement.html", "lob/DtPrivacy.html").ShowAsync();
                AtState.SaveCookie("FirstRun", "0");
            }

            //Startup.AutoStartOnce = new AutoStartInfo
            //{
            //    WinType = typeof(Dt.Sample.SamplesMain).AssemblyQualifiedName,
            //    Title = "自启动样例",
            //};

            // 1. 按默认流程启动
            //Startup.Register(typeof(DefaultHome));
            //await Startup.Run(true);

            // 2. 自定义启动过程
            //if (await Startup.OpenModelDb())
            //{
            //    Startup.Register(typeof(Sample.SamplesMain));
            //    Startup.ShowHome();
            //}

            // 3. 完全不使用dt服务
            Startup.Register(typeof(Sample.SamplesMain));
            Startup.ShowHome();
        }

        /// <summary>
        /// 后台任务处理，除 AtState、Stub、UnaryRpc、Kit.Toast 外，不可使用任何UI和外部变量，保证可独立运行！！！
        /// </summary>
        public override async Task OnBgTaskRun()
        {
            //string tpName = AtState.GetCookie("LoginPhone");
            //var cfg = await new UnaryRpc("cm", "ModelMgr.GetConfig").Call<Dict>();
            //await BackgroundLogin();
            //Kit.Toast(
            //    "样例",
            //    tpName + "\r\n" + cfg.Date("now").ToString(),
            //    new AutoStartInfo { WinType = typeof(LvHome).AssemblyQualifiedName, Title = "列表" });

            await Task.CompletedTask;
        }

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public override void OnReceiveShare(ShareInfo p_info)
        {
            Kit.OpenWin(typeof(ReceiveShareWin), "接收分享", Icons.分享, p_info);
        }

        #region 自动生成
        protected override void Init()
        {
            // 视图名称与窗口类型的映射字典，主要菜单项用
            ViewTypes = new Dictionary<string, Type>
            {
                { "报表", typeof(Dt.App.ReportView) },
                { "流程设计", typeof(Dt.App.Workflow.WorkflowMgr) },
                { "任务", typeof(Dt.App.Workflow.TasksView) },
                { "发布", typeof(Dt.App.Publish.PublishView) },
                { "发布管理", typeof(Dt.App.Publish.PublishMgr) },
                { "基础选项", typeof(Dt.App.Model.BaseOption) },
                { "菜单管理", typeof(Dt.App.Model.MenuWin) },
                { "我的设置", typeof(Dt.App.Model.MyParamsSetting) },
                { "参数定义", typeof(Dt.App.Model.UserParamsWin) },
                { "基础权限", typeof(Dt.App.Model.PrvWin) },
                { "报表设计", typeof(Dt.App.Model.RptWin) },
                { "系统角色", typeof(Dt.App.Model.RoleWin) },
                { "用户账号", typeof(Dt.App.Model.UserAccountWin) },
                { "文件", typeof(Dt.App.File.FileHome) },
                { "通讯录", typeof(Dt.Base.Chat.ChatHome) },
                { "样例", typeof(Dt.Sample.SamplesMain) },
                { "ShoppingWin", typeof(Dt.Sample.ModuleView.OneToMany1.ShoppingWin) },
            };

            // 处理服务器推送的类型字典
            PushHandlers = new Dictionary<string, Type>
            {
                { "syspushapi", typeof(Dt.Base.SysPushApi) },
                { "webrtcapi", typeof(Dt.Base.Chat.WebRtcApi) },
            };

            // 获取自定义可序列化类型字典
            SerializeTypes = new Dictionary<string, Type>
            {
                { "产品", typeof(Dt.Sample.Product) },
                { "学生", typeof(Dt.Sample.Student) },
                { "部门", typeof(Dt.Sample.Department) },
            };

            // 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
            SqliteDb = new Dictionary<string, SqliteTblsInfo>
            {
                {
                    "state",
                    new SqliteTblsInfo
                    {
                        Version = "b52141c5528d02be53e32e6fb8d2310c",
                        Tables = new List<Type>
                        {
                            typeof(Dt.App.MenuFav),
                            typeof(Dt.App.UserMenu),
                            typeof(Dt.App.File.ReadFileHistory),
                            typeof(Dt.Base.Docking.DockLayout),
                            typeof(Dt.Base.ModuleView.SearchHistory),
                            typeof(Dt.Base.FormView.CellLastVal),
                            typeof(Dt.Base.Chat.ChatMember),
                            typeof(Dt.Base.Chat.Letter),
                            typeof(Dt.Core.Model.ClientLog),
                            typeof(Dt.Core.Model.ClientCookie),
                            typeof(Dt.Core.Model.DataVersion),
                            typeof(Dt.Core.Model.UserParams),
                            typeof(Dt.Core.Model.UserPrivilege),
                        }
                    }
                },
            };
        }
        #endregion
    }
}