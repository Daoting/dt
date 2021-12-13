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
    /// Returns the t-value of the Student's t-distribution as a function of 
    /// the probability and the degrees of freedom.
    /// </summary>
    public class CalcTInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the t-value of the Student's t-distribution as a function of
        /// the probability and the degrees of freedom.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: probability, degrees_freedom.
        /// </para>
        /// <para>
        /// Probability is the probability associated with the two-tailed Student's
        /// t-distribution.
        /// </para>
        /// <para>
        /// Degrees_freedom   is the number of degrees of freedom with which to
        /// characterize the distribution.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            int num2;
            double num9;
            double maxValue;
            int num13;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToInt(args[1], out num2))
            {
                return CalcErrors.Value;
            }
            double num3 = 1E-12;
            if (((num < 0.0) || (1.0 < num)) || ((num2 < 1) || (num2 > Math.Pow(10.0, 10.0))))
            {
                return CalcErrors.Number;
            }
            double num14 = num / 2.0;
            double num15 = num14;
            if (num2 > 1E+20)
            {
                CalcNormSInvFunction function = new CalcNormSInvFunction();
                return function.Evaluate(new object[] { (double) num14 });
            }
            if (num15 < 0.5)
            {
                num13 = 0;
                num9 = 2.0 * num15;
            }
            else
            {
                num13 = 1;
                num9 = 2.0 * (1.0 - num15);
            }
            if (Math.Abs((double) (num2 - 2.0)) < num3)
            {
                if (num9 > 0.0)
                {
                    maxValue = Math.Sqrt((2.0 / (num9 * (2.0 - num9))) - 2.0);
                }
                else
                {
                    maxValue = double.MaxValue;
                }
            }
            else if (num2 < (1.0 + num3))
            {
                if (num9 > 0.0)
                {
                    double a = (num9 + 1.0) * 1.5707963267948966;
                    maxValue = -Math.Tan(a);
                }
                else
                {
                    maxValue = double.MaxValue;
                }
            }
            else
            {
                double num4 = 1.0 / (num2 - 0.5);
                double num5 = 48.0 / (num4 * num4);
                double num6 = ((((((20700.0 * num4) / num5) - 98.0) * num4) - 16.0) * num4) + 96.36;
                double num7 = (((((94.5 / (num5 + num6)) - 3.0) / num5) + 1.0) * Math.Sqrt(num4 * 1.5707963267948966)) * num2;
                double d = Math.Pow(num7 * num9, 2.0 / ((double) num2));
                if (d > (0.05 + num4))
                {
                    object obj2 = new CalcNormSInvFunction().Evaluate(new object[] { (double) (0.5 * num9) });
                    if (obj2 is CalcError)
                    {
                        return obj2;
                    }
                    double num11 = (double) ((double) obj2);
                    d = num11 * num11;
                    if (num2 < 5.0)
                    {
                        num6 += (0.3 * (num2 - 4.5)) * (num11 + 0.6);
                    }
                    num6 = (((((((((0.05 * num7) * num11) - 5.0) * num11) - 7.0) * num11) - 2.0) * num11) + num5) + num6;
                    d = (((((((((((0.4 * d) + 6.3) * d) + 36.0) * d) + 94.5) / num6) - d) - 3.0) / num5) + 1.0) * num11;
                    d = (num4 * d) * d;
                    if (d > 0.002)
                    {
                        d = Math.Exp(d) - 1.0;
                    }
                    else
                    {
                        d = ((0.5 * d) * d) + d;
                    }
                }
                else
                {
                    d = ((((((1.0 / ((((((num2 + 6.0) / (num2 * d)) - (0.089 * num7)) - 0.822) * (num2 + 2.0)) * 3.0)) + (0.5 / (num2 + 4.0))) * d) - 1.0) * (num2 + 1.0)) / (num2 + 2.0)) + (1.0 / d);
                }
                maxValue = Math.Sqrt(num2 * d);
            }
            if (num13 != 0.0)
            {
                maxValue = -maxValue;
            }
            return CalcConvert.ToResult(maxValue);
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
                return "TINV";
            }
        }
    }
}

