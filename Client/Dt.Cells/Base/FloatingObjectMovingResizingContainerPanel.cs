#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal partial class FloatingObjectMovingResizingContainerPanel : Panel
    {
        List<FloatingObjectMovingResizingFrame> _cachedFrames = new List<FloatingObjectMovingResizingFrame>();

        public FloatingObjectMovingResizingContainerPanel(GcViewport parentViewport)
        {
            ParentViewport = parentViewport;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = 0; i < _cachedFrames.Count; i++)
            {
                FloatingObjectMovingResizingFrame frame = _cachedFrames[i];
                frame.InvalidateArrange();
                frame.Arrange(OperatingFrameLayouts[i]);
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Rect[] operatingFrameLayouts = OperatingFrameLayouts;
            if ((operatingFrameLayouts == null) || (operatingFrameLayouts.Length == 0))
            {
                _cachedFrames.Clear();
                base.Children.Clear();
                return base.MeasureOverride(availableSize);
            }
            if (_cachedFrames.Count > operatingFrameLayouts.Length)
            {
                int length = operatingFrameLayouts.Length;
                int num2 = _cachedFrames.Count - operatingFrameLayouts.Length;
                _cachedFrames.RemoveRange(length, num2);
                while (num2 > 0)
                {
                    base.Children.RemoveAt(length);
                    num2--;
                }
            }
            for (int i = 0; i < operatingFrameLayouts.Length; i++)
            {
                if (i >= _cachedFrames.Count)
                {
                    FloatingObjectMovingResizingFrame frame = new FloatingObjectMovingResizingFrame();
                    _cachedFrames.Add(frame);
                    base.Children.Add(frame);
                }
                _cachedFrames[i].InvalidateMeasure();
                _cachedFrames[i].Measure(new Size(operatingFrameLayouts[i].Width, operatingFrameLayouts[i].Height));
            }
            return base.MeasureOverride(availableSize);
        }

        bool IsMoving
        {
            get { return ((ParentViewport._cachedChartShapeMovingRects != null) && (ParentViewport._cachedChartShapeMovingRects.Length > 0)); }
        }

        Rect[] OperatingFrameLayouts
        {
            get
            {
                if (IsMoving)
                {
                    return ParentViewport._cachedChartShapeMovingRects;
                }
                return ParentViewport._cachedChartShapeResizingRects;
            }
        }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class FloatingObjectMovingResizingFrame : Panel
    {
        Rectangle _frame = new Rectangle();

        public FloatingObjectMovingResizingFrame()
        {
            _frame.StrokeThickness = 1.0;
            _frame.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(50, 110, 110, 110));
            _frame.Stroke = new SolidColorBrush(Colors.Black);
            base.Children.Add(_frame);
            IsLeftVisibe = true;
            IsRightVisibe = true;
            IsTopVisibie = true;
            IsBottomVisibe = true;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                double width;
                double height;
                GeometryGroup group = new GeometryGroup();
                group.FillRule = FillRule.Nonzero;
                if (IsLeftVisibe)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = new Rect(0.0, 0.0, Thickness, finalSize.Height);
                    group.Children.Add(geometry);
                }
                if (IsRightVisibe)
                {
                    RectangleGeometry geometry2 = new RectangleGeometry();
                    geometry2.Rect = new Rect(finalSize.Width - Thickness, 0.0, Thickness, finalSize.Height);
                    group.Children.Add(geometry2);
                }
                if (IsTopVisibie)
                {
                    RectangleGeometry geometry3 = new RectangleGeometry();
                    geometry3.Rect = new Rect(0.0, 0.0, finalSize.Width, Thickness);
                    group.Children.Add(geometry3);
                }
                if (IsBottomVisibe)
                {
                    RectangleGeometry geometry4 = new RectangleGeometry();
                    geometry4.Rect = new Rect(0.0, finalSize.Height - Thickness, finalSize.Width, Thickness);
                    group.Children.Add(geometry4);
                }
                double x = IsLeftVisibe ? Thickness : 0.0;
                double y = IsTopVisibie ? Thickness : 0.0;
                if (IsLeftVisibe && IsRightVisibe)
                {
                    width = finalSize.Width - (2.0 * Thickness);
                }
                else if (IsLeftVisibe || IsRightVisibe)
                {
                    width = finalSize.Width - Thickness;
                }
                else
                {
                    width = finalSize.Width;
                }
                if (IsTopVisibie && IsBottomVisibe)
                {
                    height = finalSize.Height - (2.0 * Thickness);
                }
                else if (IsTopVisibie || IsBottomVisibe)
                {
                    height = finalSize.Height - Thickness;
                }
                else
                {
                    height = finalSize.Height;
                }
                width = Math.Max(0.0, width);
                height = Math.Max(0.0, height);
                RectangleGeometry geometry5 = new RectangleGeometry();
                geometry5.Rect = new Rect(x, y, width, height);
                group.Children.Add(geometry5);
                _frame.Arrange(new Rect(new Point(0.0, 0.0), finalSize));
            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            _frame.Measure(availableSize);
            return _frame.DesiredSize;
        }

        public bool IsBottomVisibe { get; set; }

        public bool IsLeftVisibe { get; set; }

        public bool IsRightVisibe { get; set; }

        public bool IsTopVisibie { get; set; }

        public double Thickness
        {
            get { return 3.0; }
        }
    }

}

