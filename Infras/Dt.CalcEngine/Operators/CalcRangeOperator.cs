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
    /// Represents a range operator.
    /// </summary>
    public class CalcRangeOperator : CalcBinaryOperator
    {
        /// <summary>
        /// Determines whether the operator accepts reference values
        /// for the specified operand.
        /// </summary>
        /// <param name="i">Index of the operand.</param>
        /// <returns>
        /// <see langword="true" /> if the operator accepts reference
        /// values for the specified operand; otherwise, <see langword="false" />.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return true;
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
        public override object Evaluate(object left, object right, CalcEvaluatorContext context)
        {
            CalcReference reference = left as CalcReference;
            CalcReference reference2 = right as CalcReference;
            if (((reference == null) || (reference2 == null)) || ((reference.RangeCount != 1) || (reference2.RangeCount != 1)))
            {
                return CalcErrors.Value;
            }
            CalcReference source = reference.GetSource();
            if ((source == null) || !source.Equals(reference2.GetSource()))
            {
                return CalcErrors.Value;
            }
            int row = reference.GetRow(0);
            int column = reference.GetColumn(0);
            int num3 = reference2.GetRow(0);
            int num4 = reference2.GetColumn(0);
            int num5 = Math.Min(row, num3);
            int num6 = Math.Min(column, num4);
            int rowCount = Math.Max((int) (row + reference.GetRowCount(0)), (int) (num3 + reference2.GetRowCount(0))) - num5;
            return new ConcreteReference(source, num5, num6, rowCount, Math.Max((int) (column + reference.GetColumnCount(0)), (int) (num4 + reference2.GetColumnCount(0))) - num6);
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
                return ":";
            }
        }
    }
}

