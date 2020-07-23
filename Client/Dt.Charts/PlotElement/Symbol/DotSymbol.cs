#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class DotSymbol : Dt.Charts.Symbol
    {
        public DotSymbol()
        {
        }

        internal override object Clone()
        {
            DotSymbol clone = new DotSymbol();
            base.CloneAttributes(clone);
            return clone;
        }

        protected override void UpdateGeometry(Size sz)
        {
            double num = 0.5 * Size.Width;
            double num2 = 0.5 * Size.Height;
            double num3 = 0.5 * StrokeThickness;

#if UWP
            var geometry = new EllipseGeometry();
            geometry.RadiusX = num;
            geometry.RadiusY = num2;
            geometry.Center = new Point(num + num3, num2 + num3);
            Data = geometry;
#else
            // uno不支持Path.Data为非PathGeometry！
            // 起点StartPoint终点Point画圆弧，两点不能重叠
            PathGeometry geometry = new PathGeometry();
            PathFigure pf = new PathFigure { StartPoint = new Point(StrokeThickness, num2 - 0.01) };
            pf.Segments.Add(new ArcSegment { Point = new Point(StrokeThickness, num2), Size = new Size(num - num3, num2 - num3), IsLargeArc = true, SweepDirection = SweepDirection.Clockwise });
            geometry.Figures.Add(pf);
            Data = geometry;
#endif

            Canvas.SetLeft(this, _symCenter.X - num - num3);
            Canvas.SetTop(this, _symCenter.Y - num2 - num3);
        }

        protected override Shape LegendShape
        {
            get
            {
                Ellipse shape = new Ellipse();
                base.AdjustLegendShape(shape);
                return shape;
            }
        }
    }
}

