#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Threading.Tasks;
#endregion

namespace $safeprojectname$
{
    /// <summary>
    /// 系统存根，完整用法请参见：https://github.com/Daoting/dt/blob/master/Client/Dt.Sample/App/AppStub.cs
    /// </summary>
    public class AppStub : Stub
    {
        public AppStub()
        {
            ServerUrl = "https://10.10.1.16/fz";
            Title = "搬运工";
        }

        /// <summary>
        /// 多种启动方式请参见：
        /// https://github.com/Daoting/dt/blob/428a4f039c13fa5cc5dba427eb0b00b12822e60b/Client/Dt.Sample/App/AppStub.cs#L75
        /// </summary>
        public override async Task OnStartup()
        {
            // 按默认流程启动
            //Startup.Register(typeof(DefaultHome));
            //await Startup.Run(true);

            // 完全不使用服务启动
            Startup.Register(typeof(Home));
            Startup.ShowHome();
            await Task.CompletedTask;
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
            };

            // 处理服务器推送的类型字典
            PushHandlers = new Dictionary<string, Type>
            {
                { "syspushapi", typeof(Dt.Base.SysPushApi) },
                { "webrtcapi", typeof(Dt.Base.Chat.WebRtcApi) },
            };

            // 获取自定义可序列化类型字典
            SerializeTypes = new Dictionary<string, Type>
            { };

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