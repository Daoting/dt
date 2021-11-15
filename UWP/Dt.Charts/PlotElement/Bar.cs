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
#endregion

namespace Dt.Charts
{
    public partial class Bar : PlotElement, ICustomClipping
    {
        Rect _labelRect;

        public Bar()
        {
        }

        internal override object Clone()
        {
            Bar clone = new Bar();
            CloneAttributes(clone);
            return clone;
        }

        protected override bool Render(RenderContext rc)
        {
            if (double.IsInfinity(rc.Rect.Width) || double.IsInfinity(rc.Rect.Height))
            {
                return false;
            }
            Rect rect = rc.Rect;
            if (double.IsNaN(rect.X) || double.IsNaN(rect.Y))
            {
                return false;
            }
            if ((rect.Width == 0.0) || (rect.Height == 0.0))
            {
                BaseRenderer renderer = rc.Renderer as BaseRenderer;
                if ((renderer != null) && (renderer.Stacked != StackedOptions.None))
                {
                    return false;
                }
            }
            rect = rect.IntersectRect(rc.Bounds);
            if (rect.IsEmptyRect())
            {
                return false;
            }

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            PathFigure pf = new PathFigure();
            pf.Segments.Add(new LineSegment { Point = new Point() });
            pf.Segments.Add(new LineSegment { Point = new Point(rect.Width, 0) });
            pf.Segments.Add(new LineSegment { Point = new Point(rect.Width, rect.Height) });
            pf.Segments.Add(new LineSegment { Point = new Point(0, rect.Height) });
            pf.Segments.Add(new LineSegment { Point = new Point() });
            geometry.Figures.Add(pf);
            Data = geometry;

            Canvas.SetLeft(this, rect.X);
            Canvas.SetTop(this, rect.Y);

            RectangleGeometry geometry2 = new RectangleGeometry();
            _labelRect = new Rect(-1.0, -1.0, rect.Width + 2.0, rect.Height + 2.0);
            geometry2.Rect = _labelRect;
            Clip = geometry2;
            return true;
        }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }

        protected override bool IsClustered
        {
            get { return  true; }
        }
    }
}

