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
    public partial class Chart2Signal : Win
    {
        public Chart2Signal()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = Generate.RandomWalk(1_000_000);

                _c.Add.Signal(values);

                _c.Title = "Signal Plot with 1 Million Points";
            }
        }

        void OnStyle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin());
                sig1.Color = Colors.Magenta;
                sig1.LineWidth = 10;
                sig1.LegendText = "Sine";

                var sig2 = _c.Add.Signal(Generate.Cos());
                sig2.Color = Colors.Green;
                sig2.LineWidth = 5;
                sig2.LegendText = "Cosine";

                _c.ShowLegend();
            }
        }

        void OnOffset(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = ScottPlot.Generate.Sin(51);

                var sig1 = _c.Add.Signal(values);
                sig1.LegendText = "Default";

                var sig2 = _c.Add.Signal(values);
                sig2.Data.XOffset = 10;
                sig2.Data.YOffset = .25;
                sig2.LegendText = "Offset";

                _c.Legend.IsVisible = true;
            }
        }

        void OnScale(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = ScottPlot.Generate.Sin(51);
                var signal = _c.Add.Signal(values);

                // increase the vertical scaling
                signal.Data.YScale = 500;
            }
        }

        void OnMarker(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Cos());
                sig1.LegendText = "Default";
                sig1.Data.YOffset = 3;

                var sig2 = _c.Add.Signal(Generate.Cos());
                sig2.LegendText = "Large Markers";
                sig2.MaximumMarkerSize = 20;
                sig2.Data.YOffset = 2;

                var sig3 = _c.Add.Signal(Generate.Cos());
                sig3.LegendText = "Hidden Markers";
                sig3.MaximumMarkerSize = 0;
                sig3.Data.YOffset = 1;

                _c.Legend.IsVisible = true;
            }
        }

        void OnRange(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = Generate.RandomWalk(1000);

                var sigAll = _c.Add.Signal(values);
                sigAll.LegendText = "Full";
                sigAll.Data.YOffset = 80;

                var sigLeft = _c.Add.Signal(values);
                sigLeft.LegendText = "Left";
                sigLeft.Data.YOffset = 60;
                sigLeft.Data.MaximumIndex = 700;

                var sigRight = _c.Add.Signal(values);
                sigRight.LegendText = "Right";
                sigRight.Data.YOffset = 40;
                sigRight.Data.MinimumIndex = 300;

                var sigMid = _c.Add.Signal(values);
                sigMid.LegendText = "Mid";
                sigMid.Data.YOffset = 20;
                sigMid.Data.MinimumIndex = 300;
                sigMid.Data.MaximumIndex = 700;

                _c.ShowLegend(Alignment.UpperRight);
                _c.Axes.Margins(top: .5);
            }
        }

        void OnDouble(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                int[] values = Generate.RandomIntegers(1000, -100, 100);

                _c.Add.Signal(values);
            }
        }

        void OnDate(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                DateTime start = new(2024, 1, 1);
                double[] ys = Generate.RandomWalk(200);

                var sig = _c.Add.Signal(ys);
                sig.Data.XOffset = start.ToOADate();
                sig.Data.Period = 1.0; // one day between each point

                _c.Axes.DateTimeTicksBottom();
            }
        }
        
        void OnSignalConst(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] data = Generate.RandomWalk(1_000_000);
                _c.Add.SignalConst(data);
            }
        }
    }
}