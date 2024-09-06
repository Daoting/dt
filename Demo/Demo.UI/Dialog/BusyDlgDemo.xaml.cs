#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2018-08-23 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Core;
using System;
using System.Collections.Generic;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI;

#endregion

namespace Demo.UI
{
    public sealed partial class BusyDlgDemo : Win
    {
        public BusyDlgDemo()
        {
            InitializeComponent();
            LoadData();
        }

        async void OnBusyWin(object sender, RoutedEventArgs e)
        {
            var dlg = this.Busy(_tbMsg.Text, _cbVeil.IsChecked.Value);
            await Task.Delay(3000);
            dlg.Close();
        }

        async void OnBusyFv(object sender, RoutedEventArgs e)
        {
            var dlg = _fv.Busy(_tbMsg.Text, _cbVeil.IsChecked.Value);
            await Task.Delay(3000);
            dlg.Close();
        }

        async void OnBusyTbl(object sender, RoutedEventArgs e)
        {
            var dlg = _lvTbl.Busy(_tbMsg.Text, _cbVeil.IsChecked.Value);
            await Task.Delay(3000);
            dlg.Close();
        }

        async void OnBusyList(object sender, RoutedEventArgs e)
        {
            var dlg = _lvList.Busy(_tbMsg.Text, _cbVeil.IsChecked.Value);
            await Task.Delay(3000);
            dlg.Close();
        }

        async void OnBusyTile(object sender, RoutedEventArgs e)
        {
            var dlg = _lvTile.Busy(_tbMsg.Text, _cbVeil.IsChecked.Value);
            await Task.Delay(3000);
            dlg.Close();
        }

        void LoadData()
        {
            var tbl = SampleData.CreatePersonsTbl(5);
            _lvList.Data = tbl;
            _lvTbl.Data = tbl;
            _lvTile.Data = tbl;
        }
    }
}
