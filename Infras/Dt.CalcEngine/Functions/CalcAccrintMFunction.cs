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
    /// Returns the <see cref="T:System.Double" /> accrued interest for a security that pays interest at maturity.
    /// </summary>
    public class CalcAccrintMFunction : CalcBuiltinFunction
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
            if (i != 3)
            {
                return (i == 4);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> accrued interest for a security that pays interest at maturity.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 5 items: issue, settlement, rate, [par], [basis].
        /// </para>
        /// <para>
        /// Issue is the security's issue date.
        /// </para>
        /// <para>
        /// Settlement is the security's maturity date.
        /// </para>
        /// <para>
        /// Rate is the security's annual coupon rate.
        /// </para>
        /// <para>
        /// Par is the security's par value. If you omit par, ACCRINTM uses $1,000.
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
            double num2 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToDouble(args[3]) : 1000.0;
            int basis = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 0;
            if ((((num > 0.0) && (num2 > 0.0)) && ((basis >= 0) && (4 >= basis))) && (DateTime.Compare(time, time2) <= 0))
            {
                double num4 = FinancialHelper.days_monthly_basis(time, time2, basis);
                double num5 = FinancialHelper.annual_year_basis(time, basis);
                if ((num4 >= 0.0) && (num5 > 0.0))
                {
                    return (double) (((num2 * num) * num4) / num5);
                }
            }
            return CalcErrors.Number;
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
                return "ACCRINTM";
            }
        }
    }
}

