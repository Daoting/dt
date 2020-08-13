#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-05 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// 按行布局的面板
    /// </summary>
    internal partial class RowsLayer : Panel
    {
        const int _normalZIndexBase = 10000;
        const int _spanRowZIndexBase = 20000;
        readonly CellsPanel _owner;
        readonly Dictionary<int, RowItem> _rows;
        readonly List<RowItem> _recycledRows;

        public RowsLayer(CellsPanel p_owner)
        {
            _owner = p_owner;
            _rows = new Dictionary<int, RowItem>();
            _recycledRows = new List<RowItem>();
        }

        public IEnumerable<RowItem> Rows
        {
            get { return _rows.Values; }
        }

        public RowItem GetRow(int row)
        {
            if (_rows.TryGetValue(row, out var item))
                return item;
            return null;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (_owner.SupportCellOverflow && _owner.Sheet.Excel.CanCellOverflow)
            {
                int viewportLeftColumn = _owner.Sheet.GetViewportLeftColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportLeftColumn = viewportLeftColumn;
                int viewportRightColumn = _owner.Sheet.GetViewportRightColumn(_owner.ColumnViewportIndex);
                _owner.CellOverflowLayoutBuildEngine.ViewportRightColumn = viewportRightColumn;
            }

            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
            List<RowItem> rows = _rows.Values.ToList();
            foreach (var rowItem in rows)
            {
                // 不可见的行回收或丢弃
                RowLayout layout = rowLayoutModel.FindRow(rowItem.Row);
                if (layout == null || layout.Height <= 0.0)
                    RecycleOrRemoveRow(rowItem);
            }

            double x = _owner.Location.X;
            double y = _owner.Location.Y;
            double left = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                RowItem rowItem = null;
                if (layout.Height <= 0.0)
                {
                    // 不可见的行回收或丢弃
                    if (_rows.TryGetValue(layout.Row, out rowItem))
                        RecycleOrRemoveRow(rowItem);
                    continue;
                }

                if (!_rows.TryGetValue(layout.Row, out rowItem))
                {
                    // 重新利用回收行或新创建
                    rowItem = GetRecycledRow();
                    if (rowItem == null)
                        rowItem = new RowItem(_owner);
                    rowItem.Row = layout.Row;

                    Children.Add(rowItem);
                    _rows.Add(layout.Row, rowItem);
                }

                int z = rowItem.ContainsSpanCell ? _spanRowZIndexBase + rowItem.Row : _normalZIndexBase + rowItem.Row;
                z = z % 0x7ffe;
                Canvas.SetZIndex(rowItem, z);

                rowItem.Location = new Point(x, y);
                rowItem.Measure(new Size(availableSize.Width, layout.Height));
                y += layout.Height;
                left = Math.Max(left, rowItem.DesiredSize.Width);
            }
            return new Size(left + _owner.Location.X, y);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            RowLayoutModel rowLayoutModel = _owner.GetRowLayoutModel();
            double y = 0.0;
            double rowWidth = 0.0;
            foreach (RowLayout layout in rowLayoutModel)
            {
                if (layout.Height <= 0.0)
                    continue;

                if (_rows.TryGetValue(layout.Row, out var rowItem))
                {
                    rowItem.Arrange(new Rect(0.0, y, finalSize.Width, layout.Height));
                    if (rowWidth == 0.0)
                        rowWidth = rowItem.RowWidth;
                }
                y += layout.Height;
            }
            rowWidth = Math.Min(_owner.GetViewportSize().Width, rowWidth);
            Size size = new Size(rowWidth, y);
            Clip = new RectangleGeometry { Rect = new Rect(new Point(), size) };
            return size;
        }

        void RecycleOrRemoveRow(RowItem p_rowItem)
        {
            Children.Remove(p_rowItem);
            _rows.Remove(p_rowItem.Row);
            p_rowItem.CleanUpBeforeDiscard();

            if (p_rowItem.IsRecyclable)
            {
                p_rowItem.Row = -1;
                _recycledRows.Add(p_rowItem);
            }
        }

        RowItem GetRecycledRow()
        {
            RowItem rowItem = null;
            if (_recycledRows.Count > 0)
            {
                rowItem = _recycledRows[0];
                _recycledRows.RemoveAt(0);
            }
            return rowItem;
        }
    }
}

