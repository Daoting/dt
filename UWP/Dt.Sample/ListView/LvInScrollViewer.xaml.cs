#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvInScrollViewer : Win
    {
        public LvInScrollViewer()
        {
            InitializeComponent();
            _lv.View = Resources["ListView"];
            //_lv.GroupName = "bumen";
            //_lv.ItemHeight = double.NaN;
            _lv.Data = SampleData.CreatePersonsTbl(50);
        }

        void OnRowHeight(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = 0;
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _lv.Data = SampleData.CreatePersonsTbl(int.Parse(((Button)sender).Tag.ToString()));
        }

        void OnLoadNull(object sender, RoutedEventArgs e)
        {
            _lv.Data = null;
        }

        void OnGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = "bumen";
        }

        void OnDelGroup(object sender, RoutedEventArgs e)
        {
            _lv.GroupName = null;
        }

        void OnAutoHeight(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = double.NaN;
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, _lv.Data.Count);
            _lv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 行");
        }

        void OnTopMax(object sender, RoutedEventArgs e)
        {
            _top.Height = _sv.ViewportHeight + 100;
        }

        void OnTop(object sender, RoutedEventArgs e)
        {
            _top.Height = _sv.ViewportHeight / 2;
        }

        void OnTopZero(object sender, RoutedEventArgs e)
        {
            _top.Height = 0;
        }

        void OnBottomMax(object sender, RoutedEventArgs e)
        {
            _bottom.Height = _sv.ViewportHeight + 100;
        }

        void OnBottom(object sender, RoutedEventArgs e)
        {
            _bottom.Height = _sv.ViewportHeight / 2;
        }

        void OnBottomZero(object sender, RoutedEventArgs e)
        {
            _bottom.Height = 0;
        }

        void OnScrollTop(object sender, RoutedEventArgs e)
        {
            _lv.ScrollTop();
        }

        void OnScrollBottom(object sender, RoutedEventArgs e)
        {
            _lv.ScrollBottom();
        }

        void OnToolbar(object sender, RoutedEventArgs e)
        {
            var temp = (DataTemplate)Resources["Toolbar"];
            _lv.Toolbar = temp.LoadContent() as Menu;
        }

        void OnDelToolbar(object sender, RoutedEventArgs e)
        {
            _lv.Toolbar = null;
        }

        void OnToggleViewMode(object sender, Mi e)
        {
            if (_lv.ViewMode == ViewMode.Tile)
            {
                _lv.ChangeView(Resources["ListView"], ViewMode.List);
                e.Icon = Icons.排列;
            }
            else
            {
                _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
                e.Icon = Icons.汉堡;
            }
        }
    }
}