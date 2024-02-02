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
    /// Represents the event data for the RowHeightChanged event for the GcSpreadSheet component; occurs when the height of a row has changed. 
    /// </summary>
    public class RowHeightChangedEventArgs : EventArgs
    {
        internal RowHeightChangedEventArgs(int[] rowList, bool header)
        {
            RowList = rowList;
            Header = header;
        }

        /// <summary>
        /// Gets whether the rows indexes are column header rows.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the rows indexes are column header rows.</value>
        public bool Header { get; private set; }

        /// <summary>
        /// Gets the list of rows whose heights have changed.
        /// </summary>
        /// <value>Rows whose heights have changed.</value>
        public int[] RowList { get; private set; }
    }
}

