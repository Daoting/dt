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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
#endregion

namespace Dt.App.Model
{
    [View("菜单管理")]
    public partial class MenuWin : Win
    {
        public MenuWin()
        {
            InitializeComponent();
        }

        public MenuList List => _list;

        public MenuForm Form => _form;

        public MenuRoleList RoleList => _roleList;

    }
}