#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the RangeGroupStateChanging event for an outline (range group) of rows and columns in the GcSpreadSheet component.
    /// </summary>
    public class RangeGroupStateChangingEventArgs : CancelEventArgs
    {
        internal RangeGroupStateChangingEventArgs(bool isRowGroup, int index, int level)
        {
            IsRowGroup = isRowGroup;
            Index = index;
            Level = level;
        }

        internal RangeGroupStateChangingEventArgs(bool isRowGroup, int index, int level, bool cancel) : this(isRowGroup, index, level)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets the index of the RangeGroupInfo object whose state is changing.
        /// </summary>
        /// <value>The index of the RangeGroupInfo object whose state is changing.</value>
        public int Index { get; private set; }

        /// <summary>
        /// Gets whether the outline (range group) is a group of rows.  
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the outline (range group) is a group of rows.</value>
        public bool IsRowGroup { get; private set; }

        /// <summary>
        /// Gets the level of the RangeGroupInfo object whose state is changing.
        /// </summary>
        /// <value>The level of the RangeGroupInfo object whose state is changing.</value>
        public int Level { get; private set; }
    }
}

