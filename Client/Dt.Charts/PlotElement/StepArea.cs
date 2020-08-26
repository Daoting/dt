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
#endregion

namespace Dt.Charts
{
    public partial class StepArea : Area
    {
        Rect _labelRect;

        internal override object Clone()
        {
            StepArea clone = new StepArea();
            base.CloneAttributes(clone);
            clone.Smoothed = base.Smoothed;
            return clone;
        }

        protected override bool Render(RenderContext rc)
        {
            Point[] points = rc.Points;
            if (points == null)
            {
                return false;
            }

            BaseRenderer renderer = rc.Renderer as BaseRenderer;
            bool inverted = (renderer != null) && renderer.Inverted;
            bool flag2 = (renderer != null) && renderer.IsStacked;
            double[] previousValues = rc.PreviousValues;
            double d = inverted ? rc.ConvertX(0.0) : rc.ConvertY(0.0);
            double naN = double.NaN;
            if ((rc.OptimizationRadiusScope & OptimizationRadiusScope.Lines) > ((OptimizationRadiusScope) 0))
            {
                naN = rc.OptimizationRadius;
            }
            if (double.IsNaN(d))
            {
                d = inverted ? (rc.XReversed ? (rc.Bounds2D.X + rc.Bounds2D.Width) : rc.Bounds2D.X) : (rc.YReversed ? rc.Bounds2D.Y : (rc.Bounds2D.Y + rc.Bounds2D.Height));
            }
            Rect cr = rc.IsCustomClipping ? new Rect(rc.Bounds2D.X - 2.0, rc.Bounds2D.Y - 2.0, rc.Bounds2D.Width + 4.0, rc.Bounds2D.Height + 4.0) : Extensions.EmptyRect;

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            if (flag2 && (previousValues != null))
            {
                points = Lines.CreateSteps(points, inverted);
                previousValues = Lines.CreateSteps(previousValues);
                int length = points.Length;
                Point[] pts = new Point[2 * length];
                for (int i = 0; i < length; i++)
                {
                    pts[i] = points[i];
                    if (inverted)
                    {
                        pts[length + i] = new Point(rc.ConvertX(previousValues[(length - i) - 1]), points[(length - i) - 1].Y);
                    }
                    else
                    {
                        pts[length + i] = new Point(points[(length - i) - 1].X, rc.ConvertY(previousValues[(length - i) - 1]));
                    }
                }
                if (pts != null)
                {
                    PathFigure figure = base.RenderSegment(pts);
                    geometry.Figures.Add(figure);
                }
            }
            else
            {
                points = Lines.CreateSteps(points, inverted);
                List<Point[]> list = base.SplitPointsWithHoles(points);
                if (list != null)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        PathFigure figure2 = null;
                        figure2 = base.RenderNonStacked(list[j], d, inverted, naN, cr);
                        if (figure2 != null)
                        {
                            geometry.Figures.Add(figure2);
                        }
                    }
                }
            }
            Data = geometry;

            RectangleGeometry geometry3 = new RectangleGeometry();
            _labelRect = rc.Bounds2D;
            geometry3.Rect = _labelRect;
            Clip = geometry3;
            return true;
        }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }
    }
}

