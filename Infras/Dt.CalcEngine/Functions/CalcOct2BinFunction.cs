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
    /// Returns the <see cref="T:System.String" /> number converted from octal to binary.
    /// </summary>
    public class CalcOct2BinFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Indicates whether the Evaluate method can process missing arguments.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the Evaluate method can process missing arguments; 
        /// otherwise, <see langword="false" />.
        /// </value>
        public override bool AcceptsMissingArgument(int i)
        {
            return (i == 1);
        }

        /// <summary>
        /// Returns the <see cref="T:System.String" /> number converted from octal to binary.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [places].
        /// </para>
        /// <para>
        /// Number is the octal number you want to convert.
        /// Number may not contain more than 10 characters.
        /// The most significant bit of number is the sign bit.
        /// The remaining 29 bits are magnitude bits.
        /// Negative numbers are represented using two's-complement notation.
        /// </para>
        /// <para>
        /// Places is the number of characters to use.
        /// If places is omitted, OCT2BIN uses the minimum number of characters necessary.
        /// Places is useful for padding the return value with leading 0s (zeros).
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num3;
            base.CheckArgumentsLength(args);
            string s = CalcConvert.ToString(args[0]);
            int num = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 1;
            if (10 < s.Length)
            {
                return CalcErrors.Number;
            }
            if ((num < 1) || (10 < num))
            {
                return CalcErrors.Number;
            }
            long number = EngineeringHelper.StringToLong(s, 8, out num3);
            if (num3 < s.Length)
            {
                return CalcErrors.Number;
            }
            if ((number < -512L) || (0x1ffL < number))
            {
                return CalcErrors.Number;
            }
            string str2 = EngineeringHelper.LongToString(number, 2L, (long) num);
            if (((0L <= number) && (num < str2.Length)) && CalcHelper.ArgumentExists(args, 1))
            {
                return CalcErrors.Number;
            }
            return str2;
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
                return "OCT2BIN";
            }
        }
    }
}

