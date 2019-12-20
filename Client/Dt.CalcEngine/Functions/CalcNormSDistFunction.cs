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
    /// Returns the <see cref="T:System.Double" /> standard normal cumulative distribution.
    /// </summary>
    public class CalcNormSDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> standard normal cumulative distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: z.
        /// </para>
        /// <para>
        /// Z is the value for which you want the distribution.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num4;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            double num2 = 6.0;
            if (num == 0.0)
            {
                num4 = 0.0;
            }
            else
            {
                double num3 = 0.5 * Math.Abs(num);
                if (num3 >= (num2 * 0.5))
                {
                    num4 = 1.0;
                }
                else if (num3 < 1.0)
                {
                    double num5 = num3 * num3;
                    num4 = (((((((((((((((((0.000124818987 * num5) - 0.001075204047) * num5) + 0.005198775019) * num5) - 0.019198292004) * num5) + 0.059054035642) * num5) - 0.151968751364) * num5) + 0.319152932694) * num5) - 0.5319230073) * num5) + 0.797884560593) * num3) * 2.0;
                }
                else
                {
                    num3 -= 2.0;
                    num4 = (((((((((((((((((((((((((((-4.5255659E-05 * num3) + 0.00015252929) * num3) - 1.9538132E-05) * num3) - 0.000676904986) * num3) + 0.001390604284) * num3) - 0.00079462082) * num3) - 0.002034254874) * num3) + 0.006549791214) * num3) - 0.010557625006) * num3) + 0.011630447319) * num3) - 0.009279453341) * num3) + 0.005353579108) * num3) - 0.002141268741) * num3) + 0.000535310849) * num3) + 0.999936657524;
                }
            }
            return ((num > 0.0) ? ((double) ((num4 + 1.0) * 0.5)) : ((double) ((1.0 - num4) * 0.5)));
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
                return 1;
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
                return "NORMSDIST";
            }
        }
    }
}

