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
using ScottPlot.Palettes;
using ScottPlot.TickGenerators;
#endregion

namespace Demo.UI
{
    public partial class Chart2Bar : Win
    {

        public Chart2Bar()
        {
            InitializeComponent();
        }

        void OnDef(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 5, 10, 7, 13 };
                _c.Add.Bars(values);
                _c.Axes.Margins(bottom: 0);
            }
        }

        void OnLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs1 = { 1, 2, 3, 4 };
                double[] ys1 = { 5, 10, 7, 13 };
                var bars1 = _c.Add.Bars(xs1, ys1);
                bars1.LegendText = "Alpha";

                double[] xs2 = { 6, 7, 8, 9 };
                double[] ys2 = { 7, 12, 9, 15 };
                var bars2 = _c.Add.Bars(xs2, ys2);
                bars2.LegendText = "Beta";

                _c.ShowLegend(Alignment.UpperLeft);
                _c.Axes.Margins(bottom: 0);
            }
        }

        void OnLabels(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 5, 10, 7, 13 };
                var barPlot = _c.Add.Bars(values);

                foreach (var bar in barPlot.Bars)
                {
                    bar.Label = bar.Value.ToString();
                }

                barPlot.ValueLabelStyle.Bold = true;
                barPlot.ValueLabelStyle.FontSize = 18;

                _c.Axes.Margins(bottom: 0, top: .2);
            }
        }

        void OnHorLabels(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { -20, 10, 7, 13 };

                var barPlot = _c.Add.Bars(values);
                foreach (var bar in barPlot.Bars)
                {
                    bar.Label = "Label " + bar.Value.ToString();
                }

                barPlot.ValueLabelStyle.Bold = true;
                barPlot.ValueLabelStyle.FontSize = 18;
                barPlot.Horizontal = true;

                _c.Axes.SetLimitsX(-45, 35);
                _c.Add.VerticalLine(0, 1, Colors.Black);
            }
        }

        void OnPosition(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Bar[] bars =
                {
                    new() { Position = 1, Value = 5, ValueBase = 3, FillColor = Colors.Red },
                    new() { Position = 2, Value = 7, ValueBase = 0, FillColor = Colors.Blue },
                    new() { Position = 4, Value = 3, ValueBase = 2, FillColor = Colors.Green },
                };

                _c.Add.Bars(bars);
            }
        }

        void OnError(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Bar[] bars =
                {
                    new() { Position = 1, Value = 5, Error = 1, FillColor = Colors.Red },
                    new() { Position = 2, Value = 7, Error = 2, FillColor = Colors.Orange },
                    new() { Position = 3, Value = 6, Error = 1, FillColor = Colors.Green },
                    new() { Position = 4, Value = 8, Error = 2, FillColor = Colors.Blue },
                };

                _c.Add.Bars(bars);
                _c.Axes.Margins(bottom: 0);
            }
        }

        void OnLabeled(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Bar(position: 1, value: 5, error: 1);
                _c.Add.Bar(position: 2, value: 7, error: 2);
                _c.Add.Bar(position: 3, value: 6, error: 1);
                _c.Add.Bar(position: 4, value: 8, error: 2);

                Tick[] ticks =
                {
                    new(1, "Apple"),
                    new(2, "Orange"),
                    new(3, "Pear"),
                    new(4, "Banana"),
                };

                _c.Axes.Bottom.TickGenerator = new NumericManual(ticks);
                _c.Axes.Bottom.MajorTickStyle.Length = 0;
                _c.HideGrid();

                _c.Axes.Margins(bottom: 0);
            }
        }

        void OnStacked(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Category10 palette = new();
                Bar[] bars =
                {
                    new() { Position = 1, ValueBase = 0, Value = 2, FillColor = palette.GetColor(0) },
                    new() { Position = 1, ValueBase = 2, Value = 5, FillColor = palette.GetColor(1) },
                    new() { Position = 1, ValueBase = 5, Value = 10, FillColor = palette.GetColor(2) },

                    new() { Position = 2, ValueBase = 0, Value = 4, FillColor = palette.GetColor(0) },
                    new() { Position = 2, ValueBase = 4, Value = 7, FillColor = palette.GetColor(1) },
                    new() { Position = 2, ValueBase = 7, Value = 10, FillColor = palette.GetColor(2) },
                };

                _c.Add.Bars(bars);

                Tick[] ticks =
                {
                    new(1, "Spring"),
                    new(2, "Summer"),
                };

                _c.Axes.Bottom.TickGenerator = new NumericManual(ticks);
                _c.Axes.Bottom.MajorTickStyle.Length = 0;
                _c.HideGrid();

                _c.Axes.Margins(bottom: 0);
            }
        }

        void OnGrouped(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Category10 palette = new();

                Bar[] bars =
                {
                    new() { Position = 1, Value = 2, FillColor = palette.GetColor(0), Error = 1 },
                    new() { Position = 2, Value = 5, FillColor = palette.GetColor(1), Error = 2 },
                    new() { Position = 3, Value = 7, FillColor = palette.GetColor(2), Error = 1 },

                    new() { Position = 5, Value = 4, FillColor = palette.GetColor(0), Error = 2 },
                    new() { Position = 6, Value = 7, FillColor = palette.GetColor(1), Error = 1 },
                    new() { Position = 7, Value = 13, FillColor = palette.GetColor(2), Error = 3 },

                    new() { Position = 9, Value = 5, FillColor = palette.GetColor(0), Error = 1 },
                    new() { Position = 10, Value = 6, FillColor = palette.GetColor(1), Error = 3 },
                    new() { Position = 11, Value = 11, FillColor = palette.GetColor(2), Error = 2 },
                };

                _c.Add.Bars(bars);

                _c.Legend.IsVisible = true;
                _c.Legend.Alignment = Alignment.UpperLeft;
                _c.Legend.ManualItems.Add(new() { LabelText = "Monday", FillColor = palette.GetColor(0) });
                _c.Legend.ManualItems.Add(new() { LabelText = "Tuesday", FillColor = palette.GetColor(1) });
                _c.Legend.ManualItems.Add(new() { LabelText = "Wednesday", FillColor = palette.GetColor(2) });

                Tick[] ticks =
                {
                    new(2, "Group 1"),
                    new(6, "Group 2"),
                    new(10, "Group 3"),
                };
                _c.Axes.Bottom.TickGenerator = new NumericManual(ticks);
                _c.Axes.Bottom.MajorTickStyle.Length = 0;
                _c.HideGrid();

                _c.Axes.Margins(bottom: 0);
            }
        }
        
        void OnHor(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Bar[] bars =
                {
                    new() { Position = 1, Value = 5, Error = 1, },
                    new() { Position = 2, Value = 7, Error = 2, },
                    new() { Position = 3, Value = 6, Error = 1, },
                    new() { Position = 4, Value = 8, Error = 2, },
                };

                var barPlot = _c.Add.Bars(bars);
                barPlot.Horizontal = true;

                _c.Axes.Margins(left: 0);
            }
        }

        void OnHorGrouped(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                string[] categoryNames = { "Phones", "Computers", "Tablets" };
                Color[] categoryColors = { Colors.C0, Colors.C1, Colors.C2 };

                for (int x = 0; x < 4; x++)
                {
                    double[] values = Generate.RandomSample(categoryNames.Length, 1000, 5000);

                    double nextBarBase = 0;

                    for (int i = 0; i < values.Length; i++)
                    {
                        Bar bar = new()
                        {
                            Value = nextBarBase + values[i],
                            FillColor = categoryColors[i],
                            ValueBase = nextBarBase,
                            Position = x,
                        };

                        _c.Add.Bar(bar);

                        nextBarBase += values[i];
                    }
                }

                NumericManual tickGen = new();
                for (int x = 0; x < 4; x++)
                {
                    tickGen.AddMajor(x, $"Q{x + 1}");
                }
                _c.Axes.Bottom.TickGenerator = tickGen;

                for (int i = 0; i < 3; i++)
                {
                    LegendItem item = new()
                    {
                        LabelText = categoryNames[i],
                        FillColor = categoryColors[i]
                    };
                    _c.Legend.ManualItems.Add(item);
                }
                _c.Legend.Orientation = Orientation.Horizontal;
                _c.ShowLegend(Alignment.UpperRight);

                _c.Axes.Margins(bottom: 0, top: .3);
            }
        }
    }
}