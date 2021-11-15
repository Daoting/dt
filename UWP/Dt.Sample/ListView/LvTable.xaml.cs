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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class LvTable : Win
    {
        public LvTable()
        {
            InitializeComponent();
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

        void OnFormView(object sender, RoutedEventArgs e)
        {
            _lv.ViewMode = ViewMode.List;
        }

        void OnSelectNull(object sender, RoutedEventArgs e)
        {
            _lv.SelectedItem = null;
        }

        void OnScroll(object sender, RoutedEventArgs e)
        {
            int index = new Random().Next(0, ((IList)_lv.Data).Count);
            _lv.ScrollInto(index);
            Kit.Msg($"滚动到第 {index + 1} 行");
        }
    }
}