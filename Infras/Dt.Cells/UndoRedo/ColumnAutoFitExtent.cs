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
    /// Represents the column automatic fit extent that supports the column automatic fit undo action.
    /// </summary>
    public class ColumnAutoFitExtent
    {
        int _column;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="column">The column index.</param>
        public ColumnAutoFitExtent(int column)
        {
            _column = column;
        }

        /// <summary>
        /// Gets the automatic fit column index.
        /// </summary>
        public int Column
        {
            get { return  _column; }
        }
    }
}

