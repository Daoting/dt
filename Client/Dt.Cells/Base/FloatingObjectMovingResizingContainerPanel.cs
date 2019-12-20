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
        private List<FloatingObjectMovingResizingFrame> _cachedFrames = new List<FloatingObjectMovingResizingFrame>();

        public FloatingObjectMovingResizingContainerPanel(GcViewport parentViewport)
        {
            this.ParentViewport = parentViewport;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            for (int i = 0; i < this._cachedFrames.Count; i++)
            {
                FloatingObjectMovingResizingFrame frame = this._cachedFrames[i];
                frame.InvalidateArrange();
                frame.Arrange(this.OperatingFrameLayouts[i]);
            }
            return base.ArrangeOverride(finalSize);
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            Windows.Foundation.Rect[] operatingFrameLayouts = this.OperatingFrameLayouts;
            if ((operatingFrameLayouts == null) || (operatingFrameLayouts.Length == 0))
            {
                this._cachedFrames.Clear();
                base.Children.Clear();
                return base.MeasureOverride(availableSize);
            }
            if (this._cachedFrames.Count > operatingFrameLayouts.Length)
            {
                int length = operatingFrameLayouts.Length;
                int num2 = this._cachedFrames.Count - operatingFrameLayouts.Length;
                this._cachedFrames.RemoveRange(length, num2);
                while (num2 > 0)
                {
                    base.Children.RemoveAt(length);
                    num2--;
                }
            }
            for (int i = 0; i < operatingFrameLayouts.Length; i++)
            {
                if (i >= this._cachedFrames.Count)
                {
                    FloatingObjectMovingResizingFrame frame = new FloatingObjectMovingResizingFrame();
                    this._cachedFrames.Add(frame);
                    base.Children.Add(frame);
                }
                this._cachedFrames[i].InvalidateMeasure();
                this._cachedFrames[i].Measure(new Windows.Foundation.Size(operatingFrameLayouts[i].Width, operatingFrameLayouts[i].Height));
            }
            return base.MeasureOverride(availableSize);
        }

        private bool IsMoving
        {
            get { return ((this.ParentViewport._cachedChartShapeMovingRects != null) && (this.ParentViewport._cachedChartShapeMovingRects.Length > 0)); }
        }

        private Windows.Foundation.Rect[] OperatingFrameLayouts
        {
            get
            {
                if (this.IsMoving)
                {
                    return this.ParentViewport._cachedChartShapeMovingRects;
                }
                return this.ParentViewport._cachedChartShapeResizingRects;
            }
        }

        public GcViewport ParentViewport { get; set; }
    }

    internal partial class FloatingObjectMovingResizingFrame : Panel
    {
        private Rectangle _frame = new Rectangle();

        public FloatingObjectMovingResizingFrame()
        {
            this._frame.StrokeThickness = 1.0;
            this._frame.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(50, 110, 110, 110));
            this._frame.Stroke = new SolidColorBrush(Colors.Black);
            base.Children.Add(this._frame);
            this.IsLeftVisibe = true;
            this.IsRightVisibe = true;
            this.IsTopVisibie = true;
            this.IsBottomVisibe = true;
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (!finalSize.IsEmpty)
            {
                double width;
                double height;
                GeometryGroup group = new GeometryGroup();
                group.FillRule = FillRule.Nonzero;
                if (this.IsLeftVisibe)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = new Windows.Foundation.Rect(0.0, 0.0, this.Thickness, finalSize.Height);
                    group.Children.Add(geometry);
                }
                if (this.IsRightVisibe)
                {
                    RectangleGeometry geometry2 = new RectangleGeometry();
                    geometry2.Rect = new Windows.Foundation.Rect(finalSize.Width - this.Thickness, 0.0, this.Thickness, finalSize.Height);
                    group.Children.Add(geometry2);
                }
                if (this.IsTopVisibie)
                {
                    RectangleGeometry geometry3 = new RectangleGeometry();
                    geometry3.Rect = new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, this.Thickness);
                    group.Children.Add(geometry3);
                }
                if (this.IsBottomVisibe)
                {
                    RectangleGeometry geometry4 = new RectangleGeometry();
                    geometry4.Rect = new Windows.Foundation.Rect(0.0, finalSize.Height - this.Thickness, finalSize.Width, this.Thickness);
                    group.Children.Add(geometry4);
                }
                double x = this.IsLeftVisibe ? this.Thickness : 0.0;
                double y = this.IsTopVisibie ? this.Thickness : 0.0;
                if (this.IsLeftVisibe && this.IsRightVisibe)
                {
                    width = finalSize.Width - (2.0 * this.Thickness);
                }
                else if (this.IsLeftVisibe || this.IsRightVisibe)
                {
                    width = finalSize.Width - this.Thickness;
                }
                else
                {
                    width = finalSize.Width;
                }
                if (this.IsTopVisibie && this.IsBottomVisibe)
                {
                    height = finalSize.Height - (2.0 * this.Thickness);
                }
                else if (this.IsTopVisibie || this.IsBottomVisibe)
                {
                    height = finalSize.Height - this.Thickness;
                }
                else
                {
                    height = finalSize.Height;
                }
                width = Math.Max(0.0, width);
                height = Math.Max(0.0, height);
                RectangleGeometry geometry5 = new RectangleGeometry();
                geometry5.Rect = new Windows.Foundation.Rect(x, y, width, height);
                group.Children.Add(geometry5);
                this._frame.Arrange(new Windows.Foundation.Rect(new Windows.Foundation.Point(0.0, 0.0), finalSize));
            }
            return finalSize;
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._frame.Measure(availableSize);
            return this._frame.DesiredSize;
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

