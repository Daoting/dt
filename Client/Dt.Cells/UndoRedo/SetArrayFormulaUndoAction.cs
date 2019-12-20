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
    /// Represents setting an array formula action on the worksheet.
    /// </summary>
    public class SetArrayFormulaUndoAction : ActionBase, IUndo
    {
        private CellRange _arrayFormulaRange;
        private string _formula;
        private CopyMoveCellsInfo _savedArrayFormulaViewportCells;
        private Worksheet _workSheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragFillUndoAction" /> class.
        /// </summary>
        /// <param name="workSheet">The drag fill worksheet.</param>
        /// <param name="formula">The drag fill worksheet.</param>
        public SetArrayFormulaUndoAction(Worksheet workSheet, string formula)
        {
            this._workSheet = workSheet;
            this._formula = formula;
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
            return true;
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
                if (this._arrayFormulaRange == null)
                {
                    this._arrayFormulaRange = sender.ActiveSelection;
                }
                try
                {
                    base.SuspendInvalidate(sender);
                    this._workSheet.SuspendCalcService();
                    this.SaveState();
                    this.Execute(sender, this._formula);
                }
                catch (Exception exception)
                {
                    sender.RaiseInvalidOperation(exception.Message, null, null);
                }
                finally
                {
                    this._workSheet.ResumeCalcService();
                    base.ResumeInvalidate(sender);
                }
                sender.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                sender.InvalidateCharts();
            }
        }

        private void Execute(SheetView sheetView, string formula)
        {
            if (sheetView != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(formula))
                    {
                        if ((this._arrayFormulaRange.ColumnCount == 1) && (this._arrayFormulaRange.RowCount == 1))
                        {
                            object[,] objArray = sheetView.Worksheet.FindFormulas(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount);
                            for (int i = 0; i < objArray.GetLength(0); i++)
                            {
                                CellRange range = objArray[i, 0] as CellRange;
                                string str = (string) (objArray[i, 1] as string);
                                if (((str.StartsWith("{") && (range.Row <= this._arrayFormulaRange.Row)) && ((range.Column <= this._arrayFormulaRange.Column) && ((range.Row + range.RowCount) >= (this._arrayFormulaRange.Row + this._arrayFormulaRange.RowCount)))) && ((range.Column + range.ColumnCount) >= (this._arrayFormulaRange.Column + this._arrayFormulaRange.ColumnCount)))
                                {
                                    this._arrayFormulaRange = range;
                                    using (((IUIActionExecuter) this._workSheet).BeginUIAction())
                                    {
                                        sheetView.Worksheet.SetArrayFormula(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount, formula);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((formula.Length > 1) && formula.StartsWith("="))
                        {
                            if ((this._arrayFormulaRange.ColumnCount == 1) && (this._arrayFormulaRange.RowCount == 1))
                            {
                                object[,] objArray2 = sheetView.Worksheet.FindFormulas(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount);
                                for (int k = 0; k < objArray2.GetLength(0); k++)
                                {
                                    CellRange range2 = objArray2[k, 0] as CellRange;
                                    string str2 = (string) (objArray2[k, 1] as string);
                                    if (str2.StartsWith("{"))
                                    {
                                        if (((range2.Row <= this._arrayFormulaRange.Row) && (range2.Column <= this._arrayFormulaRange.Column)) && (((range2.Row + range2.RowCount) >= (this._arrayFormulaRange.Row + this._arrayFormulaRange.RowCount)) && ((range2.Column + range2.ColumnCount) >= (this._arrayFormulaRange.Column + this._arrayFormulaRange.ColumnCount))))
                                        {
                                            this._arrayFormulaRange = range2;
                                            break;
                                        }
                                        if (((range2.Row == -1) && (range2.RowCount == -1)) && range2.Contains(this._arrayFormulaRange))
                                        {
                                            this._arrayFormulaRange = range2;
                                            break;
                                        }
                                        if (((range2.Column == -1) && (range2.ColumnCount == -1)) && range2.Contains(this._arrayFormulaRange))
                                        {
                                            this._arrayFormulaRange = range2;
                                            break;
                                        }
                                    }
                                }
                            }
                            using (((IUIActionExecuter) this._workSheet).BeginUIAction())
                            {
                                sheetView.Worksheet.SetArrayFormula(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount, formula);
                                return;
                            }
                        }
                        for (int j = 0; j < this._arrayFormulaRange.RowCount; j++)
                        {
                            for (int m = 0; m < this._arrayFormulaRange.ColumnCount; m++)
                            {
                                sheetView.Worksheet.SetValue(this._arrayFormulaRange.Row + j, this._arrayFormulaRange.Column + m, formula);
                            }
                        }
                    }
                }
                finally
                {
                    sheetView.StopCellEditing(true);
                    sheetView.Invalidate();
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            CellRange range = this.AdjustRange(this._arrayFormulaRange);
            CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(range.RowCount, range.ColumnCount);
            CopyMoveHelper.SaveViewportInfo(this._workSheet, cellsInfo, range.Row, range.Column, CopyToOption.All);
            this._savedArrayFormulaViewportCells = cellsInfo;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionArrayFormula;
        }

        private bool Undo(SheetView sheetView)
        {
            bool flag = this.UndoSetArrayFormula(sheetView);
            CellRange[] oldSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            List<CellData> oldValues = null;
            if ((this._savedArrayFormulaViewportCells != null) && this._savedArrayFormulaViewportCells.IsValueSaved())
            {
                oldValues = CopyMoveHelper.GetValues(this._workSheet, this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount);
            }
            if ((oldValues != null) && object.ReferenceEquals(sheetView.Worksheet, this._workSheet))
            {
                CopyMoveHelper.RaiseValueChanged(sheetView, this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount, oldValues);
            }
            CellRange[] newSelection = (this._workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) this._workSheet.Selections) : null;
            if (sheetView.RaiseSelectionChanging(oldSelection, newSelection))
            {
                sheetView.RaiseSelectionChanged();
            }
            sheetView.RefreshCellAreaViewport(0, 0, this._workSheet.RowCount, this._workSheet.ColumnCount);
            sheetView.InvalidateCharts();
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
            return flag;
        }

        private bool UndoSetArrayFormula(SheetView sheetView)
        {
            base.SuspendInvalidate(sheetView);
            this._workSheet.SuspendCalcService();
            try
            {
                using (((IUIActionExecuter) this._workSheet).BeginUIAction())
                {
                    sheetView.Worksheet.SetArrayFormula(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount, null);
                }
                CellRange range = new CellRange(this._arrayFormulaRange.Row, this._arrayFormulaRange.Column, this._arrayFormulaRange.RowCount, this._arrayFormulaRange.ColumnCount);
                CellRange range2 = this.AdjustRange(range);
                CopyMoveHelper.UndoCellsInfo(this._workSheet, this._savedArrayFormulaViewportCells, range2.Row, range2.Column, SheetArea.Cells);
                sheetView.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), true);
                this._workSheet.SetSelection(range);
            }
            finally
            {
                this._workSheet.ResumeCalcService();
                base.ResumeInvalidate(sheetView);
            }
            sheetView.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
            return true;
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

