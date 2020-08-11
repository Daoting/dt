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
        Rect? _cachedClip;
        CellItemBase _owner;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
                return new Size();

            Size sizeOverflow = availableSize;
            if (ContentWidth > availableSize.Width)
                sizeOverflow = new Size(ContentWidth, availableSize.Height);

            foreach (UIElement element in Children)
            {
                if (element is TextBlock)
                {
                    element.Measure(sizeOverflow);
                }
                else
                {
                    element.Measure(availableSize);
                }
            }
            return availableSize;
        }

        /// <summary>
        /// Arranges and sizes the panel content.
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange the content.</param>
        /// <returns> The size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (double.IsInfinity(finalSize.Width) || double.IsInfinity(finalSize.Height))
                return new Size();

            double width = finalSize.Width;
            double height = finalSize.Height;
            Rect? nullable = null;
            double left = 0;
            double top = 0;
            Rect rect = new Rect(left, top, width, height);
            Rect rectOverflow = rect;
            if (ContentWidth > width)
            {
                switch (HorizontalContentFlowDirection)
                {
                    case HorizontalAlignment.Left:
                        if (CellOverflowLayout != null)
                        {
                            double w = CellOverflowLayout.RightBackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(0.0, 0.0, w, finalSize.Height);
                        }
                        break;

                    case HorizontalAlignment.Right:
                        left -= ContentWidth - width;
                        if (CellOverflowLayout != null)
                        {
                            double x = finalSize.Width - CellOverflowLayout.LeftBackgroundWidth;
                            double w = CellOverflowLayout.LeftBackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(x, 0.0, w, finalSize.Height);
                        }
                        break;

                    default:
                        left -= (ContentWidth - width) / 2.0;
                        if (CellOverflowLayout != null)
                        {
                            double x = 0.0;
                            if (CellOverflowLayout.LeftBackgroundWidth > 0.0)
                                x = (finalSize.Width / 2.0) - CellOverflowLayout.LeftBackgroundWidth;

                            double w = CellOverflowLayout.BackgroundWidth;
                            if (w >= 0.0)
                                nullable = new Rect(x, 0.0, w, finalSize.Height);
                        }
                        break;
                }
                width = ContentWidth;
                rectOverflow = new Rect(left, top, width, height);
            }

            if ((_cachedClip.HasValue != nullable.HasValue) || (_cachedClip.HasValue && (_cachedClip.Value != nullable.Value)))
            {
                _cachedClip = nullable;
                if (nullable.HasValue)
                {
                    RectangleGeometry geometry = new RectangleGeometry();
                    geometry.Rect = nullable.Value;
                    Clip = geometry;
                }
                else
                {
                    ClearValue(ClipProperty);
                }
            }

            foreach (UIElement element in base.Children)
            {
                if (element != null)
                {
                    if (element is TextBlock)
                    {
                        element.Arrange(rectOverflow);
                    }
                    else
                    {
                        element.Arrange(rect);
                    }
                }
            }
            return finalSize;
        }

        CellOverflowLayout CellOverflowLayout
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

        internal CellItemBase OwneringCell
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

