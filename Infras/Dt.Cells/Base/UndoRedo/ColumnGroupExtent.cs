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
    /// Represents a column range group extent that supports a column range group undo action.
    /// </summary>
    public class ColumnGroupExtent
    {
        int _count;
        int _index;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupExtent" /> class.
        /// </summary>
        /// <param name="index">The group starting index.</param>
        /// <param name="count">The number of columns to group.</param>
        public ColumnGroupExtent(int index, int count)
        {
            _index = index;
            _count = count;
        }

        /// <summary>
        /// Gets the number of columns to group.
        /// </summary>
        public int Count
        {
            get { return  _count; }
        }

        /// <summary>
        /// Gets the group starting index.
        /// </summary>
        public int Index
        {
            get { return  _index; }
        }
    }
}

