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

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the <see cref="T:System.Double" /> value from the chi-squared distribution. 
    /// </summary>
    public class CalcChiTestFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process an array arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process an array arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsArray(int i)
        {
            return true;
        }

        /// <summary>
        /// Determines whether the function accepts CalcReference values
        /// for the specified argument.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <returns>
        /// <see langword="true" /> if the function accepts CalcReference values
        /// for the specified argument; <see langword="false" /> otherwise.
        /// </returns>
        public override bool AcceptsReference(int i)
        {
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> value from the chi-squared distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: actual_range, expected_range.
        /// </para>
        /// <para>
        /// Actual_range is the range of data that contains observations to test against expected values.
        /// </para>
        /// <para>
        /// Expected_range is the range of data that contains the ratio of the product of row totals and column totals to the grand total.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num2;
            base.CheckArgumentsLength(args);
            double num = 0.0;
            int rowCount = ArrayHelper.GetRowCount(args[0], 0);
            int columnCount = ArrayHelper.GetColumnCount(args[0], 0);
            if ((rowCount != ArrayHelper.GetRowCount(args[1], 0)) || (columnCount != ArrayHelper.GetColumnCount(args[1], 0)))
            {
                return CalcErrors.NotAvailable;
            }
            if ((rowCount > 1) && (columnCount > 1))
            {
                num2 = (rowCount - 1) * (columnCount - 1);
            }
            else if ((rowCount > 1) && (columnCount == 1))
            {
                num2 = rowCount - 1;
            }
            else if ((rowCount == 1) && (columnCount > 1))
            {
                num2 = columnCount - 1;
            }
            else
            {
                return CalcErrors.NotAvailable;
            }
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    double num7;
                    double num8;
                    object obj2 = ArrayHelper.GetValue(args[0], i, j, 0);
                    if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                    object obj3 = ArrayHelper.GetValue(args[1], i, j, 0);
                    if (obj3 is CalcError)
                    {
                        return obj3;
                    }
                    if (!CalcConvert.TryToDouble(obj2, out num7, true) || !CalcConvert.TryToDouble(obj3, out num8, true))
                    {
                        return CalcErrors.Value;
                    }
                    if (num8 == 0.0)
                    {
                        return CalcErrors.DivideByZero;
                    }
                    num += ((num7 - num8) * (num7 - num8)) / num8;
                }
            }
            CalcBuiltinFunction function = new CalcChiDistFunction();
            return function.Evaluate(new object[] { (double) num, (int) num2 });
        }

        /// <summary>
        /// Gets the maximum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The maximum number of arguments for the function.
        /// </value>
        public override int MaxArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets the minimum number of arguments for the function.
        /// </summary>
        /// <value>
        /// The minimum number of arguments for the function.
        /// </value>
        public override int MinArgs
        {
            get
            {
                return 2;
            }
        }

        /// <summary>
        /// Gets The name of the function.
        /// </summary>
        /// <value>
        /// The name of the function.
        /// </value>
        public override string Name
        {
            get
            {
                return "CHITEST";
            }
        }
    }
}

