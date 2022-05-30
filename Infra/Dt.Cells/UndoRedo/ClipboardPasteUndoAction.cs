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
    /// Represents the Clipboard paste undo action for pasting on the sheet.
    /// </summary>
    public class ClipboardPasteUndoAction : ActionBase, IUndo
    {
        ClipboardPasteRangeUndoAction[] _cachedActions;
        Worksheet _destSheet;
        Worksheet _sourceSheet;

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
            _sourceSheet = srcSheet;
            _destSheet = destSheet;
            if ((pasteExtent.TargetRanges != null) && (pasteExtent.TargetRanges.Length > 0))
            {
                _cachedActions = new ClipboardPasteRangeUndoAction[pasteExtent.TargetRanges.Length];
                for (int i = 0; i < pasteExtent.TargetRanges.Length; i++)
                {
                    ClipboardPasteRangeExtent extent = new ClipboardPasteRangeExtent(pasteExtent.SourceRange, pasteExtent.TargetRanges[i], pasteExtent.IsCutting, pasteExtent.ClipboardText);
                    _cachedActions[i] = new ClipboardPasteRangeUndoAction(srcSheet, destSheet, extent, option);
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
            if (_cachedActions == null)
            {
                return false;
            }
            foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
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
            if (_cachedActions != null)
            {
                var excel = sender as Excel;
                foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
                {
                    if (excel == null)
                    {
                        action.Execute(sender);
                    }
                    else
                    {
                        ClipboardPasteOptions options;
                        action.SaveState();
                        if (!excel.RaiseClipboardPasting(_sourceSheet, action.SourceRange, _destSheet, action.PasteRange, action.PasteOption, action.IsCutting, out options))
                        {
                            action.PasteOption = options;
                            action.Execute(sender);
                            excel.RaiseClipboardPasted(_sourceSheet, action.SourceRange, _destSheet, action.PasteRange, options);
                        }
                    }
                }
                RefreshSelection(sender);
                RefreshUI(sender);
                if (excel != null)
                {
                    excel.RefreshFloatingObjects();
                }
            }
        }

        void RefreshSelection(object sender)
        {
            var excel = sender as Excel;
            if ((excel != null) && (_cachedActions != null))
            {
                CellRange[] oldSelection = Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) excel.ActiveSheet.Selections);
                excel.ActiveSheet.ClearSelections();
                if (_cachedActions.Length > 1)
                {
                    foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
                    {
                        excel.ActiveSheet.AddSelection(action.PasteRange, false);
                    }
                }
                else if (_cachedActions.Length > 0)
                {
                    CellRange pasteRange = _cachedActions[0].PasteRange;
                    excel.ActiveSheet.AddSelection(pasteRange.Row, pasteRange.Column, pasteRange.RowCount, pasteRange.ColumnCount, false);
                    if (!pasteRange.Contains(excel.ActiveSheet.ActiveRowIndex, excel.ActiveSheet.ActiveColumnIndex))
                    {
                        excel.SetActiveCell(Math.Max(0, pasteRange.Row), Math.Max(0, pasteRange.Column), false);
                    }
                }
                if (excel.RaiseSelectionChanging(oldSelection, Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) excel.ActiveSheet.Selections)))
                {
                    excel.RaiseSelectionChanged();
                }
            }
        }

        void RefreshUI(object sender)
        {
            var excel = sender as Excel;
            if (excel != null)
            {
                excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            if (_cachedActions != null)
            {
                foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
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
            if (_cachedActions == null)
            {
                return false;
            }
            bool flag = true;
            foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
            {
                flag &= action.Undo(sender);
            }
            RefreshUI(sender);
            return flag;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                if (_cachedActions == null)
                {
                    return false;
                }
                foreach (ClipboardPasteRangeUndoAction action in _cachedActions)
                {
                    if (!action.CanUndo)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        class ClipboardPasteRangeExtent
        {
            string _clipboardText;
            bool _isCutting;
            CellRange _sourceRange;
            CellRange _targetRanges;

            public ClipboardPasteRangeExtent(CellRange sourceRange, CellRange targetRange, bool isCutting, string clipboardText)
            {
                _sourceRange = sourceRange;
                _targetRanges = targetRange;
                _isCutting = isCutting;
                _clipboardText = clipboardText;
            }

            public string ClipboardText
            {
                get { return  _clipboardText; }
            }

            public bool IsCutting
            {
                get { return  _isCutting; }
            }

            public CellRange SourceRange
            {
                get { return  _sourceRange; }
            }

            public CellRange TargetRange
            {
                get { return  _targetRanges; }
            }
        }

        class ClipboardPasteRangeUndoAction : ActionBase, IUndo
        {
            Worksheet _fromSheet;
            ClipboardPasteUndoAction.ClipboardPasteRangeExtent _pasteExtent;
            ClipboardPasteOptions _pasteOption;
            CopyMoveCellsInfo _savedFromColumnHeaderCells;
            CopyMoveColumnsInfo _savedFromColumns;
            CopyMoveFloatingObjectsInfo _savedFromFloatingObjects;
            CopyMoveCellsInfo _savedFromRowHeaderCells;
            CopyMoveRowsInfo _savedFromRows;
            CopyMoveSheetInfo _savedFromSheetInfo;
            CopyMoveCellsInfo _savedFromViewportCells;
            CopyMoveCellsInfo _savedToColumnHeaderCells;
            CopyMoveColumnsInfo _savedToColumns;
            CopyMoveFloatingObjectsInfo _savedToFloatingObjects;
            CopyMoveCellsInfo _savedToRowHeaderCells;
            CopyMoveRowsInfo _savedToRows;
            CopyMoveSheetInfo _savedToSheetInfo;
            CopyMoveCellsInfo _savedToViewportCells;
            Worksheet _toSheet;

            public ClipboardPasteRangeUndoAction(Worksheet srcSheet, Worksheet destSheet, ClipboardPasteUndoAction.ClipboardPasteRangeExtent pasteExtent, ClipboardPasteOptions option)
            {
                _fromSheet = srcSheet;
                _toSheet = destSheet;
                _pasteExtent = pasteExtent;
                _pasteOption = option;
            }

            public override bool CanExecute(object sender)
            {
                return true;
            }

            public override void Execute(object sender)
            {
                CellRange sourceRange = _pasteExtent.SourceRange;
                CellRange targetRange = _pasteExtent.TargetRange;
                if (((((_fromSheet == null) || (sourceRange == null)) || Excel.IsValidRange(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, _fromSheet.RowCount, _fromSheet.ColumnCount)) && ((_toSheet != null) && (targetRange != null))) && Excel.IsValidRange(targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, _toSheet.RowCount, _toSheet.ColumnCount))
                {
                    base.SuspendInvalidate(sender);
                    try
                    {
                        Excel.ClipboardPaste(_fromSheet, sourceRange, _toSheet, targetRange, _pasteExtent.IsCutting, _pasteExtent.ClipboardText, _pasteOption);
                        Excel excel = sender as Excel;
                        if (excel != null)
                        {
                            if ((_pasteExtent.IsCutting && (_savedFromViewportCells != null)) && (_savedFromViewportCells.IsValueSaved() && object.ReferenceEquals(excel.ActiveSheet, _fromSheet)))
                            {
                                CopyMoveHelper.RaiseValueChanged(excel, sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, _savedFromViewportCells.GetValues());
                            }
                            if (((_savedToViewportCells != null) && _savedToViewportCells.IsValueSaved()) && object.ReferenceEquals(excel.ActiveSheet, _toSheet))
                            {
                                CopyMoveHelper.RaiseValueChanged(excel, targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, _savedToViewportCells.GetValues());
                            }
                        }
                    }
                    finally
                    {
                        base.ResumeInvalidate(sender);
                    }
                }
            }

            void InitSaveState()
            {
                _savedFromSheetInfo = null;
                _savedFromColumnHeaderCells = null;
                _savedFromColumns = null;
                _savedFromViewportCells = null;
                _savedFromRowHeaderCells = null;
                _savedFromRows = null;
                _savedFromFloatingObjects = null;
                _savedToSheetInfo = null;
                _savedToColumnHeaderCells = null;
                _savedToColumns = null;
                _savedToViewportCells = null;
                _savedToRowHeaderCells = null;
                _savedToRows = null;
                _savedToFloatingObjects = null;
            }

            public void SaveState()
            {
                InitSaveState();
                bool isCutting = _pasteExtent.IsCutting;
                CopyToOption option = Excel.ConvertPasteOption(_pasteOption);
                CellRange sourceRange = _pasteExtent.SourceRange;
                CellRange targetRange = _pasteExtent.TargetRange;
                if (((_fromSheet != null) && (sourceRange != null)) && isCutting)
                {
                    int num = (sourceRange.Row < 0) ? 0 : sourceRange.Row;
                    int num2 = (sourceRange.Column < 0) ? 0 : sourceRange.Column;
                    int num3 = (sourceRange.Row < 0) ? _fromSheet.RowCount : sourceRange.RowCount;
                    int num4 = (sourceRange.Column < 0) ? _fromSheet.ColumnCount : sourceRange.ColumnCount;
                    if ((((sourceRange.Row < 0) && (sourceRange.Column < 0)) && ((targetRange.Row < 0) && (targetRange.Column < 0))) && !object.ReferenceEquals(_fromSheet, _toSheet))
                    {
                        CopyMoveSheetInfo sheetInfo = new CopyMoveSheetInfo();
                        CopyMoveHelper.SaveSheetInfo(_fromSheet, sheetInfo, option);
                        _savedFromSheetInfo = sheetInfo;
                    }
                    if (sourceRange.Row < 0)
                    {
                        CopyMoveCellsInfo headerCellsInfo = new CopyMoveCellsInfo(_fromSheet.ColumnHeader.RowCount, num4);
                        CopyMoveColumnsInfo columnsInfo = new CopyMoveColumnsInfo(num4);
                        CopyMoveHelper.SaveColumnHeaderInfo(_fromSheet, headerCellsInfo, columnsInfo, num2, option);
                        _savedFromColumnHeaderCells = headerCellsInfo;
                        _savedFromColumns = columnsInfo;
                    }
                    if (sourceRange.Column < 0)
                    {
                        CopyMoveCellsInfo info4 = new CopyMoveCellsInfo(num3, _fromSheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo rowsInfo = new CopyMoveRowsInfo(num3);
                        CopyMoveHelper.SaveRowHeaderInfo(_fromSheet, info4, rowsInfo, num, option);
                        _savedFromRowHeaderCells = info4;
                        _savedFromRows = rowsInfo;
                    }
                    CopyMoveCellsInfo info6 = new CopyMoveCellsInfo(num3, num4);
                    CopyMoveHelper.SaveViewportInfo(_fromSheet, info6, num, num2, option);
                    _savedFromViewportCells = info6;
                    if ((option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        FloatingObject[] floatingObjectsInRange = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(sourceRange, _fromSheet.RowCount, _fromSheet.ColumnCount), _fromSheet);
                        _savedFromFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        _savedFromFloatingObjects.SaveFloatingObjects(sourceRange, floatingObjectsInRange);
                    }
                }
                int baseRow = (targetRange.Row < 0) ? 0 : targetRange.Row;
                int baseColumn = (targetRange.Column < 0) ? 0 : targetRange.Column;
                int rowCount = (targetRange.Row < 0) ? _toSheet.RowCount : targetRange.RowCount;
                int columnCount = (targetRange.Column < 0) ? _toSheet.ColumnCount : targetRange.ColumnCount;
                if ((_fromSheet != null) && (sourceRange != null))
                {
                    if ((((sourceRange.Row < 0) && (sourceRange.Column < 0)) && ((targetRange.Row < 0) && (targetRange.Column < 0))) && !object.ReferenceEquals(_fromSheet, _toSheet))
                    {
                        CopyMoveSheetInfo info7 = new CopyMoveSheetInfo();
                        CopyMoveHelper.SaveSheetInfo(_toSheet, info7, option);
                        _savedToSheetInfo = info7;
                    }
                    if (sourceRange.Row < 0)
                    {
                        CopyMoveCellsInfo info8 = new CopyMoveCellsInfo(_toSheet.ColumnHeader.RowCount, columnCount);
                        CopyMoveColumnsInfo info9 = new CopyMoveColumnsInfo(columnCount);
                        CopyMoveHelper.SaveColumnHeaderInfo(_toSheet, info8, info9, baseColumn, option);
                        _savedToColumnHeaderCells = info8;
                        _savedToColumns = info9;
                    }
                    if (sourceRange.Column < 0)
                    {
                        CopyMoveCellsInfo info10 = new CopyMoveCellsInfo(rowCount, _toSheet.RowHeader.ColumnCount);
                        CopyMoveRowsInfo info11 = new CopyMoveRowsInfo(rowCount);
                        CopyMoveHelper.SaveRowHeaderInfo(_toSheet, info10, info11, baseRow, option);
                        _savedToRowHeaderCells = info10;
                        _savedToRows = info11;
                    }
                    if ((option & CopyToOption.FloatingObject) > ((CopyToOption) 0))
                    {
                        FloatingObject[] floatingObjects = CopyMoveHelper.GetFloatingObjectsInRange(CopyMoveHelper.AdjustRange(targetRange, _toSheet.RowCount, _toSheet.ColumnCount), _toSheet);
                        _savedToFloatingObjects = new CopyMoveFloatingObjectsInfo();
                        _savedToFloatingObjects.SaveFloatingObjects(targetRange, floatingObjects);
                    }
                }
                CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(rowCount, columnCount);
                CopyMoveHelper.SaveViewportInfo(_toSheet, cellsInfo, baseRow, baseColumn, option);
                _savedToViewportCells = cellsInfo;
            }

            public bool Undo(object sender)
            {
                CellRange sourceRange = _pasteExtent.SourceRange;
                CellRange targetRange = _pasteExtent.TargetRange;
                Excel excel = sender as Excel;
                if ((_toSheet == null) || (targetRange == null))
                {
                    return false;
                }
                if (!Excel.IsValidRange(targetRange.Row, targetRange.Column, targetRange.RowCount, targetRange.ColumnCount, _toSheet.RowCount, _toSheet.ColumnCount))
                {
                    return false;
                }
                if ((_fromSheet != null) && (sourceRange != null))
                {
                    if (!Excel.IsValidRange(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount, _fromSheet.RowCount, _fromSheet.ColumnCount))
                    {
                        return false;
                    }
                    if (((_fromSheet.Workbook != null) && object.ReferenceEquals(_fromSheet.Workbook, _toSheet.Workbook)) && !_toSheet.Workbook.Sheets.Contains(_fromSheet))
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
                    int rowCount = (targetRange.Row < 0) ? _toSheet.RowCount : targetRange.RowCount;
                    int columnCount = (targetRange.Column < 0) ? _toSheet.ColumnCount : targetRange.ColumnCount;
                    if (_savedToSheetInfo != null)
                    {
                        CopyMoveHelper.UndoSheetInfo(_toSheet, _savedToSheetInfo);
                        flag = true;
                    }
                    if ((_savedToViewportCells != null) && _savedToViewportCells.IsValueSaved())
                    {
                        oldValues = CopyMoveHelper.GetValues(_toSheet, row, column, rowCount, columnCount);
                    }
                    if (_savedToColumnHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_toSheet, _savedToColumnHeaderCells, 0, column, SheetArea.ColumnHeader);
                        flag = true;
                    }
                    if (_savedToColumns != null)
                    {
                        CopyMoveHelper.UndoColumnsInfo(_toSheet, _savedToColumns, column);
                        flag = true;
                    }
                    if (_savedToViewportCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_toSheet, _savedToViewportCells, row, column, SheetArea.Cells);
                        flag = true;
                    }
                    if (_savedToRowHeaderCells != null)
                    {
                        CopyMoveHelper.UndoCellsInfo(_toSheet, _savedToRowHeaderCells, row, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                        flag = true;
                    }
                    if (_savedToRows != null)
                    {
                        CopyMoveHelper.UndoRowsInfo(_toSheet, _savedToRows, row);
                        flag = true;
                    }
                    if (_savedToFloatingObjects != null)
                    {
                        CopyMoveHelper.UndoFloatingObjectsInfo(_toSheet, _savedToFloatingObjects);
                        excel.RefreshFloatingObjects();
                        flag = true;
                    }
                    int num5 = 0;
                    int num6 = 0;
                    int num7 = 0;
                    int num8 = 0;
                    if ((_fromSheet != null) && (sourceRange != null))
                    {
                        num5 = (sourceRange.Row < 0) ? 0 : sourceRange.Row;
                        num6 = (sourceRange.Column < 0) ? 0 : sourceRange.Column;
                        num7 = (sourceRange.Row < 0) ? _fromSheet.RowCount : sourceRange.RowCount;
                        num8 = (sourceRange.Column < 0) ? _fromSheet.ColumnCount : sourceRange.ColumnCount;
                        if (_savedFromSheetInfo != null)
                        {
                            CopyMoveHelper.UndoSheetInfo(_fromSheet, _savedFromSheetInfo);
                            flag = true;
                        }
                        if ((_savedFromViewportCells != null) && _savedFromViewportCells.IsValueSaved())
                        {
                            list2 = CopyMoveHelper.GetValues(_fromSheet, num5, num6, num7, num8);
                        }
                        if (_savedFromColumnHeaderCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(_fromSheet, _savedFromColumnHeaderCells, 0, num6, SheetArea.ColumnHeader);
                            flag = true;
                        }
                        if (_savedFromColumns != null)
                        {
                            CopyMoveHelper.UndoColumnsInfo(_fromSheet, _savedFromColumns, num6);
                            flag = true;
                        }
                        if (_savedFromViewportCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(_fromSheet, _savedFromViewportCells, num5, num6, SheetArea.Cells);
                            flag = true;
                        }
                        if (_savedFromRowHeaderCells != null)
                        {
                            CopyMoveHelper.UndoCellsInfo(_fromSheet, _savedFromRowHeaderCells, num5, 0, SheetArea.CornerHeader | SheetArea.RowHeader);
                            flag = true;
                        }
                        if (_savedFromRows != null)
                        {
                            CopyMoveHelper.UndoRowsInfo(_fromSheet, _savedFromRows, num5);
                            flag = true;
                        }
                        if (_savedFromFloatingObjects != null)
                        {
                            CopyMoveHelper.UndoFloatingObjectsInfo(_fromSheet, _savedFromFloatingObjects);
                            excel.RefreshFloatingObjects();
                            flag = true;
                        }
                    }
                    if (!flag || (excel == null))
                    {
                        return flag;
                    }
                    if ((oldValues != null) && object.ReferenceEquals(excel.ActiveSheet, _toSheet))
                    {
                        CopyMoveHelper.RaiseValueChanged(excel, row, column, rowCount, columnCount, oldValues);
                    }
                    if ((list2 != null) && object.ReferenceEquals(excel.ActiveSheet, _fromSheet))
                    {
                        CopyMoveHelper.RaiseValueChanged(excel, num5, num6, num7, num8, list2);
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                    if (_savedFromFloatingObjects != null)
                    {
                        excel.RefreshFloatingObjects();
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
                get { return  _pasteExtent.IsCutting; }
            }

            internal ClipboardPasteOptions PasteOption
            {
                get { return  _pasteOption; }
                set { _pasteOption = value; }
            }

            internal CellRange PasteRange
            {
                get { return  _pasteExtent.TargetRange; }
            }

            internal CellRange SourceRange
            {
                get { return  _pasteExtent.SourceRange; }
            }
        }
    }
}

