#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Charts
{
    public partial class PlotElement : Path, IPlotElement
    {
        private Point _center = new Point();
        private Rect _rect = new Rect();
        private Windows.UI.Xaml.Shapes.Shape _shape;
        private Size _sz = new Size(10.0, 10.0);
        private Dt.Charts.TransformMode _tmode;
        protected ShapeStyle autoStyle;
        public static readonly DependencyProperty DataPointProperty = Utils.RegisterProperty("DataPoint", typeof(Dt.Charts.DataPoint), typeof(PlotElement), new PropertyChangedCallback(PlotElement.OnChanged));
        protected List<UIElement> effects;
        public static readonly DependencyProperty LabelAlignmentProperty = DependencyProperty.RegisterAttached("LabelAlignment", (Type)typeof(LabelAlignment), (Type)typeof(PlotElement), new PropertyMetadata(LabelAlignment.Auto));
        public static readonly DependencyProperty LabelLineProperty = DependencyProperty.RegisterAttached("LabelLine", (Type)typeof(Line), (Type)typeof(PlotElement), null);
        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.RegisterAttached("LabelOffset", (Type)typeof(Point), (Type)typeof(PlotElement), new PropertyMetadata(new Point(0.0, 0.0)));
        internal static readonly DependencyProperty LabelProperty = Utils.RegisterProperty("Label", typeof(UIElement), typeof(PlotElement), new PropertyChangedCallback(PlotElement.OnChanged), null);
        protected bool m_isFilled = true;

        internal event EventHandler AfterLoaded;

        public event EventHandler Created;

        new public event RoutedEventHandler Loaded;

        public event PointerEventHandler MouseEnter;

        public event PointerEventHandler MouseLeave;

        public event PointerEventHandler MouseLeftButtonDown;

        public event PointerEventHandler MouseLeftButtonUp;

        internal PlotElement()
        {
            Shape = new Path();
            Path path = this;
            path.Loaded += FireLoaded;
            Path path2 = this;
            path2.PointerEntered += FireMouseEnter;
            Path path3 = this;
            path3.PointerExited += FireMouseLeave;
            Path path4 = this;
            path4.PointerPressed += FireMouseLeftButtonDown;
            Path path5 = this;
            path5.PointerReleased += FireMouseLeftButtonUp;
        }

        protected void AdjustLegendShape(Windows.UI.Xaml.Shapes.Shape shape)
        {
            if (shape != null)
            {
                if ((Size.Width <= 14.0) && (Size.Width > 0.0))
                {
                    shape.Width = Size.Width;
                }
                if ((Size.Height <= 14.0) && (Size.Height > 0.0))
                {
                    shape.Height = Size.Height;
                }
            }
        }

        protected virtual LabelAlignment AutoPosition(Size labelSize, ref Point hot, ref Point offset)
        {
            return LabelAlignment.TopCenter;
        }

        bool IPlotElement.IsCompatible(IRenderer rend)
        {
            return IsCompatible(rend);
        }

        bool IPlotElement.Render(RenderContext rc)
        {
            return Render(rc);
        }

        void IPlotElement.SetShape(ShapeStyle ss)
        {
            if ((autoStyle == null) && (ss != null))
            {
                autoStyle = ss;
                PieSlice slice = this as PieSlice;
                Point center = (slice != null) ? slice._pi.Center : new Point(double.NaN, double.NaN);
                Rect bounds = (slice != null) ? slice._geometry.Bounds : Extensions.EmptyRect;
                double radius = (slice != null) ? slice._pi.RadiusX : double.NaN;
                autoStyle.Apply(this, Shape, center, bounds, radius);
            }
        }

#if ANDROID
    new
#endif
        internal virtual object Clone()
        {
            PlotElement clone = new PlotElement();
            CloneAttributes(clone);
            return clone;
        }

        protected void CloneAttributes(PlotElement clone)
        {
            if (base.Fill != null)
            {
                clone.Fill = Utils.Clone(base.Fill);
            }
            if (base.Stroke != null)
            {
                clone.Stroke = Utils.Clone(base.Stroke);
            }
            clone.StrokeThickness = base.StrokeThickness;
            clone.Size = Size;
            clone.IsHitTestVisible = base.IsHitTestVisible;
            clone.Style = base.Style;
            clone.Loaded += new RoutedEventHandler(FireLoaded);
            clone.MouseEnter += new PointerEventHandler(FireMouseEnter);
            clone.MouseLeave += new PointerEventHandler(FireMouseLeave);
            clone.MouseLeftButtonDown += new PointerEventHandler(FireMouseLeftButtonDown);
            clone.MouseLeftButtonUp += new PointerEventHandler(FireMouseLeftButtonUp);
            if (Created != null)
            {
                Created(clone, EventArgs.Empty);
            }
        }

        private void fe_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.NewSize.Width > 0.0) && (e.NewSize.Height > 0.0))
            {
                UpdateLabelPosition();
            }
        }

        private void FireLoaded(object sender, RoutedEventArgs e)
        {
            if (Loaded != null)
            {
                Loaded(sender, e);
            }
            DataSeries series = (DataPoint != null) ? DataPoint.Series : null;
            if (series != null)
            {
                series.FireLoaded(this, EventArgs.Empty);
            }
            if (AfterLoaded != null)
            {
                AfterLoaded(this, EventArgs.Empty);
            }
        }

        private void FireMouseEnter(object sender, PointerRoutedEventArgs e)
        {
            if (MouseEnter != null)
            {
                MouseEnter(sender, e);
            }
        }

        private void FireMouseLeave(object sender, PointerRoutedEventArgs e)
        {
            if (MouseLeave != null)
            {
                MouseLeave(sender, e);
            }
        }

        private void FireMouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            if (MouseLeftButtonDown != null)
            {
                MouseLeftButtonDown(sender, e);
            }
        }

        private void FireMouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            if (MouseLeftButtonUp != null)
            {
                MouseLeftButtonUp(sender, e);
            }
        }

        internal void FirePlotElementLoaded(object sender, RoutedEventArgs e)
        {
            DataSeries series = (DataPoint != null) ? DataPoint.Series : null;
            if (series != null)
            {
                series.FirePlotElementLoaded(this, EventArgs.Empty);
            }
        }

        public static LabelAlignment GetLabelAlignment(DependencyObject obj)
        {
            return (LabelAlignment)obj.GetValue(LabelAlignmentProperty);
        }

        public static Line GetLabelLine(DependencyObject obj)
        {
            return (Line)obj.GetValue(LabelLineProperty);
        }

        public static Point GetLabelOffset(DependencyObject obj)
        {
            return (Point)obj.GetValue(LabelOffsetProperty);
        }

        protected virtual bool IsCompatible(IRenderer rend)
        {
            return (rend is Renderer2D);
        }

        private static void OnChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
        }

        protected virtual bool Render(RenderContext rc)
        {
            _shape.Width = Size.Width;
            _shape.Height = Size.Height;
            Canvas.SetLeft(this, rc.Current.X - (0.5 * _shape.Width));
            Canvas.SetTop(this, rc.Current.Y - (0.5 * _shape.Height));
            return true;
        }

        public static void SetLabelAlignment(DependencyObject obj, LabelAlignment value)
        {
            obj.SetValue(LabelAlignmentProperty, value);
        }

        public static void SetLabelLine(DependencyObject obj, Line line)
        {
            obj.SetValue(LabelLineProperty, line);
        }

        public static void SetLabelOffset(DependencyObject obj, Point offset)
        {
            obj.SetValue(LabelOffsetProperty, offset);
        }

        internal static PlotElement SymbolFromMarker(Marker marker)
        {
            switch (marker)
            {
                case Marker.Box:
                    return new BoxSymbol();

                case Marker.Dot:
                    return new DotSymbol();

                case Marker.Diamond:
                    {
                        BoxSymbol symbol = new BoxSymbol();
                        RotateTransform transform = new RotateTransform();
                        transform.Angle = 45.0;
                        symbol.RenderTransform = transform;
                        symbol.RenderTransformOrigin = new Point(0.5, 0.5);
                        return symbol;
                    }
                case Marker.Triangle:
                    {
                        RPolygon polygon = new RPolygon();
                        RotateTransform transform2 = new RotateTransform();
                        transform2.Angle = -90.0;
                        polygon.RenderTransform = transform2;
                        polygon.RenderTransformOrigin = new Point(0.5, 0.5);
                        return polygon;
                    }
                case Marker.Star4:
                    return new Star { InnerRadius = 0.4 };

                case Marker.Star8:
                    return new Star { NumVertices = 8, InnerRadius = 0.4 };

                case Marker.Cross:
                    return new Star { InnerRadius = 0.0 };

                case Marker.DiagonalCross:
                    {
                        Star star4 = new Star();
                        RotateTransform transform3 = new RotateTransform();
                        transform3.Angle = 45.0;
                        star4.RenderTransform = transform3;
                        star4.RenderTransformOrigin = new Point(0.5, 0.5);
                        star4.InnerRadius = 0.0;
                        return star4;
                    }
            }
            return null;
        }

        internal virtual void UpdateLabelPosition()
        {
            if ((Label != null) && (base.Parent is Canvas))
            {
                FrameworkElement label = Label as FrameworkElement;
                Size labelSize = new Size(label.ActualWidth, label.ActualHeight);
                Rect labelRect = LabelRect;
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);
                if (!double.IsNaN(left))
                {
                    labelRect.X += left;
                }
                if (!double.IsNaN(top))
                {
                    labelRect.Y += top;
                }
                double naN = double.NaN;
                double d = double.NaN;
                LabelAlignment labelAlignment = GetLabelAlignment(Label);
                Point labelOffset = GetLabelOffset(Label);
                Point hot = new Point();
                if (labelAlignment == LabelAlignment.Auto)
                {
                    labelAlignment = AutoPosition(labelSize, ref hot, ref labelOffset);
                }
                switch (labelAlignment)
                {
                    case LabelAlignment.BottomCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        d = labelRect.Bottom;
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Bottom);
                        break;

                    case LabelAlignment.BottomLeft:
                        naN = labelRect.X - labelSize.Width;
                        d = labelRect.Bottom;
                        hot = new Point(labelRect.Left, labelRect.Bottom);
                        break;

                    case LabelAlignment.BottomRight:
                        naN = labelRect.Right;
                        d = labelRect.Bottom;
                        hot = new Point(labelRect.Right, labelRect.Bottom);
                        break;

                    case LabelAlignment.MiddleCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        d = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.MiddleLeft:
                        naN = labelRect.X - labelSize.Width;
                        d = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.X, labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.MiddleRight:
                        naN = labelRect.Right;
                        d = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.Right, labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.TopCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        d = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Top);
                        break;

                    case LabelAlignment.TopLeft:
                        naN = labelRect.X - labelSize.Width;
                        d = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.Left, labelRect.Top);
                        break;

                    case LabelAlignment.TopRight:
                        naN = labelRect.Right;
                        d = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.Right, labelRect.Top);
                        break;
                }
                if (DataPoint != null)
                {
                    DataPoint.Point = hot;
                }
                if ((labelOffset.X != 0.0) || (labelOffset.Y != 0.0))
                {
                    Line labelLine = GetLabelLine(Label);
                    if (labelLine != null)
                    {
                        labelLine.X1 = hot.X;
                        labelLine.Y1 = hot.Y;
                    }
                    naN += labelOffset.X;
                    d += labelOffset.Y;
                    if (labelLine != null)
                    {
                        labelLine.X2 = hot.X + labelOffset.X;
                        labelLine.Y2 = hot.Y + labelOffset.Y;
                    }
                    DataPoint.LabelPoint = new Point(hot.X + labelOffset.X, hot.Y + labelOffset.Y);
                }
                if ((!double.IsNaN(naN) && !double.IsInfinity(naN)) && (!double.IsNaN(d) && !double.IsInfinity(d)))
                {
                    Canvas.SetLeft(label, naN);
                    Canvas.SetTop(label, d);
                }
                else
                {
                    label.Visibility = Visibility.Collapsed;
                }
            }
        }

        internal static void UpdateLabelPosition(PlotElement pe, FrameworkElement lbl)
        {
            if ((pe != null) && (lbl != null))
            {
                Size labelSize = Utils.GetSize(lbl);
                Rect labelRect = pe.LabelRect;
                double left = Canvas.GetLeft(pe);
                double top = Canvas.GetTop(pe);
                if (!double.IsNaN(left))
                {
                    labelRect.X += left;
                }
                if (!double.IsNaN(top))
                {
                    labelRect.Y += top;
                }
                double naN = double.NaN;
                double length = double.NaN;
                LabelAlignment labelAlignment = GetLabelAlignment(lbl);
                Point labelOffset = GetLabelOffset(lbl);
                Point hot = new Point();
                if (labelAlignment == LabelAlignment.Auto)
                {
                    labelAlignment = pe.AutoPosition(labelSize, ref hot, ref labelOffset);
                }
                switch (labelAlignment)
                {
                    case LabelAlignment.BottomCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        length = labelRect.Bottom;
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Bottom);
                        break;

                    case LabelAlignment.BottomLeft:
                        naN = labelRect.X - labelSize.Width;
                        length = labelRect.Bottom;
                        hot = new Point(labelRect.Left, labelRect.Bottom);
                        break;

                    case LabelAlignment.BottomRight:
                        naN = labelRect.Right;
                        length = labelRect.Bottom;
                        hot = new Point(labelRect.Right, labelRect.Bottom);
                        break;

                    case LabelAlignment.MiddleCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        length = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.MiddleLeft:
                        naN = labelRect.X - labelSize.Width;
                        length = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.X, labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.MiddleRight:
                        naN = labelRect.Right;
                        length = (labelRect.Y + (labelRect.Height * 0.5)) - (labelSize.Height * 0.5);
                        hot = new Point(labelRect.Right, labelRect.Y + (labelRect.Height * 0.5));
                        break;

                    case LabelAlignment.TopCenter:
                        naN = (labelRect.X + (labelRect.Width * 0.5)) - (labelSize.Width * 0.5);
                        length = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.X + (labelRect.Width * 0.5), labelRect.Top);
                        break;

                    case LabelAlignment.TopLeft:
                        naN = labelRect.X - labelSize.Width;
                        length = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.Left, labelRect.Top);
                        break;

                    case LabelAlignment.TopRight:
                        naN = labelRect.Right;
                        length = labelRect.Top - labelSize.Height;
                        hot = new Point(labelRect.Right, labelRect.Top);
                        break;
                }
                if ((labelOffset.X != 0.0) || (labelOffset.Y != 0.0))
                {
                    Line labelLine = GetLabelLine(lbl);
                    if (labelLine != null)
                    {
                        labelLine.X1 = hot.X;
                        labelLine.Y1 = hot.Y;
                    }
                    naN += labelOffset.X;
                    length += labelOffset.Y;
                    if (labelLine != null)
                    {
                        labelLine.X2 = hot.X + labelOffset.X;
                        labelLine.Y2 = hot.Y + labelOffset.Y;
                    }
                }
                Canvas.SetLeft(lbl, naN);
                Canvas.SetTop(lbl, length);
            }
        }

        internal ShapeStyle AutoStyle
        {
            get { return autoStyle; }
        }

        bool IPlotElement.IsClustered
        {
            get { return IsClustered; }
        }

        UIElement IPlotElement.Label
        {
            get { return Label; }
            set { Label = value; }
        }

        Windows.UI.Xaml.Shapes.Shape IPlotElement.LegendShape
        {
            get { return LegendShape; }
        }

        Style IPlotElement.Style
        {
            get { return Style; }
            set { base.Style = value; }
        }

#if IOS
    new
#endif
        public Point Center
        {
            get { return _center; }
            internal set { _center = value; }
        }

        public Dt.Charts.DataPoint DataPoint
        {
            get { return (Dt.Charts.DataPoint)base.GetValue(DataPointProperty); }
            internal set { base.SetValue(DataPointProperty, value); }
        }

        protected Windows.UI.Xaml.Shapes.Shape DefaultLegendShape
        {
            get
            {
                double num;
                Rectangle rectangle = new Rectangle();
                rectangle.RadiusY = num = 0.0;
                rectangle.RadiusX = num;
                rectangle.Fill = Utils.Clone(base.Fill);
                rectangle.Stroke = Utils.Clone(base.Stroke);
                rectangle.StrokeThickness = base.StrokeThickness;
                return rectangle;
            }
        }

        internal List<UIElement> Effects
        {
            get
            {
                if (effects == null)
                {
                    effects = new List<UIElement>();
                }
                return effects;
            }
        }

        protected virtual bool IsClustered
        {
            get { return false; }
        }

        internal UIElement Label
        {
            get { return (base.GetValue(LabelProperty) as UIElement); }
            set
            {
                base.SetValue(LabelProperty, value);
                FrameworkElement label = Label as FrameworkElement;
                if (label != null)
                {
                    label.SizeChanged += fe_SizeChanged;
                }
            }
        }

        internal virtual Rect LabelRect
        {
            get
            {
                Rect rect = new Rect();
                if (Shape is Path)
                {
                    return ((Path)Shape).Data.Bounds;
                }
                if (Shape is Ellipse)
                {
                    Ellipse shape = (Ellipse)Shape;
                    return new Rect(0.0, 0.0, shape.Width, shape.Height);
                }
                if (Shape is Rectangle)
                {
                    Rectangle rectangle = (Rectangle)Shape;
                    rect = new Rect(0.0, 0.0, rectangle.Width, rectangle.Height);
                }
                return rect;
            }
        }

        protected virtual Windows.UI.Xaml.Shapes.Shape LegendShape
        {
            get { return DefaultLegendShape; }
        }

        public Rect PlotRect
        {
            get { return _rect; }
            internal set { _rect = value; }
        }

        public Windows.UI.Xaml.Shapes.Shape Shape
        {
            get { return this; }
            internal set { _shape = value; }
        }

        public virtual Size Size
        {
            get { return _sz; }
            set { _sz = value; }
        }

        public Dt.Charts.TransformMode TransformMode
        {
            get { return _tmode; }
            set { _tmode = value; }
        }
    }
}

