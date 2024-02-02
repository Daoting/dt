#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents a range of cells for a sheet.
    /// </summary>
    public class CalcSheetRangeIdentity : CalcIdentity
    {
        private bool _isFullColumn;
        private bool _isFullRow;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcRangeIdentity" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        public CalcSheetRangeIdentity(ICalcSource startSource, ICalcSource endSource)
        {
            this.StartSource = startSource;
            this.EndSource = endSource;
            this.RowIndex = -1;
            this.ColumnIndex = -1;
            this.RowCount = -1;
            this.ColumnCount = -1;
            this._isFullRow = true;
            this._isFullColumn = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcSheetRangeIdentity" /> class.
        /// </summary>
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        /// <param name="bandIndex">Row or column index of first row or column in the range</param>
        /// <param name="bandCount">Number of rows or Columns in the range</param>
        /// <param name="isRowBand"><see langword="true" /> if current range is a full row range, otherwise, 
        /// current range is a full column range.</param>
        public CalcSheetRangeIdentity(ICalcSource startSource, ICalcSource endSource, int bandIndex, int bandCount, bool isRowBand)
        {
            this.StartSource = startSource;
            this.EndSource = endSource;
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
        /// <param name="startSource">Starting owner of cell range.</param>
        /// <param name="endSource">Ending owner of cell range.</param>
        /// <param name="rowIndex">Row index of first cell in the range</param>
        /// <param name="columnIndex">Column index of first cell in the range</param>
        /// <param name="rowCount">Number of rows in the range</param>
        /// <param name="columnCount">Number of columns in the range</param>
        public CalcSheetRangeIdentity(ICalcSource startSource, ICalcSource endSource, int rowIndex, int columnIndex, int rowCount, int columnCount)
        {
            this.StartSource = startSource;
            this.EndSource = endSource;
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
            CalcSheetRangeIdentity objA = other as CalcSheetRangeIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return (((((this.StartSource == objA.StartSource) && (this.EndSource == objA.EndSource)) && ((this.IsFullRow == objA.IsFullRow) && (this.IsFullColumn == objA.IsFullColumn))) && (((this.RowIndex == objA.RowIndex) && (this.ColumnIndex == objA.ColumnIndex)) && (this.RowCount == objA.RowCount))) && (this.ColumnCount == objA.ColumnCount));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            int num = (this.StartSource != null) ? this.StartSource.GetHashCode() : 0;
            int num2 = (this.EndSource != null) ? this.EndSource.GetHashCode() : 0;
            return ((num ^ num2) ^ ((((this.RowIndex ^ ((this.ColumnIndex << 13) | (this.ColumnIndex >> 0x13))) ^ (this.RowCount << 0x1a)) | ((this.RowCount >> 6) ^ (this.ColumnCount << 7))) | (this.ColumnCount >> 0x19)));
        }

        /// <summary>
        /// Converts to non-external identity.
        /// </summary>
        /// <returns></returns>
        public CalcLocalIdentity ConvertToLocal()
        {
            if (this.IsFullRow && this.IsFullColumn)
            {
                return new CalcRangeIdentity();
            }
            if (this.IsFullRow)
            {
                return new CalcRangeIdentity(this.RowIndex, this.RowCount, true);
            }
            if (this.IsFullColumn)
            {
                return new CalcRangeIdentity(this.ColumnIndex, this.ColumnCount, false);
            }
            if ((this.RowCount == 1) && (this.ColumnCount == 1))
            {
                return new CalcCellIdentity(this.RowIndex, this.ColumnIndex);
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
            return string.Format("'{0}:{1}'!{2}", new object[] { (this.StartSource != null) ? this.StartSource.GetParserContext(null).GetExternalSourceToken(this.StartSource) : CalcErrors.Reference.ToString(), (this.StartSource != null) ? this.StartSource.GetParserContext(null).GetExternalSourceToken(this.EndSource) : CalcErrors.Reference.ToString(), CalcIdentity.GetCoord(this.RowIndex, this.ColumnIndex, this.RowCount, this.ColumnCount, this.RangeType) });
        }

        /// <summary>
        /// Gets the number of columns in the range. 
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the number of columns in the range. 
        /// </value>
        public int ColumnCount { get; private set; }

        /// <summary>
        /// Gets the column index of the first cell in the range.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the column index of the first cell in the range.
        /// </value>
        public int ColumnIndex { get; private set; }

        /// <summary>
        /// Gets or sets the end source.
        /// </summary>
        /// <value>The source.</value>
        public ICalcSource EndSource { get; private set; }

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
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets the row index of the first cell in the range.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the row index of the first cell in the range.
        /// </value>
        public int RowIndex { get; private set; }

        /// <summary>
        /// Gets or sets the start source.
        /// </summary>
        /// <value>The source.</value>
        public ICalcSource StartSource { get; private set; }
    }
}

