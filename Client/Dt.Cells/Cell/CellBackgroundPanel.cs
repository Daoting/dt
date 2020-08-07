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
        Thickness _contentPadding;
        CellPresenterBase _owner;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
            {
                return new Size();
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

            Size size = new Size(width, height);
            Size size2 = size;
            if (ContentWidth > width)
            {
                size2 = new Size(ContentWidth, height);
            }

            foreach (UIElement element in Children)
            {
                if (element is TextBlock)
                {
                    element.Measure(size2);
                }
                else
                {
                    element.Measure(size);
                }
            }
            return size;
        }

        /// <summary>
        /// Arranges and sizes the panel content.
        /// </summary>
        /// <param name="finalSize">The computed size that is used to arrange the content.</param>
        /// <returns> The size of the panel.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (double.IsInfinity(finalSize.Width) || double.IsInfinity(finalSize.Height))
            {
                return new Size();
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

            Rect? nullable = null;
            double left = ContentPadding.Left;
            double top = ContentPadding.Top;
            Rect rect = new Rect(left, top, width, height);
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
                                nullable = new Rect(0.0, 0.0, num5, finalSize.Height);
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
                                nullable = new Rect(x, 0.0, num7, finalSize.Height);
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
                                nullable = new Rect(num8, 0.0, num9, finalSize.Height);
                            }
                        }
                        break;
                }
                width = ContentWidth;
            }

            Rect rect2 = new Rect(left, top, width, height);
            Rect? nullable2 = _cachedClip;
            Rect? nullable3 = nullable;
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

        /// <summary>
        /// Gets or sets the content padding in the cell panel.
        /// </summary>
        public Thickness ContentPadding
        {
            get { return  _contentPadding; }
            set
            {
                if (_contentPadding != value)
                {
                    _contentPadding = value;
                    InvalidateMeasure();
                    InvalidateArrange();
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

