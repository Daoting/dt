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
    public partial class HLBar : Bar, ICustomClipping
    {
        internal override object Clone()
        {
            HLBar clone = new HLBar();
            base.CloneAttributes(clone);
            return clone;
        }

        protected override bool Render(RenderContext rc)
        {
            //RectangleGeometry geometry = base.geometry;
            //bool inverted = ((Renderer2D) rc.Renderer).Inverted;
            //double d = 0.0;
            //double num2 = 0.0;
            //if (inverted)
            //{
            //    double num3 = rc.ConvertX(rc["HighValues"]);
            //    double num4 = rc.ConvertX(rc["LowValues"]);
            //    if (num3 < num4)
            //    {
            //        double num5 = num4;
            //        num4 = num3;
            //        num3 = num5;
            //    }
            //    double width = num3 - num4;
            //    geometry.Rect = new Rect(0.0, 0.0, width, Size.Width);
            //    d = num4;
            //    num2 = rc.Current.Y - (0.5 * Size.Height);
            //}
            //else
            //{
            //    double num7 = rc.ConvertY(rc["HighValues"]);
            //    double num8 = rc.ConvertY(rc["LowValues"]);
            //    if (num7 < num8)
            //    {
            //        double num9 = num8;
            //        num8 = num7;
            //        num7 = num9;
            //    }
            //    double height = num7 - num8;
            //    geometry.Rect = new Rect(0.0, 0.0, Size.Width, height);
            //    d = rc.Current.X - (0.5 * Size.Width);
            //    num2 = num8;
            //}
            //if (double.IsNaN(d) || double.IsNaN(num2))
            //{
            //    return false;
            //}
            //Canvas.SetLeft(this, d);
            //Canvas.SetTop(this, num2);
            //Rect rect = rc.Bounds2D;
            //if (inverted)
            //{
            //    RectangleGeometry geometry2 = new RectangleGeometry();
            //    geometry2.Rect = new Rect(rect.X - d, rect.Y - num2, rect.Width, rect.Height);
            //    base.Clip = geometry2;
            //}
            //else
            //{
            //    RectangleGeometry geometry3 = new RectangleGeometry();
            //    geometry3.Rect = new Rect(rect.X - d, rect.Y - num2, rect.Width, rect.Height);
            //    base.Clip = geometry3;
            //}
            return true;
        }

        protected override bool IsClustered
        {
            get { return  false; }
        }
    }
}

