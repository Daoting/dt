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
    /// Returns the natural logarithm of a number.
    /// </summary>
    /// <remarks>
    /// Natural logarithms are based on the constant e (2.71828182845904).
    /// </remarks>
    public class CalcLnFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the natural logarithm of a number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: number.
        /// </para>
        /// <para>
        /// Number is the positive real number for which you want the natural logarithm.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            base.CheckArgumentsLength(args);
            if (!CalcConvert.TryToDouble(args[0], out num, true))
            {
                return CalcErrors.Value;
            }
            if (num <= 0.0)
            {
                return CalcErrors.Number;
            }
            return CalcConvert.ToResult(Math.Log(num));
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
                return "LN";
            }
        }
    }
}

