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
    /// Represents a row automatic fit extent that supports the row automatic fit undo action.
    /// </summary>
    public class RowAutoFitExtent
    {
        int _row;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowAutoFitExtent" /> class.
        /// </summary>
        /// <param name="row">The row.</param>
        public RowAutoFitExtent(int row)
        {
            _row = row;
        }

        /// <summary>
        /// Gets the index of the automatically resized row.
        /// </summary>
        public int Row
        {
            get { return  _row; }
        }
    }
}

