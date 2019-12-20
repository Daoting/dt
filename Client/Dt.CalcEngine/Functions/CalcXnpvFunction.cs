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
    /// Returns the <see cref="T:System.Double" /> net present value for a series of cash flows not at regular intervals.
    /// </summary>
    public class CalcXnpvFunction : CalcBuiltinFunction
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
            return (i != 0);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process references.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process references; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsReference(int i)
        {
            return (i != 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> net present value for a series of cash flows not at regular intervals.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: rate, values, dates.
        /// </para>
        /// <para>
        /// Rate is the discount rate to apply to the cash flows.
        /// </para>
        /// <para>
        /// Values is a series of cash flows that corresponds to a schedule of payments in dates.
        /// The first payment is optional and corresponds to a cost or payment that occurs at the beginning of the investment.
        /// If the first value is a cost or payment, it must be a negative value. All succeeding payments are discounted based on a 365-day year.
        /// The series of values must contain at least one positive value and one negative value.
        /// </para>
        /// <para>
        /// Dates is a schedule of payment dates that corresponds to the cash flow payments.
        /// The first payment date indicates the beginning of the schedule of payments.
        /// All other dates must be later than this date, but they may occur in any order.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double[] numArray = new double[ArrayHelper.GetLength(args[1], 0)];
            DateTime[] timeArray = new DateTime[ArrayHelper.GetLength(args[2], 0)];
            double num2 = 0.0;
            if (ArrayHelper.GetLength(args[1], 0) != ArrayHelper.GetLength(args[2], 0))
            {
                return CalcErrors.Number;
            }
            for (int i = 0; i < ArrayHelper.GetLength(args[1], 0); i++)
            {
                numArray[i] = CalcConvert.ToDouble(ArrayHelper.GetValue(args[1], i, 0));
            }
            for (int j = 0; j < ArrayHelper.GetLength(args[2], 0); j++)
            {
                timeArray[j] = CalcConvert.ToDateTime(ArrayHelper.GetValue(args[2], j, 0));
            }
            for (int k = 0; k < ArrayHelper.GetLength(args[1], 0); k++)
            {
                num2 += numArray[k] / Math.Pow(1.0 + num, (timeArray[k].ToOADate() - timeArray[0].ToOADate()) / 365.0);
            }
            return (double) num2;
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
                return 3;
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
                return 3;
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
                return "XNPV";
            }
        }
    }
}

