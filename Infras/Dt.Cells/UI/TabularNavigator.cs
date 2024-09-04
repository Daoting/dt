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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal abstract class TabularNavigator
    {
        /// <summary>
        /// This a cache for current cell position, and it is base on Tabular.
        /// </summary>
        TabularPosition _currentCellPosition = TabularPosition.Empty;
        /// <summary>
        /// Because the Navigation action don't always start with ActiveCell, so need add another cache for the position.
        /// </summary>
        TabularPosition _startNavigateCellPosition = TabularPosition.Empty;

        protected TabularNavigator()
        {
        }

        void AdjustStartPosition()
        {
            if (_currentCellPosition.IsEmpty)
            {
                _startNavigateCellPosition = TabularPosition.Empty;
            }
            else
            {
                TabularRange range;
                if (IsMerged(_currentCellPosition, out range))
                {
                    if (!range.IsContains(new TabularRange(_startNavigateCellPosition, 1, 1)))
                    {
                        _startNavigateCellPosition = _currentCellPosition;
                    }
                }
                else if (_startNavigateCellPosition != _currentCellPosition)
                {
                    _startNavigateCellPosition = _currentCellPosition;
                }
            }
        }

        public abstract void BringCellToVisible(TabularPosition position);
        public virtual bool CanHorizontalScroll(bool isBackward)
        {
            return true;
        }

        public virtual bool CanMoveCurrent(NavigationDirection direction)
        {
            if (direction == NavigationDirection.PageUp)
            {
                return CanVerticalScroll(false);
            }
            if (direction == NavigationDirection.PageDown)
            {
                return CanVerticalScroll(true);
            }
            if (direction == NavigationDirection.PageLeft)
            {
                return CanHorizontalScroll(false);
            }
            if (direction == NavigationDirection.PageRight)
            {
                return CanHorizontalScroll(true);
            }
            return !PredictMoveCurrent(direction).IsEmpty;
        }

        public virtual bool CanMoveCurrentTo(TabularPosition cellPosition)
        {
            return true;
        }

        public virtual bool CanVerticalScroll(bool isBackward)
        {
            return true;
        }

        public virtual bool GetColumnIsVisible(int tabularColumnIndex)
        {
            return true;
        }

        public TabularPosition GetNavigationStartPosition()
        {
            return _startNavigateCellPosition;
        }

        public virtual bool GetRowIsVisible(int tabularRowIndex)
        {
            return true;
        }

        bool IsColumnInViewport(TabularRange viewPort, TabularPosition position)
        {
            return ((position.Column >= viewPort.Column) && (position.Column <= viewPort.LastColumn));
        }

        public abstract bool IsMerged(TabularPosition position, out TabularRange range);
        bool IsRowInViewport(TabularRange viewPort, TabularPosition position)
        {
            return ((position.Row >= viewPort.Row) && (position.Row <= viewPort.LastRow));
        }

        TabularPosition MoveCellByPage(NavigationDirection direction, out TabularPosition newStartPosition)
        {
            if (direction == NavigationDirection.PageUp)
            {
                return MoveUpByPage(out newStartPosition);
            }
            if (direction == NavigationDirection.PageDown)
            {
                return MoveDownByPage(out newStartPosition);
            }
            if (direction == NavigationDirection.PageLeft)
            {
                return MoveLeftByPage(out newStartPosition);
            }
            if (direction != NavigationDirection.PageRight)
            {
                throw new ArgumentException("the direction must be PageDown, PageUp, PageLeft, PageRight");
            }
            return MoveRightByPage(out newStartPosition);
        }

        public virtual bool MoveCurrent(NavigationDirection direction)
        {
            AdjustStartPosition();
            if (((direction == NavigationDirection.PageUp) || (direction == NavigationDirection.PageDown)) || ((direction == NavigationDirection.PageLeft) || (direction == NavigationDirection.PageRight)))
            {
                if (CanMoveCurrent(direction))
                {
                    TabularPosition empty = TabularPosition.Empty;
                    TabularPosition position2 = MoveCellByPage(direction, out empty);
                    if (!position2.IsEmpty && (position2 != CurrentCell))
                    {
                        CurrentCell = position2;
                        _startNavigateCellPosition = empty;
                    }
                    return true;
                }
            }
            else
            {
                TabularPosition newStartPosition = TabularPosition.Empty;
                TabularPosition position4 = PredictCell(direction, out newStartPosition);
                if (!position4.IsEmpty && (position4 != CurrentCell))
                {
                    CurrentCell = position4;
                    _startNavigateCellPosition = newStartPosition;
                    return true;
                }
            }
            return false;
        }

        public virtual bool MoveCurrentTo(TabularPosition cellPosition)
        {
            if (CanMoveCurrentTo(cellPosition))
            {
                CurrentCell = cellPosition;
                return true;
            }
            return false;
        }

        TabularPosition MoveDownByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (CanVerticalScroll(true))
            {
                TabularPosition position = _startNavigateCellPosition;
                if (CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = CurrentViewport;
                bool flag = IsRowInViewport(currentViewport, position);
                ScrollToNextPageOfRows();
                TabularRange viewPort = CurrentViewport;
                if (IsRowInViewport(viewPort, position))
                {
                    return CurrentCell;
                }
                if (flag)
                {
                    int row = position.Row + currentViewport.RowCount;
                    if (row > ContentBounds.LastRow)
                    {
                        row = ContentBounds.LastRow;
                    }
                    while (row > position.Row)
                    {
                        TabularRange range3;
                        TabularPosition topLeft = new TabularPosition(position.Area, row, position.Column);
                        if (IsMerged(topLeft, out range3))
                        {
                            topLeft = range3.TopLeft;
                        }
                        if (CanMoveCurrentTo(topLeft))
                        {
                            newStartPosition = new TabularPosition(position.Area, row, position.Column);
                            return topLeft;
                        }
                        row -= range3.RowCount;
                    }
                }
                else
                {
                    TabularRange range4;
                    for (int i = viewPort.Row; i < viewPort.LastRow; i += range4.RowCount)
                    {
                        TabularPosition position3 = new TabularPosition(_startNavigateCellPosition.Area, i, position.Column);
                        if (IsMerged(position3, out range4))
                        {
                            position3 = range4.TopLeft;
                        }
                        if (CanMoveCurrentTo(position3))
                        {
                            newStartPosition = new TabularPosition(_startNavigateCellPosition.Area, i, position.Column);
                            return position3;
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition MoveLeftByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (CanHorizontalScroll(false))
            {
                TabularPosition position = _startNavigateCellPosition;
                if (CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = CurrentViewport;
                bool flag = IsColumnInViewport(currentViewport, position);
                ScrollToPreviousPageOfColumns();
                TabularRange viewPort = CurrentViewport;
                if (IsColumnInViewport(viewPort, position))
                {
                    return CurrentCell;
                }
                int column = flag ? (position.Column - currentViewport.ColumnCount) : viewPort.Column;
                if (column < ContentBounds.Column)
                {
                    column = ContentBounds.Column;
                }
                int num2 = flag ? position.Column : viewPort.LastColumn;
                while (column < num2)
                {
                    TabularRange range3;
                    TabularPosition topLeft = new TabularPosition(_startNavigateCellPosition.Area, position.Row, column);
                    if (IsMerged(topLeft, out range3))
                    {
                        topLeft = range3.TopLeft;
                    }
                    if (CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(_startNavigateCellPosition.Area, position.Row, column);
                        return topLeft;
                    }
                    column += range3.ColumnCount;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition MoveRightByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (CanHorizontalScroll(true))
            {
                TabularPosition position = _startNavigateCellPosition;
                if (CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = CurrentViewport;
                bool flag = IsColumnInViewport(currentViewport, _startNavigateCellPosition);
                ScrollToNextPageOfColumns();
                TabularRange viewPort = CurrentViewport;
                if (IsColumnInViewport(viewPort, position))
                {
                    return CurrentCell;
                }
                if (flag)
                {
                    int column = position.Column + currentViewport.ColumnCount;
                    if (column > ContentBounds.LastColumn)
                    {
                        column = ContentBounds.LastColumn;
                    }
                    while (column > position.Column)
                    {
                        TabularRange range3;
                        TabularPosition topLeft = new TabularPosition(position.Area, position.Row, column);
                        if (IsMerged(topLeft, out range3))
                        {
                            topLeft = range3.TopLeft;
                        }
                        if (CanMoveCurrentTo(topLeft))
                        {
                            newStartPosition = new TabularPosition(position.Area, position.Row, column);
                            return topLeft;
                        }
                        column -= range3.ColumnCount;
                    }
                }
                else
                {
                    TabularRange range4;
                    for (int i = viewPort.Column; i < viewPort.LastColumn; i += range4.ColumnCount)
                    {
                        TabularPosition position3 = new TabularPosition(position.Area, position.Row, i);
                        if (IsMerged(position3, out range4))
                        {
                            position3 = range4.TopLeft;
                        }
                        if (CanMoveCurrentTo(position3))
                        {
                            newStartPosition = new TabularPosition(position.Area, position.Row, i);
                            return position3;
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition MoveUpByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (CanVerticalScroll(false))
            {
                TabularPosition position = _startNavigateCellPosition;
                if (CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = CurrentViewport;
                bool flag = IsRowInViewport(currentViewport, position);
                ScrollToPreviousPageOfRows();
                TabularRange viewPort = CurrentViewport;
                if (IsRowInViewport(viewPort, position))
                {
                    return CurrentCell;
                }
                int row = flag ? (position.Row - currentViewport.RowCount) : viewPort.Row;
                int num2 = flag ? position.Row : viewPort.LastRow;
                if (row < ContentBounds.Row)
                {
                    row = ContentBounds.Row;
                }
                while (row < num2)
                {
                    TabularRange range3;
                    TabularPosition topLeft = new TabularPosition(_startNavigateCellPosition.Area, row, position.Column);
                    if (IsMerged(topLeft, out range3))
                    {
                        topLeft = range3.TopLeft;
                    }
                    if (CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(_startNavigateCellPosition.Area, row, position.Column);
                        return topLeft;
                    }
                    row += range3.RowCount;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictBottomCell(out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
            }
            int lastRow = ContentBounds.LastRow;
            int row = position.Row;
            if (row > lastRow)
            {
                row = ContentBounds.Row - 1;
            }
            for (int i = lastRow; i > row; i -= range.RowCount)
            {
                TabularPosition topLeft = new TabularPosition(position.Area, i, position.Column);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(position.Area, topLeft.Row, position.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictCell(NavigationDirection direction, out TabularPosition newStartPosition)
        {
            if (((direction == NavigationDirection.PageUp) || (direction == NavigationDirection.PageDown)) || ((direction == NavigationDirection.PageLeft) || (direction == NavigationDirection.PageRight)))
            {
                throw new NotSupportedException("Not support");
            }
            AdjustStartPosition();
            newStartPosition = TabularPosition.Empty;
            if ((TotalColumnCount != 0) && (TotalRowCount != 0))
            {
                switch (direction)
                {
                    case NavigationDirection.Left:
                        return PredictLeftCell(out newStartPosition);

                    case NavigationDirection.Right:
                        return PredictRightCell(out newStartPosition);

                    case NavigationDirection.Up:
                        return PredictUpCell(out newStartPosition);

                    case NavigationDirection.Down:
                        return PredictDownCell(out newStartPosition);

                    case NavigationDirection.Home:
                        return PredictHomeCell(out newStartPosition);

                    case NavigationDirection.End:
                        return PredictEndCell(out newStartPosition);

                    case NavigationDirection.Top:
                        return PredictTopCell(out newStartPosition);

                    case NavigationDirection.Bottom:
                        return PredictBottomCell(out newStartPosition);

                    case NavigationDirection.First:
                        return PredictFirstCell(out newStartPosition);

                    case NavigationDirection.Last:
                        return PredictLastCell(out newStartPosition);
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictDownCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
            }
            int row = position.Row;
            TabularPosition topLeft = position;
            while (row < (TotalRowCount - 1))
            {
                TabularRange range;
                if (IsMerged(topLeft, out range))
                {
                    row = range.Row + range.RowCount;
                    if (row > (TotalRowCount - 1))
                    {
                        break;
                    }
                }
                else
                {
                    row++;
                }
                topLeft = new TabularPosition(position.Area, row, position.Column);
                if (GetRowIsVisible(row))
                {
                    if (IsMerged(topLeft, out range))
                    {
                        topLeft = range.TopLeft;
                    }
                    if (CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(position.Area, row, position.Column);
                        return topLeft;
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictEndCell(out TabularPosition newStartPosition)
        {
            TabularPosition startPosition = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                startPosition = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
            }
            return PredictEndCell(startPosition, ContentBounds.LastColumn, out newStartPosition);
        }

        TabularPosition PredictEndCell(TabularPosition startPosition, int rightmostColumn, out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            int column = startPosition.Column;
            if (column > rightmostColumn)
            {
                column = ContentBounds.Column - 1;
            }
            for (int i = rightmostColumn; i > column; i -= range.ColumnCount)
            {
                TabularPosition topLeft = new TabularPosition(startPosition.Area, startPosition.Row, i);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(startPosition.Area, startPosition.Row, topLeft.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictFirstCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            for (int i = ContentBounds.Row; i <= ContentBounds.LastRow; i++)
            {
                if (GetRowIsVisible(i))
                {
                    for (int j = ContentBounds.Column; j <= ContentBounds.LastColumn; j++)
                    {
                        if (GetColumnIsVisible(j))
                        {
                            TabularRange range;
                            TabularPosition topLeft = new TabularPosition(_currentCellPosition.Area, i, j);
                            if (IsMerged(topLeft, out range))
                            {
                                topLeft = range.TopLeft;
                            }
                            if (CanMoveCurrentTo(topLeft))
                            {
                                newStartPosition = topLeft;
                                return topLeft;
                            }
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictHomeCell(out TabularPosition newStartPosition)
        {
            TabularPosition startPosition = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                startPosition = new TabularPosition(_startNavigateCellPosition.Area, 0, TotalColumnCount - 1);
            }
            return PredictHomeCell(startPosition, ContentBounds.Column, out newStartPosition);
        }

        TabularPosition PredictHomeCell(TabularPosition startPosition, int leftmostColumn, out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            int column = startPosition.Column;
            if (column < leftmostColumn)
            {
                column = ContentBounds.LastColumn;
            }
            for (int i = leftmostColumn; i < column; i += range.ColumnCount)
            {
                TabularPosition topLeft = new TabularPosition(startPosition.Area, startPosition.Row, i);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(startPosition.Area, startPosition.Row, topLeft.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictLastCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            for (int i = ContentBounds.LastRow; i >= ContentBounds.Row; i--)
            {
                if (GetRowIsVisible(i))
                {
                    for (int j = ContentBounds.LastColumn; j >= ContentBounds.Column; j--)
                    {
                        if (GetColumnIsVisible(j))
                        {
                            TabularRange range;
                            TabularPosition topLeft = new TabularPosition(_currentCellPosition.Area, i, j);
                            if (IsMerged(topLeft, out range))
                            {
                                topLeft = range.TopLeft;
                            }
                            if (CanMoveCurrentTo(topLeft))
                            {
                                newStartPosition = topLeft;
                                return topLeft;
                            }
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictLeftCell(out TabularPosition newStartPosition)
        {
            if (CurrentCell.IsEmpty)
            {
                return PredictFirstCell(out newStartPosition);
            }
            newStartPosition = TabularPosition.Empty;
            int column = _startNavigateCellPosition.Column;
            TabularPosition topLeft = _startNavigateCellPosition;
            while (column > 0)
            {
                TabularRange range;
                if (IsMerged(topLeft, out range))
                {
                    column = range.Column - 1;
                    if (column < 0)
                    {
                        break;
                    }
                }
                else
                {
                    column--;
                }
                topLeft = new TabularPosition(_startNavigateCellPosition.Area, _startNavigateCellPosition.Row, column);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                    for (int i = 0; i < range.ColumnCount; i++)
                    {
                        column = range.LastColumn - i;
                        if (GetColumnIsVisible(column))
                        {
                            break;
                        }
                    }
                }
                if (GetColumnIsVisible(column) && CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(_startNavigateCellPosition.Area, _startNavigateCellPosition.Row, column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        public virtual TabularPosition PredictMoveCurrent(NavigationDirection direction)
        {
            TabularPosition empty = TabularPosition.Empty;
            return PredictCell(direction, out empty);
        }

        TabularPosition PredictRightCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                position = new TabularPosition(_startNavigateCellPosition.Area, 0, 0);
            }
            int column = position.Column;
            TabularPosition topLeft = position;
            while (column < (TotalColumnCount - 1))
            {
                TabularRange range;
                if (IsMerged(topLeft, out range))
                {
                    column = range.Column + range.ColumnCount;
                    if (column > (TotalColumnCount - 1))
                    {
                        break;
                    }
                }
                else
                {
                    column++;
                }
                topLeft = new TabularPosition(position.Area, position.Row, column);
                if (GetColumnIsVisible(column))
                {
                    if (IsMerged(topLeft, out range))
                    {
                        topLeft = range.TopLeft;
                    }
                    if (CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(position.Area, position.Row, column);
                        return topLeft;
                    }
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictTopCell(out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                position = new TabularPosition(_startNavigateCellPosition.Area, TotalRowCount - 1, 0);
            }
            int row = ContentBounds.Row;
            int lastRow = position.Row;
            if (lastRow < row)
            {
                lastRow = ContentBounds.LastRow;
            }
            for (int i = row; i < lastRow; i += range.RowCount)
            {
                TabularPosition topLeft = new TabularPosition(position.Area, i, position.Column);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(position.Area, topLeft.Row, position.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        TabularPosition PredictUpCell(out TabularPosition newStartPosition)
        {
            if (CurrentCell.IsEmpty)
            {
                return PredictFirstCell(out newStartPosition);
            }
            newStartPosition = TabularPosition.Empty;
            int row = _startNavigateCellPosition.Row;
            TabularPosition topLeft = _startNavigateCellPosition;
            while (row > 0)
            {
                TabularRange range;
                if (IsMerged(topLeft, out range))
                {
                    row = range.Row - 1;
                    if (row < 0)
                    {
                        break;
                    }
                }
                else
                {
                    row--;
                }
                topLeft = new TabularPosition(_startNavigateCellPosition.Area, row, _startNavigateCellPosition.Column);
                if (IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        row = range.LastRow - i;
                        if (GetRowIsVisible(row))
                        {
                            break;
                        }
                    }
                }
                if (GetRowIsVisible(row) && CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(_startNavigateCellPosition.Area, row, _startNavigateCellPosition.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        public abstract void ScrollToNextPageOfColumns();
        public abstract void ScrollToNextPageOfRows();
        public abstract void ScrollToPreviousPageOfColumns();
        public abstract void ScrollToPreviousPageOfRows();
        public void UpdateNavigationStartPosition(TabularPosition currentPosition, TabularPosition startPosition)
        {
            _currentCellPosition = currentPosition;
            _startNavigateCellPosition = startPosition;
        }

        public virtual TabularRange ContentBounds
        {
            get { return  new TabularRange(SheetArea.Cells, 0, 0, TotalRowCount, TotalColumnCount); }
        }

        public virtual TabularPosition CurrentCell
        {
            get { return  _currentCellPosition; }
            set
            {
                if (_currentCellPosition != value)
                {
                    _currentCellPosition = value;
                    if (!value.IsEmpty)
                    {
                        BringCellToVisible(value);
                    }
                    _startNavigateCellPosition = value;
                }
            }
        }

        public abstract TabularRange CurrentViewport { get; }

        public abstract int TotalColumnCount { get; }

        public abstract int TotalRowCount { get; }
    }
}

