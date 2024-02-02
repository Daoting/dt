#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal static class ConditionValueConverter
    {
        internal static bool IsNumber(object value)
        {
            return ((((((value is double) || (value is float)) || ((value is decimal) || (value is long))) || (((value is int) || (value is short)) || ((value is sbyte) || (value is ulong)))) || (((value is uint) || (value is ushort)) || ((value is byte) || (value is DateTime)))) || (value is TimeSpan));
        }

        public static double ToDouble(object value, bool useCulture = true)
        {
            double num;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
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
                    return double.Parse((string) ((string) value), (NumberStyles) NumberStyles.Any, (IFormatProvider) info);
                }
                if (value is DateTime)
                {
                    return DateTimeExtension.ToOADate((DateTime)value);
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return span.TotalDays;
                }
                num = Convert.ToDouble(value, (IFormatProvider) info);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num;
        }

        public static TimeSpan ToTimeSpan(object value, bool useCulture = true)
        {
            TimeSpan span;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
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
                    return TimeSpan.Parse((string) ((string) value), (IFormatProvider) info);
                }
                if (value is DateTime)
                {
                    return TimeSpan.FromDays(DateTimeExtension.ToOADate((DateTime) value));
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
                return new bool?(DateTimeExtension.ToOADate((DateTime) val) != 0.0);
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

        public static DateTime? TryDateTime(object val, bool useCulture = true)
        {
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
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
                double? nullable = TryDouble(val, useCulture);
                if (nullable.HasValue)
                {
                    return new DateTime?(DateTimeExtension.FromOADate(nullable.Value));
                }
            }
            else
            {
                DateTime time;
                if ((val is string) && DateTime.TryParse((string) (val as string), (IFormatProvider) info, (DateTimeStyles) DateTimeStyles.None, out time))
                {
                    return new DateTime?(time);
                }
            }
            return null;
        }

        public static double? TryDouble(object val, bool useCulture = true)
        {
            double num;
            CultureInfo info = useCulture ? CultureInfo.CurrentCulture : CultureInfo.InvariantCulture;
            if (val == null)
            {
                return 0.0;
            }
            if (IsNumber(val))
            {
                if (val is DateTime)
                {
                    return new double?(DateTimeExtension.ToOADate((DateTime) val));
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
            if ((val is string) && double.TryParse((string) (val as string), (NumberStyles) NumberStyles.Any, (IFormatProvider) info, out num))
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
                    return new int?((int) DateTimeExtension.ToOADate((DateTime) val));
                }
                if (val is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) val;
                    return new int?((int) span.TotalDays);
                }
                try
                {
                    return new int?(Convert.ToInt32(val, (IFormatProvider) info));
                }
                catch (InvalidCastException)
                {
                    return null;
                }
            }
            if (val is string)
            {
                int num;
                if (int.TryParse((string) (val as string), (NumberStyles) NumberStyles.Any, (IFormatProvider) info, out num))
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

