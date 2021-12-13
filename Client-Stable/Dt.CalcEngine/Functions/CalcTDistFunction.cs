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
    /// Returns the Percentage Points (probability) for the Student t-distribution 
    /// where a numeric value (x) is a calculated value of t for which the Percentage
    /// Points are to be computed.
    /// </summary>
    /// <remarks>
    /// The t-distribution is used in the hypothesis testing of small sample data sets.
    /// Use this function in place of a table of critical values for the t-distribution.
    /// </remarks>
    public class CalcTDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the Percentage Points (probability) for the Student t-distribution
        /// where a numeric value (x) is a calculated value of t for which the Percentage
        /// Points are to be computed.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: x, degrees_freedom, tails.
        /// </para>
        /// <para>
        /// X is the numeric value at which to evaluate the distribution.
        /// </para>
        /// <para>
        /// Degrees_freedom is an integer indicating the number of degrees of fr.
        /// </para>
        /// <para>
        /// Tails specifies the number of distribution tails to return.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            int num2;
            int num3;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToInt(args[1], out num2) || !CalcConvert.TryToInt(args[2], out num3))
            {
                return CalcErrors.Value;
            }
            if (((num2 < 1) || ((num3 != 1) && (num3 != 2))) || (num < 0.0))
            {
                return CalcErrors.Number;
            }
            double d = num2;
            double num5 = num / Math.Sqrt(d);
            double num6 = d / (d + (num * num));
            double num7 = d - 2.0;
            double num8 = d % 2.0;
            double num9 = 1.0;
            double num10 = 1.0;
            d = 1.0;
            double num11 = 2.0 + num8;
            double num12 = num11;
            if (num7 >= 2.0)
            {
                for (int i = (int) num11; i <= num7; i += 2)
                {
                    num10 = ((num10 * num6) * (num12 - 1.0)) / num12;
                    num9 += num10;
                    if (num9 == d)
                    {
                        break;
                    }
                    d = num9;
                    num12 += 2.0;
                }
            }
            if (num8 != 1.0)
            {
                return (double) (num3 * (1.0 - (0.5 + (((0.5 * num5) * Math.Sqrt(num6)) * num9))));
            }
            if (num2 == 1.0)
            {
                num9 = 0.0;
            }
            return (double) (num3 * (1.0 - (0.5 + ((((num5 * num6) * num9) + Math.Atan(num5)) * 0.3183098862))));
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
                return "TDIST";
            }
        }
    }
}

