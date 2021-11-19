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
    /// Represents a double constant value.
    /// </summary>
    public class CalcDoubleExpression : CalcConstantExpression
    {
        private string _originalValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcDoubleExpression" /> class.
        /// </summary>
        /// <param name="value">The double value.</param>
        public CalcDoubleExpression(double value) : this(value, ((double) value).ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcDoubleExpression" /> class.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <param name="originalValue">
        /// The original string of the number.
        /// </param>
        public CalcDoubleExpression(double value, string originalValue) : base((double) value)
        {
            this._originalValue = originalValue;
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
            return new CalcDoubleExpression(this.DoubleValue, this.OriginalValue);
        }

        /// <summary>
        /// Gets the value of the double expression.
        /// </summary>
        /// <value>An <see cref="T:System.Double" /> equal to the value of the represented expression.</value>
        public double DoubleValue
        {
            get
            {
                return (double) ((double) base.Value);
            }
        }

        /// <summary>
        /// Gets the original string of the number.
        /// </summary>
        /// <value>
        /// An <see cref="T:System.String" /> to indicate the original string of the number.
        /// </value>
        public string OriginalValue
        {
            get
            {
                return this._originalValue;
            }
        }
    }
}

