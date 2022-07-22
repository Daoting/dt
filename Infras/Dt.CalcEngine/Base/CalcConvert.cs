#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    /// <summary>
    /// Represents the <see cref="T:Dt.CalcEngine.CalcConvert" /> class.
    /// </summary>
    public static class CalcConvert
    {
        private static IEnumerator EnumerateCalcArray(CalcArray value)
        {
            for (int i = 0; i < value.ColumnCount; i++)
            {
                for (int j = 0; j < value.RowCount; j++)
                {
                    yield return value.GetValue(j, i);
                }
            }
        }

        private static IEnumerator EnumerateCalcRange(CalcReference value, bool excludingSubtotals)
        {
            for (int i = 0; i < value.RangeCount; i++)
            {
                for (int j = 0; j < value.GetColumnCount(i); j++)
                {
                    for (int k = 0; k < value.GetRowCount(i); k++)
                    {
                        if (value is SheetRangeReference)
                        {
                            SheetRangeReference iteratorVariable3 = value as SheetRangeReference;
                            for (int m = 0; m < iteratorVariable3.SheetCount; m++)
                            {
                                if (!excludingSubtotals || !iteratorVariable3.IsSubtotal(m, i, k, j))
                                {
                                    yield return iteratorVariable3.GetValue(m, i, k, j);
                                }
                            }
                        }
                        else if (!excludingSubtotals || !value.IsSubtotal(i, k, j))
                        {
                            yield return value.GetValue(i, k, j);
                        }
                    }
                }
            }
        }

        private static IEnumerator EnumerateScalar(object value)
        {
            yield return value;
        }
        
        /// <summary>
        /// Check whether <paramref name="value" /> is a <see cref="T:Dt.CalcEngine.CalcError" /> or not.
        /// </summary>
        /// <param name="value">The value be checked.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="value" /> is a <see cref="T:Dt.CalcEngine.CalcError" />, otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsError(object value)
        {
            return (value is CalcError);
        }

        /// <summary>
        /// Check whether <paramref name="value" /> is a number or not.
        /// </summary>
        /// <param name="value">The value be checked.</param>
        /// <returns>
        /// <see langword="true" /> if the specified value is number; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsNumber(object value)
        {
            return ((((((value is double) || (value is float)) || ((value is int) || (value is long))) || (((value is short) || (value is DateTime)) || ((value is TimeSpan) || (value is decimal)))) || (((value is sbyte) || (value is ulong)) || ((value is uint) || (value is ushort)))) || (value is byte));
        }

        internal static CalcArray ToArray(object value)
        {
            if (value is CalcArray)
            {
                return (CalcArray) value;
            }
            if (value is SheetRangeReference)
            {
                SheetRangeReference reference = value as SheetRangeReference;
                return new CalcSheetRangeWrappingCalcRange(reference.References);
            }
            if (!(value is CalcReference))
            {
                return new CalcArrayWrappingScalar(value);
            }
            CalcReference values = (CalcReference) value;
            if (values.RangeCount != 1)
            {
                throw new InvalidCastException();
            }
            return new CalcArrayWrappingCalcRange(values);
        }

        /// <summary>
        /// Convert <paramref name="value" /> to bool.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        /// A <see langword="bool" /> indicate the convert result.
        /// </returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see langword="null" />, return <see langword="false" />.
        /// If <paramref name="value" /> is <see langword="double" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0d.
        /// If <paramref name="value" /> is <see langword="float" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0f.
        /// If <paramref name="value" /> is <see langword="decimal" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0m.
        /// If <paramref name="value" /> is <see langword="long" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see cref="T:System.Int32" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="short" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="sbyte" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="ulong" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="uint" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="ushort" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="byte" />, return <see langword="true" /> only if <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="DateTime" />, return <see langword="true" /> only if the OADate of <paramref name="value" /> is not equals to 0.
        /// If <paramref name="value" /> is <see langword="TimeSpan" />, return <see langword="true" /> only if the <see cref="P:System.TimeSpan.TotalDays">TotalDays</see> of <paramref name="value" /> is not equals to 0.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see langword="bool" /></exception>
        public static bool ToBool(object value)
        {
            bool flag;
            try
            {
                if (value == null)
                {
                    return false;
                }
                if (value is double)
                {
                    return (((double) value) != 0.0);
                }
                if (value is float)
                {
                    return (((float) value) != 0f);
                }
                if (value is decimal)
                {
                    return (((decimal) value) != 0.0M);
                }
                if (value is long)
                {
                    return (((long) value) != 0L);
                }
                if (value is int)
                {
                    return (((int) value) != 0);
                }
                if (value is short)
                {
                    return (((short) value) != 0);
                }
                if (value is sbyte)
                {
                    return (((sbyte) value) != 0);
                }
                if (value is ulong)
                {
                    return (((ulong) value) != 0L);
                }
                if (value is uint)
                {
                    return (((uint) value) != 0);
                }
                if (value is ushort)
                {
                    return (((ushort) value) != 0);
                }
                if (value is byte)
                {
                    return (((byte) value) != 0);
                }
                if (value is bool)
                {
                    return (bool) ((bool) value);
                }
                if (value is DateTime)
                {
                    return (((DateTime) value).ToOADate() != 0.0);
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return (span.TotalDays != 0.0);
                }
                flag = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return flag;
        }

        /// <summary>
        /// Convert <paramref name="value" /> to <see cref="T:System.DateTime" />.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="T:System.DateTime" /> indicates the converted result.</returns>
        /// <remarks>
        /// Using  to convert  to convert, if <paramref name="value" />
        /// is not <see langword="double" />, first explicitly convert it then to date time.
        /// If <paramref name="value" /> is <see langword="null" />, pass 0d to .
        /// If <paramref name="value" /> is <see cref="T:System.String" />, try parsing with current culture.
        /// If <paramref name="value" /> is <see cref="T:System.TimeSpan" />, pass <see cref="P:System.TimeSpan.TotalDays">TotalDays</see> of <paramref name="value" />
        /// to
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see cref="T:System.DateTime" /></exception>
        public static DateTime ToDateTime(object value)
        {
            DateTime time2;
            try
            {
                if (value == null)
                {
                    return DateTimeExtension.FromOADate(0.0);
                }
                if (value is double)
                {
                    return DateTimeExtension.FromOADate((double) ((double) value));
                }
                if (value is float)
                {
                    return DateTimeExtension.FromOADate((double) ((float) value));
                }
                if (value is decimal)
                {
                    return DateTimeExtension.FromOADate((double) ((double) ((decimal) value)));
                }
                if (value is long)
                {
                    return DateTimeExtension.FromOADate((double) ((long) value));
                }
                if (value is int)
                {
                    return DateTimeExtension.FromOADate((double) ((int) value));
                }
                if (value is short)
                {
                    return DateTimeExtension.FromOADate((double) ((short) value));
                }
                if (value is sbyte)
                {
                    return DateTimeExtension.FromOADate((double) ((sbyte) value));
                }
                if (value is ulong)
                {
                    return DateTimeExtension.FromOADate((double) ((ulong) value));
                }
                if (value is uint)
                {
                    return DateTimeExtension.FromOADate((double) ((uint) value));
                }
                if (value is ushort)
                {
                    return DateTimeExtension.FromOADate((double) ((ushort) value));
                }
                if (value is byte)
                {
                    return DateTimeExtension.FromOADate((double) ((byte) value));
                }
                if (value is bool)
                {
                    return DateTimeExtension.FromOADate(((bool) value) ? 1.0 : 0.0);
                }
                if (value is DateTime)
                {
                    return (DateTime) value;
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return DateTimeExtension.FromOADate(span.TotalDays);
                }
                if (value is string)
                {
                    DateTime time;
                    if (!DateTime.TryParse((string) ((string) value), CultureInfo.CurrentCulture, DateTimeStyles.None, out time) && !DateTime.TryParse((string) ((string) value), new CultureInfo("en"), DateTimeStyles.None, out time))
                    {
                        throw new InvalidCastException();
                    }
                    return time;
                }
                time2 = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return time2;
        }

        /// <summary>
        /// Convert <paramref name="value" /> to <see langword="double" />.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see langword="double" /> indicate the convert result.</returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see langword="null" />, return 0d.
        /// If <paramref name="value" /> is <see cref="T:System.Int32" />, or <see langword="float" />, or <see langword="decimal" />, 
        /// or<see langword="long" />, or <see langword="short" />, or <see langword="sbyte" />, or <see langword="ulong" />, 
        /// or<see langword="uint" />, or <see langword="ushort" />, or <see langword="byte" />, explicitly convert it to <see langword="double" />.
        /// If <paramref name="value" /> is <see langword="bool" />, return 1d if the <paramref name="value" /> is <see langword="true" />, otherwise return 0d.
        /// If <paramref name="value" /> is <see cref="T:System.DateTime" />, return OADate of <paramref name="value" />.
        /// If <paramref name="value" /> is <see cref="T:System.TimeSpan" />, return <see cref="P:System.TimeSpan.TotalDays">TotalDays</see> of <paramref name="value" />.
        /// If <paramref name="value" /> is <see cref="T:System.String" />, first try parse it to double with current culture and any number style.
        /// if failed, then try parse with English culture (en) and any number style.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see langword="double" /></exception>
        public static double ToDouble(object value)
        {
            double num2;
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
                    double num;
                    if (!double.TryParse((string) ((string) value), NumberStyles.Any, CultureInfo.CurrentCulture, out num) && !double.TryParse((string) ((string) value), NumberStyles.Any, new CultureInfo("en"), out num))
                    {
                        throw new InvalidCastException();
                    }
                    return num;
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
                num2 = Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num2;
        }

        /// <summary>
        /// Get an enumerator to iterate items in value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="excludingSubtotals">if set to <see langword="true" /> excluding subtotals.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> indicate the iterator.</returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see cref="T:Dt.CalcEngine.CalcArray" />, it would iterate from each columns to rows.
        /// If <paramref name="value" /> is <see cref="T:Dt.CalcEngine.CalcReference" />, it would iterate all sub-area from each columns to rows.
        /// Any others, iterate the <paramref name="value" /> as an enumerator.
        /// </remarks>
        public static IEnumerator ToEnumerator(object value, bool excludingSubtotals = false)
        {
            if (value is CalcArray)
            {
                return EnumerateCalcArray(value as CalcArray);
            }
            if (value is CalcReference)
            {
                return EnumerateCalcRange(value as CalcReference, excludingSubtotals);
            }
            return EnumerateScalar(value);
        }

        /// <summary>
        /// Get an enumerator to iterate items in value with excluding subtotals item.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> indicate the iterator.</returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see cref="T:Dt.CalcEngine.CalcArray" />, it would iterate from each columns to rows.
        /// If <paramref name="value" /> is <see cref="T:Dt.CalcEngine.CalcReference" />, it would iterate all sub-area from each columns to rows.
        /// Any others, iterate the <paramref name="value" /> as an enumerator.
        /// </remarks>
        public static IEnumerator ToEnumeratorExcludingSubtotals(object value)
        {
            return ToEnumerator(value, true);
        }

        /// <summary>
        /// Convert <paramref name="value" /> to <see cref="T:System.Int32" />.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="T:System.Int32" /> indicate the convert result.</returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see langword="null" />, return 0.
        /// If <paramref name="value" /> is <see langword="double" />, or <see langword="float" />, or <see langword="decimal" />, 
        /// or<see langword="long" />, or <see langword="short" />, or <see langword="sbyte" />, or <see langword="ulong" />, 
        /// or<see langword="uint" />, or <see langword="ushort" />, or <see langword="byte" />, explicitly convert it to <see cref="T:System.Int32" />.
        /// If <paramref name="value" /> is <see langword="bool" />, return 1 if the <paramref name="value" /> is <see langword="true" />, otherwise return 0.
        /// If <paramref name="value" /> is <see cref="T:System.DateTime" />, return OADate of <paramref name="value" />.
        /// If <paramref name="value" /> is <see cref="T:System.TimeSpan" />, return <see cref="P:System.TimeSpan.TotalDays">TotalDays</see> of <paramref name="value" />.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see cref="T:System.Int32" /></exception>
        public static int ToInt(object value)
        {
            int num;
            try
            {
                if (value == null)
                {
                    return 0;
                }
                if (value is double)
                {
                    return (int) ((double) value);
                }
                if (value is float)
                {
                    return (int) ((float) value);
                }
                if (value is decimal)
                {
                    return (int) ((decimal) value);
                }
                if (value is long)
                {
                    return (int) ((long) value);
                }
                if (value is int)
                {
                    return (int) ((int) value);
                }
                if (value is short)
                {
                    return (short) value;
                }
                if (value is sbyte)
                {
                    return (sbyte) value;
                }
                if (value is ulong)
                {
                    return (int) ((ulong) value);
                }
                if (value is uint)
                {
                    return (int) ((uint) value);
                }
                if (value is ushort)
                {
                    return (ushort) value;
                }
                if (value is byte)
                {
                    return (byte) value;
                }
                if (value is bool)
                {
                    return (((bool) value) ? 1 : 0);
                }
                if (value is DateTime)
                {
                    return (int) ((DateTime) value).ToOADate();
                }
                if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    return (int) span.TotalDays;
                }
                num = Convert.ToInt32(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num;
        }

        /// <summary>
        /// Convert <paramref name="value" /> to <see langword="long" />.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see langword="long" /> indicate the convert result.</returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see langword="null" />, return 0L.
        /// If <paramref name="value" /> is <see cref="T:System.Int32" />, or <see langword="float" />, or <see langword="decimal" />, 
        /// or<see langword="long" />, or <see langword="short" />, or <see langword="sbyte" />, or <see langword="ulong" />, 
        /// or<see langword="uint" />, or <see langword="ushort" />, or <see langword="byte" />, explicitly convert it to <see langword="long" />.
        /// If <paramref name="value" /> is <see langword="bool" />, return 1L if the <paramref name="value" /> is <see langword="true" />, otherwise return 0L.
        /// If <paramref name="value" /> is <see cref="T:System.DateTime" />, return OADate of <paramref name="value" />.
        /// If <paramref name="value" /> is <see cref="T:System.TimeSpan" />, return <see cref="P:System.TimeSpan.TotalDays">TotalDays</see> of <paramref name="value" />.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see langword="long" /></exception>
        public static long ToLong(object value)
        {
            long num2;
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
                if (value is string)
                {
                    long num;
                    if (!long.TryParse((string) ((string) value), NumberStyles.Any, CultureInfo.CurrentCulture, out num) && !long.TryParse((string) ((string) value), NumberStyles.Any, new CultureInfo("en"), out num))
                    {
                        throw new InvalidCastException();
                    }
                    return num;
                }
                num2 = Convert.ToInt64(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return num2;
        }

        /// <summary>
        /// Convert <paramref name="value" /> to the result.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        /// return <see cref="F:Dt.CalcEngine.CalcErrors.Number" /> if <paramref name="value" /> is not a number or is infinity.
        /// </returns>
        public static object ToResult(double value)
        {
            if (!double.IsNaN(value) && !double.IsInfinity(value))
            {
                return (double) value;
            }
            return CalcErrors.Number;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String" /> that represents this instance.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>
        /// A <see cref="T:System.String" /> that represents this instance.
        /// </returns>
        /// <remarks>
        /// If <paramref name="value" /> is <see langword="null" />, return <see cref="F:System.String.Empty" />.
        /// If <paramref name="value" /> is <see langword="bool" />, return <b>TRUE</b> if <paramref name="value" /> is <see langword="true" />;
        /// return <b>FALSE</b> if <paramref name="value" /> is <see langword="false" />.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">If <paramref name="value" /> is <see cref="T:Dt.CalcEngine.CalcArray" />, or can not convert to string.</exception>
        public static string ToString(object value)
        {
            string str;
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
                if (value is CalcArray)
                {
                    throw new InvalidCastException();
                }
                str = Convert.ToString(value, CultureInfo.CurrentCulture);
            }
            catch
            {
                throw new InvalidCastException();
            }
            return str;
        }

        /// <summary>
        /// Convert <paramref name="value" /> to <see cref="T:System.TimeSpan" />.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <returns>A <see cref="T:System.TimeSpan" /> indicates the converted result.</returns>
        /// <remarks>
        /// Using <see cref="M:System.TimeSpan.FromDays(System.Double)" /> to convert <paramref name="value" /> to convert, if <paramref name="value" />
        /// is not <see langword="double" />, first explicitly convert it then to date time.
        /// If <paramref name="value" /> is <see langword="null" />, pass 0d to <see cref="M:System.TimeSpan.FromDays(System.Double)" />.
        /// If <paramref name="value" /> is <see cref="T:System.String" />, try parsing with current culture.
        /// If <paramref name="value" /> is <see cref="T:System.DateTime" />, pass OADate of <paramref name="value" />
        /// to <see cref="M:System.TimeSpan.FromDays(System.Double)" />.
        /// </remarks>
        /// <exception cref="T:System.InvalidCastException">Can not convert <paramref name="value" /> to <see cref="T:System.TimeSpan" /></exception>
        public static TimeSpan ToTimeSpan(object value)
        {
            TimeSpan span2;
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
                    TimeSpan span;
                    if (!TimeSpan.TryParse((string) ((string) value), CultureInfo.CurrentCulture, out span) && !TimeSpan.TryParse((string) ((string) value), new CultureInfo("en"), out span))
                    {
                        throw new InvalidCastException();
                    }
                    return span;
                }
                if (value is DateTime)
                {
                    return TimeSpan.FromDays(((DateTime) value).ToOADate());
                }
                if (!(value is TimeSpan))
                {
                    throw new InvalidCastException();
                }
                span2 = (TimeSpan) value;
            }
            catch
            {
                throw new InvalidCastException();
            }
            return span2;
        }

        internal static bool TryToBool(object value, out bool result)
        {
            try
            {
                if (value == null)
                {
                    result = false;
                }
                else if (value is double)
                {
                    result = ((double) value) != 0.0;
                }
                else if (value is float)
                {
                    result = ((float) value) != 0f;
                }
                else if (value is decimal)
                {
                    result = ((decimal) value) != 0.0M;
                }
                else if (value is long)
                {
                    result = ((long) value) != 0L;
                }
                else if (value is int)
                {
                    result = ((int) value) != 0;
                }
                else if (value is short)
                {
                    result = ((short) value) != 0;
                }
                else if (value is sbyte)
                {
                    result = ((sbyte) value) != 0;
                }
                else if (value is ulong)
                {
                    result = ((ulong) value) != 0L;
                }
                else if (value is uint)
                {
                    result = ((uint) value) != 0;
                }
                else if (value is ushort)
                {
                    result = ((ushort) value) != 0;
                }
                else if (value is byte)
                {
                    result = ((byte) value) != 0;
                }
                else if (value is bool)
                {
                    result = (bool) ((bool) value);
                }
                else if (value is DateTime)
                {
                    result = ((DateTime) value).ToOADate() != 0.0;
                }
                else if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    result = span.TotalDays != 0.0;
                }
                else
                {
                    result = Convert.ToBoolean(value, CultureInfo.CurrentCulture);
                }
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (FormatException)
            {
            }
            catch (InvalidCastException)
            {
            }
            result = false;
            return false;
        }

        /// <summary>
        /// Try convert value to DateTime.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns></returns>
        internal static bool TryToDateTime(object value, out DateTime result, bool useCulture = true)
        {
            try
            {
                if (value == null)
                {
                    result = DateTimeExtension.FromOADate(0.0);
                }
                else if (value is double)
                {
                    result = DateTimeExtension.FromOADate((double) ((double) value));
                }
                else if (value is float)
                {
                    result = DateTimeExtension.FromOADate((double) ((float) value));
                }
                else if (value is decimal)
                {
                    result = DateTimeExtension.FromOADate((double) ((double) ((decimal) value)));
                }
                else if (value is long)
                {
                    result = DateTimeExtension.FromOADate((double) ((long) value));
                }
                else if (value is int)
                {
                    result = DateTimeExtension.FromOADate((double) ((int) value));
                }
                else if (value is short)
                {
                    result = DateTimeExtension.FromOADate((double) ((short) value));
                }
                else if (value is sbyte)
                {
                    result = DateTimeExtension.FromOADate((double) ((sbyte) value));
                }
                else if (value is ulong)
                {
                    result = DateTimeExtension.FromOADate((double) ((ulong) value));
                }
                else if (value is uint)
                {
                    result = DateTimeExtension.FromOADate((double) ((uint) value));
                }
                else if (value is ushort)
                {
                    result = DateTimeExtension.FromOADate((double) ((ushort) value));
                }
                else if (value is byte)
                {
                    result = DateTimeExtension.FromOADate((double) ((byte) value));
                }
                else if (value is bool)
                {
                    result = DateTimeExtension.FromOADate(((bool) value) ? 1.0 : 0.0);
                }
                else if (value is DateTime)
                {
                    result = (DateTime) value;
                }
                else if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    result = DateTimeExtension.FromOADate(span.TotalDays);
                }
                else if (value is string)
                {
                    if (useCulture)
                    {
                        if (!DateTime.TryParse((string) ((string) value), CultureInfo.CurrentCulture, DateTimeStyles.None, out result) && !DateTime.TryParse((string) ((string) value), new CultureInfo("en"), DateTimeStyles.None, out result))
                        {
                            return false;
                        }
                    }
                    else if (!DateTime.TryParse((string) ((string) value), CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                    {
                        return false;
                    }
                }
                else
                {
                    try
                    {
                        result = Convert.ToDateTime(value, CultureInfo.CurrentCulture);
                        return true;
                    }
                    catch (FormatException)
                    {
                    }
                    result = new DateTime();
                    return false;
                }
                return true;
            }
            catch (ArgumentException)
            {
            }
            result = new DateTime();
            return false;
        }

        /// <summary>
        /// Try to <paramref name="value"></paramref> to <see langword="double" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="number">The number.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns></returns>
        internal static bool TryToDouble(object value, out double number, bool useCulture = true)
        {
            if (value == null)
            {
                number = 0.0;
            }
            else if (value is double)
            {
                number = (double) ((double) value);
            }
            else if (value is float)
            {
                number = (float) value;
            }
            else if (value is decimal)
            {
                number = (double) ((double) ((decimal) value));
            }
            else if (value is long)
            {
                number = (long) value;
            }
            else if (value is int)
            {
                number = (int) value;
            }
            else if (value is short)
            {
                number = (short) value;
            }
            else if (value is sbyte)
            {
                number = (sbyte) value;
            }
            else if (value is ulong)
            {
                number = (ulong) value;
            }
            else if (value is uint)
            {
                number = (uint) value;
            }
            else if (value is ushort)
            {
                number = (ushort) value;
            }
            else if (value is byte)
            {
                number = (byte) value;
            }
            else if (value is bool)
            {
                number = ((bool) value) ? 1.0 : 0.0;
            }
            else if (value is string)
            {
                if (!useCulture)
                {
                    if (!double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                    {
                        return false;
                    }
                }
                else if (!double.TryParse((string) ((string) value), NumberStyles.Any, CultureInfo.CurrentCulture, out number) && !double.TryParse((string) ((string) value), NumberStyles.Any, new CultureInfo("en"), out number))
                {
                    return false;
                }
            }
            else if (value is DateTime)
            {
                number = ((DateTime) value).ToOADate();
            }
            else if (value is TimeSpan)
            {
                TimeSpan span = (TimeSpan) value;
                number = span.TotalDays;
            }
            else
            {
                try
                {
                    number = Convert.ToDouble(value, CultureInfo.CurrentCulture);
                }
                catch
                {
                    number = 0.0;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Try convert value to int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        internal static bool TryToInt(object value, out int result)
        {
            try
            {
                if (value == null)
                {
                    result = 0;
                }
                else if (value is double)
                {
                    result = (int) ((double) value);
                }
                else if (value is float)
                {
                    result = (int) ((float) value);
                }
                else if (value is decimal)
                {
                    result = (int) ((decimal) value);
                }
                else if (value is long)
                {
                    result = (int) ((long) value);
                }
                else if (value is int)
                {
                    result = (int) ((int) value);
                }
                else if (value is short)
                {
                    result = (short) value;
                }
                else if (value is sbyte)
                {
                    result = (sbyte) value;
                }
                else if (value is ulong)
                {
                    result = (int) ((ulong) value);
                }
                else if (value is uint)
                {
                    result = (int) ((uint) value);
                }
                else if (value is ushort)
                {
                    result = (ushort) value;
                }
                else if (value is byte)
                {
                    result = (byte) value;
                }
                else if (value is bool)
                {
                    result = ((bool) value) ? 1 : 0;
                }
                else if (value is DateTime)
                {
                    result = (int) ((DateTime) value).ToOADate();
                }
                else if (value is TimeSpan)
                {
                    TimeSpan span = (TimeSpan) value;
                    result = (int) span.TotalDays;
                }
                else
                {
                    result = Convert.ToInt32(value, CultureInfo.CurrentCulture);
                }
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (FormatException)
            {
            }
            result = 0;
            return false;
        }

        /// <summary>
        /// Try convert value to TimeSpan.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="result">The result.</param>
        /// <param name="useCulture">if set to <c>true</c> [use culture].</param>
        /// <returns></returns>
        internal static bool TryToTimeSpan(object value, out TimeSpan result, bool useCulture = true)
        {
            try
            {
                if (value == null)
                {
                    result = TimeSpan.FromDays(0.0);
                }
                else if (value is double)
                {
                    result = TimeSpan.FromDays((double) ((double) value));
                }
                else if (value is float)
                {
                    result = TimeSpan.FromDays((double) ((float) value));
                }
                else if (value is decimal)
                {
                    result = TimeSpan.FromDays((double) ((double) ((decimal) value)));
                }
                else if (value is long)
                {
                    result = TimeSpan.FromDays((double) ((long) value));
                }
                else if (value is int)
                {
                    result = TimeSpan.FromDays((double) ((int) value));
                }
                else if (value is short)
                {
                    result = TimeSpan.FromDays((double) ((short) value));
                }
                else if (value is sbyte)
                {
                    result = TimeSpan.FromDays((double) ((sbyte) value));
                }
                else if (value is ulong)
                {
                    result = TimeSpan.FromDays((double) ((ulong) value));
                }
                else if (value is uint)
                {
                    result = TimeSpan.FromDays((double) ((uint) value));
                }
                else if (value is ushort)
                {
                    result = TimeSpan.FromDays((double) ((ushort) value));
                }
                else if (value is byte)
                {
                    result = TimeSpan.FromDays((double) ((byte) value));
                }
                else if (value is bool)
                {
                    result = TimeSpan.FromDays(((bool) value) ? 1.0 : 0.0);
                }
                else if (value is DateTime)
                {
                    result = TimeSpan.FromDays(((DateTime) value).ToOADate());
                }
                else if (value is TimeSpan)
                {
                    result = (TimeSpan) value;
                }
                else if (value is string)
                {
                    if (useCulture)
                    {
                        if (!TimeSpan.TryParse((string) ((string) value), CultureInfo.CurrentCulture, out result) && !TimeSpan.TryParse((string) ((string) value), new CultureInfo("en"), out result))
                        {
                            return false;
                        }
                    }
                    else if (!TimeSpan.TryParse((string) ((string) value), CultureInfo.InvariantCulture, out result))
                    {
                        return false;
                    }
                }
                else
                {
                    result = new TimeSpan();
                    return false;
                }
                return true;
            }
            catch (OverflowException)
            {
            }
            catch (ArgumentException)
            {
            }
            result = new TimeSpan();
            return false;
        }
        
        private class CalcArrayWrappingCalcRange : CalcArray
        {
            private CalcReference values;

            public CalcArrayWrappingCalcRange(CalcReference values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                return this.values.GetValue(0, row, column);
            }

            public override int ColumnCount
            {
                get
                {
                    return this.values.GetColumnCount(0);
                }
            }

            public override int RowCount
            {
                get
                {
                    return this.values.GetRowCount(0);
                }
            }
        }

        private class CalcArrayWrappingScalar : CalcArray
        {
            private object value;

            public CalcArrayWrappingScalar(object value)
            {
                this.value = value;
            }

            public override object GetValue(int row, int column)
            {
                return this.value;
            }

            public override int ColumnCount
            {
                get
                {
                    return 1;
                }
            }

            public override int RowCount
            {
                get
                {
                    return 1;
                }
            }
        }

        private class CalcSheetRangeWrappingCalcRange : CalcArray
        {
            private List<CalcReference> values;

            public CalcSheetRangeWrappingCalcRange(List<CalcReference> values)
            {
                this.values = values;
            }

            public override object GetValue(int row, int column)
            {
                int num = column / this.values[0].GetRowCount(0);
                int rowOffset = row / this.values[0].GetRowCount(0);
                return this.values[num].GetValue(0, rowOffset, column);
            }

            public override int ColumnCount
            {
                get
                {
                    return this.values[0].GetColumnCount(0);
                }
            }

            public override int RowCount
            {
                get
                {
                    return (this.values.Count * this.values[0].GetRowCount(0));
                }
            }
        }
    }
}

