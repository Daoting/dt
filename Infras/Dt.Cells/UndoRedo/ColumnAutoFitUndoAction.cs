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
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents the undo actions for automatic fit of a column on a sheet. 
    /// </summary>
    public class ColumnAutoFitUndoAction : ActionBase, IUndo
    {
        ColumnAutoFitExtent[] _columns;
        double[] _oldSizes;
        bool[] _oldVisibles;
        bool _rowHeader;
        Worksheet _sheet;

        /// <summary>
        /// Creates a new undo action for column automatic fit.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="columns">The resized columns.</param>
        /// <param name="rowHeader">Whether the resized column is in the row header area.</param>
        public ColumnAutoFitUndoAction(Worksheet sheet, ColumnAutoFitExtent[] columns, bool rowHeader)
        {
            _sheet = sheet;
            _columns = columns;
            _rowHeader = rowHeader;
        }

        /// <summary>
        /// Defines whether the column automatic fit action can execute.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if the action can execute; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(object sender)
        {
            if (_sheet == null)
            {
                return true;
            }
            if (_sheet.Protect)
            {
                return false;
            }
            return (((_sheet != null) && (_columns != null)) && (_columns.Length > 0));
        }

        /// <summary>
        /// Performs the column automatic fit action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (CanExecute(sender))
            {
                Excel excel = sender as Excel;
                int[] columnsReiszed = GetColumnsReiszed(_columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((excel != null) && excel.RaiseColumnWidthChanging(columnsReiszed, _rowHeader))
                    {
                        return;
                    }
                    SaveState();
                    int num = _rowHeader ? _sheet.RowHeader.ColumnCount : _sheet.ColumnCount;
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        ColumnAutoFitExtent extent = _columns[i];
                        if ((0 <= extent.Column) && (extent.Column < num))
                        {
                            if (_rowHeader && _sheet.GetColumnResizable(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                            {
                                double width = Math.Ceiling(excel.GetColumnAutoFitValue(extent.Column, true));
                                if (width >= 0.0)
                                {
                                    width = (width + 1.0) + 4.0;
                                }
                                if (width != _sheet.GetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                                {
                                    _sheet.SetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                }
                                _sheet.SetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, true);
                            }
                            else if (_sheet.GetColumnResizable(extent.Column, SheetArea.Cells))
                            {
                                double num4 = Math.Ceiling(excel.GetColumnAutoFitValue(extent.Column, false));
                                if (num4 >= 0.0)
                                {
                                    num4 = (num4 + 1.0) + 4.0;
                                }
                                if (num4 != _sheet.GetColumnWidth(extent.Column, SheetArea.Cells))
                                {
                                    _sheet.SetColumnWidth(extent.Column, SheetArea.Cells, num4);
                                }
                                _sheet.SetColumnVisible(extent.Column, SheetArea.Cells, true);
                            }
                        }
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                if (excel != null)
                {
                    excel.InvalidateLayout();
                    excel.InvalidateViewportHorizontalArrangement(-2);
                    excel.InvalidateHeaderHorizontalArrangement();
                    excel.InvalidateMeasure();
                    excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    excel.RefreshFloatingObjects();
                    excel.RaiseColumnWidthChanged(columnsReiszed, _rowHeader);
                }
            }
        }

        int[] GetColumnsReiszed(ColumnAutoFitExtent[] columnWidthChangeExtents)
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
            if (((_sheet != null) && (_columns != null)) && (_columns.Length > 0))
            {
                numArray = new double[_columns.Length];
                flagArray = new bool[_columns.Length];
                int num = _rowHeader ? _sheet.RowHeader.ColumnCount : _sheet.ColumnCount;
                for (int i = 0; i < _columns.Length; i++)
                {
                    ColumnAutoFitExtent extent = _columns[i];
                    if ((0 <= extent.Column) && (extent.Column < num))
                    {
                        if (_rowHeader)
                        {
                            numArray[i] = _sheet.GetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader);
                            flagArray[i] = _sheet.GetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader);
                        }
                        else
                        {
                            numArray[i] = _sheet.GetColumnWidth(extent.Column, SheetArea.Cells);
                            flagArray[i] = _sheet.GetColumnVisible(extent.Column, SheetArea.Cells);
                        }
                    }
                    else
                    {
                        numArray[i] = -1.0;
                        flagArray[i] = false;
                    }
                }
            }
            _oldSizes = numArray;
            _oldVisibles = flagArray;
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
            if (((_sheet != null) && (_columns != null)) && (_columns.Length > 0))
            {
                Excel excel = sender as Excel;
                int[] columnsReiszed = GetColumnsReiszed(_columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((excel != null) && excel.RaiseColumnWidthChanging(columnsReiszed, _rowHeader))
                    {
                        return true;
                    }
                    int num2 = _rowHeader ? _sheet.RowHeader.ColumnCount : _sheet.ColumnCount;
                    for (int i = 0; i < _columns.Length; i++)
                    {
                        ColumnAutoFitExtent extent = _columns[i];
                        double width = _oldSizes[i];
                        bool flag3 = _oldVisibles[i];
                        if (((0 <= i) && (i < num2)) && (width != -1.0))
                        {
                            if (_rowHeader && _sheet.GetColumnResizable(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader))
                            {
                                _sheet.SetColumnWidth(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                _sheet.SetColumnVisible(extent.Column, SheetArea.CornerHeader | SheetArea.RowHeader, flag3);
                                flag = true;
                            }
                            else if (_sheet.GetColumnResizable(extent.Column, SheetArea.Cells))
                            {
                                _sheet.SetColumnWidth(extent.Column, SheetArea.Cells, width);
                                _sheet.SetColumnVisible(extent.Column, SheetArea.Cells, flag3);
                                flag = true;
                            }
                        }
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                if (excel != null)
                {
                    excel.InvalidateLayout();
                    excel.InvalidateViewportHorizontalArrangement(-2);
                    excel.InvalidateHeaderHorizontalArrangement();
                    excel.InvalidateMeasure();
                    excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    excel.RaiseColumnWidthChanged(columnsReiszed, _rowHeader);
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

