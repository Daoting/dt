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
    public partial class Chart2Line : Win
    {
        public Chart2Line()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Line(1, 12, 12, 0);
                _c.Add.Line(7, 9, 42, 9);
                _c.Add.Line(30, 17, 30, 1);
            }
        }

        void OnLineStyle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                ScottPlot.Colormaps.Viridis colormap = new();

                for (int i = 0; i < 10; i++)
                {
                    // add a line
                    Coordinates start = Generate.RandomCoordinates();
                    Coordinates end = Generate.RandomCoordinates();
                    var line = _c.Add.Line(start, end);

                    // customize the line
                    line.LineColor = Generate.RandomColor(colormap);
                    line.LineWidth = Generate.RandomInteger(1, 4);
                    line.LinePattern = Generate.RandomLinePattern();

                    // customize markers
                    line.MarkerLineColor = line.LineStyle.Color;
                    line.MarkerShape = Generate.RandomMarkerShape();
                    line.MarkerSize = Generate.RandomInteger(5, 15);
                }
            }
        }

        void OnLineLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sin = _c.Add.Signal(Generate.Sin());
                sin.LegendText = "Sine";

                var cos = _c.Add.Signal(Generate.Cos());
                cos.LegendText = "Cosine";

                var line = _c.Add.Line(1, 12, 12, 0);
                line.LineWidth = 3;
                line.MarkerSize = 10;
                line.LegendText = "Line Plot";

                _c.ShowLegend(Alignment.UpperRight);
            }
        }
    }
}