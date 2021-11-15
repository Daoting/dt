#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Represents the event data for the RangeSorted event for the GcSpreadSheet component; occurs when columns are being automatically sorted. 
    /// </summary>
    public class RangeSortedEventArgs : EventArgs
    {
        internal RangeSortedEventArgs(int column, bool ascending)
        {
            Column = column;
            Ascending = ascending;
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

