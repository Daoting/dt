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
using System.Runtime.InteropServices;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the converting of a value to a specified data type.
    /// </summary>
    internal static class FormatConverter
    {
        /// <summary>
        /// Determines whether the specified value is a calculation error.
        /// </summary>
        /// <param name="value">Value for which to determine error</param>
        /// <returns><c>true</c> if the value is a calculation error; otherwise <c>false</c>.</returns>
        internal static bool IsError(object value)
        {
            return false;
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

        public static Windows.UI.Color ToColor(object value)
        {
            if (value is string)
            {
                Windows.UI.Color? nullable = FormatterColorHelper.FromStringValue(value.ToString());
                if (nullable.HasValue)
                {
                    return nullable.Value;
                }
            }
            else if (value is Windows.UI.Color)
            {
                return (Windows.UI.Color) value;
            }
            throw new InvalidCastException();
        }

        /// <summary>
        /// Converts the specified value to a 64-bit signed integer.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns>
        /// 64-bit signed integer equivalent to the specified value.
        /// </returns>
        /// <exception cref="T:System.InvalidCastException"></exception>
        /// <exception cref="T:System.InvalidCastException">Value cannot be converted.</exception>
        internal static long ToLong(object value, bool useCulture = true)
        {
            long num;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            try
            {
                if (value == null)
                {
                    return 0L;
                }
                if (value is double)
                {
                    return (long) ((double) value);
                }
                if (value is float)
                {
                    return (long) ((float) value);
                }
                if (value is decimal)
                {
                    return (long) ((decimal) value);
                }
                if (value is long)
                {
                    return (long) ((long) value);
                }
                if (value is int)
                {
                    return (long) ((int) value);
                }
                if (value is short)
                {
                    return (long) ((short) value);
                }
                if (value is sbyte)
                {
                    return (long) ((sbyte) value);
                }
                if (value is ulong)
                {
                    return (long) ((ulong) value);
                }
                if (value is uint)
                {
                    return (long) ((uint) value);
                }
                if (value is ushort)
                {
                    return (long) ((ushort) value);
                }
                if (value is byte)
                {
                    return (long) ((byte) value);
                }
                if (value is bool)
                {
                    return (((bool) value) ? 1L : 0L);
                }
                if (value is DateTime)
                {
                    return (long) ((DateTime) value).ToOADate();
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return (long) span.TotalDays;
                }
                num = Convert.ToInt64(value, (IFormatProvider) info);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num;
        }

        /// <summary>
        /// Converts the string to an object.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns>
        /// Returns the converted object.
        /// </returns>
        public static object ToObject(string value, bool useCulture = true)
        {
            double num;
            bool flag;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            if (double.TryParse(value, (NumberStyles)NumberStyles.Any, (IFormatProvider)info, out num))
            {
                int num2 = (int) num;
                if (num2 == num)
                {
                    return (int) num2;
                }
                return (double) num;
            }
            if (bool.TryParse(value, out flag))
            {
                return (bool) flag;
            }
            return value;
        }

        /// <summary>
        /// Converts the specified value to a string representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns>
        /// A string representation of the specified value.
        /// </returns>
        /// <exception cref="T:System.InvalidCastException"></exception>
        /// <exception cref="T:System.InvalidCastException">The value cannot be converted.</exception>
        public static string ToString(object value, bool useCulture = true)
        {
            string str;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            try
            {
                if (value == null)
                {
                    return "";
                }
                if (value is bool)
                {
                    return (((bool) value) ? "TRUE" : "FALSE");
                }
                if (value is string)
                {
                    return (string) ((string) value);
                }
                str = Convert.ToString(value, (IFormatProvider) info);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return str;
        }

        public static bool? TryBool(object val, bool useCulture = true)
        {
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
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
                try
                {
                    return new bool?(((DateTime) val).ToOADate() != 0.0);
                }
                catch
                {
                    return false;
                }
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
                return new bool?(Convert.ToBoolean(val, (IFormatProvider) info));
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

        /// <summary>
        /// Converts the specified value to a date time value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns>
        /// A date time value equivalent to the specified value.
        /// </returns>
        /// <exception cref="T:System.InvalidCastException">The value cannot be converted.</exception>
        public static DateTime? TryDateTime(object value, bool useCulture = true)
        {
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            try
            {
                if (value == null)
                {
                    return new DateTime?(DateTimeExtension.FromOADate(0.0));
                }
                if (value is double)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((double) value)));
                }
                if (value is float)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((float) value)));
                }
                if (value is decimal)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((double) ((decimal) value))));
                }
                if (value is long)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((long) value)));
                }
                if (value is int)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((int) value)));
                }
                if (value is short)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((short) value)));
                }
                if (value is sbyte)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((sbyte) value)));
                }
                if (value is ulong)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((ulong) value)));
                }
                if (value is uint)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((uint) value)));
                }
                if (value is ushort)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((ushort) value)));
                }
                if (value is byte)
                {
                    return new DateTime?(DateTimeExtension.FromOADate((double) ((byte) value)));
                }
                if (value is bool)
                {
                    return new DateTime?(DateTimeExtension.FromOADate(((bool) value) ? 1.0 : 0.0));
                }
                if (value is string)
                {
                    double num;
                    DateTime time;
                    if (double.TryParse((string)((string)value), (NumberStyles)NumberStyles.Any, (IFormatProvider)info, out num))
                    {
                        return new DateTime?(DateTimeExtension.FromOADate(num));
                    }
                    if (DateTime.TryParse((string)((string)value), (IFormatProvider)info, (DateTimeStyles)DateTimeStyles.None, out time))
                    {
                        return new DateTime?(time);
                    }
                    return null;
                }
                if (value is DateTime)
                {
                    return new DateTime?((DateTime) value);
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return new DateTime?(DateTimeExtension.FromOADate(span.TotalDays));
                }
                return new DateTime?(Convert.ToDateTime(value, (IFormatProvider) info));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Tries the double.
        /// </summary>
        /// <param name="val">The val.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns></returns>
        public static double? TryDouble(object val, bool useCulture = true)
        {
            double num;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            if (val is bool)
            {
                return new double?(((bool) val) ? 1.0 : 0.0);
            }
            if (val == null)
            {
                return 0.0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    try
                    {
                        return new double?(((DateTime) val).ToOADate());
                    }
                    catch
                    {
                        return null;
                    }
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new double?(span.TotalDays);
                }
                try
                {
                    return new double?(Convert.ToDouble(val, (IFormatProvider) info));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if ((val is string) && double.TryParse((string)(val as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)info, out num))
            {
                return new double?(num);
            }
            return null;
        }

        public static int? TryInt(object val, bool useCulture = true)
        {
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            if (val == null)
            {
                return 0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    try
                    {
                        return new int?((int) ((DateTime) val).ToOADate());
                    }
                    catch
                    {
                        return null;
                    }
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new int?((int) span.TotalDays);
                }
                try
                {
                    return new int?(Convert.ToInt32(val));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if (val is string)
            {
                int num;
                if (int.TryParse((string)(val as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)info, out num))
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

        /// <summary>
        /// Converts the specified value to a TimeSpan value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns>
        /// A TimeSpan value equivalent to the specified value.
        /// </returns>
        /// <exception cref="T:System.InvalidCastException"></exception>
        /// <exception cref="T:System.InvalidCastException">The value cannot be converted.</exception>
        public static TimeSpan? TryTimeSpan(object value, bool useCulture = true)
        {
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            try
            {
                if (value == null)
                {
                    return new TimeSpan?(TimeSpan.FromDays(0.0));
                }
                if (value is double)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((double) value)));
                }
                if (value is float)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((float) value)));
                }
                if (value is decimal)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((double) ((decimal) value))));
                }
                if (value is long)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((long) value)));
                }
                if (value is int)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((int) value)));
                }
                if (value is short)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((short) value)));
                }
                if (value is sbyte)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((sbyte) value)));
                }
                if (value is ulong)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((ulong) value)));
                }
                if (value is uint)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((uint) value)));
                }
                if (value is ushort)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((ushort) value)));
                }
                if (value is byte)
                {
                    return new TimeSpan?(TimeSpan.FromDays((double) ((byte) value)));
                }
                if (value is bool)
                {
                    return new TimeSpan?(TimeSpan.FromDays(((bool) value) ? 1.0 : 0.0));
                }
                if (value is string)
                {
                    return new TimeSpan?(TimeSpan.Parse((string) ((string) value), (IFormatProvider) info));
                }
                if (value is DateTime)
                {
                    return new TimeSpan?(TimeSpan.FromDays(((DateTime) value).ToOADate()));
                }
                if (!(value is TimeSpan))
                {
                    throw new InvalidCastException();
                }
                return new TimeSpan?((TimeSpan) value);
            }
            catch
            {
                return null;
            }
        }
    }
}

