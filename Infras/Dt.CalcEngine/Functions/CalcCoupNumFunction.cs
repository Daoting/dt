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
    /// Returns the <see cref="T:System.Double" /> number of coupons payable between the settlement date and maturity date, rounded up to the nearest whole coupon.
    /// </summary>
    public class CalcCoupNumFunction : CalcBuiltinFunction
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
            return (i == 3);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Int32" /> number of coupons payable between the settlement date and maturity date, rounded up to the nearest whole coupon.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 4 items: settlement, maturity, frequency, [basis].
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
        /// Frequency is the number of coupon payments per year.
        /// For annual payments, frequency = 1; for semiannual, frequency = 2; for quarterly, frequency = 4.
        /// </para>
        /// <para>
        /// Basis is the type of day count basis to use.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            int freq = CalcConvert.ToInt(args[2]);
            int num2 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToInt(args[3]) : 0;
            if ((num2 < 0) || (num2 > 4))
            {
                return CalcErrors.Number;
            }
            if (((freq != 1) && (freq != 2)) && (freq != 4))
            {
                return CalcErrors.Number;
            }
            if (DateTime.Compare(time, time2) >= 0)
            {
                return CalcErrors.Number;
            }
            return (int) FinancialHelper.coupnum(time, time2, freq);
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
                return "COUPNUM";
            }
        }
    }
}

