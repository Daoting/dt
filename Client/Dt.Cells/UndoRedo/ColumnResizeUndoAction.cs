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
    /// Represents the undo actions for resizing a column on a sheet. 
    /// </summary>
    public class ColumnResizeUndoAction : ActionBase, IUndo
    {
        /// <summary>
        /// Specifies the extent of the column width change.
        /// </summary>
        private ColumnResizeExtent[] columns;
        /// <summary>
        /// Specifies the old sizes.
        /// </summary>
        private object[] oldsizes;
        /// <summary>
        /// Specifies the old visible items.
        /// </summary>
        private object[] oldVisibles;
        /// <summary>
        /// Specifies whether the row header is involved.
        /// </summary>
        private bool rowHeader;
        /// <summary>
        /// Specifies the sheet that contains a resized column.
        /// </summary>
        private Worksheet sheet;
        /// <summary>
        /// Specifies the size.
        /// </summary>
        private double size;

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
            if (this.sheet != null)
            {
                return !this.sheet.Protect;
            }
            return true;
        }

        /// <summary>
        /// Performs the column resize action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((this.sheet != null) && (this.columns != null)) && this.CanExecute(sender))
            {
                SheetView view = sender as SheetView;
                int[] columnsReiszed = this.GetColumnsReiszed(this.columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseColumnWidthChanging(columnsReiszed, this.rowHeader))
                    {
                        return;
                    }
                    this.SaveState();
                    ViewportInfo viewportInfo = this.sheet.GetViewportInfo();
                    int[] numArray2 = new int[viewportInfo.LeftColumns.Length];
                    for (int i = 0; i < viewportInfo.LeftColumns.Length; i++)
                    {
                        int num2 = viewportInfo.LeftColumns[i];
                        numArray2[i] = num2;
                        for (int k = num2 - 1; k >= this.sheet.FrozenColumnCount; k--)
                        {
                            if (this.sheet.GetActualColumnWidth(k, SheetArea.Cells) > 0.0)
                            {
                                break;
                            }
                            if (this.sheet.Columns[k].CanUserResize)
                            {
                                numArray2[i] = k;
                            }
                        }
                    }
                    int num4 = 0x7fffffff;
                    int num5 = this.rowHeader ? this.sheet.RowHeader.ColumnCount : this.sheet.ColumnCount;
                    for (int j = 0; j < this.columns.Length; j++)
                    {
                        ColumnResizeExtent extent = this.columns[j];
                        for (int m = extent.FirstColumn; m <= extent.LastColumn; m++)
                        {
                            if ((0 <= m) && (m < num5))
                            {
                                if ((this.rowHeader && this.sheet.GetColumnResizable(m, SheetArea.CornerHeader | SheetArea.RowHeader)) && (this.size != this.sheet.GetColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader)))
                                {
                                    this.sheet.SetColumnWidth(m, SheetArea.CornerHeader | SheetArea.RowHeader, this.size);
                                }
                                else if (this.sheet.GetColumnResizable(m, SheetArea.Cells) && (this.size != this.sheet.GetColumnWidth(m, SheetArea.Cells)))
                                {
                                    this.sheet.SetColumnWidth(m, SheetArea.Cells, this.size);
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
                if (view != null)
                {
                    view.InvalidateLayout();
                    view.InvalidateViewportHorizontalArrangement(-2);
                    view.InvalidateHeaderHorizontalArrangement();
                    view.InvalidateMeasure();
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    view.InvalidateFloatingObjects();
                    view.RaiseColumnWidthChanged(columnsReiszed, this.rowHeader);
                }
            }
        }

        private int[] GetColumnsReiszed(ColumnResizeExtent[] columnWidthChangeExtents)
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
            if ((this.sheet != null) && (this.columns != null))
            {
                int num = this.rowHeader ? this.sheet.RowHeader.ColumnCount : this.sheet.ColumnCount;
                objArray = new object[this.columns.Length];
                objArray2 = new object[this.columns.Length];
                for (int i = 0; i < this.columns.Length; i++)
                {
                    ColumnResizeExtent extent = this.columns[i];
                    objArray[i] = new double[(extent.LastColumn - extent.FirstColumn) + 1];
                    objArray2[i] = new bool[(extent.LastColumn - extent.FirstColumn) + 1];
                    for (int j = extent.FirstColumn; j <= extent.LastColumn; j++)
                    {
                        if ((0 <= j) && (j < num))
                        {
                            ((double[]) objArray[i])[j - extent.FirstColumn] = this.rowHeader ? this.sheet.GetColumnWidth(j, SheetArea.CornerHeader | SheetArea.RowHeader) : this.sheet.GetColumnWidth(j, SheetArea.Cells);
                            ((bool[]) objArray2[i])[j - extent.FirstColumn] = this.rowHeader ? this.sheet.GetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader) : this.sheet.GetColumnVisible(j, SheetArea.Cells);
                        }
                        else
                        {
                            ((double[]) objArray[i])[j - extent.FirstColumn] = -1.0;
                            ((bool[]) objArray2[i])[j - extent.FirstColumn] = false;
                        }
                    }
                }
            }
            this.oldsizes = objArray;
            this.oldVisibles = objArray2;
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
            if ((this.sheet != null) && (this.columns != null))
            {
                SheetView view = sender as SheetView;
                int[] columnsReiszed = this.GetColumnsReiszed(this.columns);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseColumnWidthChanging(columnsReiszed, this.rowHeader))
                    {
                        return true;
                    }
                    int num = this.rowHeader ? this.sheet.RowHeader.ColumnCount : this.sheet.ColumnCount;
                    for (int i = 0; i < this.columns.Length; i++)
                    {
                        ColumnResizeExtent extent = this.columns[i];
                        for (int j = extent.FirstColumn; j <= extent.LastColumn; j++)
                        {
                            double width = ((double[]) this.oldsizes[i])[j - extent.FirstColumn];
                            bool flag3 = ((bool[]) this.oldVisibles[i])[j - extent.FirstColumn];
                            if (((0 <= j) && (j < num)) && (width != -1.0))
                            {
                                if (this.rowHeader && this.sheet.GetColumnResizable(j, SheetArea.CornerHeader | SheetArea.RowHeader))
                                {
                                    this.sheet.SetColumnWidth(j, SheetArea.CornerHeader | SheetArea.RowHeader, width);
                                    flag = true;
                                }
                                else if (this.sheet.GetColumnResizable(j, SheetArea.Cells))
                                {
                                    this.sheet.SetColumnWidth(j, SheetArea.Cells, width);
                                    flag = true;
                                }
                                if (this.rowHeader && (this.sheet.GetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader) != flag3))
                                {
                                    this.sheet.SetColumnVisible(j, SheetArea.CornerHeader | SheetArea.RowHeader, flag3);
                                    flag = true;
                                }
                                else if (this.sheet.GetColumnVisible(j, SheetArea.Cells) != flag3)
                                {
                                    this.sheet.SetColumnVisible(j, SheetArea.Cells, flag3);
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
                if (view != null)
                {
                    view.InvalidateLayout();
                    view.InvalidateViewportHorizontalArrangement(-2);
                    view.InvalidateHeaderHorizontalArrangement();
                    view.InvalidateMeasure();
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader);
                    view.RaiseColumnWidthChanged(columnsReiszed, this.rowHeader);
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

