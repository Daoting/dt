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
    /// Represents the event data for the RangeFiltered event for the GcSpreadSheet component; occurs when the column has been automatically filtered. 
    /// </summary>
    public class RangeFilteredEventArgs : EventArgs
    {
        internal RangeFilteredEventArgs(int column, object[] filterValues)
        {
            Column = column;
            FilterValues = filterValues;
        }

        /// <summary>
        /// Gets the index of the column that was automatically filtered.  
        /// </summary>
        /// <value>The index of the column that was automatically filtered.</value>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the values to filter the column by. 
        /// </summary>
        /// <value>The values to filter the column by.</value>
        public object[] FilterValues { get; private set; }
    }
}

