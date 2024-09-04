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
    /// Returns the <see cref="T:System.Double" /> internal rate of return for a schedule of cash flows that is not necessarily periodic. 
    /// </summary>
    public class CalcXIrrFunction : CalcBuiltinFunction
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
            return (i != 2);
        }

        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 2);
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
            return (i != 2);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> internal rate of return for a schedule of cash flows that is not necessarily periodic.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 - 3 items: values, dates, [guess].
        /// </para>
        /// <para>
        /// Values is a series of cash flows that corresponds to a schedule of payments in dates.
        /// </para>
        /// <para>
        /// Dates is a schedule of payment dates that corresponds to the cash flow payments.
        /// </para>
        /// <para>
        /// Guess is a number that you guess is close to the result of XIRR.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            double[] values = new double[ArrayHelper.GetLength(args[0], 0)];
            DateTime[] dates = new DateTime[ArrayHelper.GetLength(args[1], 0)];
            double num = CalcHelper.ArgumentExists(args, 2) ? CalcConvert.ToDouble(args[2]) : 0.1;
            if (ArrayHelper.GetLength(args[0], 0) == ArrayHelper.GetLength(args[1], 0))
            {
                GoalSeekData data;
                for (int i = 0; i < ArrayHelper.GetLength(args[0], 0); i++)
                {
                    values[i] = CalcConvert.ToDouble(ArrayHelper.GetValue(args[0], i, 0));
                }
                for (int j = 0; j < ArrayHelper.GetLength(args[1], 0); j++)
                {
                    dates[j] = CalcConvert.ToDateTime(ArrayHelper.GetValue(args[1], j, 0));
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
                data.xmin = -1.0;
                data.xmax = Math.Min(1000.0, data.xmax);
                if (this.goal_seek_newton(ref data, dates, values, num))
                {
                    return (double) data.root;
                }
                for (int k = 1; k <= 0x400; k += k)
                {
                    double num5 = Convert.ToDouble(k);
                    double y = 9.0 / (num5 + 9.0);
                    this.goal_seek_point(ref data, num, ref y, dates, values);
                    y = num5;
                    this.goal_seek_point(ref data, num, ref y, dates, values);
                    if (this.goal_seek_bisection(ref data, dates, values))
                    {
                        return (double) data.root;
                    }
                }
            }
            return CalcErrors.Number;
        }

        internal bool fake_df(double x, ref double dfx, double xstep, ref GoalSeekData data, DateTime[] dates, double[] values)
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
            bool flag = xirr_f(rate, ref y, dates, values);
            if (!flag)
            {
                return flag;
            }
            flag = xirr_f(num2, ref num4, dates, values);
            if (!flag)
            {
                return flag;
            }
            dfx = (num4 - y) / (num2 - rate);
            return true;
        }

        private bool goal_seek_bisection(ref GoalSeekData data, DateTime[] dates, double[] values)
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
                    double rate = 0.0;
                    double y = 0.0;
                    int num6 = ((i % 4) == 0) ? 1 : (((i % 4) == 2) ? 2 : 3);
                Label_006F:
                    switch (num6)
                    {
                        case 0:
                            rate = data.xpos - (data.ypos * ((data.xneg - data.xpos) / (data.yneg - data.ypos)));
                            goto Label_0295;

                        case 1:
                            rate = (data.xpos + data.xneg) / 2.0;
                            if (!xirr_f(rate, ref y, dates, values))
                            {
                                continue;
                            }
                            if (y != 0.0)
                            {
                                break;
                            }
                            FinancialHelper.update_data(rate, y, ref data);
                            return true;

                        case 2:
                            xpos = 0.0;
                            yneg = 0.0;
                            num10 = 0.0;
                            num11 = 0.0;
                            if (num2 <= 0.1)
                            {
                                goto Label_019F;
                            }
                            num6 = 3;
                            goto Label_006F;

                        case 3:
                            rate = (data.xpos + data.xneg) / 2.0;
                            goto Label_0295;

                        default:
                            goto Label_0295;
                    }
                    double num7 = Math.Sqrt((y * y) - (data.ypos * data.yneg));
                    if (num7 == 0.0)
                    {
                        continue;
                    }
                    rate += ((rate - data.xpos) * y) / num7;
                    goto Label_0295;
                Label_019F:
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
                            if (!xirr_f(xpos, ref yneg, dates, values))
                            {
                                continue;
                            }
                            break;
                    }
                    num10 = Math.Abs((double) (data.xpos - data.xneg)) / 1000000.0;
                    if (!this.fake_df(xpos, ref num11, num10, ref data, dates, values) || (num11 == 0.0))
                    {
                        continue;
                    }
                    rate = xpos - ((1.01 * yneg) / num11);
                    if (((rate < data.xpos) && (rate < data.xneg)) || ((rate > data.xpos) && (rate > data.xneg)))
                    {
                        continue;
                    }
                Label_0295:
                    if (xirr_f(rate, ref y, dates, values))
                    {
                        if (FinancialHelper.update_data(rate, y, ref data))
                        {
                            return true;
                        }
                        num2 = Math.Abs((double) (data.xpos - data.xneg)) / (Math.Abs(data.xpos) + Math.Abs(data.xneg));
                        if (num2 < data.precision)
                        {
                            if (data.yneg < y)
                            {
                                y = data.yneg;
                                rate = data.xneg;
                            }
                            if (data.ypos < y)
                            {
                                y = data.ypos;
                                rate = data.xpos;
                            }
                            data.root = rate;
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        internal bool goal_seek_newton(ref GoalSeekData data, DateTime[] dates, double[] values, double x0)
        {
            double num2 = data.precision / 2.0;
            for (int i = 0; i < 20; i++)
            {
                double num7;
                double y = 0.0;
                double dfx = 0.0;
                bool flag = xirr_f(x0, ref y, dates, values);
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
                flag = this.fake_df(x0, ref dfx, num7, ref data, dates, values);
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

        internal bool goal_seek_point(ref GoalSeekData data, double x0, ref double y, DateTime[] dates, double[] values)
        {
            double num = 0.0;
            if ((x0 < data.xmin) || (x0 > data.xmax))
            {
                return false;
            }
            bool flag = xirr_f(x0, ref y, dates, values);
            if (!flag)
            {
                return flag;
            }
            return FinancialHelper.update_data(x0, num, ref data);
        }

        private static bool xirr_f(double rate, ref double y, DateTime[] dates, double[] values)
        {
            double d = 0.0;
            int length = values.Length;
            for (int i = 0; i < length; i++)
            {
                double num4 = dates[i].ToOADate() - dates[0].ToOADate();
                if (num4 < 0.0)
                {
                    return false;
                }
                d += values[i] / MathHelper.pow1p(rate, num4 / 365.0);
            }
            if (double.IsPositiveInfinity(d))
            {
                y = double.MaxValue;
            }
            else if (double.IsNegativeInfinity(d))
            {
                y = double.MinValue;
            }
            else if (double.IsNaN(d))
            {
                y = double.Epsilon;
            }
            else
            {
                y = d;
            }
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
                return 2;
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
                return "XIRR";
            }
        }
    }
}

