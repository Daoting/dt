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
                new Nav("空白窗口", typeof(BlankWin), Icons.修改) { Desc = "行/项高度默认NaN，表示自动高度，因NavList的数据行数较少，每项的内容多少不同！设置0时表示以第一项高度为准，大于0时表示固定行/项高度。", Warning = "12" },
                new Nav("主区窗口", typeof(SingleViewWin), Icons.打印) { Desc = "有标题栏的空白窗口" },
                new Nav("对话框", typeof(DlgDemo), Icons.日历) { Desc = "模拟传统对话框" },
                new Nav("查找图标", typeof(IconDemo), Icons.图片) { Desc = "内置的矢量文字，可用作图标、提示" },
            };
        }

        void OnTitleIcon(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("主区窗口", typeof(SingleViewWin), Icons.打印),
                new Nav("对话框", typeof(DlgDemo), Icons.日历) { Warning = "5" },
                new Nav("查找图标", typeof(IconDemo), Icons.图片),
                new Nav("空白窗口", typeof(BlankWin), Icons.修改),
            };
        }

        void OnTitleDesc(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("查找图标", typeof(IconDemo)) { Desc = "内置的矢量文字，可用作图标、提示" },
                new Nav("空白窗口", typeof(BlankWin)) { Desc = "行/项高度默认NaN，表示自动高度，因NavList的数据行数较少，每项的内容多少不同！设置0时表示以第一项高度为准，大于0时表示固定行/项高度。" },
                new Nav("主区窗口", typeof(SingleViewWin)) { Desc = "有标题栏的空白窗口" },
                new Nav("对话框", typeof(DlgDemo)) { Warning = "2", Desc = "模拟传统对话框" },
            };
        }

        void OnTitle(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("对话框", typeof(DlgDemo)) { Warning = "1" },
                new Nav("主区窗口", typeof(SingleViewWin)),
                new Nav("查找图标", typeof(IconDemo)),
                new Nav("空白窗口", typeof(BlankWin)),
            };
        }

        void OnMisc(object sender, RoutedEventArgs e)
        {
            _nav.Data = new Nl<Nav>
            {
                new Nav("查找图标", typeof(IconDemo)),
                new Nav("主区窗口", typeof(SingleViewWin)) { Desc = "有标题栏的空白窗口" },
                new Nav("对话框", typeof(DlgDemo), Icons.日历),
                new Nav("空白窗口", typeof(BlankWin), Icons.修改) { Desc = "行/项高度默认NaN，表示自动高度，因NavList的数据行数较少，每项的内容多少不同！设置0时表示以第一项高度为准，大于0时表示固定行/项高度。", Warning = "12" },
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