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
        CompositePosition _currentCellPosition = CompositePosition.Empty;
        /// <summary>
        /// When make navigation in multiple selection, need cache current range index.
        /// </summary>
        int _currentRangeIndex = -1;
        /// <summary>
        /// Because the Navigation action don't always start with ActiveCell, so need add another cache for the position.
        /// </summary>
        CompositePosition _startNavigateCellPosition = CompositePosition.Empty;

        protected TabIndexNavigator()
        {
        }

        void AdjustStartPosition()
        {
            if (_currentCellPosition.IsEmpty)
            {
                _startNavigateCellPosition = CompositePosition.Empty;
            }
            else
            {
                CompositePosition position;
                if (IsMerged(_currentCellPosition, out position))
                {
                    if (((_startNavigateCellPosition.Column < position.Column) || (_startNavigateCellPosition.Column > (position.Column + GetColumnSpan(position)))) || ((_startNavigateCellPosition.Row < position.Row) || (_startNavigateCellPosition.Row > (position.Row + GetRowSpan(position)))))
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

        public abstract void BringCellToVisible(CompositePosition position);
        public virtual bool CanMoveCurrent(NavigationDirection direction)
        {
            return !PredictMoveCurrent(direction).IsEmpty;
        }

        public virtual bool CanMoveCurrentTo(CompositePosition cellPosition)
        {
            return true;
        }

        public abstract List<int> GetColumnIndexes();
        public abstract List<int> GetColumnIndexes(CompositeRange range);
        public abstract int GetColumnSpan(CompositePosition position);
        int GetCurrentPageIndex(CompositePosition position)
        {
            IList<CompositeRange> selections = Selections;
            for (int i = selections.Count - 1; i >= 0; i--)
            {
                CompositeRange fixedRange = GetFixedRange(selections[i]);
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
            return _startNavigateCellPosition;
        }

        CompositePosition GetNextCellInRange(CompositeRange range, int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            CompositePosition position = GetNextCellInRow(rowDataIndex, columnIndexes, startColumnDataIndex, out newStartPosition);
            if (position.IsEmpty)
            {
                for (int i = rowDataIndex + 1; i <= range.EndRow; i++)
                {
                    position = GetNextCellInRow(i, columnIndexes, -1, out newStartPosition);
                    if (!position.IsEmpty)
                    {
                        return position;
                    }
                }
            }
            return position;
        }

        CompositePosition GetNextCellInRow(CompositePosition startCellPosition, bool includeStartCell, List<int> columnIndexes, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int index = columnIndexes.IndexOf(startCellPosition.Column);
            for (int i = includeStartCell ? index : (index + 1); i < columnIndexes.Count; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(startCellPosition.Type, startCellPosition.Row, columnIndexes[i]);
                if (IsMerged(position, out position2))
                {
                    if (position.Column != position2.Column)
                    {
                        continue;
                    }
                    if (CanMoveCurrentTo(position2))
                    {
                        if (position2 == CurrentCell)
                        {
                            continue;
                        }
                        newStartPosition = position;
                        return position2;
                    }
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition GetNextCellInRow(int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            for (int i = columnIndexes.IndexOf(startColumnDataIndex) + 1; i < columnIndexes.Count; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(CurrentCell.Type, rowDataIndex, columnIndexes[i]);
                if ((!IsMerged(position, out position2) || ((position.Row == position2.Row) && (position.Column == position2.Column))) && CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition GetPreviousCellInRange(CompositeRange range, int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
        {
            CompositePosition position = GetPreviousCellInRow(rowDataIndex, columnIndexes, startColumnDataIndex, out newStartPosition);
            if (position.IsEmpty)
            {
                for (int i = rowDataIndex - 1; i >= range.Row; i--)
                {
                    position = GetPreviousCellInRow(i, columnIndexes, CompositeColumnCount, out newStartPosition);
                    if (!position.IsEmpty)
                    {
                        return position;
                    }
                }
            }
            return position;
        }

        CompositePosition GetPreviousCellInRow(CompositePosition startCellPosition, bool includeStartCell, List<int> columnIndexes, out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int index = columnIndexes.IndexOf(startCellPosition.Column);
            for (int i = includeStartCell ? index : (index - 1); i >= 0; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(startCellPosition.Type, startCellPosition.Row, columnIndexes[i]);
                if (IsMerged(position, out position2))
                {
                    if (position.Column != position2.Column)
                    {
                        continue;
                    }
                    if (CanMoveCurrentTo(position2))
                    {
                        if (position2 == CurrentCell)
                        {
                            continue;
                        }
                        newStartPosition = position;
                        return position2;
                    }
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition GetPreviousCellInRow(int rowDataIndex, List<int> columnIndexes, int startColumnDataIndex, out CompositePosition newStartPosition)
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
                CompositePosition position = new CompositePosition(CurrentCell.Type, rowDataIndex, columnIndexes[i]);
                if ((!IsMerged(position, out position2) || ((position.Row == position2.Row) && (position.Column == position2.Column))) && CanMoveCurrentTo(position))
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
            AdjustStartPosition();
            switch (direction)
            {
                case NavigationDirection.Previous:
                    empty = PredictPreviousCell(out newStartPosition);
                    break;

                case NavigationDirection.Next:
                    empty = PredictNextCell(out newStartPosition);
                    break;

                case NavigationDirection.PreviousInSelection:
                    empty = PredictPreviousCellInSelection(out newStartPosition, out rangeIndex);
                    break;

                case NavigationDirection.NextInSelection:
                    empty = PredictNextCellInSelection(out newStartPosition, out rangeIndex);
                    break;

                case NavigationDirection.FirstRow:
                    empty = PredictFirstRowCell(out newStartPosition);
                    break;

                case NavigationDirection.LastRow:
                    empty = PredictLastRowCell(out newStartPosition);
                    break;

                case NavigationDirection.NextRow:
                    empty = PredictNextRowCell(out newStartPosition);
                    break;

                case NavigationDirection.PreviousRow:
                    empty = PredictPreviousRowCell(out newStartPosition);
                    break;
            }
            if (empty.IsEmpty || (!(empty != CurrentCell) && (!(empty == CurrentCell) || (_currentRangeIndex == rangeIndex))))
            {
                return false;
            }
            CurrentCell = empty;
            _startNavigateCellPosition = newStartPosition;
            _currentRangeIndex = rangeIndex;
            return true;
        }

        public virtual void OnSelectionChanged(object sender, EventArgs e)
        {
            _currentRangeIndex = -1;
        }

        CompositePosition PredictFirstRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = _startNavigateCellPosition.IsEmpty ? 0 : _startNavigateCellPosition.Column;
            int num2 = _startNavigateCellPosition.IsEmpty ? (ContentBounds.EndRow + 1) : _startNavigateCellPosition.Row;
            for (int i = ContentBounds.StartRow; i < num2; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition PredictLastRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = _startNavigateCellPosition.IsEmpty ? 0 : _startNavigateCellPosition.Column;
            int num2 = _startNavigateCellPosition.IsEmpty ? (ContentBounds.StartRow - 1) : _startNavigateCellPosition.Row;
            for (int i = ContentBounds.EndRow; i > num2; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        public virtual CompositePosition PredictMoveCurrent(NavigationDirection direction)
        {
            if ((CompositeColumnCount != 0) && (CompositeRowCount != 0))
            {
                AdjustStartPosition();
                CompositePosition empty = CompositePosition.Empty;
                int rangeIndex = -1;
                switch (direction)
                {
                    case NavigationDirection.Previous:
                        return PredictPreviousCell(out empty);

                    case NavigationDirection.Next:
                        return PredictNextCell(out empty);

                    case NavigationDirection.PreviousInSelection:
                        return PredictPreviousCellInSelection(out empty, out rangeIndex);

                    case NavigationDirection.NextInSelection:
                        return PredictNextCellInSelection(out empty, out rangeIndex);

                    case NavigationDirection.FirstRow:
                        return PredictFirstRowCell(out empty);

                    case NavigationDirection.LastRow:
                        return PredictLastRowCell(out empty);

                    case NavigationDirection.NextRow:
                        return PredictNextRowCell(out empty);

                    case NavigationDirection.PreviousRow:
                        return PredictPreviousRowCell(out empty);
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition PredictNextCell(out CompositePosition newStartPosition)
        {
            CompositePosition startCellPosition = _startNavigateCellPosition;
            if (CurrentCell.IsEmpty)
            {
                startCellPosition = new CompositePosition(DataSheetElementType.Cell, 0, 0);
            }
            List<int> columnIndexes = GetColumnIndexes();
            CompositePosition position2 = GetNextCellInRow(startCellPosition, false, columnIndexes, out newStartPosition);
            if (position2.IsEmpty)
            {
                for (int i = startCellPosition.Row + 1; i < CompositeRowCount; i++)
                {
                    position2 = GetNextCellInRow(new CompositePosition(startCellPosition.Type, i, columnIndexes[0]), true, columnIndexes, out newStartPosition);
                    if (!position2.IsEmpty)
                    {
                        return position2;
                    }
                }
            }
            return position2;
        }

        CompositePosition PredictNextCellInSelection(out CompositePosition newStartPosition, out int rangeIndex)
        {
            rangeIndex = _currentRangeIndex;
            if (rangeIndex == -1)
            {
                rangeIndex = GetCurrentPageIndex(CurrentCell);
            }
            if (rangeIndex < 0)
            {
                newStartPosition = CurrentCell;
                return CompositePosition.Empty;
            }
            List<CompositeRange> list = new List<CompositeRange>();
            for (int i = 0; i < Selections.Count; i++)
            {
                list.Add(GetFixedRange(Selections[i]));
            }
            CompositePosition empty = CompositePosition.Empty;
            List<int> columnIndexes = GetColumnIndexes(list[rangeIndex]);
            empty = GetNextCellInRange(list[rangeIndex], CurrentCell.Row, columnIndexes, CurrentCell.Column, out newStartPosition);
            if (empty.IsEmpty)
            {
                for (int j = rangeIndex + 1; j < list.Count; j++)
                {
                    columnIndexes = GetColumnIndexes(list[j]);
                    empty = GetNextCellInRange(list[j], list[j].Row, columnIndexes, -1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = j;
                        return empty;
                    }
                }
                for (int k = 0; k < list.Count; k++)
                {
                    columnIndexes = GetColumnIndexes(list[k]);
                    empty = GetNextCellInRange(list[k], list[k].Row, columnIndexes, -1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = k;
                        return empty;
                    }
                }
            }
            return empty;
        }

        CompositePosition PredictNextRowCell(out CompositePosition newStartPosition)
        {
            newStartPosition = CompositePosition.Empty;
            int column = _startNavigateCellPosition.IsEmpty ? 0 : _startNavigateCellPosition.Column;
            int num2 = _startNavigateCellPosition.IsEmpty ? ContentBounds.StartRow : (_startNavigateCellPosition.Row + 1);
            for (int i = num2; i <= ContentBounds.EndRow; i++)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        CompositePosition PredictPreviousCell(out CompositePosition newStartPosition)
        {
            if (CurrentCell.IsEmpty)
            {
                return PredictNextCell(out newStartPosition);
            }
            List<int> columnIndexes = GetColumnIndexes();
            CompositePosition startCellPosition = _startNavigateCellPosition;
            CompositePosition position2 = GetPreviousCellInRow(startCellPosition, false, columnIndexes, out newStartPosition);
            if (position2.IsEmpty)
            {
                for (int i = startCellPosition.Row - 1; i >= 0; i--)
                {
                    position2 = GetPreviousCellInRow(new CompositePosition(startCellPosition.Type, i, columnIndexes[columnIndexes.Count - 1]), true, columnIndexes, out newStartPosition);
                    if (!position2.IsEmpty)
                    {
                        return position2;
                    }
                }
            }
            return position2;
        }

        CompositePosition PredictPreviousCellInSelection(out CompositePosition newStartPosition, out int rangeIndex)
        {
            rangeIndex = _currentRangeIndex;
            if (rangeIndex == -1)
            {
                rangeIndex = GetCurrentPageIndex(CurrentCell);
            }
            if (rangeIndex < 0)
            {
                newStartPosition = CurrentCell;
                return CompositePosition.Empty;
            }
            List<CompositeRange> list = new List<CompositeRange>();
            for (int i = 0; i < Selections.Count; i++)
            {
                list.Add(GetFixedRange(Selections[i]));
            }
            CompositePosition empty = CompositePosition.Empty;
            List<int> columnIndexes = GetColumnIndexes(list[rangeIndex]);
            empty = GetPreviousCellInRange(list[rangeIndex], CurrentCell.Row, columnIndexes, CurrentCell.Column, out newStartPosition);
            if (empty.IsEmpty)
            {
                for (int j = rangeIndex - 1; j >= 0; j--)
                {
                    columnIndexes = GetColumnIndexes(list[j]);
                    empty = GetPreviousCellInRange(list[j], list[j].EndRow, columnIndexes, list[j].EndColumn + 1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = j;
                        return empty;
                    }
                }
                for (int k = list.Count - 1; k >= 0; k--)
                {
                    columnIndexes = GetColumnIndexes(list[k]);
                    empty = GetPreviousCellInRange(list[k], list[k].EndRow, columnIndexes, list[k].EndColumn + 1, out newStartPosition);
                    if (!empty.IsEmpty)
                    {
                        rangeIndex = k;
                        return empty;
                    }
                }
            }
            return empty;
        }

        CompositePosition PredictPreviousRowCell(out CompositePosition newStartPosition)
        {
            if (CurrentCell.IsEmpty)
            {
                return PredictNextRowCell(out newStartPosition);
            }
            newStartPosition = CompositePosition.Empty;
            int column = _startNavigateCellPosition.IsEmpty ? 0 : _startNavigateCellPosition.Column;
            int num2 = _startNavigateCellPosition.IsEmpty ? ContentBounds.StartRow : (_startNavigateCellPosition.Row - 1);
            for (int i = num2; i >= ContentBounds.StartRow; i--)
            {
                CompositePosition position2;
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, i, column);
                if (IsMerged(position, out position2))
                {
                    position = position2;
                }
                if (CanMoveCurrentTo(position))
                {
                    newStartPosition = position;
                    return position;
                }
            }
            return CompositePosition.Empty;
        }

        public virtual bool ShouldNavigateInSelection()
        {
            if (Selections.Count == 0)
            {
                return false;
            }
            if (Selections.Count <= 1)
            {
                CompositeRange fixedRange = GetFixedRange(Selections[0]);
                CompositePosition topLeft = GetTopLeft(fixedRange);
                if (fixedRange.RowCount <= GetRowSpan(topLeft))
                {
                    return (fixedRange.ColumnCount > GetColumnSpan(topLeft));
                }
            }
            return true;
        }

        public void UpdateNavigationStartPosition(CompositePosition currentPosition, CompositePosition startPosition)
        {
            _currentCellPosition = currentPosition;
            if (startPosition.IsEmpty && !currentPosition.IsEmpty)
            {
                _startNavigateCellPosition = currentPosition;
            }
            else
            {
                _startNavigateCellPosition = startPosition;
            }
        }

        public abstract int CompositeColumnCount { get; }

        public abstract int CompositeRowCount { get; }

        public virtual CompositeRange ContentBounds
        {
            get { return  new CompositeRange(DataSheetElementType.Cell, 0, 0, CompositeRowCount, CompositeColumnCount); }
        }

        public virtual CompositePosition CurrentCell
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

        public CompositeRange CurrentRange
        {
            get
            {
                IList<CompositeRange> selections = Selections;
                if ((_currentRangeIndex >= 0) && (_currentRangeIndex < selections.Count))
                {
                    return selections[_currentRangeIndex];
                }
                if (!CurrentCell.IsEmpty)
                {
                    for (int i = selections.Count - 1; i >= 0; i--)
                    {
                        if (((selections[i].Row == -1) || ((CurrentCell.Row >= selections[i].Row) && (CurrentCell.Row <= selections[i].EndRow))) && ((selections[i].Column == -1) || ((CurrentCell.Column >= selections[i].Column) && (CurrentCell.Column <= selections[i].EndColumn))))
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

