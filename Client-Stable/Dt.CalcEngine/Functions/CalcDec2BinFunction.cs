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
    /// Returns the <see cref="T:System.String" /> converted from decimal to binary.
    /// </summary>
    public class CalcDec2BinFunction : CalcBuiltinFunction
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
        /// Returns the <see cref="T:System.String" /> converted from decimal to binary.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 - 2 items: number, [places].
        /// </para>
        /// <para>
        /// Number is the decimal integer you want to convert.
        /// If number is negative, valid place values are ignored and DEC2BIN
        /// returns a 10-character (10-bit) binary number in which the most significant bit is the sign bit.
        /// The remaining 9 bits are magnitude bits.
        /// Negative numbers are represented using two's-complement notation.
        /// </para>
        /// <para>
        /// Places is the number of characters to use.
        /// If places is omitted, DEC2BIN uses the minimum number of characters necessary.
        /// Places is useful for padding the return value with leading 0s (zeros).
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.String" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            base.CheckArgumentsLength(args);
            long number = CalcConvert.ToLong(args[0]);
            int num2 = CalcHelper.ArgumentExists(args, 1) ? CalcConvert.ToInt(args[1]) : 1;
            if ((number < -512L) || (0x1ffL < number))
            {
                return CalcErrors.Number;
            }
            if ((num2 < 1) || (10 < num2))
            {
                return CalcErrors.Number;
            }
            string str = EngineeringHelper.LongToString(number, 2L, (long) num2);
            if (((0L <= number) && (num2 < str.Length)) && CalcHelper.ArgumentExists(args, 1))
            {
                return CalcErrors.Number;
            }
            return str;
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
                return "DEC2BIN";
            }
        }
    }
}

