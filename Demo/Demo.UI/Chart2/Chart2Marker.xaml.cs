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
    public partial class Chart2Marker : Win
    {
        public Chart2Marker()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Add.Marker(25, .5);
                _c.Add.Marker(35, .6);
                _c.Add.Marker(45, .7);
            }
        }

        void OnPosition(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                MarkerShape[] markerShapes = Enum.GetValues<MarkerShape>().ToArray();
                ScottPlot.Palettes.Category20 palette = new();

                for (int i = 0; i < markerShapes.Length; i++)
                {
                    var mp = _c.Add.Marker(x: i, y: 0);
                    mp.MarkerStyle.Shape = markerShapes[i];
                    mp.MarkerStyle.Size = 10;

                    // markers made from filled shapes have can be customized
                    mp.MarkerStyle.FillColor = palette.GetColor(i).WithAlpha(.5);

                    // markers made from filled shapes have optional outlines
                    mp.MarkerStyle.OutlineColor = palette.GetColor(i);
                    mp.MarkerStyle.OutlineWidth = 2;

                    // markers created from lines can be customized
                    mp.MarkerStyle.LineWidth = 2f;
                    mp.MarkerStyle.LineColor = palette.GetColor(i);

                    var txt = _c.Add.Text(markerShapes[i].ToString(), i, 0.15);
                    txt.LabelRotation = -90;
                    txt.LabelAlignment = Alignment.MiddleLeft;
                    txt.LabelFontColor = Colors.Black;
                }

                _c.Title = "Marker Names";
                _c.Axes.SetLimits(-1, markerShapes.Length, -1, 4);
                _c.HideGrid();
            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sin = _c.Add.Signal(Generate.Sin());
                sin.LegendText = "Sine";

                var cos = _c.Add.Signal(Generate.Cos());
                cos.LegendText = "Cosine";

                var marker = _c.Add.Marker(25, .5);
                marker.LegendText = "Marker";
                _c.ShowLegend();
            }
        }

        void OnHor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] sin = Generate.Sin(51);
                double[] cos = Generate.Cos(51);

                _c.Add.Markers(xs, sin, MarkerShape.OpenCircle, 15, Colors.Green);
                _c.Add.Markers(xs, cos, MarkerShape.FilledDiamond, 10, Colors.Magenta);
            }
        }

        void OnColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(51);
                double[] ys = Generate.Sin(51);

                var markers = _c.Add.Markers(xs, ys);
                markers.Colormap = new ScottPlot.Colormaps.Turbo();
            }
        }

        void OnImg(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // An image can be loaded from a file or created dynamically
                ScottPlot.Image image = SampleImages.ScottPlotLogo(48, 48);

                Coordinates location1 = new(5, .5);
                Coordinates location2 = new(25, .5);

                _c.Add.ImageMarker(location1, image);
                _c.Add.ImageMarker(location2, image, scale: 2);

                var m1 = _c.Add.Marker(location1);
                var m2 = _c.Add.Marker(location2);
                m1.Color = Colors.Orange;
                m2.Color = Colors.Orange;
            }
        }
    }
}