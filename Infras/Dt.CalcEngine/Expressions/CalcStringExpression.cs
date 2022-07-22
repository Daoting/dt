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
    /// Represents a string constant value.
    /// </summary>
    public class CalcStringExpression : CalcConstantExpression
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcStringExpression" /> class.
        /// </summary>
        /// <param name="value">The string value.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="value" /> is <see langword="null" />.</exception>
        public CalcStringExpression(string value) : base(value)
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
            return new CalcStringExpression(this.StringValue);
        }

        /// <summary>
        /// Gets the value of the string expression.
        /// </summary>
        /// <value>An <see cref="T:System.String" /> equal to the value of the represented expression.</value>
        public string StringValue
        {
            get
            {
                return (string) ((string) base.Value);
            }
        }
    }
}

