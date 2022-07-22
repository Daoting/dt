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
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents the undo action for column range grouping on a sheet. 
    /// </summary>
    public class ColumnGroupUndoAction : ActionBase, IUndo
    {
        ColumnGroupExtent _columnGroupExtent;
        Worksheet _sheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="columnGroupExtent">The column group extent information.</param>
        public ColumnGroupUndoAction(Worksheet sheet, ColumnGroupExtent columnGroupExtent)
        {
            _sheet = sheet;
            _columnGroupExtent = columnGroupExtent;
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
        /// Executes the column group action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((_sheet != null) && (_columnGroupExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _columnGroupExtent.Index;
                    int count = _columnGroupExtent.Count;
                    _sheet.ColumnRangeGroup.Group(index, count);
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
        /// Saves the state used to undo an action.
        /// </summary>
        public void SaveState()
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionColumnGroup;
        }

        /// <summary>
        /// Undoes the action.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((_sheet != null) && (_columnGroupExtent != null)) && (_sheet.ColumnRangeGroup != null))
            {
                Excel excel = sender as Excel;
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _columnGroupExtent.Index;
                    int count = _columnGroupExtent.Count;
                    _sheet.ColumnRangeGroup.Ungroup(index, count);
                    flag = true;
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

