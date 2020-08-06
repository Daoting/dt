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
    /// Represents the event data for the Changed event of the worksheet selection. 
    /// </summary>
    public sealed class SheetSelectionChangedEventArgs : EventArgs
    {
        int column;
        int columnCount;
        int row;
        int rowCount;

        /// <summary>
        /// Creates a new Changed event arguments object for the selection model for the sheet.
        /// </summary>
        /// <param name="row">The row index of the start of the selection.</param>
        /// <param name="column">The column index of the start of the selection.</param>
        /// <param name="rowCount">The number of rows in the selection.</param>
        /// <param name="columnCount">The number of columns in the selection.</param>
        internal SheetSelectionChangedEventArgs(int row, int column, int rowCount, int columnCount)
        {
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
        }

        /// <summary>
        /// Returns a string that represents the current 
        /// SheetSelectionModelEventArgs object. 
        /// </summary>
        public override string ToString()
        {
            return string.Concat((string[]) new string[] { base.GetType().FullName, "(row=", ((int) this.row).ToString(), ",column=", ((int) this.column).ToString(), ",rowCount=", ((int) this.rowCount).ToString(), ",columnCount=", ((int) this.columnCount).ToString(), ")" });
        }

        /// <summary>
        /// Gets the column index of the selection.
        /// </summary>
        public int Column
        {
            get { return  this.column; }
        }

        /// <summary>
        /// Gets the number of columns in the selection. 
        /// </summary>
        public int ColumnCount
        {
            get { return  this.columnCount; }
        }

        /// <summary>
        /// Gets the row index of the selection.
        /// </summary>
        public int Row
        {
            get { return  this.row; }
        }

        /// <summary>
        /// Gets the number of rows in the selection. 
        /// </summary>
        public int RowCount
        {
            get { return  this.rowCount; }
        }
    }
}

