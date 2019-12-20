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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
    internal sealed partial class GcBorders : Panel
    {
        private Panel _borderLinesPanel = new Canvas();
        private Dictionary<ulong, Windows.Foundation.Rect> _cellBoundsCache = new Dictionary<ulong, Windows.Foundation.Rect>();
        private int _columnEnd = 0;
        private int[] _columnIndexes = new int[0];
        private int _columnStart = 0;
        private BorderLine _gridLine = null;
        private Dictionary<ulong, BorderLine> _hBorderLineCache = new Dictionary<ulong, BorderLine>();
        private Dictionary<ComboLine, LineItem> _lineMap = new Dictionary<ComboLine, LineItem>();
        private BorderLinesPool _linesPool;
        private int _rowEnd = 0;
        private int _rowEndDirty;
        private int[] _rowIndexes = new int[0];
        private int _rowStart = 0;
        private int _rowStartDirty;
        private Panel _scrollingGridlinesPanel = new Canvas();
        private SheetView _sheetView = null;
        private Dictionary<ulong, BorderLine> _vBorderLineCache = new Dictionary<ulong, BorderLine>();
        private GcViewport _viewport = null;
        private int _viewportBottomRow = -1;
        private int _viewportLeftColumn = -1;
        private int _viewportRightColumn = -1;
        private int _viewportTopRow = -1;
        private Worksheet _worksheet = null;
        private float _zoomFactor;
        private const int _HORIZONTAL = 0;
        private const int _VERTICAL = 1;

        public GcBorders(GcViewport viewport)
        {
            this._viewport = viewport;
            this._zoomFactor = 1f;
            base.Children.Add(this._borderLinesPanel);
            base.Children.Add(this._scrollingGridlinesPanel);
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (this._sheetView.fastScroll)
            {
                this._scrollingGridlinesPanel.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
                return base.ArrangeOverride(finalSize);
            }
            this._borderLinesPanel.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            foreach (KeyValuePair<ComboLine, LineItem> pair in this._lineMap)
            {
                ComboLine line = pair.Key;
                line.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
            return base.ArrangeOverride(finalSize);
        }

        private void BuildBordersInternal(ref int rIndex, ref int cIndex, int row, int column, int lineDirection, ref LineItem previousLineItem, ref BorderLine previousLine, ref BorderLine previousBreaker1, ref BorderLine previousBreaker2)
        {
            BorderLine line = null;
            if ((row != -1) || (column != -1))
            {
                if ((row == -1) && (lineDirection == 1))
                {
                    previousBreaker1 = this.GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                    previousBreaker2 = this.GetBorderLine(rIndex, cIndex, 0, this.NextColumn(cIndex), Borders.TOP);
                }
                else if ((column != -1) || (lineDirection != 0))
                {
                    if (column == -1)
                    {
                        line = this.GetBorderLine(rIndex, cIndex, row, 0, Borders.LEFT);
                    }
                    else if (row == -1)
                    {
                        line = this.GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                    }
                    else
                    {
                        Borders borderIndex = (lineDirection == 0) ? Borders.BOTTOM : Borders.RIGHT;
                        line = this.GetBorderLine(rIndex, cIndex, row, column, borderIndex);
                    }
                    bool flag = !BorderLineLayoutEngine.IsDoubleLine(line) && object.Equals(line, previousLine);
                    if (flag)
                    {
                        flag = BorderLine.Max(previousBreaker1, previousBreaker2) < line
                            || BorderLine.Max(previousBreaker1, previousBreaker2) == line;
                    }
                    if (flag && (IsDoubleLine(previousBreaker1) || IsDoubleLine(previousBreaker2)))
                    {
                        flag = false;
                    }
                    LineItem item = null;
                    if (flag)
                    {
                        item = previousLineItem;
                        switch (lineDirection)
                        {
                            case 0:
                                previousLineItem.ColumnEnd = column;
                                break;

                            case 1:
                                previousLineItem.RowEnd = row;
                                break;
                        }
                    }
                    else
                    {
                        item = new LineItem {
                            Direction = lineDirection,
                            RowFrom = row,
                            RowEnd = row,
                            ColumnFrom = column,
                            ColumnEnd = column,
                            Line = line,
                            PreviousLine = previousLine,
                            PreviousBreaker1 = previousBreaker1,
                            PreviousBreaker2 = previousBreaker2
                        };
                        if (((item.Line != BorderLine.Empty) && (item.Line != BorderLine.NoBorder)) && (!item.IsGridLine || (line.Color.A != 0)))
                        {
                            ((IThemeContextSupport) line).SetContext(this._worksheet);
                            ComboLine line2 = ComboLine.Create(this._linesPool, line);
                            ((IThemeContextSupport) line).SetContext(null);
                            this._lineMap.Add(line2, item);
                        }
                    }
                    switch (lineDirection)
                    {
                        case 0:
                            if (row != -1)
                            {
                                item.NextLine = this.GetBorderLine(rIndex, cIndex, row, this.NextColumn(cIndex), Borders.BOTTOM);
                                item.NextBreaker1 = this.GetBorderLine(rIndex, cIndex, row, column, Borders.RIGHT);
                                item.NextBreaker2 = this.GetBorderLine(rIndex, cIndex, this.NextRow(rIndex), column, Borders.RIGHT);
                                break;
                            }
                            item.NextLine = this.GetBorderLine(rIndex, cIndex, 0, this.NextColumn(cIndex), Borders.TOP);
                            item.NextBreaker1 = this._gridLine;
                            item.NextBreaker2 = this.GetBorderLine(rIndex, cIndex, 0, column, Borders.RIGHT);
                            break;

                        case 1:
                            if (column != -1)
                            {
                                item.NextLine = this.GetBorderLine(rIndex, cIndex, this.NextRow(rIndex), column, Borders.RIGHT);
                                item.NextBreaker1 = this.GetBorderLine(rIndex, cIndex, row, column, Borders.BOTTOM);
                                item.NextBreaker2 = this.GetBorderLine(rIndex, cIndex, row, this.NextColumn(cIndex), Borders.BOTTOM);
                                break;
                            }
                            item.NextLine = this.GetBorderLine(rIndex, cIndex, this.NextRow(rIndex), column, Borders.LEFT);
                            item.NextBreaker1 = this._gridLine;
                            item.NextBreaker2 = this.GetBorderLine(rIndex, cIndex, row, 0, Borders.BOTTOM);
                            break;
                    }
                    Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
                    ulong num = (ulong) row;
                    num = num << 0x20;
                    num |= (uint) column;
                    if (!this._cellBoundsCache.TryGetValue(num, out empty))
                    {
                        empty = this.GetCellBounds(row, column);
                        this._cellBoundsCache.Add(num, empty);
                    }
                    item.Bounds.Add(empty);
                    previousLine = line;
                    previousLineItem = item;
                    previousBreaker1 = item.NextBreaker1;
                    previousBreaker2 = item.NextBreaker2;
                }
            }
        }

        private void BuildHorizontalBorders()
        {
            if ((this._rowIndexes.Length != 1) || (this._rowIndexes[0] != -1))
            {
                for (int i = 0; i < this._rowIndexes.Length; i++)
                {
                    int row = this._rowIndexes[i];
                    if (row > this._viewportBottomRow)
                    {
                        return;
                    }
                    LineItem previousLineItem = null;
                    BorderLine previousLine = null;
                    BorderLine line2 = null;
                    BorderLine line3 = null;
                    int length = this._columnIndexes.Length;
                    for (int j = 0; j < length; j++)
                    {
                        int column = this._columnIndexes[j];
                        if ((j == (length - 1)) && (column > this._viewportRightColumn))
                        {
                            break;
                        }
                        this.BuildBordersInternal(ref i, ref j, row, column, 0, ref previousLineItem, ref previousLine, ref line2, ref line3);
                    }
                }
            }
        }

        private void BuildVerticalBorders()
        {
            if ((this._columnIndexes.Length != 1) || (this._columnIndexes[0] != -1))
            {
                for (int i = 0; i < this._columnIndexes.Length; i++)
                {
                    int column = this._columnIndexes[i];
                    if (column > this._viewportRightColumn)
                    {
                        return;
                    }
                    LineItem previousLineItem = null;
                    BorderLine previousLine = null;
                    BorderLine line2 = null;
                    BorderLine line3 = null;
                    int length = this._rowIndexes.Length;
                    for (int j = 0; j < length; j++)
                    {
                        int row = this._rowIndexes[j];
                        if ((j == (length - 1)) && (row > this._viewportBottomRow))
                        {
                            break;
                        }
                        this.BuildBordersInternal(ref j, ref i, row, column, 1, ref previousLineItem, ref previousLine, ref line2, ref line3);
                    }
                }
            }
        }

        private void CalcVisibleRowColumnIndexes()
        {
            switch (this._viewport.SheetArea)
            {
                case SheetArea.Cells:
                    this._viewportTopRow = this._sheetView.GetViewportTopRow(this._viewport.RowViewportIndex);
                    this._viewportBottomRow = this._sheetView.GetViewportBottomRow(this._viewport.RowViewportIndex);
                    this._viewportLeftColumn = this._sheetView.GetViewportLeftColumn(this._viewport.ColumnViewportIndex);
                    this._viewportRightColumn = this._sheetView.GetViewportRightColumn(this._viewport.ColumnViewportIndex);
                    break;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    this._viewportTopRow = this._sheetView.GetViewportTopRow(this._viewport.RowViewportIndex);
                    this._viewportBottomRow = this._sheetView.GetViewportBottomRow(this._viewport.RowViewportIndex);
                    this._viewportLeftColumn = 0;
                    this._viewportRightColumn = this._worksheet.RowHeader.ColumnCount - 1;
                    break;

                case SheetArea.ColumnHeader:
                    this._viewportTopRow = 0;
                    this._viewportBottomRow = this._worksheet.ColumnHeader.RowCount - 1;
                    this._viewportLeftColumn = this._sheetView.GetViewportLeftColumn(this._viewport.ColumnViewportIndex);
                    this._viewportRightColumn = this._sheetView.GetViewportRightColumn(this._viewport.ColumnViewportIndex);
                    break;
            }
            this._columnEnd = this._viewportRightColumn;
            this._rowEnd = this._viewportBottomRow;
            this._rowStart = this._viewportTopRow;
            this._columnStart = this._viewportLeftColumn;
            if ((this._rowStart <= this._rowEnd) && (this._columnStart <= this._columnEnd))
            {
                int num = -1;
                for (int i = this._rowStart - 1; i > -1; i--)
                {
                    if (this._worksheet.GetActualRowVisible(i, this._viewport.SheetArea))
                    {
                        num = i;
                        break;
                    }
                }
                this._rowStart = num;
                int num3 = -1;
                for (int j = this._columnStart - 1; j > -1; j--)
                {
                    if (this._worksheet.GetActualColumnVisible(j, this._viewport.SheetArea))
                    {
                        num3 = j;
                        break;
                    }
                }
                this._columnStart = num3;
                int count = this._viewport.GetDataContext().Rows.Count;
                for (int k = this._rowEnd + 1; k < count; k++)
                {
                    if (this._worksheet.GetActualRowVisible(k, this._viewport.SheetArea))
                    {
                        this._rowEnd = k;
                        break;
                    }
                }
                int num7 = this._viewport.GetDataContext().Columns.Count;
                for (int m = this._columnEnd + 1; m < num7; m++)
                {
                    if (this._worksheet.GetActualColumnVisible(m, this._viewport.SheetArea))
                    {
                        this._columnEnd = m;
                        break;
                    }
                }
                List<int> list = new List<int>();
                for (int n = this._rowStart; n <= this._rowEnd; n++)
                {
                    if (this._worksheet.GetActualRowVisible(n, this._viewport.SheetArea))
                    {
                        list.Add(n);
                    }
                    else if ((this._viewport.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) && (this._viewport.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                    {
                        int row = n + 1;
                        while ((row <= this._rowEnd) && (this._worksheet.GetActualRowHeight(row, this._viewport.SheetArea) == 0.0))
                        {
                            row++;
                        }
                        list.Add(row - 1);
                    }
                }
                this._rowIndexes = list.ToArray();
                list.Clear();
                for (int num11 = this._columnStart; num11 <= this._columnEnd; num11++)
                {
                    if (this._worksheet.GetActualColumnVisible(num11, this._viewport.SheetArea))
                    {
                        list.Add(num11);
                    }
                    else if ((this._viewport.SheetArea == SheetArea.ColumnHeader) && (this._viewport.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                    {
                        int column = num11 + 1;
                        while ((column <= this._columnEnd) && (this._worksheet.GetActualColumnWidth(column, this._viewport.SheetArea) == 0.0))
                        {
                            column++;
                        }
                        list.Add(column - 1);
                    }
                }
                this._columnIndexes = list.ToArray();
            }
        }

        private void ClearBorderLineCache()
        {
            this._vBorderLineCache.Clear();
            this._hBorderLineCache.Clear();
            this._cellBoundsCache.Clear();
        }

        private ulong ConverIndexToKey(int rIndex, int cIndex, int row, int column, Borders borderIndex)
        {
            ulong num = 0L;
            if (borderIndex == Borders.RIGHT)
            {
                num = (ulong) row;
                num = num << 0x20;
                return (num | ((uint) column));
            }
            if (borderIndex == Borders.BOTTOM)
            {
                num = (ulong) row;
                num = num << 0x20;
                return (num | ((uint)column));
            }
            if (borderIndex == Borders.LEFT)
            {
                int num2 = this.PreviousColumn(cIndex);
                num = (ulong) row;
                num = num << 0x20;
                return (num | ((uint)num2));
            }
            if (borderIndex == Borders.TOP)
            {
                num = (ulong) this.PreviousRow(rIndex);
                num = num << 0x20;
                num |= (uint)column;
            }
            return num;
        }

        /// <summary>
        /// GetCachedBorderLine 
        /// </summary>
        private BorderLine GetBorderLine(int rIndex, int cIndex, int row, int column, Borders borderIndex)
        {
            if ((row == -1) && (borderIndex != Borders.BOTTOM))
            {
                return null;
            }
            if ((column == -1) && (borderIndex != Borders.RIGHT))
            {
                return null;
            }
            ulong num = this.ConverIndexToKey(rIndex, cIndex, row, column, borderIndex);
            BorderLine noBorder = null;
            if ((borderIndex == Borders.LEFT) || (borderIndex == Borders.RIGHT))
            {
                if (this._vBorderLineCache.TryGetValue(num, out noBorder))
                {
                    return noBorder;
                }
            }
            else if (this._hBorderLineCache.TryGetValue(num, out noBorder))
            {
                return noBorder;
            }
            bool isInCellflow = false;
            noBorder = this.GetCellActualBorderLine(row, column, borderIndex, out isInCellflow);
            if (!isInCellflow)
            {
                BorderLine line2 = null;
                Borders borders = (Borders)(((int)borderIndex + 2) % 4);
                if (borderIndex == Borders.LEFT)
                {
                    line2 = this.GetCellActualBorderLine(row, this.PreviousColumn(cIndex), borders, out isInCellflow);
                }
                else if (borderIndex == Borders.TOP)
                {
                    line2 = this.GetCellActualBorderLine(this.PreviousRow(rIndex), column, borders, out isInCellflow);
                }
                else if (borderIndex == Borders.RIGHT)
                {
                    line2 = this.GetCellActualBorderLine(row, this.NextColumn(cIndex), borders, out isInCellflow);
                }
                else
                {
                    if (borderIndex != Borders.BOTTOM)
                    {
                        throw new NotSupportedException(ResourceStrings.NotSupportExceptionBorderIndexError);
                    }
                    line2 = this.GetCellActualBorderLine(this.NextRow(rIndex), column, borders, out isInCellflow);
                }
                if (!IsDoubleLine(noBorder) && IsDoubleLine(line2))
                {
                    noBorder = line2;
                }
                else if ((!IsDoubleLine(noBorder) || IsDoubleLine(line2)) && (line2 > noBorder))
                {
                    noBorder = line2;
                }
            }
            if (noBorder == null)
            {
                noBorder = BorderLine.NoBorder;
            }
            if ((borderIndex == Borders.LEFT) || (borderIndex == Borders.RIGHT))
            {
                this._vBorderLineCache.Add(num, noBorder);
                return noBorder;
            }
            this._hBorderLineCache.Add(num, noBorder);
            return noBorder;
        }

        private BorderLine GetCellActualBorderLine(int row, int column, Borders borderIndex, out bool isInCellflow)
        {
            isInCellflow = false;
            bool flag = false;
            BorderLine empty = null;
            Cell cachedCell = null;
            byte state = this._viewport.CachedSpanGraph.GetState(row, column);
            switch (borderIndex)
            {
                case Borders.LEFT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model = this._viewport.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model != null)
                        {
                            CellOverflowLayout cellOverflowLayout = model.GetCellOverflowLayout(column);
                            if (cellOverflowLayout != null)
                            {
                                if (cellOverflowLayout.StartingColumn > -1)
                                {
                                    if (cellOverflowLayout.StartingColumn == column)
                                    {
                                        empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (cellOverflowLayout.Column == column)
                                {
                                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 1) != 1)
                    {
                        flag = true;
                        break;
                    }
                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.TOP:
                    if (state <= 0)
                    {
                        empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 2) != 2)
                    {
                        flag = true;
                        break;
                    }
                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.RIGHT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model2 = this._viewport.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model2 != null)
                        {
                            CellOverflowLayout layout2 = model2.GetCellOverflowLayout(column);
                            if (layout2 != null)
                            {
                                if (layout2.EndingColumn > -1)
                                {
                                    if (layout2.EndingColumn == column)
                                    {
                                        empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (layout2.Column == column)
                                {
                                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 4) != 4)
                    {
                        flag = true;
                        break;
                    }
                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.BOTTOM:
                    if (state <= 0)
                    {
                        empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 8) != 8)
                    {
                        flag = true;
                        break;
                    }
                    empty = this.GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;
            }
            if ((!flag && !isInCellflow) && (empty == null))
            {
                if ((this._viewport.SheetArea == SheetArea.Cells) && (this._zoomFactor < 0.4f))
                {
                    return BorderLine.Empty;
                }
                empty = this._gridLine;
                if ((state & 0x20) == 0x20)
                {
                    return BorderLine.Empty;
                }
                if (state != 0)
                {
                    return empty;
                }
                if (cachedCell == null)
                {
                    cachedCell = this._viewport.CellCache.GetCachedCell(row, column);
                }
                if ((cachedCell != null) && (cachedCell.ActualBackground != null))
                {
                    empty = BorderLine.Empty;
                }
            }
            return empty;
        }

        private BorderLine GetCellBorderByBorderIndex(int row, int column, Borders borderIndex, ref Cell cell)
        {
            GcViewport viewport = this._viewport;
            if (cell == null)
            {
                cell = viewport.CellCache.GetCachedCell(row, column);
                if (cell == null)
                {
                    return null;
                }
            }
            switch (borderIndex)
            {
                case Borders.LEFT:
                    return cell.ActualBorderLeft;

                case Borders.TOP:
                    return cell.ActualBorderTop;

                case Borders.RIGHT:
                    return cell.ActualBorderRight;

                case Borders.BOTTOM:
                    return cell.ActualBorderBottom;
            }
            return null;
        }

        private Windows.Foundation.Rect GetCellBounds(int row, int column)
        {
            Windows.Foundation.Rect rect = this._viewport.GetCellBounds(row, column, true);
            if (rect.X == -1.0)
            {
                rect.X = this._viewport.Location.X;
            }
            if (rect.Y == -1.0)
            {
                rect.Y = this._viewport.Location.Y;
            }
            return rect;
        }

        private SheetView GetSheetView()
        {
            if (this._viewport != null)
            {
                return this._viewport.Sheet;
            }
            return null;
        }

        private GcViewport GetViewport()
        {
            return this._viewport;
        }

        private Worksheet GetWorksheet()
        {
            GcViewport viewport = this._viewport;
            if ((viewport != null) && (viewport.Sheet != null))
            {
                return viewport.Sheet.Worksheet;
            }
            return null;
        }

        private void InitDirtyRange()
        {
            SheetArea sheetArea = this._viewport.SheetArea;
            int num = this._worksheet.NextNonEmptyColumn(this._columnStart - 1, sheetArea);
            if ((this._columnStart <= num) && (num <= this._columnEnd))
            {
                this._rowStartDirty = this._rowStart;
                this._rowEndDirty = this._rowEnd;
            }
            else
            {
                this._rowEndDirty = this._rowStartDirty = this._worksheet.NextNonEmptyRow(this._rowStart - 1, sheetArea, StorageType.Style);
                if (this._rowStartDirty > -1)
                {
                    int row = this._rowStartDirty;
                    while (row <= this._rowEnd)
                    {
                        row = this._worksheet.NextNonEmptyRow(row, sheetArea, StorageType.Style);
                        if (row == -1)
                        {
                            break;
                        }
                        this._rowEndDirty = row;
                    }
                }
                else
                {
                    this._rowEndDirty = -1;
                }
            }
            if (this._rowStartDirty > -1)
            {
                this._rowStartDirty--;
            }
            if (this._rowEndDirty > -1)
            {
                this._rowEndDirty++;
            }
        }

        private static bool IsDoubleLine(BorderLine line)
        {
            return BorderLineLayoutEngine.IsDoubleLine(line);
        }

        private void MeasureBorders(Windows.Foundation.Size availableSize)
        {
            this._borderLinesPanel.Visibility = Visibility.Visible;
            this._scrollingGridlinesPanel.Visibility = Visibility.Collapsed;
            this.CalcVisibleRowColumnIndexes();
            this.InitDirtyRange();
            this.ClearBorderLineCache();
            this.BuildHorizontalBorders();
            this.BuildVerticalBorders();
            this.ClearBorderLineCache();
        }

        private void MeasureGridLinesForScrolling()
        {
            Action action = null;
            this._borderLinesPanel.Visibility = Visibility.Collapsed;
            this._scrollingGridlinesPanel.Visibility = Visibility.Visible;
            this._scrollingGridlinesPanel.Children.Clear();
            this.CalcVisibleRowColumnIndexes();
            BorderLine gridBorderLine = this._worksheet.GetGridLine(this._viewport.SheetArea);
            RowLayoutModel rowLayoutModel = this._sheetView.GetRowLayoutModel(this._viewport.RowViewportIndex, this._viewport.SheetArea);
            ColumnLayoutModel columnLayoutModel = this._sheetView.GetColumnLayoutModel(this._viewport.ColumnViewportIndex, this._viewport.SheetArea);
            int viewportBottomRow = this._sheetView.GetViewportBottomRow(this._viewport.RowViewportIndex);
            int viewportRightColumn = this._sheetView.GetViewportRightColumn(this._viewport.ColumnViewportIndex);
            RowLayout bottomRowLayout = rowLayoutModel.FindRow(viewportBottomRow);
            ColumnLayout rightColumnLayout = columnLayoutModel.FindColumn(viewportRightColumn);
            Windows.Foundation.Point viewportLocation = this._viewport.Location;
            if (((this._rowIndexes.Length != 1) || (this._rowIndexes[0] != -1)) && ((this._columnIndexes.Length != 1) || (this._columnIndexes[0] != -1)))
            {
                if (action == null)
                {
                    action = delegate {
                        double viewportWidth = this._sheetView.GetViewportWidth(this._viewport.ColumnViewportIndex);
                        SolidColorBrush brush = new SolidColorBrush(gridBorderLine.Color);
                        if ((rightColumnLayout != null) && (viewportWidth > ((rightColumnLayout.X + rightColumnLayout.Width) - viewportLocation.X)))
                        {
                            double x = rightColumnLayout.X;
                            double width = rightColumnLayout.Width;
                            double num9 = viewportLocation.X;
                        }
                        double num2 = viewportLocation.Y + 0.5;
                        for (int k = 0; k < this._rowIndexes.Length; k++)
                        {
                            RowLayout layout = rowLayoutModel.FindRow(this._rowIndexes[k]);
                            if (layout != null)
                            {
                                Windows.UI.Xaml.Shapes.Line line = new Windows.UI.Xaml.Shapes.Line();
                                line.X1 = 0.0;
                                line.Y1 = (layout.Y + layout.Height) - num2;
                                line.X2 = viewportWidth;
                                line.Y2 = line.Y1;
                                line.Stroke = brush;
                                line.StrokeThickness = 1.0;
                                this._scrollingGridlinesPanel.Children.Add(line);
                            }
                        }
                        double viewportHeight = this._sheetView.GetViewportHeight(this._viewport.RowViewportIndex);
                        double num5 = viewportLocation.X + 0.5;
                        double num6 = viewportHeight;
                        if ((bottomRowLayout != null) && (viewportHeight > ((bottomRowLayout.Y + bottomRowLayout.Height) - viewportLocation.Y)))
                        {
                            num6 = ((bottomRowLayout.Y + bottomRowLayout.Height) - viewportLocation.Y) - 0.5;
                        }
                        for (int i = 0; i < this._columnIndexes.Length; i++)
                        {
                            ColumnLayout layout2 = columnLayoutModel.FindColumn(this._columnIndexes[i]);
                            if (layout2 != null)
                            {
                                Windows.UI.Xaml.Shapes.Line line2 = new Windows.UI.Xaml.Shapes.Line();
                                line2.X1 = (layout2.X + layout2.Width) - num5;
                                line2.Y1 = 0.0;
                                line2.X2 = line2.X1;
                                line2.Y2 = num6;
                                line2.Stroke = brush;
                                line2.StrokeThickness = 1.0;
                                this._scrollingGridlinesPanel.Children.Add(line2);
                            }
                        }
                    };
                }
                Dt.Cells.Data.UIAdaptor.InvokeSync(action);
            }
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            this._sheetView = this.GetSheetView();
            if (this._sheetView.fastScroll)
            {
                this._scrollingGridlinesPanel.Measure(availableSize);
                this.MeasureGridLinesForScrolling();
                return base.MeasureOverride(availableSize);
            }
            this._worksheet = this.GetWorksheet();
            this._rowIndexes = new int[0];
            this._columnIndexes = new int[0];
            this._rowStart = 0;
            this._rowEnd = 0;
            this._columnStart = 0;
            this._columnEnd = 0;
            this._borderLinesPanel.Measure(availableSize);
            this._lineMap = new Dictionary<ComboLine, LineItem>();
            this._linesPool = new BorderLinesPool(this._borderLinesPanel.Children);
            this._linesPool.Reset();
            this._zoomFactor = this._sheetView.ZoomFactor;
            if (this._worksheet != null)
            {
                this._gridLine = this._worksheet.GetGridLine(this._viewport.SheetArea);
                if (((this._viewport.SheetArea == SheetArea.ColumnHeader) || (this._viewport.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader))) && this._viewport.Sheet.HeaderGridLineColor.HasValue)
                {
                    this._gridLine = new BorderLine(this._viewport.Sheet.HeaderGridLineColor.Value, BorderLineStyle.Thin);
                }
                switch (this._viewport.SheetArea)
                {
                    case SheetArea.Cells:
                    {
                        SheetSpanModel spanModel = this._worksheet.SpanModel;
                        break;
                    }
                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    {
                        SheetSpanModel rowHeaderSpanModel = this._worksheet.RowHeaderSpanModel;
                        break;
                    }
                    case SheetArea.ColumnHeader:
                    {
                        SheetSpanModel columnHeaderSpanModel = this._worksheet.ColumnHeaderSpanModel;
                        break;
                    }
                }
                this.MeasureBorders(availableSize);
            }
            this._linesPool.Collect();
            foreach (ComboLine line in this._lineMap.Keys)
            {
                LineItem lineItem = this._lineMap[line];
                Windows.Foundation.Point point = this._viewport.PointToClient(new Windows.Foundation.Point(0.0, 0.0));
                line.Width = availableSize.Width;
                line.Height = availableSize.Height;
                ((IThemeContextSupport) lineItem).SetContext(this._worksheet);
                line.Layout(lineItem, -point.X, -point.Y);
                ((IThemeContextSupport) lineItem).SetContext(null);
            }
            return base.MeasureOverride(availableSize);
        }

        private int NextColumn(int cPos)
        {
            return NextIndex(this._columnIndexes, cPos);
        }

        private static int NextIndex(int[] indexes, int rPos)
        {
            if ((rPos >= -1) && (rPos < (indexes.Length - 1)))
            {
                return indexes[rPos + 1];
            }
            return -1;
        }

        private int NextRow(int rPos)
        {
            return NextIndex(this._rowIndexes, rPos);
        }

        private int PreviousColumn(int cPos)
        {
            return PreviousIndex(this._columnIndexes, cPos);
        }

        private static int PreviousIndex(int[] indexes, int rPos)
        {
            if ((rPos >= 1) && (rPos <= indexes.Length))
            {
                return indexes[rPos - 1];
            }
            return -1;
        }

        private int PreviousRow(int rPos)
        {
            return PreviousIndex(this._rowIndexes, rPos);
        }
    }
}

