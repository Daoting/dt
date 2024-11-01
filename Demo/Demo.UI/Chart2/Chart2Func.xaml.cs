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
    public partial class Chart2Func : Win
    {
        public Chart2Func()
        {
            InitializeComponent();
        }

        void OnFunc(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                static double func1(double x) => (Math.Sin(x) * Math.Sin(x / 2));
                static double func2(double x) => (Math.Sin(x) * Math.Sin(x / 3));
                static double func3(double x) => (Math.Cos(x) * Math.Sin(x / 5));

                // Add functions to the plot
                _c.Add.Function(func1);
                _c.Add.Function(func2);
                _c.Add.Function(func3);

                // Manually set axis limits because functions do not have discrete data points
                _c.Axes.SetLimits(-10, 10, -1.5, 1.5);
            }
        }

        void OnXLimit(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var func = new Func<double, double>((x) => Math.Sin(x) * Math.Sin(x / 2));

                var f = _c.Add.Function(func);
                f.MinX = -3;
                f.MaxX = 3;

                _c.Axes.SetLimits(-5, 5, -.2, 1.0);
            }
        }
    }
}