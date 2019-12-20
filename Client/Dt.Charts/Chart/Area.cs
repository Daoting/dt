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
        private double _origin;

        public Area()
        {
            base.m_isFilled = true;
        }

        internal override object Clone()
        {
            Area clone = new Area();
            base.CloneAttributes(clone);
            clone.Smoothed = base.Smoothed;
            clone.Origin = Origin;
            return clone;
        }

        private static Point[] InitPrevious(RenderContext rc, bool inverted, Point[] pts, double[] prev)
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
            bool isStacked = renderer.IsStacked;
            bool flag3 = renderer is RadarRenderer;
            double naN = double.NaN;
            if ((rc.OptimizationRadiusScope & OptimizationRadiusScope.Lines) > ((OptimizationRadiusScope) 0))
            {
                naN = rc.OptimizationRadius;
            }
            double[] previousValues = rc.PreviousValues;
            Point[] pts = null;
            PathGeometry geometry = base._geometry;
            if (isStacked && (previousValues != null))
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
                double num5;
                if (renderer.IsStacked100 || renderer.IsStacked)
                {
                    num5 = inverted ? rc.ConvertX(0.0) : rc.ConvertY(0.0);
                }
                else
                {
                    num5 = inverted ? rc.ConvertX(Origin) : rc.ConvertY(Origin);
                }
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
                        PathFigure figure2 = null;
                        if (flag3)
                        {
                            figure2 = RenderSegment(pointArray4);
                        }
                        else
                        {
                            figure2 = RenderNonStacked(pointArray4, num5, inverted, naN, cr);
                        }
                        if (figure2 != null)
                        {
                            geometry.Figures.Add(figure2);
                        }
                    }
                }
            }
            RectangleGeometry geometry2 = new RectangleGeometry();
            geometry2.Rect = new Rect(rc.Bounds.X, rc.Bounds.Y, rc.Bounds.Width, rc.Bounds.Height);
            base.Clip = geometry2;
            return true;
        }

        internal PathFigure RenderNonStacked(Point[] pts, double origin, bool inverted, double optRadius, Rect cr)
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
            PathFigure figure = new PathFigure();
            figure.StartPoint = pts[0];
            figure.IsClosed = true;
            figure.Segments = new PathSegmentCollection();
            PolyLineSegment segment = new PolyLineSegment();
            segment.Points = pts.ToCollection();
            figure.Segments.Add(segment);
            return figure;
        }

        private PathFigure RenderSegment(PointCollection pts)
        {
            PathFigure figure = new PathFigure();
            figure.StartPoint = pts[0];
            figure.IsClosed = true;
            figure.Segments = new PathSegmentCollection();
            PolyLineSegment segment = new PolyLineSegment();
            segment.Points = pts;
            figure.Segments.Add(segment);
            return figure;
        }

        protected override Shape LegendShape
        {
            get { return  base.DefaultLegendShape; }
        }

        public double Origin
        {
            get { return  _origin; }
            set { _origin = value; }
        }
    }
}

