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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class LvGroupTemplate : Win
    {
        public LvGroupTemplate()
        {
            InitializeComponent();

            _lv.GroupName = "bumen";
            _lv.GroupContext = typeof(MyGroupContext);
            _lv.Data = SampleData.CreatePersonsTbl(100);
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
    }

    public class MyGroupContext : GroupContext
    {
        public double Sum => SumDouble("shengao");

        public string Average => AverageDouble("shengao").ToString("n2");

        public double Max => MaxDouble("shengao");

        public double Min => MinDouble("shengao");
    }
}