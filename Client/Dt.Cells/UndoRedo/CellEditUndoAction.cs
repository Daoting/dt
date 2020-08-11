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
    /// Represents a cell edit undo action that applies a new value to a cell on the sheet.
    /// </summary>
    public class CellEditUndoAction : ActionBase, IUndo
    {
        SheetView _cachedView;

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

        DataValidationResult ApplyEditing(SheetView sheetView)
        {
            SheetView view = sheetView;
            int rowIndex = CellEditExtent.RowIndex;
            int columnIndex = CellEditExtent.ColumnIndex;
            if (view == null)
            {
                return DataValidationResult.Discard;
            }
            string newValue = CellEditExtent.NewValue;
            DataValidationResult forceApply = DataValidationResult.ForceApply;
            bool flag = true;
            if (UI.FormulaUtility.IsFormula(newValue))
            {
                string str2 = UI.FormulaUtility.StringVariantToInvariant(sheetView.ActiveSheet, newValue);
                flag = Worksheet.IsValid(rowIndex, columnIndex, str2);
            }
            else
            {
                object obj2 = UI.ConditionValueConverter.TryDateTime(newValue, true);
                if (obj2 == null)
                {
                    obj2 = UI.ConditionValueConverter.TryDouble(newValue, true);
                }
                if (obj2 != null)
                {
                    flag = Worksheet.IsValid(rowIndex, columnIndex, obj2);
                }
                else
                {
                    flag = Worksheet.IsValid(rowIndex, columnIndex, newValue);
                }
            }
            if (!flag)
            {
                forceApply = ValidationError(view, rowIndex, columnIndex, newValue);
            }
            switch (forceApply)
            {
                case DataValidationResult.Discard:
                case DataValidationResult.Retry:
                    return forceApply;
            }
            if (forceApply != DataValidationResult.ForceApply)
            {
                throw new NotSupportedException(ResourceStrings.undoActionCellEditInvalidValidationFlag);
            }
            Cell bindingCell = Worksheet.Cells[rowIndex, columnIndex];
            Type valueType = null;
            if ((bindingCell != null) && (bindingCell.Value != null))
            {
                valueType = bindingCell.Value.GetType();
            }
            bool isFormulaApplied = false;
            string appliedFormula = null;
            bool flag3 = false;
            using (((IUIActionExecuter) sheetView.ActiveSheet).BeginUIAction())
            {
                flag3 = CellItemBase.ApplyValueToCell(sheetView, bindingCell, view.CanUserEditFormula, newValue, valueType, out isFormulaApplied, out appliedFormula);
            }
            if (!flag3)
            {
                sheetView.RaiseInvalidOperation(isFormulaApplied ? string.Format(ResourceStrings.undoActionCannotApplyFormula, (object[]) new object[] { appliedFormula }) : ResourceStrings.undoActionCannotApplyValue, null, null);
                return DataValidationResult.Discard;
            }
            if (isFormulaApplied)
            {
                sheetView.RaiseUserFormulaEntered(rowIndex, columnIndex, appliedFormula);
            }
            return forceApply;
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
            SheetView sheetView = parameter as SheetView;
            if (sheetView != null)
            {
                base.SuspendInvalidate(parameter);
                SaveState();
                try
                {
                    _cachedView = sheetView;
                    Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                    ApplyResult = ApplyEditing(sheetView);
                }
                finally
                {
                    Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(OnEditedCellChanged);
                    _cachedView = null;
                }
                sheetView.ResumeInvalidate();
                switch (ApplyResult)
                {
                    case DataValidationResult.ForceApply:
                    {
                        sheetView.RefreshCellAreaViewport(0, 0, Worksheet.RowCount, Worksheet.ColumnCount);
                        IList<SpreadChartBase> list = Dt.Cells.Data.SpreadChartUtility.GetChartShapeAffectedCellChanged(Worksheet, CellEditExtent.RowIndex, CellEditExtent.ColumnIndex);
                        if (list.Count <= 0)
                        {
                            sheetView.InvalidateFloatingObjects();
                            return;
                        }
                        sheetView.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                        return;
                    }
                }
                throw new ActionFailedException(this);
            }
        }

        void OnEditedCellChanged(object sender, CellChangedEventArgs e)
        {
            if ((_cachedView != null) && string.Equals(e.PropertyName, "Value"))
            {
                _cachedView.RaiseValueChanged(e.Row, e.Column);
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
                SheetView view = parameter as SheetView;
                base.SuspendInvalidate(parameter);
                if (view != null)
                {
                    _cachedView = view;
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
                        view.InvalidateFloatingObjects(Enumerable.ToArray<SpreadChartBase>((IEnumerable<SpreadChartBase>) list));
                    }
                    else
                    {
                        view.InvalidateFloatingObjects();
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
                _cachedView = null;
            }
            return flag;
        }

        DataValidationResult ValidationError(SheetView sheetView, int row, int column, string text)
        {
            DataValidationResult forceApply = DataValidationResult.ForceApply;
            StyleInfo info = Worksheet.GetActualStyleInfo(row, column, SheetArea.Cells);
            DataValidator validator = (info == null) ? null : info.DataValidator;
            if (validator != null)
            {
                ValidationErrorEventArgs eventArgs = new ValidationErrorEventArgs(row, column, validator.Clone() as DataValidator);
                sheetView.RaiseValidationError(row, column, eventArgs);
                forceApply = eventArgs.ValidationResult;
            }
            if ((forceApply == DataValidationResult.ForceApply) || (forceApply == DataValidationResult.Discard))
            {
                sheetView.FocusInternal();
            }
            return forceApply;
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
    }
}

