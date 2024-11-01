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
    public partial class Chart2Ellipse : Win
    {
        public Chart2Ellipse()
        {
            InitializeComponent();
        }

        void OnEllipse(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Random rand = new(0);
                for (int i = 0; i < 5; i++)
                {
                    _c.Add.Ellipse(
                        xCenter: rand.Next(-10, 10),
                        yCenter: rand.Next(-10, 10),
                        radiusX: rand.Next(1, 7),
                        radiusY: rand.Next(1, 7));
                }
            }
        }

        void OnCircle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Random rand = new(0);
                for (int i = 0; i < 5; i++)
                {
                    _c.Add.Circle(
                        xCenter: rand.Next(-10, 10),
                        yCenter: rand.Next(-10, 10),
                        radius: rand.Next(1, 7));
                }
            }
        }

        void OnEllipseStyle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var circle = _c.Add.Circle(center: Coordinates.Origin, radius: 5);
                circle.LineStyle.Width = 5;
                circle.LineStyle.Pattern = LinePattern.Dashed;
                circle.LineStyle.Color = Colors.Green;
                circle.FillStyle.Color = Colors.Navy;
                circle.FillStyle.HatchColor = Colors.Red;
                circle.FillStyle.Hatch = new Striped();

                _c.Axes.SetLimits(-10, 10, -10, 10);
            }
        }

        void OnRotation(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                int count = 5;
                for (int i = 0; i < count; i++)
                {
                    var el = _c.Add.Ellipse(
                        center: Coordinates.Origin,
                        radiusX: 1,
                        radiusY: 5);

                    el.Rotation = i * 180.0 / count;
                }

                SquareZoomOut rule = new(_c.Axes.Bottom, _c.Axes.Left);
                _c.Axes.Rules.Add(rule);
            }
        }

        void OnLockCircle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Random rand = new(0);
                for (int i = 0; i < 5; i++)
                {
                    _c.Add.Circle(
                        xCenter: rand.Next(-10, 10),
                        yCenter: rand.Next(-10, 10),
                        radius: rand.Next(1, 7));
                }
                _c.Axes.SquareUnits();
            }
        }
    }
}