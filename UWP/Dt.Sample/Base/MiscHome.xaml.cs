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
    public sealed partial class MiscHome : Win
    {
        public MiscHome()
        {
            InitializeComponent();
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础事件", typeof(RouteEventDemo), Icons.汉堡),
                new Nav("分隔栏", typeof(SplitterDemo), Icons.分组),
                new Nav("可停靠面板", typeof(DockPanelDemo), Icons.全选) { Desc = "停靠式窗口的布局面板" },
                new Nav("控件事件顺序", typeof(TestInvokeDemo), Icons.乐谱) { Desc = "测试不同平台主事件的调用顺序" },
            };
        }
    }
}
