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
    /// Returns the <see cref="T:System.Double" /> negative binomial distribution.
    /// </summary>
    public class CalcNegBinomDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> negative binomial distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: number_f, number_s, probability_s.
        /// </para>
        /// <para>
        /// Number_f is the number of failures.
        /// </para>
        /// <para>
        /// Number_s is the threshold number of successes.
        /// </para>
        /// <para>
        /// Probability_s is the probability of a success.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num;
            int num2;
            double num3;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToInt(args[0], out num) || !CalcConvert.TryToInt(args[1], out num2))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToDouble(args[2], out num3, true))
            {
                return CalcErrors.Value;
            }
            if ((num3 < 0.0) || (num3 >= 1.0))
            {
                return CalcErrors.Number;
            }
            if (((num + num2) - 1) <= 0)
            {
                return CalcErrors.Number;
            }
            object obj2 = new CalcCombinFunction().Evaluate(new object[] { (int) ((num + num2) - 1), (int) (num2 - 1) });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num4 = (double) ((double) obj2);
            double num5 = Math.Pow(num3, (double) num2);
            double num6 = Math.Pow(1.0 - num3, (double) num);
            return CalcConvert.ToResult((num4 * num5) * num6);
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
                return "NEGBINOMDIST";
            }
        }
    }
}

