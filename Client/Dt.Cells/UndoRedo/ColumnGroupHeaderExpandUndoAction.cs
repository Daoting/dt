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
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a column range group action used to expand or collapse column range groups on the same level.
    /// </summary>
    public class ColumnGroupHeaderExpandUndoAction : ActionBase, IUndo
    {
        ColumnGroupHeaderExpandExtent _columnGroupHeaderExpandExtent;
        Dictionary<int, bool> _oldStatus;
        Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupHeaderExpandUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnGroupHeaderExpandExtent">The column group header expand extent information.</param>
        public ColumnGroupHeaderExpandUndoAction(Worksheet sheet, ColumnGroupHeaderExpandExtent columnGroupHeaderExpandExtent)
        {
            _sheet = sheet;
            _columnGroupHeaderExpandExtent = columnGroupHeaderExpandExtent;
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
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((_sheet != null) && (_columnGroupHeaderExpandExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    SaveState();
                    int level = _columnGroupHeaderExpandExtent.Level;
                    for (int i = 0; i < level; i++)
                    {
                        _sheet.ColumnRangeGroup.Expand(i, true);
                    }
                    _sheet.ColumnRangeGroup.Expand(level, false);
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
            Dictionary<int, bool> dictionary = null;
            if (((_sheet != null) && (_columnGroupHeaderExpandExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                int level = _columnGroupHeaderExpandExtent.Level;
                dictionary = new Dictionary<int, bool>();
                for (int i = 0; i <= level; i++)
                {
                    int index = 0;
                    int columnCount = _sheet.ColumnCount;
                    RangeGroupDirection direction = _sheet.ColumnRangeGroup.Direction;
                    while (index < columnCount)
                    {
                        RangeGroupInfo info = _sheet.ColumnRangeGroup.Find(index, i);
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
            _oldStatus = dictionary;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionColumnGroupHeaderExpand;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise, <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((_sheet != null) && (_oldStatus != null)) && (_sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    foreach (int num in _oldStatus.Keys)
                    {
                        _sheet.ColumnRangeGroup.Data.SetCollapsed(num, _oldStatus[num]);
                        flag = true;
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

