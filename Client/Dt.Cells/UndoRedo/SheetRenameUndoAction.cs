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
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents a rename sheet name undo action on the sheet.
    /// </summary>
    public class SheetRenameUndoAction : ActionBase, IUndo
    {
        /// <summary>
        /// The new name of the renamed worksheet.
        /// </summary>
        protected string newName;
        /// <summary>
        /// The previous name of the worksheet.
        /// </summary>
        protected string oldName;
        /// <summary>
        /// The renamed worksheet.
        /// </summary>
        protected Worksheet worksheet;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.SheetRenameUndoAction" /> class.
        /// </summary>
        /// <param name="sheet">The renamed worksheet.</param>
        /// <param name="newName">The worksheet's new name.</param>
        public SheetRenameUndoAction(Worksheet sheet, string newName)
        {
            this.worksheet = sheet;
            this.newName = newName;
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
            return (((worksheet != null) && (newName != null)) && (newName != oldName));
        }

        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter">Object on which the action occurred.</param>
        public override void Execute(object parameter)
        {
            if (((worksheet == null) || (newName == null)) || !(newName != oldName))
            {
                throw new ActionFailedException(this);
            }
            SaveState();
            base.SuspendInvalidate(parameter);
            try
            {
                worksheet.Name = newName;
            }
            finally
            {
                base.ResumeInvalidate(parameter);
            }
            RefreshUI(parameter);
        }

        void RefreshUI(object sheetView)
        {
            var view = sheetView as Excel;
            if (view != null)
            {
                view.RefreshTabStrip();
            }
        }

        /// <summary>
        /// Saves the state for the undo action before executing the action.
        /// </summary>
        public void SaveState()
        {
            if (worksheet != null)
            {
                oldName = worksheet.Name;
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ResourceStrings.undoActionSheetRename;
        }

        /// <summary>
        /// Undoes the command or operation.
        /// </summary>
        /// <param name="parameter">The action to undo the action on.</param>
        /// <returns>
        /// <c>true</c> if undoing the action succeeds; otherwise, <c>false</c>.
        /// </returns>
        public bool Undo(object parameter)
        {
            if (worksheet == null)
            {
                return false;
            }
            base.SuspendInvalidate(parameter);
            try
            {
                worksheet.Name = oldName;
            }
            finally
            {
                base.ResumeInvalidate(parameter);
            }
            RefreshUI(parameter);
            return true;
        }

        /// <summary>
        /// Gets a value that indicates whether the action can be undone.
        /// </summary>
        public bool CanUndo
        {
            get { return  (oldName != null); }
        }
    }
}

