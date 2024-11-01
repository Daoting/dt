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
    public partial class Chart2Scatter : Win
    {
        public Chart2Scatter()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = { 1, 2, 3, 4, 5 };
                double[] ys = { 1, 4, 9, 16, 25 };

                _c.Add.Scatter(xs, ys);
            }
        }

        void OnCoor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Coordinates[] coordinates =
{
    new(1, 1),
    new(2, 4),
    new(3, 9),
    new(4, 16),
    new(5, 25),
};

                _c.Add.Scatter(coordinates);
            }
        }

        void OnDataType(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                float[] xs = { 1, 2, 3, 4, 5 };
                int[] ys = { 1, 4, 9, 16, 25 };

                _c.Add.Scatter(xs, ys);
            }
        }

        void OnList(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                List<double> xs = new() { 1, 2, 3, 4, 5 };
                List<double> ys = new() { 1, 4, 9, 16, 25 };

                _c.Add.Scatter(xs, ys);
            }
        }

        void OnLine(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] sin = Generate.Sin(51);
                double[] cos = Generate.Cos(51);

                _c.Add.ScatterLine(xs, sin);
                _c.Add.ScatterLine(xs, cos);
            }
        }

        void OnPoints(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] sin = Generate.Sin(51);
                double[] cos = Generate.Cos(51);

                _c.Add.ScatterPoints(xs, sin);
                _c.Add.ScatterPoints(xs, cos);
            }
        }

        void OnStyle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys1 = Generate.Sin(51);
                double[] ys2 = Generate.Cos(51);

                var sp1 = _c.Add.Scatter(xs, ys1);
                sp1.LegendText = "Sine";
                sp1.LineWidth = 3;
                sp1.Color = Colors.Magenta;
                sp1.MarkerSize = 15;

                var sp2 = _c.Add.Scatter(xs, ys2);
                sp2.LegendText = "Cosine";
                sp2.LineWidth = 2;
                sp2.Color = Colors.Green;
                sp2.MarkerSize = 10;

                _c.ShowLegend();
            }
        }

        void OnMode(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                List<LinePattern> patterns = [];
                patterns.AddRange(LinePattern.GetAllPatterns());
                patterns.Add(new([2, 2, 5, 10], 0, "Custom"));
                ScottPlot.Palettes.ColorblindFriendly palette = new();

                for (int i = 0; i < patterns.Count; i++)
                {
                    double yOffset = patterns.Count - i;
                    double[] xs = Generate.Consecutive(51);
                    double[] ys = Generate.Sin(51, offset: yOffset);

                    var sp = _c.Add.Scatter(xs, ys);
                    sp.LineWidth = 2;
                    sp.MarkerSize = 0;
                    sp.LinePattern = patterns[i];
                    sp.Color = palette.GetColor(i);

                    var txt = _c.Add.Text(patterns[i].ToString(), 51, yOffset);
                    txt.LabelFontColor = sp.Color;
                    txt.LabelFontSize = 22;
                    txt.LabelBold = true;
                    txt.LabelAlignment = Alignment.MiddleLeft;
                }

                _c.Axes.Margins(.05, .5, .05, .05);
            }
        }
        
        void OnDate(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                DateTime[] xs = Generate.ConsecutiveDays(100);
                double[] ys = Generate.RandomWalk(xs.Length);

                _c.Add.Scatter(xs, ys);
                _c.Axes.DateTimeTicksBottom();
            }
        }

        void OnStep(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(20);
                double[] ys1 = Generate.Consecutive(20, first: 10);
                double[] ys2 = Generate.Consecutive(20, first: 5);
                double[] ys3 = Generate.Consecutive(20, first: 0);

                var sp1 = _c.Add.Scatter(xs, ys1);
                sp1.ConnectStyle = ConnectStyle.Straight;
                sp1.LegendText = "Straight";

                var sp2 = _c.Add.Scatter(xs, ys2);
                sp2.ConnectStyle = ConnectStyle.StepHorizontal;
                sp2.LegendText = "StepHorizontal";

                var sp3 = _c.Add.Scatter(xs, ys3);
                sp3.ConnectStyle = ConnectStyle.StepVertical;
                sp3.LegendText = "StepVertical";

                _c.ShowLegend();
            }
        }

        void OnGap(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                // long stretch of empty data
                for (int i = 10; i < 20; i++)
                    ys[i] = double.NaN;

                // single missing data point
                ys[30] = double.NaN;

                // single floating data point
                for (int i = 35; i < 40; i++)
                    ys[i] = double.NaN;
                for (int i = 40; i < 45; i++)
                    ys[i] = double.NaN;

                _c.Add.Scatter(xs, ys);
            }
        }

        void OnSmooth(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(10);
                double[] ys = Generate.RandomSample(10, 5, 15);

                var sp = _c.Add.Scatter(xs, ys);
                sp.Smooth = true;
                sp.LegendText = "Smooth";
                sp.LineWidth = 2;
                sp.MarkerSize = 10;
            }
        }

        void OnTension(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.RandomWalk(10);
                double[] ys = Generate.RandomWalk(10);

                var mk = _c.Add.Markers(xs, ys);
                mk.MarkerShape = MarkerShape.OpenCircle;
                mk.Color = Colors.Black;

                double[] tensions = { 0.3, 0.5, 1.0, 3.0 };

                foreach (double tension in tensions)
                {
                    var sp = _c.Add.ScatterLine(xs, ys);
                    sp.Smooth = true;
                    sp.SmoothTension = tension;
                    sp.LegendText = $"Tension {tension}";
                    sp.LineWidth = 2;
                }

                _c.ShowLegend(Alignment.UpperLeft);
            }
        }

        void OnRadio(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(10);
                double[] ys = Generate.RandomSample(10, 5, 15);

                var sp = _c.Add.Scatter(xs, ys);
                sp.PathStrategy = new ScottPlot.PathStrategies.QuadHalfPoint();
                sp.LegendText = "Smooth";
                sp.LineWidth = 2;
                sp.MarkerSize = 10;
            }
        }

        void OnMinMax(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var sp = _c.Add.Scatter(xs, ys);
                sp.MinRenderIndex = 10;
                sp.MaxRenderIndex = 40;
            }
        }

        void OnFill(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var sp = _c.Add.Scatter(xs, ys);
                sp.FillY = true;
                sp.FillYColor = sp.Color.WithAlpha(.2);
            }
        }

        void OnCustomFill(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var sp = _c.Add.Scatter(xs, ys);
                sp.FillY = true;
                sp.FillYColor = sp.Color.WithAlpha(.2);
                sp.FillYValue = 0.6;
            }
        }

        void OnFillTop(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var sp = _c.Add.Scatter(xs, ys);
                sp.FillY = true;
                sp.FillYValue = 0;
                sp.FillYAboveColor = Colors.Green.WithAlpha(.2);
                sp.FillYBelowColor = Colors.Red.WithAlpha(.2);
            }
        }

        void OnFillColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var poly = _c.Add.ScatterLine(xs, ys);

                poly.FillY = true;
                poly.ColorPositions.Add(new(Colors.Red, 0));
                poly.ColorPositions.Add(new(Colors.Orange, 10));
                poly.ColorPositions.Add(new(Colors.Yellow, 20));
                poly.ColorPositions.Add(new(Colors.Green, 30));
                poly.ColorPositions.Add(new(Colors.Blue, 40));
                poly.ColorPositions.Add(new(Colors.Violet, 50));
            }
        }

        void OnScale(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);
                var sp = _c.Add.Scatter(xs, ys);
                sp.ScaleX = 100;
                sp.ScaleY = 10;
                sp.OffsetX = 500;
                sp.OffsetY = 5;
            }
        }

        void OnStack(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = { 1, 2, 3, 4 };
                double[] ys1 = { 1, 3, 1, 2 };
                double[] ys2 = { 3, 7, 3, 1 };
                double[] ys3 = { 5, 2, 5, 6 };

                // shift each plot vertically by the sum of all plots before it
                ys2 = Enumerable.Range(0, ys2.Length).Select(x => ys2[x] + ys1[x]).ToArray();
                ys3 = Enumerable.Range(0, ys2.Length).Select(x => ys3[x] + ys2[x]).ToArray();

                // plot the padded data points as ScatterLine
                var sp3 = _c.Add.ScatterLine(xs, ys3, Colors.Black);
                var sp2 = _c.Add.ScatterLine(xs, ys2, Colors.Black);
                var sp1 = _c.Add.ScatterLine(xs, ys1, Colors.Black);

                // set plot style
                sp1.LineWidth = 2;
                sp2.LineWidth = 2;
                sp3.LineWidth = 2;
                sp1.FillY = true;
                sp2.FillY = true;
                sp3.FillY = true;
                sp1.FillYColor = Colors.Green;
                sp2.FillYColor = Colors.Orange;
                sp3.FillYColor = Colors.Blue;

                // use tight margins so data goes to the edge of the plot
                _c.Axes.Margins(0, 0, 0, 0.1);
            }
        }
    }
}