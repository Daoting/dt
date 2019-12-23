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
    /// Represents the event data for the RangeSorting event for the GcSpreadSheet component; occurs when columns are being automatically sorted. 
    /// </summary>
    public class RangeSortingEventArgs : CancelEventArgs
    {
        internal RangeSortingEventArgs(int column, bool ascending)
        {
            this.Column = column;
            this.Ascending = ascending;
        }

        internal RangeSortingEventArgs(int column, bool ascending, bool cancel) : this(column, ascending)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets whether the automatic sort is ascending.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the automatic sort is ascending.</value>
        public bool Ascending { get; private set; }

        /// <summary>
        /// Gets the index of the column to be automatically sorted.
        /// </summary>
        /// <value>The index of the column to be automatically sorted.</value>
        public int Column { get; private set; }
    }
}
