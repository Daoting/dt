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
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.UI
{
    internal partial class HeaderItem : Panel
    {
        int _row;
        double _rowWidth;
        readonly List<HeaderCellItem> _recycledCells;

        public HeaderItem(HeaderPanel p_owner)
        {
            Owner = p_owner;
            Cells = new Dictionary<int, HeaderCellItem>();
            _recycledCells = new List<HeaderCellItem>();

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
        }

        public HeaderPanel Owner { get; }

        public Dictionary<int, HeaderCellItem> Cells { get; }

        public int Row
        {
            get { return _row; }
            set
            {
                if (_row != value)
                {
                    _row = value;
                    InvalidateMeasure();
                }
            }
        }

        internal bool CellsDirty { get; set; }

        internal double RowWidth
        {
            get { return _rowWidth; }
        }

        public Point Location { get; set; }

        internal bool ContainsSpanCell { get; set; }

        public HeaderCellItem GetCell(int column)
        {
            if (Cells.TryGetValue(column, out var cell))
                return cell;
            return null;
        }

        ColumnLayoutModel GetColumnLayoutModel()
        {
            return (Owner.Area == SheetArea.ColumnHeader) ?
                Owner.Sheet.GetColumnHeaderViewportColumnLayoutModel(Owner.ColumnViewportIndex) : Owner.Sheet.GetRowHeaderColumnLayoutModel();
        }

        public void UpdateDisplayedCells(bool forceUpdate = false)
        {
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            List<HeaderCellItem> lsRecy = new List<HeaderCellItem>();

            foreach (KeyValuePair<int, HeaderCellItem> pair in Cells)
            {
                HeaderCellItem cell = GetCell(pair.Key);
                ColumnLayout layout = columnLayoutModel.FindColumn(cell.Column);
                if ((layout == null) || (layout.Width == 0.0))
                {
                    lsRecy.Add(cell);
                }
            }

            SpanGraph cachedSpanGraph = Owner.CachedSpanGraph;
            ContainsSpanCell = false;
            for (int i = 0; i < columnLayoutModel.Count; i++)
            {
                ColumnLayout colLayout = columnLayoutModel[i];
                if (colLayout.Width <= 0.0)
                    continue;

                int column = colLayout.Column;
                HeaderCellItem cell = GetCell(column);

                byte state = cachedSpanGraph.GetState(Row, colLayout.Column);
                CellLayout layout = null;
                if (state > 0)
                {
                    CellLayoutModel cellLayoutModel = Owner.GetCellLayoutModel();
                    if (cellLayoutModel != null)
                    {
                        layout = cellLayoutModel.FindCell(Row, colLayout.Column);
                    }
                }

                bool flag = false;
                if (layout != null && layout.Width > 0.0 && layout.Height > 0.0)
                {
                    CellRange range = ClipCellRange(layout.GetCellRange());
                    flag = (Row != range.Row) || (range.Column != colLayout.Column);
                    if (layout.ColumnCount > 1)
                    {
                        int num4 = (layout.Column + layout.ColumnCount) - 1;
                        for (int j = i + 1; j < columnLayoutModel.Count; j++)
                        {
                            int num6 = columnLayoutModel[j].Column;
                            if (num6 > num4)
                                break;

                            i = j;
                            HeaderCellItem base5 = GetCell(num6);
                            if ((base5 != null) && Cells.Remove(num6))
                            {
                                Children.Remove(base5);
                            }
                        }
                    }
                }

                if (flag)
                {
                    if (cell != null)
                    {
                        Cells.Remove(cell.Column);
                        Children.Remove(cell);
                    }
                }
                else
                {
                    bool updated = false;
                    if (cell == null)
                    {
                        if ((lsRecy.Count > 0) || (_recycledCells.Count > 0))
                        {
                            if (lsRecy.Count > 0)
                            {
                                int num7 = lsRecy.Count - 1;
                                cell = lsRecy[num7];
                                lsRecy.RemoveAt(num7);
                            }
                            else
                            {
                                int num8 = _recycledCells.Count - 1;
                                cell = _recycledCells[num8];
                                _recycledCells.RemoveAt(num8);
                            }
                            Cells.Remove(cell.Column);
                            cell.Column = colLayout.Column;
                        }
                        else
                        {
                            cell = (Owner.Area == SheetArea.ColumnHeader) ? (HeaderCellItem)new ColHeaderCell() : new RowHeaderCell();
                            cell.Column = colLayout.Column;
                        }

                        if (cell.Parent != this)
                        {
                            Children.Add(cell);
                        }
                        Cells.Add(cell.Column, cell);
                        updated = true;
                    }

                    cell.CellLayout = layout;
                    if (layout != null)
                    {
                        ContainsSpanCell = true;
                    }
                    cell.Owner = this;
                    if (CellsDirty || forceUpdate || updated)
                    {
                        cell.Invalidate();
                        InvalidateMeasure();
                    }
                }
            }

            CellsDirty = false;
            foreach (var item in lsRecy)
            {
                Cells.Remove(item.Column);
                Children.Remove(item);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (double.IsInfinity(availableSize.Width) || double.IsInfinity(availableSize.Height))
                return Size.Empty;

            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            RowLayout layout = Owner.GetRowLayoutModel().FindRow(Row);
            _rowWidth = 0.0;
            foreach (ColumnLayout colLayout in columnLayoutModel)
            {
                HeaderCellItem cell = GetCell(colLayout.Column);
                if (cell != null)
                {
                    CellLayout cellLayout = cell.CellLayout;
                    if (cellLayout != null)
                    {
                        cell.Measure(new Size(cellLayout.Width, cellLayout.Height));
                    }
                    else
                    {
                        cell.Measure(new Size(colLayout.Width, layout.Height));
                    }
                }
                _rowWidth += colLayout.Width;
            }

            CellsDirty = false;
            double width = Math.Min(_rowWidth, Owner.GetViewportSize().Width);
            return new Size(width, layout.Height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ColumnLayoutModel columnLayoutModel = GetColumnLayoutModel();
            RowLayout layout = Owner.GetRowLayoutModel().FindRow(Row);
            foreach (ColumnLayout colLayout in columnLayoutModel)
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
            return finalSize;
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

