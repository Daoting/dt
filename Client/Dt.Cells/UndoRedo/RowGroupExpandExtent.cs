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
    /// Represents a row range group expand action extent that supports expanding or collapsing the row range group.
    /// </summary>
    public class RowGroupExpandExtent
    {
        private bool _isCollapsed;
        private int _level;
        private int _summaryIndex;
        private int _viewportIndex;

        /// <summary>
        /// Creates a new instance of the <see cref="T:Dt.Cells.UndoRedo.RowGroupExpandExtent" /> class.
        /// </summary>
        /// <param name="summaryIndex">The row range group summary index.</param>
        /// <param name="level">The row range group level.</param>
        /// <param name="viewportIndex">The row range group viewport index.</param>
        /// <param name="isCollapsed">The group state is <c>true</c> if collapsed; otherwise, expanded.</param>
        public RowGroupExpandExtent(int summaryIndex, int level, int viewportIndex, bool isCollapsed)
        {
            this._summaryIndex = summaryIndex;
            this._level = level;
            this._viewportIndex = viewportIndex;
            this._isCollapsed = isCollapsed;
        }

        /// <summary>
        /// Gets a value that indicates whether the row range group is collapsed.
        /// </summary>
        /// <value>
        /// <c>true</c> if collapsed; otherwise, <c>false</c>.
        /// </value>
        public bool Collapsed
        {
            get { return  this._isCollapsed; }
        }

        /// <summary>
        /// Gets the row range group summary index.
        /// </summary>
        public int Index
        {
            get { return  this._summaryIndex; }
        }

        /// <summary>
        /// Gets the row range group level.
        /// </summary>
        public int Level
        {
            get { return  this._level; }
        }

        /// <summary>
        /// Gets the row range group viewport index.
        /// </summary>
        public int ViewportIndex
        {
            get { return  this._viewportIndex; }
        }
    }
}

