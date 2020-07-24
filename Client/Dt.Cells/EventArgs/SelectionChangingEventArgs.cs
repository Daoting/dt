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

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the SelectionChanging event for the GcSpreadSheet component; occurs when the user is selecting another range of cells.
    /// </summary>
    public class SelectionChangingEventArgs : EventArgs
    {
        internal SelectionChangingEventArgs(CellRange[] oldSelections, CellRange[] newSelections)
        {
            OldSelections = oldSelections;
            NewSelections = newSelections;
        }

        /// <summary>
        /// Gets the new selection ranges. 
        /// </summary>
        /// <value>The new selection ranges.</value>
        public CellRange[] NewSelections { get; private set; }

        /// <summary>
        /// Gets the old selection ranges.
        /// </summary>
        /// <value>The old selection ranges.</value>
        public CellRange[] OldSelections { get; private set; }
    }
}

