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
    public partial class Chart2SignalXY : Win
    {
        public Chart2SignalXY()
        {
            InitializeComponent();
        }

        void OnSignalXY(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                List<double> xList = new();
                List<double> yList = new();
                for (int i = 0; i < 5; i++)
                {
                    xList.AddRange(Generate.Consecutive(1000, first: 2000 * i));
                    yList.AddRange(Generate.RandomSample(1000));
                }
                double[] xs = xList.ToArray();
                double[] ys = yList.ToArray();

                // add a SignalXY plot
                _c.Add.SignalXY(xs, ys);
            }
        }

        void OnPart(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(1000);
                double[] ys = Generate.RandomWalk(1000);

                var sigAll = _c.Add.SignalXY(xs, ys);
                sigAll.LegendText = "Full";
                sigAll.Data.YOffset = 80;

                var sigLeft = _c.Add.SignalXY(xs, ys);
                sigLeft.LegendText = "Left";
                sigLeft.Data.YOffset = 60;
                sigLeft.Data.MaximumIndex = 700;

                var sigRight = _c.Add.SignalXY(xs, ys);
                sigRight.LegendText = "Right";
                sigRight.Data.YOffset = 40;
                sigRight.Data.MinimumIndex = 300;

                var sigMid = _c.Add.SignalXY(xs, ys);
                sigMid.LegendText = "Mid";
                sigMid.Data.YOffset = 20;
                sigMid.Data.MinimumIndex = 300;
                sigMid.Data.MaximumIndex = 700;

                _c.ShowLegend(Alignment.UpperRight);
                _c.Axes.Margins(top: .5);
            }
        }

        void OnOffset(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(1000);
                double[] ys = Generate.Sin(1000);

                var sig1 = _c.Add.SignalXY(xs, ys);

                var sig2 = _c.Add.SignalXY(xs, ys);
                sig2.Data.XOffset = 250;
                sig2.Data.YOffset = .5;
            }
        }

        void OnScale(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = ScottPlot.Generate.Sin(51);
                double[] xs = ScottPlot.Generate.Consecutive(51);
                var signalXY = _c.Add.SignalXY(xs, values);

                // increase the vertical scaling
                signalXY.Data.YScale = 500;
            }
        }

        void OnVer(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(1000);
                double[] ys = Generate.RandomWalk(1000);

                var sig1 = _c.Add.SignalXY(xs, ys);
                sig1.Data.Rotated = true;
            }
        }

        void OnReverseX(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(5_000);
                double[] ys = Generate.Sin(count: xs.Length, oscillations: 4);

                // rotate it so it is vertical
                var signal = _c.Add.SignalXY(xs, ys);
                signal.Data.Rotated = true;

                // invert the horizontal axis
                _c.Axes.SetLimitsX(1, -1);
            }
        }

        void OnReverseY(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(5_000);
                double[] ys = Generate.Sin(count: xs.Length, oscillations: 4);

                // rotate it so it is vertical
                var signal = _c.Add.SignalXY(xs, ys);
                signal.Data.Rotated = true;

                // invert the vertical axis
                _c.Axes.SetLimitsY(5000, 0);
            }
        }

        void OnMarker(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var sig = _c.Add.SignalXY(xs, ys);
                sig.MarkerStyle.Shape = MarkerShape.FilledCircle;
                sig.MarkerStyle.Size = 5;
            }
        }
    }
}