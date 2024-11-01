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
    public partial class Chart2Axes : Win
    {
        public Chart2Axes()
        {
            InitializeComponent();
        }

        void OnXYTitle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.XTitle = "横坐标";
                _c.XStyle.ForeColor = Colors.Red;
                _c.XStyle.FontSize = 30;
                _c.XStyle.Italic = true;

                _c.YTitle = "纵坐标Title";
                _c.Title = "标题";
            }
        }

        void OnSubTitle(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                LeftAxisWithSubtitle customAxisY = new()
                {
                    LabelText = "主标题",
                    SubLabelText = "子标题"
                };

                _c.Axes.Remove(_c.Axes.Left);
                _c.Axes.AddLeftAxis(customAxisY);
            }
        }

        void OnAxisLimits(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Axes.SetLimits(-100, 150, -5, 5);

                var limits = _c.Axes.GetLimits();
                Kit.Msg($"({limits.Left}, {limits.Right}), ({limits.Bottom}, {limits.Top})");
            }
        }

        void OnAutoScale(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Axes.SetLimits(-100, 150, -5, 5);
                _c.Axes.AutoScale();
            }
        }

        void OnInvertedAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Axes.SetLimitsY(bottom: 1.5, top: -1.5);
                _c.Axes.SetLimitsX(0, 50);
            }
        }

        void OnSquareUnits(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Circle(0, 0, 10);

                // force pixels to have a 1:1 scale ratio
                _c.Axes.SquareUnits();

                // even if you try to "stretch" the axis, it will adjust the axis limits automatically
                _c.Axes.SetLimits(-10, 10, -20, 20);
            }
        }

        void OnAntiAlias(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] dataX = { 1, 2, 3, 4, 5 };
                double[] dataY = { 1, 4, 9, 16, 25 };
                _c.Add.Scatter(dataX, dataY);
                _c.Axes.AntiAlias(true);
            }
        }

        void OnHideAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // Hide axis label and tick
                _c.Axes.Bottom.TickLabelStyle.IsVisible = false;
                _c.Axes.Bottom.MajorTickStyle.Length = 0;
                _c.Axes.Bottom.MinorTickStyle.Length = 0;

                // Hide axis edge line
                _c.Axes.Bottom.FrameLineStyle.Width = 0;
                _c.Axes.Right.FrameLineStyle.Width = 0;
                _c.Axes.Top.FrameLineStyle.Width = 0;
            }
        }

        void OnHideGrid(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.HideGrid();
            }
        }

        void OnHideGrid2(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Grid.XAxisStyle.IsVisible = true;
                _c.Grid.YAxisStyle.IsVisible = false;
            }
        }

        void OnCustomGrid(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Grid.MajorLineColor = Colors.Green.WithOpacity(.3);
                _c.Grid.MajorLineWidth = 2;

                _c.Grid.MinorLineColor = Colors.Gray.WithOpacity(.1);
                _c.Grid.MinorLineWidth = 1;
            }
        }

        void OnSpecificGrid(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Grid.XAxisStyle.MajorLineStyle.Color = Colors.Magenta.WithAlpha(.1);
                _c.Grid.XAxisStyle.MajorLineStyle.Width = 5;

                _c.Grid.YAxisStyle.MajorLineStyle.Color = Colors.Green.WithAlpha(.3);
                _c.Grid.YAxisStyle.MajorLineStyle.Width = 2;
            }
        }

        void OnAboveGrid(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var a = _c.Add.Signal(Generate.Sin());
                a.LineWidth = 10;

                _c.Grid.MajorLineWidth = 3;
                _c.Grid.MajorLineColor = Colors.Black.WithAlpha(.2);
                _c.Grid.IsBeneathPlottables = false;
            }
        }

        void OnTopGrid(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var a = _c.Add.Signal(Generate.Sin());
                a.Axes.XAxis = _c.Axes.Top;
                _c.Grid.XAxis = _c.Axes.Top;
            }
        }

        void OnFillColors(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Grid.XAxisStyle.FillColor1 = Colors.Gray.WithOpacity(0.1);
                _c.Grid.XAxisStyle.FillColor2 = Colors.Gray.WithOpacity(0.2);
                _c.Grid.YAxisStyle.FillColor1 = Colors.Gray.WithOpacity(0.1);
                _c.Grid.YAxisStyle.FillColor2 = Colors.Gray.WithOpacity(0.2);

                // show minor grid lines too
                _c.Grid.XAxisStyle.MinorLineStyle.Width = 1;
                _c.Grid.YAxisStyle.MinorLineStyle.Width = 1;
            }
        }

        void OnFillDark(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig = _c.Add.Signal(Generate.Sin());

                sig.LineWidth = 3;
                sig.Color = new("#2b9433");
                sig.AlwaysUseLowDensityMode = true;

                // give the plot a dark background with light text
                _c.FigureBackground.Color = new("#1c1c1e");
                _c.Axes.Color(new("#888888"));

                // shade regions between major grid lines
                _c.Grid.XAxisStyle.FillColor1 = new Color("#888888").WithAlpha(10);
                _c.Grid.YAxisStyle.FillColor1 = new Color("#888888").WithAlpha(10);

                // set grid line colors
                _c.Grid.XAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(15);
                _c.Grid.YAxisStyle.MajorLineStyle.Color = Colors.White.WithAlpha(15);
                _c.Grid.XAxisStyle.MinorLineStyle.Color = Colors.White.WithAlpha(5);
                _c.Grid.YAxisStyle.MinorLineStyle.Color = Colors.White.WithAlpha(5);

                // enable minor grid lines by defining a positive width
                _c.Grid.XAxisStyle.MinorLineStyle.Width = 1;
                _c.Grid.YAxisStyle.MinorLineStyle.Width = 1;
            }
        }

        void OnCustomTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] xs = Generate.Consecutive(100, 1, -50);
                _c.Add.Scatter(xs, Generate.Sin(100));
                _c.Add.Scatter(xs, Generate.Cos(100));

                _c.Axes.Bottom.TickGenerator = new NumericAutomatic
                {
                    LabelFormatter = position =>
                    {
                        if (position == 0)
                            return "0";
                        else if (position > 0)
                            return $"+{position}";
                        else
                            return $"({-position})";
                    }
                };
            }
        }

        void OnDateTimeTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                DateTime[] xs = Generate.ConsecutiveHours(100);
                double[] ys = Generate.RandomWalk(100);
                _c.Add.Scatter(xs, ys);
                var axis = _c.Axes.DateTimeTicksBottom();

                ((DateTimeAutomatic)axis.TickGenerator).LabelFormatter = dt =>
                {
                    bool isMidnight = dt is { Hour: 0, Minute: 0, Second: 0 };
                    return isMidnight
                        ? DateOnly.FromDateTime(dt).ToString()
                        : TimeOnly.FromDateTime(dt).ToString();
                };

            }
        }

        void OnNumericFixed(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                _c.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericFixedInterval(11);
            }
        }

        void OnSetTicks(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                double[] tickPositions = { 10, 25, 40 };
                string[] tickLabels = { "Alpha", "Beta", "Gamma" };
                _c.Axes.Bottom.SetTicks(tickPositions, tickLabels);
            }
        }

        void OnTickPositions(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var ticks = new NumericManual();

                // add major ticks with their labels
                ticks.AddMajor(0, "zero");
                ticks.AddMajor(20, "twenty");
                ticks.AddMajor(50, "fifty");

                // add minor ticks
                ticks.AddMinor(22);
                ticks.AddMinor(25);
                ticks.AddMinor(32);
                ticks.AddMinor(35);
                ticks.AddMinor(42);
                ticks.AddMinor(45);

                _c.Axes.Bottom.TickGenerator = ticks;
            }
        }

        void OnRotatedTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Axes.Bottom.TickLabelStyle.Rotation = -45;
                _c.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleRight;
            }
        }

        void OnRotatedLongTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                double[] values = { 5, 10, 7, 13, 25, 60 };
                _c.Add.Bars(values);
                _c.Axes.Margins(bottom: 0);

                // create a tick for each bar
                Tick[] ticks =
                {
                    new(0, "First Long Title"),
                    new(1, "Second Long Title"),
                    new(2, "Third Long Title"),
                    new(3, "Fourth Long Title"),
                    new(4, "Fifth Long Title"),
                    new(5, "Sixth Long Title")
                };
                _c.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks);
                _c.Axes.Bottom.TickLabelStyle.Rotation = 45;
                _c.Axes.Bottom.TickLabelStyle.Alignment = Alignment.MiddleLeft;

                // determine the width of the largest tick label
                float largestLabelWidth = 0;
                using var paint = new SKPaint();
                foreach (Tick tick in ticks)
                {
                    PixelSize size = _c.Axes.Bottom.TickLabelStyle.Measure(tick.Label, paint).Size;
                    largestLabelWidth = Math.Max(largestLabelWidth, size.Width);
                }

                // ensure axis panels do not get smaller than the largest label
                _c.Axes.Bottom.MinimumSize = largestLabelWidth;
                _c.Axes.Right.MinimumSize = largestLabelWidth;
            }
        }

        void OnMiniTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                _c.Axes.Bottom.TickGenerator = new NumericAutomatic { MinimumTickSpacing = 50 };
                _c.Axes.Left.TickGenerator = new NumericAutomatic { MinimumTickSpacing = 25 };
            }
        }

        void OnTickDensity(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                _c.Axes.Bottom.TickGenerator = new NumericAutomatic { TickDensity = 0.2 };
                _c.Axes.Left.TickGenerator = new NumericAutomatic { TickDensity = 0.2 };
            }
        }

        void OnMinorTickDensity(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                // 次要刻度线是在主要刻度线之间的间隔自动生成的。默认情况下，它们的间距是均匀的，但可以自定义它们的密度。
                double[] xs = Generate.Consecutive(100);
                double[] ys = Generate.NoisyExponential(100);
                var sp = _c.Add.Scatter(xs, ys);
                sp.LineWidth = 0;

                _c.Axes.Left.TickGenerator = new NumericAutomatic { MinorTickGenerator = new EvenlySpacedMinorTickGenerator(10) };
            }
        }
        
        void OnTickCount(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                _c.Axes.Bottom.TickGenerator = new NumericAutomatic { TargetTickCount = 3 };
                _c.Axes.Left.TickGenerator = new NumericAutomatic { TargetTickCount = 5 };
            }
        }

        void OnLogScale(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                // 可以通过对要显示的数据进行对数缩放，然后自定义次要刻度和网格来实现对数缩放的外观。
                double[] xs = Generate.Consecutive(100);
                double[] ys = Generate.NoisyExponential(100);

                // 对数据进行对数缩放并考虑负值
                double[] logYs = ys.Select(Math.Log10).ToArray();

                var sp = _c.Add.Scatter(xs, logYs);
                sp.LineWidth = 0;

                _c.Axes.Left.TickGenerator = new NumericAutomatic
                {
                    MinorTickGenerator = new LogMinorTickGenerator(),
                    IntegerTicksOnly = true,
                    LabelFormatter = y => $"{Math.Pow(10, y):N0}"
                };

                _c.Grid.MajorLineColor = Colors.Black.WithOpacity(.15);
                _c.Grid.MinorLineColor = Colors.Black.WithOpacity(.05);
                _c.Grid.MinorLineWidth = 1;
            }
        }

        void OnDateTimeAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                int numberOfHours = 24;
                DateTime[] dateTimes = new DateTime[numberOfHours];
                DateTime startDateTime = new(2024, 1, 1);
                TimeSpan deltaTimeSpan = TimeSpan.FromHours(1);
                for (int i = 0; i < numberOfHours; i++)
                {
                    dateTimes[i] = startDateTime + i * deltaTimeSpan;
                }

                // create an array of doubles representing the same DateTimes one hour apart
                double[] dateDoubles = new double[numberOfHours];
                double startDouble = startDateTime.ToOADate(); // days since 1900
                double deltaDouble = 1.0 / 24.0; // an hour is 1/24 of a day
                for (int i = 0; i < numberOfHours; i++)
                {
                    dateDoubles[i] = startDouble + i * deltaDouble;
                }

                // now both arrays represent the same dates
                _c.Add.Scatter(dateTimes, Generate.Sin(numberOfHours));
                _c.Add.Scatter(dateDoubles, Generate.Cos(numberOfHours));
                _c.Axes.DateTimeTicksBottom();

                // add padding on the right to make room for wide tick labels
                _c.Axes.Right.MinimumSize = 50;
            }
        }

        void OnDateTimeFormat(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                DateTime[] dates = Generate.ConsecutiveDays(100);
                double[] ys = Generate.RandomWalk(100);
                _c.Add.Scatter(dates, ys);
                _c.Axes.DateTimeTicksBottom();

                // add logic into the RenderStarting event to customize tick labels
                _c.RenderManager.RenderStarting += (s, e) =>
                {
                    Tick[] ticks = _c.Axes.Bottom.TickGenerator.Ticks;
                    for (int i = 0; i < ticks.Length; i++)
                    {
                        DateTime dt = DateTime.FromOADate(ticks[i].Position);
                        string label = $"{dt:MMM} '{dt:yy}";
                        ticks[i] = new Tick(ticks[i].Position, label);
                    }
                };
            }
        }

        void OnDateTimeFixedTick(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                // 使刻度从自定义开始日期呈现，而不是使用绘图的开始日期（例如，在每小时的整点或每月的第一天绘制刻度等）
                DateTime[] dates = Generate.ConsecutiveMinutes(24 * 60, new DateTime(2000, 1, 1, 2, 12, 0));
                double[] ys = Generate.RandomWalk(24 * 60);
                _c.Add.Scatter(dates, ys);
                var dtAx = _c.Axes.DateTimeTicksBottom();

                // Create fixed-intervals ticks, major ticks every 6 hours, minor ticks every hour
                dtAx.TickGenerator = new DateTimeFixedInterval(
                    new Hour(), 6,
                    new Hour(), 1,
                    // Here we provide a delegate to override when the ticks start. In this case, we want the majors to be
                    // 00:00, 06:00, 12:00, etc. and the minors to be on the hour, every hour, so we start at midnight.
                    // If you do not provide this delegate, the ticks will start at whatever the Min on the x-axis is.
                    // The major ticks might end up as 1:30am, 7:30am, etc, and the tick positions will be fixed on the plot
                    // when it is panned around.
                    dt => new DateTime(dt.Year, dt.Month, dt.Day));

                // Customise gridlines to make the ticks easier to see
                _c.Grid.XAxisStyle.MajorLineStyle.Color = Colors.Black.WithOpacity();
                _c.Grid.XAxisStyle.MajorLineStyle.Width = 2;

                _c.Grid.XAxisStyle.MinorLineStyle.Color = Colors.Gray.WithOpacity(0.25);
                _c.Grid.XAxisStyle.MinorLineStyle.Width = 1;
                _c.Grid.XAxisStyle.MinorLineStyle.Pattern = LinePattern.DenselyDashed;

                // Remove labels on minor ticks, otherwise there is a lot of tick label overlap
                _c.RenderManager.RenderStarting += (s, e) =>
                {
                    Tick[] ticks = _c.Axes.Bottom.TickGenerator.Ticks;
                    for (int i = 0; i < ticks.Length; i++)
                    {
                        if (ticks[i].IsMajor)
                        {
                            continue;
                        }

                        ticks[i] = new Tick(ticks[i].Position, "", ticks[i].IsMajor);
                    }
                };
            }
        }

        void OnRightAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin(mult: 0.01));
                var sig2 = _c.Add.Signal(Generate.Cos(mult: 100));

                // tell each signal plot to use a different axis
                sig1.Axes.YAxis = _c.Axes.Left;
                sig2.Axes.YAxis = _c.Axes.Right;

                // add additional styling options to each axis
                _c.Axes.Left.Label.Text = "Left Axis";
                _c.Axes.Right.Label.Text = "Right Axis";
                _c.Axes.Left.Label.ForeColor = sig1.Color;
                _c.Axes.Right.Label.ForeColor = sig2.Color;
            }
        }

        void OnMultiAxis(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(ScottPlot.Generate.Sin(51, mult: 0.01));
                sig1.Axes.XAxis = _c.Axes.Bottom; // standard X axis
                sig1.Axes.YAxis = _c.Axes.Left; // standard Y axis
                _c.Axes.Left.Label.Text = "Primary Y Axis";

                // create a second axis and add it to the plot
                var yAxis2 = _c.Axes.AddLeftAxis();

                // add a new plottable and tell it to use the custom Y axis
                var sig2 = _c.Add.Signal(ScottPlot.Generate.Cos(51, mult: 100));
                sig2.Axes.XAxis = _c.Axes.Bottom; // standard X axis
                sig2.Axes.YAxis = yAxis2; // custom Y axis
                yAxis2.LabelText = "Secondary Y Axis";
            }
        }

        void OnLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin(51));
                sig1.LegendText = "Sin";

                var sig2 = _c.Add.Signal(Generate.Cos(51));
                sig2.LegendText = "Cos";

                _c.ShowLegend();
            }
        }

        void OnManLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));
                _c.Legend.IsVisible = true;

                LegendItem item1 = new()
                {
                    LineColor = Colors.Magenta,
                    MarkerFillColor = Colors.Magenta,
                    MarkerLineColor = Colors.Magenta,
                    LineWidth = 2,
                    LabelText = "Alpha"
                };

                LegendItem item2 = new()
                {
                    LineColor = Colors.Green,
                    MarkerFillColor = Colors.Green,
                    MarkerLineColor = Colors.Green,
                    LineWidth = 4,
                    LabelText = "Beta"
                };

                LegendItem[] items = { item1, item2 };
                _c.ShowLegend(items);
            }
        }

        void OnSetLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin(51));
                sig1.LegendText = "Sin";

                var sig2 = _c.Add.Signal(Generate.Cos(51));
                sig2.LegendText = "Cos";

                _c.Legend.IsVisible = true;
                _c.Legend.Alignment = Alignment.UpperCenter;

                _c.Legend.OutlineColor = Colors.Navy;
                _c.Legend.OutlineWidth = 5;
                _c.Legend.BackgroundColor = Colors.LightBlue;

                _c.Legend.ShadowColor = Colors.Blue.WithOpacity(.2);
                _c.Legend.ShadowOffset = new(10, 10);

                _c.Legend.FontSize = 32;
                _c.Legend.FontName = Fonts.Serif;
            }
        }

        void OnLegendOri(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin(51, phase: .2));
                var sig2 = _c.Add.Signal(Generate.Sin(51, phase: .4));
                var sig3 = _c.Add.Signal(Generate.Sin(51, phase: .6));

                sig1.LegendText = "Signal 1";
                sig2.LegendText = "Signal 2";
                sig3.LegendText = "Signal 3";

                _c.ShowLegend(Alignment.UpperLeft, Orientation.Horizontal);
            }
        }

        void OnLegendLine(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                for (int i = 1; i <= 10; i++)
                {
                    double[] data = Generate.Sin(51, phase: .02 * i);
                    var sig = _c.Add.Signal(data);
                    sig.LegendText = $"#{i}";
                }

                _c.Legend.IsVisible = true;
                _c.Legend.Orientation = Orientation.Horizontal;
            }
        }

        void OnMutiLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                for (int i = 1; i <= 5; i++)
                {
                    double[] data = Generate.Sin(51, phase: .02 * i);
                    var sig = _c.Add.Signal(data);
                    sig.LegendText = $"Signal #{i}";
                    sig.LineWidth = 2;
                }

                // default legend
                var leg1 = _c.ShowLegend();
                leg1.Alignment = Alignment.LowerRight;
                leg1.Orientation = Orientation.Vertical;

                // additional legend
                var leg2 = _c.Add.Legend();
                leg2.Alignment = Alignment.UpperCenter;
                leg2.Orientation = Orientation.Horizontal;
            }
        }

        void OnEdageLegend(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                var sig1 = _c.Add.Signal(Generate.Sin());
                var sig2 = _c.Add.Signal(Generate.Cos());

                sig1.LegendText = "Sine";
                sig2.LegendText = "Cosine";

                _c.ShowLegend(Edge.Right);
            }
        }

        void OnLegendFont(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Fonts.AddFontFile("DtIcon", Path.Combine(Package.Current.InstalledLocation.Path, "icon.ttf"));

                var sig1 = _c.Add.Signal(Generate.Sin(51));
                sig1.LegendText = "\uE001";

                var sig2 = _c.Add.Signal(Generate.Cos(51));
                sig2.LegendText = "\uE002";

                _c.Legend.FontName = "DtIcon";
                _c.Legend.FontSize = 36;
                _c.Legend.FontColor = Colors.Red;

                _c.ShowLegend();
            }
        }
        
        void OnLegendFont2(object sender, RoutedEventArgs e)
        {
            this.NaviChart();
            using (_c.Defer())
            {
                Fonts.AddFontFile("DtIcon", Path.Combine(Package.Current.InstalledLocation.Path, "icon.ttf"));

                var sig1 = _c.Add.Signal(Generate.Sin(51));
                sig1.LegendText = "Sin";

                var sig2 = _c.Add.Signal(Generate.Cos(51));
                sig2.LegendText = "Cos";

                _c.Legend.ManualItems.Add(new LegendItem()
                {
                    LabelText = "\uE001",
                    LabelFontName = "DtIcon",
                    LabelFontSize = 18,
                    LabelFontColor = Colors.Magenta,
                    LinePattern = LinePattern.Dotted,
                    LineWidth = 2,
                    LineColor = Colors.Magenta,
                });
            }
        }
    }
}