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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class PieSlice : PlotElement
    {
        public static readonly DependencyProperty OffsetProperty = DependencyProperty.Register("Offset",  typeof(double),  typeof(PieSlice), new PropertyMetadata((double) 0.0, new PropertyChangedCallback(PieSlice.OnOffsetChanged)));

        Rect _lrect = new Rect();
        internal PieInfo _pi = new PieInfo();
        Point _offset0 = new Point();

        public PieSlice()
        {
            StrokeLineJoin = PenLineJoin.Round;
        }

        protected override LabelAlignment AutoPosition(ref Point hot, ref Point offset)
        {
            hot = _pi.GetRadiusCenter();
            _pi.RadiusX += offset.X;
            _pi.RadiusY += offset.X;
            Point radiusCenter = _pi.GetRadiusCenter();
            _pi.RadiusX -= offset.X;
            _pi.RadiusY -= offset.X;
            offset = new Point(radiusCenter.X - hot.X, radiusCenter.Y - hot.Y);
            if (offset.X == 0.0)
            {
                double num = _pi.Angle + (0.5 * _pi.Sweep);
                if (num == 0.0 || num == 360.0)
                {
                    return LabelAlignment.MiddleRight;
                }
                if ((num > 0.0) && (num < 90.0))
                {
                    return LabelAlignment.BottomRight;
                }
                if (num == 90.0)
                {
                    return LabelAlignment.BottomCenter;
                }
                if ((num > 90.0) && (num < 180.0))
                {
                    return LabelAlignment.BottomLeft;
                }
                if (num == 180.0)
                {
                    return LabelAlignment.MiddleLeft;
                }
                if ((num > 180.0) && (num < 270.0))
                {
                    return LabelAlignment.TopLeft;
                }
                if (num == 270.0)
                {
                    return LabelAlignment.TopCenter;
                }
                if ((num > 270.0) && (num < 360.0))
                {
                    return LabelAlignment.TopRight;
                }
            }
            return LabelAlignment.MiddleCenter;
        }

        internal static void CreateFullPieWithHole(PathGeometry pg, PieInfo pi)
        {
            Point point = new Point();
            double radiusX = pi.RadiusX;
            double radiusY = pi.RadiusY;
            double width = radiusX * pi.InnerRadius;
            double height = radiusY * pi.InnerRadius;
            double angle = pi.Angle;
            double sweep = pi.Sweep;
            double d = angle * 0.017453292519943295;
            double num8 = (angle + 359.99999) * 0.017453292519943295;
            double num9 = Math.Cos(d);
            double num10 = Math.Sin(d);
            double num11 = Math.Cos(num8);
            double num12 = Math.Sin(num8);
            Point point2 = new Point(point.X + (radiusX * num9), point.Y + (radiusY * num10));
            Point point3 = new Point(point.X + (radiusX * num11), point.Y + (radiusY * num12));
            Point point4 = new Point(point.X + (width * num11), point.Y + (height * num12));
            Point point5 = new Point(point.X + (width * num9), point.Y + (height * num10));
            PathFigure figure = new PathFigure();
            figure.StartPoint = point2;
            figure.IsClosed = true;
            figure.IsFilled = true;
            ArcSegment segment = new ArcSegment();
            segment.Point = point3;
            segment.Size = new Size(radiusX, radiusY);
            segment.IsLargeArc = sweep > 180.0;
            segment.SweepDirection = SweepDirection.Clockwise;
            figure.Segments.Add(segment);
            pg.Figures.Add(figure);
            figure = new PathFigure();
            figure.StartPoint = point4;
            figure.IsClosed = true;
            figure.IsFilled = true;
            ArcSegment segment2 = new ArcSegment();
            segment2.Point = point5;
            segment2.Size = new Size(width, height);
            segment2.IsLargeArc = sweep > 180.0;
            segment2.SweepDirection = SweepDirection.Counterclockwise;
            figure.Segments.Add(segment2);
            pg.Figures.Add(figure);
        }

        internal static PathFigure CreatePie(PieInfo pi)
        {
            Point point = new Point();
            double angle = pi.Angle;
            double radiusX = pi.RadiusX;
            double radiusY = pi.RadiusY;
            double sweep = pi.Sweep;
            bool flag = sweep == 360.0;
            if (sweep == 360.0)
            {
                sweep = 359.99999;
            }
            double d = angle * 0.017453292519943295;
            double num6 = (angle + sweep) * 0.017453292519943295;
            double num7 = Math.Cos(d);
            double num8 = Math.Sin(d);
            double num9 = Math.Cos(num6);
            double num10 = Math.Sin(num6);
            Point point2 = new Point(point.X + (radiusX * num7), point.Y + (radiusY * num8));
            Point point3 = new Point(point.X + (radiusX * num9), point.Y + (radiusY * num10));
            PathFigure figure = new PathFigure();
            figure.StartPoint = point;
            figure.IsClosed = true;
            LineSegment segment = new LineSegment();
            segment.Point = point2;
            ArcSegment segment2 = new ArcSegment();
            segment2.Point = point3;
            figure.IsFilled = true;
            segment2.Size = new Size(radiusX, radiusY);
            segment2.IsLargeArc = sweep > 180.0;
            segment2.SweepDirection = SweepDirection.Clockwise;
            LineSegment segment3 = new LineSegment();
            segment3.Point = point;
            if (!flag)
            {
                figure.Segments.Add(segment);
            }
            else
            {
                figure.StartPoint = point2;
            }
            figure.Segments.Add(segment2);
            if (!flag)
            {
                figure.Segments.Add(segment3);
            }
            return figure;
        }

        static PathFigure CreatePieWithHole(PieInfo pi)
        {
            Point point = new Point();
            double radiusX = pi.RadiusX;
            double radiusY = pi.RadiusY;
            double width = radiusX * pi.InnerRadius;
            double height = radiusY * pi.InnerRadius;
            double angle = pi.Angle;
            double sweep = pi.Sweep;
            if (sweep == 360.0)
            {
                sweep = 359.99999;
            }
            double d = angle * 0.017453292519943295;
            double num8 = (angle + sweep) * 0.017453292519943295;
            double num9 = Math.Cos(d);
            double num10 = Math.Sin(d);
            double num11 = Math.Cos(num8);
            double num12 = Math.Sin(num8);
            Point point2 = new Point(point.X + (radiusX * num9), point.Y + (radiusY * num10));
            Point point3 = new Point(point.X + (radiusX * num11), point.Y + (radiusY * num12));
            Point point4 = new Point(point.X + (width * num11), point.Y + (height * num12));
            Point point5 = new Point(point.X + (width * num9), point.Y + (height * num10));
            PathFigure figure = new PathFigure();
            figure.StartPoint = point5;
            figure.IsClosed = true;
            LineSegment segment = new LineSegment();
            segment.Point = point2;
            ArcSegment segment2 = new ArcSegment();
            segment2.Point = point3;
            figure.IsFilled = true;
            segment2.Size = new Size(radiusX, radiusY);
            segment2.IsLargeArc = sweep > 180.0;
            segment2.SweepDirection = SweepDirection.Clockwise;
            LineSegment segment3 = new LineSegment();
            segment3.Point = point4;
            ArcSegment segment4 = new ArcSegment();
            segment4.Point = point5;
            segment4.Size = new Size(width, height);
            segment4.IsLargeArc = sweep > 180.0;
            segment4.SweepDirection = SweepDirection.Counterclockwise;
            figure.Segments.Add(segment);
            figure.Segments.Add(segment2);
            figure.Segments.Add(segment3);
            figure.Segments.Add(segment4);
            return figure;
        }

        protected override bool IsCompatible(IRenderer rend)
        {
            return (rend is PieRenderer);
        }

        protected override bool Render(RenderContext rc)
        {
            PieRenderContext context = rc as PieRenderContext;
            if ((((context == null) || double.IsNaN(context.PieInfo.Center.X)) || (double.IsNaN(context.PieInfo.Center.Y) || (context.PieInfo.RadiusX <= 0.0))) || (context.PieInfo.RadiusY <= 0.0))
            {
                return false;
            }


            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            PieInfo pieInfo = context.PieInfo;
            if ((pieInfo.InnerRadius > 0.0) && (pieInfo.Sweep == 360.0))
            {
                CreateFullPieWithHole(geometry, pieInfo);
            }
            else
            {
                var figure = (context.PieInfo.InnerRadius > 0.0) ? CreatePieWithHole(pieInfo) : CreatePie(pieInfo);
                geometry.Figures.Add(figure);
            }
            Point radiusCenter = pieInfo.GetRadiusCenter();
            _lrect.X = radiusCenter.X;
            _lrect.Y = radiusCenter.Y;
            _pi = pieInfo;
            TranslateTransform transform = new TranslateTransform();
            transform.X = _pi.Center.X;
            transform.Y = _pi.Center.Y;
            geometry.Transform = transform;
            Data = geometry;

            _offset0 = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
            UpdateOffset();
            return true;
        }

        protected void UpdateOffset()
        {
            Point point = new Point();
            if (_pi.Offset != 0.0)
            {
                double d = 0.017453292519943295 * (_pi.Angle + (_pi.Sweep * 0.5));
                point.X = _pi.Offset * Math.Cos(d);
                point.Y = _pi.Offset * Math.Sin(d);
            }
            Canvas.SetLeft(this, _offset0.X + point.X);
            Canvas.SetTop(this, _offset0.Y + point.Y);
            if (_effects != null)
            {
                foreach (UIElement element in base._effects)
                {
                    Canvas.SetLeft(element, point.X + _offset0.X);
                    Canvas.SetTop(element, point.Y + _offset0.Y);
                }
            }
        }

        internal override Rect LabelRect
        {
            get { return _lrect; }
        }

        internal override object Clone()
        {
            PieSlice clone = new PieSlice();
            base.CloneAttributes(clone);
            return clone;
        }

        static void OnOffsetChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            PieSlice slice = (PieSlice) obj;
            slice._pi.Offset = slice.Offset;
            slice.UpdateOffset();
        }

        public double Angle
        {
            get { return  (_pi.Angle + (0.5 * _pi.Sweep)); }
        }

        public double Offset
        {
            get { return  (double) ((double) base.GetValue(OffsetProperty)); }
            set { base.SetValue(OffsetProperty, (double) value); }
        }

        public Point PieCenter
        {
            get { return  _pi.Center; }
        }
    }
}

