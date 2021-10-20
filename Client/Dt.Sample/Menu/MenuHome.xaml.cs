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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public sealed partial class MenuHome : Win
    {
        public MenuHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("工具栏菜单", typeof(MenuDemo), Icons.保存) { Desc = "工具栏样式，支持单选、多层" },
                new Nav("上下文菜单", typeof(ContextMenuDemo), Icons.日历) { Desc = "附加到可视元素，支持继承数据源" },
            };
        }
    }
}
