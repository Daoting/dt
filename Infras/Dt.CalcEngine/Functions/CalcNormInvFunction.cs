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
    /// Returns the <see cref="T:System.Double" /> inverse of the normal cumulative distribution.
    /// </summary>
    public class CalcNormInvFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> inverse of the normal cumulative distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: probability, mean, standard_dev.
        /// </para>
        /// <para>
        /// Probability is a probability corresponding to the normal distribution.
        /// </para>
        /// <para>
        /// Mean is the arithmetic mean of the distribution.
        /// </para>
        /// <para>
        /// Standard_dev is the standard deviation of the distribution.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            double num5;
            double num6;
            base.CheckArgumentsLength(args);
            if ((!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true)) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if ((num < 0.0) || (1.0 < num))
            {
                return CalcErrors.Number;
            }
            if (num3 <= 0.0)
            {
                return CalcErrors.Number;
            }
            double num4 = num - 0.5;
            if (Math.Abs(num4) <= 0.42)
            {
                num5 = num4 * num4;
                num6 = (num4 * ((((((-25.44106049637 * num5) + 41.39119773534) * num5) - 18.61500062529) * num5) + 2.50662823884)) / ((((((((3.13082909833 * num5) - 21.06224101826) * num5) + 23.08336743743) * num5) + -8.4735109309) * num5) + 1.0);
            }
            else
            {
                num5 = num;
                if (num4 > 0.0)
                {
                    num5 = 1.0 - num;
                }
                if (num5 > 2.2204460492503131E-16)
                {
                    num5 = Math.Sqrt(-Math.Log(num5));
                    num6 = ((((((2.32121276858 * num5) + 4.85014127135) * num5) - 2.29796479134) * num5) - 2.78718931138) / ((((1.63706781897 * num5) + 3.54388924762) * num5) + 1.0);
                    if (num4 < 0.0)
                    {
                        num6 = -num6;
                    }
                }
                else
                {
                    if (num5 > 1E-300)
                    {
                        num6 = -2.0 * Math.Log(num);
                        num5 = Math.Log(6.2831853071795862 * num6);
                        num5 = ((num5 / num6) + ((2.0 - num5) / (num6 * num6))) + (((-14.0 + (6.0 * num5)) - (num5 * num5)) / (((2.0 * num6) * num6) * num6));
                        num6 = Math.Sqrt(num6 * (1.0 - num5));
                        if (num4 < 0.0)
                        {
                            num6 = -num6;
                        }
                        return (double) num6;
                    }
                    if (num4 < 0.0)
                    {
                        return (double) double.MinValue;
                    }
                    return (double) double.MaxValue;
                }
            }
            CalcBuiltinFunction function = new CalcNormDistFunction();
            double num7 = (num6 - 0.0) / 1.0;
            double num8 = (0.3989422804014327 * Math.Exp((-0.5 * num7) * num7)) / 1.0;
            object obj2 = function.Evaluate(new object[] { (double) num6, (double) 0.0, (double) 1.0, (bool) true });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            num6 -= (((double) obj2) - num) / num8;
            return (double) (num2 + (num3 * num6));
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
                return "NORMINV";
            }
        }
    }
}

