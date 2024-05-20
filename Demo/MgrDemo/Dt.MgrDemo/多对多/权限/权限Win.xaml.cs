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
    [View("权限")]
    public partial class 权限Win : Win
    {
        public 权限Win()
        {
            InitializeComponent();
            MainForm = new 权限Form { OwnWin = this };
        }

        public 权限List MainList => _mainList;

        public 权限Form MainForm { get; }

        public 权限角色List 角色List => _角色List;

        public 权限Query Query => _query;

        void OnQuery(QueryClause e)
        {
            _mainList.OnSearch(e);
        }
    }
}