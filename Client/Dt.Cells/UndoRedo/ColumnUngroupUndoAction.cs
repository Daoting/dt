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
    /// Represents the undo action for ungrouping a column range group on a sheet. 
    /// </summary>
    public class ColumnUngroupUndoAction : ActionBase, IUndo
    {
        private List<ColumnUngroupExtent> _cachedUngroups;
        private ColumnUngroupExtent _columnUngroupExtent;
        private Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnUngroupUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnGroupExtent">The column ungroup extent information.</param>
        public ColumnUngroupUndoAction(Worksheet sheet, ColumnUngroupExtent columnGroupExtent)
        {
            this._sheet = sheet;
            this._columnUngroupExtent = columnGroupExtent;
            this._cachedUngroups = null;
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
            this.SaveState();
            if ((this._cachedUngroups != null) && (this._cachedUngroups.Count > 0))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    foreach (ColumnUngroupExtent extent in this._cachedUngroups)
                    {
                        this._sheet.ColumnRangeGroup.Ungroup(extent.Index, extent.Count);
                    }
                }
                finally
                {
                    base.ResumeInvalidate(sender);
                }
                SheetView view = sender as SheetView;
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
            List<ColumnUngroupExtent> list = new List<ColumnUngroupExtent>();
            if (((this._sheet != null) && (this._columnUngroupExtent != null)) && (this._sheet.ColumnRangeGroup != null))
            {
                int index = this._columnUngroupExtent.Index;
                int count = this._columnUngroupExtent.Count;
                for (int i = index; i < (index + count); i++)
                {
                    RangeGroupInfo info = this._sheet.ColumnRangeGroup.Find(i, 0);
                    if (info != null)
                    {
                        list.Add(new ColumnUngroupExtent(i, Math.Min((int) (index + count), (int) (info.End + 1)) - i));
                        i = info.End;
                    }
                }
            }
            this._cachedUngroups = list;
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
            if ((this._cachedUngroups == null) || (this._cachedUngroups.Count <= 0))
            {
                return flag;
            }
            base.SuspendInvalidate(sender);
            try
            {
                foreach (ColumnUngroupExtent extent in this._cachedUngroups)
                {
                    this._sheet.ColumnRangeGroup.Group(extent.Index, extent.Count);
                }
            }
            finally
            {
                base.ResumeInvalidate(sender);
            }
            SheetView view = sender as SheetView;
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

