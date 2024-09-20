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
    public partial class Chart2Polar : Win
    {
        public Chart2Polar()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(radius: 100);

                IColormap colormap = new ScottPlot.Colormaps.Turbo();
                foreach (double fraction in ScottPlot.Generate.Range(0, 1, 0.02))
                {
                    // use the polar axis to get X/Y coordinates given a position in polar space
                    double radius = 100 * fraction;
                    double degrees = 360 * fraction;
                    Coordinates pt = polarAxis.GetCoordinates(radius, degrees);

                    // place markers or other plot types using X/Y coordinates like normal
                    var marker = _c.Add.Marker(pt);
                    marker.Color = colormap.GetColor(fraction);
                }
            }
        }

        void OnRotate(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(radius: 100);
                polarAxis.RotationDegrees = -90;

                IColormap colormap = new ScottPlot.Colormaps.Turbo();
                foreach (double fraction in ScottPlot.Generate.Range(0, 1, 0.02))
                {
                    double radius = 100 * fraction;
                    double degrees = 360 * fraction;
                    Coordinates pt = polarAxis.GetCoordinates(radius, degrees);
                    var marker = _c.Add.Marker(pt);
                    marker.Color = colormap.GetColor(fraction);
                }
            }
        }

        void OnPolarAxis(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                PolarCoordinates[] points = [
                    new(10, Angle.FromDegrees(15)),
                    new(20, Angle.FromDegrees(120)),
                    new(30, Angle.FromDegrees(240)),
                ];

                var polarAxis = _c.Add.PolarAxis(30);
                polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
                polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);

                IPalette palette = new ScottPlot.Palettes.Category10();
                Coordinates center = polarAxis.GetCoordinates(0, 0);
                for (int i = 0; i < points.Length; i++)
                {
                    Coordinates tip = polarAxis.GetCoordinates(points[i]);
                    var arrow = _c.Add.Arrow(center, tip);
                    arrow.ArrowLineWidth = 0;
                    arrow.ArrowFillColor = palette.GetColor(i).WithAlpha(.7);
                }
            }
        }

        void OnStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis();

                // style the spokes (straight lines extending from the center to mark rotations)
                var radialPalette = new ScottPlot.Palettes.Category10();
                for (int i = 0; i < polarAxis.Spokes.Count; i++)
                {
                    polarAxis.Spokes[i].LineColor = radialPalette.GetColor(i).WithAlpha(.5);
                    polarAxis.Spokes[i].LineWidth = 4;
                    polarAxis.Spokes[i].LabelStyle.ForeColor = radialPalette.GetColor(i);
                    polarAxis.Spokes[i].LabelStyle.FontSize = 16;
                    polarAxis.Spokes[i].LabelStyle.Bold = true;
                }

                // style the circles (concentric circles marking radius positions)
                var circularColormap = new ScottPlot.Colormaps.Rain();
                for (int i = 0; i < polarAxis.Circles.Count; i++)
                {
                    double fraction = (double)i / (polarAxis.Circles.Count - 1);
                    polarAxis.Circles[i].LineColor = circularColormap.GetColor(fraction).WithAlpha(.5);
                    polarAxis.Circles[i].LineWidth = 2;
                    polarAxis.Circles[i].LinePattern = LinePattern.Dashed;
                }
            }
        }

        void OnSpoke(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis();

                string[] labels = { "alpha", "beta", "gamma", "delta", "epsilon" };
                polarAxis.SetSpokes(labels, 1.1);
            }
        }

        void OnTick(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis();
                polarAxis.RotationDegrees = -90;

                double[] ticksPositions = { 5, 10, 15, 20 };
                string[] tickLabels = { "A", "B", "C", "D" };
                polarAxis.SetCircles(ticksPositions, tickLabels);

                polarAxis.SetSpokes(count: 5, length: 22, degreeLabels: false);
            }
        }

        void OnCircle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis();

                // define spoke angle and length
                polarAxis.Spokes.Clear();
                polarAxis.Spokes.Add(new(Angle.FromDegrees(0), 0.5));
                polarAxis.Spokes.Add(new(Angle.FromDegrees(45), 0.75));
                polarAxis.Spokes.Add(new(Angle.FromDegrees(90), 1.0));

                // define circle radius
                polarAxis.Circles.Clear();
                polarAxis.Circles.Add(new(0.5));
                polarAxis.Circles.Add(new(0.75));
                polarAxis.Circles.Add(new(1.0));

                // style individual spokes and circles
                ScottPlot.Palettes.Category10 pal = new();
                for (int i = 0; i < 3; i++)
                {
                    polarAxis.Circles[i].LineColor = pal.GetColor(i).WithAlpha(.5);
                    polarAxis.Spokes[i].LineColor = pal.GetColor(i).WithAlpha(.5);

                    polarAxis.Circles[i].LineWidth = 2 + i * 2;
                    polarAxis.Spokes[i].LineWidth = 2 + i * 2;
                }
            }
        }

        void OnRata(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis();
                polarAxis.RotationDegrees = -90;

                // add labeled spokes
                string[] labels = { "Alpha", "Beta", "Gamma", "Delta", "Epsilon" };
                polarAxis.SetSpokes(labels, length: 5.5);

                // add defined ticks
                double[] ticks = { 1, 2, 3, 4, 5 };
                polarAxis.SetCircles(ticks);

                // convert radar values to coordinates
                double[] values1 = { 5, 4, 5, 2, 3 };
                double[] values2 = { 2, 3, 2, 4, 2 };
                Coordinates[] cs1 = polarAxis.GetCoordinates(values1);
                Coordinates[] cs2 = polarAxis.GetCoordinates(values2);

                // add polygons for each dataset
                var poly1 = _c.Add.Polygon(cs1);
                poly1.FillColor = Colors.Green.WithAlpha(.5);
                poly1.LineColor = Colors.Black;

                var poly2 = _c.Add.Polygon(cs2);
                poly2.FillColor = Colors.Blue.WithAlpha(.5);
                poly2.LineColor = Colors.Black;
            }
        }
        
        void OnPhasorDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(30);
                polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
                polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);

                // A Phasor may be added with predefined points
                PolarCoordinates[] points1 = [
                    new(10, Angle.FromDegrees(15)),
                    new(20, Angle.FromDegrees(120)),
                    new(30, Angle.FromDegrees(240)),
                ];
                _c.Add.Phasor(points1);

                // Points on a Phasor may be added or modified after it is created
                var phaser2 = _c.Add.Phasor();
                phaser2.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(35)));
                phaser2.Points.Add(new PolarCoordinates(25, Angle.FromDegrees(140)));
                phaser2.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(260)));
            }
        }

        void OnPhasorLineStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var polarAxis = _c.Add.PolarAxis(30);
                polarAxis.Circles.ForEach(x => x.LinePattern = LinePattern.Dotted);
                polarAxis.Spokes.ForEach(x => x.LinePattern = LinePattern.Dotted);

                // create a phasor plot and points in coordinate space
                var phaser = _c.Add.Phasor();
                phaser.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(35)));
                phaser.Points.Add(new PolarCoordinates(25, Angle.FromDegrees(140)));
                phaser.Points.Add(new PolarCoordinates(20, Angle.FromDegrees(260)));

                // add labels for points
                phaser.Labels.Add("Alpha");
                phaser.Labels.Add("Beta");
                phaser.Labels.Add("Gamma");

                // style the labels
                phaser.LabelStyle.FontSize = 24;
                phaser.LabelStyle.ForeColor = Colors.Black;
                phaser.LabelStyle.FontName = Fonts.Monospace;
                phaser.LabelStyle.Bold = true;
            }
        }
    }
}