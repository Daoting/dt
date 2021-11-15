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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#endregion

namespace Dt.Shell
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public class Stub : IStub
    {
        /// <summary>
        /// 服务器地址，末尾不包含'/'
        /// </summary>
        public string ServerUrl { get; } =
#if WASM
            Kit.GetServerUrl("https://localhost/fz");
#else
            //"https://10.10.1.16/fz";
            "http://mapp.wicp.net/fz";
#endif

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title => "搬运工";

        /// <summary>
        /// 系统描述信息
        /// </summary>
        public string Desc => "搬运工平台基础样例";

        /// <summary>
        /// 默认主页(DefaultHome)的固定菜单项
        /// </summary>
        public IList<OmMenu> FixedMenus { get; } = new List<OmMenu>
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

        /// <summary>
        /// 后台任务类型
        /// </summary>
        public Type BgTaskType => typeof(Sample.BgTaskDemo);

        /// <summary>
        /// 系统启动
        /// </summary>
        public async Task OnStartup()
        {
            // 初次运行，显示用户协议、隐私政策、向导
            if (AtState.GetCookie("FirstRun") == "")
            {
                await new PrivacyDlg("lob/DtAgreement.html", "lob/DtPrivacy.html").ShowAsync();
                //await new Sample.GuideDemo().ShowAsync();
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
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public void ReceiveShare(ShareInfo p_info)
        {
            Kit.OpenWin(typeof(Dt.Sample.ReceiveShareWin), "接收分享", Icons.分享, p_info);
        }

        /// <summary>
        /// 系统注销时的处理
        /// </summary>
        public Task OnLogout()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 挂起时的处理，必须耗时小！
        /// 手机或PC平板模式下不占据屏幕时触发，此时不确定被终止还是可恢复
        /// </summary>
        /// <returns></returns>
        public Task OnSuspending()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// 恢复会话时的处理，手机或PC平板模式下再次占据屏幕时触发
        /// </summary>
        public void OnResuming()
        {
        }

        #region 自动生成
        /// <summary>
        /// 视图名称与窗口类型的映射字典，主要菜单项用
        /// </summary>
        public Dictionary<string, Type> ViewTypes { get; } = new Dictionary<string, Type>
        {
            { "报表", typeof(Dt.App.ReportView) },
            { "流程设计", typeof(Dt.App.Workflow.WorkflowMgr) },
            { "任务", typeof(Dt.App.Workflow.TasksView) },
            { "发布管理", typeof(Dt.App.Publish.PublishMgr) },
            { "发布", typeof(Dt.App.Publish.PublishView) },
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
        };

        /// <summary>
        /// 处理服务器推送的类型字典
        /// </summary>
        public Dictionary<string, Type> PushHandlers { get; } = new Dictionary<string, Type>
        {
            { "syspushapi", typeof(Dt.Base.SysPushApi) },
            { "webrtcapi", typeof(Dt.Base.Chat.WebRtcApi) },
        };

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes { get; } = new Dictionary<string, Type>
        {
            { "产品", typeof(Dt.Sample.Product) },
            { "学生", typeof(Dt.Sample.Student) },
            { "部门", typeof(Dt.Sample.Department) },
        };

        /// <summary>
        /// 本地库的结构信息，键为小写的库文件名(不含扩展名)，值为该库信息，包括版本号和表结构的映射类型
        /// </summary>
        public Dictionary<string, SqliteTblsInfo> SqliteDb { get; } = new Dictionary<string, SqliteTblsInfo>
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

        #endregion
    }
}