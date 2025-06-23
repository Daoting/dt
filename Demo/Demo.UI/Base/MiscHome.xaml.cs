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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Demo.UI
{
    public sealed partial class MiscHome : Win
    {
        public MiscHome()
        {
            InitializeComponent();
            _nav.Data = Dir;
        }

        public static Nl<Nav> Dir { get; } = new Nl<Nav>
        {
            new Nav("Tab页", typeof(TabControlDemo), Icons.排列) { Desc = "传统TabControl控件" },
            new Nav("基础事件", typeof(RouteEventDemo), Icons.汉堡),
            new Nav("分隔栏", typeof(SplitterDemo), Icons.分组),
            new Nav("可停靠面板", typeof(DockPanelDemo), Icons.全选) { Desc = "停靠式窗口的布局面板" },
            new Nav("控件虚方法顺序", typeof(TestInvokeDemo), Icons.乐谱) { Desc = "测试不同平台控件虚方法的调用顺序" },
        };
    }
}
