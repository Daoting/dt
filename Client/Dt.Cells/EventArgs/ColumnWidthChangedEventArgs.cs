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
    /// Represents the event data for the ColumnWidthChanged event for the GcSpreadSheet component; occurs when the width of a column has changed. 
    /// </summary>
    public class ColumnWidthChangedEventArgs : EventArgs
    {
        internal ColumnWidthChangedEventArgs(int[] columnList, bool header)
        {
            ColumnList = columnList;
            Header = header;
        }

        /// <summary>
        /// Gets the list of columns whose widths have changed.
        /// </summary>
        /// <value>Columns whose widths have changed.</value>
        public int[] ColumnList { get; private set; }

        /// <summary>
        /// Gets whether the column indexes are row header columns.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the column indexes are row header columns.</value>
        public bool Header { get; private set; }
    }
}

