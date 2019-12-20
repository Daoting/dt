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
    public partial class Bar : PlotElement, ICustomClipping
    {
        protected RectangleGeometry geometry = new RectangleGeometry();

        public Bar()
        {
            ((Path) base.Shape).Data = geometry;
        }

        internal override object Clone()
        {
            Bar clone = new Bar();
            base.CloneAttributes(clone);
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
            geometry.Rect = new Rect(0.0, 0.0, rect.Width, rect.Height);
            Canvas.SetLeft(this, rect.X);
            Canvas.SetTop(this, rect.Y);
            RectangleGeometry geometry2 = new RectangleGeometry();
            geometry2.Rect = new Rect(-1.0, -1.0, rect.Width + 2.0, rect.Height + 2.0);
            base.Clip = geometry2;
            return true;
        }

        protected override bool IsClustered
        {
            get { return  true; }
        }
    }
}

