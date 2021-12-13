#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents a cell reference on current sheet expression.
    /// </summary>
    public class CalcBangCellExpression : CalcCellExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcCellExpression" /> class.
        /// </summary>
        /// <param name="row">Row coordinate of cell.</param>
        /// <param name="column">Column coordinate of cell.</param>
        public CalcBangCellExpression(int row, int column) : base(row, column)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcCellExpression" /> class.
        /// </summary>
        /// <param name="row">Row coordinate of cell.</param>
        /// <param name="column">Column coordinate of cell.</param>
        /// <param name="rowRelative">Whether row coordinate is relative.</param>
        /// <param name="columnRelative">Whether the column coordinate is relative.</param>
        public CalcBangCellExpression(int row, int column, bool rowRelative, bool columnRelative) : base(row, column, rowRelative, columnRelative)
        {
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
            bool flag = (base.RowRelative && offsetRelative) || (!base.RowRelative && offsetAbsolute);
            bool flag2 = (base.ColumnRelative && offsetRelative) || (!base.ColumnRelative && offsetAbsolute);
            return new CalcBangCellExpression(base.Row + (flag ? row : 0), base.Column + (flag2 ? column : 0), base.RowRelative, base.ColumnRelative);
        }
    }
}

