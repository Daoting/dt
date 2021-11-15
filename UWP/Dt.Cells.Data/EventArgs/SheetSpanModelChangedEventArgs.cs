#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents span model event data.
    /// </summary>
    public class SheetSpanModelChangedEventArgs : EventArgs
    {
        int column;
        int columnCount;
        SheetSpanModelChangedEventAction eventType;
        int row;
        int rowCount;

        /// <summary>
        /// Creates a new event arguments object for the sheet,
        /// specifying the row and column.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        /// <param name="eventType">The type of the event</param>
        internal SheetSpanModelChangedEventArgs(int row, int column, SheetSpanModelChangedEventAction eventType) : this(row, column, 1, 1, eventType)
        {
        }

        /// <summary>
        /// Creates a new event arguments object for the sheet,
        /// specifying the row, column, and number of rows and columns.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="column">The column index</param>
        /// <param name="rowCount">Number of rows</param>
        /// <param name="columnCount">Number of columns</param>
        /// <param name="eventType">The type of the event</param>
        internal SheetSpanModelChangedEventArgs(int row, int column, int rowCount, int columnCount, SheetSpanModelChangedEventAction eventType)
        {
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.eventType = eventType;
        }

        /// <summary>
        /// Gets the column index.
        /// </summary>
        /// <value>The column index.</value>
        public int Column
        {
            get { return  this.column; }
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public int ColumnCount
        {
            get { return  this.columnCount; }
        }

        /// <summary>
        /// Gets the row index.
        /// </summary>
        /// <value>The row index.</value>
        public int Row
        {
            get { return  this.row; }
        }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>The number of rows.</value>
        public int RowCount
        {
            get { return  this.rowCount; }
        }

        /// <summary>
        /// Gets the type of change event.
        /// </summary>
        public SheetSpanModelChangedEventAction Type
        {
            get { return  this.eventType; }
        }
    }
}

