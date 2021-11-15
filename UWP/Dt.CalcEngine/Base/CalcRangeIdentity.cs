#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents a range of cells for a sheet.
    /// </summary>
    public class CalcRangeIdentity : CalcLocalIdentity
    {
        internal int _columnCount;
        internal int _columnIndex;
        internal bool _isFullColumn;
        internal bool _isFullRow;
        internal int _rowCount;
        internal int _rowIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        public CalcRangeIdentity()
        {
            this.RowIndex = -1;
            this.ColumnIndex = -1;
            this.RowCount = -1;
            this.ColumnCount = -1;
            this._isFullRow = true;
            this._isFullColumn = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="columnOffset">The column offset.</param>
        public CalcRangeIdentity(CalcRangeIdentity range, int rowOffset, int columnOffset)
        {
            if (range.IsFullColumn)
            {
                this.RowIndex = -1;
                this.RowCount = -1;
            }
            else
            {
                this.RowIndex = range.RowIndex + rowOffset;
                this.RowCount = range.RowCount;
            }
            if (range.IsFullRow)
            {
                this.ColumnIndex = -1;
                this.ColumnCount = -1;
            }
            else
            {
                this.ColumnIndex = range.ColumnIndex + columnOffset;
                this.ColumnCount = range.ColumnCount;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        /// <param name="bandIndex">Row or column index of first row or column in the range</param>
        /// <param name="bandCount">Number of rows or Columns in the range</param>
        /// <param name="isRowBand"><see langword="true" /> if current range is a full row range, otherwise, 
        /// current range is a full column range.</param>
        public CalcRangeIdentity(int bandIndex, int bandCount, bool isRowBand)
        {
            if (isRowBand)
            {
                this.RowIndex = bandIndex;
                this.ColumnIndex = -1;
                this.RowCount = bandCount;
                this.ColumnCount = -1;
                this._isFullRow = true;
            }
            else
            {
                this.RowIndex = -1;
                this.ColumnIndex = bandIndex;
                this.RowCount = -1;
                this.ColumnCount = bandCount;
                this._isFullColumn = true;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        /// <param name="rowIndex">Row index of first cell in the range</param>
        /// <param name="columnIndex">Column index of first cell in the range</param>
        /// <param name="rowCount">Number of rows in the range</param>
        /// <param name="columnCount">Number of columns in the range</param>
        public CalcRangeIdentity(int rowIndex, int columnIndex, int rowCount, int columnCount)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
            this.RowCount = rowCount;
            this.ColumnCount = columnCount;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />.
        /// </summary>
        /// <param name="other">
        /// The <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> to compare with the
        /// current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />. 
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> 
        /// is equal to the current <see cref="T:Dt.CalcEngine.CalcCellIdentity" />; 
        /// otherwise, <see langword="false" />.
        /// </returns>
        protected override bool CompareTo(CalcIdentity other)
        {
            CalcRangeIdentity objA = other as CalcRangeIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return (((((this._isFullRow == objA._isFullRow) && (this._isFullColumn == objA._isFullColumn)) && ((this._rowIndex == objA._rowIndex) && (this._columnIndex == objA._columnIndex))) && (this._rowCount == objA._rowCount)) && (this._columnCount == objA._columnCount));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            return (((this._rowIndex ^ ((this._columnIndex << 13) | (this._columnIndex >> 0x13))) ^ ((this._rowCount << 0x1a) | (this._rowCount >> 6))) ^ ((this._columnCount << 7) | (this._columnCount >> 0x19)));
        }

        /// <summary>
        /// Converts to external identity.
        /// </summary>
        /// <param name="source">
        /// The source. 
        /// </param>
        /// <returns>The external identity</returns>
        public override CalcExternalIdentity ConvertToExternal(ICalcSource source)
        {
            switch (this.RangeType)
            {
                case CalcRangeType.Cell:
                    return new CalcExternalRangeIdentity(source, this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount);

                case CalcRangeType.Row:
                    return new CalcExternalRangeIdentity(source, this.RowIndex, this.RowCount, true);

                case CalcRangeType.Column:
                    return new CalcExternalRangeIdentity(source, this.ColumnIndex, this.ColumnCount, false);

                case CalcRangeType.Sheet:
                    return new CalcExternalRangeIdentity(source);
            }
            return new CalcExternalRangeIdentity(source, this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return CalcIdentity.GetCoord(this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount, this.RangeType);
        }

        /// <summary>
        /// Gets the number of columns in the range. 
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the number of columns in the range. 
        /// </value>
        public int ColumnCount
        {
            get
            {
                return this._columnCount;
            }
            private set
            {
                this._columnCount = value;
            }
        }

        /// <summary>
        /// Gets the column index of the first cell in the range.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the column index of the first cell in the range.
        /// </value>
        public int ColumnIndex
        {
            get
            {
                return this._columnIndex;
            }
            private set
            {
                this._columnIndex = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full column.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is full column; otherwise, <see langword="false" />.
        /// </value>
        public bool IsFullColumn
        {
            get
            {
                return this._isFullColumn;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is full row.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is full row; otherwise, <see langword="false" />.
        /// </value>
        public bool IsFullRow
        {
            get
            {
                return this._isFullRow;
            }
        }

        internal CalcRangeType RangeType
        {
            get
            {
                if (this._isFullRow && this._isFullColumn)
                {
                    return CalcRangeType.Sheet;
                }
                if (this._isFullRow)
                {
                    return CalcRangeType.Row;
                }
                if (this._isFullColumn)
                {
                    return CalcRangeType.Column;
                }
                return CalcRangeType.Cell;
            }
        }

        /// <summary>
        /// Gets the number of rows in the range. 
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the number of rows in the range. 
        /// </value>
        public int RowCount
        {
            get
            {
                return this._rowCount;
            }
            private set
            {
                this._rowCount = value;
            }
        }

        /// <summary>
        /// Gets the row index of the first cell in the range.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the row index of the first cell in the range.
        /// </value>
        public int RowIndex
        {
            get
            {
                return this._rowIndex;
            }
            private set
            {
                this._rowIndex = value;
            }
        }
    }
}

