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
    /// Returns the <see cref="T:System.Double" /> hyper geometric distribution for a finite population.
    /// </summary>
    public class CalcHypGeomDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> hyper geometric distribution for a finite population.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: sample_s, number_sample, population_s, number_population.
        /// </para>
        /// <para>
        /// Sample_s is the number of successes in the sample.
        /// </para>
        /// <para>
        /// Number_sample is the size of the sample.
        /// </para>
        /// <para>
        /// Population_s is the number of successes in the population.
        /// </para>
        /// <para>
        /// Number_population is the population size.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int num = CalcConvert.ToInt(args[0]);
            int num2 = CalcConvert.ToInt(args[1]);
            int num3 = CalcConvert.ToInt(args[2]);
            int num4 = CalcConvert.ToInt(args[3]);
            if ((num < 0.0) || (num > Math.Min(num2, num3)))
            {
                return CalcErrors.Number;
            }
            if (num < Math.Max((double) 0.0, (double) ((num2 - num4) + num3)))
            {
                return CalcErrors.Number;
            }
            if ((num2 < 0.0) || (num2 > num4))
            {
                return CalcErrors.Number;
            }
            if ((num3 < 0.0) || (num3 > num4))
            {
                return CalcErrors.Number;
            }
            if (num4 < 0.0)
            {
                return CalcErrors.Number;
            }
            CalcBuiltinFunction function = new CalcCombinFunction();
            object obj2 = function.Evaluate(new object[] { (int) num3, (int) num });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num5 = (double) ((double) obj2);
            obj2 = function.Evaluate(new object[] { (int) (num4 - num3), (int) (num2 - num) });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num6 = (double) ((double) obj2);
            obj2 = function.Evaluate(new object[] { (int) num4, (int) num2 });
            if (obj2 is CalcError)
            {
                return obj2;
            }
            double num7 = (double) ((double) obj2);
            return CalcConvert.ToResult((num5 * num6) / num7);
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
                return "HYPGEOMDIST";
            }
        }
    }
}

