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
    /// Provides the ability to uniquely identify a cell.
    /// </summary>
    public class CalcExternalCellIdentity : CalcExternalIdentity
    {
        private int _columnIndex;
        private int _rowIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcExternalCellIdentity" /> class.
        /// </summary>
        /// <param name="cellIdentity">
        /// The source <see cref="T:Dt.CalcEngine.CalcExternalCellIdentity" /> 
        /// from which to create the new <see cref="T:Dt.CalcEngine.CalcExternalCellIdentity" />.
        /// </param>
        /// <param name="rowOffset">
        /// The amount to offset the row index.
        /// </param>
        /// <param name="columnOffset">
        /// The amount to offset the column index.
        /// </param>
        public CalcExternalCellIdentity(CalcExternalCellIdentity cellIdentity, int rowOffset, int columnOffset) : this(cellIdentity.Source, cellIdentity.RowIndex + rowOffset, cellIdentity.ColumnIndex + columnOffset)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.CalcExternalCellIdentity" /> class.
        /// </summary>
        /// <param name="source">
        /// The owner which contains the cell.
        /// </param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        public CalcExternalCellIdentity(ICalcSource source, int rowIndex, int columnIndex) : base(source)
        {
            this.RowIndex = rowIndex;
            this.ColumnIndex = columnIndex;
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
            CalcExternalCellIdentity objA = other as CalcExternalCellIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                return false;
            }
            return (((base._source == objA._source) && (this._rowIndex == objA._rowIndex)) && (this._columnIndex == objA._columnIndex));
        }

        /// <summary>
        /// Serves as a hash function for the <see cref="T:Dt.CalcEngine.CalcCellIdentity" /> type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        protected override int ComputeHash()
        {
            int num = (base._source != null) ? base._source.GetHashCode() : 0;
            return ((num ^ (this._rowIndex << 12)) ^ this._columnIndex);
        }

        /// <summary>
        /// Converts to non-external identity.
        /// </summary>
        /// <returns></returns>
        public override CalcLocalIdentity ConvertToLocal()
        {
            return new CalcCellIdentity(this.RowIndex, this.ColumnIndex);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("'{0}'!{1}", new object[] { (base.Source != null) ? base.Source.GetParserContext(null).GetExternalSourceToken(base.Source) : CalcErrors.Reference.ToString(), CalcIdentity.GetCoord(this.RowIndex, this.ColumnIndex) });
        }

        /// <summary>
        /// Gets the column index of the current cell.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the column index of the current cell.
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
        /// Gets the row index of the current cell.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.Int32" /> value represents the row index of the current cell.
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

