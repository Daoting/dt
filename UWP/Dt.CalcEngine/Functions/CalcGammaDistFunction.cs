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
    /// Returns the <see cref="T:System.Double" /> gamma distribution.
    /// </summary>
    public class CalcGammaDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> gamma distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: x, alpha, beta, cumulative.
        /// </para>
        /// <para>
        /// X is the value at which you want to evaluate the distribution.
        /// </para>
        /// <para>
        /// Alpha is a parameter to the distribution.
        /// </para>
        /// <para>
        /// Beta is a parameter to the distribution. If beta = 1, GAMMADIST returns the standard gamma distribution.
        /// </para>
        /// <para>
        /// Cumulative is a logical value that determines the form of the function. If cumulative is TRUE, GAMMADIST returns the cumulative distribution function; if FALSE, it returns the probability density function.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            bool flag;
            double num9;
            double num15;
            double num16;
            double num18;
            double num21;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToBool(args[3], out flag))
            {
                return CalcErrors.Value;
            }
            if (((num < 0.0) || (num2 <= 0.0)) || (num3 <= 0.0))
            {
                return CalcErrors.Number;
            }
            if (!flag)
            {
                double d = Math.Pow(num3, num2);
                if (double.IsNaN(d) || double.IsInfinity(d))
                {
                    return CalcErrors.DivideByZero;
                }
                double num5 = 1.0 / (d * EngineeringHelper.gamma(num2));
                double num6 = Math.Pow(num, num2 - 1.0);
                double num7 = Math.Exp(-(num / num3));
                double num8 = num6 * num7;
                return (double) (num5 * num8);
            }
            double y = 0.33333333333333331;
            double num23 = 100000000.0;
            double num24 = 1E+37;
            double num25 = 1000.0;
            double num26 = -88.0;
            num /= num3;
            if (num <= 0.0)
            {
                return CalcErrors.Number;
            }
            if (num2 > num25)
            {
                CalcBuiltinFunction function = new CalcNormDistFunction();
                num9 = (Math.Sqrt(num2) * 3.0) * ((Math.Pow(num / num2, y) + (1.0 / (num2 * 9.0))) - 1.0);
                object obj2 = function.Evaluate(new object[] { (double) num9, (double) 0.0, (double) 1.0, (bool) true });
                if (obj2 is CalcError)
                {
                    return obj2;
                }
                return (double) obj2;
            }
            if (num > num23)
            {
                return (double) 1.0;
            }
            if ((num <= 1.0) || (num < num2))
            {
                object obj3 = new CalcGammaLnFunction().Evaluate(new object[] { (double) (num2 + 1.0) });
                if (obj3 is CalcError)
                {
                    return obj3;
                }
                num15 = ((num2 * Math.Log(num)) - num) - ((double) obj3);
                num16 = 1.0;
                num21 = 1.0;
                num18 = num2;
                do
                {
                    num18++;
                    num16 = (num16 * num) / num18;
                    num21 += num16;
                }
                while (num16 > 2.2204460492503131E-16);
                num15 += Math.Log(num21);
                num21 = 0.0;
                if (num15 >= num26)
                {
                    num21 = Math.Exp(num15);
                }
            }
            else
            {
                object obj4 = new CalcGammaLnFunction().Evaluate(new object[] { (double) num2 });
                if (obj4 is CalcError)
                {
                    return obj4;
                }
                num15 = ((num2 * Math.Log(num)) - num) - ((double) obj4);
                num18 = 1.0 - num2;
                double num19 = (num18 + num) + 1.0;
                num16 = 0.0;
                num9 = 1.0;
                double num10 = num;
                double num11 = num + 1.0;
                double num12 = num * num19;
                num21 = num11 / num12;
                while (true)
                {
                    num18++;
                    num19 += 2.0;
                    num16++;
                    double num20 = num18 * num16;
                    double num13 = (num19 * num11) - (num20 * num9);
                    double num14 = (num19 * num12) - (num20 * num10);
                    if (Math.Abs(num14) > 0.0)
                    {
                        double num17 = num13 / num14;
                        if (Math.Abs((double) (num21 - num17)) <= Math.Min((double) 2.2204460492503131E-16, (double) (2.2204460492503131E-16 * num17)))
                        {
                            break;
                        }
                        num21 = num17;
                    }
                    num9 = num11;
                    num10 = num12;
                    num11 = num13;
                    num12 = num14;
                    if (Math.Abs(num13) >= num24)
                    {
                        num9 /= num24;
                        num10 /= num24;
                        num11 /= num24;
                        num12 /= num24;
                    }
                }
                num15 += Math.Log(num21);
                num21 = 1.0;
                if (num15 >= num26)
                {
                    num21 = 1.0 - Math.Exp(num15);
                }
            }
            return (double) num21;
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
                return "GAMMADIST";
            }
        }
    }
}

