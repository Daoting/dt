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
    /// Represents an expression that has a binary operator.
    /// </summary>
    public class CalcBinaryOperatorExpression : CalcOperatorExpression
    {
        private CalcExpression _left;
        private CalcBinaryOperator _operator;
        private CalcExpression _right;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.CalcEngine.Expressions.CalcBinaryOperatorExpression" /> class.
        /// </summary>
        /// <param name="oper">The binary operator.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        public CalcBinaryOperatorExpression(CalcBinaryOperator oper, CalcExpression left, CalcExpression right)
        {
            base.ThrowIfArgumentNull<CalcBinaryOperator>(oper, "oper");
            base.ThrowIfArgumentNull<CalcExpression>(left, "left");
            base.ThrowIfArgumentNull<CalcExpression>(right, "right");
            this._operator = oper;
            this._left = left;
            this._right = right;
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
            return new CalcBinaryOperatorExpression(this.Operator, this.Left.Offset(row, column, offsetAbsolute, offsetRelative), this.Right.Offset(row, column, offsetAbsolute, offsetRelative));
        }

        /// <summary>
        /// Gets the left operand of the binary operation.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> that represents the left operand of the binary operation.
        /// </value>
        public CalcExpression Left
        {
            get
            {
                return this._left;
            }
        }

        /// <summary>
        /// Gets the operator of the binary operation.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Operators.CalcUnaryOperator" /> that represents the binary operation.
        /// </value>
        public CalcBinaryOperator Operator
        {
            get
            {
                return this._operator;
            }
        }

        /// <summary>
        /// Gets the right operand of the binary operation.
        /// </summary>
        /// <value>
        /// A <see cref="T:Dt.CalcEngine.Expressions.CalcExpression" /> that represents the right operand of the binary operation.
        /// </value>
        public CalcExpression Right
        {
            get
            {
                return this._right;
            }
        }
    }
}

