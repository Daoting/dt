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
using System.Linq;
#endregion

namespace Dt.Cells.UI
{
    internal class SpreadXNavigation
    {
        SheetView _sheetView;
        SpreadXTabIndexNavigator _tabIndexNavigator;
        SpreadXTabularNavigator _tabularNavigator;

        public SpreadXNavigation(SheetView sheetView)
        {
            _sheetView = sheetView;
            _tabularNavigator = new SpreadXTabularNavigator(_sheetView);
            _tabIndexNavigator = new SpreadXTabIndexNavigator(_sheetView);
        }

        bool IsTabNavigation(NavigationDirection direction)
        {
            if (((direction != NavigationDirection.Next) && (direction != NavigationDirection.NextInSelection)) && ((direction != NavigationDirection.Previous) && (direction != NavigationDirection.PreviousInSelection)))
            {
                return false;
            }
            return true;
        }

        bool MoveActiveCell(NavigationDirection direction)
        {
            if (_sheetView.ActiveSheet != null)
            {
                int activeRowViewportIndex = _sheetView.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = _sheetView.GetActiveColumnViewportIndex();
                int viewportTopRow = _sheetView.GetViewportTopRow(activeRowViewportIndex);
                int viewportLeftColumn = _sheetView.GetViewportLeftColumn(activeColumnViewportIndex);
                int activeRowIndex = _sheetView.ActiveSheet.ActiveRowIndex;
                int activeColumnIndex = _sheetView.ActiveSheet.ActiveColumnIndex;
                CompositePosition navigationStartPosition = _tabIndexNavigator.GetNavigationStartPosition();
                TabularPosition position2 = _tabularNavigator.GetNavigationStartPosition();
                TabularPosition empty = TabularPosition.Empty;
                if (((_sheetView.ActiveSheet.Selections.Count > 0) && !NavigatorHelper.ActiveCellInSelection(_sheetView.ActiveSheet)) && IsTabNavigation(direction))
                {
                    empty = new TabularPosition(SheetArea.Cells, _sheetView.ActiveSheet.Selections[0].Row, _sheetView.ActiveSheet.Selections[0].Column);
                }
                else
                {
                    empty = MoveCurrent(direction);
                }
                if (empty.IsEmpty)
                {
                    if (!IsTabNavigation(direction) || !ShouldNavInSelection())
                    {
                        CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _sheetView.ActiveSheet.Selections);
                        _sheetView.SetSelection(_sheetView.ActiveSheet.ActiveRowIndex, _sheetView.ActiveSheet.ActiveColumnIndex, 1, 1);
                        if (_sheetView.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _sheetView.ActiveSheet.Selections)))
                        {
                            _sheetView.RaiseSelectionChanged();
                        }
                        NavigatorHelper.BringCellToVisible(_sheetView, _sheetView.ActiveSheet.ActiveRowIndex, _sheetView.ActiveSheet.ActiveColumnIndex);
                    }
                    return false;
                }
                int row = empty.Row;
                int column = empty.Column;
                if (SetActiveCell(row, column, false))
                {
                    if (IsTabNavigation(direction))
                    {
                        CompositePosition position4 = _tabIndexNavigator.GetNavigationStartPosition();
                        UpdateStartPosition(new TabularPosition(SheetArea.Cells, row, column), new TabularPosition(SheetArea.Cells, position4.Row, position4.Column), ActiveCellSyncState.TabularNavigator);
                    }
                    else
                    {
                        TabularPosition position5 = _tabularNavigator.GetNavigationStartPosition();
                        UpdateStartPosition(new TabularPosition(SheetArea.Cells, row, column), new TabularPosition(SheetArea.Cells, position5.Row, position5.Column), ActiveCellSyncState.TabIndexNavigator);
                    }
                    if (!IsTabNavigation(direction) || !ShouldNavInSelection())
                    {
                        CellRange[] rangeArray2 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _sheetView.ActiveSheet.Selections);
                        _sheetView.SetSelection(row, column, 1, 1);
                        if (_sheetView.RaiseSelectionChanging(rangeArray2, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _sheetView.ActiveSheet.Selections)))
                        {
                            _sheetView.RaiseSelectionChanged();
                        }
                    }
                    int num9 = _sheetView.GetActiveRowViewportIndex();
                    int num10 = _sheetView.GetActiveColumnViewportIndex();
                    if ((activeRowViewportIndex != num9) || (activeColumnViewportIndex != num10))
                    {
                        NavigatorHelper.BringCellToVisible(_sheetView, row, column);
                    }
                    return true;
                }
                _sheetView.SetViewportTopRow(activeRowViewportIndex, viewportTopRow);
                _sheetView.SetViewportLeftColumn(activeColumnViewportIndex, viewportLeftColumn);
                if (IsTabNavigation(direction))
                {
                    UpdateStartPosition(new TabularPosition(SheetArea.Cells, activeRowIndex, activeColumnIndex), new TabularPosition(SheetArea.Cells, navigationStartPosition.Row, navigationStartPosition.Column), ActiveCellSyncState.TabIndexNavigator);
                }
                else
                {
                    UpdateStartPosition(new TabularPosition(SheetArea.Cells, activeRowIndex, activeColumnIndex), new TabularPosition(SheetArea.Cells, position2.Row, position2.Column), ActiveCellSyncState.TabularNavigator);
                }
            }
            return false;
        }

        TabularPosition MoveCurrent(NavigationDirection direction)
        {
            int activeRowIndex = _sheetView.ActiveSheet.ActiveRowIndex;
            int activeColumnIndex = _sheetView.ActiveSheet.ActiveColumnIndex;
            if ((activeRowIndex != -1) && (activeColumnIndex != -1))
            {
                if (IsTabNavigation(direction))
                {
                    if (_tabIndexNavigator.MoveCurrent(direction))
                    {
                        CompositePosition currentCell = _tabIndexNavigator.CurrentCell;
                        return new TabularPosition(SheetArea.Cells, currentCell.Row, currentCell.Column);
                    }
                }
                else if (_tabularNavigator.MoveCurrent(direction))
                {
                    TabularPosition position2 = _tabularNavigator.CurrentCell;
                    return new TabularPosition(SheetArea.Cells, position2.Row, position2.Column);
                }
            }
            return TabularPosition.Empty;
        }

        public void ProcessNavigation(NavigationDirection? direction)
        {
            if (direction.HasValue)
            {
                MoveActiveCell(direction.Value);
            }
        }

        bool SetActiveCell(int row, int column, bool clearSelection)
        {
            var worksheet = _sheetView.ActiveSheet;
            if (!_sheetView.RaiseLeaveCell(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, row, column))
            {
                worksheet.SetActiveCell(row, column, clearSelection);
                _sheetView.RaiseEnterCell(row, column);
                return true;
            }
            return false;
        }

        public bool ShouldNavInSelection()
        {
            return _tabIndexNavigator.ShouldNavigateInSelection();
        }

        internal void UpdateStartPosition(int row, int column)
        {
            if (_sheetView.ActiveSheet != null)
            {
                row = (row < 0) ? 0 : row;
                column = (column < 0) ? 0 : column;
                CellRange spanCell = _sheetView.ActiveSheet.GetSpanCell(row, column);
                TabularPosition currentPosition = new TabularPosition(SheetArea.Cells, row, column);
                TabularPosition navigationStartPosition = _tabularNavigator.GetNavigationStartPosition();
                if ((spanCell == null) || !spanCell.Contains(navigationStartPosition.Row, navigationStartPosition.Column))
                {
                    navigationStartPosition = new TabularPosition(SheetArea.Cells, row, column);
                }
                UpdateStartPosition(currentPosition, navigationStartPosition, ActiveCellSyncState.TabularNavigator);
                CompositePosition position3 = _tabIndexNavigator.GetNavigationStartPosition();
                if ((spanCell == null) || !spanCell.Contains(position3.Row, position3.Column))
                {
                    position3 = new CompositePosition(DataSheetElementType.Cell, row, column);
                }
                UpdateStartPosition(currentPosition, new TabularPosition(SheetArea.Cells, position3.Row, position3.Column), ActiveCellSyncState.TabIndexNavigator);
            }
        }

        void UpdateStartPosition(TabularPosition currentPosition, TabularPosition startPosition, ActiveCellSyncState state)
        {
            if ((state & ActiveCellSyncState.TabIndexNavigator) == ActiveCellSyncState.TabIndexNavigator)
            {
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, currentPosition.Row, currentPosition.Column);
                CompositePosition position2 = new CompositePosition(DataSheetElementType.Cell, startPosition.Row, startPosition.Column);
                _tabIndexNavigator.UpdateNavigationStartPosition(position, position2);
            }
            if ((state & ActiveCellSyncState.TabularNavigator) == ActiveCellSyncState.TabularNavigator)
            {
                TabularPosition position3 = new TabularPosition(SheetArea.Cells, currentPosition.Row, currentPosition.Column);
                TabularPosition position4 = new TabularPosition(SheetArea.Cells, startPosition.Row, startPosition.Column);
                _tabularNavigator.UpdateNavigationStartPosition(position3, position4);
            }
        }

        [Flags]
        enum ActiveCellSyncState
        {
            Both = 3,
            TabIndexNavigator = 2,
            TabularNavigator = 1
        }
    }
}

