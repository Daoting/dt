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
    public class CalcExternalRangeIdentity : CalcExternalIdentity
    {
        private int _columnCount;
        private int _columnIndex;
        private bool _isFullColumn;
        private bool _isFullRow;
        private int _rowCount;
        private int _rowIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        /// <param name="source">
        /// The owner which contains the cell range.
        /// </param>
        public CalcExternalRangeIdentity(ICalcSource source) : base(source)
        {
            this.RowIndex = -1;
            this.ColumnIndex = -1;
            this.RowCount = -1;
            this.ColumnCount = -1;
            this._isFullRow = true;
            this._isFullColumn = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcExternalRangeIdentity" /> class.
        /// </summary>
        /// <param name="source">
        /// The owner which contains the cell range.
        /// </param>
        /// <param name="bandIndex">Row or column index of first row or column in the range</param>
        /// <param name="bandCount">Number of rows or Columns in the range</param>
        /// <param name="isRowBand"><see langword="true" /> if current range is a full row range, otherwise, 
        /// current range is a full column range.</param>
        public CalcExternalRangeIdentity(ICalcSource source, int bandIndex, int bandCount, bool isRowBand) : base(source)
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
        /// <param name="source">
        /// The owner which contains the cell range.
        /// </param>
        /// <param name="rowIndex">Row index of first cell in the range</param>
        /// <param name="columnIndex">Column index of first cell in the range</param>
        /// <param name="rowCount">Number of rows in the range</param>
        /// <param name="columnCount">Number of columns in the range</param>
        public CalcExternalRangeIdentity(ICalcSource source, int rowIndex, int columnIndex, int rowCount, int columnCount) : base(source)
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
            CalcExternalRangeIdentity objA = other as CalcExternalRangeIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return (((((base._source == objA._source) && (this._isFullRow == objA._isFullRow)) && ((this._isFullColumn == objA._isFullColumn) && (this._rowIndex == objA._rowIndex))) && ((this._columnIndex == objA._columnIndex) && (this._rowCount == objA._rowCount))) && (this._columnCount == objA._columnCount));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            int num = (base.Source != null) ? base.Source.GetHashCode() : 0;
            return (num ^ ((((this.RowIndex ^ ((this.ColumnIndex << 13) | (this.ColumnIndex >> 0x13))) ^ (this.RowCount << 0x1a)) | ((this.RowCount >> 6) ^ (this.ColumnCount << 7))) | (this.ColumnCount >> 0x19)));
        }

        /// <summary>
        /// Converts to non-external identity.
        /// </summary>
        /// <returns></returns>
        public override CalcLocalIdentity ConvertToLocal()
        {
            switch (this.RangeType)
            {
                case CalcRangeType.Cell:
                    return new CalcRangeIdentity(this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount);

                case CalcRangeType.Row:
                    return new CalcRangeIdentity(this.RowIndex, this.RowCount, true);

                case CalcRangeType.Column:
                    return new CalcRangeIdentity(this.ColumnIndex, this.ColumnCount, false);

                case CalcRangeType.Sheet:
                    return new CalcRangeIdentity();
            }
            return new CalcRangeIdentity(this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("'{0}'!{1}", new object[] { (base.Source != null) ? base.Source.GetParserContext(null).GetExternalSourceToken(base.Source) : CalcErrors.Reference.ToString(), CalcIdentity.GetCoord(this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount, this.RangeType) });
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
                if (this.IsFullRow && this.IsFullColumn)
                {
                    return CalcRangeType.Sheet;
                }
                if (this.IsFullRow)
                {
                    return CalcRangeType.Row;
                }
                if (this.IsFullColumn)
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

