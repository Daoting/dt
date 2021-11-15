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
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class RPolygon : Dt.Charts.Symbol
    {
        public static readonly DependencyProperty NumVerticesProperty = Utils.RegisterProperty("NumVertices", typeof(int), typeof(RPolygon), new PropertyChangedCallback(RPolygon.OnNumVerticesChanged), (int) 3);

        public RPolygon()
        {
            NumVertices = 3;
        }

        internal override object Clone()
        {
            RPolygon clone = new RPolygon();
            base.CloneAttributes(clone);
            clone.NumVertices = NumVertices;
            return clone;
        }

        static void OnNumVerticesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            RPolygon polygon = (RPolygon) obj;
            polygon.UpdateGeometry(polygon.Size);
        }

        protected override void UpdateGeometry(Size sz)
        {
            if (sz.IsEmpty)
                sz = Size;

            double rx = 0.5 * sz.Width;
            double ry = 0.5 * sz.Height;
            double num3 = 0.5 * base.StrokeThickness;
            Point center = new Point(rx + num3, ry + num3);
            double numVertices = NumVertices;
            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(center.X + rx, center.Y);
            figure.IsClosed = true;
            for (int i = 1; i < numVertices; i++)
            {
                double d = ((((double) i) / numVertices) * 2.0) * 3.1415926535897931;
                double x = center.X + (rx * Math.Cos(d));
                double y = center.Y + (ry * Math.Sin(d));
                LineSegment segment = new LineSegment();
                segment.Point = new Point(x, y);
                figure.Segments.Add(segment);
            }

            // uno不支持Path.Data为非PathGeometry！
            // wasm中在给Path.Data赋值前内容必须完整，后添加的Figures无效！众里寻他千百度，因为赋值没按顺序，操！
            PathGeometry geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            Data = geometry;

            Canvas.SetLeft(this, (_symCenter.X - rx) - num3);
            Canvas.SetTop(this, (_symCenter.Y - ry) - num3);
        }

        protected override Shape LegendShape
        {
            get
            {
                Path path = new Path();
                path.VerticalAlignment = VerticalAlignment.Center;
                path.HorizontalAlignment = HorizontalAlignment.Center;
                Size sz = Size;
                if (sz.Width > 16.0)
                {
                    sz.Width = 16.0;
                }
                if (sz.Height > 16.0)
                {
                    sz.Height = 16.0;
                }
                path.RenderTransform = RenderTransform;
                path.RenderTransformOrigin = RenderTransformOrigin;

                double rx = 0.5 * sz.Width;
                double ry = 0.5 * sz.Height;
                double num3 = 0.5 * base.StrokeThickness;
                Point center = new Point(rx + num3, ry + num3);
                double numVertices = NumVertices;
                PathFigure figure = new PathFigure();
                figure.StartPoint = new Point(center.X + rx, center.Y);
                figure.IsClosed = true;
                for (int i = 1; i < numVertices; i++)
                {
                    double d = ((((double)i) / numVertices) * 2.0) * 3.1415926535897931;
                    double x = center.X + (rx * Math.Cos(d));
                    double y = center.Y + (ry * Math.Sin(d));
                    LineSegment segment = new LineSegment();
                    segment.Point = new Point(x, y);
                    figure.Segments.Add(segment);
                }
                PathGeometry geometry = new PathGeometry();
                geometry.Figures.Add(figure);
                path.Data = geometry;

                return path;
            }
        }

        public int NumVertices
        {
            get { return (int)GetValue(NumVerticesProperty); }
            set { SetValue(NumVerticesProperty, value); }
        }
    }
}

