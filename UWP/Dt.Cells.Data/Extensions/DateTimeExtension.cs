#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
using Windows.Globalization.DateTimeFormatting;
#endregion

namespace Dt.Cells.Data
{
    internal static class DateTimeExtension
    {
        const int DatePartMonth = 2;
        const long DoubleDateOffset = 0x85103c0cb83c000L;
        const long MaxMillis = 0x11efae44cb400L;
        const int MillisPerDay = 0x5265c00;
        const double OADateMaxAsDouble = 2958466.0;
        const double OADateMinAsDouble = -657435.0;
        const long OADateMinAsTicks = 0x6efdddaec64000L;
        const long TicksPerDay = 0xc92a69c000L;
        const long TicksPerMillisecond = 0x2710L;

        internal static long DoubleDateToTicks(double value)
        {
            if ((value >= 2958466.0) || (value <= -657435.0))
            {
                throw new ArgumentException("Arg_OleAutDateInvalid");
            }
            long num =(long) ((value * 86400000.0) + ((value >= 0.0) ? 0.5 : -0.5));
            if (num < 0L)
            {
                num -= (num % 0x5265c00L) * 2L;
            }
            num += 0x3680b5e1fc00L;
            if ((num < 0L) || (num >= 0x11efae44cb400L))
            {
                throw new ArgumentException("Arg_OleAutDateScale");
            }
            return (num * 0x2710L);
        }

        public static DateTime FromOADate(double d)
        {
            return new DateTime(DoubleDateToTicks(d), (DateTimeKind) DateTimeKind.Unspecified);
        }

        static double TicksToOADate(long value)
        {
            if (value == 0L)
            {
                return 0.0;
            }
            if (value < 0xc92a69c000L)
            {
                value += 0x85103c0cb83c000L;
            }
            if (value < 0x6efdddaec64000L)
            {
                throw new OverflowException("Arg_OleAutDateInvalid");
            }
            long num = (value - 0x85103c0cb83c000L) / 0x2710L;
            if (num < 0L)
            {
                long num2 = num % 0x5265c00L;
                if (num2 != 0L)
                {
                    num -= (0x5265c00L + num2) * 2L;
                }
            }
            return (((double) num) / 86400000.0);
        }

        public static double ToOADate(this DateTime This)
        {
            return TicksToOADate(This.Ticks);
        }

        internal static string ToShortDateString(this DateTime This)
        {
            return DateTimeFormatter.ShortDate.Format((DateTimeOffset) This);
        }

        internal static string ToShortTimeString(this DateTime This)
        {
            return DateTimeFormatter.ShortTime.Format((DateTimeOffset) This);
        }
    }
}

