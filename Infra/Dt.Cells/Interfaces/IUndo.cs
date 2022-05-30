#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Specifies that a command or operation can be undone.
    /// </summary>
    public interface IUndo
    {
        /// <summary>
        /// Saves the state for undoing the command or operation.
        /// </summary>
        void SaveState();
        /// <summary>
        /// Undoes the command or operation.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to undo the command or operation.
        /// </param>
        /// <returns>
        /// <c>true</c> if an undo operation on the command or operation succeeds; otherwise, <c>false</c>.
        /// </returns>
        bool Undo(object parameter);

        /// <summary>
        /// Gets a value that indicates whether the command or operation can be undone.
        /// </summary>
        bool CanUndo { get; }
    }
}

