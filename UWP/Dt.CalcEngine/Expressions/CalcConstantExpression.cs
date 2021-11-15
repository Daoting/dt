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

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an expression that has a constant value.
    /// This is an abstract class.
    /// </summary>
    public abstract class CalcConstantExpression : CalcExpression
    {
        private object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcConstantExpression" /> class.
        /// </summary>
        /// <param name="value">The constant value.</param>
        protected CalcConstantExpression(object value)
        {
            this._value = value;
        }

        /// <summary>
        /// Gets the value of the constant expression.
        /// </summary>
        /// <value>An <see cref="T:System.Object" /> equal to the value of the represented expression.</value>
        public object Value
        {
            get
            {
                return this._value;
            }
        }
    }
}

