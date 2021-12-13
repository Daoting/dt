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
    /// Represents a row range group expand action extent that supports expanding or collapsing row range groups on the same level.
    /// </summary>
    public class RowGroupHeaderExpandExtent
    {
        int _level;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowGroupHeaderExpandExtent" /> class.
        /// </summary>
        /// <param name="level">The row range groups' level.</param>
        public RowGroupHeaderExpandExtent(int level)
        {
            _level = level;
        }

        /// <summary>
        /// Gets the row range group level.
        /// </summary>
        public int Level
        {
            get { return  _level; }
        }
    }
}

