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
        Panel _borderLinesPanel = new Canvas();
        Dictionary<ulong, Windows.Foundation.Rect> _cellBoundsCache = new Dictionary<ulong, Windows.Foundation.Rect>();
        int _columnEnd = 0;
        int[] _columnIndexes = new int[0];
        int _columnStart = 0;
        BorderLine _gridLine = null;
        Dictionary<ulong, BorderLine> _hBorderLineCache = new Dictionary<ulong, BorderLine>();
        Dictionary<ComboLine, LineItem> _lineMap = new Dictionary<ComboLine, LineItem>();
        BorderLinesPool _linesPool;
        int _rowEnd = 0;
        int _rowEndDirty;
        int[] _rowIndexes = new int[0];
        int _rowStart = 0;
        int _rowStartDirty;
        Panel _scrollingGridlinesPanel = new Canvas();
        SheetView _sheetView = null;
        Dictionary<ulong, BorderLine> _vBorderLineCache = new Dictionary<ulong, BorderLine>();
        GcViewport _viewport = null;
        int _viewportBottomRow = -1;
        int _viewportLeftColumn = -1;
        int _viewportRightColumn = -1;
        int _viewportTopRow = -1;
        Worksheet _worksheet = null;
        float _zoomFactor;
        const int _HORIZONTAL = 0;
        const int _VERTICAL = 1;

        public GcBorders(GcViewport viewport)
        {
            _viewport = viewport;
            _zoomFactor = 1f;
            base.Children.Add(_borderLinesPanel);
            base.Children.Add(_scrollingGridlinesPanel);
        }

        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            if (_sheetView._fastScroll)
            {
                _scrollingGridlinesPanel.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
                return base.ArrangeOverride(finalSize);
            }

            _borderLinesPanel.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            foreach (KeyValuePair<ComboLine, LineItem> pair in _lineMap)
            {
                ComboLine line = pair.Key;
                line.Arrange(new Windows.Foundation.Rect(0.0, 0.0, finalSize.Width, finalSize.Height));
            }
            return base.ArrangeOverride(finalSize);
        }

        void BuildBordersInternal(ref int rIndex, ref int cIndex, int row, int column, int lineDirection, ref LineItem previousLineItem, ref BorderLine previousLine, ref BorderLine previousBreaker1, ref BorderLine previousBreaker2)
        {
            BorderLine line = null;
            if ((row != -1) || (column != -1))
            {
                if ((row == -1) && (lineDirection == 1))
                {
                    previousBreaker1 = GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                    previousBreaker2 = GetBorderLine(rIndex, cIndex, 0, NextColumn(cIndex), Borders.TOP);
                }
                else if ((column != -1) || (lineDirection != 0))
                {
                    if (column == -1)
                    {
                        line = GetBorderLine(rIndex, cIndex, row, 0, Borders.LEFT);
                    }
                    else if (row == -1)
                    {
                        line = GetBorderLine(rIndex, cIndex, 0, column, Borders.TOP);
                    }
                    else
                    {
                        Borders borderIndex = (lineDirection == 0) ? Borders.BOTTOM : Borders.RIGHT;
                        line = GetBorderLine(rIndex, cIndex, row, column, borderIndex);
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
                            ((IThemeContextSupport) line).SetContext(_worksheet);
                            ComboLine line2 = ComboLine.Create(_linesPool, line);
                            ((IThemeContextSupport) line).SetContext(null);
                            _lineMap.Add(line2, item);
                        }
                    }
                    switch (lineDirection)
                    {
                        case 0:
                            if (row != -1)
                            {
                                item.NextLine = GetBorderLine(rIndex, cIndex, row, NextColumn(cIndex), Borders.BOTTOM);
                                item.NextBreaker1 = GetBorderLine(rIndex, cIndex, row, column, Borders.RIGHT);
                                item.NextBreaker2 = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.RIGHT);
                                break;
                            }
                            item.NextLine = GetBorderLine(rIndex, cIndex, 0, NextColumn(cIndex), Borders.TOP);
                            item.NextBreaker1 = _gridLine;
                            item.NextBreaker2 = GetBorderLine(rIndex, cIndex, 0, column, Borders.RIGHT);
                            break;

                        case 1:
                            if (column != -1)
                            {
                                item.NextLine = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.RIGHT);
                                item.NextBreaker1 = GetBorderLine(rIndex, cIndex, row, column, Borders.BOTTOM);
                                item.NextBreaker2 = GetBorderLine(rIndex, cIndex, row, NextColumn(cIndex), Borders.BOTTOM);
                                break;
                            }
                            item.NextLine = GetBorderLine(rIndex, cIndex, NextRow(rIndex), column, Borders.LEFT);
                            item.NextBreaker1 = _gridLine;
                            item.NextBreaker2 = GetBorderLine(rIndex, cIndex, row, 0, Borders.BOTTOM);
                            break;
                    }
                    Windows.Foundation.Rect empty = Windows.Foundation.Rect.Empty;
                    ulong num = (ulong) row;
                    num = num << 0x20;
                    num |= (uint) column;
                    if (!_cellBoundsCache.TryGetValue(num, out empty))
                    {
                        empty = GetCellBounds(row, column);
                        _cellBoundsCache.Add(num, empty);
                    }
                    item.Bounds.Add(empty);
                    previousLine = line;
                    previousLineItem = item;
                    previousBreaker1 = item.NextBreaker1;
                    previousBreaker2 = item.NextBreaker2;
                }
            }
        }

        void BuildHorizontalBorders()
        {
            if ((_rowIndexes.Length != 1) || (_rowIndexes[0] != -1))
            {
                for (int i = 0; i < _rowIndexes.Length; i++)
                {
                    int row = _rowIndexes[i];
                    if (row > _viewportBottomRow)
                    {
                        return;
                    }
                    LineItem previousLineItem = null;
                    BorderLine previousLine = null;
                    BorderLine line2 = null;
                    BorderLine line3 = null;
                    int length = _columnIndexes.Length;
                    for (int j = 0; j < length; j++)
                    {
                        int column = _columnIndexes[j];
                        if ((j == (length - 1)) && (column > _viewportRightColumn))
                        {
                            break;
                        }
                        BuildBordersInternal(ref i, ref j, row, column, 0, ref previousLineItem, ref previousLine, ref line2, ref line3);
                    }
                }
            }
        }

        void BuildVerticalBorders()
        {
            if ((_columnIndexes.Length != 1) || (_columnIndexes[0] != -1))
            {
                for (int i = 0; i < _columnIndexes.Length; i++)
                {
                    int column = _columnIndexes[i];
                    if (column > _viewportRightColumn)
                    {
                        return;
                    }
                    LineItem previousLineItem = null;
                    BorderLine previousLine = null;
                    BorderLine line2 = null;
                    BorderLine line3 = null;
                    int length = _rowIndexes.Length;
                    for (int j = 0; j < length; j++)
                    {
                        int row = _rowIndexes[j];
                        if ((j == (length - 1)) && (row > _viewportBottomRow))
                        {
                            break;
                        }
                        BuildBordersInternal(ref j, ref i, row, column, 1, ref previousLineItem, ref previousLine, ref line2, ref line3);
                    }
                }
            }
        }

        void CalcVisibleRowColumnIndexes()
        {
            switch (_viewport.SheetArea)
            {
                case SheetArea.Cells:
                    _viewportTopRow = _sheetView.GetViewportTopRow(_viewport.RowViewportIndex);
                    _viewportBottomRow = _sheetView.GetViewportBottomRow(_viewport.RowViewportIndex);
                    _viewportLeftColumn = _sheetView.GetViewportLeftColumn(_viewport.ColumnViewportIndex);
                    _viewportRightColumn = _sheetView.GetViewportRightColumn(_viewport.ColumnViewportIndex);
                    break;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    _viewportTopRow = _sheetView.GetViewportTopRow(_viewport.RowViewportIndex);
                    _viewportBottomRow = _sheetView.GetViewportBottomRow(_viewport.RowViewportIndex);
                    _viewportLeftColumn = 0;
                    _viewportRightColumn = _worksheet.RowHeader.ColumnCount - 1;
                    break;

                case SheetArea.ColumnHeader:
                    _viewportTopRow = 0;
                    _viewportBottomRow = _worksheet.ColumnHeader.RowCount - 1;
                    _viewportLeftColumn = _sheetView.GetViewportLeftColumn(_viewport.ColumnViewportIndex);
                    _viewportRightColumn = _sheetView.GetViewportRightColumn(_viewport.ColumnViewportIndex);
                    break;
            }
            _columnEnd = _viewportRightColumn;
            _rowEnd = _viewportBottomRow;
            _rowStart = _viewportTopRow;
            _columnStart = _viewportLeftColumn;
            if ((_rowStart <= _rowEnd) && (_columnStart <= _columnEnd))
            {
                int num = -1;
                for (int i = _rowStart - 1; i > -1; i--)
                {
                    if (_worksheet.GetActualRowVisible(i, _viewport.SheetArea))
                    {
                        num = i;
                        break;
                    }
                }
                _rowStart = num;
                int num3 = -1;
                for (int j = _columnStart - 1; j > -1; j--)
                {
                    if (_worksheet.GetActualColumnVisible(j, _viewport.SheetArea))
                    {
                        num3 = j;
                        break;
                    }
                }
                _columnStart = num3;
                int count = _viewport.GetDataContext().Rows.Count;
                for (int k = _rowEnd + 1; k < count; k++)
                {
                    if (_worksheet.GetActualRowVisible(k, _viewport.SheetArea))
                    {
                        _rowEnd = k;
                        break;
                    }
                }
                int num7 = _viewport.GetDataContext().Columns.Count;
                for (int m = _columnEnd + 1; m < num7; m++)
                {
                    if (_worksheet.GetActualColumnVisible(m, _viewport.SheetArea))
                    {
                        _columnEnd = m;
                        break;
                    }
                }
                List<int> list = new List<int>();
                for (int n = _rowStart; n <= _rowEnd; n++)
                {
                    if (_worksheet.GetActualRowVisible(n, _viewport.SheetArea))
                    {
                        list.Add(n);
                    }
                    else if ((_viewport.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader)) && (_viewport.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                    {
                        int row = n + 1;
                        while ((row <= _rowEnd) && (_worksheet.GetActualRowHeight(row, _viewport.SheetArea) == 0.0))
                        {
                            row++;
                        }
                        list.Add(row - 1);
                    }
                }
                _rowIndexes = list.ToArray();
                list.Clear();
                for (int num11 = _columnStart; num11 <= _columnEnd; num11++)
                {
                    if (_worksheet.GetActualColumnVisible(num11, _viewport.SheetArea))
                    {
                        list.Add(num11);
                    }
                    else if ((_viewport.SheetArea == SheetArea.ColumnHeader) && (_viewport.Sheet.ResizeZeroIndicator == ResizeZeroIndicator.Enhanced))
                    {
                        int column = num11 + 1;
                        while ((column <= _columnEnd) && (_worksheet.GetActualColumnWidth(column, _viewport.SheetArea) == 0.0))
                        {
                            column++;
                        }
                        list.Add(column - 1);
                    }
                }
                _columnIndexes = list.ToArray();
            }
        }

        void ClearBorderLineCache()
        {
            _vBorderLineCache.Clear();
            _hBorderLineCache.Clear();
            _cellBoundsCache.Clear();
        }

        ulong ConverIndexToKey(int rIndex, int cIndex, int row, int column, Borders borderIndex)
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
                int num2 = PreviousColumn(cIndex);
                num = (ulong) row;
                num = num << 0x20;
                return (num | ((uint)num2));
            }
            if (borderIndex == Borders.TOP)
            {
                num = (ulong) PreviousRow(rIndex);
                num = num << 0x20;
                num |= (uint)column;
            }
            return num;
        }

        /// <summary>
        /// GetCachedBorderLine 
        /// </summary>
        BorderLine GetBorderLine(int rIndex, int cIndex, int row, int column, Borders borderIndex)
        {
            if ((row == -1) && (borderIndex != Borders.BOTTOM))
            {
                return null;
            }
            if ((column == -1) && (borderIndex != Borders.RIGHT))
            {
                return null;
            }
            ulong num = ConverIndexToKey(rIndex, cIndex, row, column, borderIndex);
            BorderLine noBorder = null;
            if ((borderIndex == Borders.LEFT) || (borderIndex == Borders.RIGHT))
            {
                if (_vBorderLineCache.TryGetValue(num, out noBorder))
                {
                    return noBorder;
                }
            }
            else if (_hBorderLineCache.TryGetValue(num, out noBorder))
            {
                return noBorder;
            }
            bool isInCellflow = false;
            noBorder = GetCellActualBorderLine(row, column, borderIndex, out isInCellflow);
            if (!isInCellflow)
            {
                BorderLine line2 = null;
                Borders borders = (Borders)(((int)borderIndex + 2) % 4);
                if (borderIndex == Borders.LEFT)
                {
                    line2 = GetCellActualBorderLine(row, PreviousColumn(cIndex), borders, out isInCellflow);
                }
                else if (borderIndex == Borders.TOP)
                {
                    line2 = GetCellActualBorderLine(PreviousRow(rIndex), column, borders, out isInCellflow);
                }
                else if (borderIndex == Borders.RIGHT)
                {
                    line2 = GetCellActualBorderLine(row, NextColumn(cIndex), borders, out isInCellflow);
                }
                else
                {
                    if (borderIndex != Borders.BOTTOM)
                    {
                        throw new NotSupportedException(ResourceStrings.NotSupportExceptionBorderIndexError);
                    }
                    line2 = GetCellActualBorderLine(NextRow(rIndex), column, borders, out isInCellflow);
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
                _vBorderLineCache.Add(num, noBorder);
                return noBorder;
            }
            _hBorderLineCache.Add(num, noBorder);
            return noBorder;
        }

        BorderLine GetCellActualBorderLine(int row, int column, Borders borderIndex, out bool isInCellflow)
        {
            isInCellflow = false;
            bool flag = false;
            BorderLine empty = null;
            Cell cachedCell = null;
            byte state = _viewport.CachedSpanGraph.GetState(row, column);
            switch (borderIndex)
            {
                case Borders.LEFT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model = _viewport.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model != null)
                        {
                            CellOverflowLayout cellOverflowLayout = model.GetCellOverflowLayout(column);
                            if (cellOverflowLayout != null)
                            {
                                if (cellOverflowLayout.StartingColumn > -1)
                                {
                                    if (cellOverflowLayout.StartingColumn == column)
                                    {
                                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (cellOverflowLayout.Column == column)
                                {
                                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 1) != 1)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.TOP:
                    if (state <= 0)
                    {
                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 2) != 2)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.RIGHT:
                    if (state <= 0)
                    {
                        CellOverflowLayoutModel model2 = _viewport.CellOverflowLayoutBuildEngine.GetModel(row);
                        if (model2 != null)
                        {
                            CellOverflowLayout layout2 = model2.GetCellOverflowLayout(column);
                            if (layout2 != null)
                            {
                                if (layout2.EndingColumn > -1)
                                {
                                    if (layout2.EndingColumn == column)
                                    {
                                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                    }
                                    else
                                    {
                                        empty = null;
                                        isInCellflow = true;
                                    }
                                }
                                else if (layout2.Column == column)
                                {
                                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                                }
                                else
                                {
                                    empty = null;
                                    isInCellflow = true;
                                }
                            }
                            else
                            {
                                empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                            }
                        }
                        else
                        {
                            empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        }
                        break;
                    }
                    if ((state & 4) != 4)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;

                case Borders.BOTTOM:
                    if (state <= 0)
                    {
                        empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                        break;
                    }
                    if ((state & 8) != 8)
                    {
                        flag = true;
                        break;
                    }
                    empty = GetCellBorderByBorderIndex(row, column, borderIndex, ref cachedCell);
                    break;
            }
            if ((!flag && !isInCellflow) && (empty == null))
            {
                if ((_viewport.SheetArea == SheetArea.Cells) && (_zoomFactor < 0.4f))
                {
                    return BorderLine.Empty;
                }
                empty = _gridLine;
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
                    cachedCell = _viewport.CellCache.GetCachedCell(row, column);
                }
                if ((cachedCell != null) && (cachedCell.ActualBackground != null))
                {
                    empty = BorderLine.Empty;
                }
            }
            return empty;
        }

        BorderLine GetCellBorderByBorderIndex(int row, int column, Borders borderIndex, ref Cell cell)
        {
            GcViewport viewport = _viewport;
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

        Windows.Foundation.Rect GetCellBounds(int row, int column)
        {
            Windows.Foundation.Rect rect = _viewport.GetCellBounds(row, column, true);
            if (rect.X == -1.0)
            {
                rect.X = _viewport.Location.X;
            }
            if (rect.Y == -1.0)
            {
                rect.Y = _viewport.Location.Y;
            }
            return rect;
        }

        SheetView GetSheetView()
        {
            if (_viewport != null)
            {
                return _viewport.Sheet;
            }
            return null;
        }

        GcViewport GetViewport()
        {
            return _viewport;
        }

        Worksheet GetWorksheet()
        {
            GcViewport viewport = _viewport;
            if ((viewport != null) && (viewport.Sheet != null))
            {
                return viewport.Sheet.Worksheet;
            }
            return null;
        }

        void InitDirtyRange()
        {
            SheetArea sheetArea = _viewport.SheetArea;
            int num = _worksheet.NextNonEmptyColumn(_columnStart - 1, sheetArea);
            if ((_columnStart <= num) && (num <= _columnEnd))
            {
                _rowStartDirty = _rowStart;
                _rowEndDirty = _rowEnd;
            }
            else
            {
                _rowEndDirty = _rowStartDirty = _worksheet.NextNonEmptyRow(_rowStart - 1, sheetArea, StorageType.Style);
                if (_rowStartDirty > -1)
                {
                    int row = _rowStartDirty;
                    while (row <= _rowEnd)
                    {
                        row = _worksheet.NextNonEmptyRow(row, sheetArea, StorageType.Style);
                        if (row == -1)
                        {
                            break;
                        }
                        _rowEndDirty = row;
                    }
                }
                else
                {
                    _rowEndDirty = -1;
                }
            }
            if (_rowStartDirty > -1)
            {
                _rowStartDirty--;
            }
            if (_rowEndDirty > -1)
            {
                _rowEndDirty++;
            }
        }

        static bool IsDoubleLine(BorderLine line)
        {
            return BorderLineLayoutEngine.IsDoubleLine(line);
        }

        void MeasureBorders(Windows.Foundation.Size availableSize)
        {
            _borderLinesPanel.Visibility = Visibility.Visible;
            _scrollingGridlinesPanel.Visibility = Visibility.Collapsed;
            CalcVisibleRowColumnIndexes();
            InitDirtyRange();
            ClearBorderLineCache();
            BuildHorizontalBorders();
            BuildVerticalBorders();
            ClearBorderLineCache();
        }

        void MeasureGridLinesForScrolling()
        {
            _borderLinesPanel.Visibility = Visibility.Collapsed;
            _scrollingGridlinesPanel.Visibility = Visibility.Visible;
            _scrollingGridlinesPanel.Children.Clear();
            CalcVisibleRowColumnIndexes();
            BorderLine gridBorderLine = _worksheet.GetGridLine(_viewport.SheetArea);
            RowLayoutModel rowLayoutModel = _sheetView.GetRowLayoutModel(_viewport.RowViewportIndex, _viewport.SheetArea);
            ColumnLayoutModel columnLayoutModel = _sheetView.GetColumnLayoutModel(_viewport.ColumnViewportIndex, _viewport.SheetArea);
            int viewportBottomRow = _sheetView.GetViewportBottomRow(_viewport.RowViewportIndex);
            int viewportRightColumn = _sheetView.GetViewportRightColumn(_viewport.ColumnViewportIndex);
            RowLayout bottomRowLayout = rowLayoutModel.FindRow(viewportBottomRow);
            ColumnLayout rightColumnLayout = columnLayoutModel.FindColumn(viewportRightColumn);
            Windows.Foundation.Point viewportLocation = _viewport.Location;
            if (((_rowIndexes.Length != 1) || (_rowIndexes[0] != -1)) && ((_columnIndexes.Length != 1) || (_columnIndexes[0] != -1)))
            {
                double viewportWidth = _sheetView.GetViewportWidth(_viewport.ColumnViewportIndex);
                SolidColorBrush brush = new SolidColorBrush(gridBorderLine.Color);
                if ((rightColumnLayout != null) && (viewportWidth > ((rightColumnLayout.X + rightColumnLayout.Width) - viewportLocation.X)))
                {
                    double x = rightColumnLayout.X;
                    double width = rightColumnLayout.Width;
                    double num9 = viewportLocation.X;
                }
                double num2 = viewportLocation.Y + 0.5;
                for (int k = 0; k < _rowIndexes.Length; k++)
                {
                    RowLayout layout = rowLayoutModel.FindRow(_rowIndexes[k]);
                    if (layout != null)
                    {
                        Windows.UI.Xaml.Shapes.Line line = new Windows.UI.Xaml.Shapes.Line();
                        line.X1 = 0.0;
                        line.Y1 = (layout.Y + layout.Height) - num2;
                        line.X2 = viewportWidth;
                        line.Y2 = line.Y1;
                        line.Stroke = brush;
                        line.StrokeThickness = 1.0;
                        _scrollingGridlinesPanel.Children.Add(line);
                    }
                }
                double viewportHeight = _sheetView.GetViewportHeight(_viewport.RowViewportIndex);
                double num5 = viewportLocation.X + 0.5;
                double num6 = viewportHeight;
                if ((bottomRowLayout != null) && (viewportHeight > ((bottomRowLayout.Y + bottomRowLayout.Height) - viewportLocation.Y)))
                {
                    num6 = ((bottomRowLayout.Y + bottomRowLayout.Height) - viewportLocation.Y) - 0.5;
                }
                for (int i = 0; i < _columnIndexes.Length; i++)
                {
                    ColumnLayout layout2 = columnLayoutModel.FindColumn(_columnIndexes[i]);
                    if (layout2 != null)
                    {
                        Windows.UI.Xaml.Shapes.Line line2 = new Windows.UI.Xaml.Shapes.Line();
                        line2.X1 = (layout2.X + layout2.Width) - num5;
                        line2.Y1 = 0.0;
                        line2.X2 = line2.X1;
                        line2.Y2 = num6;
                        line2.Stroke = brush;
                        line2.StrokeThickness = 1.0;
                        _scrollingGridlinesPanel.Children.Add(line2);
                    }
                }
            }
        }

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            _sheetView = GetSheetView();
            if (_sheetView._fastScroll)
            {
                _scrollingGridlinesPanel.Measure(availableSize);
                MeasureGridLinesForScrolling();
                return base.MeasureOverride(availableSize);
            }
            _worksheet = GetWorksheet();
            _rowIndexes = new int[0];
            _columnIndexes = new int[0];
            _rowStart = 0;
            _rowEnd = 0;
            _columnStart = 0;
            _columnEnd = 0;
            _borderLinesPanel.Measure(availableSize);
            _lineMap = new Dictionary<ComboLine, LineItem>();
            _linesPool = new BorderLinesPool(_borderLinesPanel.Children);
            _linesPool.Reset();
            _zoomFactor = _sheetView.ZoomFactor;
            if (_worksheet != null)
            {
                _gridLine = _worksheet.GetGridLine(_viewport.SheetArea);
                if (((_viewport.SheetArea == SheetArea.ColumnHeader) || (_viewport.SheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader))) && _viewport.Sheet.HeaderGridLineColor.HasValue)
                {
                    _gridLine = new BorderLine(_viewport.Sheet.HeaderGridLineColor.Value, BorderLineStyle.Thin);
                }
                switch (_viewport.SheetArea)
                {
                    case SheetArea.Cells:
                    {
                        SheetSpanModel spanModel = _worksheet.SpanModel;
                        break;
                    }
                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    {
                        SheetSpanModel rowHeaderSpanModel = _worksheet.RowHeaderSpanModel;
                        break;
                    }
                    case SheetArea.ColumnHeader:
                    {
                        SheetSpanModel columnHeaderSpanModel = _worksheet.ColumnHeaderSpanModel;
                        break;
                    }
                }
                MeasureBorders(availableSize);
            }
            _linesPool.Collect();
            foreach (ComboLine line in _lineMap.Keys)
            {
                LineItem lineItem = _lineMap[line];
                Windows.Foundation.Point point = _viewport.PointToClient(new Windows.Foundation.Point(0.0, 0.0));
                line.Width = availableSize.Width;
                line.Height = availableSize.Height;
                ((IThemeContextSupport) lineItem).SetContext(_worksheet);
                line.Layout(lineItem, -point.X, -point.Y);
                ((IThemeContextSupport) lineItem).SetContext(null);
            }
            return base.MeasureOverride(availableSize);
        }

        int NextColumn(int cPos)
        {
            return NextIndex(_columnIndexes, cPos);
        }

        static int NextIndex(int[] indexes, int rPos)
        {
            if ((rPos >= -1) && (rPos < (indexes.Length - 1)))
            {
                return indexes[rPos + 1];
            }
            return -1;
        }

        int NextRow(int rPos)
        {
            return NextIndex(_rowIndexes, rPos);
        }

        int PreviousColumn(int cPos)
        {
            return PreviousIndex(_columnIndexes, cPos);
        }

        static int PreviousIndex(int[] indexes, int rPos)
        {
            if ((rPos >= 1) && (rPos <= indexes.Length))
            {
                return indexes[rPos - 1];
            }
            return -1;
        }

        int PreviousRow(int rPos)
        {
            return PreviousIndex(_rowIndexes, rPos);
        }
    }
}

