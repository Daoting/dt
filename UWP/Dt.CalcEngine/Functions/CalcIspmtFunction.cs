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
    /// Returns the <see cref="T:System.Double" /> interest paid during a specific period of an investment.
    /// </summary>
    public class CalcIspmtFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> interest paid during a specific period of an investment.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: rate, per, nper, pv.
        /// </para>
        /// <para>
        /// Rate is the interest rate for the investment.
        /// </para>
        /// <para>
        /// Per is the period for which you want to find the interest, and must be between 1 and nper.
        /// </para>
        /// <para>
        /// Nper is the total number of payment periods for the investment.
        /// </para>
        /// <para>
        /// Pv is the present value of the investment. For a loan, pv is the loan amount.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            int num2 = CalcConvert.ToInt(args[1]);
            int num3 = CalcConvert.ToInt(args[2]);
            double num4 = CalcConvert.ToDouble(args[3]);
            if (num3 == 0)
            {
                return CalcErrors.DivideByZero;
            }
            return CalcConvert.ToResult((num4 * num) * ((((double) num2) / ((double) num3)) - 1.0));
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
                return 4;
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
                return 4;
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
                return "ISPMT";
            }
        }
    }
}

