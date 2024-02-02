#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-09 创建
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
        }

        public RoleList MainList => _mainList;

        public RoleForm MainForm => _mainForm;

        public RoleUserList UserList => _userList;

        public RoleMenuList MenuList => _menuList;

        public RolePerList PerList => _permissionList;

        public RoleGroupList GroupList => _groupList;

    }
}