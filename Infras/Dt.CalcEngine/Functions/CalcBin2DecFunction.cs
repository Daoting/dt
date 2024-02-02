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
    /// Returns the <see cref="T:System.Int32" /> converted from binary to decimal.
    /// </summary>
    public class CalcBin2DecFunction : CalcBuiltinFunction
    {
        /// <summary>
        /// Returns the <see cref="T:System.Int32" /> converted from binary to decimal.
        /// </summary>
        /// <param name="args"><para>
        /// The args contains 1 item: number.
        /// </para>
        /// <para>
        /// Number is the binary number you want to convert.
        /// Number cannot contain more than 10 characters (10 bits).
        /// The most significant bit of number is the sign bit.
        /// The remaining 9 bits are magnitude bits.
        /// Negative numbers are represented using two's-complement notation.
        /// </para></param>
        /// <returns>
        /// A <see cref="T:System.Int32" /> value that indicates the evaluate result.
        /// </returns>
        public override object Evaluate(object[] args)
        {
            int num2;
            base.CheckArgumentsLength(args);
            string s = CalcConvert.ToString(args[0]);
            if (s.Length > 10)
            {
                return CalcErrors.Number;
            }
            long num = EngineeringHelper.StringToLong(s, 2, out num2);
            if (s.Length > num2)
            {
                return CalcErrors.Number;
            }
            return (int) CalcConvert.ToInt((long) num);
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
                return "BIN2DEC";
            }
        }
    }
}

