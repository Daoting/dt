#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
#endregion

namespace Dt.UIDemo
{
    public partial class ChartLabel : Win
    {

        public ChartLabel()
        {
            InitializeComponent();

            _cbType.ItemsSource = new List<string> { "Column", "LineSymbols", "Pie" };
        }

        void OnChartTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _chart.ChartType = (ChartType)Enum.Parse(typeof(ChartType), _cbType.SelectedItem.ToString(), false);
        }
    }
}