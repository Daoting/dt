#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2022-08-04
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 自动生成各类字典，生成方法：win版app -> 系统日志 -> 内置存根
    /// </summary>
    public abstract partial class DefaultStub : Stub
    {
        protected override Dictionary<string, Type> GetInternalViewTypes()
        {
            return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                { "通讯录", typeof(Dt.Base.Chat.ChatHome) },
                { "报表", Type.GetType("Dt.Mgr.ReportView,Dt.Mgr") },
                { "流程设计", Type.GetType("Dt.Mgr.Workflow.WorkflowMgr,Dt.Mgr") },
                { "任务", Type.GetType("Dt.Mgr.Workflow.TasksView,Dt.Mgr") },
                { "发布", Type.GetType("Dt.Mgr.Publish.PublishView,Dt.Mgr") },
                { "发布管理", Type.GetType("Dt.Mgr.Publish.PublishMgr,Dt.Mgr") },
                { "基础选项", Type.GetType("Dt.Mgr.Model.BaseOption,Dt.Mgr") },
                { "菜单管理", Type.GetType("Dt.Mgr.Model.MenuWin,Dt.Mgr") },
                { "我的设置", Type.GetType("Dt.Mgr.Model.MyParamsSetting,Dt.Mgr") },
                { "参数定义", Type.GetType("Dt.Mgr.Model.UserParamsWin,Dt.Mgr") },
                { "基础权限", Type.GetType("Dt.Mgr.Model.PrvWin,Dt.Mgr") },
                { "报表设计", Type.GetType("Dt.Mgr.Model.RptWin,Dt.Mgr") },
                { "系统角色", Type.GetType("Dt.Mgr.Model.RoleWin,Dt.Mgr") },
                { "用户账号", Type.GetType("Dt.Mgr.Model.UserAccountWin,Dt.Mgr") },
                { "文件", Type.GetType("Dt.Mgr.File.FileHome,Dt.Mgr") },
            };
        }

        protected override Dictionary<string, SqliteTblsInfo> GetInternalSqliteDbs()
        {
            return new Dictionary<string, SqliteTblsInfo>(StringComparer.OrdinalIgnoreCase)
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
                            Type.GetType("Dt.Mgr.MenuFav,Dt.Mgr"),
                            Type.GetType("Dt.Mgr.UserMenu,Dt.Mgr"),
                            Type.GetType("Dt.Mgr.File.ReadFileHistory,Dt.Mgr"),
                        }
                    }
                },
            };
        }
    }
}