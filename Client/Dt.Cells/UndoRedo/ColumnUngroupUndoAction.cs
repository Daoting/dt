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
    /// Represents the undo action for ungrouping a column range group on a sheet. 
    /// </summary>
    public class ColumnUngroupUndoAction : ActionBase, IUndo
    {
        List<ColumnUngroupExtent> _cachedUngroups;
        ColumnUngroupExtent _columnUngroupExtent;
        Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnUngroupUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnGroupExtent">The column ungroup extent information.</param>
        public ColumnUngroupUndoAction(Worksheet sheet, ColumnUngroupExtent columnGroupExtent)
        {
            _sheet = sheet;
            _columnUngroupExtent = columnGroupExtent;
            _cachedUngroups = null;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Object on which the action occurred.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the ungroup action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            SaveState();
            if ((_cachedUngroups != null) && (_cachedUngroups.Count > 0))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    foreach (ColumnUngroupExtent extent in _cachedUngroups)
                    {
                        _sheet.ColumnRangeGroup.Ungroup(extent.Index, extent.Count);
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                Excel excel = sender as Excel;
                if (excel != null)
                {
                    excel.InvalidateLayout();
                    excel.InvalidateViewportHorizontalArrangement(-2);
                    excel.InvalidateHeaderHorizontalArrangement();
                    excel.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            List<ColumnUngroupExtent> list = new List<ColumnUngroupExtent>();
            if (((_sheet != null) && (_columnUngroupExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                int index = _columnUngroupExtent.Index;
                int count = _columnUngroupExtent.Count;
                for (int i = index; i < (index + count); i++)
                {
                    RangeGroupInfo info = _sheet.ColumnRangeGroup.Find(i, 0);
                    if (info != null)
                    {
                        list.Add(new ColumnUngroupExtent(i, Math.Min((int) (index + count), (int) (info.End + 1)) - i));
                        i = info.End;
                    }
                }
            }
            _cachedUngroups = list;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionColumnUngroup;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if ((_cachedUngroups == null) || (_cachedUngroups.Count <= 0))
            {
                return flag;
            }
            base.SuspendInvalidate(sender);
            try
            {
                foreach (ColumnUngroupExtent extent in _cachedUngroups)
                {
                    _sheet.ColumnRangeGroup.Group(extent.Index, extent.Count);
                }
            }
            finally
            {
                base.ResumeInvalidate(sender);
            }
            Excel excel = sender as Excel;
            if (excel != null)
            {
                excel.InvalidateLayout();
                excel.InvalidateViewportHorizontalArrangement(-2);
                excel.InvalidateHeaderHorizontalArrangement();
                excel.InvalidateMeasure();
            }
            return true;
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

