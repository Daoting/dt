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
    /// Returns the integer portion of a division.
    /// </summary>
    /// <remarks>
    /// Use this function when you want to discard the remainder of a division.
    /// </remarks>
    public class CalcQuotientFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the integer portion of a division.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: numerator, denominator.
        /// </para>
        /// <para>
        /// Numerator is the dividend.
        /// </para>
        /// <para>
        /// Denominator is the divisor.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (args[0] is CalcError)
            {
                return args[0];
            }
            if (args[1] is CalcError)
            {
                return args[1];
            }
            if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            if (num2 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return (int) (num / num2);
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
                return 2;
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
                return 2;
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
                return "QUOTIENT";
            }
        }
    }
}

