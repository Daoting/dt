#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Base.Tools;
using Dt.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Data;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public sealed partial class TestLvLeak : Win
    {
        public TestLvLeak()
        {
            InitializeComponent();
            _lv.View = Resources["TableView"];
            _lv.GroupName = "bumen";
            _lv.Data = SampleData.CreatePersonsTbl(100);
        }

        void OnLoadData(object sender, RoutedEventArgs e)
        {
            _lv.Data = SampleData.CreatePersonsTbl(int.Parse(((Button)sender).Tag.ToString()));
        }

        void OnLoadNull(object sender, RoutedEventArgs e)
        {
            _lv.Data = null;
        }
        
        void OnGridView(object sender, RoutedEventArgs e)
        {
            SelectTab(0);
            _lv.ChangeView(Resources["TableView"], ViewMode.Table);
        }

        void OnListView(object sender, RoutedEventArgs e)
        {
            SelectTab(0);
            _lv.ChangeView(Resources["ListView"], ViewMode.List);
        }

        void OnFormList(object sender, RoutedEventArgs e)
        {
            SelectTab(0);
            _lv.ChangeView(Resources["TableView"], ViewMode.List);
        }

        void OnTileView(object sender, RoutedEventArgs e)
        {
            SelectTab(0);
            _lv.ChangeView(Resources["TileView"], ViewMode.Tile);
        }

        void OnFormTile(object sender, RoutedEventArgs e)
        {
            SelectTab(0);
            _lv.ChangeView(Resources["TableView"], ViewMode.Tile);
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

        void OnVir(object sender, RoutedEventArgs e)
        {
            _lv.ItemHeight = 0;
        }
    }
}