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
        private SheetView _sheetView;
        private SpreadXTabIndexNavigator _tabIndexNavigator;
        private SpreadXTabularNavigator _tabularNavigator;

        public SpreadXNavigation(SheetView sheetView)
        {
            this._sheetView = sheetView;
            this._tabularNavigator = new SpreadXTabularNavigator(this._sheetView);
            this._tabIndexNavigator = new SpreadXTabIndexNavigator(this._sheetView);
        }

        private bool IsTabNavigation(NavigationDirection direction)
        {
            if (((direction != NavigationDirection.Next) && (direction != NavigationDirection.NextInSelection)) && ((direction != NavigationDirection.Previous) && (direction != NavigationDirection.PreviousInSelection)))
            {
                return false;
            }
            return true;
        }

        private bool MoveActiveCell(NavigationDirection direction)
        {
            if (this._sheetView.Worksheet != null)
            {
                int activeRowViewportIndex = this._sheetView.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = this._sheetView.GetActiveColumnViewportIndex();
                int viewportTopRow = this._sheetView.GetViewportTopRow(activeRowViewportIndex);
                int viewportLeftColumn = this._sheetView.GetViewportLeftColumn(activeColumnViewportIndex);
                int activeRowIndex = this._sheetView.Worksheet.ActiveRowIndex;
                int activeColumnIndex = this._sheetView.Worksheet.ActiveColumnIndex;
                CompositePosition navigationStartPosition = this._tabIndexNavigator.GetNavigationStartPosition();
                TabularPosition position2 = this._tabularNavigator.GetNavigationStartPosition();
                TabularPosition empty = TabularPosition.Empty;
                if (((this._sheetView.Worksheet.Selections.Count > 0) && !NavigatorHelper.ActiveCellInSelection(this._sheetView.Worksheet)) && this.IsTabNavigation(direction))
                {
                    empty = new TabularPosition(SheetArea.Cells, this._sheetView.Worksheet.Selections[0].Row, this._sheetView.Worksheet.Selections[0].Column);
                }
                else
                {
                    empty = this.MoveCurrent(direction);
                }
                if (empty.IsEmpty)
                {
                    if (!this.IsTabNavigation(direction) || !this.ShouldNavInSelection())
                    {
                        CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._sheetView.Worksheet.Selections);
                        this._sheetView.SetSelection(this._sheetView.Worksheet.ActiveRowIndex, this._sheetView.Worksheet.ActiveColumnIndex, 1, 1);
                        if (this._sheetView.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._sheetView.Worksheet.Selections)))
                        {
                            this._sheetView.RaiseSelectionChanged();
                        }
                        NavigatorHelper.BringCellToVisible(this._sheetView, this._sheetView.Worksheet.ActiveRowIndex, this._sheetView.Worksheet.ActiveColumnIndex);
                    }
                    return false;
                }
                int row = empty.Row;
                int column = empty.Column;
                if (this.SetActiveCell(row, column, false))
                {
                    if (this.IsTabNavigation(direction))
                    {
                        CompositePosition position4 = this._tabIndexNavigator.GetNavigationStartPosition();
                        this.UpdateStartPosition(new TabularPosition(SheetArea.Cells, row, column), new TabularPosition(SheetArea.Cells, position4.Row, position4.Column), ActiveCellSyncState.TabularNavigator);
                    }
                    else
                    {
                        TabularPosition position5 = this._tabularNavigator.GetNavigationStartPosition();
                        this.UpdateStartPosition(new TabularPosition(SheetArea.Cells, row, column), new TabularPosition(SheetArea.Cells, position5.Row, position5.Column), ActiveCellSyncState.TabIndexNavigator);
                    }
                    if (!this.IsTabNavigation(direction) || !this.ShouldNavInSelection())
                    {
                        CellRange[] rangeArray2 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._sheetView.Worksheet.Selections);
                        this._sheetView.SetSelection(row, column, 1, 1);
                        if (this._sheetView.RaiseSelectionChanging(rangeArray2, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._sheetView.Worksheet.Selections)))
                        {
                            this._sheetView.RaiseSelectionChanged();
                        }
                    }
                    int num9 = this._sheetView.GetActiveRowViewportIndex();
                    int num10 = this._sheetView.GetActiveColumnViewportIndex();
                    if ((activeRowViewportIndex != num9) || (activeColumnViewportIndex != num10))
                    {
                        NavigatorHelper.BringCellToVisible(this._sheetView, row, column);
                    }
                    return true;
                }
                this._sheetView.SetViewportTopRow(activeRowViewportIndex, viewportTopRow);
                this._sheetView.SetViewportLeftColumn(activeColumnViewportIndex, viewportLeftColumn);
                if (this.IsTabNavigation(direction))
                {
                    this.UpdateStartPosition(new TabularPosition(SheetArea.Cells, activeRowIndex, activeColumnIndex), new TabularPosition(SheetArea.Cells, navigationStartPosition.Row, navigationStartPosition.Column), ActiveCellSyncState.TabIndexNavigator);
                }
                else
                {
                    this.UpdateStartPosition(new TabularPosition(SheetArea.Cells, activeRowIndex, activeColumnIndex), new TabularPosition(SheetArea.Cells, position2.Row, position2.Column), ActiveCellSyncState.TabularNavigator);
                }
            }
            return false;
        }

        private TabularPosition MoveCurrent(NavigationDirection direction)
        {
            int activeRowIndex = this._sheetView.Worksheet.ActiveRowIndex;
            int activeColumnIndex = this._sheetView.Worksheet.ActiveColumnIndex;
            if ((activeRowIndex != -1) && (activeColumnIndex != -1))
            {
                if (this.IsTabNavigation(direction))
                {
                    if (this._tabIndexNavigator.MoveCurrent(direction))
                    {
                        CompositePosition currentCell = this._tabIndexNavigator.CurrentCell;
                        return new TabularPosition(SheetArea.Cells, currentCell.Row, currentCell.Column);
                    }
                }
                else if (this._tabularNavigator.MoveCurrent(direction))
                {
                    TabularPosition position2 = this._tabularNavigator.CurrentCell;
                    return new TabularPosition(SheetArea.Cells, position2.Row, position2.Column);
                }
            }
            return TabularPosition.Empty;
        }

        public void ProcessNavigation(NavigationDirection? direction)
        {
            if (direction.HasValue)
            {
                this.MoveActiveCell(direction.Value);
            }
        }

        private bool SetActiveCell(int row, int column, bool clearSelection)
        {
            Worksheet worksheet = this._sheetView.Worksheet;
            if (!this._sheetView.RaiseLeaveCell(worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, row, column))
            {
                worksheet.SetActiveCell(row, column, clearSelection);
                this._sheetView.RaiseEnterCell(row, column);
                return true;
            }
            return false;
        }

        public bool ShouldNavInSelection()
        {
            return this._tabIndexNavigator.ShouldNavigateInSelection();
        }

        internal void UpdateStartPosition(int row, int column)
        {
            if (this._sheetView.Worksheet != null)
            {
                row = (row < 0) ? 0 : row;
                column = (column < 0) ? 0 : column;
                CellRange spanCell = this._sheetView.Worksheet.GetSpanCell(row, column);
                TabularPosition currentPosition = new TabularPosition(SheetArea.Cells, row, column);
                TabularPosition navigationStartPosition = this._tabularNavigator.GetNavigationStartPosition();
                if ((spanCell == null) || !spanCell.Contains(navigationStartPosition.Row, navigationStartPosition.Column))
                {
                    navigationStartPosition = new TabularPosition(SheetArea.Cells, row, column);
                }
                this.UpdateStartPosition(currentPosition, navigationStartPosition, ActiveCellSyncState.TabularNavigator);
                CompositePosition position3 = this._tabIndexNavigator.GetNavigationStartPosition();
                if ((spanCell == null) || !spanCell.Contains(position3.Row, position3.Column))
                {
                    position3 = new CompositePosition(DataSheetElementType.Cell, row, column);
                }
                this.UpdateStartPosition(currentPosition, new TabularPosition(SheetArea.Cells, position3.Row, position3.Column), ActiveCellSyncState.TabIndexNavigator);
            }
        }

        private void UpdateStartPosition(TabularPosition currentPosition, TabularPosition startPosition, ActiveCellSyncState state)
        {
            if ((state & ActiveCellSyncState.TabIndexNavigator) == ActiveCellSyncState.TabIndexNavigator)
            {
                CompositePosition position = new CompositePosition(DataSheetElementType.Cell, currentPosition.Row, currentPosition.Column);
                CompositePosition position2 = new CompositePosition(DataSheetElementType.Cell, startPosition.Row, startPosition.Column);
                this._tabIndexNavigator.UpdateNavigationStartPosition(position, position2);
            }
            if ((state & ActiveCellSyncState.TabularNavigator) == ActiveCellSyncState.TabularNavigator)
            {
                TabularPosition position3 = new TabularPosition(SheetArea.Cells, currentPosition.Row, currentPosition.Column);
                TabularPosition position4 = new TabularPosition(SheetArea.Cells, startPosition.Row, startPosition.Column);
                this._tabularNavigator.UpdateNavigationStartPosition(position3, position4);
            }
        }

        [Flags]
        private enum ActiveCellSyncState
        {
            Both = 3,
            TabIndexNavigator = 2,
            TabularNavigator = 1
        }
    }
}

