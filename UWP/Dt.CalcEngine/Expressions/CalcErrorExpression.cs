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
    /// Represents an error constant value.
    /// </summary>
    public class CalcErrorExpression : CalcConstantExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcErrorExpression" /> class.
        /// </summary>
        /// <param name="value">The error value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public CalcErrorExpression(CalcError value) : base(value)
        {
            if (value == null)
            {
                throw new ArgumentNullException();
            }
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
            return new CalcErrorExpression(this.ErrorValue);
        }

        /// <summary>
        /// Gets the value of the error expression.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.CalcEngine.CalcError" /> equal 
        /// to the value of the represented expression.
        /// </value>
        public CalcError ErrorValue
        {
            get
            {
                return (CalcError) base.Value;
            }
        }
    }
}

