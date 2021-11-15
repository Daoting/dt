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
    /// Rounds a number down, toward zero.
    /// </summary>
    public class CalcRoundDownFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Rounds a number down, toward zero.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 2 items: number, num_digits.
        /// </para>
        /// <para>
        /// Number is any real number that you want rounded down.
        /// </para>
        /// <para>
        /// Num_digits is the number of digits to which you want to round number.
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
            int num2 = CalcConvert.ToInt(args[1]);
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
                return "ROUNDDOWN";
            }
        }
    }
}

