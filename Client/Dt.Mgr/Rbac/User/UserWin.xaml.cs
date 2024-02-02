#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-06 创建
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
        }

        public UserList MainList => _mainList;

        public UserForm MainForm => _mainForm;

        public UserGroupList GroupList => _groupList;

        public UserRoleList RoleList => _roleList;

    }
}