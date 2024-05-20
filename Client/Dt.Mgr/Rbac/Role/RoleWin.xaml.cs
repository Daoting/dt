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

namespace Dt.Mgr.Rbac
{
    [View(LobViews.系统角色)]
    public partial class RoleWin : Win
    {
        public RoleWin()
        {
            InitializeComponent();
            MainForm = new RoleForm { OwnWin = this };
        }

        public RoleList MainList => _mainList;

        public RoleForm MainForm { get; }

        public RoleUserList UserList => _userList;

        public RoleMenuList MenuList => _menuList;

        public RolePerList PermissionList => _permissionList;

        public RoleGroupList GroupList => _groupList;

        public FuzzySearch Query => _query;

        void OnSearch(string e)
        {
            _mainList.OnSearch(new QueryClause(e));
        }
    }
}