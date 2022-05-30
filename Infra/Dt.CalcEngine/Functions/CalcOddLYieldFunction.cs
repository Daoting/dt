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
    /// Returns the <see cref="T:System.Double" /> yield of a security that has an odd (short or long) last period.
    /// </summary>
    public class CalcOddLYieldFunction : CalcBuiltinFunction
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
            return (i == 7);
        }

        private double calc_oddlyield(DateTime settlement, DateTime maturity, DateTime last_interest, double rate, double price, double redemption, int freq, int basis)
        {
            DateTime time = new DateTime(last_interest.Year, last_interest.Month, last_interest.Day);
            do
            {
                time = time.AddMonths(12 / freq);
            }
            while (DateTime.Compare(time, maturity) < 0);
            double num = FinancialHelper.date_ratio(last_interest, settlement, time, freq, basis);
            double num2 = FinancialHelper.date_ratio(last_interest, maturity, time, freq, basis);
            double num3 = FinancialHelper.date_ratio(settlement, maturity, time, freq, basis);
            return (((freq * (redemption - price)) + ((100.0 * rate) * (num2 - num))) / ((num3 * price) + ((((100.0 * rate) * num) * num3) / ((double) freq))));
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> yield of a security that has an odd (short or long) last period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 7 - 8 items: settlement, maturity, last_interest, rate, pr, redemption, frequency, [basis].
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
        /// Last_interest is the security's last coupon date.
        /// </para>
        /// <para>
        /// Rate is the security's interest rate.
        /// </para>
        /// <para>
        /// Pr is the security's price.
        /// </para>
        /// <para>
        /// Redemption is the security's redemption value per $100 face value.
        /// </para>
        /// <para>
        /// Frequency is the number of coupon payments per year.
        /// For annual payments, frequency = 1; for semiannual, frequency = 2; for quarterly, frequency = 4.
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
            double rate = CalcConvert.ToDouble(args[3]);
            double price = CalcConvert.ToDouble(args[4]);
            double redemption = CalcConvert.ToDouble(args[5]);
            int freq = CalcConvert.ToInt(args[6]);
            int basis = CalcHelper.ArgumentExists(args, 7) ? CalcConvert.ToInt(args[7]) : 0;
            if (((((basis >= 0) && (basis <= 4)) && (((freq == 1) || (freq == 2)) || (freq == 4))) && ((DateTime.Compare(time, time2) <= 0) && (DateTime.Compare(time3, time) <= 0))) && (((rate >= 0.0) && (price >= 0.0)) && (redemption > 0.0)))
            {
                return (double) this.calc_oddlyield(time, time2, time3, rate, price, redemption, freq, basis);
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
                return 8;
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
                return 7;
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
                return "ODDLYIELD";
            }
        }
    }
}

