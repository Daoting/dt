#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dt.Base;
using Dt.Core;
using Dt.Sample.ModuleView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class MvHome : Win
    {
        public MvHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav { Type = typeof(MvNavi), Title = "Tab内导航", Desc = "Mv之间导航时输入输出参数、带遮罩的模式视图" },
                new Nav { Type = typeof(SearchMvWin), Title = "搜索面板", Desc = "通用搜索功能，包括固定搜索项、历史搜索项、统一搜索事件、统一导航等功能" },
                new Nav { Type = typeof(NavListDemo), Title = "功能列表面板", Desc = "" },
                new Nav { Type = typeof(MyEntityWin), Title = "单表模板", Desc = "单表增删改模板" },
                new Nav { Type = typeof(MainWin), Title = "多对多模板", Desc = "多对多增删改模板" },
                new Nav { Type = typeof(ModuleView.OneToMany1.ShoppingWin), Title = "一对多，三栏", Desc = "一对多增删改模板" },
                new Nav { Type = typeof(ModuleView.OneToMany2.ShoppingWin), Title = "一对多，两栏", Desc = "一对多增删改模板" },
            };
        }
    }
}
