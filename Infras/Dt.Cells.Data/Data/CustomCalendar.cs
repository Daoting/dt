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
using Windows.Globalization;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class CustomCalendar : System.Globalization.Calendar
    {
        protected readonly Windows.Globalization.Calendar Calendar = new Windows.Globalization.Calendar();

        protected CustomCalendar()
        {
        }

        public override DateTime AddMonths(DateTime time, int months)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            this.Calendar.AddMonths(months);
            return this.Calendar.GetDateTime().DateTime;
        }

        public override DateTime AddYears(DateTime time, int years)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            this.Calendar.AddYears(years);
            return this.Calendar.GetDateTime().DateTime;
        }

        public override int GetDayOfMonth(DateTime time)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            return this.Calendar.NumberOfDaysInThisMonth;
        }

        public override System.DayOfWeek GetDayOfWeek(DateTime time)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            return (System.DayOfWeek)this.Calendar.DayOfWeek;
        }

        public override int GetDayOfYear(DateTime time)
        {
            return time.DayOfYear;
        }

        public override int GetDaysInMonth(int year, int month)
        {
            this.Calendar.Year = year;
            this.Calendar.Month = month;
            return this.Calendar.NumberOfDaysInThisMonth;
        }

        public override int GetDaysInMonth(int year, int month, int era)
        {
            this.Calendar.Era = era;
            this.Calendar.Year = year;
            this.Calendar.Month = month;
            this.Calendar.Day = this.Calendar.LastDayInThisMonth;
            return this.Calendar.NumberOfDaysInThisMonth;
        }

        public override int GetDaysInYear(int year, int era)
        {
            this.Calendar.Era = era;
            this.Calendar.Year = year;
            this.Calendar.Month = this.Calendar.FirstMonthInThisYear;
            this.Calendar.Day = this.Calendar.FirstDayInThisMonth;
            DateTimeOffset dateTime = this.Calendar.GetDateTime();
            this.Calendar.Month = this.Calendar.LastMonthInThisYear;
            this.Calendar.Day = this.Calendar.LastDayInThisMonth;
            TimeSpan span = (TimeSpan) (this.Calendar.GetDateTime() - dateTime);
            return span.Days;
        }

        public override int GetEra(DateTime time)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            return this.Calendar.NumberOfYearsInThisEra;
        }

        public override int GetMonth(DateTime time)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            return this.Calendar.NumberOfMonthsInThisYear;
        }

        public override int GetMonthsInYear(int year, int era)
        {
            this.Calendar.Era = era;
            this.Calendar.Year = year;
            return (this.Calendar.LastMonthInThisYear - this.Calendar.FirstMonthInThisYear);
        }

        public override int GetYear(DateTime time)
        {
            this.Calendar.SetDateTime((DateTimeOffset) time);
            return this.Calendar.Year;
        }

        public override bool IsLeapDay(int year, int month, int day, int era)
        {
            if ((month < 1) || (month > 12))
            {
                throw new ArgumentOutOfRangeException("month", "ArgumentOutOfRange_Range");
            }
            if ((era != 0) && (era != 1))
            {
                throw new ArgumentOutOfRangeException("era", "ArgumentOutOfRange_InvalidEraValue");
            }
            if ((year < 1) || (year > 0x270f))
            {
                throw new ArgumentOutOfRangeException("year", "ArgumentOutOfRange_Range");
            }
            if ((day < 1) || (day > this.GetDaysInMonth(year, month)))
            {
                throw new ArgumentOutOfRangeException("day", "ArgumentOutOfRange_Range");
            }
            if (!this.IsLeapYear(year))
            {
                return false;
            }
            return ((month == 2) && (day == 0x1d));
        }

        public override bool IsLeapMonth(int year, int month, int era)
        {
            if ((era != 0) && (era != 1))
            {
                throw new ArgumentOutOfRangeException("era", "ArgumentOutOfRange_InvalidEraValue");
            }
            if ((year < 1) || (year > 0x270f))
            {
                throw new ArgumentOutOfRangeException("year", "ArgumentOutOfRange_Range");
            }
            if ((month < 1) || (month > 12))
            {
                throw new ArgumentOutOfRangeException("month", "ArgumentOutOfRange_Range");
            }
            return false;
        }

        public override bool IsLeapYear(int year, int era)
        {
            if ((era != 0) && (era != 1))
            {
                throw new ArgumentOutOfRangeException("era", "ArgumentOutOfRange_InvalidEraValue");
            }
            if ((year < 1) || (year > 0x270f))
            {
                throw new ArgumentOutOfRangeException("year", "ArgumentOutOfRange_Range");
            }
            if ((year % 4) != 0)
            {
                return false;
            }
            if ((year % 100) == 0)
            {
                return ((year % 400) == 0);
            }
            return true;
        }

        public override DateTime ToDateTime(int year, int month, int day, int hour, int minute, int second, int millisecond, int era)
        {
            this.Calendar.Era = era;
            this.Calendar.Year = year;
            this.Calendar.Month = month;
            this.Calendar.Day = day;
            this.Calendar.Hour = hour;
            this.Calendar.Minute = minute;
            this.Calendar.Second = second;
            this.Calendar.Nanosecond = millisecond * 0x3e8;
            return this.Calendar.GetDateTime().Date;
        }

        public override int[] Eras
        {
            get { return  new int[] { this.Calendar.Era }; }
        }
    }
}

