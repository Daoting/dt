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
    /// Represents the undo action for grouping a row range group on a sheet. 
    /// </summary>
    public class RowGroupUndoAction : ActionBase, IUndo
    {
        RowGroupExtent _rowGroupExtent;
        Worksheet _sheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowGroupUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The worksheet.</param>
        /// <param name="rowGroupExtent">The row group extent information.</param>
        public RowGroupUndoAction(Worksheet sheet, RowGroupExtent rowGroupExtent)
        {
            _sheet = sheet;
            _rowGroupExtent = rowGroupExtent;
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
        /// Executes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        public override void Execute(object sender)
        {
            if (((_sheet != null) && (_rowGroupExtent != null)) && (_sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _rowGroupExtent.Index;
                    int count = _rowGroupExtent.Count;
                    _sheet.RowRangeGroup.Group(index, count);
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
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionRowGroup;
        }

        /// <summary>
        /// Undoes the action on the specified sender.
        /// </summary>
        /// <param name="sender">Object on which the action occurred.</param>
        /// <returns><c>true</c> if undo is successful; otherwise <c>false</c>.</returns>
        public bool Undo(object sender)
        {
            bool flag = false;
            if (((_sheet != null) && (_rowGroupExtent != null)) && (_sheet.RowRangeGroup != null))
            {
                base.SuspendInvalidate(sender);
                try
                {
                    int index = _rowGroupExtent.Index;
                    int count = _rowGroupExtent.Count;
                    _sheet.RowRangeGroup.Ungroup(index, count);
                    flag = true;
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

