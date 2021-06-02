#region 文件描述
/******************************************************************************
* 创建: $username$
* 摘要: 
* 日志: $time$ 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.App;
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Shell
{
    /// <summary>
    /// 系统存根
    /// </summary>
    public class Stub : IStub
    {
#if WASM
        readonly string _serverUrl = AtWasm.GetServerUrl();

        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerUrl
        {
            get { return _serverUrl; }
        }
#else
        /// <summary>
        /// 服务器地址，末尾不包含'/'
        /// </summary>
        public string ServerUrl => "https://10.10.1.16/fz";
#endif

        /// <summary>
        /// 系统标题
        /// </summary>
        public string Title => "搬运工";

        /// <summary>
        /// 登录页面
        /// </summary>
        /// <returns></returns>
        public UIElement LoginPage => new Login { Desc = "搬运工平台基础样例" };

        /// <summary>
        /// 系统启动
        /// </summary>
        /// <param name="p_info">提示信息</param>
        public async Task OnStartup(StartupInfo p_info)
        {
            // 设置固定菜单项
            MenuKit.FixedMenus = new List<OmMenu>
            {
                new OmMenu(
                    ID: 1110,
                    Name: "通讯录",
                    Icon: "留言",
                    ViewName: "通讯录"),
            };

            if (ViewTypes.TryGetValue("主页", out var type) && type == typeof(DefaultHome))
            {
                // 联网模式
                // 更新打开模型库
                string error = await AtApp.OpenModelDb("cm");
                if (!string.IsNullOrEmpty(error))
                {
                    p_info.SetMessage(error);
                    return;
                }

                string phone = AtState.GetCookie("LoginPhone");
                string pwd = AtState.GetCookie("LoginPwd");
                if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(pwd))
                {
                    // 自动登录
                    Dict dt = await AtCm.LoginByPwd(phone, pwd);
                    if (dt.Bool("valid"))
                    {
                        // 登录成功
                        AtApp.LoginSuccess(dt);
                        return;
                    }
                }

                // 未登录或登录失败
                AtSys.Login(false);
            }
            else
            {
                // 单机模式
                AtApp.LoadRootUI();
            }
        }

        /// <summary>
        /// 接收分享内容
        /// </summary>
        /// <param name="p_info">分享内容描述</param>
        public void ReceiveShare(ShareInfo p_info)
        {
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
        /// 获取视图字典
        /// </summary>
        public Dictionary<string, Type> ViewTypes { get; } = new Dictionary<string, Type>
        {
            { "主页", typeof(Dt.App.DefaultHome) },
            { "报表", typeof(Dt.App.ReportView) },
            { "流程设计", typeof(Dt.App.Workflow.WorkflowMgr) },
            { "任务", typeof(Dt.App.Workflow.TasksView) },
            { "发布管理", typeof(Dt.App.Publish.PublishMgr) },
            { "发布", typeof(Dt.App.Publish.PublishView) },
            { "基础选项", typeof(Dt.App.Model.BaseOption) },
            { "菜单管理", typeof(Dt.App.Model.SysMenu) },
            { "我的设置", typeof(Dt.App.Model.MyParamsSetting) },
            { "参数定义", typeof(Dt.App.Model.UserParamsMgr) },
            { "基础权限", typeof(Dt.App.Model.BasePrivilege) },
            { "报表设计", typeof(Dt.App.Model.ReportMgr) },
            { "系统角色", typeof(Dt.App.Model.SysRole) },
            { "用户账号", typeof(Dt.App.Model.UserAccount) },
            { "文件", typeof(Dt.App.File.FileHome) },
            { "通讯录", typeof(Dt.App.Chat.ChatHome) },
        };

        /// <summary>
        /// 处理服务器推送的类型字典
        /// </summary>
        public Dictionary<string, Type> PushHandlers { get; } = new Dictionary<string, Type>
        {
            { "syspushapi", typeof(Dt.Base.SysPushApi) },
        };

        /// <summary>
        /// 获取自定义可序列化类型字典
        /// </summary>
        public Dictionary<string, Type> SerializeTypes { get; } = new Dictionary<string, Type>
        {
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
                    Version = "fb870916510cceb33282093c12a11311",
                    Tables = new List<Type>
                    {
                        typeof(Dt.App.MenuFav),
                        typeof(Dt.App.UserMenu),
                        typeof(Dt.App.File.ReadFileHistory),
                        typeof(Dt.Base.ChatMember),
                        typeof(Dt.Base.Letter),
                        typeof(Dt.Base.Docking.DockLayout),
                        typeof(Dt.Base.FormView.CellLastVal),
                        typeof(Dt.Base.FormView.SearchFvHis),
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