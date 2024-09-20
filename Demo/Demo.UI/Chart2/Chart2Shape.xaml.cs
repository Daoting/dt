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
#endregion

namespace Demo.UI
{
    public partial class Chart2Shape : Win
    {

        public Chart2Shape()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Rectangle(0, 1, 0, 1);

                // add a rectangle using more expressive shapes
                Coordinates location = new(2, 0);
                CoordinateSize size = new(1, 1);
                CoordinateRect rect = new(location, size);
                _c.Add.Rectangle(rect);

                // style rectangles after they are added to the plot
                var rp = _c.Add.Rectangle(4, 5, 0, 1);
                rp.FillStyle.Color = Colors.Magenta.WithAlpha(.2);
                rp.LineStyle.Color = Colors.Green;
                rp.LineStyle.Width = 3;
                rp.LineStyle.Pattern = LinePattern.Dashed;
            }
        }

        void OnCircle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var c1 = _c.Add.Circle(1, 0, .5);
                var c2 = _c.Add.Circle(2, 0, .5);
                var c3 = _c.Add.Circle(3, 0, .5);

                c1.FillStyle.Color = Colors.Blue;
                c2.FillStyle.Color = Colors.Blue.Darken(.75);
                c3.FillStyle.Color = Colors.Blue.Lighten(.75);

                c1.LineWidth = 0;
                c2.LineWidth = 0;
                c3.LineWidth = 0;

                // force circles to remain circles
                ScottPlot.AxisRules.SquareZoomOut squareRule = new(_c.Axes.Bottom, _c.Axes.Left);
                _c.Axes.Rules.Add(squareRule);
            }
        }

        void OnEllipse(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 10; i++)
                {
                    var el = _c.Add.Ellipse(0, 0, 1, 10, rotation: i * 10);
                    double fraction = i / 10.0;
                    el.LineColor = Colors.Blue.WithAlpha(fraction);
                }

                // force circles to remain circles
                ScottPlot.AxisRules.SquareZoomOut squareRule = new(_c.Axes.Bottom, _c.Axes.Left);
                _c.Axes.Rules.Add(squareRule);
            }
        }

        void OnPolygon(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                Coordinates[] points =
                {
                    new (0, 0.25),
                    new (0.3, 0.75),
                    new (1, 1),
                    new (0.7, 0.5),
                    new (1, 0)
                };

                var poly = _c.Add.Polygon(points);
                poly.FillColor = Colors.Green;
                poly.FillHatchColor = Colors.Blue;
                poly.FillHatch = new Gradient()
                {
                    GradientType = GradientType.Linear,
                    AlignmentStart = Alignment.UpperRight,
                    AlignmentEnd = Alignment.LowerLeft,
                };

                poly.LineColor = Colors.Black;
                poly.LinePattern = LinePattern.Dashed;
                poly.LineWidth = 2;

                poly.MarkerShape = MarkerShape.OpenCircle;
                poly.MarkerSize = 8;
                poly.MarkerFillColor = Colors.Gold;
                poly.MarkerLineColor = Colors.Brown;

            }
        }
    }
}