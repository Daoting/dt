#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using System;
#endregion

namespace Dt.Cells.UI
{
    public abstract partial class HeaderCellItem : Panel
    {
        HeaderItem _owner;
        Cell _bindingCell;
        int _column = -1;
        Border _border;
        TextBlock _tb;
        CellLayout _cellLayout;

        public HeaderCellItem()
        {
            _border = new Border
            {
                BorderBrush = BrushRes.浅灰边框,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Background = BrushRes.浅灰背景,
            };
            Children.Add(_border);

            _tb = new TextBlock { TextAlignment = Windows.UI.Xaml.TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Children.Add(_tb);
        }

        internal HeaderItem Owner
        {
            get { return _owner; }
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    if (_owner == null)
                    {
                        _bindingCell = null;
                    }
                    else
                    {
                        int row = _owner.Row;
                        int column = _column;
                        if (_cellLayout != null)
                        {
                            row = _cellLayout.Row;
                            column = _cellLayout.Column;
                        }
                        _bindingCell = _owner.Owner.CellCache.GetCachedCell(row, column);
                    }
                    Invalidate();
                }
            }
        }

        internal CellLayout CellLayout
        {
            get { return _cellLayout; }
            set
            {
                if (_cellLayout != value)
                {
                    _cellLayout = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates the row index.
        /// </summary>
        public int Row
        {
            get { return _owner.Row; }
        }

        public int Column
        {
            get { return _column; }
            internal set
            {
                if (value != _column)
                {
                    _column = value;
                    Invalidate();
                }
            }
        }

        public void ApplyState()
        {
            if (IsMouseOver)
            {
                _border.Background = BrushRes.浅黄背景;
            }
            else
            {
                SheetView sheet = Owner.Owner.Sheet;
                bool allowSelect = !sheet.HideSelectionWhenPrinting && !sheet.HasSelectedFloatingObject();
                if ((IsSelected && allowSelect)
                    || IsHightlighted)
                {
                    _border.Background = BrushRes.黄色背景;
                }
                else
                {
                    _border.Background = BrushRes.浅灰背景;
                }
            }
        }

        protected abstract bool IsMouseOver { get; }

        protected abstract bool IsSelected { get; }

        protected abstract bool IsHightlighted { get; }

        
        internal void Invalidate()
        {
            if (_bindingCell == null)
                return;

            string text = _bindingCell.Text;
            if (string.IsNullOrEmpty(text))
            {
                _tb.ClearValue(TextBlock.TextProperty);
            }
            else
            {
                _tb.Text = text;
                ApplyStyle();
            }
            ApplyState();
        }

        void ApplyStyle()
        {

        }

        #region 测量布局
        protected override Size MeasureOverride(Size availableSize)
        {
            _border.Measure(availableSize);
            _tb.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            Rect rc = new Rect(new Point(), finalSize);
            _border.Arrange(rc);
            _tb.Arrange(rc);
            return finalSize;
        }
        #endregion
    }
}
