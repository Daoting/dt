#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents a cell panel to display its content, it works like a grid,
    /// and mainly to process the cell content to overflow.
    /// </summary>
    public partial class CellBackgroundPanel : Panel
    {
        Windows.Foundation.Rect? _cachedClip;
        Windows.UI.Xaml.Thickness _contentPadding;
        CellPresenterBase _owner;

        /// <summary>
        /// Arranges and sizes the panel content.
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange the content.</param>
        /// <returns> The size of the panel.</returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (double.IsInfinity(finalSize.Width) || double.IsInfinity(finalSize.Height))
            {
                return new Windows.Foundation.Size();
            }
            double width = finalSize.Width - (ContentPadding.Left + ContentPadding.Right);
            if (width < 0.0)
            {
                width = 0.0;
            }
            double height = finalSize.Height - (ContentPadding.Top + ContentPadding.Bottom);
            if (height < 0.0)
            {
                height = 0.0;
            }
            Windows.Foundation.Rect? nullable = null;
            double left = ContentPadding.Left;
            double top = ContentPadding.Top;
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(left, top, width, height);
            if (ContentWidth > width)
            {
                switch (HorizontalContentFlowDirection)
                {
                    case HorizontalAlignment.Left:
                        if (CellOverflowLayout != null)
                        {
                            double num5 = CellOverflowLayout.RightBackgroundWidth - ContentPadding.Right;
                            if (num5 >= 0.0)
                            {
                                nullable = new Windows.Foundation.Rect(0.0, 0.0, num5, finalSize.Height);
                            }
                        }
                        break;

                    case HorizontalAlignment.Right:
                        left -= ContentWidth - width;
                        if (CellOverflowLayout != null)
                        {
                            double x = finalSize.Width - CellOverflowLayout.LeftBackgroundWidth;
                            double num7 = CellOverflowLayout.LeftBackgroundWidth - ContentPadding.Left;
                            if (num7 >= 0.0)
                            {
                                nullable = new Windows.Foundation.Rect(x, 0.0, num7, finalSize.Height);
                            }
                        }
                        break;

                    default:
                        left -= (ContentWidth - width) / 2.0;
                        if (CellOverflowLayout != null)
                        {
                            double num8 = 0.0;
                            if (CellOverflowLayout.LeftBackgroundWidth > 0.0)
                            {
                                num8 = ((finalSize.Width / 2.0) - CellOverflowLayout.LeftBackgroundWidth) + ContentPadding.Left;
                            }
                            double num9 = CellOverflowLayout.BackgroundWidth - ContentPadding.Right;
                            if (num9 >= 0.0)
                            {
                                nullable = new Windows.Foundation.Rect(num8, 0.0, num9, finalSize.Height);
                            }
                        }
                        break;
                }
                width = ContentWidth;
            }
            Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(left, top, width, height);
            Windows.Foundation.Rect? nullable2 = _cachedClip;
            Windows.Foundation.Rect? nullable3 = nullable;
            if ((nullable2.HasValue != nullable3.HasValue) || (nullable2.HasValue && (nullable2.GetValueOrDefault() != nullable3.GetValueOrDefault())))
            {
                _cachedClip = nullable;
                if (nullable.HasValue)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = nullable.Value;
                    base.Clip = geometry;
                }
                else
                {
                    base.ClearValue(UIElement.ClipProperty);
                }
            }
            foreach (UIElement element in base.Children)
            {
                if (element != null)
                {
                    if (element is TextBlock)
                    {
                        element.Arrange(rect2);
                    }
                    else
                    {
                        element.Arrange(rect);
                    }
                }
            }
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        /// <summary>
        /// Called to re-measure a control.
        /// </summary>
        /// <param name="availableSize">The maximum size that the method can return.</param>
        /// <returns>The size of the control, up to the maximum specified by the constraint.</returns>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return new Windows.Foundation.Size();
            }
            double width = availableSize.Width - (ContentPadding.Left + ContentPadding.Right);
            if (width < 0.0)
            {
                width = 0.0;
            }
            double height = availableSize.Height - (ContentPadding.Top + ContentPadding.Bottom);
            if (height < 0.0)
            {
                height = 0.0;
            }
            Windows.Foundation.Size size = new Windows.Foundation.Size(width, height);
            Windows.Foundation.Size size2 = size;
            if (ContentWidth > width)
            {
                size2 = new Windows.Foundation.Size(ContentWidth, height);
            }
            foreach (UIElement element in base.Children)
            {
                if (element != null)
                {
                    if (element is TextBlock)
                    {
                        element.Measure(size2);
                    }
                    else
                    {
                        element.InvalidateMeasure();
                        element.Measure(size);
                    }
                }
            }
            base.MeasureOverride(availableSize);
            return size;
        }

        Dt.Cells.UI.CellOverflowLayout CellOverflowLayout
        {
            get
            {
                if (OwneringCell != null)
                {
                    return OwneringCell.CellOverflowLayout;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the content padding in the cell panel.
        /// </summary>
        public Windows.UI.Xaml.Thickness ContentPadding
        {
            get { return  _contentPadding; }
            set
            {
                if (_contentPadding != value)
                {
                    _contentPadding = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }

        double ContentWidth
        {
            get
            {
                if (CellOverflowLayout != null)
                {
                    return CellOverflowLayout.ContentWidth;
                }
                return 0.0;
            }
        }

        HorizontalAlignment HorizontalContentFlowDirection
        {
            get
            {
                if (OwneringCell != null)
                {
                    Cell bindingCell = OwneringCell.BindingCell;
                    if (bindingCell != null)
                    {
                        return bindingCell.ToHorizontalAlignment();
                    }
                }
                return HorizontalAlignment.Left;
            }
        }

        internal CellPresenterBase OwneringCell
        {
            get { return  _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    base.InvalidateMeasure();
                    base.InvalidateArrange();
                }
            }
        }
    }
}

