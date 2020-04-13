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
            _lv.View = GetResource("ListView");
            _lv.GroupName = "bumen";
            _lv.Data = SampleData.CreatePersonsTbl(50);
        }

        void OnRowHeight(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = 0;
        }

        void OnGridView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TableView"), ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("ListView"), ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            _lv.ChangeView(GetResource("TileView"), ViewMode.Tile);
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
            AtKit.Msg($"滚动到第 {index + 1} 行");
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

        object GetResource(string p_key)
        {
#if UWP
            return Resources[p_key];
#else
            if (p_key == "ListView")
                return StaticResources.ListView;
            if (p_key == "TileView")
                return StaticResources.TileView;
            return StaticResources.TableView;
#endif
        }
    }
}