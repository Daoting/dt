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
    /// Represents an expression type for parentheses surrounding a specified expression.
    /// </summary>
    public class CalcParenthesesExpression : CalcExpression
    {
        private CalcExpression _argument;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcParenthesesExpression" /> class.
        /// </summary>
        /// <param name="arg">Expression inside the parentheses.</param>
        /// <exception cref="T:System.ArgumentNullException">arg is <see langword="null" />. </exception>
        public CalcParenthesesExpression(CalcExpression arg)
        {
            base.ThrowIfArgumentNull<CalcExpression>(arg, "arg");
            this._argument = arg;
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
            return new CalcParenthesesExpression(this.Arg.Offset(row, column, offsetAbsolute, offsetRelative));
        }

        /// <summary>
        /// Gets the expression inside the parentheses.
        /// </summary>
        /// <value>The expression inside the parentheses.</value>
        public CalcExpression Arg
        {
            get
            {
                return this._argument;
            }
        }
    }
}

