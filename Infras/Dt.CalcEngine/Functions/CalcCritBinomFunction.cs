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
    /// Returns the <see cref="T:System.Double" /> smallest number which is the cumulative binomial distribution that is greater than a criterion value.
    /// </summary>
    public class CalcCritBinomFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> smallest number which is the cumulative binomial distribution that is greater than a criterion value.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: trials, probability_s, alpha.
        /// </para>
        /// <para>
        /// Trials is the number of Bernoulli trials.
        /// </para>
        /// <para>
        /// Probability_s is the probability of a success on each trial.
        /// </para>
        /// <para>
        /// Alpha is the criterion value.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            double num2;
            double num3;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToInt(args[0], out num))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToDouble(args[1], out num2, true) || !CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (((num < 0) || (num2 < 0.0)) || (((1.0 < num2) || (num3 <= 0.0)) || (1.0 <= num3)))
            {
                return CalcErrors.Number;
            }
            double x = 1.0 - num2;
            double num5 = Math.Pow(x, (double) num);
            if (num5 == 0.0)
            {
                num5 = Math.Pow(num2, (double) num);
                if (num5 == 0.0)
                {
                    return CalcErrors.Number;
                }
                double num6 = 1.0 - num5;
                int num7 = 0;
                while ((num7 < num) && (num6 >= num3))
                {
                    num5 *= ((((double) (num - num7)) / ((double) (num7 + 1))) * x) / num2;
                    num6 -= num5;
                    num7++;
                }
                return (double) (num - num7);
            }
            double num8 = num5;
            int num9 = 0;
            while ((num9 < num) && (num8 < num3))
            {
                num5 *= ((((double) (num - num9)) / ((double) (num9 + 1))) * num2) / x;
                num8 += num5;
                num9++;
            }
            return (double) num9;
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
                return "CRITBINOM";
            }
        }
    }
}

