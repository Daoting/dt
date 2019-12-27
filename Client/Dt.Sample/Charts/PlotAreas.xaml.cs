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
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
#endregion

namespace Dt.Sample
{
    public partial class PlotAreas : Win
    {
        public PlotAreas()
        {
            InitializeComponent();

            NewData();
            _chart.PointerPressed += (s, e) => NewData();
        }

        void NewData()
        {
            _chart.BeginUpdate();
            _chart.Reset(true);

            _chart.ChartType = ChartType.Line;

            var ax = _chart.View.AxisX;
            ax.Title = "X0";
            ax.Position = AxisPosition.Far | AxisPosition.DisableLastLabelOverflow;
            ax.Min = 0;
            ax.Max = 1;

            for (int i = 0; i < 6; i++)
            {
                if (i > 0)
                {
                    var axisname = "X" + i;
                    _chart.View.Axes.Add(new Axis()
                    {
                        AxisType = AxisType.X,
                        Position = AxisPosition.Far | AxisPosition.DisableLastLabelOverflow,
                        Name = axisname,
                        PlotAreaIndex = i,
                        Title = axisname,
                        MajorGridStroke = _chart.View.AxisX.MajorGridStroke,
                        Min = 0,
                        Max = 1
                    });
                    var ds = ChartSampleData.CreateDataSeries(100, true);
                    ds.AxisX = axisname;
                    _chart.Data.Children.Add(ds);
                }
                else
                {
                    var ds = ChartSampleData.CreateDataSeries(100, true);
                    _chart.Data.Children.Add(ds);
                }
            }

            var ay = _chart.View.AxisY;
            ay.Reversed = true;
            ay.Title = "Depth, meters";

            _chart.EndUpdate();
        }
    }
}