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
    /// Represents the event data for the RowHeightChanging event for the GcSpreadSheet component; occurs when the height of a row is changing. 
    /// </summary>
    public class RowHeightChangingEventArgs : CancelEventArgs
    {
        internal RowHeightChangingEventArgs(int[] rowList, bool header)
        {
            RowList = rowList;
            Header = header;
        }

        internal RowHeightChangingEventArgs(int[] rowList, bool header, bool cancel) : this(rowList, header)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets whether the rows indexes are column header rows.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the rows indexes are column header rows.</value>
        public bool Header { get; private set; }

        /// <summary>
        /// Gets the list of rows whose heights are changing.
        /// </summary>
        /// <value>Rows whose heights are changing.</value>
        public int[] RowList { get; private set; }
    }
}

