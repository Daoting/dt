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
    /// Returns the factorial of a number.
    /// </summary>
    /// <remarks>
    /// The factorial of a number is equal to 1*2*3*...* number.
    /// </remarks>
    public class CalcFactFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the factorial of a number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: number.
        /// </para>
        /// <para>
        /// Number is the nonnegative number for which you want the factorial.
        /// If number is not an integer, it is truncated.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            int num = CalcConvert.ToInt(args[0]);
            double num2 = 1.0;
            if ((num < 0) || (170 < num))
            {
                return CalcErrors.Number;
            }
            for (int i = 1; i <= num; i++)
            {
                num2 *= i;
            }
            return (double) num2;
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
                return "FACT";
            }
        }
    }
}

