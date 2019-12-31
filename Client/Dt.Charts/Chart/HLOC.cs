#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class HLOC : PlotElement, ICustomClipping
    {
        protected HLOCAppearance app;

        public HLOC()
        {
        }

        internal override object Clone()
        {
            HLOC clone = new HLOC();
            base.CloneAttributes(clone);
            clone.Appearance = Appearance;
            return clone;
        }

        static PathFigure CreateRectFigure(Rect rect, bool isStroked)
        {
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(rect.Left, rect.Top);
            PolyLineSegment segment = new PolyLineSegment();
            PointCollection points = new PointCollection {
                new Point(rect.Right, rect.Top),
                new Point(rect.Right, rect.Bottom),
                new Point(rect.Left, rect.Bottom),
                new Point(rect.Left, rect.Top)
            };
            segment.Points = points;
            figure.Segments = new PathSegmentCollection();
            figure.Segments.Add(segment);
            figure.IsClosed = true;
            return figure;
        }

        protected override bool Render(RenderContext rc)
        {
            if (Appearance == HLOCAppearance.Candle)
            {
                RenderCandle(rc);
            }
            else
            {
                RenderDefault(rc);
            }
            if (_geometry.Bounds.IntersectRect(rc.Bounds2D).IsEmpty)
            {
                return false;
            }
            RectangleGeometry geometry = new RectangleGeometry();
            geometry.Rect = rc.Bounds2D;
            base.Clip = geometry;
            return true;
        }

        protected void RenderCandle(RenderContext rc)
        {
            bool inverted = false;
            if (rc.Renderer is BaseRenderer)
            {
                inverted = ((BaseRenderer) rc.Renderer).Inverted;
            }
            double d = rc["HighValues"];
            double num2 = rc["LowValues"];
            double num3 = rc["OpenValues"];
            double num4 = rc["CloseValues"];
            PathGeometry geometry = _geometry;
            if (!double.IsNaN(num3) && !double.IsNaN(num4))
            {
                bool flag2 = num3 > num4;
                Rect rect = new Rect();
                if (inverted)
                {
                    num4 = rc.ConvertX(num4);
                    num3 = rc.ConvertX(num3);
                    double height = Size.Height;
                    double y = rc.Current.Y - (0.5 * height);
                    double x = num4;
                    double width = num3 - num4;
                    if (width < 0.0)
                    {
                        x = num3;
                        width = -width;
                    }
                    rect = new Rect(x, y, width, height);
                }
                else
                {
                    double num9 = Size.Width;
                    double num10 = rc.Current.X - (0.5 * num9);
                    num4 = rc.ConvertY(num4);
                    num3 = rc.ConvertY(num3);
                    double num11 = num4;
                    double num12 = num3 - num4;
                    if (num12 < 0.0)
                    {
                        num11 = num3;
                        num12 = -num12;
                    }
                    rect = new Rect(num10, num11, num9, num12);
                }
                PathFigure figure = CreateRectFigure(rect, true);
                if (!flag2)
                {
                    base.Fill = new SolidColorBrush(Colors.Transparent);
                }
                geometry.Figures.Add(figure);
                if (!double.IsNaN(d))
                {
                    PathFigure figure2 = new PathFigure();
                    LineSegment segment = new LineSegment();
                    if (inverted)
                    {
                        d = rc.ConvertX(d);
                        double num13 = num3;
                        if (Math.Abs((double) (d - num4)) < Math.Abs((double) (d - num3)))
                        {
                            num13 = num4;
                        }
                        figure2.StartPoint = new Point(d, rc.Current.Y);
                        segment.Point = new Point(num13, rc.Current.Y);
                    }
                    else
                    {
                        d = rc.ConvertY(d);
                        double num14 = num3;
                        if (Math.Abs((double) (d - num4)) < Math.Abs((double) (d - num3)))
                        {
                            num14 = num4;
                        }
                        figure2.StartPoint = new Point(rc.Current.X, d);
                        segment.Point = new Point(rc.Current.X, num14);
                    }
                    figure2.Segments = new PathSegmentCollection();
                    figure2.Segments.Add(segment);
                    geometry.Figures.Add(figure2);
                }
                if (!double.IsNaN(num2))
                {
                    PathFigure figure3 = new PathFigure();
                    LineSegment segment2 = new LineSegment();
                    if (inverted)
                    {
                        num2 = rc.ConvertX(num2);
                        double num15 = num3;
                        if (Math.Abs((double) (num2 - num4)) < Math.Abs((double) (num2 - num3)))
                        {
                            num15 = num4;
                        }
                        figure3.StartPoint = new Point(num2, rc.Current.Y);
                        segment2.Point = new Point(num15, rc.Current.Y);
                    }
                    else
                    {
                        num2 = rc.ConvertY(num2);
                        double num16 = num3;
                        if (Math.Abs((double) (num2 - num4)) < Math.Abs((double) (num2 - num3)))
                        {
                            num16 = num4;
                        }
                        figure3.StartPoint = new Point(rc.Current.X, num2);
                        segment2.Point = new Point(rc.Current.X, num16);
                    }
                    figure3.Segments = new PathSegmentCollection();
                    figure3.Segments.Add(segment2);
                    geometry.Figures.Add(figure3);
                }
            }
        }

        void RenderDefault(RenderContext rc)
        {
            base.Fill = new SolidColorBrush(Colors.Transparent);
            double d = rc["HighValues"];
            double num2 = rc["LowValues"];
            double num3 = rc["OpenValues"];
            double num4 = rc["CloseValues"];
            PathGeometry geometry = _geometry;
            double width = Size.Width;
            double x = rc.Current.X - (0.5 * width);
            double num7 = x + width;
            bool inverted = false;
            if (rc.Renderer is BaseRenderer)
            {
                inverted = ((BaseRenderer) rc.Renderer).Inverted;
            }
            if (!double.IsNaN(num2) && !double.IsNaN(d))
            {
                PathFigure figure2 = new PathFigure();
                figure2.IsFilled = false;
                PathFigure figure = figure2;
                LineSegment segment = new LineSegment();
                if (inverted)
                {
                    num2 = rc.ConvertX(num2);
                    d = rc.ConvertX(d);
                    figure.StartPoint = new Point(num2, rc.Current.Y);
                    segment.Point = new Point(d, rc.Current.Y);
                }
                else
                {
                    num2 = rc.ConvertY(num2);
                    d = rc.ConvertY(d);
                    figure.StartPoint = new Point(rc.Current.X, num2);
                    segment.Point = new Point(rc.Current.X, d);
                }
                figure.Segments = new PathSegmentCollection();
                figure.Segments.Add(segment);
                geometry.Figures.Add(figure);
            }
            if (!double.IsNaN(num3))
            {
                PathFigure figure4 = new PathFigure();
                figure4.IsFilled = false;
                PathFigure figure3 = figure4;
                LineSegment segment2 = new LineSegment();
                if (inverted)
                {
                    num3 = rc.ConvertX(num3);
                    figure3.StartPoint = new Point(num3, rc.Current.Y + (0.5 * Size.Height));
                    segment2.Point = new Point(num3, rc.Current.Y);
                }
                else
                {
                    num3 = rc.ConvertY(num3);
                    figure3.StartPoint = new Point(x, num3);
                    segment2.Point = new Point(rc.Current.X, num3);
                }
                figure3.Segments = new PathSegmentCollection();
                figure3.Segments.Add(segment2);
                geometry.Figures.Add(figure3);
            }
            if (!double.IsNaN(num4))
            {
                PathFigure figure6 = new PathFigure();
                figure6.IsFilled = false;
                PathFigure figure5 = figure6;
                LineSegment segment3 = new LineSegment();
                if (inverted)
                {
                    num4 = rc.ConvertX(num4);
                    figure5.StartPoint = new Point(num4, rc.Current.Y - (0.5 * Size.Height));
                    segment3.Point = new Point(num4, rc.Current.Y);
                }
                else
                {
                    num4 = rc.ConvertY(num4);
                    figure5.StartPoint = new Point(num7, num4);
                    segment3.Point = new Point(rc.Current.X, num4);
                }
                figure5.Segments = new PathSegmentCollection();
                figure5.Segments.Add(segment3);
                geometry.Figures.Add(figure5);
            }
            foreach(PathFigure figure in geometry.Figures)
            {
                figure.IsFilled = true;
            }
        }

#if IOS
    new
#endif
        public HLOCAppearance Appearance
        {
            get { return  app; }
            set { app = value; }
        }
    }
}

