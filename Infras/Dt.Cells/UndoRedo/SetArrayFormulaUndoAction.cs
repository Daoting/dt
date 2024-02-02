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
    /// Represents setting an array formula action on the worksheet.
    /// </summary>
    public class SetArrayFormulaUndoAction : ActionBase, IUndo
    {
        CellRange _arrayFormulaRange;
        string _formula;
        CopyMoveCellsInfo _savedArrayFormulaViewportCells;
        Worksheet _workSheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.DragFillUndoAction" /> class.
        /// </summary>
        /// <param name="workSheet">The drag fill worksheet.</param>
        /// <param name="formula">The drag fill worksheet.</param>
        public SetArrayFormulaUndoAction(Worksheet workSheet, string formula)
        {
            _workSheet = workSheet;
            _formula = formula;
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
            return true;
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
                if (_arrayFormulaRange == null)
                {
                    _arrayFormulaRange = sender.GetActiveSelection();
                }
                try
                {
                    base.SuspendInvalidate(sender);
                    _workSheet.SuspendCalcService();
                    SaveState();
                    Execute(sender, _formula);
                }
                catch (Exception exception)
                {
                    sender.RaiseInvalidOperation(exception.Message, null, null);
                }
                finally
                {
                    _workSheet.ResumeCalcService();
                    base.ResumeInvalidate(sender);
                }
                sender.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                sender.RefreshCharts();
            }
        }

        void Execute(Excel excel, string formula)
        {
            if (excel != null)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(formula))
                    {
                        if ((_arrayFormulaRange.ColumnCount == 1) && (_arrayFormulaRange.RowCount == 1))
                        {
                            object[,] objArray = excel.ActiveSheet.FindFormulas(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount);
                            for (int i = 0; i < objArray.GetLength(0); i++)
                            {
                                CellRange range = objArray[i, 0] as CellRange;
                                string str = (string) (objArray[i, 1] as string);
                                if (((str.StartsWith("{") && (range.Row <= _arrayFormulaRange.Row)) && ((range.Column <= _arrayFormulaRange.Column) && ((range.Row + range.RowCount) >= (_arrayFormulaRange.Row + _arrayFormulaRange.RowCount)))) && ((range.Column + range.ColumnCount) >= (_arrayFormulaRange.Column + _arrayFormulaRange.ColumnCount)))
                                {
                                    _arrayFormulaRange = range;
                                    using (((IUIActionExecuter) _workSheet).BeginUIAction())
                                    {
                                        excel.ActiveSheet.SetArrayFormula(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount, formula);
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
                            if ((_arrayFormulaRange.ColumnCount == 1) && (_arrayFormulaRange.RowCount == 1))
                            {
                                object[,] objArray2 = excel.ActiveSheet.FindFormulas(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount);
                                for (int k = 0; k < objArray2.GetLength(0); k++)
                                {
                                    CellRange range2 = objArray2[k, 0] as CellRange;
                                    string str2 = (string) (objArray2[k, 1] as string);
                                    if (str2.StartsWith("{"))
                                    {
                                        if (((range2.Row <= _arrayFormulaRange.Row) && (range2.Column <= _arrayFormulaRange.Column)) && (((range2.Row + range2.RowCount) >= (_arrayFormulaRange.Row + _arrayFormulaRange.RowCount)) && ((range2.Column + range2.ColumnCount) >= (_arrayFormulaRange.Column + _arrayFormulaRange.ColumnCount))))
                                        {
                                            _arrayFormulaRange = range2;
                                            break;
                                        }
                                        if (((range2.Row == -1) && (range2.RowCount == -1)) && range2.Contains(_arrayFormulaRange))
                                        {
                                            _arrayFormulaRange = range2;
                                            break;
                                        }
                                        if (((range2.Column == -1) && (range2.ColumnCount == -1)) && range2.Contains(_arrayFormulaRange))
                                        {
                                            _arrayFormulaRange = range2;
                                            break;
                                        }
                                    }
                                }
                            }
                            using (((IUIActionExecuter) _workSheet).BeginUIAction())
                            {
                                excel.ActiveSheet.SetArrayFormula(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount, formula);
                                return;
                            }
                        }
                        for (int j = 0; j < _arrayFormulaRange.RowCount; j++)
                        {
                            for (int m = 0; m < _arrayFormulaRange.ColumnCount; m++)
                            {
                                excel.ActiveSheet.SetValue(_arrayFormulaRange.Row + j, _arrayFormulaRange.Column + m, formula);
                            }
                        }
                    }
                }
                finally
                {
                    excel.StopCellEditing(true);
                    excel.RefreshAll();
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            CellRange range = AdjustRange(_arrayFormulaRange);
            CopyMoveCellsInfo cellsInfo = new CopyMoveCellsInfo(range.RowCount, range.ColumnCount);
            CopyMoveHelper.SaveViewportInfo(_workSheet, cellsInfo, range.Row, range.Column, CopyToOption.All);
            _savedArrayFormulaViewportCells = cellsInfo;
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

        bool Undo(Excel excel)
        {
            bool flag = UndoSetArrayFormula(excel);
            CellRange[] oldSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            List<CellData> oldValues = null;
            if ((_savedArrayFormulaViewportCells != null) && _savedArrayFormulaViewportCells.IsValueSaved())
            {
                oldValues = CopyMoveHelper.GetValues(_workSheet, _arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount);
            }
            if ((oldValues != null) && object.ReferenceEquals(excel.ActiveSheet, _workSheet))
            {
                CopyMoveHelper.RaiseValueChanged(excel, _arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount, oldValues);
            }
            CellRange[] newSelection = (_workSheet.Selections != null) ? Enumerable.ToArray<CellRange>((IEnumerable<CellRange>) _workSheet.Selections) : null;
            if (excel.RaiseSelectionChanging(oldSelection, newSelection))
            {
                excel.RaiseSelectionChanged();
            }
            excel.RefreshCellAreaViewport(0, 0, _workSheet.RowCount, _workSheet.ColumnCount);
            excel.RefreshCharts();
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
            return flag;
        }

        bool UndoSetArrayFormula(Excel excel)
        {
            base.SuspendInvalidate(excel);
            _workSheet.SuspendCalcService();
            try
            {
                using (((IUIActionExecuter) _workSheet).BeginUIAction())
                {
                    excel.ActiveSheet.SetArrayFormula(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount, null);
                }
                CellRange range = new CellRange(_arrayFormulaRange.Row, _arrayFormulaRange.Column, _arrayFormulaRange.RowCount, _arrayFormulaRange.ColumnCount);
                CellRange range2 = AdjustRange(range);
                CopyMoveHelper.UndoCellsInfo(_workSheet, _savedArrayFormulaViewportCells, range2.Row, range2.Column, SheetArea.Cells);
                excel.SetActiveCell(Math.Max(0, range.Row), Math.Max(0, range.Column), true);
                _workSheet.SetSelection(range);
            }
            finally
            {
                _workSheet.ResumeCalcService();
                base.ResumeInvalidate(excel);
            }
            excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
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

