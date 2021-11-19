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
    /// Returns the Weibull distribution.
    /// </summary>
    /// <remarks>
    /// Use this distribution in reliability analysis, such as calculating
    /// a device's mean time to failure.
    /// </remarks>
    public class CalcWeibullFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the Weibull distribution.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 4 items: x, alpha, beta, cumulative.
        /// </para>
        /// <para>
        /// X is the value at which to evaluate the function.
        /// </para>
        /// <para>
        /// Alpha is a parameter to the distribution.
        /// </para>
        /// <para>
        /// Beta is a parameter to the distribution.
        /// </para>
        /// <para>
        /// Cumulative determines the form of the function.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            double num3;
            bool flag;
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
            if (flag)
            {
                return CalcConvert.ToResult(1.0 - Math.Exp(-Math.Pow(num / num3, num2)));
            }
            return CalcConvert.ToResult(((num2 / Math.Pow(num3, num2)) * Math.Pow(num, num2 - 1.0)) * Math.Exp(-Math.Pow(num / num3, num2)));
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
                return "WEIBULL";
            }
        }
    }
}

