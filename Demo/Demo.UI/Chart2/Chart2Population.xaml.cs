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
using ScottPlot.Plottables;
#endregion

namespace Demo.UI
{
    public partial class Chart2Population : Win
    {
        public Chart2Population()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    _c.Add.Population(values, x: i);
                }

                // make the bottom of the plot snap to zero by default
                _c.Axes.Margins(bottom: 0);

                // replace the default numeric ticks with custom ones
                double[] tickPositions = Generate.Consecutive(5);
                string[] tickLabels = Enumerable.Range(1, 5).Select(x => $"Group {x}").ToArray();
                _c.Axes.Bottom.SetTicks(tickPositions, tickLabels);

                // refine appearance of the plot
                _c.Axes.Bottom.MajorTickStyle.Length = 0;
                _c.Axes.Margins(bottom: 0);
                _c.HideGrid();
            }
        }

        void OnBox(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);

                    // disable visibility of the bar symbol
                    pop.Bar.IsVisible = false;

                    // enable visibility of the box symbol
                    pop.Box.IsVisible = true;
                }

                // refine appearance of the plot
                _c.HideGrid();
            }
        }

        void OnBoxVal(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);
                    pop.Bar.IsVisible = false;
                    pop.Box.IsVisible = true;

                    pop.BoxValueConfig = PopulationSymbol.BoxValueConfigurator_MeanStdErrStDev;
                }

                // refine appearance of the plot
                _c.HideGrid();
            }
        }

        void OnBarStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);

                    pop.Bar.FillColor = pop.Marker.MarkerLineColor.WithAlpha(.5);
                    pop.Bar.BorderLineWidth = 2;
                    pop.Bar.ErrorLineWidth = 2;
                    pop.Bar.ErrorNegative = false;
                }

                // refine appearance of the plot
                _c.Axes.Margins(bottom: 0);
                _c.HideGrid();
            }
        }

        void OnBoxStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);
                    pop.Bar.IsVisible = false;
                    pop.Box.IsVisible = true;
                    pop.Box.LineWidth = 2;
                    pop.Box.FillColor = pop.Marker.MarkerLineColor.WithAlpha(.5);
                }

                // refine appearance of the plot
                _c.HideGrid();
            }
        }

        void OnMarkStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);

                    pop.Marker.LineWidth = 2;
                    pop.Marker.Color = Colors.Black.WithAlpha(.5);
                    pop.Marker.Shape = MarkerShape.OpenTriangleUp;
                }

                // refine appearance of the plot
                _c.Axes.Margins(bottom: 0);
                _c.HideGrid();
            }
        }

        void OnDotPos(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                for (int i = 0; i < 5; i++)
                {
                    double[] values = Generate.RandomNormal(10, mean: 3 + i);
                    var pop = _c.Add.Population(values, x: i);

                    pop.MarkerAlignment = ScottPlot.HorizontalAlignment.Center;
                    pop.BarAlignment = ScottPlot.HorizontalAlignment.Center;
                    pop.Marker.Shape = MarkerShape.OpenDiamond;
                    pop.Marker.Color = Colors.Black.WithAlpha(.5);
                    pop.Bar.FillColor = Colors.Gray;
                    pop.Bar.BorderLineWidth = 2;
                    pop.Bar.ErrorLineWidth = 2;
                    pop.Width = 0.5;
                }

                // refine appearance of the plot
                _c.Axes.Margins(bottom: 0);
                _c.HideGrid();
            }
        }

        void OnCustom(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                string[] groupNames = { "Gen X", "Gen Y", "Gen Z" };
                string[] categoryNames = { "Python", "C#", "Rust" };
                Color[] categoryColors = { Colors.C0, Colors.C1, Colors.C2 };

                // add random data to the plot
                for (int groupIndex = 0; groupIndex < groupNames.Length; groupIndex++)
                {
                    for (int categoryIndex = 0; categoryIndex < categoryNames.Length; categoryIndex++)
                    {
                        double[] values = Generate.RandomNormal(10, mean: 2 + groupIndex * 2);
                        double x = groupIndex * (categoryNames.Length + 1) + categoryIndex;
                        var pop = _c.Add.Population(values, x);
                        pop.Marker.MarkerLineColor = categoryColors[categoryIndex].WithAlpha(.75);
                        pop.Marker.Size = 7;
                        pop.Marker.LineWidth = 1.5f;
                        pop.Bar.FillColor = categoryColors[categoryIndex];
                    }
                }

                // apply group names to horizontal tick labels
                double tickDelta = categoryNames.Length + 1;
                double[] tickPositions = Enumerable.Range(0, groupNames.Length)
                    .Select(x => x * tickDelta + tickDelta / 2 - 1)
                    .ToArray();
                _c.Axes.Bottom.SetTicks(tickPositions, groupNames);
                _c.Axes.Bottom.MajorTickStyle.Length = 0;

                // show category colors in the legend
                for (int i = 0; i < categoryNames.Length; i++)
                {
                    LegendItem item = new()
                    {
                        FillColor = categoryColors[i],
                        LabelText = categoryNames[i]
                    };
                    _c.Legend.ManualItems.Add(item);
                }
                _c.Legend.Orientation = Orientation.Horizontal;
                _c.Legend.Alignment = Alignment.UpperLeft;

                // refine appearance of the plot
                _c.Axes.Margins(bottom: 0, top: 0.3);
                _c.YTitle = "Bugs per Hour";
                _c.HideGrid();
            }
        }
    }
}