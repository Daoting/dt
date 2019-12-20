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
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents the undo actions for automatic fit of a column on a sheet. 
    /// </summary>
    public class ColumnAutoFitUndoAction : ActionBase, IUndo
    {
        private ColumnAutoFitExtent[] _columns;
        private double[] _oldSizes;
        private bool[] _oldVisibles;
        private bool _rowHeader;
        private Worksheet _sheet;

        /// <summary>
        /// Creates a new undo action for column automatic fit.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="columns">The resized columns.</param>
        /// <param name="rowHeader">Whether the resized column is in the row header area.</param>
        public ColumnAutoFitUndoAction(Worksheet sheet, ColumnAutoFitExtent[] columns, bool rowHeader)
        {
            this._sheet = sheet;
            this._columns = columns;
            this._rowHeader = rowHeader;
        }

        /// <summary>
        /// Defines whether the column automatic fit action can execute.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if the action can execute; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(object sender)
        {
            if (this._sheet == null)
            {
                return true;
            }
            if (this._sheet.Protect)
            {
                return false;
            }
            return (((this._sheet != null) && (this._columns != null)) && (this._columns.Length > 0));
        }

        /// <summary>
        /// Performs the column automatic fit action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (this.CanExecute(sender))
            {
                SheetView view = sender as SheetView;
                int[] columnsReiszed = this.GetColumnsReiszed(this._columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseColumnWidthChanging(columnsReiszed, this._rowHeader))
                    {
                        return;
                    }
                    this.SaveState();
                    int num = this._rowHeader ? this._sheet.RowHeader.ColumnCount : this._sheet.ColumnCount;
                    for (int i = 0; i < this._columns.Length; i++)
                    {
                        ColumnAutoFitExtent extent = this._columns[i];
                        if ((0 <= extent.Column) && (extent.Column < num))
                        {
                            if (this._rowHeader && this._sheet.GetColumnResizable(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                            {
                                double width = Math.Ceiling(view.GetColumnAutoFitValue(extent.Column, true));
                                if (width >= 0.0)
                                {
                                    width = (width + 1.0) + 4.0;
                                }
                                if (width != this._sheet.GetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                                {
                                    this._sheet.SetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                }
                                this._sheet.SetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, true);
                            }
                            else if (this._sheet.GetColumnResizable(extent.Column, SheetArea.Cells))
                            {
                                double num4 = Math.Ceiling(view.GetColumnAutoFitValue(extent.Column, false));
                                if (num4 >= 0.0)
                                {
                                    num4 = (num4 + 1.0) + 4.0;
                                }
                                if (num4 != this._sheet.GetColumnWidth(extent.Column, SheetArea.Cells))
                                {
                                    this._sheet.SetColumnWidth(extent.Column, SheetArea.Cells, num4);
                                }
                                this._sheet.SetColumnVisible(extent.Column, SheetArea.Cells, true);
                            }
                        }
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                if (view != null)
                {
                    view.InvalidateLayout();
                    view.InvalidateViewportHorizontalArrangement(-2);
                    view.InvalidateHeaderHorizontalArrangement();
                    view.InvalidateMeasure();
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    view.InvalidateFloatingObjects();
                    view.RaiseColumnWidthChanged(columnsReiszed, this._rowHeader);
                }
            }
        }

        private int[] GetColumnsReiszed(ColumnAutoFitExtent[] columnWidthChangeExtents)
        {
            List<int> list = new List<int>();
            foreach (ColumnAutoFitExtent extent in columnWidthChangeExtents)
            {
                list.Add(extent.Column);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Saves undo information.
        /// </summary>
        public void SaveState()
        {
            double[] numArray = null;
            bool[] flagArray = null;
            if (((this._sheet != null) && (this._columns != null)) && (this._columns.Length > 0))
            {
                numArray = new double[this._columns.Length];
                flagArray = new bool[this._columns.Length];
                int num = this._rowHeader ? this._sheet.RowHeader.ColumnCount : this._sheet.ColumnCount;
                for (int i = 0; i < this._columns.Length; i++)
                {
                    ColumnAutoFitExtent extent = this._columns[i];
                    if ((0 <= extent.Column) && (extent.Column < num))
                    {
                        if (this._rowHeader)
                        {
                            numArray[i] = this._sheet.GetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader);
                            flagArray[i] = this._sheet.GetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader);
                        }
                        else
                        {
                            numArray[i] = this._sheet.GetColumnWidth(extent.Column, SheetArea.Cells);
                            flagArray[i] = this._sheet.GetColumnVisible(extent.Column, SheetArea.Cells);
                        }
                    }
                    else
                    {
                        numArray[i] = -1.0;
                        flagArray[i] = false;
                    }
                }
            }
            this._oldSizes = numArray;
            this._oldVisibles = flagArray;
        }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionColumnAutoFit;
        }

        /// <summary>
        /// Undoes the column automatic fit action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if successful; otherwise <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((this._sheet != null) && (this._columns != null)) && (this._columns.Length > 0))
            {
                SheetView view = sender as SheetView;
                int[] columnsReiszed = this.GetColumnsReiszed(this._columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseColumnWidthChanging(columnsReiszed, this._rowHeader))
                    {
                        return true;
                    }
                    int num2 = this._rowHeader ? this._sheet.RowHeader.ColumnCount : this._sheet.ColumnCount;
                    for (int i = 0; i < this._columns.Length; i++)
                    {
                        ColumnAutoFitExtent extent = this._columns[i];
                        double width = this._oldSizes[i];
                        bool flag3 = this._oldVisibles[i];
                        if (((0 <= i) && (i < num2)) && (width != -1.0))
                        {
                            if (this._rowHeader && this._sheet.GetColumnResizable(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                            {
                                this._sheet.SetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                this._sheet.SetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, flag3);
                                flag = true;
                            }
                            else if (this._sheet.GetColumnResizable(extent.Column, SheetArea.Cells))
                            {
                                this._sheet.SetColumnWidth(extent.Column, SheetArea.Cells, width);
                                this._sheet.SetColumnVisible(extent.Column, SheetArea.Cells, flag3);
                                flag = true;
                            }
                        }
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                if (view != null)
                {
                    view.InvalidateLayout();
                    view.InvalidateViewportHorizontalArrangement(-2);
                    view.InvalidateHeaderHorizontalArrangement();
                    view.InvalidateMeasure();
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    view.RaiseColumnWidthChanged(columnsReiszed, this._rowHeader);
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
    }
}

