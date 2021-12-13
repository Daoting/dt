#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// A class implements <see cref="T:Dt.Xls.IRange" /> used to represents a range
    /// </summary>
    public class ExcelCellRange : IRange
    {
        /// <summary>
        /// Specifies whether this GrepaCity.Excel.ExcelCellRange has the same setting compared with the specified System.Object
        /// </summary>
        /// <param name="obj">The System.Object used to compare</param>
        /// <returns>true if the obj is a ExcelCellRange instance and has the same setting as this Dt.Xls.ExcelCellRange</returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ExcelCellRange))
            {
                return false;
            }
            ExcelCellRange range = obj as ExcelCellRange;
            return ((((this.Row == range.Row) && (this.Column == range.Column)) && (this.RowSpan == range.RowSpan)) && (this.ColumnSpan == range.ColumnSpan));
        }

        /// <summary>
        /// Returns a hash code for this GrepaCity.Excel.ExcelCellRange
        /// </summary>
        /// <returns>&gt;An integer value that specifies a hash value for this object</returns>
        public override int GetHashCode()
        {
            return (((((int) this.Row).GetHashCode() ^ ((int) this.Column).GetHashCode()) ^ ((int) this.RowSpan).GetHashCode()) ^ ((int) this.ColumnSpan).GetHashCode());
        }

        /// <summary>
        /// Gets the zero-based index of the start column of the range.
        /// </summary>
        /// <value>The start column index.</value>
        public int Column { get; set; }

        /// <summary>
        /// Gets the column span of the range.
        /// </summary>
        /// <value>The column span.</value>
        public int ColumnSpan { get; set; }

        /// <summary>
        /// Gets the zero-based index of the start row of the range.
        /// </summary>
        /// <value>The start row index.</value>
        public int Row { get; set; }

        /// <summary>
        /// Gets the row span of the range.
        /// </summary>
        /// <value>The row span.</value>
        public int RowSpan { get; set; }
    }
}

