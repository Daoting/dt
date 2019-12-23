﻿#region 文件描述
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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Sample
{
    public partial class BaseChart : Win
    {
        ChartSampleData _data;

        public BaseChart()
        {
            InitializeComponent();

            _chart.View.AxisY.Title = "成绩";
            _data = new ChartSampleData();

            _cbType.ItemsSource = EnumDataSource.FromType(typeof(ChartType));
            _cbType.SelectionChanged += OnChartTypeSelectionChanged;

            _chart.Data = _data.GetData(ChartType.Column);
            _chart.ChartType = ChartType.Column;
        }

        private void OnChartTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartType tp = (ChartType)((EnumMember)_cbType.SelectedItem).Value;
            _chart.Data = _data.GetData(tp);
            _chart.ChartType = tp;
        }

        private async void OnSnapshot(object sender, RoutedEventArgs e)
        {
            var file = await _chart.SaveSnapshot();
            if (file != null)
                AtKit.Msg(string.Format("截图【{0}】保存成功！", file.Name));
        }
    }
}