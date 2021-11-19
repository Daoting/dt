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
    /// Represents a <see cref="T:Dt.CalcEngine.CalcArray" /> which wrap a source object like a two dimension array. 
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="P:Dt.CalcEngine.CalcArray.RowCount" /> indicates the count of row dimension. 
    /// The <see cref="P:Dt.CalcEngine.CalcArray.ColumnCount" /> indicates the count of column dimension.
    /// </para>
    /// <para>
    /// The index have two forms, one is expressed with Row and Column, and another is zero-based index.
    /// The equivalence for these tow forms is as below: <br />
    /// [index] = [row]*[<see cref="P:Dt.CalcEngine.CalcArray.RowCount" />] + [column]
    /// </para>
    /// </remarks>
    public abstract class CalcArray
    {
        protected CalcArray()
        {
        }

        /// <summary>
        /// Gets the value at the specified position in the array.
        /// </summary>
        /// <param name="index">The at which value should be get.</param>
        /// <returns>Value at the specified position.</returns>
        public virtual object GetValue(int index)
        {
            return this.GetValue(index / this.ColumnCount, index % this.ColumnCount);
        }

        /// <summary>
        /// Gets the value at the specified position in the array.
        /// </summary>
        /// <param name="row">The row index at which value should be get.</param>
        /// <param name="column">The column index at which value should be get.</param>
        /// <returns>Value at the specified position.</returns>
        public abstract object GetValue(int row, int column);

        /// <summary>
        /// Gets the number of columns in the array.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that indicates the number of rows in the array.
        /// </value>
        public abstract int ColumnCount { get; }

        /// <summary>
        /// Gets the length of this array.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that indicates the length of this array.
        /// </value>
        public virtual int Length
        {
            get
            {
                return (this.RowCount * this.ColumnCount);
            }
        }

        /// <summary>
        /// Gets the number of rows in the array.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that indicates the number of rows in the array.
        /// </value>
        public abstract int RowCount { get; }
    }
}

