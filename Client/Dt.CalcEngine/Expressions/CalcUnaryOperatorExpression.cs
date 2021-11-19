#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Operators;
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Expressions
{
    /// <summary>
    /// Represents an expression that has a unary operator.
    /// </summary>
    public class CalcUnaryOperatorExpression : CalcOperatorExpression
    {
        private CalcExpression _operand;
        private CalcUnaryOperator _operator;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcUnaryOperatorExpression" /> class.
        /// </summary>
        /// <param name="oper">The unary operator.</param>
        /// <param name="operand">The operand.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="oper" /> or <paramref name="operand" /> is <see langword="null" />.</exception>
        public CalcUnaryOperatorExpression(CalcUnaryOperator oper, CalcExpression operand)
        {
            base.ThrowIfArgumentNull<CalcUnaryOperator>(oper, "oper");
            base.ThrowIfArgumentNull<CalcExpression>(operand, "operand");
            this._operator = oper;
            this._operand = operand;
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
            return new CalcUnaryOperatorExpression(this.Operator, this.Operand.Offset(row, column, offsetAbsolute, offsetRelative));
        }

        /// <summary>
        /// Gets the operand of the unary operation.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> that represents the operand of the unary operation.
        /// </value>
        public CalcExpression Operand
        {
            get
            {
                return this._operand;
            }
        }

        /// <summary>
        /// Gets the operator of the unary operation.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Operators.CalcUnaryOperator" /> that represents the unary operation.
        /// </value>
        public CalcUnaryOperator Operator
        {
            get
            {
                return this._operator;
            }
        }
    }
}

