#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents a cell reference expression.
    /// </summary>
    public class CalcCellExpression : CalcReferenceExpression
    {
        private int _column;
        private bool _columnRelative;
        private int _row;
        private bool _rowRelative;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcCellExpression" /> class.
        /// </summary>
        /// <param name="row">Row coordinate of cell.</param>
        /// <param name="column">Column coordinate of cell.</param>
        public CalcCellExpression(int row, int column) : this(row, column, false, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcCellExpression" /> class.
        /// </summary>
        /// <param name="row">Row coordinate of cell.</param>
        /// <param name="column">Column coordinate of cell.</param>
        /// <param name="rowRelative">Whether row coordinate is relative.</param>
        /// <param name="columnRelative">Whether the column coordinate is relative.</param>
        public CalcCellExpression(int row, int column, bool rowRelative, bool columnRelative)
        {
            this._row = row;
            this._column = column;
            this._rowRelative = rowRelative;
            this._columnRelative = columnRelative;
        }

        /// <summary>
        /// Gets the identity of current expressions based on <paramref name="row" /> and <paramref name="column" />.
        /// </summary>
        /// <param name="row">The base row.</param>
        /// <param name="column">The base column.</param>
        /// <returns></returns>
        public override CalcIdentity GetId(int row, int column)
        {
            return new CalcCellIdentity(this._rowRelative ? CalcHelper.NormalizeRowIndex(this.Row + row, 0xfffff) : this.Row, this._columnRelative ? CalcHelper.NormalizeRowIndex(this.Column + column, 0xfffff) : this.Column);
        }

        /// <summary>
        /// Get a new expression with specific offset.
        /// </summary>
        /// <param name="row">the row offset</param>
        /// <param name="column">the column offset</param>
        /// <param name="offsetAbsolute"><c>true</c> if offset the absolute indexes.</param>
        /// <param name="offsetRelative"><c>true</c> if offset the relative indexes.</param>
        /// <returns>
        /// Return a <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> which offset from current expression.
        /// </returns>
        public override CalcExpression Offset(int row, int column, bool offsetAbsolute = false, bool offsetRelative = true)
        {
            bool flag = (this.RowRelative && offsetRelative) || (!this.RowRelative && offsetAbsolute);
            bool flag2 = (this.ColumnRelative && offsetRelative) || (!this.ColumnRelative && offsetAbsolute);
            return new CalcCellExpression(this.Row + (flag ? row : 0), this.Column + (flag2 ? column : 0), this.RowRelative, this.ColumnRelative);
        }

        /// <summary>
        /// Gets the column index of current cell.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the column index of current cell.
        /// </value>
        public int Column
        {
            get
            {
                return this._column;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the column coordinate is relative;
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool ColumnRelative
        {
            get
            {
                return this._columnRelative;
            }
        }

        /// <summary>
        /// Gets the row index of current cell.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.Int32" /> value that represents the row index of current cell.
        /// </value>
        public int Row
        {
            get
            {
                return this._row;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the row coordinate is relative.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the row coordinate is relative; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public bool RowRelative
        {
            get
            {
                return this._rowRelative;
            }
        }
    }
}

