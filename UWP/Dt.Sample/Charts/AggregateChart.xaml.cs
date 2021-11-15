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
    public partial class AggregateChart : Win
    {
        Dictionary<string, Brush> _dict = new Dictionary<string, Brush>();
        Random rnd = new Random();

        public AggregateChart()
        {
            InitializeComponent();

            _dict["red"] = CreateBrush(Colors.Red);
            _dict["blue"] = CreateBrush(Color.FromArgb(255, 2, 150, 252));
            _dict["yellow"] = CreateBrush(Colors.Yellow);

            _cb.ItemsSource = EnumDataSource.FromType(typeof(Aggregate));
            _cb.SelectedIndex = 0;
            _cb.SelectionChanged += (s, e) => _chart.Aggregate = (Aggregate)((EnumMember)_cb.SelectedItem).Value;

            int cnt = 30;
            _chart.Data.ItemNames = CreateRandomStrings(cnt, new string[] { "blue", "red", "yellow" }); ;

            var vals = CreateRandomValues(cnt);

            var ds = new DataSeries() { ValuesSource = vals };

            ds.PlotElementLoaded += (s, e) =>
            {
                PlotElement pe = (PlotElement)s;
                if (_dict.ContainsKey(pe.DataPoint.Name))
                    pe.Fill = _dict[pe.DataPoint.Name];
                pe.StrokeThickness = 0;
            };

            _chart.Data.Children.Add(ds);
            _chart.View.AxisX.AnnoVisibility = AnnoVisibility.ShowAll;
            BarColumnOptions.SetRadiusX(_chart, 4);
            BarColumnOptions.SetRadiusY(_chart, 4);
        }

        double[] CreateRandomValues(int cnt)
        {
            double[] vals = new double[cnt];
            for (int i = 0; i < cnt; i++)
                vals[i] = rnd.NextDouble() * 100;
            return vals;
        }

        string[] CreateRandomStrings(int cnt, string[] list)
        {
            string[] vals = new string[cnt];
            for (int i = 0; i < cnt; i++)
                vals[i] = list[rnd.Next(0, list.Length)];
            return vals;
        }

        Brush CreateBrush(Color clr)
        {
            return new LinearGradientBrush()
            {
                GradientStops =
                {
                  new GradientStop() { Color= Color.FromArgb(128, clr.R,clr.G,clr.B) , Offset = 0},
                  new GradientStop() { Color= clr, Offset = 0.2},
                  new GradientStop() { Color= clr, Offset = 0.7},
                  new GradientStop() { Color= Color.FromArgb(128, clr.R,clr.G,clr.B) , Offset = 1},
                },
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 1),
            };
        }
    }
}