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
    /// <summary>
    /// 基础图元，继承Path时在uno中只支持Data为PathGeometry！
    /// </summary>
    public partial class PlotElement : Path, IPlotElement
    {
        #region 静态内容
        public static readonly DependencyProperty LabelLineProperty = DependencyProperty.RegisterAttached(
            "LabelLine",
            typeof(Line),
            typeof(PlotElement),
            null);
        public static Line GetLabelLine(DependencyObject obj)
        {
            return (Line)obj.GetValue(LabelLineProperty);
        }

        public static void SetLabelLine(DependencyObject obj, Line line)
        {
            obj.SetValue(LabelLineProperty, line);
        }

        public static readonly DependencyProperty LabelOffsetProperty = DependencyProperty.RegisterAttached(
            "LabelOffset",
            typeof(Point),
            typeof(PlotElement),
            new PropertyMetadata(new Point()));
        public static Point GetLabelOffset(DependencyObject obj)
        {
            return (Point)obj.GetValue(LabelOffsetProperty);
        }

        public static void SetLabelOffset(DependencyObject obj, Point offset)
        {
            obj.SetValue(LabelOffsetProperty, offset);
        }

        public static readonly DependencyProperty LabelAlignmentProperty = DependencyProperty.RegisterAttached(
            "LabelAlignment",
            typeof(LabelAlignment),
            typeof(PlotElement),
            new PropertyMetadata(LabelAlignment.Auto));
        public static LabelAlignment GetLabelAlignment(DependencyObject obj)
        {
            return (LabelAlignment)obj.GetValue(LabelAlignmentProperty);
        }

        public static void SetLabelAlignment(DependencyObject obj, LabelAlignment value)
        {
            obj.SetValue(LabelAlignmentProperty, value);
        }

        public static readonly DependencyProperty DataPointProperty = DependencyProperty.Register(
            "DataPoint",
            typeof(Dt.Charts.DataPoint),
            typeof(PlotElement),
            null);
        internal static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label",
            typeof(UIElement),
            typeof(PlotElement),
            null);

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
                    labelAlignment = pe.AutoPosition(ref hot, ref labelOffset);
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
        #endregion

        #region 成员变量
        Point _center = new Point();
        Rect _rect = new Rect();
        Size _sz = new Size(10.0, 10.0);
        Dt.Charts.TransformMode _tmode;
        protected ShapeStyle _autoStyle;
        protected List<UIElement> _effects;
        #endregion

        #region 构造方法
        internal PlotElement()
        {
            Loaded += FireLoaded;
            PointerEntered += FireMouseEnter;
            PointerExited += FireMouseLeave;
            PointerPressed += FireMouseLeftButtonDown;
            PointerReleased += FireMouseLeftButtonUp;
        }
        #endregion

        #region 事件
        public event EventHandler Created;

        public event PointerEventHandler MouseEnter;

        public event PointerEventHandler MouseLeave;

        public event PointerEventHandler MouseLeftButtonDown;

        public event PointerEventHandler MouseLeftButtonUp;
        #endregion

        #region 属性
        internal ShapeStyle AutoStyle
        {
            get { return _autoStyle; }
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
            get { return base.Style; }
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
                if (_effects == null)
                {
                    _effects = new List<UIElement>();
                }
                return _effects;
            }
        }

        protected virtual bool IsClustered
        {
            get { return false; }
        }

        internal UIElement Label
        {
            get { return (UIElement)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        internal virtual Rect LabelRect
        {
            get
            {
                // uno未实现
                return Data.Bounds;
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
        #endregion

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

        protected virtual LabelAlignment AutoPosition(ref Point hot, ref Point offset)
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
            if ((_autoStyle == null) && (ss != null))
            {
                _autoStyle = ss;
                _autoStyle.Apply(this);
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

        void FireLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= FireLoaded;
            DataSeries series = (DataPoint != null) ? DataPoint.Series : null;
            if (series != null)
            {
                series.FireLoaded(this, EventArgs.Empty);
            }
        }

        void FireMouseEnter(object sender, PointerRoutedEventArgs e)
        {
            MouseEnter?.Invoke(sender, e);
        }

        void FireMouseLeave(object sender, PointerRoutedEventArgs e)
        {
            MouseLeave?.Invoke(sender, e);
        }

        void FireMouseLeftButtonDown(object sender, PointerRoutedEventArgs e)
        {
            MouseLeftButtonDown?.Invoke(sender, e);
        }

        void FireMouseLeftButtonUp(object sender, PointerRoutedEventArgs e)
        {
            MouseLeftButtonUp?.Invoke(sender, e);
        }

        internal void FirePlotElementLoaded(object sender, RoutedEventArgs e)
        {
            DataSeries series = (DataPoint != null) ? DataPoint.Series : null;
            if (series != null)
            {
                series.FirePlotElementLoaded(this, EventArgs.Empty);
            }
        }

        protected virtual bool IsCompatible(IRenderer rend)
        {
            return (rend is Renderer2D);
        }

        protected virtual bool Render(RenderContext rc)
        {
            Width = Size.Width;
            Height = Size.Height;
            Canvas.SetLeft(this, rc.Current.X - (0.5 * Width));
            Canvas.SetTop(this, rc.Current.Y - (0.5 * Height));
            return true;
        }
    }
}

