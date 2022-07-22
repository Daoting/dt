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
    /// Represents the event data for the EditChange and EditEnd events for the GcSpreadSheet component.
    /// </summary>
    public class EditCellEventArgs : EventArgs
    {
        internal EditCellEventArgs(int row, int column)
        {
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Gets the cell column index.
        /// </summary>
        /// <value>The column index of the cell.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the cell row index.
        /// </summary>
        /// <value>The cell row index.</value>
        public int Row { get; private set; }
    }
}

