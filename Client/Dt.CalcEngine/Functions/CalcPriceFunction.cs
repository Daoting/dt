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
    /// Returns the <see cref="T:System.Double" /> price per $100 face value of a security that pays periodic interest.
    /// </summary>
    public class CalcPriceFunction : CalcBuiltinFunction
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
            return (i == 6);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> price per $100 face value of a security that pays periodic interest.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 6 - 7 items: settlement, maturity, rate, yld, redemption, frequency, [basis].
        /// </para>
        /// <para>
        /// Settlement is the security's settlement date.
        /// The security settlement date is the date after the issue date when the security is traded to the buyer.
        /// </para>
        /// <para>
        /// Maturity is the security's maturity date. The maturity date is the date when the security expires.
        /// </para>
        /// <para>
        /// Rate is the security's annual coupon rate.
        /// </para>
        /// <para>
        /// Yld is the security's annual yield.
        /// </para>
        /// <para>
        /// Redemption is the security's redemption value per $100 face value.
        /// </para>
        /// <para>
        /// Frequency is the number of coupon payments per year. For annual payments, frequency = 1; for semiannual, frequency = 2; for quarterly, frequency = 4.
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
            double rate = CalcConvert.ToDouble(args[2]);
            double yield = CalcConvert.ToDouble(args[3]);
            double redemption = CalcConvert.ToDouble(args[4]);
            int freq = CalcConvert.ToInt(args[5]);
            int basis = CalcHelper.ArgumentExists(args, 6) ? CalcConvert.ToInt(args[6]) : 0;
            if (((yield < 0.0) || (rate < 0.0)) || (redemption == 0.0))
            {
                return CalcErrors.Number;
            }
            if ((basis < 0) || (basis > 4))
            {
                return CalcErrors.Number;
            }
            if (((freq != 1) && (freq != 2)) && (freq != 4))
            {
                return CalcErrors.Number;
            }
            if (DateTime.Compare(time, time2) > 0)
            {
                return CalcErrors.Number;
            }
            return (double) FinancialHelper.price(time, time2, rate, yield, redemption, freq, basis);
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
                return 7;
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
                return 6;
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
                return "PRICE";
            }
        }
    }
}

