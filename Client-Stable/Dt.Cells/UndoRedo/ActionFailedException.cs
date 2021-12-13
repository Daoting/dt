#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents an exception when the ActionBase.Execute() method does not return a success or failure value; 
    /// the Execute() method throws an ActionFailedException if the method fails 
    /// during implementation.
    /// </summary>
    public class ActionFailedException : Exception
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="action">The <see cref="T:GrapeCity.Windows.SpreadSheet.UI.UndoRedo.ActionBase" />.</param>
        public ActionFailedException(ActionBase action) : base(ResourceStrings.undoActionActionFailed)
        {
            Action = action;
        }

        /// <summary>
        /// Gets the failed action.
        /// </summary>
        public ActionBase Action { get; private set; }
    }
}

