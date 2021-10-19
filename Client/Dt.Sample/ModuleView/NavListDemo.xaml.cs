#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2021-10-19 创建
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

namespace Dt.Sample
{
    public partial class NavListDemo : Win
    {
        public NavListDemo()
        {
            InitializeComponent();
            OnTitleIconDesc(null, null);
        }

        void OnTitleIconDesc(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础视图", typeof(LvViewBase), Icons.汉堡) { Desc = "三类视图，两种数据源，三种选择模式，支持分组三类视图，两种数据源，三种选择模式，支持分组", Warning = "12" },
                new Nav("表格视图", typeof(LvTable), Icons.分组),
                new Nav("列表视图", typeof(LvList), Icons.全选) { Desc = "只垂直滚动" },
                new Nav("磁贴视图", typeof(LvTile), Icons.日历) { Desc = "平铺式磁贴，一行多格，只垂直滚动" },
            };
        }

        void OnTitleIcon(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("表格视图", typeof(LvTable), Icons.分组),
                new Nav("基础视图", typeof(LvViewBase), Icons.汉堡) { Warning = "5" },
                new Nav("列表视图", typeof(LvList), Icons.全选),
                new Nav("磁贴视图", typeof(LvTile), Icons.日历),
            };
        }

        void OnTitleDesc(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础视图", typeof(LvViewBase)) { Desc = "三类视图，两种数据源，三种选择模式，支持分组三类视图，两种数据源，三种选择模式，支持分组", Warning = "12" },
                new Nav("表格视图", typeof(LvTable)),
                new Nav("列表视图", typeof(LvList)) { Desc = "只垂直滚动" },
                new Nav("磁贴视图", typeof(LvTile)) { Desc = "平铺式磁贴，一行多格，只垂直滚动" },
            };
        }

        void OnTitle(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("列表视图", typeof(LvList)),
                new Nav("基础视图", typeof(LvViewBase)) { Warning = "1" },
                new Nav("表格视图", typeof(LvTable)),
                new Nav("磁贴视图", typeof(LvTile)),
            };
        }

        void OnMisc(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("基础视图", typeof(LvViewBase), Icons.汉堡) { Desc = "三类视图，两种数据源，三种选择模式，支持分组三类视图，两种数据源，三种选择模式，支持分组", Warning = "12" },
                new Nav("表格视图", typeof(LvTable), Icons.分组),
                new Nav("列表视图", typeof(LvList)) { Desc = "只垂直滚动" },
                new Nav("磁贴视图", typeof(LvTile)),
            };
        }

        void OnWarning(object sender, RoutedEventArgs e)
        {
            ((Nav)_nav.Data[0]).Warning = new Random().Next(20).ToString();
        }

        void OnCustomTemp(object sender, RoutedEventArgs e)
        {
            _nav.ItemTemplate = (DataTemplate)Resources["CustomView"];
        }

        void OnDefaultTemp(object sender, RoutedEventArgs e)
        {
            _nav.ItemTemplate = null;
        }

        void OnAutoHeight(object sender, RoutedEventArgs e)
        {
            _nav.ItemHeight = double.NaN;
        }
    }
}