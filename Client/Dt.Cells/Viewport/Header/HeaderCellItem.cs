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
                BorderBrush = BrushRes.浅灰边框,
                BorderThickness = new Thickness(0, 0, 1, 1),
                Background = BrushRes.浅灰背景,
            };
            Children.Add(_border);

            _tb = new TextBlock { TextAlignment = Windows.UI.Xaml.TextAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
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
                _border.Background = BrushRes.浅黄背景;
            }
            else
            {
                SheetView sheet = OwnRow.Owner.Sheet;
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

        void ApplyStyle()
        {

        }

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
                ApplyStyle();
            }
            ApplyState();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Column == -1
                || availableSize.Width == 0.0
                || availableSize.Height == 0.0)
                return new Size();

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
