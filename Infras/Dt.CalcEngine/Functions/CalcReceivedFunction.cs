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
    /// Returns the <see cref="T:System.Double" /> amount received at maturity for a fully invested security.
    /// </summary>
    public class CalcReceivedFunction : CalcBuiltinFunction
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
            return (i == 4);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> amount received at maturity for a fully invested security.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 - 5 items: settlement, maturity, investment, discount, [basis].
        /// </para>
        /// <para>
        /// Settlement is the security's settlement date.
        /// The security settlement date is the date after the issue date when the security is traded to the buyer.
        /// </para>
        /// <para>
        /// Maturity is the security's maturity date. The maturity date is the date when the security expires.
        /// </para>
        /// <para>
        /// Investment is the amount invested in the security.
        /// </para>
        /// <para>
        /// Discount is the security's discount rate.
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
            double num = CalcConvert.ToDouble(args[2]);
            double num2 = CalcConvert.ToDouble(args[3]);
            int basis = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 0;
            if (DateTime.Compare(time, time2) >= 0)
            {
                return CalcErrors.Number;
            }
            if (((num <= 0.0) || (num2 <= 0.0)) || ((basis < 0) || (4 < basis)))
            {
                return CalcErrors.Number;
            }
            double num4 = FinancialHelper.days_monthly_basis(time, time2, basis);
            double num5 = FinancialHelper.annual_year_basis(time, basis);
            if ((num4 <= 0.0) || (num5 <= 0.0))
            {
                return CalcErrors.Number;
            }
            double num6 = 1.0 - ((num2 * num4) / num5);
            if (num6 <= 0.0)
            {
                return CalcErrors.Number;
            }
            return (double) (num / num6);
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
                return 5;
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
                return "RECEIVED";
            }
        }
    }
}

