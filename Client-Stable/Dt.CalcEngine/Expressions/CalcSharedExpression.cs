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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an expression that refer another expression.
    /// </summary>
    public class CalcSharedExpression : CalcExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcSharedExpression" />.
        /// </summary>
        /// <param name="expr"></param>
        public CalcSharedExpression(CalcExpression expr)
        {
            this.Expression = expr;
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
            return this.Expression.Offset(row, column, offsetAbsolute, offsetRelative);
        }

        /// <summary>
        /// Gets the referenced expression.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> value that declares the referenced expression.
        /// </value>
        public CalcExpression Expression { get; private set; }
    }
}

