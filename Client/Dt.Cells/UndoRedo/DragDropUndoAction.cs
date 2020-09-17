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
using Dt.Cells.UI;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a drag drop undo action used to drag a range date and drop it to another range on the sheet.
    /// </summary>
    public class DragDropUndoAction : ActionBase, IUndo
    {
        bool _copy;
        Dt.Cells.UndoRedo.DragDropExtent _dragDropExtent;
        bool _insert;
        CopyToOption _option;
        int _savedAcitveColumnViewportIndex = -2;
        int _savedAcitveRowViewportIndex = -2;
        int _savedActiveColumn = -1;
        int _savedActiveRow = -1;
        CopyMoveCellsInfo _savedFromColumnHeaderCells;
        CopyMoveColumnsInfo _savedFromColumns;
        CopyMoveFloatingObjectsInfo _savedFromFloatingObjects;
        CopyMoveCellsInfo _savedFromRowHeaderCells;
        CopyMoveRowsInfo _savedFromRows;
        CopyMoveCellsInfo _savedFromViewportCells;
        CopyMoveCellsInfo _savedToColumnHeaderCells;
        CopyMoveColumnsInfo _savedToColumns;
        CopyMoveFloatingObjectsInfo _savedToFloatingObjects;
        CopyMoveCellsInfo _savedToRowHeaderCells;
        CopyMoveRowsInfo _savedToRows;
        CopyMoveCellsInfo _savedToViewportCells;
        Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragDropUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet to drag and drop.</param>
        /// <param name="dragMoveExtent">The drag drop extent information.</param>
        /// <param name="copy">if set to <c>true</c> copy; otherwise, <c>false</c>.</param>
        /// <param name="insert">if set to <c>true</c> inserts the drag data in the drop row or column.</param>
        /// <param name="option">The <see cref="T:GrapeCity.Windows.SpreadSheet.Data.CopyToOption" /> indicates the content type to drag and drop.</param>
        public DragDropUndoAction(Worksheet sheet, Dt.Cells.UndoRedo.DragDropExtent dragMoveExtent, bool copy, bool insert, CopyToOption option)
        {
            if (sheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            if (dragMoveExtent == null)
            {
                throw new ArgumentNullException("dragMoveExtent");
            }
            _sheet = sheet;
            _dragDropExtent = dragMoveExtent;
            _copy = copy;
            _insert = insert;
            _option = option;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed in, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (Excel.IsValidRange(_dragDropExtent.FromRow, _dragDropExtent.FromColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount, _sheet.RowCount, _sheet.ColumnCount) && (_insert || Excel.IsValidRange(_dragDropExtent.ToRow, _dragDropExtent.ToColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount, _sheet.RowCount, _sheet.ColumnCount)))
            {
                SaveState();
                if (_insert)
                {
                    if ((_dragDropExtent.FromColumn < 0) || (_dragDropExtent.FromRow < 0))
                    {
                        if (_dragDropExtent.FromColumn < 0)
                        {
                            if (_dragDropExtent.FromRow >= 0)
                            {
                                int fromRow = _dragDropExtent.FromRow;
                                int toRow = _dragDropExtent.ToRow;
                                int rowCount = _dragDropExtent.RowCount;
                                int row = _dragDropExtent.ToRow;
                                base.SuspendInvalidate(sender);
                                try
                                {
                                    _sheet.AddRows(toRow, rowCount);
                                    if (_copy)
                                    {
                                        _sheet.CopyTo((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, -1, toRow, -1, rowCount, -1, _option);
                                    }
                                    else
                                    {
                                        _sheet.MoveTo((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, -1, toRow, -1, rowCount, -1, _option);
                                        _sheet.RemoveRows((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, rowCount);
                                        if (fromRow < toRow)
                                        {
                                            row = toRow - rowCount;
                                        }
                                    }
                                }
                                finally
                                {
                                    base.ResumeInvalidate(sender);
                                }
                                Excel view2 = sender as Excel;
                                if (view2 != null)
                                {
                                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view2.ActiveSheet.Selections);
                                    view2.SetSelection(row, -1, rowCount, -1);
                                    if (view2.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view2.ActiveSheet.Selections)))
                                    {
                                        view2.RaiseSelectionChanged();
                                    }
                                    view2.SetActiveCell(row, 0, false);
                                    view2.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                                    view2.RefreshFloatingObjects();
                                }
                            }
                        }
                        else
                        {
                            int fromColumn = _dragDropExtent.FromColumn;
                            int toColumn = _dragDropExtent.ToColumn;
                            int columnCount = _dragDropExtent.ColumnCount;
                            int column = _dragDropExtent.ToColumn;
                            base.SuspendInvalidate(sender);
                            try
                            {
                                _sheet.AddColumns(toColumn, columnCount);
                                if (_copy)
                                {
                                    _sheet.CopyTo(-1, (toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, -1, toColumn, -1, columnCount, _option);
                                }
                                else
                                {
                                    _sheet.MoveTo(-1, (toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, -1, toColumn, -1, columnCount, _option);
                                    _sheet.RemoveColumns((toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, columnCount);
                                    if (fromColumn < toColumn)
                                    {
                                        column = toColumn - columnCount;
                                    }
                                }
                            }
                            finally
                            {
                                base.ResumeInvalidate(sender);
                            }
                            Excel view = sender as Excel;
                            if (view != null)
                            {
                                CellRange[] rangeArray = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.ActiveSheet.Selections);
                                view.SetSelection(-1, column, -1, columnCount);
                                if (view.RaiseSelectionChanging(rangeArray, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.ActiveSheet.Selections)))
                                {
                                    view.RaiseSelectionChanged();
                                }
                                view.SetActiveCell(0, column, false);
                                view.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                                view.RefreshFloatingObjects();
                            }
                        }
                    }
                }
                else
                {
                    int num9 = _dragDropExtent.FromRow;
                    int num10 = _dragDropExtent.FromColumn;
                    int num11 = _dragDropExtent.ToRow;
                    int num12 = _dragDropExtent.ToColumn;
                    int num13 = _dragDropExtent.RowCount;
                    int num14 = _dragDropExtent.ColumnCount;
                    Excel sheetView = sender as Excel;
                    base.SuspendInvalidate(sender);
                    try
                    {
                        if (_copy)
                        {
                            _sheet.CopyTo(num9, num10, num11, num12, num13, num14, _option);
                        }
                        else
                        {
                            _sheet.MoveTo(num9, num10, num11, num12, num13, num14, _option);
                        }
                        if (sheetView != null)
                        {
                            CellRange[] rangeArray3 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections);
                            sheetView.SetSelection(num11, num12, num13, num14);
                            if (sheetView.RaiseSelectionChanging(rangeArray3, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.SetActiveCell(Math.Max(0, num11), Math.Max(0, num12), false);
                            if ((!_copy && (_savedFromViewportCells != null)) && _savedFromViewportCells.IsValueSaved())
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, num9, num10, num13, num14, _savedFromViewportCells.GetValues());
                            }
                            if ((_savedToViewportCells != null) && _savedToViewportCells.IsValueSaved())
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, num11, num12, num13, num14, _savedToViewportCells.GetValues());
                            }
                        }
                    }
                    finally
                    {
                        base.ResumeInvalidate(sender);
                    }
                    if (sheetView != null)
                    {
                        sheetView.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                        sheetView.RefreshFloatingObjects();
                    }
                }
            }
        }

        void InitSaveState()
        {
            _savedFromColumnHeaderCells = null;
            _savedFromColumns = null;
            _savedFromViewportCells = null;
            _savedFromRowHeaderCells = null;
            _savedFromRows = null;
            _savedFromFloatingObjects = null;
            _savedToColumnHeaderCells = null;
            _savedToColumns = null;
            _savedToViewportCells = null;
            _savedToRowHeaderCells = null;
            _savedToRows = null;
            _savedToFloatingObjects = null;
            _savedAcitveRowViewportIndex = -2;
            _savedAcitveColumnViewportIndex = -2;
            _savedActiveRow = -1;
            _savedActiveColumn = -1;
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            InitSaveState();
            int baseRow = (_dragDropExtent.FromRow < 0) ? 0 : _dragDropExtent.FromRow;
            int baseColumn = (_dragDropExtent.FromColumn < 0) ? 0 : _dragDropExtent.FromColumn;
            int row = (_dragDropExtent.ToRow < 0) ? 0 : _dragDropExtent.ToRow;
            int num4 = (_dragDropExtent.ToColumn < 0) ? 0 : _dragDropExtent.ToColumn;
            int rowCount = (_dragDropExtent.FromRow < 0) ? _sheet.RowCount : _dragDropExtent.RowCount;
            int columnCount = (_dragDropExtent.FromColumn < 0) ? _sheet.ColumnCount : _dragDropExtent.ColumnCount;
            if (_insert)
            {
                if (((_dragDropExtent.FromColumn < 0) || (_dragDropExtent.FromRow < 0)) && (((_dragDropExtent.FromColumn < 0) && (_dragDropExtent.FromRow >= 0)) && (!_copy && Excel.HasTable(_sheet, row, -1, 1, -1, true))))
                {
                    CopyMoveCellsInfo headerCellsInfo = new CopyMoveCellsInfo(rowCount, _sheet.RowHeader.ColumnCount);
                    CopyMoveRowsInfo rowsInfo = new CopyMoveRowsInfo(rowCount);
                    CopyMoveHelper.SaveRowHeaderInfo(_sheet, headerCellsInfo, rowsInfo, baseRow, _option);
                    _savedFromRowHeaderCells = headerCellsInfo;
                    _savedFromRows = rowsInfo;
                    CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(rowCount, columnCount);
                    CopyMoveHelper.SaveViewportInfo(_sheet, cellsInfo, baseRow, baseColumn, _option);
                    _savedFromViewportCells = cellsInfo;
                }
            }
            else
            {
                if (_dragDropExtent.FromRow < 0)
                {
                    CopyMoveCellsInfo info4 = new CopyMoveCellsInfo(_sheet.ColumnHeader.RowCount, columnCount);
                    CopyMoveColumnsInfo columnsInfo = new CopyMoveColumnsInfo(columnCount);
                    CopyMoveHelper.SaveColumnHeaderInfo(_sheet, info4, columnsInfo, num4, _option);
                    _savedToColumnHeaderCells = info4;
                    _savedToColumns = columnsInfo;
                    if (!_copy)
                    {
                        CopyMoveCellsInfo info6 = new CopyMoveCellsInfo(_sheet.ColumnHeader.RowCount, columnCount);
                        CopyMoveColumnsInfo info7 = new CopyMoveColumnsInfo(columnCount);
                        CopyMoveHelper.SaveColumnHeaderInfo(_sheet, info6, info7, baseColumn, _option);
                        _savedFromColumnHeaderCells = info6;
                        _savedFromColumns = info7;
                    }
                }
                if (_dragDropExtent.FromColumn < 0)
                {
                    CopyMoveCellsInfo info8 = new CopyMoveCellsInfo(rowCount, _sheet.RowHeader.ColumnCount);
                    CopyMoveRowsInfo info9 = new CopyMoveRowsInfo(rowCount);
                    CopyMoveHelper.SaveRowHeaderInfo(_sheet, info8, info9, row, _option);
                    _savedToRowHeaderCells = info8;
                    _savedToRows = info9;
                    if (!_copy)
                    {
                        CopyMoveCellsInfo info10 = new CopyMoveCellsInfo(rowCount, _sheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo info11 = new CopyMoveRowsInfo(rowCount);
                        CopyMoveHelper.SaveRowHeaderInfo(_sheet, info10, info11, baseRow, _option);
                        _savedFromRowHeaderCells = info10;
                        _savedFromRows = info11;
                    }
                }
                CopyMoveCellsInfo info12 = new CopyMoveCellsInfo(rowCount, columnCount);
                CopyMoveHelper.SaveViewportInfo(_sheet, info12, row, num4, _option);
                _savedToViewportCells = info12;
                if (!_copy)
                {
                    CopyMoveCellsInfo info13 = new CopyMoveCellsInfo(rowCount, columnCount);
                    CopyMoveHelper.SaveViewportInfo(_sheet, info13, baseRow, baseColumn, _option);
                    _savedFromViewportCells = info13;
                    if ((_option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        CellRange range = new CellRange(_dragDropExtent.FromRow, _dragDropExtent.FromColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount);
                        FloatingObject[] floatingObjectsInRange = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(range, _sheet.RowCount, _sheet.ColumnCount), _sheet);
                        _savedFromFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        _savedFromFloatingObjects.SaveFloatingObjects(range, floatingObjectsInRange);
                    }
                }
                if ((_option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                {
                    CellRange range3 = new CellRange(_dragDropExtent.ToRow, _dragDropExtent.ToColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount);
                    FloatingObject[] floatingObjects = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(range3, _sheet.RowCount, _sheet.ColumnCount), _sheet);
                    _savedToFloatingObjects = new CopyMoveFloatingObjectsInfo();
                    _savedToFloatingObjects.SaveFloatingObjects(range3, floatingObjects);
                }
            }
            _savedAcitveRowViewportIndex = _sheet.GetActiveRowViewportIndex();
            _savedAcitveColumnViewportIndex = _sheet.GetActiveColumnViewportIndex();
            _savedActiveRow = _sheet.ActiveRowIndex;
            _savedActiveColumn = _sheet.ActiveColumnIndex;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionDragDrop;
        }

        /// <summary>
        /// Undoes the action of the saved information.
        /// </summary>
        /// <param name="parameter">The parameter to undo the action on. </param>
        /// <returns>
        /// <c>true</c> if the undo action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            if (!Excel.IsValidRange(_dragDropExtent.FromRow, _dragDropExtent.FromColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount, _sheet.RowCount, _sheet.ColumnCount))
            {
                return false;
            }
            if (!_insert && !Excel.IsValidRange(_dragDropExtent.ToRow, _dragDropExtent.ToColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount, _sheet.RowCount, _sheet.ColumnCount))
            {
                return false;
            }
            bool flag = false;
            Excel sheetView = parameter as Excel;
            if (_insert)
            {
                if ((_dragDropExtent.FromColumn < 0) || (_dragDropExtent.FromRow < 0))
                {
                    if (_dragDropExtent.FromColumn >= 0)
                    {
                        int fromColumn = _dragDropExtent.FromColumn;
                        int columnCount = _dragDropExtent.ColumnCount;
                        base.SuspendInvalidate(parameter);
                        try
                        {
                            if (_copy)
                            {
                                _sheet.RemoveColumns(_dragDropExtent.ToColumn, columnCount);
                            }
                            else
                            {
                                int toColumn = _dragDropExtent.ToColumn;
                                int column = _dragDropExtent.FromColumn;
                                if (_dragDropExtent.FromColumn < _dragDropExtent.ToColumn)
                                {
                                    toColumn = _dragDropExtent.ToColumn - columnCount;
                                }
                                else
                                {
                                    column = _dragDropExtent.FromColumn + columnCount;
                                }
                                _sheet.AddColumns(column, columnCount);
                                _sheet.CopyTo(-1, (column <= toColumn) ? (toColumn + columnCount) : toColumn, -1, column, -1, columnCount, _option);
                                _sheet.RemoveColumns((column <= toColumn) ? (toColumn + columnCount) : toColumn, columnCount);
                                if (toColumn < column)
                                {
                                    fromColumn = column - columnCount;
                                }
                            }
                        }
                        finally
                        {
                            base.ResumeInvalidate(parameter);
                        }
                        if (sheetView != null)
                        {
                            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections);
                            sheetView.SetSelection(-1, fromColumn, -1, columnCount);
                            if (sheetView.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            sheetView.RefreshFloatingObjects();
                        }
                        flag = true;
                    }
                    else if (_dragDropExtent.FromRow >= 0)
                    {
                        int rowCount = _dragDropExtent.RowCount;
                        int fromRow = _dragDropExtent.FromRow;
                        base.SuspendInvalidate(parameter);
                        try
                        {
                            if (_copy)
                            {
                                _sheet.RemoveRows(_dragDropExtent.ToRow, rowCount);
                            }
                            else
                            {
                                int toRow = _dragDropExtent.ToRow;
                                int row = _dragDropExtent.FromRow;
                                if (_dragDropExtent.FromRow < _dragDropExtent.ToRow)
                                {
                                    toRow = _dragDropExtent.ToRow - rowCount;
                                }
                                else
                                {
                                    row = _dragDropExtent.FromRow + rowCount;
                                }
                                _sheet.AddRows(row, rowCount);
                                if (_savedFromViewportCells != null)
                                {
                                    CopyMoveHelper.UndoCellsInfo(_sheet, _savedFromViewportCells, row, 0, SheetArea.Cells);
                                    flag = true;
                                }
                                if (_savedFromRowHeaderCells != null)
                                {
                                    CopyMoveHelper.UndoCellsInfo(_sheet, _savedFromRowHeaderCells, row, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                                    flag = true;
                                }
                                if (_savedFromRows != null)
                                {
                                    CopyMoveHelper.UndoRowsInfo(_sheet, _savedFromRows, row);
                                    flag = true;
                                }
                                if (!flag)
                                {
                                    _sheet.MoveTo((row <= toRow) ? (toRow + rowCount) : toRow, -1, row, -1, rowCount, -1, _option);
                                }
                                _sheet.RemoveRows((row <= toRow) ? (toRow + rowCount) : toRow, rowCount);
                                if (toRow < row)
                                {
                                    fromRow = row - rowCount;
                                }
                            }
                        }
                        finally
                        {
                            base.ResumeInvalidate(parameter);
                        }
                        if (sheetView != null)
                        {
                            CellRange[] rangeArray2 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections);
                            sheetView.SetSelection(fromRow, -1, rowCount, -1);
                            if (sheetView.RaiseSelectionChanging(rangeArray2, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.RefreshAll();
                            sheetView.UpdateLayout();
                            sheetView.RefreshFloatingObjects();
                        }
                        flag = true;
                    }
                }
            }
            else
            {
                int num9 = (_dragDropExtent.FromRow < 0) ? 0 : _dragDropExtent.FromRow;
                int num10 = (_dragDropExtent.FromColumn < 0) ? 0 : _dragDropExtent.FromColumn;
                int num11 = (_dragDropExtent.ToRow < 0) ? 0 : _dragDropExtent.ToRow;
                int num12 = (_dragDropExtent.ToColumn < 0) ? 0 : _dragDropExtent.ToColumn;
                int num13 = (_dragDropExtent.FromRow < 0) ? _sheet.RowCount : _dragDropExtent.RowCount;
                int num14 = (_dragDropExtent.FromColumn < 0) ? _sheet.ColumnCount : _dragDropExtent.ColumnCount;
                List<CellData> oldValues = null;
                List<CellData> list2 = null;
                if ((!_copy && (_savedFromViewportCells != null)) && _savedFromViewportCells.IsValueSaved())
                {
                    list2 = CopyMoveHelper.GetValues(_sheet, num9, num10, num13, num14);
                }
                if ((_savedToViewportCells != null) && _savedToViewportCells.IsValueSaved())
                {
                    oldValues = CopyMoveHelper.GetValues(_sheet, num11, num12, num13, num14);
                }
                base.SuspendInvalidate(parameter);
                try
                {
                    if (_savedToColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedToColumnHeaderCells, 0, num12, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (_savedToColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(_sheet, _savedToColumns, num12);
                        flag = true;
                    }
                    if (_savedToViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedToViewportCells, num11, num12, SheetArea.Cells);
                        flag = true;
                    }
                    if (_savedToRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedToRowHeaderCells, num11, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (_savedToRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(_sheet, _savedToRows, num11);
                        flag = true;
                    }
                    if (_savedToFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(_sheet, _savedToFloatingObjects);
                        sheetView.RefreshFloatingObjects();
                        flag = true;
                    }
                    if (_savedFromColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedFromColumnHeaderCells, 0, num10, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (_savedFromColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(_sheet, _savedFromColumns, num10);
                        flag = true;
                    }
                    if (_savedFromViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedFromViewportCells, num9, num10, SheetArea.Cells);
                        flag = true;
                    }
                    if (_savedFromRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_sheet, _savedFromRowHeaderCells, num9, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (_savedFromRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(_sheet, _savedFromRows, num9);
                        flag = true;
                    }
                    if (_savedFromFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(_sheet, _savedFromFloatingObjects);
                        sheetView.RefreshFloatingObjects();
                        flag = true;
                    }
                }
                finally
                {
                    base.ResumeInvalidate(parameter);
                }
                if (flag && (sheetView != null))
                {
                    CellRange[] rangeArray3 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections);
                    sheetView.SetSelection(_dragDropExtent.FromRow, _dragDropExtent.FromColumn, _dragDropExtent.RowCount, _dragDropExtent.ColumnCount);
                    if (sheetView.RaiseSelectionChanging(rangeArray3, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.ActiveSheet.Selections)))
                    {
                        sheetView.RaiseSelectionChanged();
                    }
                    if (oldValues != null)
                    {
                        CopyMoveHelper.RaiseValueChanged(sheetView, num11, num12, num13, num14, oldValues);
                    }
                    if (list2 != null)
                    {
                        CopyMoveHelper.RaiseValueChanged(sheetView, num9, num10, num13, num14, list2);
                    }
                    sheetView.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    sheetView.RefreshFloatingObjects();
                }
            }
            if (flag && (sheetView != null))
            {
                if ((_savedAcitveRowViewportIndex != -2) && (_savedAcitveColumnViewportIndex != -2))
                {
                    sheetView.SetActiveRowViewportIndex(_savedAcitveRowViewportIndex);
                    sheetView.SetActiveColumnViewportIndex(_savedAcitveColumnViewportIndex);
                }
                if ((_savedActiveRow != -1) && (_savedActiveColumn != -1))
                {
                    CellRange range = sheetView.ActiveSheet.Selections[0];
                    if (range.Contains(_savedActiveRow, _savedActiveColumn))
                    {
                        sheetView.SetActiveCell(_savedActiveRow, _savedActiveColumn, false);
                    }
                    else
                    {
                        sheetView.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), false);
                    }
                }
                if (((_savedAcitveRowViewportIndex != -2) && (_savedAcitveColumnViewportIndex != -2)) && ((_savedActiveRow != -1) && (_savedActiveColumn != -1)))
                {
                    sheetView.ShowCell(_savedAcitveRowViewportIndex, _savedAcitveColumnViewportIndex, _savedActiveRow, _savedActiveColumn, VerticalPosition.Nearest, HorizontalPosition.Nearest);
                }
            }
            return flag;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }

        /// <summary>
        /// Gets the drag drop extent for DragDropUndoAction.
        /// </summary>
        /// <value>
        /// The drag drop extent.
        /// </value>
        public Dt.Cells.UndoRedo.DragDropExtent DragDropExtent
        {
            get { return  _dragDropExtent; }
        }
    }
}

