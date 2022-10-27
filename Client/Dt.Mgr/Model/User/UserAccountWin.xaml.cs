#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
#endregion

namespace Dt.Mgr.Model
{
    [View(LobViews.用户账号)]
    public partial class UserAccountWin : Win
    {
        public UserAccountWin()
        {
            InitializeComponent();
        }

        public UserAccountList List => _list;

        public UserAccountForm Form => _form;

        public UserRoleList RoleList => _roleList;
    }
}