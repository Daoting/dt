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
    /// Represents a binary operator.
    /// </summary>
    public abstract class CalcBinaryOperator : CalcOperator
    {
        protected CalcBinaryOperator()
        {
        }

        /// <summary>
        /// Determines whether the operator accepts reference values
        /// for the specified operand.
        /// </summary>
        /// <param name="i">Index of the operand.</param>
        /// <returns>
        /// <see langword="true" /> if the operator accepts reference
        /// values for the specified operand; otherwise, <see langword="false" />.
        /// </returns>
        public virtual bool AcceptsReference(int i)
        {
            return false;
        }

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
        public abstract object Evaluate(object left, object right, CalcEvaluatorContext context);
        internal object EvaluteInternal(object left, object right, CalcEvaluatorContext context)
        {
            bool flag = CalcHelper.TryExtractToSingleValue(left, out left);
            bool flag2 = CalcHelper.TryExtractToSingleValue(right, out right);
            if (flag && flag2)
            {
                return this.EvaluteSingleValue(left, right);
            }
            int num = flag ? -1 : (left as CalcArray).RowCount;
            int num2 = flag ? -1 : (left as CalcArray).ColumnCount;
            int num3 = flag2 ? -1 : (right as CalcArray).RowCount;
            int num4 = flag2 ? -1 : (right as CalcArray).ColumnCount;
            int num5 = -1;
            int num6 = -1;
            if (!flag)
            {
                num5 = num;
                num6 = num2;
            }
            if (!flag2)
            {
                num5 = (num5 == -1) ? num3 : ((num3 > 1) ? ((num5 > 1) ? Math.Min(num3, num5) : num3) : num5);
                num6 = (num6 == -1) ? num4 : ((num4 > 1) ? ((num6 > 1) ? Math.Min(num4, num6) : num4) : num6);
            }
            object[,] values = new object[num5, num6];
            for (int i = 0; i < num5; i++)
            {
                for (int j = 0; j < num6; j++)
                {
                    values[i, j] = this.EvaluteSingleValue(flag ? left : CalcHelper.GetArrayValue(left as CalcArray, i, j), flag2 ? right : CalcHelper.GetArrayValue(right as CalcArray, i, j));
                }
            }
            return new ConcreteArray<object>(values);
        }

        private object EvaluteSingleValue(object left, object right)
        {
            return this.EvaluteSingleValueCore(left, right);
        }

        internal virtual object EvaluteSingleValueCore(object left, object right)
        {
            throw new InvalidOperationException();
        }
    }
}

