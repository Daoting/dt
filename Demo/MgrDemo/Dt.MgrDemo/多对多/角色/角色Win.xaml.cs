#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.MgrDemo
{
    [View("角色")]
    public partial class 角色Win : Win
    {
        public 角色Win()
        {
            InitializeComponent();
            MainForm = new 角色Form { OwnWin = this };
        }

        public 角色List MainList => _mainList;

        public 角色Form MainForm { get; }

        public 角色权限List 权限List => _权限List;

        public 角色用户List 用户List => _用户List;

        public 角色Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _mainList.OnSearch(e);
        }
    }
}