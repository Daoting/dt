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
    public partial class Chart2Heatmap : Win
    {
        public Chart2Heatmap()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();
                _c.Add.Heatmap(data);
            }
        }

        void OnReverse(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                var hm1 = _c.Add.Heatmap(data);
                hm1.Colormap = new ScottPlot.Colormaps.Viridis();
                hm1.Position = new(0, 65, 0, 100);

                var hm2 = _c.Add.Heatmap(data);
                hm2.Colormap = new ScottPlot.Colormaps.Viridis().Reversed();
                hm2.Position = new(100, 165, 0, 100);
            }
        }

        void OnColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                var hm1 = _c.Add.Heatmap(data);
                hm1.Colormap = new ScottPlot.Colormaps.Turbo();

                _c.Add.ColorBar(hm1);
            }
        }

        void OnMultiColor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                var hm1 = _c.Add.Heatmap(data);
                hm1.Extent = new(0, 1, 0, 1);
                hm1.Colormap = new ScottPlot.Colormaps.Turbo();
                _c.Add.ColorBar(hm1);

                var hm2 = _c.Add.Heatmap(data);
                hm2.Extent = new(1.5, 2.5, 0, 1);
                hm2.Colormap = new ScottPlot.Colormaps.Viridis();
                _c.Add.ColorBar(hm2);
            }
        }

        void OnColorTitle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                var hm = _c.Add.Heatmap(data);
                hm.Colormap = new ScottPlot.Colormaps.Turbo();

                var cb = _c.Add.ColorBar(hm);
                cb.Label = "Intensity";
                cb.LabelStyle.FontSize = 24;
            }
        }

        void OnFormatTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                var hm = _c.Add.Heatmap(data);
                var cb = _c.Add.ColorBar(hm);

                // create a static function containing the string formatting logic
                static string CustomFormatter(double position)
                {
                    return $"{Math.Round(position / 2.55)} %";
                }

                // create a custom tick generator using your custom label formatter
                ScottPlot.TickGenerators.NumericAutomatic myTickGenerator = new()
                {
                    LabelFormatter = CustomFormatter
                };

                // tell the colorbar to use the custom tick generator
                cb.Axis.TickGenerator = myTickGenerator;
            }
        }

        void OnFlip(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                _c.Add.Text("default", 0, 1.5);
                var hm1 = _c.Add.Heatmap(data);
                hm1.Position = new CoordinateRect(0, 1, 0, 1);

                _c.Add.Text("flip X", 2, 1.5);
                var hm2 = _c.Add.Heatmap(data);
                hm2.Position = new CoordinateRect(2, 3, 0, 1);
                hm2.FlipHorizontally = true;

                _c.Add.Text("flip Y", 4, 1.5);
                var hm3 = _c.Add.Heatmap(data);
                hm3.Position = new CoordinateRect(4, 5, 0, 1);
                hm3.FlipVertically = true;

                _c.Add.Text("flip X&Y", 6, 1.5);
                var hm4 = _c.Add.Heatmap(data);
                hm4.Position = new CoordinateRect(6, 7, 0, 1);
                hm4.FlipHorizontally = true;
                hm4.FlipVertically = true;

                _c.Axes.SetLimits(-.5, 7.5, -1, 2);
            }
        }

        void OnSmooth(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                _c.Add.Text("Smooth = false", 0, 1.1);
                var hm1 = _c.Add.Heatmap(data);
                hm1.Position = new CoordinateRect(0, 1, 0, 1);

                _c.Add.Text("Smooth = true", 1.1, 1.1);
                var hm2 = _c.Add.Heatmap(data);
                hm2.Position = new CoordinateRect(1.1, 2.1, 0, 1);
                hm2.Smooth = true;
            }
        }

        void OnTransparent(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();
                for (int y = 20; y < 80; y++)
                {
                    for (int x = 20; x < 60; x++)
                    {
                        data[y, x] = double.NaN;
                    }
                }

                // create a line chart
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // plot the heatmap on top of the line chart
                var hm1 = _c.Add.Heatmap(data);
                hm1.Position = new(10, 35, -1.5, .5);

                // the NaN transparency color can be customized
                var hm2 = _c.Add.Heatmap(data);
                hm2.Position = new(40, 55, -.5, .75);
                hm2.NaNCellColor = Colors.Magenta.WithAlpha(.4);
            }
        }

        void OnOpacity(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                // create a line chart
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // plot the heatmap on top of the line chart
                var hm = _c.Add.Heatmap(data);
                hm.Position = new(10, 35, -1.5, .5);
                hm.Opacity = 0.5;
            }
        }

        void OnAlphaMap(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                // an alpha map controls transparency of each cell
                byte[,] alphaMap = new byte[data.GetLength(0), data.GetLength(1)];

                // fill the alpha map with values from 0 (transparent) to 255 (opaque)
                for (int y = 0; y < alphaMap.GetLength(0); y++)
                {
                    for (int x = 0; x < alphaMap.GetLength(1); x++)
                    {
                        double fractionAcross = (double)x / alphaMap.GetLength(1);
                        alphaMap[y, x] = (byte)(fractionAcross * 255);
                    }
                }

                // create a line chart
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // plot the heatmap on top of the line chart
                var hm = _c.Add.Heatmap(data);
                hm.Position = new(10, 35, -1.5, .5);
                hm.AlphaMap = alphaMap;
            }
        }

        void OnFrameless(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data =
                    {
                        { 1, 2, 3 },
                        { 4, 5, 6 },
                        { 7, 8, 9 },
                    };

                // add a heatmap to the plot
                _c.Add.Heatmap(data);

                // hide axes on all edges of the figure
                _c.Layout.Frameless();

                // disable padding around the heatmap data
                _c.Axes.Margins(0, 0);
            }
        }

        void OnAlignment(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data =
                    {
                        { 1, 2, 3 },
                        { 4, 5, 6 },
                        { 7, 8, 9 },
                    };

                var hm = _c.Add.Heatmap(data);
                hm.CellAlignment = Alignment.LowerLeft;
            }
        }

        void OnCell(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data =
                    {
                        { 1, 2, 3 },
                        { 4, 5, 6 },
                        { 7, 8, 9 },
                    };

                var hm = _c.Add.Heatmap(data);
                hm.CellAlignment = Alignment.LowerLeft;
                hm.CellWidth = 100;
                hm.CellHeight = 10;
            }
        }

        void OnManualRange(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[,] data = ScottPlot.SampleData.MonaLisa();

                // add a heatmap and colorbar to the plot
                var hm = _c.Add.Heatmap(data);
                hm.Colormap = new ScottPlot.Colormaps.Turbo();
                _c.Add.ColorBar(hm);

                // force the colormap to span a manual range of values
                hm.ManualRange = new(50, 150);
            }
        }
    }
}