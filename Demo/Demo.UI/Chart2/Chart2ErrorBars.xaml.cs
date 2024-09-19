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
    public partial class Chart2ErrorBars : Win
    {
        public Chart2ErrorBars()
        {
            InitializeComponent();
        }

        void OnError(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                int points = 30;

                double[] xs = Generate.Consecutive(points);
                double[] ys = Generate.RandomWalk(points);
                double[] err = Generate.RandomSample(points, 0.1, 1);

                var scatter = _c.Add.Scatter(xs, ys);
                var errorbars = _c.Add.ErrorBar(xs, ys, err);
                errorbars.Color = scatter.Color;
            }
        }

        void OnValues(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                int points = 10;

                double[] xs = Generate.Consecutive(points);
                double[] ys = Generate.RandomWalk(points);
                var scatter = _c.Add.Scatter(xs, ys);
                scatter.LineStyle.Width = 0;

                ScottPlot.Plottables.ErrorBar eb = new(
                    xs: xs,
                    ys: ys,
                    xErrorsNegative: Generate.RandomSample(points, .5),
                    xErrorsPositive: Generate.RandomSample(points, .5),
                    yErrorsNegative: Generate.RandomSample(points),
                    yErrorsPositive: Generate.RandomSample(points));

                eb.Color = scatter.Color;

                _c.Add.Plottable(eb);
            }
        }
    }
}