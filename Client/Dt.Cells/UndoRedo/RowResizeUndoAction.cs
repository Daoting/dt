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
    /// Represents the undo actions for resizing a row on a sheet. 
    /// </summary>
    public class RowResizeUndoAction : ActionBase, IUndo
    {
        bool columnHeader;
        object[] oldsizes;
        object[] oldVisibles;
        RowResizeExtent[] rows;
        Worksheet sheet;
        double size;

        /// <summary>
        /// Creates a new undo action for row resizing.
        /// </summary>
        /// <param name="sheetView">The sheet.</param>
        /// <param name="rows">The resize rows.</param>
        /// <param name="size">The resize size.</param>
        /// <param name="columnHeader">Whether the row being resized is in the column header area.</param>
        public RowResizeUndoAction(Worksheet sheetView, RowResizeExtent[] rows, double size, bool columnHeader)
        {
            sheet = sheetView;
            this.rows = rows;
            this.size = size;
            this.columnHeader = columnHeader;
        }

        /// <summary>
        /// Defines whether the row resize action can execute.
        /// </summary>
        /// <param name="parameter">Object on which the action occurred.</param>
        /// <returns><c>true</c> if can execute; otherwise, <c>false</c>.</returns>
        public override bool CanExecute(object parameter)
        {
            if (sheet != null)
            {
                return !sheet.Protect;
            }
            return true;
        }

        /// <summary>
        /// Performs the row resize action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((sheet != null) && (rows != null)) && CanExecute(sender))
            {
                Excel view = sender as Excel;
                int[] rowsResized = GetRowsResized(rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, columnHeader))
                    {
                        return;
                    }
                    SaveState();
                    ViewportInfo viewportInfo = sheet.GetViewportInfo();
                    int[] numArray2 = new int[viewportInfo.TopRows.Length];
                    for (int i = 0; i < viewportInfo.TopRows.Length; i++)
                    {
                        int num2 = viewportInfo.TopRows[i];
                        numArray2[i] = num2;
                        for (int k = num2 - 1; k >= sheet.FrozenRowCount; k--)
                        {
                            if (sheet.GetActualRowHeight(k, SheetArea.Cells) > 0.0)
                            {
                                break;
                            }
                            if (sheet.Rows[k].CanUserResize)
                            {
                                numArray2[i] = k;
                            }
                        }
                    }
                    int num4 = columnHeader ? sheet.ColumnHeader.RowCount : sheet.RowCount;
                    int num5 = 0x7fffffff;
                    for (int j = 0; j < rows.Length; j++)
                    {
                        RowResizeExtent extent = rows[j];
                        for (int m = extent.FirstRow; m <= extent.LastRow; m++)
                        {
                            if ((0 <= m) && (m < num4))
                            {
                                if ((columnHeader && sheet.GetRowResizable(m, SheetArea.ColumnHeader)) && (size != sheet.GetRowHeight(m, SheetArea.ColumnHeader)))
                                {
                                    sheet.SetRowHeight(m, SheetArea.ColumnHeader, size);
                                }
                                else if (sheet.GetRowResizable(m, SheetArea.Cells) && (size != sheet.GetRowHeight(m, SheetArea.Cells)))
                                {
                                    sheet.SetRowHeight(m, SheetArea.Cells, size);
                                }
                                num5 = Math.Min(m, num5);
                            }
                        }
                    }
                    if (num5 != 0x7fffffff)
                    {
                        for (int n = 0; n < viewportInfo.TopRows.Length; n++)
                        {
                            int num9 = viewportInfo.TopRows[n];
                            int num10 = numArray2[n];
                            if ((num5 <= num9) && (num9 != num10))
                            {
                                viewportInfo.TopRows[n] = num10;
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
                    view.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.ColumnHeader | SheetArea.RowHeader);
                    view.RefreshFloatingObjects();
                    view.RaiseRowHeightChanged(rowsResized, columnHeader);
                }
            }
        }

        int[] GetRowsResized(RowResizeExtent[] rowHeightChangeExtents)
        {
            List<int> list = new List<int>();
            foreach (RowResizeExtent extent in rowHeightChangeExtents)
            {
                for (int i = extent.FirstRow; i <= extent.LastRow; i++)
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
            if ((sheet != null) && (rows != null))
            {
                int num = columnHeader ? sheet.ColumnHeader.RowCount : sheet.RowCount;
                objArray = new object[rows.Length];
                objArray2 = new object[rows.Length];
                for (int i = 0; i < rows.Length; i++)
                {
                    RowResizeExtent extent = rows[i];
                    objArray[i] = new double[(extent.LastRow - extent.FirstRow) + 1];
                    objArray2[i] = new bool[(extent.LastRow - extent.FirstRow) + 1];
                    for (int j = extent.FirstRow; j <= extent.LastRow; j++)
                    {
                        if ((0 <= j) && (j < num))
                        {
                            ((double[]) objArray[i])[j - extent.FirstRow] = columnHeader ? sheet.GetRowHeight(j, SheetArea.ColumnHeader) : sheet.GetRowHeight(j, SheetArea.Cells);
                            ((bool[]) objArray2[i])[j - extent.FirstRow] = columnHeader ? sheet.GetRowVisible(j, SheetArea.ColumnHeader) : sheet.GetRowVisible(j, SheetArea.Cells);
                        }
                        else
                        {
                            ((double[]) objArray[i])[j - extent.FirstRow] = -1.0;
                            ((bool[]) objArray2[i])[j - extent.FirstRow] = false;
                        }
                    }
                }
            }
            oldVisibles = objArray2;
            oldsizes = objArray;
        }

        /// <summary>
        /// Returns a string that represents this object.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionRowResize;
        }

        /// <summary>
        /// Undoes the row resizing action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if ((sheet != null) && (rows != null))
            {
                Excel view = sender as Excel;
                int[] rowsResized = GetRowsResized(rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, columnHeader))
                    {
                        return true;
                    }
                    int num = columnHeader ? sheet.ColumnHeader.RowCount : sheet.RowCount;
                    for (int i = 0; i < rows.Length; i++)
                    {
                        RowResizeExtent extent = rows[i];
                        for (int j = extent.FirstRow; j <= extent.LastRow; j++)
                        {
                            double height = ((double[]) oldsizes[i])[j - extent.FirstRow];
                            bool flag3 = ((bool[]) oldVisibles[i])[j - extent.FirstRow];
                            if (((0 <= j) && (j < num)) && (height != -1.0))
                            {
                                if (columnHeader && sheet.GetRowResizable(j, SheetArea.ColumnHeader))
                                {
                                    sheet.SetRowHeight(j, SheetArea.ColumnHeader, height);
                                    flag = true;
                                }
                                else if (sheet.GetRowResizable(j, SheetArea.Cells))
                                {
                                    sheet.SetRowHeight(j, SheetArea.Cells, height);
                                    flag = true;
                                }
                                if (columnHeader && (sheet.GetRowVisible(j, SheetArea.ColumnHeader) != flag3))
                                {
                                    sheet.SetRowVisible(j, SheetArea.ColumnHeader, flag3);
                                    flag = true;
                                }
                                else if (sheet.GetRowVisible(j, SheetArea.Cells) != flag3)
                                {
                                    sheet.SetRowVisible(j, SheetArea.Cells, flag3);
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
                    view.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                    view.RefreshFloatingObjects();
                    view.RaiseRowHeightChanged(rowsResized, columnHeader);
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

