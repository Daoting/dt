#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class Area : Lines
    {
        public Area()
        {
            IsClosed = true;
        }

        internal override object Clone()
        {
            Area clone = new Area();
            base.CloneAttributes(clone);
            clone.Smoothed = base.Smoothed;
            return clone;
        }

        static Point[] InitPrevious(RenderContext rc, bool inverted, Point[] pts, double[] prev)
        {
            int length = pts.Length;
            Point[] pointArray = new Point[length];
            if (inverted)
            {
                for (int j = 0; j < length; j++)
                {
                    pointArray[j] = new Point(rc.ConvertX(prev[j]), pts[j].Y);
                }
                return pointArray;
            }
            for (int i = 0; i < length; i++)
            {
                pointArray[i] = new Point(pts[i].X, rc.ConvertY(prev[i]));
            }
            return pointArray;
        }

        protected override bool Render(RenderContext rc)
        {
            Point[] points = rc.Points;
            if ((points == null) || (points.Length < 2))
            {
                return false;
            }
            BaseRenderer renderer = rc.Renderer as BaseRenderer;
            if (renderer == null)
            {
                return false;
            }
            bool inverted = renderer.Inverted;
            double naN = double.NaN;
            if ((rc.OptimizationRadiusScope & OptimizationRadiusScope.Lines) > ((OptimizationRadiusScope)0))
            {
                naN = rc.OptimizationRadius;
            }
            double[] previousValues = rc.PreviousValues;
            Point[] pts = null;

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            if (renderer.IsStacked && previousValues != null)
            {
                int length = points.Length;
                if ((previousValues != null) && (previousValues.Length == length))
                {
                    if (base.Smoothed && (length > 3))
                    {
                        Point[] pointArray3 = InitPrevious(rc, inverted, points, previousValues);
                        points = new SplineNew(points).Calculate();
                        pointArray3 = new SplineNew(pointArray3).Calculate();
                        length = points.Length;
                        pts = new Point[2 * length];
                        for (int i = 0; i < length; i++)
                        {
                            pts[i] = points[i];
                            pts[length + i] = pointArray3[(length - i) - 1];
                        }
                    }
                    else
                    {
                        pts = new Point[2 * length];
                        for (int j = 0; j < length; j++)
                        {
                            pts[j] = points[j];
                            if (inverted)
                            {
                                pts[length + j] = new Point(rc.ConvertX(previousValues[(length - j) - 1]), points[(length - j) - 1].Y);
                            }
                            else
                            {
                                pts[length + j] = new Point(points[(length - j) - 1].X, rc.ConvertY(previousValues[(length - j) - 1]));
                            }
                        }
                    }
                }
                if (pts != null)
                {
                    PathFigure figure = RenderSegment(pts);
                    geometry.Figures.Add(figure);
                }
            }
            else
            {
                double num5 = inverted ? rc.ConvertX(0.0) : rc.ConvertY(0.0);
                if (double.IsNaN(num5))
                {
                    num5 = inverted ? (rc.XReversed ? (rc.Bounds.X + rc.Bounds.Width) : rc.Bounds.X) : (rc.YReversed ? rc.Bounds.Y : (rc.Bounds.Y + rc.Bounds.Height));
                }
                List<Point[]> list = base.SplitPointsWithHoles(points);
                Rect cr = rc.IsCustomClipping ? new Rect(rc.Bounds2D.X - 2.0, rc.Bounds2D.Y - 2.0, rc.Bounds2D.Width + 4.0, rc.Bounds2D.Height + 4.0) : Extensions.EmptyRect;
                if (list != null)
                {
                    for (int k = 0; k < list.Count; k++)
                    {
                        Point[] pointArray4 = list[k];
                        PathFigure figure;
                        if (renderer is RadarRenderer)
                        {
                            figure = RenderSegment(pointArray4);
                        }
                        else
                        {
                            figure = RenderNonStacked(pointArray4, num5, inverted, naN, cr);
                        }
                        if (figure != null)
                        {
                            geometry.Figures.Add(figure);
                        }
                    }
                }
            }
            Data = geometry;

            RectangleGeometry geometry2 = new RectangleGeometry();
            geometry2.Rect = new Rect(rc.Bounds.X, rc.Bounds.Y, rc.Bounds.Width, rc.Bounds.Height);
            base.Clip = geometry2;
            return true;
        }

        protected PathFigure RenderNonStacked(Point[] pts, double origin, bool inverted, double optRadius, Rect cr)
        {
            PointCollection points = null;
            int length = pts.Length;
            if (base.Smoothed && (length > 3))
            {
                points = new SplineNew(pts).CalculateCollection();
                length = pts.Length;
            }
            else if (!double.IsNaN(optRadius))
            {
                points = base.DecimateAsCollection(pts, optRadius);
                length = pts.Length;
            }
            else
            {
                points = new PointCollection();
                for (int i = 0; i < length; i++)
                {
                    points.Add(pts[i]);
                }
            }
            if (inverted)
            {
                points.Add(new Point(origin, pts[length - 1].Y));
                points.Add(new Point(origin, pts[0].Y));
            }
            else
            {
                points.Add(new Point(pts[length - 1].X, origin));
                points.Add(new Point(pts[0].X, origin));
            }
            points.Add(points[0]);
            if (!cr.IsEmptyRect())
            {
                Point[] inArray = new Point[points.Count];
                for (int j = 0; j < points.Count; j++)
                {
                    inArray[j] = points[j];
                }
                inArray = PolygonClipping.sClipPolygonByRect(inArray, cr);
                if (inArray == null)
                {
                    return null;
                }
                points = Utils.ToCollection(inArray);
            }
            return RenderSegment(points);
        }

        protected PathFigure RenderSegment(Point[] pts)
        {
            return RenderSegment(pts.ToCollection());
        }

        PathFigure RenderSegment(PointCollection pts)
        {
            PathFigure figure = new PathFigure();
            figure.StartPoint = pts[0];
            figure.IsClosed = true;
            figure.IsFilled = true;
            figure.Segments.Add(new PolyLineSegment { Points = pts });
            return figure;
        }

        protected override Shape LegendShape
        {
            get { return base.DefaultLegendShape; }
        }
    }
}

