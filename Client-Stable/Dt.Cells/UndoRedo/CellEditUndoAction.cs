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
    /// Represents a cell edit undo action that applies a new value to a cell on the sheet.
    /// </summary>
    public class CellEditUndoAction : ActionBase, IUndo
    {
        Excel _excel;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.CellEditUndoAction" /> class.
        /// </summary>
        /// <param name="worksheet">The edit cell worksheet.</param>
        /// <param name="extent">The edit cell extent information.</param>
        public CellEditUndoAction(Dt.Cells.Data.Worksheet worksheet, Dt.Cells.UndoRedo.CellEditExtent extent)
        {
            Worksheet = worksheet;
            CellEditExtent = extent;
        }

        DataValidationResult ApplyEditing(Excel excel)
        {
            int rowIndex = CellEditExtent.RowIndex;
            int columnIndex = CellEditExtent.ColumnIndex;
            if (excel == null)
            {
                return DataValidationResult.Discard;
            }
            string newValue = CellEditExtent.NewValue;

            //DataValidationResult forceApply = DataValidationResult.ForceApply;
            //bool flag = true;
            //if (UI.FormulaUtility.IsFormula(newValue))
            //{
            //    string str2 = UI.FormulaUtility.StringVariantToInvariant(excel.ActiveSheet, newValue);
            //    flag = Worksheet.IsValid(rowIndex, columnIndex, str2);
            //}
            //else
            //{
            //    object obj2 = UI.ConditionValueConverter.TryDateTime(newValue, true);
            //    if (obj2 == null)
            //    {
            //        obj2 = UI.ConditionValueConverter.TryDouble(newValue, true);
            //    }
            //    if (obj2 != null)
            //    {
            //        flag = Worksheet.IsValid(rowIndex, columnIndex, obj2);
            //    }
            //    else
            //    {
            //        flag = Worksheet.IsValid(rowIndex, columnIndex, newValue);
            //    }
            //}
            //if (!flag)
            //{
            //    forceApply = ValidationError(excel, rowIndex, columnIndex, newValue);
            //}
            //switch (forceApply)
            //{
            //    case DataValidationResult.Discard:
            //    case DataValidationResult.Retry:
            //        return forceApply;
            //}
            //if (forceApply != DataValidationResult.ForceApply)
            //{
            //    throw new NotSupportedException(ResourceStrings.undoActionCellEditInvalidValidationFlag);
            //}

            Cell bindingCell = Worksheet.Cells[rowIndex, columnIndex];
            Type valueType = null;
            if ((bindingCell != null) && (bindingCell.Value != null))
            {
                valueType = bindingCell.Value.GetType();
            }
            bool isFormulaApplied = false;
            string appliedFormula = null;
            bool flag3 = false;
            using (((IUIActionExecuter) excel.ActiveSheet).BeginUIAction())
            {
                flag3 = ApplyValueToCell(excel, bindingCell, excel.CanUserEditFormula, newValue, valueType, out isFormulaApplied, out appliedFormula);
            }
            if (!flag3)
            {
                excel.RaiseInvalidOperation(isFormulaApplied ? string.Format(ResourceStrings.undoActionCannotApplyFormula, (object[]) new object[] { appliedFormula }) : ResourceStrings.undoActionCannotApplyValue, null, null);
                return DataValidationResult.Discard;
            }
            if (isFormulaApplied)
            {
                excel.RaiseUserFormulaEntered(rowIndex, columnIndex, appliedFormula);
            }
            return DataValidationResult.ForceApply;
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
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Object on which the action occurred.</param>
        public override void Execute(object parameter)
        {
            Excel excel = parameter as Excel;
            if (excel != null)
            {
                base.SuspendInvalidate(parameter);
                SaveState();
                try
                {
                    _excel = excel;
                    Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                    ApplyResult = ApplyEditing(excel);
                }
                finally
                {
                    Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                    _excel = null;
                }
                excel.ResumeInvalidate();
                switch (ApplyResult)
                {
                    case DataValidationResult.ForceApply:
                    {
                        excel.RefreshCellAreaViewport(0, 0, Worksheet.RowCount, Worksheet.ColumnCount);
                        IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(Worksheet, CellEditExtent.RowIndex, CellEditExtent.ColumnIndex);
                        if (list.Count <= 0)
                        {
                            excel.RefreshFloatingObjects();
                            return;
                        }
                        excel.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                        return;
                    }
                }
                throw new ActionFailedException(this);
            }
        }

        void OnEditedCellChanged(object sender, CellChangedEventArgs e)
        {
            if ((_excel != null) && string.Equals(e.PropertyName, "Value"))
            {
                _excel.RaiseValueChanged(e.Row, e.Column);
            }
        }

        /// <summary>
        /// Saves the cell state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            string formula = Worksheet.GetFormula(CellEditExtent.RowIndex, CellEditExtent.ColumnIndex);
            if (!string.IsNullOrEmpty(formula))
            {
                OldValue = formula;
                OldValueIsFormula = true;
            }
            else
            {
                OldValue = Worksheet.GetValue(CellEditExtent.RowIndex, CellEditExtent.ColumnIndex);
                OldValueIsFormula = false;
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
            if (CellEditExtent != null)
            {
                return CellEditExtent.ToString();
            }
            return ResourceStrings.undoActionEditingCell;
        }

        /// <summary>
        /// Undoes the command or operation.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if undoing the action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            bool flag;
            int rowIndex = CellEditExtent.RowIndex;
            int columnIndex = CellEditExtent.ColumnIndex;
            try
            {
                Excel view = parameter as Excel;
                base.SuspendInvalidate(parameter);
                if (view != null)
                {
                    _excel = view;
                    Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                }
                if (OldValueIsFormula)
                {
                    string oldValue = (string) (OldValue as string);
                    Worksheet.SetFormula(rowIndex, columnIndex, oldValue);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Worksheet.GetFormula(rowIndex, columnIndex)))
                    {
                        Worksheet.SetFormula(rowIndex, columnIndex, null);
                    }
                    Worksheet.SetValue(rowIndex, columnIndex, OldValue);
                }
                base.ResumeInvalidate(parameter);
                if (view != null)
                {
                    view.RefreshCellAreaViewport(0, 0, Worksheet.RowCount, Worksheet.ColumnCount);
                    IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(Worksheet, rowIndex, columnIndex);
                    if (list.Count > 0)
                    {
                        view.RefreshFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                    }
                    else
                    {
                        view.RefreshFloatingObjects();
                    }
                }
                flag = true;
            }
            catch
            {
                flag = false;
            }
            finally
            {
                Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                _excel = null;
            }
            return flag;
        }

        /// <summary>
        /// Gets or sets the applied result when there is a data validator error.
        /// </summary>
        /// <value>
        /// The data validation result when there is a data validator error.
        /// </value>
        public DataValidationResult ApplyResult { get; protected set; }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  true; }
        }

        Dt.Cells.UndoRedo.CellEditExtent CellEditExtent { get; set; }

        object OldValue { get; set; }

        bool OldValueIsFormula { get; set; }

        Dt.Cells.Data.Worksheet Worksheet { get; set; }

        static bool ApplyValueToCell(
            Excel excel,
            Cell bindingCell,
            bool allowFormula,
            object editorValue,
            Type valueType,
            out bool isFormulaApplied,
            out string appliedFormula)
        {
            isFormulaApplied = false;
            appliedFormula = null;
            if (bindingCell == null)
                return true;

            if (ContainsArrayFormula(bindingCell.Worksheet.FindFormulas(bindingCell.Row.Index, bindingCell.Column.Index, 1, 1)))
            {
                return false;
            }

            string str = editorValue as string;
            if (allowFormula
                && str != null
                && str.StartsWith("=")
                && str.Length > 1)
            {
                appliedFormula = str.TrimStart(new char[] { '=' });
                try
                {
                    isFormulaApplied = true;
                    bindingCell.Formula = appliedFormula;
                }
                catch
                {
                    return false;
                }
                return true;
            }

            if (!string.IsNullOrEmpty(bindingCell.Formula))
            {
                bindingCell.Formula = null;
            }
            if (!string.IsNullOrEmpty(str))
            {
                try
                {
                    if (str.StartsWith("'="))
                    {
                        str = str.Substring(1);
                    }
                    IFormatter actualFormatter = bindingCell.ActualFormatter;
                    if ((actualFormatter != null) && !(actualFormatter is AutoFormatter))
                    {
                        object obj2 = actualFormatter.Parse(str);
                        object obj3 = null;
                        if (obj2 == null)
                        {
                            obj3 = str;
                        }
                        else
                        {
                            obj3 = obj2;
                        }
                        obj3 = excel.RaiseCellValueApplying(bindingCell.Row.Index, bindingCell.Column.Index, obj3);
                        bindingCell.Value = obj3;
                    }
                    else
                    {
                        UpdateFormatter(str, bindingCell, valueType);
                    }
                    goto Label_0139;
                }
                catch (InvalidCastException)
                {
                    bindingCell.Value = editorValue as string;
                    goto Label_0139;
                }
            }
            bindingCell.Value = null;
        Label_0139:
            return true;
        }

        static bool ContainsArrayFormula(object[,] formulas)
        {
            if (formulas != null)
            {
                for (int i = 0; i < formulas.GetLength(0); i++)
                {
                    CellRange range = formulas[i, 0] as CellRange;
                    if ((range != null) && ((range.RowCount > 1) || (range.ColumnCount > 1)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void UpdateFormatter(string text, Cell cell, Type cacheValueType)
        {
            object obj2 = null;
            GeneralFormatter preferredDisplayFormatter = new GeneralFormatter().GetPreferredDisplayFormatter(text, out obj2) as GeneralFormatter;
            object obj3 = obj2;
            if (((cell.ActualFormatter != null) && (obj2 != null)) && ((cell.ActualFormatter is AutoFormatter) && !preferredDisplayFormatter.FormatString.Equals("General")))
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            else if (cell.ActualFormatter == null)
            {
                cell.Formatter = new AutoFormatter(preferredDisplayFormatter);
            }
            cell.Value = obj3;
        }
    }
}

