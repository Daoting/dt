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
    /// Represents the data for the CellChanged event.
    /// </summary>
    public class CellChangedEventArgs : EventArgs
    {
        int column;
        int columnCount;
        string propertyName;
        int row;
        int rowCount;
        Dt.Cells.Data.SheetArea sheetArea;

        internal CellChangedEventArgs(string propertyName, int row, int column, Dt.Cells.Data.SheetArea sheetArea) : this(propertyName, row, column, 1, 1, sheetArea)
        {
        }

        internal CellChangedEventArgs(string propertyName, int row, int column, int rowCount, int columnCount, Dt.Cells.Data.SheetArea sheetArea)
        {
            this.propertyName = propertyName;
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.sheetArea = sheetArea;
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
        /// Gets the property name.
        /// </summary>
        public string PropertyName
        {
            get { return  this.propertyName; }
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
        /// Gets the sheet area of the cell.
        /// </summary>
        public Dt.Cells.Data.SheetArea SheetArea
        {
            get { return  this.sheetArea; }
        }
    }
}

