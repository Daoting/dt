#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-09-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.Mgr.Rbac
{
    [View(LobViews.菜单管理)]
    public partial class MenuWin : Win
    {
        public MenuWin()
        {
            InitializeComponent();
            Form = new MenuForm { OwnWin = this };
        }

        public MenuTree Tree => _tree;
        
        public MenuList List => _list;

        public MenuForm Form { get; }

        public MenuRoleList RoleList => _roleList;
    }
}