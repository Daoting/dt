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
    /// Represents a drag fill action to drag and fill a range on the worksheet.
    /// </summary>
    public class DragFillUndoAction : ActionBase, IUndo
    {
        private List<SheetTable> _cachedTables;
        private ClearValueUndoAction _clearValueUndoAction;
        private DragFillExtent _dragFillExtent;
        private FillSeries _fillSeries;
        private CopyMoveCellsInfo _savedFilledViewportCells;
        private CopyMoveCellsInfo _savedStartViewportCells;
        private CellRange _wholeFillRange;
        private Worksheet _workSheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragFillUndoAction" /> class.
        /// </summary>
        /// <param name="workSheet">The drag fill worksheet.</param>
        /// <param name="dragFillExtent">The drag fill extent information.</param>
        public DragFillUndoAction(Worksheet workSheet, DragFillExtent dragFillExtent)
        {
            this._workSheet = workSheet;
            this._dragFillExtent = dragFillExtent;
            if (this._dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                this._clearValueUndoAction = new ClearValueUndoAction(workSheet, new CellRange[] { this._dragFillExtent.FillRange });
            }
            else
            {
                this.InitWholeFilledRange();
            }
            if ((this._dragFillExtent.FillDirection == FillDirection.Left) || (this._dragFillExtent.FillDirection == FillDirection.Right))
            {
                this._fillSeries = FillSeries.Row;
            }
            else
            {
                this._fillSeries = FillSeries.Column;
            }
        }

        private CellRange AdjustRange(CellRange range)
        {
            int row = (range.Row != -1) ? range.Row : 0;
            int column = (range.Column != -1) ? range.Column : 0;
            int rowCount = (range.RowCount != -1) ? range.RowCount : this._workSheet.RowCount;
            return new CellRange(row, column, rowCount, (range.ColumnCount != -1) ? range.ColumnCount : this._workSheet.ColumnCount);
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
            if (this._dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                return this.CanExecuteDragClear();
            }
            return this.CanExecuteDragFill();
        }

        private bool CanExecuteDragClear()
        {
            return true;
        }

        private bool CanExecuteDragFill()
        {
            CellRange startRange = this._dragFillExtent.StartRange;
            if (this._dragFillExtent.FillRange.Intersects(startRange.Row, startRange.Column, startRange.RowCount, startRange.ColumnCount))
            {
                return false;
            }
            return true;
        }

        private void ClearData(CellRange cellRange)
        {
            StorageType data = StorageType.Data;
            this._workSheet.Clear(cellRange.Row, cellRange.Column, cellRange.RowCount, cellRange.ColumnCount, SheetArea.Cells, data);
        }

        private void CopyCells(CellRange fromRange, CellRange toRange, CopyToOption copyOption)
        {
            CellRange range = this.AdjustRange(fromRange);
            CellRange range2 = this.AdjustRange(toRange);
            if (this._fillSeries == FillSeries.Column)
            {
                int num = range2.RowCount / range.RowCount;
                for (int i = 0; i < num; i++)
                {
                    int num5;
                    int row = range.Row;
                    int column = range.Column;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
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
                    this._workSheet.CopyTo(row, column, num5, toColumn, rowCount, columnCount, copyOption);
                }
                int num10 = range2.RowCount % range.RowCount;
                if (num10 != 0)
                {
                    int num11;
                    int num14;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num11 = range.Row;
                    }
                    else
                    {
                        int num12 = range.RowCount - (range2.RowCount - (range.RowCount * num));
                        num11 = range.Row + num12;
                    }
                    int fromColumn = range.Column;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
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
                    this._workSheet.CopyTo(num11, fromColumn, num14, num16, num17, num18, copyOption);
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
                    if (this._dragFillExtent.FillDirection == FillDirection.Right)
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
                    this._workSheet.CopyTo(fromRow, num22, toRow, num24, num26, num27, copyOption);
                }
                int num28 = range2.ColumnCount % range.ColumnCount;
                if (num28 != 0)
                {
                    int num30;
                    int num33;
                    int num29 = range.Row;
                    if (this._dragFillExtent.FillDirection == FillDirection.Right)
                    {
                        num30 = range.Column;
                    }
                    else
                    {
                        int num31 = range.ColumnCount - (range2.ColumnCount - (range.ColumnCount * num19));
                        num30 = range.Column + num31;
                    }
                    int num32 = range2.Row;
                    if (this._dragFillExtent.FillDirection == FillDirection.Right)
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
                    this._workSheet.CopyTo(num29, num30, num32, num33, num35, num36, copyOption);
                }
            }
        }

        private void Execute(SheetView sheetView)
        {
            CellRange[] oldSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            if (this._dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                this.ExecuteDragFillClear(sheetView);
            }
            else
            {
                this.ExecuteDragFill(sheetView);
            }
            if (((this._savedFilledViewportCells != null) && this._savedFilledViewportCells.IsValueSaved()) && object.ReferenceEquals(sheetView.Worksheet, this._workSheet))
            {
                CellRange fillRange = this._dragFillExtent.FillRange;
                CopyMoveHelper.RaiseValueChanged(sheetView, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount, this._savedFilledViewportCells.GetValues());
            }
            CellRange[] newSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            if (sheetView.RaiseSelectionChanging(oldSelection, newSelection))
            {
                sheetView.RaiseSelectionChanged();
            }
        }

        /// <summary>
        /// Executes the drag fill action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        public override void Execute(object parameter)
        {
            if (this.CanExecute(parameter))
            {
                SheetView sender = parameter as SheetView;
                try
                {
                    base.SuspendInvalidate(sender);
                    this._workSheet.SuspendCalcService();
                    this.SaveState();
                    this.Execute(sender);
                }
                finally
                {
                    this._workSheet.ResumeCalcService();
                    base.ResumeInvalidate(sender);
                }
                sender.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(this._workSheet, this._dragFillExtent.FillRange.Row, this._dragFillExtent.FillRange.Column, this._dragFillExtent.FillRange.RowCount, this._dragFillExtent.FillRange.ColumnCount);
                if (list.Count > 0)
                {
                    sender.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                }
                else
                {
                    sender.InvalidateFloatingObjects();
                }
            }
        }

        private void ExecuteDragFill(SheetView sheetView)
        {
            CellRange startRange = this._dragFillExtent.StartRange;
            CellRange fillRange = this._dragFillExtent.FillRange;
            if (this._dragFillExtent.AutoFillType == AutoFillType.FillSeries)
            {
                this.ClearData(fillRange);
                if ((this._dragFillExtent.FillDirection == FillDirection.Down) || (this._dragFillExtent.FillDirection == FillDirection.Right))
                {
                    this._workSheet.FillAuto(this._wholeFillRange, this._fillSeries);
                }
                else
                {
                    this._workSheet.FillLinear(this._wholeFillRange, this._fillSeries);
                }
            }
            else if (this._dragFillExtent.AutoFillType == AutoFillType.CopyCells)
            {
                this.CopyCells(startRange, fillRange, CopyToOption.All);
            }
            else if (this._dragFillExtent.AutoFillType == AutoFillType.FillFormattingOnly)
            {
                this.CopyCells(startRange, fillRange, CopyToOption.Style);
            }
            else if (this._dragFillExtent.AutoFillType == AutoFillType.FillWithoutFormatting)
            {
                this.ClearData(fillRange);
                if ((((startRange.RowCount == 1) && (startRange.ColumnCount == 1)) && ((startRange.Row != -1) || (startRange.Column == -1))) && ((startRange.Column != -1) || (startRange.Row == -1)))
                {
                    CopyToOption copyOption = CopyToOption.Tag | CopyToOption.Span | CopyToOption.Sparkline | CopyToOption.RangeGroup | CopyToOption.Formula | CopyToOption.Value;
                    this.CopyCells(startRange, fillRange, copyOption);
                }
                else
                {
                    CellRange range3 = this.AdjustRange(this._wholeFillRange);
                    object[,] objArray = new object[range3.RowCount, range3.ColumnCount];
                    for (int i = 0; i < range3.RowCount; i++)
                    {
                        for (int k = 0; k < range3.ColumnCount; k++)
                        {
                            objArray[i, k] = CopyMoveHelper.GetStyleObject(this._workSheet, range3.Row + i, range3.Column + k, SheetArea.Cells);
                        }
                    }
                    if ((this._dragFillExtent.FillDirection == FillDirection.Down) || (this._dragFillExtent.FillDirection == FillDirection.Right))
                    {
                        this._workSheet.FillAuto(this._wholeFillRange, this._fillSeries);
                    }
                    else
                    {
                        this._workSheet.FillLinear(this._wholeFillRange, this._fillSeries);
                    }
                    for (int j = 0; j < range3.RowCount; j++)
                    {
                        for (int m = 0; m < range3.ColumnCount; m++)
                        {
                            CopyMoveHelper.SetStyleObject(this._workSheet, range3.Row + j, range3.Column + m, SheetArea.Cells, objArray[j, m]);
                        }
                    }
                }
            }
            sheetView.SetActiveCell(Math.Max(0, this._wholeFillRange.Row), Math.Max(0, this._wholeFillRange.Column), true);
            this._workSheet.SetSelection(this._wholeFillRange);
        }

        private void ExecuteDragFillClear(SheetView sheetView)
        {
            this._clearValueUndoAction.Execute(sheetView);
            CellRange startRange = this._dragFillExtent.StartRange;
            CellRange fillRange = this._dragFillExtent.FillRange;
            if (!startRange.Equals(fillRange))
            {
                if (this._fillSeries == FillSeries.Column)
                {
                    CellRange range = new CellRange(startRange.Row, startRange.Column, startRange.RowCount - fillRange.RowCount, startRange.ColumnCount);
                    sheetView.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), true);
                    this._workSheet.SetSelection(range);
                }
                else
                {
                    CellRange range4 = new CellRange(startRange.Row, startRange.Column, startRange.RowCount, startRange.ColumnCount - fillRange.ColumnCount);
                    sheetView.SetActiveCell(Math.Max(0, range4.Row), Math.Max(0, range4.Column), true);
                    this._workSheet.SetSelection(range4);
                }
            }
        }

        private object GetCopyCellsValue(CellRange fromRange, CellRange toRange)
        {
            Dictionary<long, object> dictionary = new Dictionary<long, object>();
            CellRange range = this.AdjustRange(fromRange);
            CellRange range2 = this.AdjustRange(toRange);
            if (this._fillSeries == FillSeries.Column)
            {
                int num = range2.RowCount / range.RowCount;
                for (int j = 0; j < num; j++)
                {
                    int num5;
                    int row = range.Row;
                    int column = range.Column;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
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
                    dictionary[(long) ((num5 << 4) | num7)] = this._workSheet.GetText((row + rowCount) - 1, (column + columnCount) - 1);
                }
                int num10 = range2.RowCount % range.RowCount;
                if (num10 != 0)
                {
                    int num11;
                    int num14;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
                    {
                        num11 = range.Row;
                    }
                    else
                    {
                        int num12 = range.RowCount - (range2.RowCount - (range.RowCount * num));
                        num11 = range.Row + num12;
                    }
                    int num13 = range.Column;
                    if (this._dragFillExtent.FillDirection == FillDirection.Down)
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
                    dictionary[(long) ((num14 << 4) | num16)] = this._workSheet.GetText((num11 + num17) - 1, (num13 + num18) - 1);
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
                if (this._dragFillExtent.FillDirection == FillDirection.Right)
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
                dictionary[(long) ((num23 << 4) | num24)] = this._workSheet.GetText((num21 + num26) - 1, (num22 + num27) - 1);
            }
            int num28 = range2.ColumnCount % range.ColumnCount;
            if (num28 != 0)
            {
                int num30;
                int num33;
                int num29 = range.Row;
                if (this._dragFillExtent.FillDirection == FillDirection.Right)
                {
                    num30 = range.Column;
                }
                else
                {
                    int num31 = range.ColumnCount - (range2.ColumnCount - (range.ColumnCount * num19));
                    num30 = range.Column + num31;
                }
                int num32 = range2.Row;
                if (this._dragFillExtent.FillDirection == FillDirection.Right)
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
                dictionary[(long) ((num32 << 4) | num33)] = this._workSheet.GetText((num29 + num35) - 1, (num30 + num36) - 1);
            }
            return dictionary;
        }

        private void InitWholeFilledRange()
        {
            int row = 0;
            int rowCount = 0;
            int column = 0;
            int columnCount = 0;
            FillDirection fillDirection = this._dragFillExtent.FillDirection;
            CellRange startRange = this._dragFillExtent.StartRange;
            CellRange fillRange = this._dragFillExtent.FillRange;
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
            this._wholeFillRange = new CellRange(row, column, rowCount, columnCount);
        }

        private void SaveDragClearState()
        {
            this._clearValueUndoAction.SaveState();
            this.SaveFillRangeTables();
        }

        private void SaveDragFillState()
        {
            CellRange fillRange = this._dragFillExtent.FillRange;
            this._savedFilledViewportCells = this.SaveRangeStates(this._dragFillExtent.FillRange);
            this._savedStartViewportCells = this.SaveRangeStates(this._dragFillExtent.StartRange);
            this.SaveFillRangeTables();
        }

        private void SaveFillRangeTables()
        {
            this._cachedTables = new List<SheetTable>();
            SheetTable[] tables = this._workSheet.GetTables();
            if ((tables != null) && (tables.Length > 0))
            {
                foreach (SheetTable table in tables)
                {
                    if (this._dragFillExtent.FillRange.Contains(table.Range))
                    {
                        this._cachedTables.Add(table);
                    }
                }
            }
        }

        private CopyMoveCellsInfo SaveRangeStates(CellRange range)
        {
            CellRange range2 = this.AdjustRange(range);
            CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(range2.RowCount, range2.ColumnCount);
            CopyMoveHelper.SaveViewportInfo(this._workSheet, cellsInfo, range2.Row, range2.Column, CopyToOption.All);
            return cellsInfo;
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            if (this._dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                this.SaveDragClearState();
            }
            else
            {
                this.SaveDragFillState();
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

        private bool Undo(SheetView sheetView)
        {
            bool flag;
            CellRange[] oldSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            List<CellData> oldValues = null;
            CellRange fillRange = this._dragFillExtent.FillRange;
            if ((this._savedFilledViewportCells != null) && this._savedFilledViewportCells.IsValueSaved())
            {
                oldValues = CopyMoveHelper.GetValues(this._workSheet, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount);
            }
            if (this._dragFillExtent.AutoFillType == AutoFillType.ClearValues)
            {
                flag = this.UndoDragClear(sheetView);
            }
            else
            {
                flag = this.UndoDragFill(sheetView);
            }
            sheetView.CloseDragFillPopup();
            if ((oldValues != null) && object.ReferenceEquals(sheetView.Worksheet, this._workSheet))
            {
                CopyMoveHelper.RaiseValueChanged(sheetView, fillRange.Row, fillRange.Column, fillRange.RowCount, fillRange.ColumnCount, oldValues);
            }
            CellRange[] newSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            if (sheetView.RaiseSelectionChanging(oldSelection, newSelection))
            {
                sheetView.RaiseSelectionChanged();
            }
            sheetView.RefreshCellAreaViewport(0, 0, this._workSheet.RowCount, this._workSheet.ColumnCount);
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
            SheetView sheetView = parameter as SheetView;
            try
            {
                flag = this.Undo(sheetView);
            }
            finally
            {
                this._workSheet.ResumeCalcService();
                base.ResumeInvalidate(sheetView);
            }
            sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(this._workSheet, this._dragFillExtent.FillRange.Row, this._dragFillExtent.FillRange.Column, this._dragFillExtent.FillRange.RowCount, this._dragFillExtent.FillRange.ColumnCount);
            if (list.Count > 0)
            {
                sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                return flag;
            }
            sheetView.InvalidateFloatingObjects();
            return flag;
        }

        private bool UndoDragClear(SheetView sheetView)
        {
            bool flag = this._clearValueUndoAction.Undo(sheetView);
            this.UndoFillRangeTables();
            sheetView.SetActiveCell(Math.Max(0, this._dragFillExtent.StartRange.Row), Math.Max(0, this._dragFillExtent.StartRange.Column), true);
            this._workSheet.SetSelection(this._dragFillExtent.StartRange);
            return flag;
        }

        private bool UndoDragFill(SheetView sheetView)
        {
            base.SuspendInvalidate(sheetView);
            this._workSheet.SuspendCalcService();
            try
            {
                this.UndoRangeStates(this._savedFilledViewportCells, this._dragFillExtent.FillRange);
                this.UndoRangeStates(this._savedStartViewportCells, this._dragFillExtent.StartRange);
                this.UndoFillRangeTables();
                sheetView.SetActiveCell(Math.Max(0, this._dragFillExtent.StartRange.Row), Math.Max(0, this._dragFillExtent.StartRange.Column), true);
                this._workSheet.SetSelection(this._dragFillExtent.StartRange);
            }
            finally
            {
                this._workSheet.ResumeCalcService();
                base.ResumeInvalidate(sheetView);
            }
            sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            return true;
        }

        private void UndoFillRangeTables()
        {
            if ((this._cachedTables != null) && (this._cachedTables.Count > 0))
            {
                foreach (SheetTable table in this._cachedTables)
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
                    SheetTable table2 = this._workSheet.AddTable(table.Name, row, column, rowCount, columnCount, table.Style);
                    table2.BandedColumns = table.BandedColumns;
                    table2.BandedRows = table.BandedRows;
                    table2.HighlightFirstColumn = table.HighlightFirstColumn;
                    table2.HighlightLastColumn = table.HighlightLastColumn;
                    table2.ShowFooter = table.ShowFooter;
                    table2.ShowHeader = table.ShowHeader;
                }
            }
        }

        private void UndoRangeStates(CopyMoveCellsInfo savedInfo, CellRange range)
        {
            CellRange range2 = this.AdjustRange(range);
            CopyMoveHelper.UndoCellsInfo(this._workSheet, savedInfo, range2.Row, range2.Column, SheetArea.Cells);
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

