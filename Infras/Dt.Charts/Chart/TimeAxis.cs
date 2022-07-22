#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Charts
{
    internal class TimeAxis
    {
        static long getNiceInc(int[] tik, long ts, long mult)
        {
            for (int i = 0; i < tik.Length; i++)
            {
                long num2 = tik[i] * mult;
                if (ts <= num2)
                {
                    return num2;
                }
            }
            return 0L;
        }

        public static string GetTimeDefaultFormat(double maxdate, double mindate)
        {
            if (double.IsNaN(maxdate) || double.IsNaN(mindate))
            {
                return "";
            }
            string str = "s";
            DateTime time = maxdate.FromOADate();
            DateTime time2 = mindate.FromOADate();
            TimeSpan span = time.Subtract(time2);
            long totalSeconds = (long) span.TotalSeconds;
            if (totalSeconds > 0x3c26700L)
            {
                return "yyyy";
            }
            if (totalSeconds > 0x1e13380L)
            {
                return "MMM yy";
            }
            if (totalSeconds > 0x7a9b80L)
            {
                return "MMM";
            }
            if (totalSeconds > 0x127500L)
            {
                return "MMM d";
            }
            if (totalSeconds > 0x2a300L)
            {
                return "ddd d";
            }
            if (totalSeconds > 0x15180L)
            {
                return "ddd H:mm";
            }
            if (totalSeconds > 0xe10L)
            {
                return "H:mm";
            }
            if (totalSeconds >= 1L)
            {
                return "H:mm:ss";
            }
            if (totalSeconds > 0L)
            {
                long ticks = span.Ticks;
                str = "s" + NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;
                while (ticks < 0x989680L)
                {
                    ticks *= 10L;
                    str = str + "f";
                }
            }
            return str;
        }

        static tmInc manualTimeInc(string manualformat)
        {
            tmInc second = tmInc.second;
            if ((manualformat == null) || (manualformat.Length == 0))
            {
                return second;
            }
            int index = manualformat.IndexOf('f');
            if (index < 0)
            {
                if (manualformat.IndexOf('s') >= 0)
                {
                    return tmInc.second;
                }
                if (manualformat.IndexOf('m') >= 0)
                {
                    return tmInc.minute;
                }
                if (manualformat.IndexOfAny(new char[] { 'h', 'H' }) >= 0)
                {
                    return tmInc.hour;
                }
                if (manualformat.IndexOf('d') >= 0)
                {
                    return tmInc.day;
                }
                if (manualformat.IndexOf('M') >= 0)
                {
                    return tmInc.month;
                }
                if (manualformat.IndexOf('y') >= 0)
                {
                    second = tmInc.year;
                }
                return second;
            }
            int num2 = -1;
            if ((index > 0) && manualformat.Substring(index - 1, 1).Equals((char) '%'))
            {
                num2 = -1;
            }
            else
            {
                for (int i = 1; i < 6; i++)
                {
                    if (((index + i) >= manualformat.Length) || (manualformat.Substring(index + i, 1) != "f"))
                    {
                        break;
                    }
                    num2--;
                }
            }
            return (tmInc) num2;
        }

        public static TimeSpan NiceTimeSpan(TimeSpan ts, string manualformat)
        {
            tmInc second = tmInc.second;
            if (!string.IsNullOrEmpty(manualformat))
            {
                second = manualTimeInc(manualformat);
            }
            long ticks = 0L;
            long num2 = 0L;
            if ((second < tmInc.second) && (ts.TotalSeconds < 10.0))
            {
                ticks = ts.Ticks;
                num2 = TimeSpanFromTmInc(second).Ticks;
                while (ticks > (10L * num2))
                {
                    num2 *= 10L;
                }
                long num3 = num2;
                if (ticks > num3)
                {
                    num3 *= 2L;
                }
                if (ticks > num3)
                {
                    num3 = 5L * num2;
                }
                if (ticks > num3)
                {
                    num3 = 10L * num2;
                }
                return TimeSpan.FromTicks(num3);
            }
            ticks = (long) Math.Ceiling(ts.TotalSeconds);
            if (ticks == 0L)
            {
                return TimeSpanFromTmInc(second);
            }
            num2 = 1L;
            if (second < tmInc.minute)
            {
                if (ticks < 60L)
                {
                    num2 = getNiceInc(new int[] { 1, 2, 5, 10, 15, 30 }, ticks, (long) second);
                    if (num2 != 0L)
                    {
                        return TimeSpan.FromSeconds((double) num2);
                    }
                }
                second = tmInc.minute;
            }
            if (second < tmInc.hour)
            {
                if (ticks < 0xe10L)
                {
                    num2 = getNiceInc(new int[] { 1, 2, 5, 10, 15, 30 }, ticks, (long) second);
                    if (num2 != 0L)
                    {
                        return TimeSpan.FromSeconds((double) num2);
                    }
                }
                second = tmInc.hour;
            }
            if (second < tmInc.day)
            {
                if (ticks < 0x15180L)
                {
                    num2 = getNiceInc(new int[] { 1, 3, 6, 12 }, ticks, (long) second);
                    if (num2 != 0L)
                    {
                        return TimeSpan.FromSeconds((double) num2);
                    }
                }
                second = tmInc.day;
            }
            if (second < tmInc.month)
            {
                if (ticks < 0x28de80L)
                {
                    num2 = getNiceInc(new int[] { 1, 2, 7, 14 }, ticks, (long) second);
                    if (num2 != 0L)
                    {
                        return TimeSpan.FromSeconds((double) num2);
                    }
                }
                second = tmInc.month;
            }
            if (second < tmInc.year)
            {
                if (ticks < 0x1e13380L)
                {
                    num2 = getNiceInc(new int[] { 1, 2, 3, 4, 6 }, ticks, (long) second);
                    if (num2 != 0L)
                    {
                        return TimeSpan.FromSeconds((double) num2);
                    }
                }
                second = tmInc.year;
            }
            num2 = 0xbbf81e00L;
            if (ticks < num2)
            {
                num2 = getNiceInc(new int[] { 1, 2, 5, 10, 20, 50 }, ticks, (long) second);
                if (num2 == 0L)
                {
                    num2 = 0xbbf81e00L;
                }
            }
            return TimeSpan.FromSeconds((double) num2);
        }

        public static double NiceTimeUnit(double timeinc, string manualformat)
        {
            return NiceTimeSpan(TimeSpan.FromDays(timeinc), manualformat).TotalDays;
        }

        public static double RoundTime(double timevalue, double unit, bool roundup)
        {
            long totalSeconds = (long) TimeSpan.FromDays(unit).TotalSeconds;
            if (totalSeconds > 0L)
            {
                timeHelper helper = new timeHelper(timevalue);
                if (totalSeconds < 60L)
                {
                    helper.second = tround(helper.second, totalSeconds, roundup);
                    return helper.getTimeAsDouble();
                }
                helper.second = 0;
                if (totalSeconds < 0xe10L)
                {
                    totalSeconds /= 60L;
                    helper.minute = tround(helper.minute, totalSeconds, roundup);
                    return helper.getTimeAsDouble();
                }
                helper.minute = 0;
                if (totalSeconds < 0x15180L)
                {
                    totalSeconds /= 0xe10L;
                    helper.hour = tround(helper.hour, totalSeconds, roundup);
                    return helper.getTimeAsDouble();
                }
                helper.hour = 0;
                if (totalSeconds < 0x28de80L)
                {
                    totalSeconds /= 0x15180L;
                    helper.day = tround(helper.day, totalSeconds, roundup);
                    return helper.getTimeAsDouble();
                }
                helper.day = 1;
                if (totalSeconds < 0x1e13380L)
                {
                    totalSeconds /= 0x28de80L;
                    if (helper.month != 1)
                    {
                        helper.month = tround(helper.month, totalSeconds, roundup);
                    }
                    return helper.getTimeAsDouble();
                }
                helper.month = 1;
                totalSeconds /= 0x1e13380L;
                helper.year = tround(helper.year, totalSeconds, roundup);
                return helper.getTimeAsDouble();
            }
            double num2 = timevalue;
            double num3 = num2 - totalSeconds;
            double num4 = (num3 / unit) * unit;
            if (roundup && (num4 != num3))
            {
                num4 += unit;
            }
            return (totalSeconds + num4);
        }

        static TimeSpan TimeSpanFromTmInc(tmInc ti)
        {
            TimeSpan span = TimeSpan.FromSeconds(1.0);
            if (ti.Equals(tmInc.maxtime))
            {
                return span;
            }
            if (ti.CompareTo(tmInc.tickf1) > 0)
            {
                return TimeSpan.FromSeconds((double) ti);
            }
            int num = (int) ti;
            long ticks = 1L;
            for (num += 7; num > 0; num--)
            {
                ticks *= 10L;
            }
            return new TimeSpan(ticks);
        }

        static int tround(int tval, long tunit, bool roundup)
        {
            int num = (int)((((long) tval) / tunit) * tunit);
            if (roundup && (num != tval))
            {
                num += (int) tunit;
            }
            return num;
        }

        class timeHelper
        {
            public int day;
            public int hour;
            public int minute;
            public int month;
            public int second;
            public int year;

            public timeHelper(DateTime dt)
            {
                init(dt);
            }

            public timeHelper(double dbltime)
            {
                init(dbltime.FromOADate());
            }

            public DateTime getTimeAsDateTime()
            {
                int num = 0;
                int num2 = 0;
                int num3 = 0;
                if (hour >= 0x18)
                {
                    hour -= 0x18;
                    day++;
                }
                if (day < 1)
                {
                    num2 = -1 - day;
                    day = 1;
                }
                else if (day > 0x1c)
                {
                    num2 = day - 0x1c;
                    day = 0x1c;
                }
                if (month < 1)
                {
                    num = -1 - day;
                    month = 1;
                }
                else if (month > 12)
                {
                    num = month - 12;
                    month = 12;
                }
                if (second > 0x3b)
                {
                    num3 = second - 0x3b;
                    second = 0x3b;
                }
                int num4 = 0;
                if (minute > 0x3b)
                {
                    num4 = minute - 0x3b;
                    minute = 0x3b;
                }
                DateTime time = new DateTime(year, month, day, hour, minute, second);
                return time.AddDays((double) num2).AddMonths(num).AddSeconds((double) num3).AddMinutes((double) num4);
            }

            public double getTimeAsDouble()
            {
                return getTimeAsDateTime().ToOADate();
            }

            void init(DateTime dt)
            {
                year = dt.Year;
                month = dt.Month;
                day = dt.Day;
                hour = dt.Hour;
                minute = dt.Minute;
                second = dt.Second;
            }
        }

        public enum tmInc
        {
            day = 0x15180,
            hour = 0xe10,
            maxtime = 0x7fffffff,
            minute = 60,
            month = 0x28de80,
            second = 1,
            tickf1 = -1,
            tickf2 = -2,
            tickf3 = -3,
            tickf4 = -4,
            tickf5 = -5,
            tickf6 = -6,
            tickf7 = -7,
            week = 0x93a80,
            year = 0x1e13380
        }
    }
}

