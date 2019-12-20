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
    public partial class BoxSymbol : Dt.Charts.Symbol
    {
        public BoxSymbol()
        {
            Geometry geometry;
            base.Data = geometry = new RectangleGeometry();
            base.geometry = geometry;
        }

        internal override object Clone()
        {
            BoxSymbol clone = new BoxSymbol();
            base.CloneAttributes(clone);
            return clone;
        }

        protected override void UpdateGeometry(PathGeometry pg, Size sz)
        {
            double num = 0.5 * Size.Width;
            double num2 = 0.5 * Size.Height;
            double x = 0.5 * base.StrokeThickness;
            ((RectangleGeometry) base.geometry).Rect = new Rect(x, x, Size.Width, Size.Height);
            Canvas.SetLeft(this, (symCenter.X - num) - x);
            Canvas.SetTop(this, (symCenter.Y - num2) - x);
        }

        protected override Shape LegendShape
        {
            get
            {
                Rectangle shape = new Rectangle();
                base.AdjustLegendShape(shape);
                shape.RenderTransform = base.RenderTransform;
                shape.RenderTransformOrigin = base.RenderTransformOrigin;
                return shape;
            }
        }
    }
}

