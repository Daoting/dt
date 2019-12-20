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
    /// Returns the result of a number raised to a power.
    /// </summary>
    public class CalcPowerFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the result of a number raised to a power.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: number, power.
        /// </para>
        /// <para>
        /// Number is the base number. It can be any real number.
        /// </para>
        /// <para>
        /// Power is the exponent to which the base number is raised.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Double" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            double num;
            double num2;
            base.CheckArgumentsLength(args);
            if (CalcConvert.TryToDouble(args[0], out num, true) && CalcConvert.TryToDouble(args[1], out num2, true))
            {
                return CalcConvert.ToResult(Math.Pow(num, num2));
            }
            return CalcErrors.Value;
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
                return "POWER";
            }
        }
    }
}

