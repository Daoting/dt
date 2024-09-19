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
    public partial class Chart2FillY : Win
    {
        public Chart2FillY()
        {
            InitializeComponent();
        }

        void OnFillY(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                RandomDataGenerator dataGen = new(0);

                int count = 20;
                double[] xs = Generate.Consecutive(count);
                double[] ys1 = dataGen.RandomWalk(count, offset: -5);
                double[] ys2 = dataGen.RandomWalk(count, offset: 5);

                var fill = _c.Add.FillY(xs, ys1, ys2);
                fill.FillColor = Colors.Blue.WithAlpha(100);
                fill.LineColor = Colors.Blue;
                fill.MarkerColor = Colors.Blue;
                fill.LineWidth = 2;
            }
        }

        void OnValues(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                RandomDataGenerator dataGen = new(0);

                int count = 20;
                double[] xs = Generate.Consecutive(count);
                double[] ys1 = dataGen.RandomWalk(count, offset: -5);
                double[] ys2 = dataGen.RandomWalk(count, offset: 5);

                var scatter1 = _c.Add.Scatter(xs, ys1);
                var scatter2 = _c.Add.Scatter(xs, ys2);

                var fill = _c.Add.FillY(scatter1, scatter2);
                fill.FillColor = Colors.Blue.WithAlpha(.1);
                fill.LineWidth = 0;

                _c.MoveToBack(fill);
            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                List<(int, int, int)> data = new();
                Random rand = new(0);
                for (int i = 0; i < 10; i++)
                {
                    int x = i;
                    int y1 = rand.Next(0, 10);
                    int y2 = rand.Next(20, 30);
                    data.Add((x, y1, y2));
                }

                static (double, double, double) MyConverter((int, int, int) s) => (s.Item1, s.Item2, s.Item3);

                var fill = _c.Add.FillY(data, MyConverter);
                fill.FillColor = Colors.Blue.WithAlpha(.2);
                fill.LineColor = Colors.Blue;
            }
        }

        void OnStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                RandomDataGenerator dataGen = new(0);

                int count = 20;
                double[] xs = Generate.Consecutive(count);
                double[] ys1 = dataGen.RandomWalk(count, offset: -5);
                double[] ys2 = dataGen.RandomWalk(count, offset: 5);

                var fill = _c.Add.FillY(xs, ys1, ys2);
                fill.MarkerShape = MarkerShape.FilledDiamond;
                fill.MarkerSize = 15;
                fill.MarkerColor = Colors.Blue;
                fill.LineColor = Colors.Blue;
                fill.LinePattern = LinePattern.Dotted;
                fill.LineWidth = 2;
                fill.FillColor = Colors.Blue.WithAlpha(.2);
                fill.FillHatch = new ScottPlot.Hatches.Striped(ScottPlot.Hatches.StripeDirection.DiagonalUp);
                fill.FillHatchColor = Colors.Blue.WithAlpha(.4);
                fill.LegendText = "Filled Area";

                _c.ShowLegend();
            }
        }
    }
}