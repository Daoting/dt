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
    /// Represents the undo actions for an automatically resized row on a sheet. 
    /// </summary>
    public class RowAutoFitUndoAction : ActionBase, IUndo
    {
        bool _columnHeader;
        double[] _oldSizes;
        bool[] _oldVisibles;
        RowAutoFitExtent[] _rows;
        Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowAutoFitUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rows">The automatically resized rows.</param>
        /// <param name="columnHeader">if set to <c>true</c> the row is in the column header.</param>
        public RowAutoFitUndoAction(Worksheet sheet, RowAutoFitExtent[] rows, bool columnHeader)
        {
            _sheet = sheet;
            _rows = rows;
            _columnHeader = columnHeader;
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
            if (_sheet == null)
            {
                return true;
            }
            if (_sheet.Protect)
            {
                return false;
            }
            return (((_sheet != null) && (_rows != null)) && (_rows.Length > 0));
        }

        /// <summary>
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (CanExecute(sender))
            {
                Excel view = sender as Excel;
                int[] rowsResized = GetRowsResized(_rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, _columnHeader))
                    {
                        return;
                    }
                    SaveState();
                    int num = _columnHeader ? _sheet.ColumnHeader.RowCount : _sheet.RowCount;
                    for (int i = 0; i < _rows.Length; i++)
                    {
                        RowAutoFitExtent extent = _rows[i];
                        if ((0 <= extent.Row) && (extent.Row < num))
                        {
                            if (_columnHeader && _sheet.GetRowResizable(extent.Row, SheetArea.ColumnHeader))
                            {
                                double height = Math.Ceiling(view.GetRowAutoFitValue(extent.Row, true));
                                if (height >= 0.0)
                                {
                                    height = (height + 1.0) + 2.0;
                                    if (height < _sheet.ColumnHeader.DefaultRowHeight)
                                    {
                                        height = _sheet.ColumnHeader.DefaultRowHeight;
                                    }
                                }
                                if (height != _sheet.GetRowHeight(extent.Row, SheetArea.ColumnHeader))
                                {
                                    _sheet.SetRowHeight(extent.Row, SheetArea.ColumnHeader, height);
                                }
                                _sheet.SetRowVisible(extent.Row, SheetArea.ColumnHeader, true);
                            }
                            else if (_sheet.GetRowResizable(extent.Row, SheetArea.Cells))
                            {
                                double defaultRowHeight = Math.Ceiling(view.GetRowAutoFitValue(extent.Row, false));
                                if (defaultRowHeight >= 0.0)
                                {
                                    defaultRowHeight = (defaultRowHeight + 1.0) + 2.0;
                                    if (defaultRowHeight < _sheet.DefaultRowHeight)
                                    {
                                        defaultRowHeight = _sheet.DefaultRowHeight;
                                    }
                                }
                                if (defaultRowHeight != _sheet.GetRowHeight(extent.Row, SheetArea.Cells))
                                {
                                    _sheet.SetRowHeight(extent.Row, SheetArea.Cells, defaultRowHeight);
                                }
                                _sheet.SetRowVisible(extent.Row, SheetArea.Cells, true);
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
                    view.RaiseRowHeightChanged(rowsResized, _columnHeader);
                }
            }
        }

        int[] GetRowsResized(RowAutoFitExtent[] rowHeightChangeExtents)
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
            if (((_sheet != null) && (_rows != null)) && (_rows.Length > 0))
            {
                numArray = new double[_rows.Length];
                flagArray = new bool[_rows.Length];
                int num = _columnHeader ? _sheet.ColumnHeader.RowCount : _sheet.RowCount;
                for (int i = 0; i < _rows.Length; i++)
                {
                    RowAutoFitExtent extent = _rows[i];
                    if ((0 <= extent.Row) && (extent.Row < num))
                    {
                        if (_columnHeader)
                        {
                            numArray[i] = _sheet.GetRowHeight(extent.Row, SheetArea.ColumnHeader);
                            flagArray[i] = _sheet.GetRowVisible(extent.Row, SheetArea.ColumnHeader);
                        }
                        else
                        {
                            numArray[i] = _sheet.GetRowHeight(extent.Row, SheetArea.Cells);
                            flagArray[i] = _sheet.GetRowVisible(extent.Row, SheetArea.Cells);
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
            if (((_sheet != null) && (_rows != null)) && (_rows.Length > 0))
            {
                Excel view = sender as Excel;
                int[] rowsResized = GetRowsResized(_rows);
                base.SuspendInvalidate(sender);
                try
                {
                    if ((view != null) && view.RaiseRowHeightChanging(rowsResized, _columnHeader))
                    {
                        return true;
                    }
                    int num2 = _columnHeader ? _sheet.ColumnHeader.RowCount : _sheet.RowCount;
                    for (int i = 0; i < _rows.Length; i++)
                    {
                        RowAutoFitExtent extent = _rows[i];
                        double height = _oldSizes[i];
                        bool flag3 = _oldVisibles[i];
                        if (((0 <= i) && (i < num2)) && (height != -1.0))
                        {
                            if (_columnHeader && _sheet.GetRowResizable(extent.Row, SheetArea.ColumnHeader))
                            {
                                _sheet.SetRowHeight(extent.Row, SheetArea.ColumnHeader, height);
                                _sheet.SetRowVisible(extent.Row, SheetArea.ColumnHeader, flag3);
                                flag = true;
                            }
                            else if (_sheet.GetRowResizable(extent.Row, SheetArea.Cells))
                            {
                                _sheet.SetRowHeight(extent.Row, SheetArea.Cells, height);
                                _sheet.SetRowVisible(extent.Row, SheetArea.Cells, flag3);
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
                    view.RefreshRange(-1, -1, -1, -1, SheetArea.Cells | SheetArea.RowHeader);
                    view.RaiseRowHeightChanged(rowsResized, _columnHeader);
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

