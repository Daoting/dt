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
    /// Represents the undo actions for an automatically resized row on a sheet. 
    /// </summary>
    public class RowAutoFitUndoAction : ActionBase, IUndo
    {
        private bool _columnHeader;
        private double[] _oldSizes;
        private bool[] _oldVisibles;
        private RowAutoFitExtent[] _rows;
        private Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowAutoFitUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rows">The automatically resized rows.</param>
        /// <param name="columnHeader">if set to <c>true</c> the row is in the column header.</param>
        public RowAutoFitUndoAction(Worksheet sheet, RowAutoFitExtent[] rows, bool columnHeader)
        {
            this._sheet = sheet;
            this._rows = rows;
            this._columnHeader = columnHeader;
        }

        /// <summary>
        /// Determines whether this row automatic resize action can execute on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns>
        /// <c>true</c> if this instance can execute on the specified sender; otherwise, <c>false</c>.
        /// </returns>
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
            return (((this._sheet != null) && (this._rows != null)) && (this._rows.Length > 0));
        }

        /// <summary>
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (this.CanExecute(sender))
            {
                SheetView view = sender as SheetView;
                int[] rowsResized = this.GetRowsResized(this._rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, this._columnHeader))
                    {
                        return;
                    }
                    this.SaveState();
                    int num = this._columnHeader ? this._sheet.ColumnHeader.RowCount : this._sheet.RowCount;
                    for (int i = 0; i < this._rows.Length; i++)
                    {
                        RowAutoFitExtent extent = this._rows[i];
                        if ((0 <= extent.Row) && (extent.Row < num))
                        {
                            if (this._columnHeader && this._sheet.GetRowResizable(extent.Row, SheetArea.ColumnHeader))
                            {
                                double height = Math.Ceiling(view.GetRowAutoFitValue(extent.Row, true));
                                if (height >= 0.0)
                                {
                                    height = (height + 1.0) + 2.0;
                                    if (height < this._sheet.ColumnHeader.DefaultRowHeight)
                                    {
                                        height = this._sheet.ColumnHeader.DefaultRowHeight;
                                    }
                                }
                                if (height != this._sheet.GetRowHeight(extent.Row, SheetArea.ColumnHeader))
                                {
                                    this._sheet.SetRowHeight(extent.Row, SheetArea.ColumnHeader, height);
                                }
                                this._sheet.SetRowVisible(extent.Row, SheetArea.ColumnHeader, true);
                            }
                            else if (this._sheet.GetRowResizable(extent.Row, SheetArea.Cells))
                            {
                                double defaultRowHeight = Math.Ceiling(view.GetRowAutoFitValue(extent.Row, false));
                                if (defaultRowHeight >= 0.0)
                                {
                                    defaultRowHeight = (defaultRowHeight + 1.0) + 2.0;
                                    if (defaultRowHeight < this._sheet.DefaultRowHeight)
                                    {
                                        defaultRowHeight = this._sheet.DefaultRowHeight;
                                    }
                                }
                                if (defaultRowHeight != this._sheet.GetRowHeight(extent.Row, SheetArea.Cells))
                                {
                                    this._sheet.SetRowHeight(extent.Row, SheetArea.Cells, defaultRowHeight);
                                }
                                this._sheet.SetRowVisible(extent.Row, SheetArea.Cells, true);
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
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                    view.InvalidateFloatingObjects();
                    view.RaiseRowHeightChanged(rowsResized, this._columnHeader);
                }
            }
        }

        private int[] GetRowsResized(RowAutoFitExtent[] rowHeightChangeExtents)
        {
            List<int> list = new List<int>();
            foreach (RowAutoFitExtent extent in rowHeightChangeExtents)
            {
                list.Add(extent.Row);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Saves the undo information.
        /// </summary>
        public void SaveState()
        {
            double[] numArray = null;
            bool[] flagArray = null;
            if (((this._sheet != null) && (this._rows != null)) && (this._rows.Length > 0))
            {
                numArray = new double[this._rows.Length];
                flagArray = new bool[this._rows.Length];
                int num = this._columnHeader ? this._sheet.ColumnHeader.RowCount : this._sheet.RowCount;
                for (int i = 0; i < this._rows.Length; i++)
                {
                    RowAutoFitExtent extent = this._rows[i];
                    if ((0 <= extent.Row) && (extent.Row < num))
                    {
                        if (this._columnHeader)
                        {
                            numArray[i] = this._sheet.GetRowHeight(extent.Row, SheetArea.ColumnHeader);
                            flagArray[i] = this._sheet.GetRowVisible(extent.Row, SheetArea.ColumnHeader);
                        }
                        else
                        {
                            numArray[i] = this._sheet.GetRowHeight(extent.Row, SheetArea.Cells);
                            flagArray[i] = this._sheet.GetRowVisible(extent.Row, SheetArea.Cells);
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
            return ResourceStrings.undoActionRowAutoFit;
        }

        /// <summary>
        /// Undoes the row automatic fit action.
        /// </summary>
        /// <param name="sender">Object on which the action occurs.</param>
        /// <returns><c>true</c> if the undo is a success; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((this._sheet != null) && (this._rows != null)) && (this._rows.Length > 0))
            {
                SheetView view = sender as SheetView;
                int[] rowsResized = this.GetRowsResized(this._rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, this._columnHeader))
                    {
                        return true;
                    }
                    int num2 = this._columnHeader ? this._sheet.ColumnHeader.RowCount : this._sheet.RowCount;
                    for (int i = 0; i < this._rows.Length; i++)
                    {
                        RowAutoFitExtent extent = this._rows[i];
                        double height = this._oldSizes[i];
                        bool flag3 = this._oldVisibles[i];
                        if (((0 <= i) && (i < num2)) && (height != -1.0))
                        {
                            if (this._columnHeader && this._sheet.GetRowResizable(extent.Row, SheetArea.ColumnHeader))
                            {
                                this._sheet.SetRowHeight(extent.Row, SheetArea.ColumnHeader, height);
                                this._sheet.SetRowVisible(extent.Row, SheetArea.ColumnHeader, flag3);
                                flag = true;
                            }
                            else if (this._sheet.GetRowResizable(extent.Row, SheetArea.Cells))
                            {
                                this._sheet.SetRowHeight(extent.Row, SheetArea.Cells, height);
                                this._sheet.SetRowVisible(extent.Row, SheetArea.Cells, flag3);
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
                    view.InvalidateRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                    view.RaiseRowHeightChanged(rowsResized, this._columnHeader);
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

