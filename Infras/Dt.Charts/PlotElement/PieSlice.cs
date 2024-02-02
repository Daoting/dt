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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class PieSlice : PlotElement
    {
        Rect _lrect = new Rect();
        PieInfo _pi = new PieInfo();

        public PieSlice()
        {
            StrokeLineJoin = PenLineJoin.Round;
        }

        protected override bool Render(RenderContext rc)
        {
            PieRenderContext context = rc as PieRenderContext;
            if (context == null
                || double.IsNaN(context.PieInfo.Center.X)
                || double.IsNaN(context.PieInfo.Center.Y)
                || context.PieInfo.RadiusX <= 0.0
                || context.PieInfo.RadiusY <= 0.0)
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

            // 直到 uno4.3 也不支持 PathGeometry.Transform，统一放在 CreatePie
            //TranslateTransform transform = new TranslateTransform();
            //transform.X = _pi.Center.X;
            //transform.Y = _pi.Center.Y;
            //geometry.Transform = transform;
            Data = geometry;
            
            UpdateOffset();
            return true;
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

        static void CreateFullPieWithHole(PathGeometry pg, PieInfo pi)
        {
            // 以中心位置为参考基点，等价 PathGeometry.Transform 的平移
            Point point = new Point(pi.Center.X, pi.Center.Y);

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

        static PathFigure CreatePie(PieInfo pi)
        {
            // 以中心位置为参考基点，等价 PathGeometry.Transform 的平移
            Point point = new Point(pi.Center.X, pi.Center.Y);

            double sweep = pi.Sweep;
            bool flag = sweep == 360.0;
            if (flag)
            {
                sweep = 359.99999;
            }

            double d = pi.Angle * 0.017453292519943295;
            double num6 = (pi.Angle + sweep) * 0.017453292519943295;
            double num7 = Math.Cos(d);
            double num8 = Math.Sin(d);
            double num9 = Math.Cos(num6);
            double num10 = Math.Sin(num6);
            Point point2 = new Point(point.X + (pi.RadiusX * num7), point.Y + (pi.RadiusY * num8));
            Point point3 = new Point(point.X + (pi.RadiusX * num9), point.Y + (pi.RadiusY * num10));

            PathFigure figure = new PathFigure();
            figure.StartPoint = point;
            figure.IsClosed = true;
            figure.IsFilled = true;

            LineSegment segment = new LineSegment();
            segment.Point = point2;

            // 圆弧
            ArcSegment segment2 = new ArcSegment();
            segment2.Point = point3;
            segment2.Size = new Size(pi.RadiusX, pi.RadiusY);
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
            // 以中心位置为参考基点，等价 PathGeometry.Transform 的平移
            Point point = new Point(pi.Center.X, pi.Center.Y);

            double width = pi.RadiusX * pi.InnerRadius;
            double height = pi.RadiusY * pi.InnerRadius;
            double sweep = pi.Sweep;
            if (sweep == 360.0)
            {
                sweep = 359.99999;
            }

            double d = pi.Angle * 0.017453292519943295;
            double num8 = (pi.Angle + sweep) * 0.017453292519943295;
            double num9 = Math.Cos(d);
            double num10 = Math.Sin(d);
            double num11 = Math.Cos(num8);
            double num12 = Math.Sin(num8);
            Point ptStart = new Point(point.X + (width * num9), point.Y + (height * num10));

            PathFigure figure = new PathFigure();
            figure.StartPoint = ptStart;
            figure.IsClosed = true;
            figure.IsFilled = true;

            // 半径
            LineSegment segment = new LineSegment();
            segment.Point = new Point(point.X + (pi.RadiusX * num9), point.Y + (pi.RadiusY * num10));
            figure.Segments.Add(segment);

            // 小环的弧线
            ArcSegment segment2 = new ArcSegment();
            segment2.Point = new Point(point.X + (pi.RadiusX * num11), point.Y + (pi.RadiusY * num12));
            segment2.Size = new Size(pi.RadiusX, pi.RadiusY);
            segment2.IsLargeArc = sweep > 180.0;
            segment2.SweepDirection = SweepDirection.Clockwise;
            figure.Segments.Add(segment2);

            LineSegment segment3 = new LineSegment();
            segment3.Point = new Point(point.X + (width * num11), point.Y + (height * num12));
            figure.Segments.Add(segment3);

            // 大环的弧线
            ArcSegment segment4 = new ArcSegment();
            segment4.Point = ptStart;
            segment4.Size = new Size(width, height);
            segment4.IsLargeArc = sweep > 180.0;
            segment4.SweepDirection = SweepDirection.Counterclockwise;
            figure.Segments.Add(segment4);

            return figure;
        }

        protected override bool IsCompatible(IRenderer rend)
        {
            return (rend is PieRenderer);
        }

        void UpdateOffset()
        {
            Point point = new Point();
            if (_pi.Offset != 0.0)
            {
                double d = 0.017453292519943295 * (_pi.Angle + (_pi.Sweep * 0.5));
                point.X = _pi.Offset * Math.Cos(d);
                point.Y = _pi.Offset * Math.Sin(d);
            }

            double centerX = Canvas.GetLeft(this) + point.X;
            double centerY = Canvas.GetTop(this) + point.Y;
            Canvas.SetLeft(this, centerX);
            Canvas.SetTop(this, centerY);
            if (_effects != null)
            {
                foreach (UIElement element in base._effects)
                {
                    Canvas.SetLeft(element, centerX);
                    Canvas.SetTop(element, centerY);
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
    }
}

