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
using ScottPlot.AxisPanels.Experimental;
using ScottPlot.TickGenerators;
using ScottPlot.TickGenerators.TimeUnits;
using SkiaSharp;
using System.Data;
using Windows.ApplicationModel;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class Chart2Style : Win
    {
        public Chart2Style()
        {
            InitializeComponent();
        }

        void OnColor(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                // visible items have public properties that can be customized
                _c.Axes.Bottom.Label.Text = "Horizontal Axis";
                _c.Axes.Left.Label.Text = "Vertical Axis";
                _c.Axes.Title.Label.Text = "Plot Title";

                // some items must be styled directly
                _c.Grid.MajorLineColor = Color.FromHex("#0e3d54");
                _c.FigureBackground.Color = Color.FromHex("#07263b");
                _c.DataBackground.Color = Color.FromHex("#0b3049");

                // the Style object contains helper methods to style many items at once
                _c.Axes.Color(Color.FromHex("#a0acb5"));

            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                _c.Axes.Title.Label.Text = "Plot Title";
                _c.Axes.Title.Label.ForeColor = Colors.RebeccaPurple;
                _c.Axes.Title.Label.FontSize = 32;
                _c.Axes.Title.Label.FontName = Fonts.Serif;
                _c.Axes.Title.Label.Rotation = -5;
                _c.Axes.Title.Label.Bold = false;

                _c.Axes.Left.Label.Text = "Vertical Axis";
                _c.Axes.Left.Label.ForeColor = Colors.Magenta;
                _c.Axes.Left.Label.Italic = true;

                _c.Axes.Bottom.Label.Text = "Horizontal Axis";
                _c.Axes.Bottom.Label.Bold = false;
                _c.Axes.Bottom.Label.FontName = Fonts.Monospace;

                _c.Axes.Bottom.MajorTickStyle.Length = 10;
                _c.Axes.Bottom.MajorTickStyle.Width = 3;
                _c.Axes.Bottom.MajorTickStyle.Color = Colors.Magenta;
                _c.Axes.Bottom.MinorTickStyle.Length = 5;
                _c.Axes.Bottom.MinorTickStyle.Width = 0.5f;
                _c.Axes.Bottom.MinorTickStyle.Color = Colors.Green;
                _c.Axes.Bottom.FrameLineStyle.Color = Colors.Blue;
                _c.Axes.Bottom.FrameLineStyle.Width = 3;

                _c.Axes.Right.FrameLineStyle.Width = 0;
            }
        }

        void OnPalette(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Nord();

                for (int i = 0; i < 5; i++)
                {
                    double[] data = Generate.Sin(100, phase: -i / 20.0f);
                    var sig = _c.Add.Signal(data);
                    sig.LineWidth = 3;
                }
            }
        }

        void OnArrow(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                ArrowShape[] arrowShapes = Enum.GetValues<ArrowShape>().ToArray();

                for (int i = 0; i < arrowShapes.Length; i++)
                {
                    Coordinates arrowTip = new(0, -i);
                    Coordinates arrowBase = arrowTip.WithDelta(1, 0);

                    var arrow = _c.Add.Arrow(arrowBase, arrowTip);
                    arrow.ArrowShape = arrowShapes[i].GetShape();

                    var txt = _c.Add.Text(arrowShapes[i].ToString(), arrowBase.WithDelta(.1, 0));
                    txt.LabelFontColor = arrow.ArrowLineColor;
                    txt.LabelAlignment = Alignment.MiddleLeft;
                    txt.LabelFontSize = 18;
                }

                _c.Axes.SetLimits(-1, 3, -arrowShapes.Length, 1);
                _c.HideGrid();
            }
        }

        void OnLineStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                LinePattern[] linePatterns = Enum.GetValues<LinePattern>().ToArray();

                for (int i = 0; i < linePatterns.Length; i++)
                {
                    LinePattern pattern = linePatterns[i];

                    var line = _c.Add.Line(0, -i, 1, -i);
                    line.LinePattern = pattern;
                    line.LineWidth = 2;
                    line.Color = Colors.Black;

                    var txt = _c.Add.Text(pattern.ToString(), 1.1, -i);
                    txt.LabelFontSize = 18;
                    txt.LabelBold = true;
                    txt.LabelFontColor = Colors.Black;
                    txt.LabelAlignment = Alignment.MiddleLeft;
                }

                _c.Axes.Margins(right: 1);
                _c.HideGrid();
                _c.Layout.Frameless();

                _c.ShowLegend();
            }
        }

        void OnScaleFactor(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.ScaleFactor = 2;
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
            }
        }

        void OnHairline(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                // 发际线模式允许轴帧、刻度线和网格线始终呈现单个像素宽，而不管比例因子如何。启用细线模式，以便在使用大比例因子时使交互式绘图感觉更平滑
                _c.ScaleFactor = 2;
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Axes.Hairline(true);
            }
        }

        void OnDark(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Palette = new ScottPlot.Palettes.Penumbra();

                // add things to the plot
                for (int i = 0; i < 5; i++)
                {
                    var sig = _c.Add.Signal(Generate.Sin(51, phase: -.05 * i));
                    sig.LineWidth = 3;
                    sig.LegendText = $"Line {i + 1}";
                }
                _c.XTitle = "Horizontal Axis";
                _c.YTitle = "Vertical Axis";
                _c.Title = "ScottPlot 5 in Dark Mode";
                _c.ShowLegend();

                // change figure colors
                _c.FigureBackground.Color = Color.FromHex("#181818");
                _c.DataBackground.Color = Color.FromHex("#1f1f1f");

                // change axis and grid colors
                _c.Axes.Color(Color.FromHex("#d7d7d7"));
                _c.Grid.MajorLineColor = Color.FromHex("#404040");

                // change legend colors
                _c.Legend.BackgroundColor = Color.FromHex("#404040");
                _c.Legend.FontColor = Color.FromHex("#d7d7d7");
                _c.Legend.OutlineColor = Color.FromHex("#d7d7d7");
            }
        }

        void OnColormap(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                IColormap colormap = new ScottPlot.Colormaps.Turbo();

                for (int count = 1; count < 10; count++)
                {
                    double[] xs = Generate.Consecutive(count);
                    double[] ys = Generate.Repeating(count, count);
                    Color[] colors = colormap.GetColors(count);

                    for (int i = 0; i < count; i++)
                    {
                        var circle = _c.Add.Circle(xs[i], ys[i], 0.45);
                        circle.FillColor = colors[i];
                        circle.LineWidth = 0;
                    }
                }

                _c.YTitle = "number of colors";
            }
        }

        void OnDataBackground(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin());
                var sig2 = _c.Add.Signal(Generate.Cos());
                sig1.LineWidth = 3;
                sig2.LineWidth = 3;

                // One could load an image from a file...
                // Image bgImage = new("background.png");

                // But in this example we will use a sample image:
                Image bgImage = SampleImages.ScottPlotLogo();
                _c.DataBackground.Image = bgImage;
            }
        }

        void OnBackground(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin());
                var sig2 = _c.Add.Signal(Generate.Cos());

                // One could load an image from a file...
                // Image bgImage = new("background.png");

                // But in this example we will use a sample image:
                Image bgImage = SampleImages.MonaLisa();
                _c.FigureBackground.Image = bgImage;

                // Color the axes and data so they stand out against the dark background
                _c.Axes.Color(Colors.White);
                sig1.Color = sig1.Color.Lighten(.2);
                sig2.Color = sig2.Color.Lighten(.2);
                sig1.LineWidth = 3;
                sig2.LineWidth = 3;

                // Shade the data area to make it stand out
                _c.DataBackground.Color = Colors.Black.WithAlpha(.5);
            }
        }

        void OnRectImg(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                Image img = ScottPlot.SampleImages.MonaLisa();
                CoordinateRect rect = new(left: 0, right: img.Width, bottom: 0, top: img.Height);
                _c.Add.ImageRect(img, rect);
            }
        }

        void OnRgb(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i <= 10; i++)
                {
                    double fraction = (double)i / 10;
                    double x = i;
                    double y = Math.Sin(Math.PI * 2 * fraction);
                    var circle = _c.Add.Circle(x, y, 2);
                    circle.FillColor = Colors.Blue.MixedWith(Colors.Green, fraction);
                    circle.LineColor = Colors.Black.WithAlpha(.5);
                }
            }
        }

        void OnNoAxes(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                // make the data area cover the full figure
                _c.Layout.Frameless();

                // set the data area background so we can observe its size
                _c.DataBackground.Color = Colors.WhiteSmoke;
            }
        }

        void OnPadding(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                // use a fixed amount of of pixel padding on each side
                PixelPadding padding = new(100, 50, 100, 50);
                _c.Layout.Fixed(padding);

                // darken the figure background so we can observe its dimensions
                _c.FigureBackground.Color = Colors.LightBlue;
                _c.DataBackground.Color = Colors.White;
            }
        }

        void OnRect(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                // set the data area to render inside a fixed rectangle
                PixelSize size = new(300, 200);
                Pixel offset = new(50, 50);
                PixelRect rect = new(size, offset);
                _c.Layout.Fixed(rect);

                // darken the figure background so we can observe its dimensions
                _c.FigureBackground.Color = Colors.LightBlue;
                _c.DataBackground.Color = Colors.White;
            }
        }
    }
}