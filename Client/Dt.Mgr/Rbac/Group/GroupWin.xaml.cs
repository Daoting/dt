#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2023-03-08 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.Mgr.Rbac
{
    [View("分组")]
    public partial class GroupWin : Win
    {
        public GroupWin()
        {
            InitializeComponent();
        }

        public GroupList MainList => _mainList;

        public GroupForm MainForm => _mainForm;

        public GroupRoleList RoleList => _roleList;

        public GroupUserList UserList => _userList;

    }
}