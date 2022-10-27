﻿#region 文件描述
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
    [View(LobViews.系统角色)]
    public partial class RoleWin : Win
    {
        public RoleWin()
        {
            InitializeComponent();
        }

        public RoleList List => _list;

        public RoleForm Form => _form;

        public RoleUserList UserList => _userList;

        public RoleMenuList MenuList => _menuList;

        public RolePrvList PrvList => _prvList;
    }
}