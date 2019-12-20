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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a clear cells value undo action on the sheet.
    /// </summary>
    public class ClearValueUndoAction : ActionBase, IUndo
    {
        private ClearRangeValueUndoAction[] _cachedActions;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ClearValueUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet to clear values on.</param>
        /// <param name="ranges">The clear value cell ranges.</param>
        public ClearValueUndoAction(Dt.Cells.Data.Worksheet sheet, CellRange[] ranges)
        {
            this.Worksheet = sheet;
            this.CellRanges = ranges;
            if ((ranges != null) && (ranges.Length > 0))
            {
                this._cachedActions = new ClearRangeValueUndoAction[ranges.Length];
                for (int i = 0; i < ranges.Length; i++)
                {
                    this._cachedActions[i] = new ClearRangeValueUndoAction(sheet, ranges[i]);
                }
            }
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
            if (this._cachedActions == null)
            {
                return false;
            }
            foreach (ClearRangeValueUndoAction action in this._cachedActions)
            {
                if (!action.CanExecute(parameter))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Executes the action on the worksheet.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        public override void Execute(object parameter)
        {
            this.SaveState();
            if (this._cachedActions != null)
            {
                base.SuspendInvalidate(parameter);
                try
                {
                    foreach (ClearRangeValueUndoAction action in this._cachedActions)
                    {
                        action.Execute(parameter);
                    }
                }
                catch (InvalidOperationException exception)
                {
                    (parameter as SheetView).RaiseInvalidOperation(exception.Message, null, null);
                    throw;
                }
                finally
                {
                    base.ResumeInvalidate(parameter);
                }
                this.RefreshUI(parameter);
            }
        }

        private void RefreshUI(object view)
        {
            SheetView view2 = view as SheetView;
            if (view2 != null)
            {
                view2.InvalidateLayout();
                view2.InvalidateViewportHorizontalArrangement(-2);
                view2.InvalidateHeaderHorizontalArrangement();
                view2.InvalidateMeasure();
                view2.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                view2.InvalidateFloatingObjects();
            }
        }

        /// <summary>
        /// Saves the state for undoing the action before executing the action.
        /// </summary>
        public void SaveState()
        {
            if (this._cachedActions != null)
            {
                foreach (ClearRangeValueUndoAction action in this._cachedActions)
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
            return ResourceStrings.undoActionClear;
        }

        /// <summary>
        /// Undoes the action using the saved state information.
        /// </summary>
        /// <param name="parameter">Object on which the undo action occurred.</param>
        /// <returns>
        /// <c>true</c> if undoing the action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            if (this._cachedActions == null)
            {
                return false;
            }
            for (int i = this._cachedActions.Length - 1; i >= 0; i--)
            {
                ClearRangeValueUndoAction action = this._cachedActions[i];
                bool flag = false;
                base.SuspendInvalidate(parameter);
                try
                {
                    flag = action.Undo(parameter);
                }
                finally
                {
                    base.ResumeInvalidate(parameter);
                }
                if (!flag)
                {
                    return false;
                }
                this.RefreshUI(parameter);
            }
            return true;
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
                foreach (ClearRangeValueUndoAction action in this._cachedActions)
                {
                    if (!action.CanUndo)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets the clear cell ranges.
        /// </summary>
        public CellRange[] CellRanges { get; private set; }

        private Dt.Cells.Data.Worksheet Worksheet { get; set; }

        private class ClearRangeValueUndoAction : ActionBase, IUndo
        {
            private Dictionary<CellRange, string> _arrayFormulas = new Dictionary<CellRange, string>();
            private List<int> _cachedFilteredColumns;
            private List<SheetTable> _cachedTables;
            private Dictionary<ulong, CellValueEntry> _cachedValues;
            private SheetView _cachedView;

            public ClearRangeValueUndoAction(Dt.Cells.Data.Worksheet sheet, CellRange clearRange)
            {
                this.Worksheet = sheet;
                this.ClearRange = clearRange;
            }

            public override bool CanExecute(object parameter)
            {
                if (this.Worksheet == null)
                {
                    return false;
                }
                CellRange range = this.FixRange(this.Worksheet, this.ClearRange);
                if (this.Worksheet.Protect && SheetView.IsAnyCellInRangeLocked(this.Worksheet, range.Row, range.Column, range.RowCount, range.ColumnCount))
                {
                    return false;
                }
                return true;
            }

            public override void Execute(object parameter)
            {
                this.SaveState();
                if (this.Worksheet != null)
                {
                    CellRange range = this.FixRange(this.Worksheet, this.ClearRange);
                    if ((range.ColumnCount > 0) && (range.RowCount > 0))
                    {
                        try
                        {
                            this._cachedView = parameter as SheetView;
                            this.Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.OnEditedCellChanged);
                            this.Worksheet.Clear(this.ClearRange.Row, this.ClearRange.Column, this.ClearRange.RowCount, this.ClearRange.ColumnCount, SheetArea.Cells, StorageType.Data);
                            this.RefreshUI(parameter);
                        }
                        finally
                        {
                            this.Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.OnEditedCellChanged);
                            this._cachedView = null;
                        }
                    }
                }
            }

            private CellRange FixRange(Dt.Cells.Data.Worksheet sheet, CellRange range)
            {
                int row = (range.Row < 0) ? 0 : range.Row;
                int column = (range.Column < 0) ? 0 : range.Column;
                int rowCount = (range.Row < 0) ? sheet.RowCount : range.RowCount;
                return new CellRange(row, column, rowCount, (range.Column < 0) ? sheet.ColumnCount : range.ColumnCount);
            }

            private void OnEditedCellChanged(object sender, CellChangedEventArgs e)
            {
                if ((this._cachedView != null) && string.Equals(e.PropertyName, "Value"))
                {
                    CellRange range = this.FixRange(this.Worksheet, this.ClearRange);
                    int row = e.Row - range.Row;
                    int column = e.Column - range.Column;
                    ulong num3 = CopyMoveHelper.ConvertToKey(row, column);
                    if (this._cachedValues.ContainsKey(num3))
                    {
                        CellValueEntry entry = this._cachedValues[num3];
                        if (entry.Value is string)
                        {
                            if (!string.IsNullOrEmpty((string) (entry.Value as string)))
                            {
                                this._cachedView.RaiseValueChanged(e.Row, e.Column);
                            }
                        }
                        else if (entry.Value != null)
                        {
                            this._cachedView.RaiseValueChanged(e.Row, e.Column);
                        }
                    }
                }
            }

            private void RefreshUI(object view)
            {
                SheetView view2 = view as SheetView;
                if (view2 != null)
                {
                    view2.RefreshCellAreaViewport(0, 0, this.Worksheet.RowCount, this.Worksheet.ColumnCount);
                }
            }

            public void SaveState()
            {
                if (this.Worksheet != null)
                {
                    this._cachedTables = new List<SheetTable>();
                    SheetTable[] tables = this.Worksheet.GetTables();
                    if ((tables != null) && (tables.Length > 0))
                    {
                        foreach (SheetTable table in tables)
                        {
                            if (this.ClearRange.Contains(table.Range))
                            {
                                this._cachedTables.Add(table);
                            }
                        }
                    }
                    this._cachedFilteredColumns = new List<int>();
                    RowFilterBase rowFilter = this.Worksheet.RowFilter;
                    if (((rowFilter != null) && (rowFilter.Range != null)) && rowFilter.IsFiltered)
                    {
                        int row = rowFilter.Range.Row;
                        int rowCount = rowFilter.Range.RowCount;
                        if (rowFilter.ShowFilterButton)
                        {
                            row = rowFilter.Range.Row - 1;
                            rowCount = rowFilter.Range.RowCount + 1;
                            if (row < 0)
                            {
                                row = -1;
                                rowCount = -1;
                            }
                        }
                        if (this.ClearRange.Contains(row, rowFilter.Range.Column, rowCount, rowFilter.Range.ColumnCount))
                        {
                            int num3 = (rowFilter.Range.Column < 0) ? 0 : rowFilter.Range.Column;
                            int num4 = (rowFilter.Range.Column < 0) ? this.Worksheet.ColumnCount : rowFilter.Range.ColumnCount;
                            for (int i = 0; i < num4; i++)
                            {
                                if (rowFilter.IsColumnFiltered(num3 + i))
                                {
                                    this._cachedFilteredColumns.Add(num3 + i);
                                }
                            }
                        }
                    }
                    CellRange range = this.FixRange(this.Worksheet, this.ClearRange);
                    if ((range.ColumnCount > 0) && (range.RowCount > 0))
                    {
                        List<CellRange> list = new List<CellRange>();
                        List<string> list2 = new List<string>();
                        object[,] objArray = this.Worksheet.FindFormulas(range.Row, range.Column, range.RowCount, range.ColumnCount);
                        for (int j = 0; j < objArray.GetLength(0); j++)
                        {
                            CellRange range2 = objArray[j, 0] as CellRange;
                            string str = (string) (objArray[j, 1] as string);
                            Match match = new Regex(@"^\s*{\s*(.*?)}\s*$").Match(str);
                            if (((match.Success && (range2.Row >= range.Row)) && ((range2.Column >= range.Column) && ((range2.Row + range2.RowCount) <= (range.Row + range.RowCount)))) && ((range2.Column + range2.ColumnCount) <= (range.Column + range.ColumnCount)))
                            {
                                this._arrayFormulas[range2] = match.Groups[1].Value;
                                list.Add(range2);
                                list2.Add(str);
                            }
                        }
                        this._cachedValues = new Dictionary<ulong, CellValueEntry>();
                        for (int k = 0; k < range.RowCount; k++)
                        {
                            for (int m = 0; m < range.ColumnCount; m++)
                            {
                                int num9 = range.Row + k;
                                int column = range.Column + m;
                                bool flag = false;
                                foreach (CellRange range3 in list)
                                {
                                    if (range3.Contains(num9, column))
                                    {
                                        column = (range3.Column + range3.ColumnCount) - 1;
                                        flag = true;
                                        break;
                                    }
                                }
                                if (!flag)
                                {
                                    string formula = this.Worksheet.GetFormula(num9, column);
                                    if (!string.IsNullOrEmpty(formula))
                                    {
                                        this._cachedValues.Add(CopyMoveHelper.ConvertToKey(k, m), new CellValueEntry(formula, true));
                                    }
                                    else
                                    {
                                        object obj2 = this.Worksheet.GetValue(num9, column);
                                        if (obj2 != null)
                                        {
                                            this._cachedValues.Add(CopyMoveHelper.ConvertToKey(k, m), new CellValueEntry(obj2, false));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            public bool Undo(object parameter)
            {
                if (this.Worksheet != null)
                {
                    bool flag = false;
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
                            SheetTable table2 = this.Worksheet.AddTable(table.Name, row, column, rowCount, columnCount, table.Style);
                            table2.BandedColumns = table.BandedColumns;
                            table2.BandedRows = table.BandedRows;
                            table2.HighlightFirstColumn = table.HighlightFirstColumn;
                            table2.HighlightLastColumn = table.HighlightLastColumn;
                            table2.ShowFooter = table.ShowFooter;
                            table2.ShowHeader = table.ShowHeader;
                        }
                    }
                    CellRange range2 = this.FixRange(this.Worksheet, this.ClearRange);
                    if (((this._cachedValues != null) && (range2.ColumnCount > 0)) && (range2.RowCount > 0))
                    {
                        try
                        {
                            this._cachedView = parameter as SheetView;
                            this.Worksheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.OnEditedCellChanged);
                            int num5 = range2.RowCount;
                            int num6 = range2.ColumnCount;
                            for (int i = 0; i < num5; i++)
                            {
                                for (int j = 0; j < num6; j++)
                                {
                                    int num9 = range2.Row + i;
                                    int num10 = range2.Column + j;
                                    ulong num11 = CopyMoveHelper.ConvertToKey(i, j);
                                    if (this._cachedValues.ContainsKey(num11))
                                    {
                                        CellValueEntry entry = this._cachedValues[num11];
                                        if (entry.IsFormula)
                                        {
                                            this.Worksheet.SetFormula(num9, num10, (string) (entry.Value as string));
                                        }
                                        else
                                        {
                                            this.Worksheet.SetFormula(num9, num10, null);
                                            this.Worksheet.SetValue(num9, num10, entry.Value);
                                        }
                                    }
                                    else
                                    {
                                        this.Worksheet.SetFormula(num9, num10, null);
                                        this.Worksheet.SetValue(num9, num10, null);
                                    }
                                }
                            }
                            foreach (KeyValuePair<CellRange, string> pair in this._arrayFormulas)
                            {
                                this.Worksheet.SetArrayFormula(pair.Key.Row, pair.Key.Column, pair.Key.RowCount, pair.Key.ColumnCount, pair.Value);
                            }
                            flag = true;
                        }
                        finally
                        {
                            this.Worksheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.OnEditedCellChanged);
                            this._cachedView = null;
                        }
                    }
                    if (((this._cachedFilteredColumns != null) && (this._cachedFilteredColumns.Count > 0)) && (this.Worksheet.RowFilter != null))
                    {
                        foreach (int num12 in this._cachedFilteredColumns)
                        {
                            this.Worksheet.RowFilter.Filter(num12);
                        }
                    }
                    if (flag)
                    {
                        this.RefreshUI(parameter);
                        return true;
                    }
                }
                return false;
            }

            public bool CanUndo
            {
                get { return  (this._cachedValues != null); }
            }

            public CellRange ClearRange { get; private set; }

            private Dt.Cells.Data.Worksheet Worksheet { get; set; }

            [StructLayout(LayoutKind.Sequential)]
            private struct CellValueEntry
            {
                public object Value;
                public bool IsFormula;
                public CellValueEntry(object value, bool isFormula)
                {
                    this.Value = value;
                    this.IsFormula = isFormula;
                }
            }
        }
    }
}

