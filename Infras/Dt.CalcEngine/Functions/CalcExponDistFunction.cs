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
    /// Returns the <see cref="T:System.Double" /> exponential distribution.
    /// </summary>
    public class CalcExponDistFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> exponential distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 3 items: x, lambda, cumulative.
        /// </para>
        /// <para>
        /// X is the value of the function.
        /// </para>
        /// <para>
        /// Lambda is the parameter value.
        /// </para>
        /// <para>
        /// Cumulative is a logical value that indicates which form of the exponential function to provide. If cumulative is TRUE, EXPONDIST returns the cumulative distribution function; if FALSE, it returns the probability density function.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num2;
            double num3;
            bool flag;
            base.CheckArgumentsLength(args);
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is CalcError)
                {
                    return args[i];
                }
            }
            if (!CalcConvert.TryToDouble(args[0], out num2, true) || !CalcConvert.TryToDouble(args[1], out num3, true))
            {
                return CalcErrors.Value;
            }
            if (!CalcConvert.TryToBool(args[2], out flag))
            {
                return CalcErrors.Value;
            }
            if (num2 < 0.0)
            {
                return CalcErrors.Number;
            }
            if (num3 <= 0.0)
            {
                return CalcErrors.Number;
            }
            double num4 = Math.Exp(-num3 * num2);
            return CalcConvert.ToResult(flag ? (1.0 - num4) : (num3 * num4));
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
                return "EXPONDIST";
            }
        }
    }
}

