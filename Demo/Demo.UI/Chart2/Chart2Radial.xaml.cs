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
#endregion

namespace Demo.UI
{
    public partial class Chart2Radial : Win
    {
        public Chart2Radial()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 100, 80, 65, 45, 20 };
                _c.Add.RadialGaugePlot(values);
            }
        }

        void OnColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };
                _c.Add.RadialGaugePlot(values);
            }
        }

        void OnVal(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, -65, 45, -20 };
                _c.Add.RadialGaugePlot(values);
            }
        }

        void OnSequential(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 50 };
                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.GaugeMode = ScottPlot.RadialGaugeMode.Sequential;
            }
        }

        void OnOrder(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 50 };
                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.GaugeMode = ScottPlot.RadialGaugeMode.Sequential;
                radialGaugePlot.OrderInsideOut = false;
            }
        }

        void OnSingle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.GaugeMode = ScottPlot.RadialGaugeMode.SingleGauge;
                radialGaugePlot.MaximumAngle = 180;
                radialGaugePlot.StartingAngle = 180;
            }
        }

        void OnOri(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.Clockwise = false;
            }
        }

        void OnSize(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };
                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.SpaceFraction = .05;
            }
        }
        
        void OnStartingAngle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.StartingAngle = 180;
            }
        }

        void OnMaximumAngle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.MaximumAngle = 180;
            }
        }

        void OnShowLevel(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.ShowLevels = false;
            }
        }

        void OnPositionFraction(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.LabelPositionFraction = 0.5;
            }
        }

        void OnFontSizeFraction(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.FontSizeFraction = .4;
            }
        }

        void OnFontColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.Font.Color = Colors.Black;
            }
        }

        void OnLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.Labels = new string[] { "alpha", "beta", "gamma", "delta", "epsilon" };
                _c.ShowLegend();
            }
        }

        void OnTransparent(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.BackgroundTransparencyFraction = .5;
            }
        }

        void OnCircular(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();
                double[] values = { 100, 80, 65, 45, 20 };

                var radialGaugePlot = _c.Add.RadialGaugePlot(values);
                radialGaugePlot.CircularBackground = false;
                radialGaugePlot.MaximumAngle = 180;
                radialGaugePlot.StartingAngle = 180;
            }
        }
    }
}