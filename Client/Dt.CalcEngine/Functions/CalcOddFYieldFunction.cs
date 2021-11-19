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
    /// Returns the <see cref="T:System.Double" /> yield of a security that has an odd (short or long) first period.
    /// </summary>
    public class CalcOddFYieldFunction : CalcBuiltinFunction
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
            return (i == 8);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> yield of a security that has an odd (short or long) first period.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 8 - 9 items: settlement, maturity, issue, first_coupon, rate, pr, redemption, frequency, [basis].
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
        /// Issue is the security's issue date.
        /// </para>
        /// <para>
        /// First_coupon is the security's first coupon date.
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
        /// Basis   is the type of day count basis to use.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            GoalSeekData data;
            base.CheckArgumentsLength(args);
            DateTime time = CalcConvert.ToDateTime(args[0]);
            DateTime time2 = CalcConvert.ToDateTime(args[1]);
            DateTime time3 = CalcConvert.ToDateTime(args[2]);
            DateTime time4 = CalcConvert.ToDateTime(args[3]);
            double rate = CalcConvert.ToDouble(args[4]);
            double price = CalcConvert.ToDouble(args[5]);
            double redem = CalcConvert.ToDouble(args[6]);
            int freq = CalcConvert.ToInt(args[7]);
            int basis = CalcHelper.ArgumentExists(args, 8) ? CalcConvert.ToInt(args[8]) : 0;
            double num6 = 0.1;
            if ((((basis < 0) || (basis > 4)) || (((freq != 1) && (freq != 2)) && (freq != 4))) || (((DateTime.Compare(time3, time) > 0) || (DateTime.Compare(time, time4) > 0)) || (DateTime.Compare(time4, time2) > 0)))
            {
                return CalcErrors.Number;
            }
            if (((rate < 0.0) || (price <= 0.0)) || (redem <= 0.0))
            {
                return CalcErrors.Number;
            }
            data.xmin = 0.0;
            data.xmax = 0.0;
            data.precision = 0.0;
            data.havexpos = false;
            data.xpos = 0.0;
            data.ypos = 0.0;
            data.havexneg = false;
            data.xneg = 0.0;
            data.yneg = 0.0;
            data.root = 0.0;
            FinancialHelper.goal_seek_initialise(ref data);
            data.xmin = Math.Max(data.xmin, 0.0);
            data.xmax = Math.Min(data.xmax, 1000.0);
            bool flag = this.goal_seek_newton(ref data, time, time2, time3, time4, rate, price, redem, freq, basis, num6);
            if (flag)
            {
                for (num6 = 1E-10; num6 < data.xmax; num6 *= 2.0)
                {
                    this.goal_seek_point(ref data, time, time2, time3, time4, rate, price, redem, freq, basis, num6);
                }
                flag = this.goal_seek_bisection(ref data, time, time2, time3, time4, rate, price, redem, freq, basis);
            }
            if (!flag)
            {
                return CalcErrors.Number;
            }
            return (double) data.root;
        }

        internal bool fake_df(double x, ref double dfx, double xstep, ref GoalSeekData data, DateTime settle, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double price, double redemption, int freq, int basis)
        {
            double y = 0.0;
            double num4 = 0.0;
            double yield = x - xstep;
            if (yield < data.xmin)
            {
                yield = x;
            }
            double num2 = x + xstep;
            if (num2 > data.xmax)
            {
                num2 = x;
            }
            if (yield == num2)
            {
                return false;
            }
            bool flag = this.oddyield_f(yield, ref y, settle, maturity, issue, first_coupon, rate, price, redemption, freq, basis);
            if (!flag)
            {
                return flag;
            }
            flag = this.oddyield_f(num2, ref num4, settle, maturity, issue, first_coupon, rate, price, redemption, freq, basis);
            if (!flag)
            {
                return flag;
            }
            dfx = (num4 - y) / (num2 - yield);
            return true;
        }

        private bool goal_seek_bisection(ref GoalSeekData data, DateTime settle, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double price, double redem, int freq, int basis)
        {
            int num3 = 0;
            if (data.havexpos && data.havexneg)
            {
                double num2 = Math.Abs((double) (data.xpos - data.xneg)) / (Math.Abs(data.xpos) + Math.Abs(data.xneg));
                for (int i = 0; i < 0x6c; i++)
                {
                    double xpos;
                    double yneg;
                    double num10;
                    double num11;
                    double yield = 0.0;
                    double y = 0.0;
                    int num6 = ((i % 4) == 0) ? 1 : (((i % 4) == 2) ? 2 : 3);
                Label_006F:
                    switch (num6)
                    {
                        case 0:
                            yield = data.xpos - (data.ypos * ((data.xneg - data.xpos) / (data.yneg - data.ypos)));
                            goto Label_02C1;

                        case 1:
                            yield = (data.xpos + data.xneg) / 2.0;
                            if (!this.oddyield_f(yield, ref y, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis))
                            {
                                continue;
                            }
                            if (y != 0.0)
                            {
                                break;
                            }
                            FinancialHelper.update_data(yield, y, ref data);
                            return true;

                        case 2:
                            xpos = 0.0;
                            yneg = 0.0;
                            num10 = 0.0;
                            num11 = 0.0;
                            if (num2 <= 0.1)
                            {
                                goto Label_01AE;
                            }
                            num6 = 3;
                            goto Label_006F;

                        case 3:
                            yield = (data.xpos + data.xneg) / 2.0;
                            goto Label_02C1;

                        default:
                            goto Label_02C1;
                    }
                    double num7 = Math.Sqrt((y * y) - (data.ypos * data.yneg));
                    if (num7 == 0.0)
                    {
                        continue;
                    }
                    yield += ((yield - data.xpos) * y) / num7;
                    goto Label_02C1;
                Label_01AE:
                    switch ((num3++ % 4))
                    {
                        case 0:
                            xpos = data.xpos;
                            xpos = data.ypos;
                            break;

                        case 2:
                            xpos = data.xneg;
                            yneg = data.yneg;
                            break;

                        default:
                            xpos = (data.xpos + data.xneg) / 2.0;
                            if (!this.oddyield_f(xpos, ref yneg, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis))
                            {
                                continue;
                            }
                            break;
                    }
                    num10 = Math.Abs((double) (data.xpos - data.xneg)) / 1000000.0;
                    if (!this.fake_df(xpos, ref num11, num10, ref data, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis) || (num11 == 0.0))
                    {
                        continue;
                    }
                    yield = xpos - ((1.01 * yneg) / num11);
                    if (((yield < data.xpos) && (yield < data.xneg)) || ((yield > data.xpos) && (yield > data.xneg)))
                    {
                        continue;
                    }
                Label_02C1:
                    if (this.oddyield_f(yield, ref y, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis))
                    {
                        if (FinancialHelper.update_data(yield, y, ref data))
                        {
                            return true;
                        }
                        num2 = Math.Abs((double) (data.xpos - data.xneg)) / (Math.Abs(data.xpos) + Math.Abs(data.xneg));
                        if (num2 < data.precision)
                        {
                            if (data.yneg < y)
                            {
                                y = data.yneg;
                                yield = data.xneg;
                            }
                            if (data.ypos < y)
                            {
                                y = data.ypos;
                                yield = data.xpos;
                            }
                            data.root = yield;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool goal_seek_newton(ref GoalSeekData data, DateTime settle, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double price, double redem, int freq, int basis, double x0)
        {
            double num2 = data.precision / 2.0;
            for (int i = 0; i < 20; i++)
            {
                double num7;
                double y = 0.0;
                double dfx = 0.0;
                if ((x0 < data.xmin) || (x0 > data.xmax))
                {
                    return false;
                }
                bool flag = this.oddyield_f(x0, ref y, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis);
                if (!flag)
                {
                    return flag;
                }
                if (FinancialHelper.update_data(x0, y, ref data))
                {
                    return true;
                }
                if (Math.Abs(x0) < 1E-10)
                {
                    if (data.havexneg && data.havexpos)
                    {
                        num7 = Math.Abs((double) (data.xpos - data.xneg)) / 1000000.0;
                    }
                    else
                    {
                        num7 = (data.xmax - data.xmin) / 1000000.0;
                    }
                }
                else
                {
                    num7 = Math.Abs(x0) / 1000000.0;
                }
                flag = this.fake_df(x0, ref dfx, num7, ref data, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis);
                if (!flag)
                {
                    return flag;
                }
                if (dfx == 0.0)
                {
                    return false;
                }
                double num3 = x0 - ((1.000001 * y) / dfx);
                if (num3 == x0)
                {
                    data.root = x0;
                    return true;
                }
                double num4 = Math.Abs((double) (num3 - x0)) / (Math.Abs(x0) + Math.Abs(num3));
                x0 = num3;
                if (num4 < num2)
                {
                    data.root = x0;
                    return true;
                }
            }
            return false;
        }

        private bool goal_seek_point(ref GoalSeekData data, DateTime settle, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double price, double redem, int freq, int basis, double x0)
        {
            double y = 0.0;
            if ((x0 < data.xmin) || (x0 > data.xmax))
            {
                return false;
            }
            bool flag = this.oddyield_f(x0, ref y, settle, maturity, issue, first_coupon, rate, price, redem, freq, basis);
            if (!flag)
            {
                return flag;
            }
            return FinancialHelper.update_data(x0, y, ref data);
        }

        private bool oddyield_f(double yield, ref double y, DateTime settlement, DateTime maturity, DateTime issue, DateTime first_coupon, double rate, double price, double redemption, int freq, int basis)
        {
            y = FinancialHelper.calc_oddfprice(settlement, maturity, issue, first_coupon, rate, yield, redemption, freq, basis) - price;
            return true;
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
                return 9;
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
                return 8;
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
                return "ODDFYIELD";
            }
        }
    }
}

