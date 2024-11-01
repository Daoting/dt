#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Microsoft.UI.Xaml;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Hatches;
using ScottPlot.Plottables;
#endregion

namespace Demo.UI
{
    public partial class Chart2Radar : Win
    {
        double[,] _values = {
                    { 78,  83, 84, 76, 43 },
                    { 100, 50, 70, 60, 90 }
                };
        
        public Chart2Radar()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 78, 83, 84, 76, 43 };
                _c.Add.Radar(values);
            }
        }

        void OnArray(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                
                _c.Add.Radar(_values);
            }
        }

        void OnLang(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var radar = _c.Add.Radar(_values);
                radar.Series[0].LegendText = "Sebastian";
                radar.Series[1].LegendText = "Fernando";
            }
        }

        void OnRadarSeries(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var radar = _c.Add.Radar(_values);

                radar.Series[0].FillColor = Colors.Transparent;
                radar.Series[0].LineColor = Colors.Blue;
                radar.Series[0].LineWidth = 3;
                radar.Series[0].LinePattern = LinePattern.DenselyDashed;

                radar.Series[1].FillColor = Colors.Green.WithAlpha(.2);
                radar.Series[1].LineColor = Colors.Green;
                radar.Series[1].LineWidth = 2;
            }
        }

        void OnSpokeLabel(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var radar = _c.Add.Radar(_values);

                string[] spokeLabels = { "Wins", "Poles", "Podiums", "Points", "DNFs" };
                radar.PolarAxis.SetSpokes(spokeLabels, length: 110);
            }
        }

        void OnPolarAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var radar = _c.Add.Radar(_values);

                double[] tickPositions = { 25, 50, 75, 100 };
                string[] tickLabels = tickPositions.Select(x => x.ToString()).ToArray();
                radar.PolarAxis.SetCircles(tickPositions, tickLabels);
            }
        }

        void OnStraightLine(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 78, 83, 100, 76, 43 };
                var radar = _c.Add.Radar(values);
                radar.PolarAxis.StraightLines = true;
            }
        }
    }
}