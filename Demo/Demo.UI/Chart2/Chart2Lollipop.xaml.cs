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
    public partial class Chart2Lollipop : Win
    {
        public Chart2Lollipop()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = Generate.Sin(25);
                _c.Add.Lollipop(values);
            }
        }

        void OnPosition(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Range(0, 6.28, 0.314);
                double[] ys = xs.Select(Math.Sin).ToArray();
                var lollipop = _c.Add.Lollipop(ys, xs);
            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = Generate.Sin(21);
                var lollipop = _c.Add.Lollipop(values);

                lollipop.MarkerColor = Colors.Red;
                lollipop.MarkerSize = 15;
                lollipop.MarkerShape = MarkerShape.FilledDiamond;

                lollipop.LineColor = Colors.Green;
                lollipop.LineWidth = 3;
                lollipop.LinePattern = LinePattern.Dotted;
            }
        }

        void OnHor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Sin(21);
                double[] ys = Generate.Consecutive(21);
                Coordinates[] coordinates = Coordinates.Zip(xs, ys);

                var lollipop = _c.Add.Lollipop(coordinates);
                lollipop.Orientation = Orientation.Horizontal;
            }
        }
    }
}