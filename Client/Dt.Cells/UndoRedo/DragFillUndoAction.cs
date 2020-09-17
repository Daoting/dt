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
    /// Represents a drag fill action to drag and fill a range on the worksheet.
    /// </summary>
    public class DragFillUndoAction : ActionBase, IUndo
    {
        List<SheetTable> _cachedTables;
        ClearValueUndoAction _clearValueUndoAction;
        DragFillExtent _dragFillExtent;
        FillSeries _fillSeries;
        CopyMoveCellsInfo _savedFilledViewportCells;
        CopyMoveCellsInfo _savedStartViewportCells;
        CellRange _wholeFillRange;
        Worksheet _workSheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragFillUndoAction" /> class.
        /// </summary>
        /// <param name="workSheet">The drag fill worksheet.</param>
        /// <param name="dragFillExtent">The drag fill extent information.</param>
        public DragFillUndoAction(Worksheet workSheet, DragFillExtent dragFillExtent)
        {
            _workSheet = workSheet;
            _dragFillExtent = dragFillExtent;
            if (_dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                _clearValueUndoAction = new ClearValueUndoAction(workSheet, new CellRange[] { _dragFillExtent.FillRange });
            }
            else
            {
                InitWholeFilledRange();
            }
            if ((_dragFillExtent.FillDirection == FillDirection.Left) || (_dragFillExtent.FillDirection == FillDirection.Right))
            {
                _fillSeries = FillSeries.Row;
            }
            else
            {
                _fillSeries = FillSeries.Column;
            }
        }

        CellRange AdjustRange(CellRange range)
        {
            int row = (range.Row != -1) ? range.Row : 0;
            int column = (range.Column != -1) ? range.Column : 0;
            int rowCount = (range.RowCount != -1) ? range.RowCount : _workSheet.RowCount;
            return new CellRange(row, column, rowCount, (range.ColumnCount != -1) ? range.ColumnCount : _workSheet.ColumnCount);
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            if (_dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                return CanExecuteDragClear();
            }
            return CanExecuteDragFill();
        }

        bool CanExecuteDragClear()
        {
            return true;
        }

        bool CanExecuteDragFill()
        {
            CellRange startRange = _dragFillExtent.StartRange;
            if (_dragFillExtent.FillRange.Intersects(startRange.Row, startRange.Column, startRange.RowCount, startRange.ColumnCount))
            {
                return false;
            }
            return true;
        }

        void ClearData(CellRange cellRange)
        {
            StorageType data = StorageType.Data;
            _workSheet.Clear(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount, SheetArea.Cells, data);
        }

        void CopyCells(CellRange fromRange, CellRange toRange, CopyToOption copyOption)
        {
            CellRange range = AdjustRange(fromRange);
            CellRange range2 = AdjustRange(toRange);
            if (_fillSeries == FillSeries.Column)
            {
                int num = range2.RowCount / range.RowCount;
                for (int i = 0; i < num; i++)
                {
                    int num5;
                    int row = range.Row;
                    int column = range.Column;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num5 = range2.Row + (i * range.RowCount);
                    }
                    else
                    {
                        int num6 = range2.Row + range2.RowCount;
                        num5 = num6 - ((i + 1) * range.RowCount);
                    }
                    int toColumn = range2.Column;
                    int rowCount = range.RowCount;
                    int columnCount = range.ColumnCount;
                    _workSheet.CopyTo(row, column, num5, toColumn, rowCount, columnCount, copyOption);
                }
                int num10 = range2.RowCount % range.RowCount;
                if (num10 != 0)
                {
                    int num11;
                    int num14;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num11 = range.Row;
                    }
                    else
                    {
                        int num12 = range.RowCount - (range2.RowCount - (range.RowCount * num));
                        num11 = range.Row + num12;
                    }
                    int fromColumn = range.Column;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num14 = range2.Row + (range.RowCount * num);
                    }
                    else
                    {
                        int num15 = range2.Row + range2.RowCount;
                        num14 = (num15 - (num * range.RowCount)) - num10;
                    }
                    int num16 = range2.Column;
                    int num17 = num10;
                    int num18 = range.ColumnCount;
                    _workSheet.CopyTo(num11, fromColumn, num14, num16, num17, num18, copyOption);
                }
            }
            else
            {
                int num19 = range2.ColumnCount / range.ColumnCount;
                for (int j = 0; j < num19; j++)
                {
                    int num24;
                    int fromRow = range.Row;
                    int num22 = range.Column;
                    int toRow = range2.Row;
                    if (_dragFillExtent.FillDirection == FillDirection.Right)
                    {
                        num24 = range2.Column + (j * range.ColumnCount);
                    }
                    else
                    {
                        int num25 = range2.Column + range2.ColumnCount;
                        num24 = num25 - ((j + 1) * range.ColumnCount);
                    }
                    int num26 = range.RowCount;
                    int num27 = range.ColumnCount;
                    _workSheet.CopyTo(fromRow, num22, toRow, num24, num26, num27, copyOption);
                }
                int num28 = range2.ColumnCount % range.ColumnCount;
                if (num28 != 0)
                {
                    int num30;
                    int num33;
                    int num29 = range.Row;
                    if (_dragFillExtent.FillDirection == FillDirection.Right)
                    {
                        num30 = range.Column;
                    }
                    else
                    {
                        int num31 = range.ColumnCount - (range2.ColumnCount - (range.ColumnCount * num19));
                        num30 = range.Column + num31;
                    }
                    int num32 = range2.Row;
                    if (_dragFillExtent.FillDirection == FillDirection.Right)
                    {
                        num33 = range2.Column + (range.ColumnCount * num19);
                    }
                    else
                    {
                        int num34 = range2.Column + range2.ColumnCount;
                        num33 = (num34 - (num19 * range.ColumnCount)) - num28;
                    }
                    int num35 = range.RowCount;
                    int num36 = num28;
                    _workSheet.CopyTo(num29, num30, num32, num33, num35, num36, copyOption);
                }
            }
        }

        void Execute(Excel excel)
        {
            CellRange[] oldSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            if (_dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                ExecuteDragFillClear(excel);
            }
            else
            {
                ExecuteDragFill(excel);
            }
            if (((_savedFilledViewportCells != null) && _savedFilledViewportCells.IsValueSaved()) && object.ReferenceEquals(excel.ActiveSheet, _workSheet))
            {
                CellRange fillRange = _dragFillExtent.FillRange;
                CopyMoveHelper.RaiseValueChanged(excel, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount, _savedFilledViewportCells.GetValues());
            }
            CellRange[] newSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            if (excel.RaiseSelectionChanging(oldSelection, newSelection))
            {
                excel.RaiseSelectionChanged();
            }
        }

        /// <summary>
        /// Executes the drag fill action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        public override void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                Excel sender = parameter as Excel;
                try
                {
                    base.SuspendInvalidate(sender);
                    _workSheet.SuspendCalcService();
                    SaveState();
                    Execute(sender);
                }
                finally
                {
                    _workSheet.ResumeCalcService();
                    base.ResumeInvalidate(sender);
                }
                sender.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(_workSheet, _dragFillExtent.FillRange.Row, _dragFillExtent.FillRange.Column, _dragFillExtent.FillRange.RowCount, _dragFillExtent.FillRange.ColumnCount);
                if (list.Count > 0)
                {
                    sender.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                }
                else
                {
                    sender.RefreshFloatingObjects();
                }
            }
        }

        void ExecuteDragFill(Excel excel)
        {
            CellRange startRange = _dragFillExtent.StartRange;
            CellRange fillRange = _dragFillExtent.FillRange;
            if (_dragFillExtent.AutoFillType == AutoFillType.FillSeries)
            {
                ClearData(fillRange);
                if ((_dragFillExtent.FillDirection == FillDirection.Down) || (_dragFillExtent.FillDirection == FillDirection.Right))
                {
                    _workSheet.FillAuto(_wholeFillRange, _fillSeries);
                }
                else
                {
                    _workSheet.FillLinear(_wholeFillRange, _fillSeries);
                }
            }
            else if (_dragFillExtent.AutoFillType == AutoFillType.CopyCells)
            {
                CopyCells(startRange, fillRange, CopyToOption.All);
            }
            else if (_dragFillExtent.AutoFillType == AutoFillType.FillFormattingOnly)
            {
                CopyCells(startRange, fillRange, CopyToOption.Style);
            }
            else if (_dragFillExtent.AutoFillType == AutoFillType.FillWithoutFormatting)
            {
                ClearData(fillRange);
                if ((((startRange.RowCount == 1) && (startRange.ColumnCount == 1)) && ((startRange.Row != -1) || (startRange.Column == -1))) && ((startRange.Column != -1) || (startRange.Row == -1)))
                {
                    CopyToOption copyOption = CopyToOption.Tag | CopyToOption.Span | CopyToOption.Sparkline | CopyToOption.RangeGroup | CopyToOption.Formula | CopyToOption.Value;
                    CopyCells(startRange, fillRange, copyOption);
                }
                else
                {
                    CellRange range3 = AdjustRange(_wholeFillRange);
                    object[,] objArray = new object[range3.RowCount, range3.ColumnCount];
                    for (int i = 0; i < range3.RowCount; i++)
                    {
                        for (int k = 0; k < range3.ColumnCount; k++)
                        {
                            objArray[i, k] = CopyMoveHelper.GetStyleObject(_workSheet, range3.Row + i, range3.Column + k, SheetArea.Cells);
                        }
                    }
                    if ((_dragFillExtent.FillDirection == FillDirection.Down) || (_dragFillExtent.FillDirection == FillDirection.Right))
                    {
                        _workSheet.FillAuto(_wholeFillRange, _fillSeries);
                    }
                    else
                    {
                        _workSheet.FillLinear(_wholeFillRange, _fillSeries);
                    }
                    for (int j = 0; j < range3.RowCount; j++)
                    {
                        for (int m = 0; m < range3.ColumnCount; m++)
                        {
                            CopyMoveHelper.SetStyleObject(_workSheet, range3.Row + j, range3.Column + m, SheetArea.Cells, objArray[j, m]);
                        }
                    }
                }
            }
            excel.SetActiveCell(Math.Max(0, _wholeFillRange.Row), Math.Max(0, _wholeFillRange.Column), true);
            _workSheet.SetSelection(_wholeFillRange);
        }

        void ExecuteDragFillClear(Excel excel)
        {
            _clearValueUndoAction.Execute(excel);
            CellRange startRange = _dragFillExtent.StartRange;
            CellRange fillRange = _dragFillExtent.FillRange;
            if (!startRange.Equals(fillRange))
            {
                if (_fillSeries == FillSeries.Column)
                {
                    CellRange range = new CellRange(startRange.Row, startRange.Column, startRange.RowCount - fillRange.RowCount, startRange.ColumnCount);
                    excel.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), true);
                    _workSheet.SetSelection(range);
                }
                else
                {
                    CellRange range4 = new CellRange(startRange.Row, startRange.Column, startRange.RowCount, startRange.ColumnCount - fillRange.ColumnCount);
                    excel.SetActiveCell(Math.Max(0, range4.Row), Math.Max(0, range4.Column), true);
                    _workSheet.SetSelection(range4);
                }
            }
        }

        object GetCopyCellsValue(CellRange fromRange, CellRange toRange)
        {
            Dictionary<long, object> dictionary = new Dictionary<long, object>();
            CellRange range = AdjustRange(fromRange);
            CellRange range2 = AdjustRange(toRange);
            if (_fillSeries == FillSeries.Column)
            {
                int num = range2.RowCount / range.RowCount;
                for (int j = 0; j < num; j++)
                {
                    int num5;
                    int row = range.Row;
                    int column = range.Column;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num5 = range2.Row + (j * range.RowCount);
                    }
                    else
                    {
                        int num6 = range2.Row + range2.RowCount;
                        num5 = num6 - ((j + 1) * range.RowCount);
                    }
                    int num7 = range2.Column;
                    int rowCount = range.RowCount;
                    int columnCount = range.ColumnCount;
                    dictionary[(long) ((num5 << 4) | num7)] = _workSheet.GetText((row + rowCount) - 1, (column + columnCount) - 1);
                }
                int num10 = range2.RowCount % range.RowCount;
                if (num10 != 0)
                {
                    int num11;
                    int num14;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num11 = range.Row;
                    }
                    else
                    {
                        int num12 = range.RowCount - (range2.RowCount - (range.RowCount * num));
                        num11 = range.Row + num12;
                    }
                    int num13 = range.Column;
                    if (_dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num14 = range2.Row + (range.RowCount * num);
                    }
                    else
                    {
                        int num15 = range2.Row + range2.RowCount;
                        num14 = (num15 - (num * range.RowCount)) - num10;
                    }
                    int num16 = range2.Column;
                    int num17 = num10;
                    int num18 = range.ColumnCount;
                    dictionary[(long) ((num14 << 4) | num16)] = _workSheet.GetText((num11 + num17) - 1, (num13 + num18) - 1);
                }
                return dictionary;
            }
            int num19 = range2.ColumnCount / range.ColumnCount;
            for (int i = 0; i < num19; i++)
            {
                int num24;
                int num21 = range.Row;
                int num22 = range.Column;
                int num23 = range2.Row;
                if (_dragFillExtent.FillDirection == FillDirection.Right)
                {
                    num24 = range2.Column + (i * range.ColumnCount);
                }
                else
                {
                    int num25 = range2.Column + range2.ColumnCount;
                    num24 = num25 - ((i + 1) * range.ColumnCount);
                }
                int num26 = range.RowCount;
                int num27 = range.ColumnCount;
                dictionary[(long) ((num23 << 4) | num24)] = _workSheet.GetText((num21 + num26) - 1, (num22 + num27) - 1);
            }
            int num28 = range2.ColumnCount % range.ColumnCount;
            if (num28 != 0)
            {
                int num30;
                int num33;
                int num29 = range.Row;
                if (_dragFillExtent.FillDirection == FillDirection.Right)
                {
                    num30 = range.Column;
                }
                else
                {
                    int num31 = range.ColumnCount - (range2.ColumnCount - (range.ColumnCount * num19));
                    num30 = range.Column + num31;
                }
                int num32 = range2.Row;
                if (_dragFillExtent.FillDirection == FillDirection.Right)
                {
                    num33 = range2.Column + (range.ColumnCount * num19);
                }
                else
                {
                    int num34 = range2.Column + range2.ColumnCount;
                    num33 = (num34 - (num19 * range.ColumnCount)) - num28;
                }
                int num35 = range.RowCount;
                int num36 = num28;
                dictionary[(long) ((num32 << 4) | num33)] = _workSheet.GetText((num29 + num35) - 1, (num30 + num36) - 1);
            }
            return dictionary;
        }

        void InitWholeFilledRange()
        {
            int row = 0;
            int rowCount = 0;
            int column = 0;
            int columnCount = 0;
            FillDirection fillDirection = _dragFillExtent.FillDirection;
            CellRange startRange = _dragFillExtent.StartRange;
            CellRange fillRange = _dragFillExtent.FillRange;
            switch (fillDirection)
            {
                case FillDirection.Left:
                case FillDirection.Right:
                    row = startRange.Row;
                    rowCount = startRange.RowCount;
                    column = (fillDirection == FillDirection.Left) ? fillRange.Column : startRange.Column;
                    columnCount = startRange.ColumnCount + fillRange.ColumnCount;
                    break;

                default:
                    row = (fillDirection == FillDirection.Up) ? fillRange.Row : startRange.Row;
                    rowCount = startRange.RowCount + fillRange.RowCount;
                    column = startRange.Column;
                    columnCount = startRange.ColumnCount;
                    break;
            }
            _wholeFillRange = new CellRange(row, column, rowCount, columnCount);
        }

        void SaveDragClearState()
        {
            _clearValueUndoAction.SaveState();
            SaveFillRangeTables();
        }

        void SaveDragFillState()
        {
            CellRange fillRange = _dragFillExtent.FillRange;
            _savedFilledViewportCells = SaveRangeStates(_dragFillExtent.FillRange);
            _savedStartViewportCells = SaveRangeStates(_dragFillExtent.StartRange);
            SaveFillRangeTables();
        }

        void SaveFillRangeTables()
        {
            _cachedTables = new List<SheetTable>();
            SheetTable[] tables = _workSheet.GetTables();
            if ((tables != null) && (tables.Length > 0))
            {
                foreach (SheetTable table in tables)
                {
                    if (_dragFillExtent.FillRange.Contains(table.Range))
                    {
                        _cachedTables.Add(table);
                    }
                }
            }
        }

        CopyMoveCellsInfo SaveRangeStates(CellRange range)
        {
            CellRange range2 = AdjustRange(range);
            CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(range2.RowCount, range2.ColumnCount);
            CopyMoveHelper.SaveViewportInfo(_workSheet, cellsInfo, range2.Row, range2.Column, CopyToOption.All);
            return cellsInfo;
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            if (_dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                SaveDragClearState();
            }
            else
            {
                SaveDragFillState();
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
            return ResourceStrings.undoActionAutoFill;
        }

        bool Undo(Excel excel)
        {
            bool flag;
            CellRange[] oldSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            List<CellData> oldValues = null;
            CellRange fillRange = _dragFillExtent.FillRange;
            if ((_savedFilledViewportCells != null) && _savedFilledViewportCells.IsValueSaved())
            {
                oldValues = CopyMoveHelper.GetValues(_workSheet, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount);
            }
            if (_dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                flag = UndoDragClear(excel);
            }
            else
            {
                flag = UndoDragFill(excel);
            }
            excel.CloseDragFillPopup();
            if ((oldValues != null) && object.ReferenceEquals(excel.ActiveSheet, _workSheet))
            {
                CopyMoveHelper.RaiseValueChanged(excel, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount, oldValues);
            }
            CellRange[] newSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            if (excel.RaiseSelectionChanging(oldSelection, newSelection))
            {
                excel.RaiseSelectionChanged();
            }
            excel.RefreshCellAreaViewport(0, 0, _workSheet.RowCount, _workSheet.ColumnCount);
            return flag;
        }

        /// <summary>
        /// Undoes the action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if undoing the action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            bool flag;
            Excel excel = parameter as Excel;
            try
            {
                flag = Undo(excel);
            }
            finally
            {
                _workSheet.ResumeCalcService();
                base.ResumeInvalidate(excel);
            }
            excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(_workSheet, _dragFillExtent.FillRange.Row, _dragFillExtent.FillRange.Column, _dragFillExtent.FillRange.RowCount, _dragFillExtent.FillRange.ColumnCount);
            if (list.Count > 0)
            {
                excel.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                return flag;
            }
            excel.RefreshFloatingObjects();
            return flag;
        }

        bool UndoDragClear(Excel excel)
        {
            bool flag = _clearValueUndoAction.Undo(excel);
            UndoFillRangeTables();
            excel.SetActiveCell(Math.Max(0, _dragFillExtent.StartRange.Row), Math.Max(0, _dragFillExtent.StartRange.Column), true);
            _workSheet.SetSelection(_dragFillExtent.StartRange);
            return flag;
        }

        bool UndoDragFill(Excel excel)
        {
            base.SuspendInvalidate(excel);
            _workSheet.SuspendCalcService();
            try
            {
                UndoRangeStates(_savedFilledViewportCells, _dragFillExtent.FillRange);
                UndoRangeStates(_savedStartViewportCells, _dragFillExtent.StartRange);
                UndoFillRangeTables();
                excel.SetActiveCell(Math.Max(0, _dragFillExtent.StartRange.Row), Math.Max(0, _dragFillExtent.StartRange.Column), true);
                _workSheet.SetSelection(_dragFillExtent.StartRange);
            }
            finally
            {
                _workSheet.ResumeCalcService();
                base.ResumeInvalidate(excel);
            }
            excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            return true;
        }

        void UndoFillRangeTables()
        {
            if ((_cachedTables != null) && (_cachedTables.Count > 0))
            {
                foreach (SheetTable table in _cachedTables)
                {
                    CellRange range = table.Range;
                    int row = range.Row;
                    int column = range.Column;
                    int rowCount = range.RowCount;
                    int columnCount = range.ColumnCount;
                    if (!table.ShowHeader)
                    {
                        row--;
                        rowCount++;
                    }
                    if (table.ShowFooter)
                    {
                        rowCount--;
                    }
                    SheetTable table2 = _workSheet.AddTable(table.Name, row, column, rowCount, columnCount, table.Style);
                    table2.BandedColumns = table.BandedColumns;
                    table2.BandedRows = table.BandedRows;
                    table2.HighlightFirstColumn = table.HighlightFirstColumn;
                    table2.HighlightLastColumn = table.HighlightLastColumn;
                    table2.ShowFooter = table.ShowFooter;
                    table2.ShowHeader = table.ShowHeader;
                }
            }
        }

        void UndoRangeStates(CopyMoveCellsInfo savedInfo, CellRange range)
        {
            CellRange range2 = AdjustRange(range);
            CopyMoveHelper.UndoCellsInfo(_workSheet, savedInfo, range2.Row, range2.Column, SheetArea.Cells);
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }
    }
}

