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
    /// Represents a row range group action used to expand or collapse row range groups on the same level.
    /// </summary>
    public class RowGroupHeaderExpandUndoAction : ActionBase, IUndo
    {
        private Dictionary<int, bool> _oldStatus;
        private RowGroupHeaderExpandExtent _rowGroupHeaderExpandExtent;
        private Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowGroupHeaderExpandUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="rowGroupHeaderExpandExtent">The row group header expand extent information.</param>
        public RowGroupHeaderExpandUndoAction(Worksheet sheet, RowGroupHeaderExpandExtent rowGroupHeaderExpandExtent)
        {
            this._sheet = sheet;
            this._rowGroupHeaderExpandExtent = rowGroupHeaderExpandExtent;
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">Data used by the action. If the action does not require data to be passed in, this object can be set to null.</param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((this._sheet != null) && (this._rowGroupHeaderExpandExtent != null)) && (this._sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    this.SaveState();
                    int level = this._rowGroupHeaderExpandExtent.Level;
                    for (int i = 0; i < level; i++)
                    {
                        this._sheet.RowRangeGroup.Expand(i, true);
                    }
                    this._sheet.RowRangeGroup.Expand(level, false);
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
            Dictionary<int, bool> dictionary = null;
            if (((this._sheet != null) && (this._rowGroupHeaderExpandExtent != null)) && (this._sheet.RowRangeGroup != null))
            {
                int level = this._rowGroupHeaderExpandExtent.Level;
                dictionary = new Dictionary<int, bool>();
                for (int i = 0; i <= level; i++)
                {
                    int index = 0;
                    int rowCount = this._sheet.RowCount;
                    RangeGroupDirection direction = this._sheet.RowRangeGroup.Direction;
                    while (index < rowCount)
                    {
                        RangeGroupInfo info = this._sheet.RowRangeGroup.Find(index, i);
                        if (info != null)
                        {
                            int num5 = -1;
                            switch (direction)
                            {
                                case RangeGroupDirection.Backward:
                                    num5 = info.Start - 1;
                                    break;

                                case RangeGroupDirection.Forward:
                                    num5 = info.End + 1;
                                    break;
                            }
                            bool flag = info.State == GroupState.Collapsed;
                            if (!dictionary.ContainsKey(num5))
                            {
                                dictionary.Add(num5, flag);
                            }
                            index += (info.End - info.Start) + 1;
                        }
                        index++;
                    }
                }
            }
            this._oldStatus = dictionary;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionRowGroupHeaderExpand;
        }

        /// <summary>
        /// Undoes the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((this._sheet != null) && (this._oldStatus != null)) && (this._sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    foreach (int num in this._oldStatus.Keys)
                    {
                        this._sheet.RowRangeGroup.Data.SetCollapsed(num, this._oldStatus[num]);
                        flag = true;
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

