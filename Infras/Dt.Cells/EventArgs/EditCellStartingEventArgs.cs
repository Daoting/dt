#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the EditStarting event for the GcSpreadSheet component; occurs when a cell goes into edit mode.
    /// </summary>
    public class EditCellStartingEventArgs : CancelEventArgs
    {
        internal EditCellStartingEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }

        internal EditCellStartingEventArgs(int row, int column, bool cancel) : this(row, column)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets the column index of the cell.
        /// </summary>
        /// <value>The column index of the cell.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the row index of the cell.
        /// </summary>
        /// <value>The row index of the cell.</value>
        public int Row { get; private set; }
    }
}

