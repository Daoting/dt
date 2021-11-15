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
    /// Represents the undo actions for resizing a column on a sheet. 
    /// </summary>
    public class ColumnResizeUndoAction : ActionBase, IUndo
    {
        /// <summary>
        /// Specifies the extent of the column width change.
        /// </summary>
        ColumnResizeExtent[] columns;
        /// <summary>
        /// Specifies the old sizes.
        /// </summary>
        object[] oldsizes;
        /// <summary>
        /// Specifies the old visible items.
        /// </summary>
        object[] oldVisibles;
        /// <summary>
        /// Specifies whether the row header is involved.
        /// </summary>
        bool rowHeader;
        /// <summary>
        /// Specifies the sheet that contains a resized column.
        /// </summary>
        Worksheet sheet;
        /// <summary>
        /// Specifies the size.
        /// </summary>
        double size;

        /// <summary>
        /// Creates a new undo action for column resizing.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="columns">The resize columns.</param>
        /// <param name="size">The resized size.</param>
        /// <param name="rowHeader">Whether the column being resized is in the row header area.</param>
        public ColumnResizeUndoAction(Worksheet sheet, ColumnResizeExtent[] columns, double size, bool rowHeader)
        {
            this.sheet = sheet;
            this.columns = columns;
            this.size = size;
            this.rowHeader = rowHeader;
        }

        /// <summary>
        /// Define whether the column resize action can execute.
        /// </summary>
        /// <param name="parameter">Object on which the action occurred.</param>
        /// <returns><c>true</c> if the action can execute; otherwise <c>false</c>.</returns>
        public override bool CanExecute(object parameter)
        {
            if (sheet != null)
            {
                return !sheet.Protect;
            }
            return true;
        }

        /// <summary>
        /// Performs the column resize action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((sheet != null) && (columns != null)) && CanExecute(sender))
            {
                Excel excel = sender as Excel;
                int[] columnsReiszed = GetColumnsReiszed(columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((excel != null) && excel.RaiseColumnWidthChanging(columnsReiszed, rowHeader))
                    {
                        return;
                    }
                    SaveState();
                    ViewportInfo viewportInfo = sheet.GetViewportInfo();
                    int[] numArray2 = new int[viewportInfo.LeftColumns.Length];
                    for (int i = 0; i < viewportInfo.LeftColumns.Length; i++)
                    {
                        int num2 = viewportInfo.LeftColumns[i];
                        numArray2[i] = num2;
                        for (int k = num2 - 1; k >= sheet.FrozenColumnCount; k--)
                        {
                            if (sheet.GetActualColumnWidth(k, SheetArea.Cells) > 0.0)
                            {
                                break;
                            }
                            if (sheet.Columns[k].CanUserResize)
                            {
                                numArray2[i] = k;
                            }
                        }
                    }
                    int num4 = 0x7fffffff;
                    int num5 = rowHeader ? sheet.RowHeader.ColumnCount : sheet.ColumnCount;
                    for (int j = 0; j < columns.Length; j++)
                    {
                        ColumnResizeExtent extent = columns[j];
                        for (int m = extent.FirstColumn; m <= extent.LastColumn; m++)
                        {
                            if ((0 <= m) && (m < num5))
                            {
                                if ((rowHeader && sheet.GetColumnResizable(m, SheetArea.CornerHeader | SheetArea.RowHeader)) && (size != sheet.GetColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader)))
                                {
                                    sheet.SetColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader, size);
                                }
                                else if (sheet.GetColumnResizable(m, SheetArea.Cells) && (size != sheet.GetColumnWidth(m, SheetArea.Cells)))
                                {
                                    sheet.SetColumnWidth(m, SheetArea.Cells, size);
                                }
                                num4 = Math.Min(num4, m);
                            }
                        }
                    }
                    if (num4 != 0x7fffffff)
                    {
                        for (int n = 0; n < viewportInfo.LeftColumns.Length; n++)
                        {
                            int num9 = viewportInfo.LeftColumns[n];
                            int num10 = numArray2[n];
                            if ((num4 <= num9) && (num9 != num10))
                            {
                                viewportInfo.LeftColumns[n] = num10;
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
                    excel.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    excel.RefreshFloatingObjects();
                    excel.RaiseColumnWidthChanged(columnsReiszed, rowHeader);
                }
            }
        }

        int[] GetColumnsReiszed(ColumnResizeExtent[] columnWidthChangeExtents)
        {
            List<int> list = new List<int>();
            foreach (ColumnResizeExtent extent in columnWidthChangeExtents)
            {
                for (int i = extent.FirstColumn; i <= extent.LastColumn; i++)
                {
                    list.Add(i);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Saves undo information.
        /// </summary>
        public void SaveState()
        {
            object[] objArray = null;
            object[] objArray2 = null;
            if ((sheet != null) && (columns != null))
            {
                int num = rowHeader ? sheet.RowHeader.ColumnCount : sheet.ColumnCount;
                objArray = new object[columns.Length];
                objArray2 = new object[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                {
                    ColumnResizeExtent extent = columns[i];
                    objArray[i] = new double[(extent.LastColumn - extent.FirstColumn) + 1];
                    objArray2[i] = new bool[(extent.LastColumn - extent.FirstColumn) + 1];
                    for (int j = extent.FirstColumn; j <= extent.LastColumn; j++)
                    {
                        if ((0 <= j) && (j < num))
                        {
                            ((double[]) objArray[i])[j - extent.FirstColumn] = rowHeader ? sheet.GetColumnWidth(j, SheetArea.CornerHeader | SheetArea.RowHeader) : sheet.GetColumnWidth(j, SheetArea.Cells);
                            ((bool[]) objArray2[i])[j - extent.FirstColumn] = rowHeader ? sheet.GetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader) : sheet.GetColumnVisible(j, SheetArea.Cells);
                        }
                        else
                        {
                            ((double[]) objArray[i])[j - extent.FirstColumn] = -1.0;
                            ((bool[]) objArray2[i])[j - extent.FirstColumn] = false;
                        }
                    }
                }
            }
            oldsizes = objArray;
            oldVisibles = objArray2;
        }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionColumnResize;
        }

        /// <summary>
        /// Undoes the column resizing action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if ((sheet != null) && (columns != null))
            {
                Excel excel = sender as Excel;
                int[] columnsReiszed = GetColumnsReiszed(columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((excel != null) && excel.RaiseColumnWidthChanging(columnsReiszed, rowHeader))
                    {
                        return true;
                    }
                    int num = rowHeader ? sheet.RowHeader.ColumnCount : sheet.ColumnCount;
                    for (int i = 0; i < columns.Length; i++)
                    {
                        ColumnResizeExtent extent = columns[i];
                        for (int j = extent.FirstColumn; j <= extent.LastColumn; j++)
                        {
                            double width = ((double[]) oldsizes[i])[j - extent.FirstColumn];
                            bool flag3 = ((bool[]) oldVisibles[i])[j - extent.FirstColumn];
                            if (((0 <= j) && (j < num)) && (width != -1.0))
                            {
                                if (rowHeader && sheet.GetColumnResizable(j, SheetArea.CornerHeader | SheetArea.RowHeader))
                                {
                                    sheet.SetColumnWidth(j, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                    flag = true;
                                }
                                else if (sheet.GetColumnResizable(j, SheetArea.Cells))
                                {
                                    sheet.SetColumnWidth(j, SheetArea.Cells, width);
                                    flag = true;
                                }
                                if (rowHeader && (sheet.GetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader) != flag3))
                                {
                                    sheet.SetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader, flag3);
                                    flag = true;
                                }
                                else if (sheet.GetColumnVisible(j, SheetArea.Cells) != flag3)
                                {
                                    sheet.SetColumnVisible(j, SheetArea.Cells, flag3);
                                    flag = true;
                                }
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
                    excel.RaiseColumnWidthChanged(columnsReiszed, rowHeader);
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

