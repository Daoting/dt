#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number helper class.
    /// </summary>
    internal static class NumberHelper
    {
        /// <summary>
        /// The Japanese special char that is used to display a date time.
        /// </summary>
        static readonly int[] JapaneseChars = new int[] { 
            0xff10, 0xff11, 0xff12, 0xff13, 0xff14, 0xff15, 0xff16, 0xff17, 0xff18, 0xff19, 0xff04, 0xffe5, 0xff05, 0xff25, 0xff45, 0xff0b, 
            0xff0d, 0xff0c, 0xff0e, 0xff0f, 0xff1a, 0x3000, 0xff21, 0xff30, 0xff2d, 0xff41, 0xff50, 0xff4d, 0xff2d, 0xff34, 0xff33, 0xff28, 
            0x30fb
         };

        /// <summary>
        /// Fixes the Japanese characters.
        /// </summary>
        /// <param name="s">The string that contains Japanese number characters.</param>
        /// <returns>Returns the fixed number string.</returns>
        public static string FixJapaneseChars(string s)
        {
            return s;
        }

        /// <summary>
        /// Formats the specified formatted string.
        /// </summary>
        /// <typeparam name="T">The type of the formattable object.</typeparam>
        /// <param name="t">The formattable object.</param>
        /// <param name="formatString">The format string.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>Returns the formatted string.</returns>
        public static string Format<T>(T t, string formatString, IFormatProvider formatProvider) where T: IFormattable
        {
            return t.ToString(formatString, formatProvider);
        }

        /// <summary>
        /// Gets the fraction.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="denominatorDigits">The denominator digits (1-3).</param>
        /// <param name="integer">The integer.</param>
        /// <param name="numerator">The numerator.</param>
        /// <param name="denominator">The denominator.</param>
        /// <returns>Returns the fraction number information.</returns>
        public static bool GetFraction(double value, int denominatorDigits, out double integer, out double numerator, out double denominator)
        {
            numerator = 0.0;
            denominator = 0.0;
            integer = 0.0;
            double num = 0.0;
            if (value > 0.0)
            {
                num = (value - Math.Ceiling(value)) + 1.0;
                if (num == 1.0)
                {
                    num = 0.0;
                    integer = value;
                }
                else
                {
                    integer = Math.Ceiling(value) - 1.0;
                }
            }
            else if (value < 0.0)
            {
                integer = Math.Ceiling(value);
                num = Math.Ceiling(value) - value;
            }
            double num2 = 2.0;
            double num3 = 9.0;
            num2 = Math.Pow(10.0, (double) (denominatorDigits - 1));
            num3 = Math.Pow(10.0, (double) denominatorDigits) - 1.0;
            if (num2 < 2.0)
            {
                num2 = 2.0;
            }
            if (num3 < 2.0)
            {
                num3 = 2.0;
            }
            bool flag = false;
            double num4 = 0.0;
            for (double i = num2; i <= num3; i++)
            {
                double a = i * num;
                double num7 = Math.Round(a);
                double num8 = num7 / i;
                double num9 = Math.Abs((double) (num8 - num));
                if (flag ? (num9 < Math.Abs((double) (num4 - num))) : true)
                {
                    flag = true;
                    num4 = num8;
                    numerator = num7;
                    denominator = i;
                    if (num9 < 0.0005)
                    {
                        return flag;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// Parses the hex character.
        /// </summary>
        /// <param name="c">The character to parse.</param>
        /// <returns>Returns the hex value.</returns>
        public static int ParseHexChar(char c)
        {
            int num = c;
            if ((num >= 0x30) && (num <= 0x39))
            {
                return (num - 0x30);
            }
            if ((num >= 0x61) && (num <= 0x66))
            {
                return ((num - 0x61) + 10);
            }
            if ((num < 0x41) || (num > 70))
            {
                throw new FormatException(ResourceStrings.FormatIllegalCharError);
            }
            return ((num - 0x41) + 10);
        }

        /// <summary>
        /// Parses the hex string.
        /// </summary>
        /// <param name="str">The string to parse.</param>
        /// <returns>Returns the hex value.</returns>
        public static int ParseHexString(string str)
        {
            if ((str == null) || (str == string.Empty))
            {
                throw new FormatException(ResourceStrings.FormatIllegalStringError);
            }
            int num = 0;
            for (int i = 0; i < str.Length; i++)
            {
                num += ((int) ParseHexChar(str[(str.Length - i) - 1])) << (i * 4);
            }
            return num;
        }

        public static bool TryInteger(double val, out int iVal)
        {
            if ((val <= 2147483647.0) && (val >= -2147483648.0))
            {
                iVal = (int) val;
                return true;
            }
            iVal = -1;
            return false;
        }

        public static bool TryLong(double val, out long iVal)
        {
            if ((val <= 9.2233720368547758E+18) && (val >= -9.2233720368547758E+18))
            {
                iVal = (long) val;
                return true;
            }
            iVal = -1L;
            return false;
        }
    }
}

