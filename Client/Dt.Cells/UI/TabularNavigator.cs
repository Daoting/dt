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
        private TabularPosition _currentCellPosition = TabularPosition.Empty;
        /// <summary>
        /// Because the Navigation action don't always start with ActiveCell, so need add another cache for the position.
        /// </summary>
        private TabularPosition _startNavigateCellPosition = TabularPosition.Empty;

        protected TabularNavigator()
        {
        }

        private void AdjustStartPosition()
        {
            if (this._currentCellPosition.IsEmpty)
            {
                this._startNavigateCellPosition = TabularPosition.Empty;
            }
            else
            {
                TabularRange range;
                if (this.IsMerged(this._currentCellPosition, out range))
                {
                    if (!range.IsContains(new TabularRange(this._startNavigateCellPosition, 1, 1)))
                    {
                        this._startNavigateCellPosition = this._currentCellPosition;
                    }
                }
                else if (this._startNavigateCellPosition != this._currentCellPosition)
                {
                    this._startNavigateCellPosition = this._currentCellPosition;
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
                return this.CanVerticalScroll(false);
            }
            if (direction == NavigationDirection.PageDown)
            {
                return this.CanVerticalScroll(true);
            }
            if (direction == NavigationDirection.PageLeft)
            {
                return this.CanHorizontalScroll(false);
            }
            if (direction == NavigationDirection.PageRight)
            {
                return this.CanHorizontalScroll(true);
            }
            return !this.PredictMoveCurrent(direction).IsEmpty;
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
            return this._startNavigateCellPosition;
        }

        public virtual bool GetRowIsVisible(int tabularRowIndex)
        {
            return true;
        }

        private bool IsColumnInViewport(TabularRange viewPort, TabularPosition position)
        {
            return ((position.Column >= viewPort.Column) && (position.Column <= viewPort.LastColumn));
        }

        public abstract bool IsMerged(TabularPosition position, out TabularRange range);
        private bool IsRowInViewport(TabularRange viewPort, TabularPosition position)
        {
            return ((position.Row >= viewPort.Row) && (position.Row <= viewPort.LastRow));
        }

        private TabularPosition MoveCellByPage(NavigationDirection direction, out TabularPosition newStartPosition)
        {
            if (direction == NavigationDirection.PageUp)
            {
                return this.MoveUpByPage(out newStartPosition);
            }
            if (direction == NavigationDirection.PageDown)
            {
                return this.MoveDownByPage(out newStartPosition);
            }
            if (direction == NavigationDirection.PageLeft)
            {
                return this.MoveLeftByPage(out newStartPosition);
            }
            if (direction != NavigationDirection.PageRight)
            {
                throw new ArgumentException("the direction must be PageDown, PageUp, PageLeft, PageRight");
            }
            return this.MoveRightByPage(out newStartPosition);
        }

        public virtual bool MoveCurrent(NavigationDirection direction)
        {
            this.AdjustStartPosition();
            if (((direction == NavigationDirection.PageUp) || (direction == NavigationDirection.PageDown)) || ((direction == NavigationDirection.PageLeft) || (direction == NavigationDirection.PageRight)))
            {
                if (this.CanMoveCurrent(direction))
                {
                    TabularPosition empty = TabularPosition.Empty;
                    TabularPosition position2 = this.MoveCellByPage(direction, out empty);
                    if (!position2.IsEmpty && (position2 != this.CurrentCell))
                    {
                        this.CurrentCell = position2;
                        this._startNavigateCellPosition = empty;
                    }
                    return true;
                }
            }
            else
            {
                TabularPosition newStartPosition = TabularPosition.Empty;
                TabularPosition position4 = this.PredictCell(direction, out newStartPosition);
                if (!position4.IsEmpty && (position4 != this.CurrentCell))
                {
                    this.CurrentCell = position4;
                    this._startNavigateCellPosition = newStartPosition;
                    return true;
                }
            }
            return false;
        }

        public virtual bool MoveCurrentTo(TabularPosition cellPosition)
        {
            if (this.CanMoveCurrentTo(cellPosition))
            {
                this.CurrentCell = cellPosition;
                return true;
            }
            return false;
        }

        private TabularPosition MoveDownByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (this.CanVerticalScroll(true))
            {
                TabularPosition position = this._startNavigateCellPosition;
                if (this.CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = this.CurrentViewport;
                bool flag = this.IsRowInViewport(currentViewport, position);
                this.ScrollToNextPageOfRows();
                TabularRange viewPort = this.CurrentViewport;
                if (this.IsRowInViewport(viewPort, position))
                {
                    return this.CurrentCell;
                }
                if (flag)
                {
                    int row = position.Row + currentViewport.RowCount;
                    if (row > this.ContentBounds.LastRow)
                    {
                        row = this.ContentBounds.LastRow;
                    }
                    while (row > position.Row)
                    {
                        TabularRange range3;
                        TabularPosition topLeft = new TabularPosition(position.Area, row, position.Column);
                        if (this.IsMerged(topLeft, out range3))
                        {
                            topLeft = range3.TopLeft;
                        }
                        if (this.CanMoveCurrentTo(topLeft))
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
                        TabularPosition position3 = new TabularPosition(this._startNavigateCellPosition.Area, i, position.Column);
                        if (this.IsMerged(position3, out range4))
                        {
                            position3 = range4.TopLeft;
                        }
                        if (this.CanMoveCurrentTo(position3))
                        {
                            newStartPosition = new TabularPosition(this._startNavigateCellPosition.Area, i, position.Column);
                            return position3;
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition MoveLeftByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (this.CanHorizontalScroll(false))
            {
                TabularPosition position = this._startNavigateCellPosition;
                if (this.CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = this.CurrentViewport;
                bool flag = this.IsColumnInViewport(currentViewport, position);
                this.ScrollToPreviousPageOfColumns();
                TabularRange viewPort = this.CurrentViewport;
                if (this.IsColumnInViewport(viewPort, position))
                {
                    return this.CurrentCell;
                }
                int column = flag ? (position.Column - currentViewport.ColumnCount) : viewPort.Column;
                if (column < this.ContentBounds.Column)
                {
                    column = this.ContentBounds.Column;
                }
                int num2 = flag ? position.Column : viewPort.LastColumn;
                while (column < num2)
                {
                    TabularRange range3;
                    TabularPosition topLeft = new TabularPosition(this._startNavigateCellPosition.Area, position.Row, column);
                    if (this.IsMerged(topLeft, out range3))
                    {
                        topLeft = range3.TopLeft;
                    }
                    if (this.CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(this._startNavigateCellPosition.Area, position.Row, column);
                        return topLeft;
                    }
                    column += range3.ColumnCount;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition MoveRightByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (this.CanHorizontalScroll(true))
            {
                TabularPosition position = this._startNavigateCellPosition;
                if (this.CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = this.CurrentViewport;
                bool flag = this.IsColumnInViewport(currentViewport, this._startNavigateCellPosition);
                this.ScrollToNextPageOfColumns();
                TabularRange viewPort = this.CurrentViewport;
                if (this.IsColumnInViewport(viewPort, position))
                {
                    return this.CurrentCell;
                }
                if (flag)
                {
                    int column = position.Column + currentViewport.ColumnCount;
                    if (column > this.ContentBounds.LastColumn)
                    {
                        column = this.ContentBounds.LastColumn;
                    }
                    while (column > position.Column)
                    {
                        TabularRange range3;
                        TabularPosition topLeft = new TabularPosition(position.Area, position.Row, column);
                        if (this.IsMerged(topLeft, out range3))
                        {
                            topLeft = range3.TopLeft;
                        }
                        if (this.CanMoveCurrentTo(topLeft))
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
                        if (this.IsMerged(position3, out range4))
                        {
                            position3 = range4.TopLeft;
                        }
                        if (this.CanMoveCurrentTo(position3))
                        {
                            newStartPosition = new TabularPosition(position.Area, position.Row, i);
                            return position3;
                        }
                    }
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition MoveUpByPage(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            if (this.CanVerticalScroll(false))
            {
                TabularPosition position = this._startNavigateCellPosition;
                if (this.CurrentCell.IsEmpty)
                {
                    position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
                }
                TabularRange currentViewport = this.CurrentViewport;
                bool flag = this.IsRowInViewport(currentViewport, position);
                this.ScrollToPreviousPageOfRows();
                TabularRange viewPort = this.CurrentViewport;
                if (this.IsRowInViewport(viewPort, position))
                {
                    return this.CurrentCell;
                }
                int row = flag ? (position.Row - currentViewport.RowCount) : viewPort.Row;
                int num2 = flag ? position.Row : viewPort.LastRow;
                if (row < this.ContentBounds.Row)
                {
                    row = this.ContentBounds.Row;
                }
                while (row < num2)
                {
                    TabularRange range3;
                    TabularPosition topLeft = new TabularPosition(this._startNavigateCellPosition.Area, row, position.Column);
                    if (this.IsMerged(topLeft, out range3))
                    {
                        topLeft = range3.TopLeft;
                    }
                    if (this.CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(this._startNavigateCellPosition.Area, row, position.Column);
                        return topLeft;
                    }
                    row += range3.RowCount;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictBottomCell(out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
            }
            int lastRow = this.ContentBounds.LastRow;
            int row = position.Row;
            if (row > lastRow)
            {
                row = this.ContentBounds.Row - 1;
            }
            for (int i = lastRow; i > row; i -= range.RowCount)
            {
                TabularPosition topLeft = new TabularPosition(position.Area, i, position.Column);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(position.Area, topLeft.Row, position.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictCell(NavigationDirection direction, out TabularPosition newStartPosition)
        {
            if (((direction == NavigationDirection.PageUp) || (direction == NavigationDirection.PageDown)) || ((direction == NavigationDirection.PageLeft) || (direction == NavigationDirection.PageRight)))
            {
                throw new NotSupportedException("Not support");
            }
            this.AdjustStartPosition();
            newStartPosition = TabularPosition.Empty;
            if ((this.TotalColumnCount != 0) && (this.TotalRowCount != 0))
            {
                switch (direction)
                {
                    case NavigationDirection.Left:
                        return this.PredictLeftCell(out newStartPosition);

                    case NavigationDirection.Right:
                        return this.PredictRightCell(out newStartPosition);

                    case NavigationDirection.Up:
                        return this.PredictUpCell(out newStartPosition);

                    case NavigationDirection.Down:
                        return this.PredictDownCell(out newStartPosition);

                    case NavigationDirection.Home:
                        return this.PredictHomeCell(out newStartPosition);

                    case NavigationDirection.End:
                        return this.PredictEndCell(out newStartPosition);

                    case NavigationDirection.Top:
                        return this.PredictTopCell(out newStartPosition);

                    case NavigationDirection.Bottom:
                        return this.PredictBottomCell(out newStartPosition);

                    case NavigationDirection.First:
                        return this.PredictFirstCell(out newStartPosition);

                    case NavigationDirection.Last:
                        return this.PredictLastCell(out newStartPosition);
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictDownCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
            }
            int row = position.Row;
            TabularPosition topLeft = position;
            while (row < (this.TotalRowCount - 1))
            {
                TabularRange range;
                if (this.IsMerged(topLeft, out range))
                {
                    row = range.Row + range.RowCount;
                    if (row > (this.TotalRowCount - 1))
                    {
                        break;
                    }
                }
                else
                {
                    row++;
                }
                topLeft = new TabularPosition(position.Area, row, position.Column);
                if (this.GetRowIsVisible(row))
                {
                    if (this.IsMerged(topLeft, out range))
                    {
                        topLeft = range.TopLeft;
                    }
                    if (this.CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(position.Area, row, position.Column);
                        return topLeft;
                    }
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictEndCell(out TabularPosition newStartPosition)
        {
            TabularPosition startPosition = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                startPosition = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
            }
            return this.PredictEndCell(startPosition, this.ContentBounds.LastColumn, out newStartPosition);
        }

        private TabularPosition PredictEndCell(TabularPosition startPosition, int rightmostColumn, out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            int column = startPosition.Column;
            if (column > rightmostColumn)
            {
                column = this.ContentBounds.Column - 1;
            }
            for (int i = rightmostColumn; i > column; i -= range.ColumnCount)
            {
                TabularPosition topLeft = new TabularPosition(startPosition.Area, startPosition.Row, i);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(startPosition.Area, startPosition.Row, topLeft.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictFirstCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            for (int i = this.ContentBounds.Row; i <= this.ContentBounds.LastRow; i++)
            {
                if (this.GetRowIsVisible(i))
                {
                    for (int j = this.ContentBounds.Column; j <= this.ContentBounds.LastColumn; j++)
                    {
                        if (this.GetColumnIsVisible(j))
                        {
                            TabularRange range;
                            TabularPosition topLeft = new TabularPosition(this._currentCellPosition.Area, i, j);
                            if (this.IsMerged(topLeft, out range))
                            {
                                topLeft = range.TopLeft;
                            }
                            if (this.CanMoveCurrentTo(topLeft))
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

        private TabularPosition PredictHomeCell(out TabularPosition newStartPosition)
        {
            TabularPosition startPosition = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                startPosition = new TabularPosition(this._startNavigateCellPosition.Area, 0, this.TotalColumnCount - 1);
            }
            return this.PredictHomeCell(startPosition, this.ContentBounds.Column, out newStartPosition);
        }

        private TabularPosition PredictHomeCell(TabularPosition startPosition, int leftmostColumn, out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            int column = startPosition.Column;
            if (column < leftmostColumn)
            {
                column = this.ContentBounds.LastColumn;
            }
            for (int i = leftmostColumn; i < column; i += range.ColumnCount)
            {
                TabularPosition topLeft = new TabularPosition(startPosition.Area, startPosition.Row, i);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(startPosition.Area, startPosition.Row, topLeft.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictLastCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            for (int i = this.ContentBounds.LastRow; i >= this.ContentBounds.Row; i--)
            {
                if (this.GetRowIsVisible(i))
                {
                    for (int j = this.ContentBounds.LastColumn; j >= this.ContentBounds.Column; j--)
                    {
                        if (this.GetColumnIsVisible(j))
                        {
                            TabularRange range;
                            TabularPosition topLeft = new TabularPosition(this._currentCellPosition.Area, i, j);
                            if (this.IsMerged(topLeft, out range))
                            {
                                topLeft = range.TopLeft;
                            }
                            if (this.CanMoveCurrentTo(topLeft))
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

        private TabularPosition PredictLeftCell(out TabularPosition newStartPosition)
        {
            if (this.CurrentCell.IsEmpty)
            {
                return this.PredictFirstCell(out newStartPosition);
            }
            newStartPosition = TabularPosition.Empty;
            int column = this._startNavigateCellPosition.Column;
            TabularPosition topLeft = this._startNavigateCellPosition;
            while (column > 0)
            {
                TabularRange range;
                if (this.IsMerged(topLeft, out range))
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
                topLeft = new TabularPosition(this._startNavigateCellPosition.Area, this._startNavigateCellPosition.Row, column);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                    for (int i = 0; i < range.ColumnCount; i++)
                    {
                        column = range.LastColumn - i;
                        if (this.GetColumnIsVisible(column))
                        {
                            break;
                        }
                    }
                }
                if (this.GetColumnIsVisible(column) && this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(this._startNavigateCellPosition.Area, this._startNavigateCellPosition.Row, column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        public virtual TabularPosition PredictMoveCurrent(NavigationDirection direction)
        {
            TabularPosition empty = TabularPosition.Empty;
            return this.PredictCell(direction, out empty);
        }

        private TabularPosition PredictRightCell(out TabularPosition newStartPosition)
        {
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                position = new TabularPosition(this._startNavigateCellPosition.Area, 0, 0);
            }
            int column = position.Column;
            TabularPosition topLeft = position;
            while (column < (this.TotalColumnCount - 1))
            {
                TabularRange range;
                if (this.IsMerged(topLeft, out range))
                {
                    column = range.Column + range.ColumnCount;
                    if (column > (this.TotalColumnCount - 1))
                    {
                        break;
                    }
                }
                else
                {
                    column++;
                }
                topLeft = new TabularPosition(position.Area, position.Row, column);
                if (this.GetColumnIsVisible(column))
                {
                    if (this.IsMerged(topLeft, out range))
                    {
                        topLeft = range.TopLeft;
                    }
                    if (this.CanMoveCurrentTo(topLeft))
                    {
                        newStartPosition = new TabularPosition(position.Area, position.Row, column);
                        return topLeft;
                    }
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictTopCell(out TabularPosition newStartPosition)
        {
            TabularRange range;
            newStartPosition = TabularPosition.Empty;
            TabularPosition position = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                position = new TabularPosition(this._startNavigateCellPosition.Area, this.TotalRowCount - 1, 0);
            }
            int row = this.ContentBounds.Row;
            int lastRow = position.Row;
            if (lastRow < row)
            {
                lastRow = this.ContentBounds.LastRow;
            }
            for (int i = row; i < lastRow; i += range.RowCount)
            {
                TabularPosition topLeft = new TabularPosition(position.Area, i, position.Column);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                }
                if (this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(position.Area, topLeft.Row, position.Column);
                    return topLeft;
                }
            }
            return TabularPosition.Empty;
        }

        private TabularPosition PredictUpCell(out TabularPosition newStartPosition)
        {
            if (this.CurrentCell.IsEmpty)
            {
                return this.PredictFirstCell(out newStartPosition);
            }
            newStartPosition = TabularPosition.Empty;
            int row = this._startNavigateCellPosition.Row;
            TabularPosition topLeft = this._startNavigateCellPosition;
            while (row > 0)
            {
                TabularRange range;
                if (this.IsMerged(topLeft, out range))
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
                topLeft = new TabularPosition(this._startNavigateCellPosition.Area, row, this._startNavigateCellPosition.Column);
                if (this.IsMerged(topLeft, out range))
                {
                    topLeft = range.TopLeft;
                    for (int i = 0; i < range.RowCount; i++)
                    {
                        row = range.LastRow - i;
                        if (this.GetRowIsVisible(row))
                        {
                            break;
                        }
                    }
                }
                if (this.GetRowIsVisible(row) && this.CanMoveCurrentTo(topLeft))
                {
                    newStartPosition = new TabularPosition(this._startNavigateCellPosition.Area, row, this._startNavigateCellPosition.Column);
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
            this._currentCellPosition = currentPosition;
            this._startNavigateCellPosition = startPosition;
        }

        public virtual TabularRange ContentBounds
        {
            get { return  new TabularRange(SheetArea.Cells, 0, 0, this.TotalRowCount, this.TotalColumnCount); }
        }

        public virtual TabularPosition CurrentCell
        {
            get { return  this._currentCellPosition; }
            set
            {
                if (this._currentCellPosition != value)
                {
                    this._currentCellPosition = value;
                    if (!value.IsEmpty)
                    {
                        this.BringCellToVisible(value);
                    }
                    this._startNavigateCellPosition = value;
                }
            }
        }

        public abstract TabularRange CurrentViewport { get; }

        public abstract int TotalColumnCount { get; }

        public abstract int TotalRowCount { get; }
    }
}

