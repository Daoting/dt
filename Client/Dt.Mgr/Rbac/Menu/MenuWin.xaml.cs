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
        readonly MenuForm _form;
        
        public MenuWin()
        {
            InitializeComponent();
            _form = new MenuForm { OwnWin = this };
            Attach();
        }

        public MenuTree Tree => _tree;
                
        void Attach()
        {
            _tree.Msg += e => _list.Query(e.ID);
            _tree.Navi += () => NaviTo(_list.Title + "," + _roleList.Title);

            _list.Msg += e => _ = _form.Query(e);

            _form.UpdateList += e =>
            {
                _ = _list.Refresh(e.ID);
                var data = _form.Data;
                if (data != null && data.IsGroup)
                    _ = _tree.Refresh();
            };
            _form.UpdateRelated += e => _roleList.Query(e.ID);
        }
    }
}