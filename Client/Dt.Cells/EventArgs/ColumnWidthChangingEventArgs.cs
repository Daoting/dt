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
    /// Represents the event data for the ColumnWidthChanging event for the GcSpreadSheet component; occurs when the width of a column is changing. 
    /// </summary>
    public class ColumnWidthChangingEventArgs : CancelEventArgs
    {
        internal ColumnWidthChangingEventArgs(int[] columnList, bool header)
        {
            ColumnList = columnList;
            Header = header;
        }

        internal ColumnWidthChangingEventArgs(int[] columnList, bool header, bool cancel) : this(columnList, header)
        {
            base.Cancel = cancel;
        }

        /// <summary>
        /// Gets the list of columns whose widths are changing.
        /// </summary>
        /// <value>Columns whose widths are changing.</value>
        public int[] ColumnList { get; private set; }

        /// <summary>
        /// Gets whether the column indexes are row header columns.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the column indexes are row header columns.</value>
        public bool Header { get; private set; }
    }
}

