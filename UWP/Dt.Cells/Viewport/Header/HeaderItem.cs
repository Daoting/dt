#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-08-12 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Cells.UI
{
    internal partial class HeaderItem : Panel
    {
        static Rect _rcEmpty = new Rect();
        static Size _szEmpty = new Size();
        readonly List<HeaderCellItem> _recycledCells;

        public HeaderItem(HeaderPanel p_owner)
        {
            Owner = p_owner;
            Cells = new Dictionary<int, HeaderCellItem>();
            _recycledCells = new List<HeaderCellItem>();
            Row = -1;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public HeaderPanel Owner { get; }

        public Dictionary<int, HeaderCellItem> Cells { get; }

        public int Row { get; set; }

        public double RowWidth { get; set; }

        public Point Location { get; set; }

        public bool ContainsSpanCell { get; private set; }

        public HeaderCellItem GetCell(int column)
        {
            if (Cells.TryGetValue(column, out var cell))
                return cell;
            return null;
        }

        ColumnLayoutModel GetColumnLayoutModel()
        {
            return (Owner.Area == SheetArea.ColumnHeader) ?
                Owner.Excel.GetColumnHeaderViewportColumnLayoutModel(Owner.ColumnViewportIndex) : Owner.Excel.GetRowHeaderColumnLayoutModel();
        }

        #region 测量布局
        //*** HeaderPanel.Measure -> HeaderItem.UpdateChildren -> 行列改变时 HeaderCellItem.UpdateChildren -> HeaderItem.Measure -> HeaderCellItem.Measure ***//

        public void UpdateChildren(bool p_updateAllCell)
        {
            // 频繁增删Children子元素会出现卡顿现象！
            // Children = Cells + _recycledCells
            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            int less = colLayoutModel.Count - Children.Count;
            if (less > 0)
            {
                for (int i = 0; i < less; i++)
                {
                    var cell = (Owner.Area == SheetArea.ColumnHeader) ?
                        (HeaderCellItem)new ColHeaderCell(this) : new RowHeaderCell(this);
                    Children.Add(cell);
                    _recycledCells.Add(cell);
                }
            }

            // 先回收不可见格
            var cols = Cells.Values.ToList();
            foreach (var cell in cols)
            {
                ColumnLayout layout = colLayoutModel.FindColumn(cell.Column);
                if (layout == null || layout.Width <= 0.0)
                {
                    PushRecycledCell(cell);
                }
            }

            int updateCells = 0;
            ContainsSpanCell = false;
            SpanGraph cachedSpanGraph = Owner.CachedSpanGraph;
            for (int i = 0; i < colLayoutModel.Count; i++)
            {
                ColumnLayout colLayout = colLayoutModel[i];
                if (colLayout.Width <= 0.0)
                    continue;

                byte state = cachedSpanGraph.GetState(Row, colLayout.Column);
                CellLayout layout = null;
                if (state > 0)
                {
                    CellLayoutModel cellLayoutModel = Owner.GetCellLayoutModel();
                    if (cellLayoutModel != null)
                    {
                        layout = cellLayoutModel.FindCell(Row, colLayout.Column);
                        if (layout != null)
                            ContainsSpanCell = true;
                    }
                }

                HeaderCellItem cell = GetCell(colLayout.Column);
                bool rangeChanged = false;
                if (layout != null && layout.Width > 0.0 && layout.Height > 0.0)
                {
                    CellRange range = ClipCellRange(layout.GetCellRange());
                    rangeChanged = (Row != range.Row) || (range.Column != colLayout.Column);

                    // 跨多列
                    if (layout.ColumnCount > 1)
                    {
                        // 移除跨度区域内的所有格
                        int maxCol = (layout.Column + layout.ColumnCount) - 1;
                        for (int j = i + 1; j < colLayoutModel.Count; j++)
                        {
                            int curCol = colLayoutModel[j].Column;
                            if (curCol > maxCol)
                                break;

                            i = j;
                            var ci = GetCell(curCol);
                            if (ci != null)
                                PushRecycledCell(ci);
                        }
                    }
                }

                // 跨度不同不显示
                if (rangeChanged)
                {
                    if (cell != null)
                        PushRecycledCell(cell);
                    continue;
                }

                bool updated = false;
                if (cell == null)
                {
                    // 重新利用回收的格
                    cell = _recycledCells[0];
                    _recycledCells.RemoveAt(0);
                    cell.Column = colLayout.Column;
                    Cells.Add(colLayout.Column, cell);
                    updated = true;
                }
                cell.CellLayout = layout;

                if (p_updateAllCell || updated)
                {
                    cell.UpdateChildren();
                    updateCells++;
                }
            }

#if !IOS
            // iOS在 MeasureOverride 内部调用子元素的 InvalidateMeasure 会造成死循环！
            // 不重新测量会造成如：迷你图忽大忽小的情况
            if (updateCells > 0)
                InvalidateMeasure();
#endif
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Row == -1
                || availableSize.Width == 0.0
                || availableSize.Height == 0.0)
                return new Size();

            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = Owner.GetRowLayoutModel().FindRow(Row);
            RowWidth = 0.0;
            double height = 0.0;

            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                RowWidth += colLayout.Width;
                HeaderCellItem cell = GetCell(colLayout.Column);
                if (cell == null)
                    continue;

                CellLayout cellLayout = cell.CellLayout;
                if (cellLayout != null)
                {
                    cell.Measure(new Size(cellLayout.Width, cellLayout.Height));
                    if (cellLayout.Height > height)
                        height = cellLayout.Height;
                }
                else
                {
                    cell.Measure(new Size(colLayout.Width, layout.Height));
                    if (layout.Height > height)
                        height = layout.Height;
                }
            }

            if (_recycledCells.Count > 0)
            {
                foreach (var cell in _recycledCells)
                {
                    cell.Measure(_szEmpty);
                }
            }

            double width = Math.Min(RowWidth, Owner.GetViewportSize().Width);
            return new Size(width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Row == -1 || finalSize.Width == 0 || finalSize.Height == 0)
            {
                if (Children.Count > 0)
                {
                    foreach (UIElement elem in Children)
                    {
                        elem.Arrange(_rcEmpty);
                    }
                }
                return finalSize;
            }

            ColumnLayoutModel colLayoutModel = GetColumnLayoutModel();
            RowLayout layout = Owner.GetRowLayoutModel().FindRow(Row);
            foreach (ColumnLayout colLayout in colLayoutModel)
            {
                if (colLayout.Width <= 0.0)
                    continue;

                HeaderCellItem cell = GetCell(colLayout.Column);
                if (cell != null)
                {
                    double x = colLayout.X;
                    double y = layout.Y;
                    double width = colLayout.Width;
                    double height = layout.Height;
                    int num5 = 0x2710 + colLayout.Column;

                    CellLayout cellLayout = cell.CellLayout;
                    if (cellLayout != null)
                    {
                        x = cellLayout.X;
                        y = cellLayout.Y;
                        width = cellLayout.Width;
                        height = cellLayout.Height;
                        num5 = 0x4e20 + colLayout.Column;
                    }
                    num5 = num5 % 0x7ffe;
                    Canvas.SetZIndex(cell, num5);
                    cell.Arrange(new Rect(x - Location.X, y - Location.Y, width, height));
                }
            }

            if (_recycledCells.Count > 0)
            {
                foreach (var cell in _recycledCells)
                {
                    cell.Arrange(_rcEmpty);
                }
            }
            return finalSize;
        }
        #endregion

        void PushRecycledCell(HeaderCellItem cell)
        {
            _recycledCells.Add(cell);
            Cells.Remove(cell.Column);
            cell.Column = -1;
        }

        CellRange ClipCellRange(CellRange source)
        {
            RowLayoutModel rowLayoutModel = Owner.GetRowLayoutModel();
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            ICellsSupport dataContext = Owner.GetDataContext();
            int row = Math.Max(rowLayoutModel[0].Row, source.Row);
            int num2 = Math.Min(rowLayoutModel[rowLayoutModel.Count - 1].Row, (source.Row + source.RowCount) - 1);
            int column = Math.Max(columnLayoutModel[0].Column, source.Column);
            int num4 = Math.Min(columnLayoutModel[columnLayoutModel.Count - 1].Column, (source.Column + source.ColumnCount) - 1);
            for (int i = row; i <= num2; i++)
            {
                if (dataContext.Rows[i].ActualHeight > 0.0)
                {
                    row = i;
                    break;
                }
            }
            for (int j = column; j <= num4; j++)
            {
                if (dataContext.Columns[j].ActualWidth > 0.0)
                {
                    column = j;
                    break;
                }
            }
            return new CellRange(row, column, (num2 - row) + 1, (num4 - column) + 1);
        }
    }
}

