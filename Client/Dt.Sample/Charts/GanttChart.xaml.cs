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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    public partial class GanttChart : Win
    {
        public GanttChart()
        {
            InitializeComponent();

            // 调换xy轴
            _chart.View.Inverted = true;
            _chart.View.PlotBackground = new SolidColorBrush(Colors.White);

            // x轴为时间
            _chart.View.AxisX.IsTime = true;

            // 设置y轴
            Axis yAxis = _chart.View.AxisY;
            yAxis.Title = "分钟";
            yAxis.MajorUnit = 20;
            yAxis.MinorGridStrokeThickness = 1;

            HighLowSeries ds = new HighLowSeries() { Label = "Left Room" };
            ds.XValuesSource = new DateTime[] { new DateTime(2013, 10, 1), new DateTime(2013, 10, 1) };
            ds.LowValuesSource = new double[] { 10, 30 };
            ds.HighValuesSource = new double[] { 20, 50 };
            ds.PointTooltipTemplate = Resources["label"] as DataTemplate;
            _chart.Data.Children.Add(ds);

            ds = new HighLowSeries() { Label = "Left Early" };
            ds.XValuesSource = new DateTime[] { new DateTime(2013, 9, 15), new DateTime(2013, 10, 11), new DateTime(2013, 10, 16) };
            ds.LowValuesSource = new double[] { 40, 53, 46 };
            ds.HighValuesSource = new double[] { 60, 60, 60 };
            ds.PointTooltipTemplate = Resources["label"] as DataTemplate;
            _chart.Data.Children.Add(ds);

            ds = new HighLowSeries() { Label = "Arrived Late" };
            ds.XValuesSource = new DateTime[] { new DateTime(2013, 9, 5), new DateTime(2013, 10, 16) };
            ds.LowValuesSource = new double[] { 0, 2 };
            ds.HighValuesSource = new double[] { 10, 20 };
            ds.PointTooltipTemplate = Resources["label"] as DataTemplate;
            ds.SymbolSize = new Size(30, 30);
            _chart.Data.Children.Add(ds);
        }
    }
}