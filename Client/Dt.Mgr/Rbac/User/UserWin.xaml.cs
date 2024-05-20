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
    [View(LobViews.用户账号)]
    public partial class UserWin : Win
    {
        public UserWin()
        {
            InitializeComponent();
            MainForm = new UserForm { OwnWin = this };
        }

        public UserList MainList => _mainList;

        public UserForm MainForm { get; }

        public UserGroupList GroupList => _groupList;

        public UserRoleList RoleList => _roleList;

        public UserMenuList MenuList => _menuList;

        public UserPerList PerList => _perList;

        public UserQuery Query => _query;

        void OnQuery(QueryClause e)
        {
            _mainList.OnSearch(e);
        }
    }
}