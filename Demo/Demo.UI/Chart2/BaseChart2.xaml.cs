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
    public partial class BaseChart2 : Win
    {

        public BaseChart2()
        {
            InitializeComponent();
        }

        void OnDefAnnotation(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());
                _c.Add.Annotation("默认注释内容");
            }
        }

        void OnCustomAnnotation(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var anno = _c.Add.Annotation("自定义\n注释");
                anno.LabelFontSize = 32;
                anno.LabelBackgroundColor = Colors.RebeccaPurple.WithAlpha(.3);
                anno.LabelFontColor = Colors.RebeccaPurple;
                anno.LabelBorderColor = Colors.Green;
                anno.LabelBorderWidth = 3;
                anno.LabelShadowColor = Colors.Transparent;
                anno.OffsetY = 40;
                anno.OffsetX = 20;
            }
        }

        void OnAnnotationPos(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                foreach (Alignment alignment in Enum.GetValues(typeof(Alignment)))
                {
                    _c.Add.Annotation(alignment.ToString(), alignment);
                }
            }
        }

        void OnArrow(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                Coordinates arrowTip = new(0, 0);
                Coordinates arrowBase = new(1, 1);
                CoordinateLine arrowLine = new(arrowBase, arrowTip);

                // add a simple arrow
                _c.Add.Arrow(arrowLine);

                // arrow line and fill styles can be customized
                var arrow2 = _c.Add.Arrow(arrowLine.WithDelta(1, 0));
                arrow2.ArrowLineColor = Colors.Red;
                arrow2.ArrowMinimumLength = 100;
                arrow2.ArrowLineColor = Colors.Black;
                arrow2.ArrowFillColor = Colors.Transparent;

                // the shape of the arrowhead can be adjusted
                var skinny = _c.Add.Arrow(arrowLine.WithDelta(2, 0));
                skinny.ArrowFillColor = Colors.Green;
                skinny.ArrowLineWidth = 0;
                skinny.ArrowWidth = 3;
                skinny.ArrowheadLength = 20;
                skinny.ArrowheadAxisLength = 20;
                skinny.ArrowheadWidth = 7;

                var fat = _c.Add.Arrow(arrowLine.WithDelta(3, 0));
                fat.ArrowFillColor = Colors.Blue;
                fat.ArrowLineWidth = 0;
                fat.ArrowWidth = 18;
                fat.ArrowheadLength = 20;
                fat.ArrowheadAxisLength = 20;
                fat.ArrowheadWidth = 30;

                // offset backs the arrow away from the tip coordinate
                _c.Add.Marker(arrowLine.WithDelta(4, 0).End);
                var arrow4 = _c.Add.Arrow(arrowLine.WithDelta(4, 0));
                arrow4.ArrowOffset = 15;

                _c.Axes.SetLimits(-1, 6, -1, 2);
            }
        }


        void OnHorAxes(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                _c.Add.VerticalLine(24);
                _c.Add.HorizontalLine(0.73);
            }
        }


        void OnAxesLable(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                // by default labels are drawn on the same side as the axis label

                var axLine1 = _c.Add.VerticalLine(24);
                axLine1.Text = "Line 1";

                var axLine2 = _c.Add.HorizontalLine(0.75);
                axLine2.Text = "Line 2";

                // labels may be drawn on the side opposite of the axis label

                var axLine3 = _c.Add.VerticalLine(37);
                axLine3.Text = "Line 3";
                axLine3.LabelOppositeAxis = true;

                var axLine4 = _c.Add.HorizontalLine(-.75);
                axLine4.Text = "Line 4";
                axLine4.LabelOppositeAxis = true;

                // extra padding on the right and top ensures labels have room
                _c.Axes.Right.MinimumSize = 30;
                _c.Axes.Top.MinimumSize = 30;
            }
        }

        void OnAxesPos(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var axLine1 = _c.Add.VerticalLine(42);
                axLine1.Text = "Line 1";
                axLine1.LabelRotation = -90;
                axLine1.LabelAlignment = Alignment.MiddleRight;

                var axLine2 = _c.Add.HorizontalLine(0.75);
                axLine2.Text = "Line 2";
                axLine2.LabelRotation = 0;
                axLine2.LabelAlignment = Alignment.MiddleRight;

                var axLine3 = _c.Add.VerticalLine(20);
                axLine3.Text = "Line 3";
                axLine3.LabelRotation = -45;
                axLine3.LabelAlignment = Alignment.UpperRight;

                // extra padding on the bottom and left for the rotated labels
                _c.Axes.Bottom.MinimumSize = 60;
                _c.Axes.Left.MinimumSize = 60;
            }
        }

        void OnAxesType(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var vl1 = _c.Add.VerticalLine(24);
                vl1.LineWidth = 3;
                vl1.Color = Colors.Magenta;

                var hl1 = _c.Add.HorizontalLine(0.75);
                hl1.LineWidth = 2;
                hl1.Color = Colors.Green;
                hl1.LinePattern = LinePattern.Dashed;

                var hl2 = _c.Add.HorizontalLine(-.23);
                hl2.LineColor = Colors.Navy;
                hl2.LineWidth = 5;
                hl2.Text = "Hello";
                hl2.LabelFontSize = 24;
                hl2.LabelBackgroundColor = Colors.Blue;
                hl2.LabelFontColor = Colors.Yellow;
                hl2.LinePattern = LinePattern.DenselyDashed;
            }
        }


        void OnAxesLegend(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var axLine1 = _c.Add.VerticalLine(24);
                axLine1.Text = "Line 1";

                var axLine2 = _c.Add.HorizontalLine(0.75);

                var axLine3 = _c.Add.VerticalLine(37);
                axLine3.Text = "Line 3";
                //axLine3.ExcludeFromLegend = true;

                var axLine4 = _c.Add.HorizontalLine(0.25);
                axLine4.Text = "Line 4";

                var axLine5 = _c.Add.HorizontalLine(-.75);
                axLine5.Text = "Line 5";
                //axLine5.ExcludeFromLegend = true;

                _c.ShowLegend();
            }
        }


        void OnAxesIgnore(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin(51));
                _c.Add.Signal(Generate.Cos(51));

                var hline = _c.Add.HorizontalLine(0.23);
                hline.IsDraggable = true;
                hline.EnableAutoscale = false;

                var hSpan = _c.Add.HorizontalSpan(-10, 20);
                hSpan.IsDraggable = true;
                hSpan.EnableAutoscale = false;

            }
        }

        void OnAxisSpan(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var hSpan = _c.Add.HorizontalSpan(10, 20);
                var vSpan = _c.Add.VerticalSpan(0.25, 0.75);

                hSpan.LegendText = "Horizontal Span";
                vSpan.LegendText = "Vertical Span";
                _c.ShowLegend();

            }
        }

        void OnAxisSpanStyle(object sender, RoutedEventArgs e)
        {
            using (_c.Defer())
            {
                _c.Add.Signal(Generate.Sin());
                _c.Add.Signal(Generate.Cos());

                var hs = _c.Add.HorizontalSpan(10, 20);
                hs.LegendText = "Hello";
                hs.LineStyle.Width = 2;
                hs.LineStyle.Color = Colors.Magenta;
                hs.LineStyle.Pattern = LinePattern.Dashed;
                hs.FillStyle.Color = Colors.Magenta.WithAlpha(.2);
            }
        }
    }
}