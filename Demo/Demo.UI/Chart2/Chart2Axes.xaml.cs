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
            using (_c.Defer(true))
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
            using (_c.Defer(true))
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
            using (_c.Defer(true))
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
            using (_c.Defer(true))
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Axes.SetLimits(-100, 150, -5, 5);
                _c.Axes.AutoScale();
            }
        }

        void OnInvertedAxis(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(true))
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Axes.SetLimitsY(bottom: 1.5, top: -1.5);
                _c.Axes.SetLimitsX(0, 50);
            }
        }

        void OnSquareUnits(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(true))
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
            using (_c.Defer(true))
            {
                double[] dataX = { 1, 2, 3, 4, 5 };
                double[] dataY = { 1, 4, 9, 16, 25 };
                _c.Add.Scatter(dataX, dataY);
                _c.Axes.AntiAlias(true);
            }
        }

        void OnHideAxis(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(true))
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
            using (_c.Defer(true))
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.HideGrid();
            }
        }

        void OnCustomGrid(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(true))
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
            using (_c.Defer(true))
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
            using (_c.Defer(true))
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
            using (_c.Defer(true))
            {
                var a = _c.Add.Signal(Generate.Sin());
                a.Axes.XAxis = _c.Axes.Top;
                _c.Grid.XAxis = _c.Axes.Top;
            }
        }

        void OnFillColors(object sender, RoutedEventArgs e)
        {
            using (_c.Defer(true))
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
            using (_c.Defer(true))
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
    }
}