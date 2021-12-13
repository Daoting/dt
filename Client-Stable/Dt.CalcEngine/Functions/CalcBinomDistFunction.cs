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
    /// Returns the <see cref="T:System.Double" /> individual term binomial distribution probability. 
    /// </summary>
    public class CalcBinomDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> individual term binomial distribution probability.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: number_s, trials, probability_s, cumulative.
        /// </para>
        /// <para>
        /// Number_s is the number of successes in trials.
        /// </para>
        /// <para>
        /// Trials is the number of independent trials.
        /// </para>
        /// <para>
        /// Probability_s is the probability of success on each trial.
        /// </para>
        /// <para>
        /// Cumulative is a logical value that determines the form of the function.
        /// If cumulative is TRUE, then BINOMDIST returns the cumulative distribution function,
        /// which is the probability that there are at most number_s successes;
        /// if FALSE, it returns the probability mass function,
        /// which is the probability that there are number_s successes.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            int num2;
            double num3;
            bool flag;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToInt(args[0], out num) || !CalcConvert.TryToInt(args[1], out num2))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToBool(args[3], out flag))
            {
                return CalcErrors.Value;
            }
            if (((num < 0) || (num2 < 0)) || (((num2 < num) || (num3 < 0.0)) || (1.0 < num3)))
            {
                return CalcErrors.Number;
            }
            if (!flag)
            {
                double num4 = 1.0 - num3;
                double num5 = Math.Pow(num4, (double) num2);
                if (num5 == 0.0)
                {
                    num5 = Math.Pow(num3, (double) num2);
                    if (num5 == 0.0)
                    {
                        return CalcErrors.Number;
                    }
                    for (int k = 0; (k < (num2 - num)) && (num5 > 0.0); k++)
                    {
                        num5 *= ((((double) (num2 - k)) / ((double) (k + 1))) * num4) / num3;
                    }
                    return (double) num5;
                }
                for (int j = 0; (j < num) && (num5 > 0.0); j++)
                {
                    num5 *= ((((double) (num2 - j)) / ((double) (j + 1))) * num3) / num4;
                }
                return (double) num5;
            }
            if (num2 == num)
            {
                return (double) 1.0;
            }
            double x = 1.0 - num3;
            double num9 = Math.Pow(x, (double) num2);
            if (num9 == 0.0)
            {
                num9 = Math.Pow(num3, (double) num2);
                if (num9 == 0.0)
                {
                    return CalcErrors.Number;
                }
                double num10 = 1.0 - num9;
                for (int m = 0; (m < (num2 - num)) && (num9 > 0.0); m++)
                {
                    num9 *= ((((double) (num2 - m)) / ((double) (m + 1))) * x) / num3;
                    num10 -= num9;
                }
                if (num10 < 0.0)
                {
                    return (double) 0.0;
                }
                return (double) num10;
            }
            double num12 = num9;
            for (int i = 0; (i < num) && (num9 > 0.0); i++)
            {
                num9 *= ((((double) (num2 - i)) / ((double) (i + 1))) * num3) / x;
                num12 += num9;
            }
            return (double) num12;
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
                return "BINOMDIST";
            }
        }
    }
}

