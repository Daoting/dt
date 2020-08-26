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
        Rect _labelRect;

        internal override object Clone()
        {
            HLBar clone = new HLBar();
            base.CloneAttributes(clone);
            return clone;
        }

        protected override bool Render(RenderContext rc)
        {
            bool inverted = ((Renderer2D)rc.Renderer).Inverted;
            double left = 0.0;
            double top = 0.0;

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            if (inverted)
            {
                double num3 = rc.ConvertX(rc["HighValues"]);
                double num4 = rc.ConvertX(rc["LowValues"]);
                if (num3 < num4)
                {
                    double num5 = num4;
                    num4 = num3;
                    num3 = num5;
                }
                double width = num3 - num4;
                PathFigure pf = new PathFigure();
                pf.Segments.Add(new LineSegment { Point = new Point() });
                pf.Segments.Add(new LineSegment { Point = new Point(width, 0) });
                pf.Segments.Add(new LineSegment { Point = new Point(width, Size.Width) });
                pf.Segments.Add(new LineSegment { Point = new Point(0, Size.Width) });
                pf.Segments.Add(new LineSegment { Point = new Point() });
                geometry.Figures.Add(pf);

                left = num4;
                top = rc.Current.Y - (0.5 * Size.Height);
            }
            else
            {
                double num7 = rc.ConvertY(rc["HighValues"]);
                double num8 = rc.ConvertY(rc["LowValues"]);
                if (num7 < num8)
                {
                    double num9 = num8;
                    num8 = num7;
                    num7 = num9;
                }
                double height = num7 - num8;
                PathFigure pf = new PathFigure();
                pf.Segments.Add(new LineSegment { Point = new Point() });
                pf.Segments.Add(new LineSegment { Point = new Point(Size.Width, 0) });
                pf.Segments.Add(new LineSegment { Point = new Point(Size.Width, height) });
                pf.Segments.Add(new LineSegment { Point = new Point(0, height) });
                pf.Segments.Add(new LineSegment { Point = new Point() });
                geometry.Figures.Add(pf);

                left = rc.Current.X - (0.5 * Size.Width);
                top = num8;
            }
            Data = geometry;

            if (double.IsNaN(left) || double.IsNaN(top))
            {
                return false;
            }

            Canvas.SetLeft(this, left);
            Canvas.SetTop(this, top);
            _labelRect = rc.Bounds2D;
            RectangleGeometry geo = new RectangleGeometry();
            geo.Rect = new Rect(_labelRect.X - left, _labelRect.Y - top, _labelRect.Width, _labelRect.Height);
            Clip = geo;
            return true;
        }

        internal override Rect LabelRect
        {
            get { return _labelRect; }
        }

        protected override bool IsClustered
        {
            get { return  false; }
        }
    }
}

