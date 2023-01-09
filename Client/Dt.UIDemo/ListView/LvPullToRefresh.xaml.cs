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

namespace Dt.Sample
{
    public partial class LvPullToRefresh : Win
    {
        public LvPullToRefresh()
        {
            InitializeComponent();
            _lv.Data = SampleData.CreatePersonsTbl(10);
            _lv.RefreshRequested += OnRefreshRequested;
        }

        async void OnRefreshRequested(object sender, AsyncEventArgs e)
        {
            using (e.Wait())
            {
                await Task.Delay(2000);
                var tbl = _lv.Table;
                _lv.Data = SampleData.CreatePersonsTbl(tbl.Count + 10);
            }
        }

        void OnRequestRefresh(object sender, RoutedEventArgs e)
        {
            _lv.RequestRefresh();
        }

        void OnPullToRefreshCheck(object sender, RoutedEventArgs e)
        {
            _lv.PullToRefresh = (bool)_cb.IsChecked;
        }
    }
}