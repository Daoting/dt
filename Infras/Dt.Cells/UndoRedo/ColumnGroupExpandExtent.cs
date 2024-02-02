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
    /// Represents a column range group expand action extent that supports expanding or collapsing a column range group.
    /// </summary>
    public class ColumnGroupExpandExtent
    {
        bool _isCollapsed;
        int _level;
        int _summaryIndex;
        int _viewportIndex;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.ColumnGroupExpandExtent" /> class.
        /// </summary>
        /// <param name="summaryIndex">The column range group summary index.</param>
        /// <param name="level">The column range group level.</param>
        /// <param name="viewportIndex">The column group viewport index.</param>
        /// <param name="isCollapsed">The group state, if set to <c>true</c> is collapsed; otherwise, expanded.</param>
        public ColumnGroupExpandExtent(int summaryIndex, int level, int viewportIndex, bool isCollapsed)
        {
            _summaryIndex = summaryIndex;
            _level = level;
            _viewportIndex = viewportIndex;
            _isCollapsed = isCollapsed;
        }

        /// <summary>
        /// Gets a value that indicates whether the column range group is collapsed.
        /// </summary>
        /// <value>
        /// <c>true</c> if collapsed; otherwise, <c>false</c>.
        /// </value>
        public bool Collapsed
        {
            get { return  _isCollapsed; }
        }

        /// <summary>
        /// Gets the column range group summary index.
        /// </summary>
        public int Index
        {
            get { return  _summaryIndex; }
        }

        /// <summary>
        /// Gets the column range group level.
        /// </summary>
        public int Level
        {
            get { return  _level; }
        }

        /// <summary>
        /// Gets the index of the column range group viewport.
        /// </summary>
        public int ViewportIndex
        {
            get { return  _viewportIndex; }
        }
    }
}

