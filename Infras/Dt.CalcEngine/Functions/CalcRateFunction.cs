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
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine.Functions
{
    /// <summary>
    /// Returns the <see cref="T:System.Double" /> interest rate per period of an annuity. 
    /// </summary>
    public class CalcRateFunction : CalcBuiltinFunction
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
            if ((i != 3) && (i != 4))
            {
                return (i == 5);
            }
            return true;
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> interest rate per period of an annuity.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 - 6 items: nper, pmt, pv, [fv], [type], [guess].
        /// </para>
        /// <para>
        /// Nper is the total number of payment periods in an annuity.
        /// </para>
        /// <para>
        /// Pmt is the payment made each period and cannot change over the life of the annuity.
        /// </para>
        /// <para>
        /// Pv is the present value ¡ª the total amount that a series of future payments is worth now.
        /// </para>
        /// <para>
        /// Fv is the future value, or a cash balance you want to attain after the last payment is made.
        /// If fv is omitted, it is assumed to be 0 (the future value of a loan, for example, is 0).
        /// </para>
        /// <para>
        /// Type is the number 0 or 1 and indicates when payments are due.
        /// </para>
        /// <para>
        /// Guess is your guess for what the rate will be.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double num = CalcConvert.ToDouble(args[0]);
            double num2 = CalcConvert.ToDouble(args[1]);
            double num3 = CalcConvert.ToDouble(args[2]);
            double num4 = CalcHelper.ArgumentExists(args, 3) ? CalcConvert.ToDouble(args[3]) : 0.0;
            int num5 = CalcHelper.ArgumentExists(args, 4) ? CalcConvert.ToInt(args[4]) : 0;
            double num6 = CalcHelper.ArgumentExists(args, 5) ? CalcConvert.ToDouble(args[5]) : 0.1;
            if (num > 0.0)
            {
                GoalSeekData data;
                RateData data2;
                if (num5 < 0)
                {
                    return CalcErrors.Value;
                }
                if (num5 > 1)
                {
                    num5 = 1;
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
                data.xmin = Math.Max(data.xmin, -Math.Pow(1.7976931348623157E+298, 1.0 / ((double) num)) + 1.0);
                data.xmax = Math.Min(data.xmax, Math.Pow(1.7976931348623157E+298, 1.0 / ((double) num)) - 1.0);
                data2.nper = (double) num;
                data2.pmt = (double) num2;
                data2.pv = (double) num3;
                data2.fv = (double) num4;
                data2.type = num5;
                bool flag = this.goal_seek_newton(ref data, ref data2, (double) num6);
                if (!flag)
                {
                    for (int i = 2; (!data.havexneg || !data.havexpos) && (i < 100); i *= 2)
                    {
                        this.goal_seek_point(ref data, ref data2, ((double) num6) * i);
                        this.goal_seek_point(ref data, ref data2, ((double) num6) / ((double) i));
                    }
                    flag = this.goal_seek_bisection(ref data, ref data2);
                }
                if (flag)
                {
                    return (double) data.root;
                }
            }
            return CalcErrors.Number;
        }

        internal bool fake_df(double x, ref double dfx, double xstep, ref GoalSeekData data, ref RateData user_data)
        {
            double y = 0.0;
            double num4 = 0.0;
            double rate = x - xstep;
            if (rate < data.xmin)
            {
                rate = x;
            }
            double num2 = x + xstep;
            if (num2 > data.xmax)
            {
                num2 = x;
            }
            if (rate == num2)
            {
                return false;
            }
            bool flag = this.rate_f(rate, ref y, ref user_data);
            if (!flag)
            {
                return flag;
            }
            flag = this.rate_f(num2, ref num4, ref user_data);
            if (!flag)
            {
                return flag;
            }
            dfx = (num4 - y) / (num2 - rate);
            return true;
        }

        internal bool goal_seek_bisection(ref GoalSeekData data, ref RateData user_data)
        {
            int num3 = 0;
            if (data.havexpos && data.havexneg)
            {
                double num2 = Math.Abs((double) (data.xpos - data.xneg)) / (Math.Abs(data.xpos) + Math.Abs(data.xneg));
                for (int i = 0; i < 160; i++)
                {
                    double xneg;
                    double xpos;
                    double yneg;
                    double num11;
                    double y = 0.0;
                    int num6 = 0;
                    num6 = ((i % 4) == 0) ? 1 : (((i % 4) == 2) ? 2 : 3);
                Label_0068:
                    switch (num6)
                    {
                        case 0:
                            xneg = data.xpos - (data.ypos * ((data.xneg - data.xpos) / (data.yneg - data.ypos)));
                            goto Label_0274;

                        case 1:
                            xneg = (data.xpos + data.xneg) / 2.0;
                            if (!this.rate_f(xneg, ref y, ref user_data))
                            {
                                continue;
                            }
                            if (y != 0.0)
                            {
                                break;
                            }
                            FinancialHelper.update_data(xneg, y, ref data);
                            return true;

                        case 2:
                            yneg = 0.0;
                            num11 = 0.0;
                            if (num2 <= 0.1)
                            {
                                goto Label_017F;
                            }
                            num6 = 3;
                            goto Label_0068;

                        case 3:
                            xneg = (data.xpos + data.xneg) / 2.0;
                            goto Label_0274;

                        default:
                            return false;
                    }
                    double num7 = Math.Sqrt((y * y) - (data.ypos * data.yneg));
                    if (num7 == 0.0)
                    {
                        continue;
                    }
                    xneg += ((xneg - data.xpos) * y) / num7;
                    goto Label_0274;
                Label_017F:
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
                            if (!this.rate_f(xpos, ref yneg, ref user_data))
                            {
                                continue;
                            }
                            break;
                    }
                    double xstep = Math.Abs((double) (data.xpos - data.xneg)) / 1000000.0;
                    if (!this.fake_df(xpos, ref num11, xstep, ref data, ref user_data) || (num11 == 0.0))
                    {
                        continue;
                    }
                    xneg = xpos - ((1.01 * yneg) / num11);
                    if (((xneg < data.xpos) && (xneg < data.xneg)) || ((xneg > data.xpos) && (xneg > data.xneg)))
                    {
                        continue;
                    }
                Label_0274:
                    if (this.rate_f(xneg, ref y, ref user_data))
                    {
                        if (FinancialHelper.update_data(xneg, y, ref data))
                        {
                            return true;
                        }
                        num2 = Math.Abs((double) (data.xpos - data.xneg)) / (Math.Abs(data.xpos) + Math.Abs(data.xneg));
                        if (num2 < data.precision)
                        {
                            if (data.yneg < y)
                            {
                                y = data.yneg;
                            }
                            xneg = data.xneg;
                            if (data.ypos < y)
                            {
                                y = data.ypos;
                            }
                            xneg = data.xpos;
                            data.root = xneg;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool goal_seek_newton(ref GoalSeekData data, ref RateData user_data, double x0)
        {
            double num2 = data.precision / 2.0;
            for (int i = 0; i < 100; i++)
            {
                double y = 0.0;
                double num6 = 0.0;
                if ((x0 < data.xmin) || (x0 > data.xmax))
                {
                    return false;
                }
                bool flag = this.rate_f(x0, ref y, ref user_data);
                if (!flag)
                {
                    return flag;
                }
                if (FinancialHelper.update_data(x0, y, ref data))
                {
                    return true;
                }
                flag = this.rate_df(x0, ref num6, ref user_data);
                if (!flag)
                {
                    return flag;
                }
                if (num6 == 0.0)
                {
                    return false;
                }
                double num3 = x0 - ((1.000001 * y) / num6);
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

        internal bool goal_seek_point(ref GoalSeekData data, ref RateData user_data, double x0)
        {
            double y = 0.0;
            if ((x0 < data.xmin) || (x0 > data.xmax))
            {
                return false;
            }
            bool flag = this.rate_f(x0, ref y, ref user_data);
            if (!flag)
            {
                return flag;
            }
            return FinancialHelper.update_data(x0, y, ref data);
        }

        internal bool rate_df(double rate, ref double y, ref RateData user_data)
        {
            if ((rate > -1.0) && (rate != 0.0))
            {
                RateData data = user_data;
                double num = Math.Pow(1.0 + rate, ((double) data.nper) - 1.0);
                double num2 = (Math.Pow(1.0 + rate, data.nper) - 1.0) / rate;
                y = ((-data.pmt * num2) / rate) + ((num * data.nper) * (data.pv + (data.pmt * (data.type + (1.0 / rate)))));
                return true;
            }
            return false;
        }

        internal bool rate_f(double rate, ref double y, ref RateData user_data)
        {
            if ((rate > -1.0) && (rate != 0.0))
            {
                RateData data = user_data;
                double num = Math.Pow(1.0 + rate, data.nper);
                double num2 = (Math.Pow(1.0 + rate, data.nper) - 1.0) / rate;
                y = ((data.pv * num) + ((data.pmt * (1.0 + (rate * data.type))) * num2)) + data.fv;
                return true;
            }
            return false;
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
                return "RATE";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RateData
        {
            public double nper;
            public double pmt;
            public double pv;
            public double fv;
            public int type;
        }
    }
}

