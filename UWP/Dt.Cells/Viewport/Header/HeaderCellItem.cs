#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal abstract partial class HeaderCellItem : Panel
    {
        Border _border;
        TextBlock _tb;

        public HeaderCellItem(HeaderItem p_rowItem)
        {
            OwnRow = p_rowItem;
            _border = new Border
            {
                BorderBrush = BrushRes.浅灰2,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Background = BrushRes.浅灰1,
            };
            Children.Add(_border);

            _tb = new TextBlock { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            Children.Add(_tb);
        }

        public HeaderItem OwnRow { get; }

        public int Row
        {
            get { return OwnRow.Row; }
        }

        public int Column { get; set; }

        public CellLayout CellLayout { get; set; }

        public void ApplyState()
        {
            if (IsMouseOver)
            {
                _border.Background = BrushRes.浅黄;
            }
            else
            {
                var excel = OwnRow.Owner.Excel;
                bool allowSelect = excel.ShowSelection && !excel.HasSelectedFloatingObject();
                if ((IsSelected && allowSelect)
                    || IsHightlighted)
                {
                    _border.Background = BrushRes.中黄;
                }
                else
                {
                    _border.Background = BrushRes.浅灰1;
                }
            }
        }

        protected abstract bool IsMouseOver { get; }

        protected abstract bool IsSelected { get; }

        protected abstract bool IsHightlighted { get; }

        #region 测量布局
        //*** HeaderPanel.Measure -> HeaderItem.UpdateChildren -> 行列改变时 HeaderCellItem.UpdateChildren -> HeaderItem.Measure -> HeaderCellItem.Measure ***//

        public void UpdateChildren()
        {
            // 刷新绑定的Cell
            int row = OwnRow.Row;
            int column = Column;
            if (CellLayout != null)
            {
                row = CellLayout.Row;
                column = CellLayout.Column;
            }
            var bindingCell = OwnRow.Owner.CellCache.GetCachedCell(row, column);
            if (bindingCell == null)
                return;

            string text = bindingCell.Text;
            if (string.IsNullOrEmpty(text))
            {
                _tb.ClearValue(TextBlock.TextProperty);
            }
            else
            {
                _tb.Text = text;
                ApplyStyle(bindingCell);
            }
            ApplyState();
        }

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

        /// <summary>
        /// 未完全应用CellItem中的样式
        /// </summary>
        /// <param name="p_cell"></param>
        void ApplyStyle(Cell p_cell)
        {
            // uno绘制Right位置错误，慎用TextAlignment！
            HorizontalAlignment horAlignment;
            switch (p_cell.ActualHorizontalAlignment)
            {
                case CellHorizontalAlignment.Center:
                    horAlignment = HorizontalAlignment.Center;
                    break;
                case CellHorizontalAlignment.Right:
                    horAlignment = HorizontalAlignment.Right;
                    break;
                default:
                    horAlignment = HorizontalAlignment.Left;
                    break;
            }
            if (_tb.HorizontalAlignment != horAlignment)
                _tb.HorizontalAlignment = horAlignment;

            VerticalAlignment verAlignment;
            switch (p_cell.ActualVerticalAlignment)
            {
                case CellVerticalAlignment.Top:
                    verAlignment = VerticalAlignment.Top;
                    break;
                case CellVerticalAlignment.Bottom:
                    verAlignment = VerticalAlignment.Bottom;
                    break;
                default:
                    verAlignment = VerticalAlignment.Center;
                    break;
            }
            if (_tb.VerticalAlignment != verAlignment)
                _tb.VerticalAlignment = verAlignment;

            var foreground = p_cell.ActualForeground;
            if (foreground == null)
            {
                // 默认黑色
                if (_tb.ReadLocalValue(TextBlock.ForegroundProperty) != DependencyProperty.UnsetValue)
                    _tb.ClearValue(TextBlock.ForegroundProperty);
            }
            else if (foreground != _tb.Foreground)
            {
                _tb.Foreground = foreground;
            }

            var fontStyle = p_cell.ActualFontStyle;
            if (_tb.FontStyle != fontStyle)
                _tb.FontStyle = fontStyle;

            var fontWeight = p_cell.ActualFontWeight;
            if (_tb.FontWeight.Weight != fontWeight.Weight)
                _tb.FontWeight = fontWeight;

            var fontFamily = p_cell.ActualFontFamily;
            if (fontFamily != null && _tb.FontFamily.Source != fontFamily.Source)
                _tb.FontFamily = fontFamily;

            double fontSize = p_cell.ActualFontSize * (double)OwnRow.Owner.Excel.ZoomFactor;
            if (_tb.FontSize != fontSize)
                _tb.FontSize = fontSize;
        }
    }
}
