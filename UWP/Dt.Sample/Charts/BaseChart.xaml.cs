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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Sample
{
    public partial class BaseChart : Win
    {
        ChartSampleData _data;

        public BaseChart()
        {
            InitializeComponent();

            //_chart.View.AxisX.Title = "课程";
            _chart.View.AxisY.Title = "成绩";

            _data = new ChartSampleData();

            _chart.Data = _data.GetData(ChartType.Column);
            _chart.ChartType = ChartType.Column;
        }

        void OnChartTypeChanged(object sender, object e)
        {
            _chart.Data = _data.GetData((ChartType)e);
        }

        async void OnSnapshot(object sender, RoutedEventArgs e)
        {
            var file = await _chart.SaveSnapshot();
            if (file != null)
                Kit.Msg(string.Format("截图【{0}】保存成功！", file.Name));
        }

        void OnMajorGrid(object sender, RoutedEventArgs e)
        {
            _chart.BeginUpdate();
            if (_chart.View.AxisY.MajorGridFill == null)
                _chart.View.AxisY.MajorGridFill = new SolidColorBrush(Colors.Yellow);
            else
                _chart.View.AxisY.MajorGridFill = null;
            _chart.EndUpdate();
        }
    }
}