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
    /// Returns the remainder after number is divided by divisor.
    /// The result has the same sign as divisor.
    /// </summary>
    public class CalcModFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the remainder after number is divided by divisor.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: number, divisor.
        /// </para>
        /// <para>
        /// Number is the number for which you want to find the remainder.
        /// </para>
        /// <para>
        /// Divisor is the number by which you want to divide number.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true) || !CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcErrors.Value;
            }
            if (num2 == 0.0)
            {
                return CalcErrors.DivideByZero;
            }
            return (double) (num - (num2 * (num / num2).ApproxFloor()));
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
                return "MOD";
            }
        }
    }
}

