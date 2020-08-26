#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class Lines : PlotElement
    {
        bool _smoothed;
        bool _xinc = false;
        Rect _labelRect;

        public Lines()
        {
            StrokeLineJoin = PenLineJoin.Bevel;
            StrokeEndLineCap = PenLineCap.Round;
            StrokeStartLineCap = PenLineCap.Round;
        }

        static Point[] ClipPoints(Rect bnds, Point[] pts)
        {
            int num = -1;
            int num2 = -1;
            int length = pts.Length;
            for (int i = 0; i < length; i++)
            {
                if (num == -1)
                {
                    if (pts[i].X >= bnds.Left)
                    {
                        num = Math.Min(0, i - 1);
                    }
                }
                else if ((num2 == -1) && (pts[i].X >= bnds.Right))
                {
                    num2 = i;
                    break;
                }
            }
            if ((num < 0) || (num2 <= num))
            {
                return pts;
            }
            Point[] pointArray = new Point[(num2 - num) + 1];
            for (int j = num; j <= num2; j++)
            {
                pointArray[j - num] = pts[j];
            }
            return pointArray;
        }

        internal override object Clone()
        {
            Lines clone = new Lines();
            base.CloneAttributes(clone);
            clone.Smoothed = Smoothed;
            clone.IsFilled = IsFilled;
            clone.IsClosed = IsClosed;
            return clone;
        }

        internal static double[] CreateSteps(double[] pts)
        {
            if ((pts == null) || (pts.Length < 2))
            {
                return null;
            }
            int length = pts.Length;
            double[] numArray = new double[(2 * length) - 1];
            int index = 0;
            int num3 = 0;
            while (index < length)
            {
                numArray[num3++] = pts[index];
                if (index < (length - 1))
                {
                    numArray[num3++] = pts[index];
                }
                index++;
            }
            return numArray;
        }

        internal static Point[] CreateSteps(Point[] pts, bool inverted)
        {
            if ((pts == null) || (pts.Length < 2))
            {
                return null;
            }
            int length = pts.Length;
            Point[] pointArray = new Point[(2 * length) - 1];
            int index = 0;
            int num3 = 0;
            while (index < length)
            {
                pointArray[num3++] = pts[index];
                if (index < (length - 1))
                {
                    if (inverted)
                    {
                        pointArray[num3++] = new Point(pts[index].X, pts[index + 1].Y);
                    }
                    else
                    {
                        pointArray[num3++] = new Point(pts[index + 1].X, pts[index].Y);
                    }
                }
                index++;
            }
            return pointArray;
        }

        protected Point[] Decimate(Point[] pts, double radius)
        {
            List<Point> list = new List<Point>();
            Point point = pts[0];
            list.Add(point);
            int length = pts.Length;
            for (int i = 1; i < length; i++)
            {
                Point point2 = pts[i];
                if (((Math.Abs((double) (point2.X - point.X)) > radius) || (Math.Abs((double) (point2.Y - point.Y)) > radius)) || (i == (length - 1)))
                {
                    point = pts[i];
                    list.Add(point);
                }
            }
            return list.ToArray();
        }

        protected PointCollection DecimateAsCollection(Point[] pts, double radius)
        {
            PointCollection points = new PointCollection();
            Point point = pts[0];
            points.Add(point);
            int length = pts.Length;
            for (int i = 1; i < length; i++)
            {
                Point point2 = pts[i];
                if (((Math.Abs((double) (point2.X - point.X)) > radius) || (Math.Abs((double) (point2.Y - point.Y)) > radius)) || (i == (length - 1)))
                {
                    point = pts[i];
                    points.Add(point);
                }
            }
            return points;
        }

        protected override bool IsCompatible(IRenderer rend)
        {
            if (!base.IsCompatible(rend))
            {
                return (rend is RadarRenderer);
            }
            return true;
        }

        protected override bool Render(RenderContext rc)
        {
            Point[] points = rc.Points;
            if (points == null)
            {
                return false;
            }
            Rect clipBounds = rc.ClipBounds;
            if (_xinc)
            {
                points = ClipPoints(rc.Bounds, points);
            }
            clipBounds = Utils.InflateRect(clipBounds, base.StrokeThickness, base.StrokeThickness);
            bool isCustomClipping = rc.IsCustomClipping;
            double naN = double.NaN;
            if ((rc.OptimizationRadiusScope & OptimizationRadiusScope.Lines) > ((OptimizationRadiusScope) 0))
            {
                naN = rc.OptimizationRadius;
            }

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            if (rc.hasNan)
            {
                List<Point[]> list = SplitPointsWithHoles(points);
                if (list != null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        PathFigure[] figureArray = RenderSegment(list[i], naN, isCustomClipping ? clipBounds : Extensions.EmptyRect);
                        int length = figureArray.Length;
                        for (int j = 0; j < length; j++)
                        {
                            geometry.Figures.Add(figureArray[j]);
                        }
                    }
                }
            }
            else
            {
                PathFigure[] figureArray2 = RenderSegment(points, naN, isCustomClipping ? clipBounds : Extensions.EmptyRect);
                if (figureArray2 != null)
                {
                    int num5 = figureArray2.Length;
                    for (int k = 0; k < num5; k++)
                    {
                        geometry.Figures.Add(figureArray2[k]);
                    }
                }
            }
            Data = geometry;
            _labelRect = clipBounds;

            if (!isCustomClipping)
            {
                RectangleGeometry geometry2 = new RectangleGeometry();
                geometry2.Rect = clipBounds;
                base.Clip = geometry2;
            }
            return true;
        }

        protected PathFigure[] RenderSegment(Point[] pts, double optRadius, Rect clip)
        {
            if ((pts == null) || (pts.Length == 0))
            {
                return null;
            }
            if (Smoothed && (pts.Length > 3))
            {
                pts = new SplineNew(pts).Calculate();
            }
            else if (!double.IsNaN(optRadius))
            {
                pts = Decimate(pts, optRadius);
            }
            if (clip.IsEmptyRect())
            {
                PathFigure figure = new PathFigure();
                figure.StartPoint = pts[0];
                figure.IsClosed = IsClosed;
                figure.IsFilled = IsFilled;
                PolyLineSegment segment = new PolyLineSegment();
                segment.Points = pts.ToCollection();
                figure.Segments.Add(segment);
                return new PathFigure[] { figure };
            }
            if (!IsClosed)
            {
                return ClippingEngine.CreateClippedLines(pts, clip);
            }
            pts = PolygonClipping.sClipPolygonByRect(pts, clip);
            if ((pts == null) || (pts.Length <= 0))
            {
                return null;
            }
            PathFigure figure2 = new PathFigure();
            figure2.StartPoint = pts[0];
            figure2.IsClosed = IsClosed;
            figure2.IsFilled = IsFilled;
            figure2.Segments = new PathSegmentCollection();
            if (!IsFilled)
            {
                PathFigure[] figureArray = new PathFigure[pts.Length];
                PathFigure figure4 = new PathFigure();
                figure4.StartPoint = pts[pts.Length - 1];
                figureArray[0] = figure4;
                LineSegment segment3 = new LineSegment();
                segment3.Point = pts[0];
                figureArray[0].Segments.Add(segment3);
                for (int i = 1; i < figureArray.Length; i++)
                {
                    PathFigure figure3 = new PathFigure();
                    figure3.StartPoint = pts[i - 1];
                    figureArray[i] = figure3;
                    LineSegment segment2 = new LineSegment();
                    segment2.Point = pts[i];
                    figureArray[i].Segments.Add(segment2);
                }
                return figureArray;
            }
            PolyLineSegment segment4 = new PolyLineSegment();
            segment4.Points = pts.ToCollection();
            figure2.Segments.Add(segment4);
            return new PathFigure[] { figure2 };
        }

        protected List<Point[]> SplitPointsWithHoles(Point[] pts)
        {
            if ((pts == null) || (pts.Length == 0))
            {
                return null;
            }
            List<Point[]> list = new List<Point[]>();
            int length = pts.Length;
            List<Point> list2 = new List<Point>();
            for (int i = 0; i < length; i++)
            {
                if (double.IsNaN(pts[i].X) || double.IsNaN(pts[i].Y))
                {
                    if (list2.Count > 0)
                    {
                        list.Add(list2.ToArray());
                        list2.Clear();
                    }
                }
                else
                {
                    list2.Add(pts[i]);
                }
            }
            if (list2.Count > 0)
            {
                list.Add(list2.ToArray());
            }
            return list;
        }

        public bool IsClosed { get; set; }

        public bool IsFilled { get; set; }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }

        protected override Shape LegendShape
        {
            get
            {
                if (IsFilled)
                {
                    return base.DefaultLegendShape;
                }
                Line line = new Line();
                line.VerticalAlignment = VerticalAlignment.Center;
                line.HorizontalAlignment = HorizontalAlignment.Stretch;
                line.X1 = 2.0;
                line.X2 = 22.0;
                line.Stroke = Utils.Clone(base.Stroke);
                line.StrokeThickness = base.StrokeThickness;
                TranslateTransform transform = new TranslateTransform();
                transform.Y = 0.5 * base.StrokeThickness;
                line.RenderTransform = transform;
                line.Tag = this;
                return line;
            }
        }

        public bool Smoothed
        {
            get { return  _smoothed; }
            set { _smoothed = value; }
        }
    }
}

