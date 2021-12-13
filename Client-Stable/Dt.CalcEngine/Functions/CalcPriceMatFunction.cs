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
    /// Returns the <see cref="T:System.Double" /> price per $100 face value of a security that pays interest at maturity.
    /// </summary>
    public class CalcPriceMatFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 5);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> price per $100 face value of a security that pays interest at maturity.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 5 - 6 items: settlement, maturity, issue, rate, yld, [basis].
        /// </para>
        /// <para>
        /// Settlement is the security's settlement date.
        /// The security settlement date is the date after the issue date when the security is traded to the buyer.
        /// </para>
        /// <para>
        /// Maturity is the security's maturity date.
        /// The maturity date is the date when the security expires.
        /// </para>
        /// <para>
        /// Issue is the security's issue date, expressed as a serial date number.
        /// </para>
        /// <para>
        /// Rate is the security's interest rate at date of issue.
        /// </para>
        /// <para>
        /// Yld is the security's annual yield.
        /// </para>
        /// <para>
        /// Basis is the type of day count basis to use.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            DateTime time3 = CalcConvert.ToDateTime(args[2]);
            double num = CalcConvert.ToDouble(args[3]);
            double num2 = CalcConvert.ToDouble(args[4]);
            int basis = CalcHelper.ArgumentExists(args, 5) ? CalcConvert.ToInt(args[5]) : 0;
            if (DateTime.Compare(time, time2) >= 0)
            {
                return CalcErrors.Number;
            }
            if (((num < 0.0) || (num2 < 0.0)) || ((basis < 0) || (4 < basis)))
            {
                return CalcErrors.Number;
            }
            double num6 = FinancialHelper.days_monthly_basis(time, time2, basis);
            double num7 = FinancialHelper.days_monthly_basis(time3, time2, basis);
            double num4 = FinancialHelper.days_monthly_basis(time3, time, basis);
            double num5 = FinancialHelper.annual_year_basis(time, basis);
            if (((num4 <= 0.0) || (num5 <= 0.0)) || ((num6 <= 0.0) || (num7 <= 0.0)))
            {
                return CalcErrors.Number;
            }
            double num8 = 1.0 + ((num6 / num5) * num2);
            if (num8 == 0.0)
            {
                return CalcErrors.Number;
            }
            return (double) (((100.0 + (((num7 / num5) * num) * 100.0)) / num8) - (((num4 / num5) * num) * 100.0));
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
                return 6;
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
                return 5;
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
                return "PRICEMAT";
            }
        }
    }
}

