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
    /// Represents a plus operator.
    /// </summary>
    public class CalcPlusOperator : CalcUnaryOperator
    {
        /// <summary>
        /// Returns the result of the operator applied to the operand.
        /// </summary>
        /// <param name="operand">
        /// Operand for the operator evaluation.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:Dt.CalcEngine.CalcEvaluatorContext" /> associate with the operator evaluation.
        /// </param>
        /// <returns>
        /// Result of the operator applied to the operand.
        /// </returns>
        public override object Evaluate(object operand, CalcEvaluatorContext context)
        {
            return base.EvaluteInternal(operand, context);
        }

        internal override object EvaluteSingleValue(object operand, CalcEvaluatorContext context)
        {
            double num;
            if (operand is TimeSpan)
            {
                return (TimeSpan) operand;
            }
            if (CalcConvert.TryToDouble(operand, out num, true))
            {
                return (double) num;
            }
            if (operand is string)
            {
                return operand;
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

