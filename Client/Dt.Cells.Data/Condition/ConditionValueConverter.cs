#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Globalization;
using System.Reflection;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    internal static class ConditionValueConverter
    {
        /// <summary>
        /// Converts a color from a string value.
        /// </summary>
        /// <param name="color">The color (#ff223344).</param>
        /// <returns>Returns the color.</returns>
        public static Windows.UI.Color? ColorFromStringValue(string color)
        {
            if ((color != null) && color.StartsWith("#"))
            {
                try
                {
                    int num = ParseHexString(color.TrimStart(new char[] { '#' }));
                    return new Windows.UI.Color?(Windows.UI.Color.FromArgb((byte) ((num & 0xff000000L) >> 0x18), (byte) ((num & 0xff0000) >> 0x10), (byte) ((num & 0xff00) >> 8), (byte) (num & 0xff)));
                }
                catch
                {
                }
            }
            return null;
        }

        /// <summary>
        /// Determines whether the specified value is a numeric data type.
        /// </summary>
        /// <param name="value">Value for which to determine data type</param>
        /// <returns><c>true</c> if the value is numeric; otherwise <c>false</c>.</returns>
        internal static bool IsNumber(object value)
        {
            return ((((((value is double) || (value is float)) || ((value is decimal) || (value is long))) || (((value is int) || (value is short)) || ((value is sbyte) || (value is ulong)))) || (((value is uint) || (value is ushort)) || ((value is byte) || (value is DateTime)))) || (value is TimeSpan));
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

        /// <summary>
        /// Converts the specified value to a double-precision, floating-point number.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>Double-precision, floating-point number equivalent to the specified value.</returns>
        /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
        public static double ToDouble(object value)
        {
            double num;
            try
            {
                if (value == null)
                {
                    return 0.0;
                }
                if (value is double)
                {
                    return (double) ((double) value);
                }
                if (value is float)
                {
                    return (double) ((float) value);
                }
                if (value is decimal)
                {
                    return (double) ((double) ((decimal) value));
                }
                if (value is long)
                {
                    return (double) ((long) value);
                }
                if (value is int)
                {
                    return (double) ((int) value);
                }
                if (value is short)
                {
                    return (double) ((short) value);
                }
                if (value is sbyte)
                {
                    return (double) ((sbyte) value);
                }
                if (value is ulong)
                {
                    return (double) ((ulong) value);
                }
                if (value is uint)
                {
                    return (double) ((uint) value);
                }
                if (value is ushort)
                {
                    return (double) ((ushort) value);
                }
                if (value is byte)
                {
                    return (double) ((byte) value);
                }
                if (value is bool)
                {
                    return (((bool) value) ? 1.0 : 0.0);
                }
                if (value is string)
                {
                    return double.Parse((string) ((string) value), (NumberStyles) NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture);
                }
                if (value is DateTime)
                {
                    return ((DateTime) value).ToOADate();
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return span.TotalDays;
                }
                num = Convert.ToDouble(value, (IFormatProvider) CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num;
        }

        /// <summary>
        /// Converts the specified value to a TimeSpan value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>TimeSpan value equivalent to the specified value.</returns>
        /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
        public static TimeSpan ToTimeSpan(object value)
        {
            TimeSpan span;
            try
            {
                if (value == null)
                {
                    return TimeSpan.FromDays(0.0);
                }
                if (value is double)
                {
                    return TimeSpan.FromDays((double) ((double) value));
                }
                if (value is float)
                {
                    return TimeSpan.FromDays((double) ((float) value));
                }
                if (value is decimal)
                {
                    return TimeSpan.FromDays((double) ((double) ((decimal) value)));
                }
                if (value is long)
                {
                    return TimeSpan.FromDays((double) ((long) value));
                }
                if (value is int)
                {
                    return TimeSpan.FromDays((double) ((int) value));
                }
                if (value is short)
                {
                    return TimeSpan.FromDays((double) ((short) value));
                }
                if (value is sbyte)
                {
                    return TimeSpan.FromDays((double) ((sbyte) value));
                }
                if (value is ulong)
                {
                    return TimeSpan.FromDays((double) ((ulong) value));
                }
                if (value is uint)
                {
                    return TimeSpan.FromDays((double) ((uint) value));
                }
                if (value is ushort)
                {
                    return TimeSpan.FromDays((double) ((ushort) value));
                }
                if (value is byte)
                {
                    return TimeSpan.FromDays((double) ((byte) value));
                }
                if (value is bool)
                {
                    return TimeSpan.FromDays(((bool) value) ? 1.0 : 0.0);
                }
                if (value is string)
                {
                    return TimeSpan.Parse((string) ((string) value), (IFormatProvider) CultureInfo.InvariantCulture);
                }
                if (value is DateTime)
                {
                    return TimeSpan.FromDays(((DateTime) value).ToOADate());
                }
                if (!(value is TimeSpan))
                {
                    throw new InvalidCastException();
                }
                span = (TimeSpan) value;
            }
            catch
            {
                throw new InvalidCastException();
            }
            return span;
        }

        public static bool? TryBool(object val)
        {
            if (val == null)
            {
                return false;
            }
            if (val is bool)
            {
                return new bool?((bool) ((bool) val));
            }
            if (!IsNumber(val) && !(val is string))
            {
                return null;
            }
            if (val is DateTime)
            {
                return new bool?(((DateTime) val).ToOADate() != 0.0);
            }
            if (val is TimeSpan)
            {
                TimeSpan span = (TimeSpan) val;
                return new bool?(span.TotalDays != 0.0);
            }
            if (val is string)
            {
                bool flag;
                if (bool.TryParse((string) (val as string), out flag))
                {
                    return new bool?(flag);
                }
                return null;
            }
            try
            {
                return new bool?(Convert.ToBoolean(val));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public static Windows.UI.Color? TryColor(object val)
        {
            if (val is string)
            {
                Windows.UI.Color? nullable = ColorFromStringValue(val.ToString());
                if (nullable.HasValue)
                {
                    return new Windows.UI.Color?(nullable.Value);
                }
            }
            else if (val is Windows.UI.Color)
            {
                return new Windows.UI.Color?((Windows.UI.Color) val);
            }
            return null;
        }

        public static DateTime? TryDateTime(object val)
        {
            if (val == null)
            {
                return new DateTime?(DateTimeExtension.FromOADate(0.0));
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    return new DateTime?((DateTime) val);
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new DateTime?(DateTimeExtension.FromOADate(span.TotalDays));
                }
                double? nullable = TryDouble(val);
                if (nullable.HasValue)
                {
                    return new DateTime?(DateTimeExtension.FromOADate(nullable.Value));
                }
            }
            else
            {
                DateTime time;
                if ((val is string) && DateTime.TryParse((string)(val as string), (IFormatProvider)CultureInfo.InvariantCulture, (DateTimeStyles)DateTimeStyles.None, out time))
                {
                    return new DateTime?(time);
                }
            }
            return null;
        }

        public static double? TryDouble(object val)
        {
            double num;
            if (val == null)
            {
                return 0.0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    return new double?(((DateTime) val).ToOADate());
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new double?(span.TotalDays);
                }
                try
                {
                    return new double?(Convert.ToDouble(val, (IFormatProvider) CultureInfo.InvariantCulture));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if ((val is string) && double.TryParse((string)(val as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
            {
                return new double?(num);
            }
            return null;
        }

        public static int? TryInt(object val)
        {
            if (val == null)
            {
                return 0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    return new int?((int) ((DateTime) val).ToOADate());
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new int?((int) span.TotalDays);
                }
                try
                {
                    return new int?(Convert.ToInt32(val, (IFormatProvider) CultureInfo.InvariantCulture));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if (val is string)
            {
                int num;
                if (int.TryParse((string)(val as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out num))
                {
                    return new int?(num);
                }
            }
            else if (IntrospectionExtensions.GetTypeInfo(val.GetType()).IsEnum)
            {
                return new int?((int) ((int) val));
            }
            return null;
        }
    }
}

