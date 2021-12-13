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
    /// Returns the <see cref="T:System.Double" /> number converted from hexadecimal to decimal.
    /// </summary>
    public class CalcHex2DecFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Double" /> number converted from hexadecimal to decimal.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: number.
        /// </para>
        /// <para>
        /// Number is the hexadecimal number you want to convert.
        /// Number cannot contain more than 10 characters (40 bits).
        /// The most significant bit of number is the sign bit.
        /// The remaining 39 bits are magnitude bits.
        /// Negative numbers are represented using two's-complement notation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Object" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num2;
            base.CheckArgumentsLength(args);
            string s = CalcConvert.ToString(args[0]);
            if (10 < s.Length)
            {
                return CalcErrors.Number;
            }
            long num = EngineeringHelper.StringToLong(s, 0x10, out num2);
            if (num2 < s.Length)
            {
                return CalcErrors.Number;
            }
            return CalcConvert.ToResult((double) num);
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
                return "HEX2DEC";
            }
        }
    }
}

