#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// epresents the event data when user execute a undo/redo action.
    /// </summary>
    public class UndoRedoEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:UndoRedoEventArgs" /> class.
        /// </summary>
        /// <param name="undoRedo">The undo redo.</param>
        /// <param name="action">The action.</param>
        public UndoRedoEventArgs(UndoRedoOperation undoRedo, string action)
        {
            UndoRedo = undoRedo;
            Action = action;
        }

        /// <summary>
        /// Gets the current action string.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public string Action { get; private set; }

        /// <summary>
        /// Gets a value indicating whether it is a undo or redo action.
        /// </summary>
        /// <value>
        /// The undo redo.
        /// </value>
        public UndoRedoOperation UndoRedo { get; private set; }
    }
}

