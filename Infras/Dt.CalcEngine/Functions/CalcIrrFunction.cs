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
    /// Returns the <see cref="T:System.Double" /> internal rate of return for a series of cash flows represented by the numbers in values. 
    /// </summary>
    public class CalcIrrFunction : CalcBuiltinFunction
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
            return (i == 0);
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
            return (i == 1);
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
            return (i == 0);
        }

        /// <summary>
        /// Returns the <see cref="T:System.Double" /> internal rate of return for a series of cash flows represented by the numbers in values.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: values, [guess].
        /// </para>
        /// <para>
        /// Values is an array or a reference to cells that contain numbers for which you want to calculate the internal rate of return.
        /// </para>
        /// <para>
        /// Guess is a number that you guess is close to the result of IRR.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            GoalSeekData data;
            base.CheckArgumentsLength(args);
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
            object o = args[0];
            double num = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToDouble(args[1]) : 0.1;
            if (Math.Abs(num) > 1.0)
            {
                num = 0.1;
            }
            int length = ArrayHelper.GetLength(o, 0);
            if (ArrayHelper.IsArrayOrRange(o) && (length >= 2))
            {
                double[] vals = new double[length];
                data.xmin = -1.0;
                data.xmax = Math.Min(data.xmax, Math.Pow(1.7976931348623157E+298, 1.0 / ((double) length)) - 1.0);
                bool flag = false;
                bool flag2 = false;
                for (int i = 0; i < length; i++)
                {
                    object obj3 = ArrayHelper.GetValue(o, i, 0);
                    if (CalcConvert.IsNumber(obj3))
                    {
                        double num4 = CalcConvert.ToDouble(ArrayHelper.GetValue(o, i, 0));
                        vals[i] = num4;
                        if (num4 > 0.0)
                        {
                            flag = true;
                        }
                        if (num4 < 0.0)
                        {
                            flag2 = true;
                        }
                    }
                    else if (obj3 is CalcError)
                    {
                        return obj3;
                    }
                }
                if (!flag || !flag2)
                {
                    return CalcErrors.Number;
                }
                bool flag3 = this.goal_seek_newton(ref data, vals, (double) num);
                if (!flag3)
                {
                    for (int j = 2; (!data.havexneg || !data.havexpos) && (j < 100); j *= 2)
                    {
                        this.goal_seek_point(ref data, vals, ((double) num) * j);
                        this.goal_seek_point(ref data, vals, ((double) num) / ((double) j));
                    }
                    flag3 = this.goal_seek_bisection(ref data, vals);
                }
                if (flag3)
                {
                    return (double) data.root;
                }
            }
            return CalcErrors.Number;
        }

        internal bool fake_df(double x, ref double dfx, double xstep, ref GoalSeekData data, double[] vals)
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
            bool flag = this.irr_npv(rate, ref y, vals);
            if (!flag)
            {
                return flag;
            }
            flag = this.irr_npv(num2, ref num4, vals);
            if (!flag)
            {
                return flag;
            }
            dfx = (num4 - y) / (num2 - rate);
            return true;
        }

        internal bool goal_seek_bisection(ref GoalSeekData data, double[] vals)
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
                            if (!this.irr_npv(xneg, ref y, vals))
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
                            if (!this.irr_npv(xpos, ref yneg, vals))
                            {
                                continue;
                            }
                            break;
                    }
                    double xstep = Math.Abs((double) (data.xpos - data.xneg)) / 1000000.0;
                    if (!this.fake_df(xpos, ref num11, xstep, ref data, vals) || (num11 == 0.0))
                    {
                        continue;
                    }
                    xneg = xpos - ((1.01 * yneg) / num11);
                    if (((xneg < data.xpos) && (xneg < data.xneg)) || ((xneg > data.xpos) && (xneg > data.xneg)))
                    {
                        continue;
                    }
                Label_0274:
                    if (this.irr_npv(xneg, ref y, vals))
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

        internal bool goal_seek_newton(ref GoalSeekData data, double[] vals, double x0)
        {
            double num2 = data.precision / 2.0;
            for (int i = 0; i < 20; i++)
            {
                double y = 0.0;
                double num6 = 0.0;
                if ((x0 < data.xmin) || (x0 > data.xmax))
                {
                    return false;
                }
                bool flag = this.irr_npv(x0, ref y, vals);
                if (!flag)
                {
                    return flag;
                }
                if (FinancialHelper.update_data(x0, y, ref data))
                {
                    return true;
                }
                flag = this.irr_npv_df(x0, ref num6, vals);
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

        internal bool goal_seek_point(ref GoalSeekData data, double[] vals, double x0)
        {
            double y = 0.0;
            if ((x0 < data.xmin) || (x0 > data.xmax))
            {
                return false;
            }
            bool flag = this.irr_npv(x0, ref y, vals);
            if (!flag)
            {
                return flag;
            }
            return FinancialHelper.update_data(x0, y, ref data);
        }

        internal bool irr_npv(double rate, ref double y, double[] vals)
        {
            int length = vals.Length;
            double num2 = 0.0;
            double num3 = 1.0;
            double num4 = 1.0 / (rate + 1.0);
            for (int i = 0; i < length; i++)
            {
                num2 += vals[i] * num3;
                num3 *= num4;
            }
            y = num2;
            return true;
        }

        internal bool irr_npv_df(double rate, ref double y, double[] vals)
        {
            int length = vals.Length;
            double num2 = 0.0;
            double num3 = 1.0;
            double num4 = 1.0 / (rate + 1.0);
            for (int i = 1; i < length; i++)
            {
                num2 += (vals[i] * -i) * num3;
                num3 *= num4;
            }
            y = num2;
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
                return 2;
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
                return 1;
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
                return "IRR";
            }
        }
    }
}

