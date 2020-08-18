#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cells.UI
{
    internal class SpreadXSelection
    {
        KeyboardSelectNavigator _keyboardNavigator;
        Excel _excel;

        public SpreadXSelection(Excel p_excel)
        {
            _excel = p_excel;
            _keyboardNavigator = new KeyboardSelectNavigator(_excel);
        }

        static CellRange CellRangeUnion(CellRange range1, CellRange range2)
        {
            int row = Math.Min(range1.Row, range2.Row);
            int column = Math.Min(range1.Column, range2.Column);
            int num3 = Math.Max((int) ((range1.Row + range1.RowCount) - 1), (int) ((range2.Row + range2.RowCount) - 1));
            int num4 = Math.Max((int) ((range1.Column + range1.ColumnCount) - 1), (int) ((range2.Column + range2.ColumnCount) - 1));
            return new CellRange(row, column, (num3 - row) + 1, (num4 - column) + 1);
        }

        CellRange ExpandRange(List<CellRange> spans, CellRange range)
        {
            if ((spans != null) && (spans.Count > 0))
            {
                for (int i = 0; i < spans.Count; i++)
                {
                    CellRange range2 = spans[i];
                    if (range.Intersects(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount))
                    {
                        spans.RemoveAt(i--);
                        return ExpandRange(spans, CellRangeUnion(range, range2));
                    }
                }
            }
            return range;
        }

        CellRange GetActiveCell()
        {
            return _excel.GetActiveCell();
        }

        static void GetAdjustedEdge(int row, int column, int rowCount, int columnCount, NavigationDirection navigationDirection, bool shrink, out TabularPosition startPosition, out TabularPosition endPosition)
        {
            startPosition = TabularPosition.Empty;
            endPosition = TabularPosition.Empty;
            KeyboardSelectDirection none = KeyboardSelectDirection.None;
            switch (navigationDirection)
            {
                case NavigationDirection.Left:
                case NavigationDirection.PageLeft:
                case NavigationDirection.Home:
                    none = KeyboardSelectDirection.Left;
                    break;

                case NavigationDirection.Right:
                case NavigationDirection.PageRight:
                case NavigationDirection.End:
                    none = KeyboardSelectDirection.Right;
                    break;

                case NavigationDirection.Up:
                case NavigationDirection.PageUp:
                case NavigationDirection.Top:
                case NavigationDirection.First:
                    none = KeyboardSelectDirection.Top;
                    break;

                case NavigationDirection.Down:
                case NavigationDirection.PageDown:
                case NavigationDirection.Bottom:
                case NavigationDirection.Last:
                    none = KeyboardSelectDirection.Bottom;
                    break;
            }
            if (shrink)
            {
                switch (navigationDirection)
                {
                    case NavigationDirection.Left:
                        none = KeyboardSelectDirection.Right;
                        break;

                    case NavigationDirection.Right:
                        none = KeyboardSelectDirection.Left;
                        break;

                    case NavigationDirection.Up:
                        none = KeyboardSelectDirection.Bottom;
                        break;

                    case NavigationDirection.Down:
                        none = KeyboardSelectDirection.Top;
                        break;
                }
            }
            switch (none)
            {
                case KeyboardSelectDirection.Left:
                    startPosition = new TabularPosition(SheetArea.Cells, row, (column + columnCount) - 1);
                    endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, column);
                    return;

                case KeyboardSelectDirection.Top:
                    startPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, column);
                    endPosition = new TabularPosition(SheetArea.Cells, row, (column + columnCount) - 1);
                    return;

                case KeyboardSelectDirection.Right:
                    startPosition = new TabularPosition(SheetArea.Cells, row, column);
                    endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, (column + columnCount) - 1);
                    return;

                case KeyboardSelectDirection.Bottom:
                    startPosition = new TabularPosition(SheetArea.Cells, row, column);
                    endPosition = new TabularPosition(SheetArea.Cells, (row + rowCount) - 1, (column + columnCount) - 1);
                    return;
            }
        }

        CellRange GetExpandIntersectedRange(CellRange range)
        {
            if (_excel.ActiveSheet.SpanModel.IsEmpty())
            {
                return range;
            }
            List<CellRange> spans = new List<CellRange>();
            foreach (object obj2 in _excel.ActiveSheet.SpanModel)
            {
                spans.Add((CellRange) obj2);
            }
            return ExpandRange(spans, range);
        }

        static KeyboardSelectKind GetKeyboardSelectionKind(NavigationDirection navigationDirection)
        {
            switch (navigationDirection)
            {
                case NavigationDirection.Left:
                case NavigationDirection.Right:
                case NavigationDirection.Up:
                case NavigationDirection.Down:
                    return KeyboardSelectKind.Line;

                case NavigationDirection.PageUp:
                case NavigationDirection.PageDown:
                case NavigationDirection.PageLeft:
                case NavigationDirection.PageRight:
                    return KeyboardSelectKind.Page;

                case NavigationDirection.Home:
                case NavigationDirection.End:
                case NavigationDirection.Top:
                case NavigationDirection.Bottom:
                case NavigationDirection.First:
                case NavigationDirection.Last:
                    return KeyboardSelectKind.Through;
            }
            return KeyboardSelectKind.None;
        }

        CellRange GetSelectionRange()
        {
            if ((_excel.ActiveSheet == null) || (_excel.ActiveSheet.Selections.Count <= 0))
            {
                return null;
            }
            if (_excel.ActiveSheet.SelectionPolicy == SelectionPolicy.Single)
            {
                return null;
            }
            return _excel.GetActiveSelection();
        }

        CellRange KeyboardLineSelect(CellRange currentRange, NavigationDirection navigationDirection, bool shrink)
        {
            TabularPosition position;
            TabularPosition position2;
            TabularPosition currentCell;
            CellRange expandIntersectedRange;
            int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
            int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
            int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
            int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
            GetAdjustedEdge(row, column, rowCount, columnCount, navigationDirection, shrink, out position, out position2);
            if ((position == TabularPosition.Empty) || (position2 == TabularPosition.Empty))
            {
                return null;
            }
            _keyboardNavigator.CurrentCell = position2;
            CellRange activeCell = GetActiveCell();
            do
            {
                if (!_keyboardNavigator.MoveCurrent(navigationDirection))
                {
                    return null;
                }
                currentCell = _keyboardNavigator.CurrentCell;
                expandIntersectedRange = GetExpandIntersectedRange(TabularPositionUnion(position, currentCell));
                if (!expandIntersectedRange.Contains(activeCell))
                {
                    return null;
                }
            }
            while (expandIntersectedRange.Equals(row, column, rowCount, columnCount));
            bool flag = true;
            int viewCellRow = currentCell.Row;
            int viewCellColumn = currentCell.Column;
            int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
            int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
            int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
            int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
            int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
            int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
            if ((navigationDirection == NavigationDirection.Up) || (navigationDirection == NavigationDirection.Down))
            {
                if ((expandIntersectedRange.Column == 0) && (expandIntersectedRange.ColumnCount == _excel.ActiveSheet.ColumnCount))
                {
                    if ((currentCell.Row >= viewportTopRow) && (currentCell.Row < viewportBottomRow))
                    {
                        flag = false;
                    }
                    else
                    {
                        viewCellColumn = viewportLeftColumn;
                    }
                }
            }
            else if (((navigationDirection == NavigationDirection.Left) || (navigationDirection == NavigationDirection.Right)) && ((expandIntersectedRange.Row == 0) && (expandIntersectedRange.RowCount == _excel.ActiveSheet.RowCount)))
            {
                if ((currentCell.Column >= viewportLeftColumn) && (currentCell.Column < viewportRightColumn))
                {
                    flag = false;
                }
                else
                {
                    viewCellRow = viewportTopRow;
                }
            }
            if (flag)
            {
                NavigatorHelper.BringCellToVisible(_excel, viewCellRow, viewCellColumn);
            }
            return expandIntersectedRange;
        }

        CellRange KeyboardPageSelect(CellRange currentRange, NavigationDirection direction)
        {
            int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
            int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
            int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
            int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
            int num5 = (row + rowCount) - 1;
            int num6 = (column + columnCount) - 1;
            int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
            int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
            int num9 = _excel.ActiveSheet.RowCount;
            int num10 = _excel.ActiveSheet.ColumnCount;
            int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
            _excel.GetViewportBottomRow(activeRowViewportIndex);
            int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
            _excel.GetViewportRightColumn(activeColumnViewportIndex);
            int num13 = GetActiveCell().Row;
            int num14 = GetActiveCell().Column;
            CellRange range = null;
            if (direction == NavigationDirection.PageDown)
            {
                NavigatorHelper.ScrollToNextPageOfRows(_excel);
                int num15 = _excel.GetViewportTopRow(activeRowViewportIndex);
                int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                int num17 = num15 - viewportTopRow;
                if (num17 > 0)
                {
                    int num18 = num13;
                    int num19 = num5 + num17;
                    if (row != num13)
                    {
                        num18 = row + num17;
                        num19 = num5;
                        if (num18 >= num13)
                        {
                            num18 = num13;
                            num19 = num5 + (num17 - (num13 - row));
                        }
                    }
                    if (num19 < num15)
                    {
                        num19 = num15;
                    }
                    else if (num18 > viewportBottomRow)
                    {
                        num18 = viewportBottomRow;
                        num19 = num13;
                    }
                    else if ((num19 > viewportBottomRow) && (num13 <= viewportBottomRow))
                    {
                        num19 = viewportBottomRow;
                    }
                    return new CellRange(num18, column, (num19 - num18) + 1, columnCount);
                }
                int num20 = (num9 - row) - rowCount;
                if ((num20 > 0) && (_excel.ActiveSheet.FrozenTrailingRowCount == 0))
                {
                    int num21 = num13;
                    int num22 = num9 - 1;
                    range = new CellRange(num21, column, (num22 - num21) + 1, columnCount);
                }
                return range;
            }
            if (direction == NavigationDirection.PageUp)
            {
                NavigatorHelper.ScrollToPreviousPageOfRows(_excel);
                int num23 = _excel.GetViewportTopRow(activeRowViewportIndex);
                int num24 = _excel.GetViewportBottomRow(activeRowViewportIndex);
                int num25 = viewportTopRow - num23;
                if (num25 > 0)
                {
                    int num26 = row - num25;
                    int num27 = num5;
                    if (num5 != num13)
                    {
                        num26 = row;
                        num27 = num5 - num25;
                        if (num27 <= num13)
                        {
                            num26 = row - (num25 - (num5 - num13));
                            num27 = num13;
                        }
                    }
                    if (num27 < num23)
                    {
                        num26 = num13;
                        num27 = num23;
                    }
                    else if (num26 > num24)
                    {
                        num26 = num24;
                    }
                    else if ((num26 < num23) && (num13 >= num23))
                    {
                        num26 = num23;
                    }
                    return new CellRange(num26, column, (num27 - num26) + 1, columnCount);
                }
                if ((row > 0) && (_excel.ActiveSheet.FrozenRowCount == 0))
                {
                    int num28 = 0;
                    int num29 = num13;
                    range = new CellRange(num28, column, (num29 - num28) + 1, columnCount);
                }
                return range;
            }
            if (direction == NavigationDirection.PageRight)
            {
                NavigatorHelper.ScrollToNextPageOfColumns(_excel);
                int num30 = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                int num32 = num30 - viewportLeftColumn;
                if (num32 > 0)
                {
                    int num33 = num14;
                    int num34 = num6 + num32;
                    if (column != num14)
                    {
                        num33 = column + num32;
                        num34 = num6;
                        if (num33 >= num14)
                        {
                            num33 = num14;
                            num34 = num6 + (num32 - (num14 - column));
                        }
                    }
                    if (num34 < num30)
                    {
                        num34 = num30;
                    }
                    else if (num33 > viewportRightColumn)
                    {
                        num33 = viewportRightColumn;
                        num34 = num14;
                    }
                    else if ((num34 > viewportRightColumn) && (num14 <= viewportRightColumn))
                    {
                        num34 = viewportRightColumn;
                    }
                    return new CellRange(row, num33, rowCount, (num34 - num33) + 1);
                }
                int num35 = (num10 - column) - columnCount;
                if ((num35 > 0) && (_excel.ActiveSheet.FrozenTrailingColumnCount == 0))
                {
                    int num36 = num14;
                    int num37 = num10 - 1;
                    range = new CellRange(row, num36, rowCount, (num37 - num36) + 1);
                }
                return range;
            }
            if (direction == NavigationDirection.PageLeft)
            {
                NavigatorHelper.ScrollToPreviousPageOfColumns(_excel);
                int num38 = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                int num39 = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                int num40 = viewportLeftColumn - num38;
                if (num40 > 0)
                {
                    int num41 = column - num40;
                    int num42 = num6;
                    if (num6 != num14)
                    {
                        num41 = column;
                        num42 = num6 - num40;
                        if (num42 <= num14)
                        {
                            num41 = column - (num40 - (num6 - num14));
                            num42 = num14;
                        }
                    }
                    if (num42 < num38)
                    {
                        num41 = num14;
                        num42 = num38;
                    }
                    else if (num41 > num39)
                    {
                        num41 = num39;
                    }
                    else if ((num41 < num38) && (num14 >= num38))
                    {
                        num41 = num38;
                    }
                    return new CellRange(row, num41, rowCount, (num42 - num41) + 1);
                }
                if ((column > 0) && (_excel.ActiveSheet.FrozenColumnCount == 0))
                {
                    int num43 = 0;
                    int num44 = num14;
                    range = new CellRange(row, num43, rowCount, (num44 - num43) + 1);
                }
            }
            return range;
        }

        public void KeyboardSelect(NavigationDirection direction)
        {
            CellRange selectionRange = GetSelectionRange();
            if (selectionRange != null)
            {
                KeyboardSelectKind keyboardSelectionKind = GetKeyboardSelectionKind(direction);
                CellRange range = null;
                switch (keyboardSelectionKind)
                {
                    case KeyboardSelectKind.Line:
                        range = KeyboardLineSelect(selectionRange, direction, true);
                        if (range == null)
                        {
                            range = KeyboardLineSelect(selectionRange, direction, false);
                        }
                        break;

                    case KeyboardSelectKind.Page:
                        range = KeyboardPageSelect(selectionRange, direction);
                        break;

                    case KeyboardSelectKind.Through:
                        range = KeyboardThroughSelect(selectionRange, direction);
                        break;
                }
                if ((range != null) && !range.Equals(selectionRange))
                {
                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _excel.ActiveSheet.Selections);
                    _excel.ActiveSheet.ClearSelection(selectionRange);
                    range = GetExpandIntersectedRange(range);
                    _excel.ActiveSheet.AddSelection(range, false);
                    if (_excel.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _excel.ActiveSheet.Selections)))
                    {
                        _excel.RaiseSelectionChanged();
                    }
                }
            }
        }

        CellRange KeyboardThroughSelect(CellRange currentRange, NavigationDirection direction)
        {
            int row = (currentRange.Row < 0) ? 0 : currentRange.Row;
            int column = (currentRange.Column < 0) ? 0 : currentRange.Column;
            int rowCount = (currentRange.Row < 0) ? _excel.ActiveSheet.RowCount : currentRange.RowCount;
            int columnCount = (currentRange.Column < 0) ? _excel.ActiveSheet.ColumnCount : currentRange.ColumnCount;
            CellRange activeCell = GetActiveCell();
            CellRange range2 = null;
            if (direction == NavigationDirection.Home)
            {
                range2 = new CellRange(row, 0, rowCount, activeCell.Column + activeCell.ColumnCount);
            }
            else if (direction == NavigationDirection.End)
            {
                range2 = new CellRange(row, activeCell.Column, rowCount, _excel.ActiveSheet.ColumnCount - activeCell.Column);
            }
            else if (direction == NavigationDirection.Top)
            {
                range2 = new CellRange(0, column, activeCell.Row + activeCell.RowCount, columnCount);
            }
            else if (direction == NavigationDirection.Bottom)
            {
                range2 = new CellRange(activeCell.Row, column, _excel.ActiveSheet.RowCount - activeCell.Row, columnCount);
            }
            else if (direction == NavigationDirection.First)
            {
                range2 = new CellRange(_excel.ActiveSheet.FrozenRowCount, _excel.ActiveSheet.FrozenColumnCount, (activeCell.Row + activeCell.RowCount) - _excel.ActiveSheet.FrozenRowCount, (activeCell.Column + activeCell.ColumnCount) - _excel.ActiveSheet.FrozenColumnCount);
            }
            else if (direction == NavigationDirection.Last)
            {
                range2 = new CellRange(activeCell.Row, activeCell.Column, (_excel.ActiveSheet.RowCount - _excel.ActiveSheet.FrozenTrailingRowCount) - activeCell.Row, (_excel.ActiveSheet.ColumnCount - _excel.ActiveSheet.FrozenTrailingColumnCount) - activeCell.Column);
            }
            if (range2 != null)
            {
                int viewCellRow = range2.Row;
                int num6 = (range2.Row + range2.RowCount) - 1;
                int viewCellColumn = range2.Column;
                int num8 = (range2.Column + range2.ColumnCount) - 1;
                if ((direction == NavigationDirection.Top) || (direction == NavigationDirection.First))
                {
                    NavigatorHelper.BringCellToVisible(_excel, viewCellRow, viewCellColumn);
                    return range2;
                }
                if ((direction == NavigationDirection.Home) || (direction == NavigationDirection.End))
                {
                    int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                    int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                    int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                    if (direction == NavigationDirection.Home)
                    {
                        if (num6 < viewportTopRow)
                        {
                            NavigatorHelper.BringCellToVisible(_excel, row, viewCellColumn);
                            return range2;
                        }
                        if (viewCellRow > viewportBottomRow)
                        {
                            NavigatorHelper.BringCellToVisible(_excel, num6, viewCellColumn);
                            return range2;
                        }
                        NavigatorHelper.BringCellToVisible(_excel, viewportTopRow, viewCellColumn);
                        return range2;
                    }
                    if (num6 < viewportTopRow)
                    {
                        NavigatorHelper.BringCellToVisible(_excel, row, num8);
                        return range2;
                    }
                    if (viewCellRow > viewportBottomRow)
                    {
                        NavigatorHelper.BringCellToVisible(_excel, num6, num8);
                        return range2;
                    }
                    NavigatorHelper.BringCellToVisible(_excel, viewportTopRow, num8);
                    return range2;
                }
                if ((direction == NavigationDirection.Bottom) || (direction == NavigationDirection.Last))
                {
                    NavigatorHelper.BringCellToVisible(_excel, num6, num8);
                }
            }
            return range2;
        }

        static CellRange TabularPositionUnion(TabularPosition startPosition, TabularPosition endPosition)
        {
            int row = Math.Min(startPosition.Row, endPosition.Row);
            int column = Math.Min(startPosition.Column, endPosition.Column);
            int rowCount = Math.Abs((int) (startPosition.Row - endPosition.Row)) + 1;
            return new CellRange(row, column, rowCount, Math.Abs((int) (startPosition.Column - endPosition.Column)) + 1);
        }

        enum KeyboardSelectDirection
        {
            None,
            Left,
            Top,
            Right,
            Bottom
        }

        enum KeyboardSelectKind
        {
            None,
            Line,
            Page,
            Through
        }

        class KeyboardSelectNavigator : SpreadXTabularNavigator
        {
            public KeyboardSelectNavigator(Excel sheetView) : base(sheetView)
            {
            }

            public override void BringCellToVisible(TabularPosition position)
            {
            }

            public override bool CanMoveCurrentTo(TabularPosition cellPosition)
            {
                return (((((base._excel.ActiveSheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < base._excel.ActiveSheet.RowCount) && (cellPosition.Column >= 0))) && ((cellPosition.Column < base._excel.ActiveSheet.ColumnCount) && GetRowIsVisible(cellPosition.Row))) && GetColumnIsVisible(cellPosition.Column));
            }
        }
    }
}

