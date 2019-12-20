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
    /// Represents the Clipboard paste undo action for pasting on the sheet.
    /// </summary>
    public class ClipboardPasteUndoAction : ActionBase, IUndo
    {
        private ClipboardPasteRangeUndoAction[] _cachedActions;
        private Worksheet _destSheet;
        private Worksheet _sourceSheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ClipboardPasteUndoAction.ClipboardPasteRangeUndoAction" /> class.
        /// </summary>
        /// <param name="srcSheet">The source worksheet of the copy or cut.</param>
        /// <param name="destSheet">The target sheet of the paste.</param>
        /// <param name="pasteExtent">The paste extent information.</param>
        /// <param name="option">The Clipboard pasting option that indicates which content to paste.</param>
        public ClipboardPasteUndoAction(Worksheet srcSheet, Worksheet destSheet, ClipboardPasteExtent pasteExtent, ClipboardPasteOptions option)
        {
            if (destSheet == null)
            {
                throw new ArgumentNullException("destSheet");
            }
            if (pasteExtent == null)
            {
                throw new ArgumentNullException("pasteExtent");
            }
            this._sourceSheet = srcSheet;
            this._destSheet = destSheet;
            if ((pasteExtent.TargetRanges != null) && (pasteExtent.TargetRanges.Length > 0))
            {
                this._cachedActions = new ClipboardPasteRangeUndoAction[pasteExtent.TargetRanges.Length];
                for (int i = 0; i < pasteExtent.TargetRanges.Length; i++)
                {
                    ClipboardPasteRangeExtent extent = new ClipboardPasteRangeExtent(pasteExtent.SourceRange, pasteExtent.TargetRanges[i], pasteExtent.IsCutting, pasteExtent.ClipboardText);
                    this._cachedActions[i] = new ClipboardPasteRangeUndoAction(srcSheet, destSheet, extent, option);
                }
            }
        }

        /// <summary>
        /// Determines whether this instance can execute the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns>
        /// <c>true</c> if this instance can execute the specified sender; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object sender)
        {
            if (this._cachedActions == null)
            {
                return false;
            }
            foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
            {
                if (!action.CanExecute(sender))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (this._cachedActions != null)
            {
                SheetView view = sender as SheetView;
                foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
                {
                    if (view == null)
                    {
                        action.Execute(sender);
                    }
                    else
                    {
                        ClipboardPasteOptions options;
                        action.SaveState();
                        if (!view.RaiseClipboardPasting(this._sourceSheet, action.SourceRange, this._destSheet, action.PasteRange, action.PasteOption, action.IsCutting, out options))
                        {
                            action.PasteOption = options;
                            action.Execute(sender);
                            view.RaiseClipboardPasted(this._sourceSheet, action.SourceRange, this._destSheet, action.PasteRange, options);
                        }
                    }
                }
                this.RefreshSelection(sender);
                this.RefreshUI(sender);
                if (view != null)
                {
                    view.InvalidateFloatingObjects();
                }
            }
        }

        private void RefreshSelection(object sender)
        {
            SheetView view = sender as SheetView;
            if ((view != null) && (this._cachedActions != null))
            {
                CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.Worksheet.Selections);
                view.Worksheet.ClearSelections();
                if (this._cachedActions.Length > 1)
                {
                    foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
                    {
                        view.Worksheet.AddSelection(action.PasteRange, false);
                    }
                }
                else if (this._cachedActions.Length > 0)
                {
                    CellRange pasteRange = this._cachedActions[0].PasteRange;
                    view.Worksheet.AddSelection(pasteRange.Row, pasteRange.Column, pasteRange.RowCount, pasteRange.ColumnCount, false);
                    if (!pasteRange.Contains(view.Worksheet.ActiveRowIndex, view.Worksheet.ActiveColumnIndex))
                    {
                        view.SetActiveCell(Math.Max(0, pasteRange.Row), Math.Max(0, pasteRange.Column), false);
                    }
                }
                if (view.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) view.Worksheet.Selections)))
                {
                    view.RaiseSelectionChanged();
                }
            }
        }

        private void RefreshUI(object sender)
        {
            SheetView view = sender as SheetView;
            if (view != null)
            {
                view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            if (this._cachedActions != null)
            {
                foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
                {
                    action.SaveState();
                }
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionClipboardPaste;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns></returns>
        public bool Undo(object sender)
        {
            if (this._cachedActions == null)
            {
                return false;
            }
            bool flag = true;
            foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
            {
                flag &= action.Undo(sender);
            }
            this.RefreshUI(sender);
            return flag;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (this._cachedActions == null)
                {
                    return false;
                }
                foreach (ClipboardPasteRangeUndoAction action in this._cachedActions)
                {
                    if (!action.CanUndo)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private class ClipboardPasteRangeExtent
        {
            private string _clipboardText;
            private bool _isCutting;
            private CellRange _sourceRange;
            private CellRange _targetRanges;

            public ClipboardPasteRangeExtent(CellRange sourceRange, CellRange targetRange, bool isCutting, string clipboardText)
            {
                this._sourceRange = sourceRange;
                this._targetRanges = targetRange;
                this._isCutting = isCutting;
                this._clipboardText = clipboardText;
            }

            public string ClipboardText
            {
                get { return  this._clipboardText; }
            }

            public bool IsCutting
            {
                get { return  this._isCutting; }
            }

            public CellRange SourceRange
            {
                get { return  this._sourceRange; }
            }

            public CellRange TargetRange
            {
                get { return  this._targetRanges; }
            }
        }

        private class ClipboardPasteRangeUndoAction : ActionBase, IUndo
        {
            private Worksheet _fromSheet;
            private ClipboardPasteUndoAction.ClipboardPasteRangeExtent _pasteExtent;
            private ClipboardPasteOptions _pasteOption;
            private CopyMoveCellsInfo _savedFromColumnHeaderCells;
            private CopyMoveColumnsInfo _savedFromColumns;
            private CopyMoveFloatingObjectsInfo _savedFromFloatingObjects;
            private CopyMoveCellsInfo _savedFromRowHeaderCells;
            private CopyMoveRowsInfo _savedFromRows;
            private CopyMoveSheetInfo _savedFromSheetInfo;
            private CopyMoveCellsInfo _savedFromViewportCells;
            private CopyMoveCellsInfo _savedToColumnHeaderCells;
            private CopyMoveColumnsInfo _savedToColumns;
            private CopyMoveFloatingObjectsInfo _savedToFloatingObjects;
            private CopyMoveCellsInfo _savedToRowHeaderCells;
            private CopyMoveRowsInfo _savedToRows;
            private CopyMoveSheetInfo _savedToSheetInfo;
            private CopyMoveCellsInfo _savedToViewportCells;
            private Worksheet _toSheet;

            public ClipboardPasteRangeUndoAction(Worksheet srcSheet, Worksheet destSheet, ClipboardPasteUndoAction.ClipboardPasteRangeExtent pasteExtent, ClipboardPasteOptions option)
            {
                this._fromSheet = srcSheet;
                this._toSheet = destSheet;
                this._pasteExtent = pasteExtent;
                this._pasteOption = option;
            }

            public override bool CanExecute(object sender)
            {
                return true;
            }

            public override void Execute(object sender)
            {
                CellRange sourceRange = this._pasteExtent.SourceRange;
                CellRange targetRange = this._pasteExtent.TargetRange;
                if (((((this._fromSheet == null) || (sourceRange == null)) || SheetView.IsValidRange(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, this._fromSheet.RowCount, this._fromSheet.ColumnCount)) && ((this._toSheet != null) && (targetRange != null))) && SheetView.IsValidRange(targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, this._toSheet.RowCount, this._toSheet.ColumnCount))
                {
                    base.SuspendInvalidate(sender);
                    try
                    {
                        SheetView.ClipboardPaste(this._fromSheet, sourceRange, this._toSheet, targetRange, this._pasteExtent.IsCutting, this._pasteExtent.ClipboardText, this._pasteOption);
                        SheetView sheetView = sender as SheetView;
                        if (sheetView != null)
                        {
                            if ((this._pasteExtent.IsCutting && (this._savedFromViewportCells != null)) && (this._savedFromViewportCells.IsValueSaved() && object.ReferenceEquals(sheetView.Worksheet, this._fromSheet)))
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, this._savedFromViewportCells.GetValues());
                            }
                            if (((this._savedToViewportCells != null) && this._savedToViewportCells.IsValueSaved()) && object.ReferenceEquals(sheetView.Worksheet, this._toSheet))
                            {
                                CopyMoveHelper.RaiseValueChanged(sheetView, targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, this._savedToViewportCells.GetValues());
                            }
                        }
                    }
                    finally
                    {
                        base.ResumeInvalidate(sender);
                    }
                }
            }

            private void InitSaveState()
            {
                this._savedFromSheetInfo = null;
                this._savedFromColumnHeaderCells = null;
                this._savedFromColumns = null;
                this._savedFromViewportCells = null;
                this._savedFromRowHeaderCells = null;
                this._savedFromRows = null;
                this._savedFromFloatingObjects = null;
                this._savedToSheetInfo = null;
                this._savedToColumnHeaderCells = null;
                this._savedToColumns = null;
                this._savedToViewportCells = null;
                this._savedToRowHeaderCells = null;
                this._savedToRows = null;
                this._savedToFloatingObjects = null;
            }

            public void SaveState()
            {
                this.InitSaveState();
                bool isCutting = this._pasteExtent.IsCutting;
                CopyToOption option = SheetView.ConvertPasteOption(this._pasteOption);
                CellRange sourceRange = this._pasteExtent.SourceRange;
                CellRange targetRange = this._pasteExtent.TargetRange;
                if (((this._fromSheet != null) && (sourceRange != null)) && isCutting)
                {
                    int num = (sourceRange.Row < 0) ? 0 : sourceRange.Row;
                    int num2 = (sourceRange.Column < 0) ? 0 : sourceRange.Column;
                    int num3 = (sourceRange.Row < 0) ? this._fromSheet.RowCount : sourceRange.RowCount;
                    int num4 = (sourceRange.Column < 0) ? this._fromSheet.ColumnCount : sourceRange.ColumnCount;
                    if ((((sourceRange.Row < 0) && (sourceRange.Column < 0)) && ((targetRange.Row < 0) && (targetRange.Column < 0))) && !object.ReferenceEquals(this._fromSheet, this._toSheet))
                    {
                        CopyMoveSheetInfo sheetInfo = new CopyMoveSheetInfo();
                        CopyMoveHelper.SaveSheetInfo(this._fromSheet, sheetInfo, option);
                        this._savedFromSheetInfo = sheetInfo;
                    }
                    if (sourceRange.Row < 0)
                    {
                        CopyMoveCellsInfo headerCellsInfo = new CopyMoveCellsInfo(this._fromSheet.ColumnHeader.RowCount, num4);
                        CopyMoveColumnsInfo columnsInfo = new CopyMoveColumnsInfo(num4);
                        CopyMoveHelper.SaveColumnHeaderInfo(this._fromSheet, headerCellsInfo, columnsInfo, num2, option);
                        this._savedFromColumnHeaderCells = headerCellsInfo;
                        this._savedFromColumns = columnsInfo;
                    }
                    if (sourceRange.Column < 0)
                    {
                        CopyMoveCellsInfo info4 = new CopyMoveCellsInfo(num3, this._fromSheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo rowsInfo = new CopyMoveRowsInfo(num3);
                        CopyMoveHelper.SaveRowHeaderInfo(this._fromSheet, info4, rowsInfo, num, option);
                        this._savedFromRowHeaderCells = info4;
                        this._savedFromRows = rowsInfo;
                    }
                    CopyMoveCellsInfo info6 = new CopyMoveCellsInfo(num3, num4);
                    CopyMoveHelper.SaveViewportInfo(this._fromSheet, info6, num, num2, option);
                    this._savedFromViewportCells = info6;
                    if ((option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        FloatingObject[] floatingObjectsInRange = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(sourceRange, this._fromSheet.RowCount, this._fromSheet.ColumnCount), this._fromSheet);
                        this._savedFromFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        this._savedFromFloatingObjects.SaveFloatingObjects(sourceRange, floatingObjectsInRange);
                    }
                }
                int baseRow = (targetRange.Row < 0) ? 0 : targetRange.Row;
                int baseColumn = (targetRange.Column < 0) ? 0 : targetRange.Column;
                int rowCount = (targetRange.Row < 0) ? this._toSheet.RowCount : targetRange.RowCount;
                int columnCount = (targetRange.Column < 0) ? this._toSheet.ColumnCount : targetRange.ColumnCount;
                if ((this._fromSheet != null) && (sourceRange != null))
                {
                    if ((((sourceRange.Row < 0) && (sourceRange.Column < 0)) && ((targetRange.Row < 0) && (targetRange.Column < 0))) && !object.ReferenceEquals(this._fromSheet, this._toSheet))
                    {
                        CopyMoveSheetInfo info7 = new CopyMoveSheetInfo();
                        CopyMoveHelper.SaveSheetInfo(this._toSheet, info7, option);
                        this._savedToSheetInfo = info7;
                    }
                    if (sourceRange.Row < 0)
                    {
                        CopyMoveCellsInfo info8 = new CopyMoveCellsInfo(this._toSheet.ColumnHeader.RowCount, columnCount);
                        CopyMoveColumnsInfo info9 = new CopyMoveColumnsInfo(columnCount);
                        CopyMoveHelper.SaveColumnHeaderInfo(this._toSheet, info8, info9, baseColumn, option);
                        this._savedToColumnHeaderCells = info8;
                        this._savedToColumns = info9;
                    }
                    if (sourceRange.Column < 0)
                    {
                        CopyMoveCellsInfo info10 = new CopyMoveCellsInfo(rowCount, this._toSheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo info11 = new CopyMoveRowsInfo(rowCount);
                        CopyMoveHelper.SaveRowHeaderInfo(this._toSheet, info10, info11, baseRow, option);
                        this._savedToRowHeaderCells = info10;
                        this._savedToRows = info11;
                    }
                    if ((option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        FloatingObject[] floatingObjects = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(targetRange, this._toSheet.RowCount, this._toSheet.ColumnCount), this._toSheet);
                        this._savedToFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        this._savedToFloatingObjects.SaveFloatingObjects(targetRange, floatingObjects);
                    }
                }
                CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(rowCount, columnCount);
                CopyMoveHelper.SaveViewportInfo(this._toSheet, cellsInfo, baseRow, baseColumn, option);
                this._savedToViewportCells = cellsInfo;
            }

            public bool Undo(object sender)
            {
                CellRange sourceRange = this._pasteExtent.SourceRange;
                CellRange targetRange = this._pasteExtent.TargetRange;
                SheetView sheetView = sender as SheetView;
                if ((this._toSheet == null) || (targetRange == null))
                {
                    return false;
                }
                if (!SheetView.IsValidRange(targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, this._toSheet.RowCount, this._toSheet.ColumnCount))
                {
                    return false;
                }
                if ((this._fromSheet != null) && (sourceRange != null))
                {
                    if (!SheetView.IsValidRange(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, this._fromSheet.RowCount, this._fromSheet.ColumnCount))
                    {
                        return false;
                    }
                    if (((this._fromSheet.Workbook != null) && object.ReferenceEquals(this._fromSheet.Workbook, this._toSheet.Workbook)) && !this._toSheet.Workbook.Sheets.Contains(this._fromSheet))
                    {
                        return false;
                    }
                }
                base.SuspendInvalidate(sender);
                bool flag = false;
                try
                {
                    List<CellData> oldValues = null;
                    List<CellData> list2 = null;
                    int row = (targetRange.Row < 0) ? 0 : targetRange.Row;
                    int column = (targetRange.Column < 0) ? 0 : targetRange.Column;
                    int rowCount = (targetRange.Row < 0) ? this._toSheet.RowCount : targetRange.RowCount;
                    int columnCount = (targetRange.Column < 0) ? this._toSheet.ColumnCount : targetRange.ColumnCount;
                    if (this._savedToSheetInfo != null)
                    {
                        CopyMoveHelper.UndoSheetInfo(this._toSheet, this._savedToSheetInfo);
                        flag = true;
                    }
                    if ((this._savedToViewportCells != null) && this._savedToViewportCells.IsValueSaved())
                    {
                        oldValues = CopyMoveHelper.GetValues(this._toSheet, row, column, rowCount, columnCount);
                    }
                    if (this._savedToColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._toSheet, this._savedToColumnHeaderCells, 0, column, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (this._savedToColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(this._toSheet, this._savedToColumns, column);
                        flag = true;
                    }
                    if (this._savedToViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._toSheet, this._savedToViewportCells, row, column, SheetArea.Cells);
                        flag = true;
                    }
                    if (this._savedToRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(this._toSheet, this._savedToRowHeaderCells, row, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (this._savedToRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(this._toSheet, this._savedToRows, row);
                        flag = true;
                    }
                    if (this._savedToFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(this._toSheet, this._savedToFloatingObjects);
                        sheetView.InvalidateFloatingObjects();
                        flag = true;
                    }
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    if ((this._fromSheet != null) && (sourceRange != null))
                    {
                        num5 = (sourceRange.Row < 0) ? 0 : sourceRange.Row;
                        num6 = (sourceRange.Column < 0) ? 0 : sourceRange.Column;
                        num7 = (sourceRange.Row < 0) ? this._fromSheet.RowCount : sourceRange.RowCount;
                        num8 = (sourceRange.Column < 0) ? this._fromSheet.ColumnCount : sourceRange.ColumnCount;
                        if (this._savedFromSheetInfo != null)
                        {
                            CopyMoveHelper.UndoSheetInfo(this._fromSheet, this._savedFromSheetInfo);
                            flag = true;
                        }
                        if ((this._savedFromViewportCells != null) && this._savedFromViewportCells.IsValueSaved())
                        {
                            list2 = CopyMoveHelper.GetValues(this._fromSheet, num5, num6, num7, num8);
                        }
                        if (this._savedFromColumnHeaderCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(this._fromSheet, this._savedFromColumnHeaderCells, 0, num6, SheetArea.ColumnHeader);
                            flag = true;
                        }
                        if (this._savedFromColumns != null)
                        {
                            CopyMoveHelper.UndoColumnsInfo(this._fromSheet, this._savedFromColumns, num6);
                            flag = true;
                        }
                        if (this._savedFromViewportCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(this._fromSheet, this._savedFromViewportCells, num5, num6, SheetArea.Cells);
                            flag = true;
                        }
                        if (this._savedFromRowHeaderCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(this._fromSheet, this._savedFromRowHeaderCells, num5, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                            flag = true;
                        }
                        if (this._savedFromRows != null)
                        {
                            CopyMoveHelper.UndoRowsInfo(this._fromSheet, this._savedFromRows, num5);
                            flag = true;
                        }
                        if (this._savedFromFloatingObjects != null)
                        {
                            CopyMoveHelper.UndoFloatingObjectsInfo(this._fromSheet, this._savedFromFloatingObjects);
                            sheetView.InvalidateFloatingObjects();
                            flag = true;
                        }
                    }
                    if (!flag || (sheetView == null))
                    {
                        return flag;
                    }
                    if ((oldValues != null) && object.ReferenceEquals(sheetView.Worksheet, this._toSheet))
                    {
                        CopyMoveHelper.RaiseValueChanged(sheetView, row, column, rowCount, columnCount, oldValues);
                    }
                    if ((list2 != null) && object.ReferenceEquals(sheetView.Worksheet, this._fromSheet))
                    {
                        CopyMoveHelper.RaiseValueChanged(sheetView, num5, num6, num7, num8, list2);
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                    if (this._savedFromFloatingObjects != null)
                    {
                        sheetView.InvalidateFloatingObjects();
                    }
                }
                return flag;
            }

            public bool CanUndo
            {
                get { return  true; }
            }

            internal bool IsCutting
            {
                get { return  this._pasteExtent.IsCutting; }
            }

            internal ClipboardPasteOptions PasteOption
            {
                get { return  this._pasteOption; }
                set { this._pasteOption = value; }
            }

            internal CellRange PasteRange
            {
                get { return  this._pasteExtent.TargetRange; }
            }

            internal CellRange SourceRange
            {
                get { return  this._pasteExtent.SourceRange; }
            }
        }
    }
}

