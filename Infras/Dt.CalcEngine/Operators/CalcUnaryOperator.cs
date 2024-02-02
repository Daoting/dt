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
    /// Represents an unary operator.
    /// </summary>
    public abstract class CalcUnaryOperator : CalcOperator
    {
        protected CalcUnaryOperator()
        {
        }

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
        public abstract object Evaluate(object operand, CalcEvaluatorContext context);
        internal object EvaluteInternal(object operand, CalcEvaluatorContext context)
        {
            CalcArray array = operand as CalcArray;
            CalcReference reference = operand as CalcReference;
            if (reference != null)
            {
                array = CalcConvert.ToArray(reference);
            }
            if (array == null)
            {
                return this.EvaluteSingleValue(operand, context);
            }
            if (array.Length < 1)
            {
                return CalcErrors.Number;
            }
            object[,] values = new object[array.RowCount, array.ColumnCount];
            CalcOperator.OperatorArray array2 = new CalcOperator.OperatorArray(values);
            object obj2 = array.GetValue(0);
            Type type = (obj2 != null) ? obj2.GetType() : typeof(object);
            for (int i = 0; i < array.RowCount; i++)
            {
                for (int j = 0; j < array.ColumnCount; j++)
                {
                    object obj3 = array.GetValue(i, j);
                    if ((obj3 != null) && (obj3.GetType() != type))
                    {
                        return CalcErrors.Number;
                    }
                    object obj4 = this.EvaluteSingleValue(obj3, context);
                    values[i, j] = obj4;
                }
            }
            return array2;
        }

        internal virtual object EvaluteSingleValue(object operand, CalcEvaluatorContext context)
        {
            throw new InvalidOperationException();
        }
    }
}

