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
    /// Represents the event data for the LeaveCell event for the GcSpreadSheet component; occurs when focus leaves a cell. 
    /// </summary>
    public class LeaveCellEventArgs : CancelEventArgs
    {
        internal LeaveCellEventArgs(int row, int column, int toRow, int toColumn)
        {
            Row = row;
            Column = column;
            ToRow = toRow;
            ToColumn = toColumn;
        }

        /// <summary>
        /// Gets the column index of the cell being left.
        /// </summary>
        /// <value>The column index of the cell being left.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the row index of the cell being left.
        /// </summary>
        /// <value>The row index of the cell being left.</value>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the automatic column.
        /// </summary>
        /// <value>
        /// The automatic column.
        /// </value>
        public int ToColumn { get; private set; }

        /// <summary>
        /// Gets the automatic row.
        /// </summary>
        /// <value>
        /// The automatic row.
        /// </value>
        public int ToRow { get; private set; }
    }
}

