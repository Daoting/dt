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
        Rect _labelRect;

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
            // uno中未实现Bounds
            //if (_geometry.Bounds.IntersectRect(rc.Bounds2D).IsEmpty)
            //{
            //    return false;
            //}
            RectangleGeometry geometry = new RectangleGeometry();
            _labelRect = rc.Bounds2D;
            geometry.Rect = _labelRect;
            base.Clip = geometry;
            return true;
        }

        protected void RenderCandle(RenderContext rc)
        {
            bool inverted = false;
            if (rc.Renderer is BaseRenderer)
            {
                inverted = ((BaseRenderer)rc.Renderer).Inverted;
            }
            double d = rc["HighValues"];
            double num2 = rc["LowValues"];
            double num3 = rc["OpenValues"];
            double num4 = rc["CloseValues"];
            if (double.IsNaN(num3) || double.IsNaN(num4))
                return;

            bool flag2 = num3 > num4;
            Rect rect;
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

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
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
                    if (Math.Abs((double)(d - num4)) < Math.Abs((double)(d - num3)))
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
                    if (Math.Abs((double)(d - num4)) < Math.Abs((double)(d - num3)))
                    {
                        num14 = num4;
                    }
                    figure2.StartPoint = new Point(rc.Current.X, d);
                    segment.Point = new Point(rc.Current.X, num14);
                }
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
                    if (Math.Abs((double)(num2 - num4)) < Math.Abs((double)(num2 - num3)))
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
                    if (Math.Abs((double)(num2 - num4)) < Math.Abs((double)(num2 - num3)))
                    {
                        num16 = num4;
                    }
                    figure3.StartPoint = new Point(rc.Current.X, num2);
                    segment2.Point = new Point(rc.Current.X, num16);
                }
                figure3.Segments.Add(segment2);
                geometry.Figures.Add(figure3);
            }
            Data = geometry;
        }

        void RenderDefault(RenderContext rc)
        {
            Fill = new SolidColorBrush(Colors.Transparent);
            double high = rc["HighValues"];
            double low = rc["LowValues"];
            double open = rc["OpenValues"];
            double close = rc["CloseValues"];
            double width = Size.Width;
            double x = rc.Current.X - (0.5 * width);
            double num7 = x + width;
            bool inverted = false;

            if (rc.Renderer is BaseRenderer)
            {
                inverted = ((BaseRenderer)rc.Renderer).Inverted;
            }

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            PathFigure figure;
            LineSegment segment;
            if (!double.IsNaN(low) && !double.IsNaN(high))
            {
                figure = new PathFigure();
                figure.IsFilled = true;
                segment = new LineSegment();
                if (inverted)
                {
                    low = rc.ConvertX(low);
                    high = rc.ConvertX(high);
                    figure.StartPoint = new Point(low, rc.Current.Y);
                    segment.Point = new Point(high, rc.Current.Y);
                }
                else
                {
                    low = rc.ConvertY(low);
                    high = rc.ConvertY(high);
                    figure.StartPoint = new Point(rc.Current.X, low);
                    segment.Point = new Point(rc.Current.X, high);
                }
                figure.Segments.Add(segment);
                geometry.Figures.Add(figure);
            }

            if (!double.IsNaN(open))
            {
                figure = new PathFigure();
                figure.IsFilled = true;
                segment = new LineSegment();
                if (inverted)
                {
                    open = rc.ConvertX(open);
                    figure.StartPoint = new Point(open, rc.Current.Y + (0.5 * Size.Height));
                    segment.Point = new Point(open, rc.Current.Y);
                }
                else
                {
                    open = rc.ConvertY(open);
                    figure.StartPoint = new Point(x, open);
                    segment.Point = new Point(rc.Current.X, open);
                }
                figure.Segments.Add(segment);
                geometry.Figures.Add(figure);
            }

            if (!double.IsNaN(close))
            {
                figure = new PathFigure();
                figure.IsFilled = true;
                segment = new LineSegment();
                if (inverted)
                {
                    close = rc.ConvertX(close);
                    figure.StartPoint = new Point(close, rc.Current.Y - (0.5 * Size.Height));
                    segment.Point = new Point(close, rc.Current.Y);
                }
                else
                {
                    close = rc.ConvertY(close);
                    figure.StartPoint = new Point(num7, close);
                    segment.Point = new Point(rc.Current.X, close);
                }
                figure.Segments.Add(segment);
                geometry.Figures.Add(figure);
            }
            Data = geometry;
        }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }

#if IOS
    new
#endif
        public HLOCAppearance Appearance
        {
            get { return app; }
            set { app = value; }
        }
    }
}

