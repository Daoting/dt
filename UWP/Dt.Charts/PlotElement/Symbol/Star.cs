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
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Charts
{
    public partial class Star : RPolygon
    {
        public static readonly DependencyProperty InnerRadiusProperty = Utils.RegisterProperty("InnerRadius", typeof(double), typeof(Star), new PropertyChangedCallback(Star.OnInnerRadiusChanged), (double) 0.5);

        public Star()
        {
            InnerRadius = 0.5;
            base.NumVertices = 4;
        }

        internal override object Clone()
        {
            Star clone = new Star {
                NumVertices = base.NumVertices,
                InnerRadius = InnerRadius
            };
            base.CloneAttributes(clone);
            return clone;
        }

        static void OnInnerRadiusChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            Star star = (Star) obj;
            star.UpdateGeometry(star.Size);
        }

        protected override void UpdateGeometry(Size sz)
        {
            if (sz.IsEmpty)
                sz = Size;

            double rx = 0.5 * sz.Width;
            double ry = 0.5 * sz.Height;
            double num3 = 0.5 * base.StrokeThickness;
            double innerRadius = InnerRadius;
            if ((innerRadius > 1.0) || (innerRadius < 0.0))
            {
                innerRadius = 0.5;
            }
            double num5 = innerRadius * rx;
            double num6 = innerRadius * ry;
            Point center = new Point(rx + num3, ry + num3);
            double numVertices = base.NumVertices;
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(center.X + rx, center.Y);
            figure.IsFilled = true;
            figure.IsClosed = true;
            for (int i = 1; i <= numVertices; i++)
            {
                double d = (((i - 0.5) / numVertices) * 2.0) * 3.1415926535897931;
                double x = center.X + (num5 * Math.Cos(d));
                double y = center.Y + (num6 * Math.Sin(d));
                LineSegment segment = new LineSegment();
                segment.Point = new Point(x, y);
                figure.Segments.Add(segment);
                if (i == numVertices)
                {
                    break;
                }
                d = ((((double) i) / numVertices) * 2.0) * 3.1415926535897931;
                x = center.X + (rx * Math.Cos(d));
                y = center.Y + (ry * Math.Sin(d));
                LineSegment segment2 = new LineSegment();
                segment2.Point = new Point(x, y);
                figure.Segments.Add(segment2);
            }

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            Data = geometry;

            Canvas.SetLeft(this, (_symCenter.X - rx) - num3);
            Canvas.SetTop(this, (_symCenter.Y - ry) - num3);
        }

        public double InnerRadius
        {
            get { return (double)GetValue(InnerRadiusProperty); }
            set { SetValue(InnerRadiusProperty, value); }
        }

        public override Size Size
        {
            get { return  base.Size; }
            set
            {
                base.Size = value;
                UpdateGeometry(Size.Empty);
            }
        }
    }
}

