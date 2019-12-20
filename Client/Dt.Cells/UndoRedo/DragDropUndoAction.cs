#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
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
        private bool _copy;
        private Dt.Cells.UndoRedo.DragDropExtent _dragDropExtent;
        private bool _insert;
        private CopyToOption _option;
        private int _savedAcitveColumnViewportIndex = -2;
        private int _savedAcitveRowViewportIndex = -2;
        private int _savedActiveColumn = -1;
        private int _savedActiveRow = -1;
        private CopyMoveCellsInfo _savedFromColumnHeaderCells;
        private CopyMoveColumnsInfo _savedFromColumns;
        private CopyMoveFloatingObjectsInfo _savedFromFloatingObjects;
        private CopyMoveCellsInfo _savedFromRowHeaderCells;
        private CopyMoveRowsInfo _savedFromRows;
        private CopyMoveCellsInfo _savedFromViewportCells;
        private CopyMoveCellsInfo _savedToColumnHeaderCells;
        private CopyMoveColumnsInfo _savedToColumns;
        private CopyMoveFloatingObjectsInfo _savedToFloatingObjects;
        private CopyMoveCellsInfo _savedToRowHeaderCells;
        private CopyMoveRowsInfo _savedToRows;
        private CopyMoveCellsInfo _savedToViewportCells;
        private Worksheet _sheet;

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
            this._sheet = sheet;
            this._dragDropExtent = dragMoveExtent;
            this._copy = copy;
            this._insert = insert;
            this._option = option;
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
            if (SheetView.IsValidRange(this._dragDropExtent.FromRow, this._dragDropExtent.FromColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount, this._sheet.RowCount, this._sheet.ColumnCount) && (this._insert || SheetView.IsValidRange(this._dragDropExtent.ToRow, this._dragDropExtent.ToColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount, this._sheet.RowCount, this._sheet.ColumnCount)))
            {
                this.SaveState();
                if (this._insert)
                {
                    if ((this._dragDropExtent.FromColumn < 0) || (this._dragDropExtent.FromRow < 0))
                    {
                        if (this._dragDropExtent.FromColumn < 0)
                        {
                            if (this._dragDropExtent.FromRow >= 0)
                            {
                                int fromRow = this._dragDropExtent.FromRow;
                                int toRow = this._dragDropExtent.ToRow;
                                int rowCount = this._dragDropExtent.RowCount;
                                int row = this._dragDropExtent.ToRow;
                                base.SuspendInvalidate(sender);
                                try
                                {
                                    this._sheet.AddRows(toRow, rowCount);
                                    if (this._copy)
                                    {
                                        this._sheet.CopyTo((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, -1, toRow, -1, rowCount, -1, this._option);
                                    }
                                    else
                                    {
                                        this._sheet.MoveTo((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, -1, toRow, -1, rowCount, -1, this._option);
                                        this._sheet.RemoveRows((toRow <= fromRow) ? (fromRow + rowCount) : fromRow, rowCount);
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
                                SheetView view2 = sender as SheetView;
                                if (view2 != null)
                                {
                                    CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view2.Worksheet.Selections);
                                    view2.SetSelection(row, -1, rowCount, -1);
                                    if (view2.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view2.Worksheet.Selections)))
                                    {
                                        view2.RaiseSelectionChanged();
                                    }
                                    view2.SetActiveCell(row, 0, false);
                                    view2.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                                    view2.InvalidateFloatingObjects();
                                }
                            }
                        }
                        else
                        {
                            int fromColumn = this._dragDropExtent.FromColumn;
                            int toColumn = this._dragDropExtent.ToColumn;
                            int columnCount = this._dragDropExtent.ColumnCount;
                            int column = this._dragDropExtent.ToColumn;
                            base.SuspendInvalidate(sender);
                            try
                            {
                                this._sheet.AddColumns(toColumn, columnCount);
                                if (this._copy)
                                {
                                    this._sheet.CopyTo(-1, (toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, -1, toColumn, -1, columnCount, this._option);
                                }
                                else
                                {
                                    this._sheet.MoveTo(-1, (toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, -1, toColumn, -1, columnCount, this._option);
                                    this._sheet.RemoveColumns((toColumn <= fromColumn) ? (fromColumn + columnCount) : fromColumn, columnCount);
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
                            SheetView view = sender as SheetView;
                            if (view != null)
                            {
                                CellRange[] rangeArray = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.Worksheet.Selections);
                                view.SetSelection(-1, column, -1, columnCount);
                                if (view.RaiseSelectionChanging(rangeArray, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.Worksheet.Selections)))
                                {
                                    view.RaiseSelectionChanged();
                                }
                                view.SetActiveCell(0, column, false);
                                view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                                view.InvalidateFloatingObjects();
                            }
                        }
                    }
                }
                else
                {
                    int num9 = this._dragDropExtent.FromRow;
                    int num10 = this._dragDropExtent.FromColumn;
                    int num11 = this._dragDropExtent.ToRow;
                    int num12 = this._dragDropExtent.ToColumn;
                    int num13 = this._dragDropExtent.RowCount;
                    int num14 = this._dragDropExtent.ColumnCount;
                    SheetView sheetView = sender as SheetView;
                    base.SuspendInvalidate(sender);
                    try
                    {
                        if (this._copy)
                        {
                            this._sheet.CopyTo(num9, num10, num11, num12, num13, num14, this._option);
                        }
                        else
                        {
                            this._sheet.MoveTo(num9, num10, num11, num12, num13, num14, this._option);
                        }
                        if (sheetView != null)
                        {
                            CellRange[] rangeArray3 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections);
                            sheetView.SetSelection(num11, num12, num13, num14);
                            if (sheetView.RaiseSelectionChanging(rangeArray3, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.SetActiveCell(Math.Max(0, num11), Math.Max(0, num12), false);
                            if ((!this._copy && (this._savedFromViewportCells != null)) && this._savedFromViewportCells.IsValueSaved())
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, num9, num10, num13, num14, this._savedFromViewportCells.GetValues());
                            }
                            if ((this._savedToViewportCells != null) && this._savedToViewportCells.IsValueSaved())
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, num11, num12, num13, num14, this._savedToViewportCells.GetValues());
                            }
                        }
                    }
                    finally
                    {
                        base.ResumeInvalidate(sender);
                    }
                    if (sheetView != null)
                    {
                        sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                        sheetView.InvalidateFloatingObjects();
                    }
                }
            }
        }

        private void InitSaveState()
        {
            this._savedFromColumnHeaderCells = null;
            this._savedFromColumns = null;
            this._savedFromViewportCells = null;
            this._savedFromRowHeaderCells = null;
            this._savedFromRows = null;
            this._savedFromFloatingObjects = null;
            this._savedToColumnHeaderCells = null;
            this._savedToColumns = null;
            this._savedToViewportCells = null;
            this._savedToRowHeaderCells = null;
            this._savedToRows = null;
            this._savedToFloatingObjects = null;
            this._savedAcitveRowViewportIndex = -2;
            this._savedAcitveColumnViewportIndex = -2;
            this._savedActiveRow = -1;
            this._savedActiveColumn = -1;
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            this.InitSaveState();
            int baseRow = (this._dragDropExtent.FromRow < 0) ? 0 : this._dragDropExtent.FromRow;
            int baseColumn = (this._dragDropExtent.FromColumn < 0) ? 0 : this._dragDropExtent.FromColumn;
            int row = (this._dragDropExtent.ToRow < 0) ? 0 : this._dragDropExtent.ToRow;
            int num4 = (this._dragDropExtent.ToColumn < 0) ? 0 : this._dragDropExtent.ToColumn;
            int rowCount = (this._dragDropExtent.FromRow < 0) ? this._sheet.RowCount : this._dragDropExtent.RowCount;
            int columnCount = (this._dragDropExtent.FromColumn < 0) ? this._sheet.ColumnCount : this._dragDropExtent.ColumnCount;
            if (this._insert)
            {
                if (((this._dragDropExtent.FromColumn < 0) || (this._dragDropExtent.FromRow < 0)) && (((this._dragDropExtent.FromColumn < 0) && (this._dragDropExtent.FromRow >= 0)) && (!this._copy && SheetView.HasTable(this._sheet, row, -1, 1, -1, true))))
                {
                    CopyMoveCellsInfo headerCellsInfo = new CopyMoveCellsInfo(rowCount, this._sheet.RowHeader.ColumnCount);
                    CopyMoveRowsInfo rowsInfo = new CopyMoveRowsInfo(rowCount);
                    CopyMoveHelper.SaveRowHeaderInfo(this._sheet, headerCellsInfo, rowsInfo, baseRow, this._option);
                    this._savedFromRowHeaderCells = headerCellsInfo;
                    this._savedFromRows = rowsInfo;
                    CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(rowCount, columnCount);
                    CopyMoveHelper.SaveViewportInfo(this._sheet, cellsInfo, baseRow, baseColumn, this._option);
                    this._savedFromViewportCells = cellsInfo;
                }
            }
            else
            {
                if (this._dragDropExtent.FromRow < 0)
                {
                    CopyMoveCellsInfo info4 = new CopyMoveCellsInfo(this._sheet.ColumnHeader.RowCount, columnCount);
                    CopyMoveColumnsInfo columnsInfo = new CopyMoveColumnsInfo(columnCount);
                    CopyMoveHelper.SaveColumnHeaderInfo(this._sheet, info4, columnsInfo, num4, this._option);
                    this._savedToColumnHeaderCells = info4;
                    this._savedToColumns = columnsInfo;
                    if (!this._copy)
                    {
                        CopyMoveCellsInfo info6 = new CopyMoveCellsInfo(this._sheet.ColumnHeader.RowCount, columnCount);
                        CopyMoveColumnsInfo info7 = new CopyMoveColumnsInfo(columnCount);
                        CopyMoveHelper.SaveColumnHeaderInfo(this._sheet, info6, info7, baseColumn, this._option);
                        this._savedFromColumnHeaderCells = info6;
                        this._savedFromColumns = info7;
                    }
                }
                if (this._dragDropExtent.FromColumn < 0)
                {
                    CopyMoveCellsInfo info8 = new CopyMoveCellsInfo(rowCount, this._sheet.RowHeader.ColumnCount);
                    CopyMoveRowsInfo info9 = new CopyMoveRowsInfo(rowCount);
                    CopyMoveHelper.SaveRowHeaderInfo(this._sheet, info8, info9, row, this._option);
                    this._savedToRowHeaderCells = info8;
                    this._savedToRows = info9;
                    if (!this._copy)
                    {
                        CopyMoveCellsInfo info10 = new CopyMoveCellsInfo(rowCount, this._sheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo info11 = new CopyMoveRowsInfo(rowCount);
                        CopyMoveHelper.SaveRowHeaderInfo(this._sheet, info10, info11, baseRow, this._option);
                        this._savedFromRowHeaderCells = info10;
                        this._savedFromRows = info11;
                    }
                }
                CopyMoveCellsInfo info12 = new CopyMoveCellsInfo(rowCount, columnCount);
                CopyMoveHelper.SaveViewportInfo(this._sheet, info12, row, num4, this._option);
                this._savedToViewportCells = info12;
                if (!this._copy)
                {
                    CopyMoveCellsInfo info13 = new CopyMoveCellsInfo(rowCount, columnCount);
                    CopyMoveHelper.SaveViewportInfo(this._sheet, info13, baseRow, baseColumn, this._option);
                    this._savedFromViewportCells = info13;
                    if ((this._option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        CellRange range = new CellRange(this._dragDropExtent.FromRow, this._dragDropExtent.FromColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount);
                        FloatingObject[] floatingObjectsInRange = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(range, this._sheet.RowCount, this._sheet.ColumnCount), this._sheet);
                        this._savedFromFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        this._savedFromFloatingObjects.SaveFloatingObjects(range, floatingObjectsInRange);
                    }
                }
                if ((this._option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                {
                    CellRange range3 = new CellRange(this._dragDropExtent.ToRow, this._dragDropExtent.ToColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount);
                    FloatingObject[] floatingObjects = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(range3, this._sheet.RowCount, this._sheet.ColumnCount), this._sheet);
                    this._savedToFloatingObjects = new CopyMoveFloatingObjectsInfo();
                    this._savedToFloatingObjects.SaveFloatingObjects(range3, floatingObjects);
                }
            }
            this._savedAcitveRowViewportIndex = this._sheet.GetActiveRowViewportIndex();
            this._savedAcitveColumnViewportIndex = this._sheet.GetActiveColumnViewportIndex();
            this._savedActiveRow = this._sheet.ActiveRowIndex;
            this._savedActiveColumn = this._sheet.ActiveColumnIndex;
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
            if (!SheetView.IsValidRange(this._dragDropExtent.FromRow, this._dragDropExtent.FromColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount, this._sheet.RowCount, this._sheet.ColumnCount))
            {
                return false;
            }
            if (!this._insert && !SheetView.IsValidRange(this._dragDropExtent.ToRow, this._dragDropExtent.ToColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount, this._sheet.RowCount, this._sheet.ColumnCount))
            {
                return false;
            }
            bool flag = false;
            SheetView sheetView = parameter as SheetView;
            if (this._insert)
            {
                if ((this._dragDropExtent.FromColumn < 0) || (this._dragDropExtent.FromRow < 0))
                {
                    if (this._dragDropExtent.FromColumn >= 0)
                    {
                        int fromColumn = this._dragDropExtent.FromColumn;
                        int columnCount = this._dragDropExtent.ColumnCount;
                        base.SuspendInvalidate(parameter);
                        try
                        {
                            if (this._copy)
                            {
                                this._sheet.RemoveColumns(this._dragDropExtent.ToColumn, columnCount);
                            }
                            else
                            {
                                int toColumn = this._dragDropExtent.ToColumn;
                                int column = this._dragDropExtent.FromColumn;
                                if (this._dragDropExtent.FromColumn < this._dragDropExtent.ToColumn)
                                {
                                    toColumn = this._dragDropExtent.ToColumn - columnCount;
                                }
                                else
                                {
                                    column = this._dragDropExtent.FromColumn + columnCount;
                                }
                                this._sheet.AddColumns(column, columnCount);
                                this._sheet.CopyTo(-1, (column <= toColumn) ? (toColumn + columnCount) : toColumn, -1, column, -1, columnCount, this._option);
                                this._sheet.RemoveColumns((column <= toColumn) ? (toColumn + columnCount) : toColumn, columnCount);
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
                            CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections);
                            sheetView.SetSelection(-1, fromColumn, -1, columnCount);
                            if (sheetView.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                            sheetView.InvalidateFloatingObjects();
                        }
                        flag = true;
                    }
                    else if (this._dragDropExtent.FromRow >= 0)
                    {
                        int rowCount = this._dragDropExtent.RowCount;
                        int fromRow = this._dragDropExtent.FromRow;
                        base.SuspendInvalidate(parameter);
                        try
                        {
                            if (this._copy)
                            {
                                this._sheet.RemoveRows(this._dragDropExtent.ToRow, rowCount);
                            }
                            else
                            {
                                int toRow = this._dragDropExtent.ToRow;
                                int row = this._dragDropExtent.FromRow;
                                if (this._dragDropExtent.FromRow < this._dragDropExtent.ToRow)
                                {
                                    toRow = this._dragDropExtent.ToRow - rowCount;
                                }
                                else
                                {
                                    row = this._dragDropExtent.FromRow + rowCount;
                                }
                                this._sheet.AddRows(row, rowCount);
                                if (this._savedFromViewportCells != null)
                                {
                                    CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedFromViewportCells, row, 0, SheetArea.Cells);
                                    flag = true;
                                }
                                if (this._savedFromRowHeaderCells != null)
                                {
                                    CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedFromRowHeaderCells, row, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                                    flag = true;
                                }
                                if (this._savedFromRows != null)
                                {
                                    CopyMoveHelper.UndoRowsInfo(this._sheet, this._savedFromRows, row);
                                    flag = true;
                                }
                                if (!flag)
                                {
                                    this._sheet.MoveTo((row <= toRow) ? (toRow + rowCount) : toRow, -1, row, -1, rowCount, -1, this._option);
                                }
                                this._sheet.RemoveRows((row <= toRow) ? (toRow + rowCount) : toRow, rowCount);
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
                            CellRange[] rangeArray2 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections);
                            sheetView.SetSelection(fromRow, -1, rowCount, -1);
                            if (sheetView.RaiseSelectionChanging(rangeArray2, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections)))
                            {
                                sheetView.RaiseSelectionChanged();
                            }
                            sheetView.Invalidate();
                            sheetView.UpdateLayout();
                            sheetView.InvalidateFloatingObjects();
                        }
                        flag = true;
                    }
                }
            }
            else
            {
                int num9 = (this._dragDropExtent.FromRow < 0) ? 0 : this._dragDropExtent.FromRow;
                int num10 = (this._dragDropExtent.FromColumn < 0) ? 0 : this._dragDropExtent.FromColumn;
                int num11 = (this._dragDropExtent.ToRow < 0) ? 0 : this._dragDropExtent.ToRow;
                int num12 = (this._dragDropExtent.ToColumn < 0) ? 0 : this._dragDropExtent.ToColumn;
                int num13 = (this._dragDropExtent.FromRow < 0) ? this._sheet.RowCount : this._dragDropExtent.RowCount;
                int num14 = (this._dragDropExtent.FromColumn < 0) ? this._sheet.ColumnCount : this._dragDropExtent.ColumnCount;
                List<CellData> oldValues = null;
                List<CellData> list2 = null;
                if ((!this._copy && (this._savedFromViewportCells != null)) && this._savedFromViewportCells.IsValueSaved())
                {
                    list2 = CopyMoveHelper.GetValues(this._sheet, num9, num10, num13, num14);
                }
                if ((this._savedToViewportCells != null) && this._savedToViewportCells.IsValueSaved())
                {
                    oldValues = CopyMoveHelper.GetValues(this._sheet, num11, num12, num13, num14);
                }
                base.SuspendInvalidate(parameter);
                try
                {
                    if (this._savedToColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedToColumnHeaderCells, 0, num12, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (this._savedToColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(this._sheet, this._savedToColumns, num12);
                        flag = true;
                    }
                    if (this._savedToViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedToViewportCells, num11, num12, SheetArea.Cells);
                        flag = true;
                    }
                    if (this._savedToRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedToRowHeaderCells, num11, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (this._savedToRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(this._sheet, this._savedToRows, num11);
                        flag = true;
                    }
                    if (this._savedToFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(this._sheet, this._savedToFloatingObjects);
                        sheetView.InvalidateFloatingObjects();
                        flag = true;
                    }
                    if (this._savedFromColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedFromColumnHeaderCells, 0, num10, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (this._savedFromColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(this._sheet, this._savedFromColumns, num10);
                        flag = true;
                    }
                    if (this._savedFromViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedFromViewportCells, num9, num10, SheetArea.Cells);
                        flag = true;
                    }
                    if (this._savedFromRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._sheet, this._savedFromRowHeaderCells, num9, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (this._savedFromRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(this._sheet, this._savedFromRows, num9);
                        flag = true;
                    }
                    if (this._savedFromFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(this._sheet, this._savedFromFloatingObjects);
                        sheetView.InvalidateFloatingObjects();
                        flag = true;
                    }
                }
                finally
                {
                    base.ResumeInvalidate(parameter);
                }
                if (flag && (sheetView != null))
                {
                    CellRange[] rangeArray3 = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections);
                    sheetView.SetSelection(this._dragDropExtent.FromRow, this._dragDropExtent.FromColumn, this._dragDropExtent.RowCount, this._dragDropExtent.ColumnCount);
                    if (sheetView.RaiseSelectionChanging(rangeArray3, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) sheetView.Worksheet.Selections)))
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
                    sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    sheetView.InvalidateFloatingObjects();
                }
            }
            if (flag && (sheetView != null))
            {
                if ((this._savedAcitveRowViewportIndex != -2) && (this._savedAcitveColumnViewportIndex != -2))
                {
                    sheetView.SetActiveRowViewportIndex(this._savedAcitveRowViewportIndex);
                    sheetView.SetActiveColumnViewportIndex(this._savedAcitveColumnViewportIndex);
                }
                if ((this._savedActiveRow != -1) && (this._savedActiveColumn != -1))
                {
                    CellRange range = sheetView.Worksheet.Selections[0];
                    if (range.Contains(this._savedActiveRow, this._savedActiveColumn))
                    {
                        sheetView.SetActiveCell(this._savedActiveRow, this._savedActiveColumn, false);
                    }
                    else
                    {
                        sheetView.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), false);
                    }
                }
                if (((this._savedAcitveRowViewportIndex != -2) && (this._savedAcitveColumnViewportIndex != -2)) && ((this._savedActiveRow != -1) && (this._savedActiveColumn != -1)))
                {
                    sheetView.ShowCell(this._savedAcitveRowViewportIndex, this._savedAcitveColumnViewportIndex, this._savedActiveRow, this._savedActiveColumn, VerticalPosition.Nearest, HorizontalPosition.Nearest);
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
            get { return  this._dragDropExtent; }
        }
    }
}

