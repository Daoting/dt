#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal abstract class TabIndexNavigator
    {
        /// <summary>
        /// This a cache for current cell position, and it is base on Composite.
        /// </summary>
        private CompositePosition _currentCellPosition = CompositePosition.Empty;
        /// <summary>
        /// When make navigation in multiple selection, need cache current range index.
        /// </summary>
        private int _currentRangeIndex = -1;
        /// <summary>
        /// Because the Navigation action don't always start with ActiveCell, so need add another cache for the position.
        /// </summary>
        private CompositePosition _startNavigateCellPosition = CompositePosition.Empty;

        protected TabIndexNavigator()
        {
        }

        private void AdjustStartPosition()
        {
            if (this._currentCellPosition.IsEmpty)
            {
                this._startNavigateCellPosition = CompositePosition.Empty;
            }
            else
            {
                CompositePosition position;
                if (this.IsMerged(this._currentCellPosition, out position))
                {
                    if (((this._startNavigateCellPosition.Column < position.Column) || (this._startNavigateCellPosition.Column > (position.Column + this.GetColumnSpan(position)))) || ((this._startNavigateCellPosition.Row < position.Row) || (this._startNavigateCellPosition.Row > (position.Row + this.GetRowSpan(position)))))
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

        public abstract void BringCellToVisible(CompositePosition position);
        public virtual bool CanMoveCurrent(NavigationDirection direction)
        {
            return !this.PredictMoveCurrent(direction).IsEmpty;
        }

        public virtual bool CanMoveCurrentTo(CompositePosition cellPosition)
        {
            return true;
        }

        public abstract List<int> GetColumnIndexes();
        public abstract List<int> GetColumnIndexes(CompositeRange range);
        public abstract int GetColumnSpan(CompositePosition position);
        private int GetCurrentPageIndex(CompositePosition position)
        {
            IList<CompositeRange> selections = this.Selections;
            for (int i = selections.Count - 1; i >= 0; i--)
            {
                CompositeRange fixedRange = this.GetFixedRange(selections[i]);
                if (((position.Column >= fixedRange.Column) && (position.Column <= fixedRange.EndColumn)) && ((position.Row >= fixedRange.Row) && (position.Row <= fixedRange.EndRow)))
                {
                    return i;
                }
            }
            return -1;
        }

        public abstract CompositeRange GetFixedRange(CompositeRange range);
        public CompositePosition GetNavigationStartPosition()
        {
            return this._startNavigateCellPosition;
        }

        private CompositePosition GetNextCellInRange(CompositeRange range, int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            CompositePosition position = this.GetNextCellInRow(rowDataIndex, columnIndexes, startColumnDataIndex, out newStartPosition);
            if (position.IsEmpty)
            {
                for (int i = rowDataIndex + 1; i <= range.EndRow; i++)
                {
                    position = this.GetNextCellInRow(i, columnIndexes, -1, out newStartPosition);
                    if (!position.IsEmpty)
                    {
                        return position;
                    }
                }
            }
            return position;
        }

        private CompositePosition GetNextCellInRow(CompositePosition startCellPosition, bool includeStartCell, List<int> columnIndexes, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int index = columnIndexes.IndexOf(startCellPosition.Column);
            for (int i = includeStartCell ? index : (index + 1); i < columnIndexes.Count; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(startCellPosition.Type, startCellPosition.Row, columnIndexes[i]);
                if (this.IsMerged(position, out position2))
                {
                    if (position.Column != position2.Column)
                    {
                        continue;
                    }
                    if (this.CanMoveCurrentTo(position2))
                    {
                        if (position2 == this.CurrentCell)
                        {
                            continue;
                        }
                        newStartPosition = position;
                        return position2;
                    }
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition GetNextCellInRow(int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            for (int i = columnIndexes.IndexOf(startColumnDataIndex) + 1; i < columnIndexes.Count; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(this.CurrentCell.Type, rowDataIndex, columnIndexes[i]);
                if ((!this.IsMerged(position, out position2) || ((position.Row == position2.Row) && (position.Column == position2.Column))) && this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition GetPreviousCellInRange(CompositeRange range, int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            CompositePosition position = this.GetPreviousCellInRow(rowDataIndex, columnIndexes, startColumnDataIndex, out newStartPosition);
            if (position.IsEmpty)
            {
                for (int i = rowDataIndex - 1; i >= range.Row; i--)
                {
                    position = this.GetPreviousCellInRow(i, columnIndexes, this.CompositeColumnCount, out newStartPosition);
                    if (!position.IsEmpty)
                    {
                        return position;
                    }
                }
            }
            return position;
        }

        private CompositePosition GetPreviousCellInRow(CompositePosition startCellPosition, bool includeStartCell, List<int> columnIndexes, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int index = columnIndexes.IndexOf(startCellPosition.Column);
            for (int i = includeStartCell ? index : (index - 1); i >= 0; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(startCellPosition.Type, startCellPosition.Row, columnIndexes[i]);
                if (this.IsMerged(position, out position2))
                {
                    if (position.Column != position2.Column)
                    {
                        continue;
                    }
                    if (this.CanMoveCurrentTo(position2))
                    {
                        if (position2 == this.CurrentCell)
                        {
                            continue;
                        }
                        newStartPosition = position;
                        return position2;
                    }
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition GetPreviousCellInRow(int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int index = columnIndexes.IndexOf(startColumnDataIndex);
            if (index == -1)
            {
                index = columnIndexes.Count;
            }
            for (int i = index - 1; i >= 0; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(this.CurrentCell.Type, rowDataIndex, columnIndexes[i]);
                if ((!this.IsMerged(position, out position2) || ((position.Row == position2.Row) && (position.Column == position2.Column))) && this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        public abstract int GetRowSpan(CompositePosition position);
        public abstract CompositePosition GetTopLeft(CompositeRange range);
        public abstract bool IsMerged(CompositePosition position, out CompositePosition topLeftPosition);
        public virtual bool MoveCurrent(NavigationDirection direction)
        {
            CompositePosition empty = CompositePosition.Empty;
            CompositePosition newStartPosition = CompositePosition.Empty;
            int rangeIndex = -1;
            this.AdjustStartPosition();
            switch (direction)
            {
                case NavigationDirection.Previous:
                    empty = this.PredictPreviousCell(out newStartPosition);
                    break;

                case NavigationDirection.Next:
                    empty = this.PredictNextCell(out newStartPosition);
                    break;

                case NavigationDirection.PreviousInSelection:
                    empty = this.PredictPreviousCellInSelection(out newStartPosition, out rangeIndex);
                    break;

                case NavigationDirection.NextInSelection:
                    empty = this.PredictNextCellInSelection(out newStartPosition, out rangeIndex);
                    break;

                case NavigationDirection.FirstRow:
                    empty = this.PredictFirstRowCell(out newStartPosition);
                    break;

                case NavigationDirection.LastRow:
                    empty = this.PredictLastRowCell(out newStartPosition);
                    break;

                case NavigationDirection.NextRow:
                    empty = this.PredictNextRowCell(out newStartPosition);
                    break;

                case NavigationDirection.PreviousRow:
                    empty = this.PredictPreviousRowCell(out newStartPosition);
                    break;
            }
            if (empty.IsEmpty || (!(empty != this.CurrentCell) && (!(empty == this.CurrentCell) || (this._currentRangeIndex == rangeIndex))))
            {
                return false;
            }
            this.CurrentCell = empty;
            this._startNavigateCellPosition = newStartPosition;
            this._currentRangeIndex = rangeIndex;
            return true;
        }

        public virtual void OnSelectionChanged(object sender, EventArgs e)
        {
            this._currentRangeIndex = -1;
        }

        private CompositePosition PredictFirstRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = this._startNavigateCellPosition.IsEmpty ? 0 : this._startNavigateCellPosition.Column;
            int num2 = this._startNavigateCellPosition.IsEmpty ? (this.ContentBounds.EndRow + 1) : this._startNavigateCellPosition.Row;
            for (int i = this.ContentBounds.StartRow; i < num2; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (this.IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition PredictLastRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = this._startNavigateCellPosition.IsEmpty ? 0 : this._startNavigateCellPosition.Column;
            int num2 = this._startNavigateCellPosition.IsEmpty ? (this.ContentBounds.StartRow - 1) : this._startNavigateCellPosition.Row;
            for (int i = this.ContentBounds.EndRow; i > num2; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (this.IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        public virtual CompositePosition PredictMoveCurrent(NavigationDirection direction)
        {
            if ((this.CompositeColumnCount != 0) && (this.CompositeRowCount != 0))
            {
                this.AdjustStartPosition();
                CompositePosition empty = CompositePosition.Empty;
                int rangeIndex = -1;
                switch (direction)
                {
                    case NavigationDirection.Previous:
                        return this.PredictPreviousCell(out empty);

                    case NavigationDirection.Next:
                        return this.PredictNextCell(out empty);

                    case NavigationDirection.PreviousInSelection:
                        return this.PredictPreviousCellInSelection(out empty, out rangeIndex);

                    case NavigationDirection.NextInSelection:
                        return this.PredictNextCellInSelection(out empty, out rangeIndex);

                    case NavigationDirection.FirstRow:
                        return this.PredictFirstRowCell(out empty);

                    case NavigationDirection.LastRow:
                        return this.PredictLastRowCell(out empty);

                    case NavigationDirection.NextRow:
                        return this.PredictNextRowCell(out empty);

                    case NavigationDirection.PreviousRow:
                        return this.PredictPreviousRowCell(out empty);
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition PredictNextCell(out CompositePosition newStartPosition)
        {
            CompositePosition startCellPosition = this._startNavigateCellPosition;
            if (this.CurrentCell.IsEmpty)
            {
                startCellPosition = new CompositePosition(DataSheetElementType.Cell, 0, 0);
            }
            List<int> columnIndexes = this.GetColumnIndexes();
            CompositePosition position2 = this.GetNextCellInRow(startCellPosition, false, columnIndexes, out newStartPosition);
            if (position2.IsEmpty)
            {
                for (int i = startCellPosition.Row + 1; i < this.CompositeRowCount; i++)
                {
                    position2 = this.GetNextCellInRow(new CompositePosition(startCellPosition.Type, i, columnIndexes[0]), true, columnIndexes, out newStartPosition);
                    if (!position2.IsEmpty)
                    {
                        return position2;
                    }
                }
            }
            return position2;
        }

        private CompositePosition PredictNextCellInSelection(out CompositePosition newStartPosition, out int rangeIndex)
        {
            rangeIndex = this._currentRangeIndex;
            if (rangeIndex == -1)
            {
                rangeIndex = this.GetCurrentPageIndex(this.CurrentCell);
            }
            if (rangeIndex < 0)
            {
                newStartPosition = this.CurrentCell;
                return CompositePosition.Empty;
            }
            List<CompositeRange> list = new List<CompositeRange>();
            for (int i = 0; i < this.Selections.Count; i++)
            {
                list.Add(this.GetFixedRange(this.Selections[i]));
            }
            CompositePosition empty = CompositePosition.Empty;
            List<int> columnIndexes = this.GetColumnIndexes(list[rangeIndex]);
            empty = this.GetNextCellInRange(list[rangeIndex], this.CurrentCell.Row, columnIndexes, this.CurrentCell.Column, out newStartPosition);
            if (empty.IsEmpty)
            {
                for (int j = rangeIndex + 1; j < list.Count; j++)
                {
                    columnIndexes = this.GetColumnIndexes(list[j]);
                    empty = this.GetNextCellInRange(list[j], list[j].Row, columnIndexes, -1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = j;
                        return empty;
                    }
                }
                for (int k = 0; k < list.Count; k++)
                {
                    columnIndexes = this.GetColumnIndexes(list[k]);
                    empty = this.GetNextCellInRange(list[k], list[k].Row, columnIndexes, -1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = k;
                        return empty;
                    }
                }
            }
            return empty;
        }

        private CompositePosition PredictNextRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = this._startNavigateCellPosition.IsEmpty ? 0 : this._startNavigateCellPosition.Column;
            int num2 = this._startNavigateCellPosition.IsEmpty ? this.ContentBounds.StartRow : (this._startNavigateCellPosition.Row + 1);
            for (int i = num2; i <= this.ContentBounds.EndRow; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (this.IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        private CompositePosition PredictPreviousCell(out CompositePosition newStartPosition)
        {
            if (this.CurrentCell.IsEmpty)
            {
                return this.PredictNextCell(out newStartPosition);
            }
            List<int> columnIndexes = this.GetColumnIndexes();
            CompositePosition startCellPosition = this._startNavigateCellPosition;
            CompositePosition position2 = this.GetPreviousCellInRow(startCellPosition, false, columnIndexes, out newStartPosition);
            if (position2.IsEmpty)
            {
                for (int i = startCellPosition.Row - 1; i >= 0; i--)
                {
                    position2 = this.GetPreviousCellInRow(new CompositePosition(startCellPosition.Type, i, columnIndexes[columnIndexes.Count - 1]), true, columnIndexes, out newStartPosition);
                    if (!position2.IsEmpty)
                    {
                        return position2;
                    }
                }
            }
            return position2;
        }

        private CompositePosition PredictPreviousCellInSelection(out CompositePosition newStartPosition, out int rangeIndex)
        {
            rangeIndex = this._currentRangeIndex;
            if (rangeIndex == -1)
            {
                rangeIndex = this.GetCurrentPageIndex(this.CurrentCell);
            }
            if (rangeIndex < 0)
            {
                newStartPosition = this.CurrentCell;
                return CompositePosition.Empty;
            }
            List<CompositeRange> list = new List<CompositeRange>();
            for (int i = 0; i < this.Selections.Count; i++)
            {
                list.Add(this.GetFixedRange(this.Selections[i]));
            }
            CompositePosition empty = CompositePosition.Empty;
            List<int> columnIndexes = this.GetColumnIndexes(list[rangeIndex]);
            empty = this.GetPreviousCellInRange(list[rangeIndex], this.CurrentCell.Row, columnIndexes, this.CurrentCell.Column, out newStartPosition);
            if (empty.IsEmpty)
            {
                for (int j = rangeIndex - 1; j >= 0; j--)
                {
                    columnIndexes = this.GetColumnIndexes(list[j]);
                    empty = this.GetPreviousCellInRange(list[j], list[j].EndRow, columnIndexes, list[j].EndColumn + 1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = j;
                        return empty;
                    }
                }
                for (int k = list.Count - 1; k >= 0; k--)
                {
                    columnIndexes = this.GetColumnIndexes(list[k]);
                    empty = this.GetPreviousCellInRange(list[k], list[k].EndRow, columnIndexes, list[k].EndColumn + 1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = k;
                        return empty;
                    }
                }
            }
            return empty;
        }

        private CompositePosition PredictPreviousRowCell(out CompositePosition newStartPosition)
        {
            if (this.CurrentCell.IsEmpty)
            {
                return this.PredictNextRowCell(out newStartPosition);
            }
            newStartPosition = CompositePosition.Empty;
            int column = this._startNavigateCellPosition.IsEmpty ? 0 : this._startNavigateCellPosition.Column;
            int num2 = this._startNavigateCellPosition.IsEmpty ? this.ContentBounds.StartRow : (this._startNavigateCellPosition.Row - 1);
            for (int i = num2; i >= this.ContentBounds.StartRow; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (this.IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (this.CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        public virtual bool ShouldNavigateInSelection()
        {
            if (this.Selections.Count == 0)
            {
                return false;
            }
            if (this.Selections.Count <= 1)
            {
                CompositeRange fixedRange = this.GetFixedRange(this.Selections[0]);
                CompositePosition topLeft = this.GetTopLeft(fixedRange);
                if (fixedRange.RowCount <= this.GetRowSpan(topLeft))
                {
                    return (fixedRange.ColumnCount > this.GetColumnSpan(topLeft));
                }
            }
            return true;
        }

        public void UpdateNavigationStartPosition(CompositePosition currentPosition, CompositePosition startPosition)
        {
            this._currentCellPosition = currentPosition;
            if (startPosition.IsEmpty && !currentPosition.IsEmpty)
            {
                this._startNavigateCellPosition = currentPosition;
            }
            else
            {
                this._startNavigateCellPosition = startPosition;
            }
        }

        public abstract int CompositeColumnCount { get; }

        public abstract int CompositeRowCount { get; }

        public virtual CompositeRange ContentBounds
        {
            get { return  new CompositeRange(DataSheetElementType.Cell, 0, 0, this.CompositeRowCount, this.CompositeColumnCount); }
        }

        public virtual CompositePosition CurrentCell
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

        public CompositeRange CurrentRange
        {
            get
            {
                IList<CompositeRange> selections = this.Selections;
                if ((this._currentRangeIndex >= 0) && (this._currentRangeIndex < selections.Count))
                {
                    return selections[this._currentRangeIndex];
                }
                if (!this.CurrentCell.IsEmpty)
                {
                    for (int i = selections.Count - 1; i >= 0; i--)
                    {
                        if (((selections[i].Row == -1) || ((this.CurrentCell.Row >= selections[i].Row) && (this.CurrentCell.Row <= selections[i].EndRow))) && ((selections[i].Column == -1) || ((this.CurrentCell.Column >= selections[i].Column) && (this.CurrentCell.Column <= selections[i].EndColumn))))
                        {
                            return selections[i];
                        }
                    }
                }
                return CompositeRange.Empty;
            }
        }

        public abstract IList<CompositeRange> Selections { get; }
    }
}

