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
#endregion

namespace Dt.CalcEngine.Operators
{
    /// <summary>
    /// Represents an addition operator.
    /// </summary>
    public class CalcAddOperator : CalcBinaryOperator
    {
        /// <summary>
        /// Returns the result of the operator applied to the operands.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand..</param>
        /// <param name="context">
        /// The <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" /> associate with the operator evaluation.
        /// </param>
        /// <returns>
        /// Result of the operator applied to the operand.
        /// </returns>
        public override object Evaluate(object left, object right, CalcEvaluatorContext context)
        {
            return base.EvaluteInternal(left, right, context);
        }

        internal override object EvaluteSingleValueCore(object left, object right)
        {
            double num;
            double num2;
            if (left is CalcError)
            {
                return left;
            }
            if (right is CalcError)
            {
                return right;
            }
            if ((left is DateTime) && (right is TimeSpan))
            {
                try
                {
                    return (((DateTime) left) + ((TimeSpan) right));
                }
                catch (ArgumentOutOfRangeException)
                {
                    return CalcErrors.Number;
                }
            }
            if ((left is TimeSpan) && (right is DateTime))
            {
                try
                {
                    return (((DateTime) right) + ((TimeSpan) left));
                }
                catch (ArgumentOutOfRangeException)
                {
                    return CalcErrors.Number;
                }
            }
            if ((left is TimeSpan) && (right is TimeSpan))
            {
                try
                {
                    return (((TimeSpan) left) + ((TimeSpan) right));
                }
                catch (OverflowException)
                {
                    return CalcErrors.Number;
                }
            }
            if (CalcConvert.TryToDouble(left, out num, true) && CalcConvert.TryToDouble(right, out num2, true))
            {
                return CalcConvert.ToResult(num + num2);
            }
            return CalcErrors.Value;
        }

        /// <summary>
        /// Gets the name of the operator.
        /// </summary>
        /// <value>
        /// A <see cref="T:System.String" /> value that represents the name of the operator.
        /// </value>
        public override string Name
        {
            get
            {
                return "+";
            }
        }
    }
}

