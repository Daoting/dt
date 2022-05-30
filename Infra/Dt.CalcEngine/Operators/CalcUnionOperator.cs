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
    /// Represents an union operator.
    /// </summary>
    public class CalcUnionOperator : CalcBinaryOperator
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
            if ((reference == null) || (reference2 == null))
            {
                return CalcErrors.Value;
            }
            int rangeCount = reference.RangeCount;
            int num2 = reference2.RangeCount;
            ConcreteReference.Range[] areas = new ConcreteReference.Range[rangeCount + num2];
            for (int i = 0; i < rangeCount; i++)
            {
                int row = reference.GetRow(i);
                int column = reference.GetColumn(i);
                int rowCount = reference.GetRowCount(i);
                int columnCount = reference.GetColumnCount(i);
                areas[i] = new ConcreteReference.Range(row, column, rowCount, columnCount);
            }
            for (int j = 0; j < num2; j++)
            {
                int num9 = reference2.GetRow(j);
                int num10 = reference2.GetColumn(j);
                int num11 = reference2.GetRowCount(j);
                int num12 = reference2.GetColumnCount(j);
                areas[rangeCount + j] = new ConcreteReference.Range(num9, num10, num11, num12);
            }
            return new ConcreteReference(reference.GetSource(), areas);
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
                return ",";
            }
        }
    }
}

