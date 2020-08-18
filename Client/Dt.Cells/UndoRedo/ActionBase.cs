#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using System;
using System.Windows.Input;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents the actions in the component for which the user can perform an undo or redo.
    /// </summary>
    public abstract class ActionBase : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether the action should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        protected ActionBase()
        {
        }

        /// <summary>
        /// Defines the method that determines whether the action can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// Data used by the action. If the action does not require data to be passed, this object can be set to null.
        /// </param>
        /// <returns>
        /// <c>true</c> if this action can be executed; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanExecute(object parameter);
        /// <summary>
        /// Defines the method to be called when the action is invoked.
        /// </summary>
        /// <param name="parameter"> 
        /// Data used by the action. If the action does not require data to be passed, this object can be set to null.
        /// </param>
        public abstract void Execute(object parameter);
        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        protected void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// Resumes all Invalidate methods if the sender is Excel.
        /// </summary>
        /// <param name="sender"></param>
        protected void ResumeInvalidate(object sender)
        {
            if (sender is Excel excel)
            {
                excel.ResumeInvalidate();
            }
        }

        /// <summary>
        /// Suspends all Invalidate methods if the sender is Excel.
        /// </summary>
        /// <param name="sender"></param>
        protected void SuspendInvalidate(object sender)
        {
            if (sender is Excel excel)
            {
                excel.SuspendInvalidate();
            }
        }
    }
}

