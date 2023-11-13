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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#endregion

namespace Dt.UIDemo
{
    public partial class LvListStyle : Win
    {
        public LvListStyle()
        {
            InitializeComponent();
            _lv.View = Resources["Table"];
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

        void OnSelectNull(object sender, RoutedEventArgs e)
        {
            _lv.SelectedItem = null;
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, _lv.Data.Count);
            _lv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 行");
        }

        void OnLoadStyle(object sender, RoutedEventArgs e)
        {
            _lv.View = Resources[((Button)sender).Tag];
        }

        void OnFilterCfg(object sender, RoutedEventArgs e)
        {
            _lv.FilterCfg = new FilterCfg();
        }

        void OnDelFilter(object sender, RoutedEventArgs e)
        {
            _lv.FilterCfg = null;
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

        void OnCustomHeader(object sender, RoutedEventArgs e)
        {
            var temp = (DataTemplate)Resources["CustomHeader"];
            _lv.CustomListHeader = temp.LoadContent() as FrameworkElement;
        }

        void OnDelCustomHeader(object sender, RoutedEventArgs e)
        {
            _lv.CustomListHeader = null;
        }
    }
}