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
    /// Represents the event data for the DragDropBlock event for the GcSpreadSheet component; occurs when a range of cells is being dragged and dropped.
    /// </summary>
    public class ValidationDragDropBlockEventArgs : EventArgs
    {
        internal ValidationDragDropBlockEventArgs(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount, bool copy, bool insert)
        {
            FromRow = fromRow;
            FromColumn = fromColumn;
            ToRow = toRow;
            ToColumn = toColumn;
            RowCount = rowCount;
            ColumnCount = columnCount;
            Copy = copy;
            Insert = insert;
        }

        /// <summary>
        /// Gets the column count of the cell range being dragged.
        /// </summary>
        /// <value>Column count of the cell range being dragged.</value>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Gets whether the source range is copied.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the source range is copied</value>
        public bool Copy { get; private set; }

        /// <summary>
        /// Gets the column index of the top left cell of the source range (range being dragged).  
        /// </summary>
        /// <value>The column index of the top left cell of the source range (range being dragged)</value>
        public int FromColumn { get; private set; }

        /// <summary>
        /// Gets the row index of the top left cell of the source range (range being dragged).  
        /// </summary>
        /// <value>The row index of the top left cell of the source range (range being dragged).</value>
        public int FromRow { get; private set; }

        /// <summary>
        /// Gets or sets whether the event is handled.
        /// </summary>
        /// <value>
        /// <c>true</c> if [handle]; otherwise, <c>false</c>.
        /// </value>
        public bool Handle { get; set; }

        /// <summary>
        /// Gets whether the source range is inserted.
        /// </summary>
        /// <value>A <c>bool</c> value that indicates whether the source range is inserted.</value>
        public bool Insert { get; private set; }

        /// <summary>
        /// Gets or sets the invalid message.
        /// </summary>
        /// <value>
        /// The invalid message.
        /// </value>
        public string InvalidMessage { get; set; }

        /// <summary>
        /// Gets or sets whether the value is valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if [is valid]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInvalid { get; set; }

        /// <summary>
        /// Gets the row count of the cell range being dragged.
        /// </summary>
        /// <value>Row count of cell range being dragged.</value>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets the column index of the bottom right cell of the destination range (where selection is dropped). 
        /// </summary>
        /// <value>The column index of the bottom right cell of the destination range (where selection is dropped)</value>
        public int ToColumn { get; private set; }

        /// <summary>
        /// Gets the row index of the top left cell of the destination range (where selection is dropped).  
        /// </summary>
        /// <value>The row index of the top left cell of the destination range (where selection is dropped)</value>
        public int ToRow { get; private set; }
    }
}

