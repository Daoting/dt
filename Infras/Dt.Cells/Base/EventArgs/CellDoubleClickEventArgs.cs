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
    /// Represents the event data for the CellDoubleClick events for the GcSpreadSheet component; occurs when the user double-clicks the mouse button with the pointer on a cell. 
    /// </summary>
    public class CellDoubleClickEventArgs : EventArgs
    {
        internal CellDoubleClickEventArgs(Dt.Cells.Data.SheetArea sheetArea, int row, int column)
        {
            SheetArea = sheetArea;
            Row = row;
            Column = column;
        }

        /// <summary>
        /// Gets the column index of the clicked cell.
        /// </summary>
        /// <value>The column index of the clicked cell.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the row index of the clicked cell.
        /// </summary>
        /// <value>The row index of the clicked cell.</value>
        public int Row { get; private set; }

        /// <summary>
        /// Gets the area the clicked cell is in.
        /// </summary>
        /// <value>The area the clicked cell is in.</value>
        public Dt.Cells.Data.SheetArea SheetArea { get; private set; }
    }
}

