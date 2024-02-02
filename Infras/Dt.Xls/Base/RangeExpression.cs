#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    internal class RangeExpression
    {
        private int column;
        private int columnCount;
        private bool columnRelative;
        internal bool isEntireColumn;
        internal bool isEntrieRow;
        private int row;
        private int rowCount;
        private bool rowRelative;

        public RangeExpression(int row, int column, bool rowRelative, bool columnRelative) : this(row, column, 1, 1, rowRelative, columnRelative)
        {
        }

        /// <summary>
        /// Creates a new expression with a range reference expression with absolute coordinates.
        /// </summary>
        /// <param name="row">Starting row coordinate of the range</param>
        /// <param name="column">Starting column coordinate of the range</param>
        /// <param name="rowCount">Number of rows in the range</param>
        /// <param name="columnCount">Number of columns in the range</param>
        public RangeExpression(int row, int column, int rowCount, int columnCount) : this(row, column, rowCount, columnCount, false, false)
        {
        }

        /// <summary>
        /// Creates a new expression representing a range reference expression.
        /// </summary>
        /// <param name="row">Starting row coordinate of the range</param>
        /// <param name="column">Starting column coordinate of the range</param>
        /// <param name="rowCount">Number of rows in the range</param>
        /// <param name="columnCount">Number of columns in the range</param>
        /// <param name="rowRelative">Whether the row coordinates are relative or absolute</param>
        /// <param name="columnRelative">Whether the column coordinates are relative or absolute</param>
        public RangeExpression(int row, int column, int rowCount, int columnCount, bool rowRelative, bool columnRelative)
        {
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
            this.rowRelative = rowRelative;
            this.columnRelative = columnRelative;
        }

        /// <summary>
        /// Gets the starting column coordinate of the range.
        /// </summary>
        public int Column
        {
            get { return  this.column; }
        }

        /// <summary>
        /// Gets the number of columns in the range.
        /// </summary>
        public int ColumnCount
        {
            get { return  this.columnCount; }
        }

        /// <summary>
        /// Determines whether the column coordinates are relative.
        /// </summary>
        public bool ColumnRelative
        {
            get { return  this.columnRelative; }
        }

        /// <summary>
        /// Gets the starting row coordinate of the range.
        /// </summary>
        public int Row
        {
            get { return  this.row; }
        }

        /// <summary>
        /// Gets the number of rows in the range.
        /// </summary>
        public int RowCount
        {
            get { return  this.rowCount; }
        }

        /// <summary>
        /// Determines whether the row coordinates are relative.
        /// </summary>
        public bool RowRelative
        {
            get { return  this.rowRelative; }
        }
    }
}

