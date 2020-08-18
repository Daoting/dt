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
    /// Represents the undo action for ungrouping row range groups on a sheet. 
    /// </summary>
    public class RowUngroupUndoAction : ActionBase, IUndo
    {
        List<RowUngroupExtent> _cachedUngroups;
        RowUngroupExtent _rowUngroupExtent;
        Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowUngroupUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="rowUngroupExtent">The row ungroup extent information.</param>
        public RowUngroupUndoAction(Worksheet sheet, RowUngroupExtent rowUngroupExtent)
        {
            _sheet = sheet;
            _rowUngroupExtent = rowUngroupExtent;
            _cachedUngroups = null;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the action on the specified sender.
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
                    foreach (RowUngroupExtent extent in _cachedUngroups)
                    {
                        _sheet.RowRangeGroup.Ungroup(extent.Index, extent.Count);
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                Excel view = sender as Excel;
                if (view != null)
                {
                    view.InvalidateLayout();
                    view.InvalidateViewportHorizontalArrangement(-2);
                    view.InvalidateHeaderHorizontalArrangement();
                    view.InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Saves the state for undoing the action.
        /// </summary>
        public void SaveState()
        {
            List<RowUngroupExtent> list = new List<RowUngroupExtent>();
            if (((_sheet != null) && (_rowUngroupExtent != null)) && (_sheet.RowRangeGroup != null))
            {
                int index = _rowUngroupExtent.Index;
                int count = _rowUngroupExtent.Count;
                for (int i = index; i < (index + count); i++)
                {
                    RangeGroupInfo info = _sheet.RowRangeGroup.Find(i, 0);
                    if (info != null)
                    {
                        list.Add(new RowUngroupExtent(i, Math.Min((int) (index + count), (int) (info.End + 1)) - i));
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
            return ResourceStrings.undoActionRowUngroup;
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
            base.ResumeInvalidate(sender);
            try
            {
                foreach (RowUngroupExtent extent in _cachedUngroups)
                {
                    _sheet.RowRangeGroup.Group(extent.Index, extent.Count);
                }
            }
            finally
            {
                base.ResumeInvalidate(sender);
            }
            Excel view = sender as Excel;
            if (view != null)
            {
                view.InvalidateLayout();
                view.InvalidateViewportHorizontalArrangement(-2);
                view.InvalidateHeaderHorizontalArrangement();
                view.InvalidateMeasure();
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

