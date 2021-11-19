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
    /// Represents a column resize extent that supports the column resize undo action.
    /// </summary>
    public class ColumnResizeExtent
    {
        int firstColumn;
        int lastColumn;

        /// <summary>
        /// Creates a new ColumnResizeExtent object.
        /// </summary>
        /// <param name="firstColumn">First column of the contiguous selection.</param>
        /// <param name="lastColumn">Last column of the contiguous selection.</param>
        public ColumnResizeExtent(int firstColumn, int lastColumn)
        {
            this.firstColumn = firstColumn;
            this.lastColumn = lastColumn;
        }

        /// <summary>
        /// Gets the index of the first column in the contiguous selection.
        /// </summary>
        public int FirstColumn
        {
            get { return  firstColumn; }
        }

        /// <summary>
        /// Gets the index of the last column in the contiguous selection.
        /// </summary>
        public int LastColumn
        {
            get { return  lastColumn; }
        }
    }
}

