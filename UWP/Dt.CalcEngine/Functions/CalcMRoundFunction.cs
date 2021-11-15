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
    /// Returns a number rounded to the desired multiple.
    /// </summary>
    public class CalcMRoundFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns a number rounded to the desired multiple.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: number, multiple.
        /// </para>
        /// <para>
        /// Number is the value to round.
        /// </para>
        /// <para>
        /// Multiple is the multiple to which you want to round number.
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
            if ((num == 0.0) || (num2 == 0.0))
            {
                return (double) 0.0;
            }
            if (((num >= 0.0) || (0.0 >= num2)) && ((num2 >= 0.0) || (0.0 >= num)))
            {
                return (double) (((num / num2) + 0.5).ApproxFloor() * num2);
            }
            return CalcErrors.Number;
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
                return "MROUND";
            }
        }
    }
}

