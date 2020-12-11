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
            _lv.Data = new Nl<MainInfo>
            {
                new MainInfo(Icons.保存, "工具栏菜单", typeof(MenuDemo), "工具栏样式，支持单选、多层"),
                new MainInfo(Icons.日历, "上下文菜单", typeof(ContextMenuDemo), "附加到可视元素，支持继承数据源"),
            };
        }
    }
}
