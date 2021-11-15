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

namespace Dt.Cells.UndoRedo
{
    /// <summary>
    /// Represents the row resize extent that supports the row resize undo action.
    /// </summary>
    public class RowResizeExtent
    {
        int firstRow;
        int lastRow;

        /// <summary>
        /// Creates a new RowResizeExtent object.
        /// </summary>
        /// <param name="firstRow">First row of the contiguous selection.</param>
        /// <param name="lastRow">Last row of the contiguous selection.</param>
        public RowResizeExtent(int firstRow, int lastRow)
        {
            this.firstRow = firstRow;
            this.lastRow = lastRow;
        }

        /// <summary>
        /// Gets the index of the first row in the contiguous selection.
        /// </summary>
        public int FirstRow
        {
            get { return  firstRow; }
        }

        /// <summary>
        /// Gets the index of the last row in the contiguous selection.
        /// </summary>
        public int LastRow
        {
            get { return  lastRow; }
        }
    }
}

