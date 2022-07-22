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
    /// Truncates a number to an integer by removing the fractional part of the number.
    /// </summary>
    public class CalcTruncFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <param name="i">Index of the argument</param>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 1);
        }

        /// <summary>
        /// Truncates a number to an integer by removing the fractional part of the number.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [num_digits].
        /// </para>
        /// <para>
        /// Number is the number you want to truncate.
        /// </para>
        /// <para>
        /// [Num_digits] is a number specifying the precision of the truncation.
        /// The default value for num_digits is 0.
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
            int num2 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 0;
            double num3 = MathHelper.Pow10(Math.Abs(num2));
            if (num2 < 0)
            {
                num /= num3;
            }
            else
            {
                num *= num3;
            }
            if (num < 0.0)
            {
                num = num.ApproxCeiling();
            }
            else
            {
                num = num.ApproxFloor();
            }
            if (num2 < 0)
            {
                num *= num3;
            }
            else
            {
                num /= num3;
            }
            return CalcConvert.ToResult(num);
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
                return "TRUNC";
            }
        }
    }
}

