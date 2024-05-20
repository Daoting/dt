#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-02-04 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    [View(LobViews.分组管理)]
    public partial class GroupWin : Win
    {
        public GroupWin()
        {
            InitializeComponent();
            MainForm = new GroupForm { OwnWin = this };
        }

        public GroupList MainList => _mainList;

        public GroupForm MainForm { get; }

        public GroupUserList UserList => _userList;

        public GroupRoleList RoleList => _roleList;

        public FuzzySearch Query => _query;

        void OnSearch(string e)
        {
            _mainList.OnSearch(new QueryClause(e));
        }
    }
}