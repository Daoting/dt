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
    /// Represents an area in a spreadsheet.
    /// </summary>
    public abstract class CalcReference
    {
        protected CalcReference()
        {
        }

        /// <summary>
        /// Gets the base column index for a range which specified by <paramref name="range" />.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <returns>An <see cref="T:System.Int32" /> indicates the base column index for range at <paramref name="range" />.</returns>
        public abstract int GetColumn(int range);
        /// <summary>
        /// Gets the count of columns in a range which specified by <paramref name="range" />.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <returns>An <see cref="T:System.Int32" /> indicates the count of rows in range at <paramref name="range" />.</returns>
        public abstract int GetColumnCount(int range);
        /// <summary>
        /// Gets the base row index for a range which specified by <paramref name="range" />.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <returns>An <see cref="T:System.Int32" /> indicates the base row index for range at <paramref name="range" />.</returns>
        public abstract int GetRow(int range);
        /// <summary>
        /// Get the count of rows in a range which specified by <paramref name="range" />.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <returns>An <see cref="T:System.Int32" /> indicates the count of columns in range at <paramref name="range" />.</returns>
        public abstract int GetRowCount(int range);
        /// <summary>
        /// Gets the source area.
        /// </summary>
        /// <returns>A <see cref="T:Dt.CalcEngine.CalcReference" /> indicates the source area.</returns>
        public abstract CalcReference GetSource();
        /// <summary>
        /// Gets the value at specified position.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="columnOffset">The column offset.</param>
        /// <returns>the value.</returns>
        public abstract object GetValue(int range, int rowOffset, int columnOffset);
        /// <summary>
        /// Determines whether the specified range is subtotal.
        /// </summary>
        /// <param name="range">The range index.</param>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="columnOffset">The column offset.</param>
        /// <returns>
        /// <see langword="true" /> if the specified range is subtotal; otherwise, <see langword="false" />.
        /// </returns>
        public abstract bool IsSubtotal(int range, int rowOffset, int columnOffset);

        /// <summary>
        /// Gets the count of the ranges in current area.
        /// </summary>
        /// <value>The range count.</value>
        public abstract int RangeCount { get; }
    }
}

