#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a date condition.
    /// </summary>
    public sealed class DateExCondition : ConditionBase
    {
        int expectTypeId;

        /// <summary>
        /// Creates a new date condition.
        /// </summary>
        public DateExCondition() : this(DateOccurringType.Today)
        {
        }

        /// <summary>
        /// Creates a new date condition with the specified date occurring type.
        /// </summary>
        /// <param name="occurring">The date occurring type for this condition.</param>
        internal DateExCondition(DateOccurringType occurring) : base(occurring, null)
        {
        }

        DateExCondition(object expected) : base(expected, null)
        {
        }

        bool CheckCondition(int expected, IActualValue actualValue)
        {
            if (this.IgnoreBlank && (actualValue.GetValue() == null))
            {
                return true;
            }
            object obj2 = actualValue.GetValue(NumberFormatType.DateTime);
            if (obj2 is DateTime)
            {
                if (this.expectTypeId != 0)
                {
                    if (this.expectTypeId == 1)
                    {
                        int? nullable3 = base.GetExpectedInt(null, 0, 0);
                        if (nullable3.HasValue)
                        {
                            return this.IsEqualsYear(nullable3.Value, (DateTime) obj2);
                        }
                    }
                    else if (this.expectTypeId == 2)
                    {
                        int? nullable4 = base.GetExpectedInt(null, 0, 0);
                        if (nullable4.HasValue)
                        {
                            return this.IsEqualsQuarter(nullable4.Value, (DateTime) obj2);
                        }
                    }
                    else if (this.expectTypeId == 3)
                    {
                        int? nullable5 = base.GetExpectedInt(null, 0, 0);
                        if (nullable5.HasValue)
                        {
                            return this.IsEqualsMonth(nullable5.Value, (DateTime) obj2);
                        }
                    }
                    else if (this.expectTypeId == 4)
                    {
                        int? nullable6 = base.GetExpectedInt(null, 0, 0);
                        if (nullable6.HasValue)
                        {
                            return this.IsEqualsWeek(nullable6.Value, (DateTime) obj2);
                        }
                    }
                    else if (this.expectTypeId == 5)
                    {
                        int? nullable7 = base.GetExpectedInt(null, 0, 0);
                        if (nullable7.HasValue)
                        {
                            return this.IsEqualsDay(nullable7.Value, (DateTime) obj2);
                        }
                    }
                }
                else
                {
                    DateTime? nullable = null;
                    DateTime? nullable2 = null;
                    switch (((DateOccurringType) expected))
                    {
                        case DateOccurringType.Today:
                        {
                            DateTime time4 = DateTime.Now;
                            nullable = new DateTime(time4.Year, time4.Month, time4.Day, 0, 0, 0);
                            nullable2 = new DateTime(time4.Year, time4.Month, time4.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.Yesterday:
                        {
                            DateTime time3 = DateTime.Now;
                            TimeSpan span2 = new TimeSpan(1, 0, 0, 0);
                            time3 -= span2;
                            nullable = new DateTime(time3.Year, time3.Month, time3.Day, 0, 0, 0);
                            nullable2 = new DateTime(time3.Year, time3.Month, time3.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.Tomorrow:
                        {
                            DateTime time5 = DateTime.Now;
                            TimeSpan span3 = new TimeSpan(1, 0, 0, 0);
                            time5 += span3;
                            nullable = new DateTime(time5.Year, time5.Month, time5.Day, 0, 0, 0);
                            nullable2 = new DateTime(time5.Year, time5.Month, time5.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.Last7Days:
                        {
                            DateTime time = DateTime.Now;
                            TimeSpan span = new TimeSpan(6, 0, 0, 0);
                            DateTime time2 = time - span;
                            nullable = new DateTime(time2.Year, time2.Month, time2.Day, 0, 0, 0);
                            nullable2 = new DateTime(time.Year, time.Month, time.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.ThisMonth:
                        {
                            DateTime time21 = DateTime.Now;
                            TimeSpan span8 = new TimeSpan(time21.Day - 1, 0, 0, 0);
                            DateTime time22 = time21 - span8;
                            DateTime time24 = time22.AddMonths(1).AddDays(-1.0);
                            nullable = new DateTime(time22.Year, time22.Month, time22.Day, 0, 0, 0);
                            nullable2 = new DateTime(time24.Year, time24.Month, time24.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.LastMonth:
                        {
                            DateTime time17 = DateTime.Now;
                            TimeSpan span7 = new TimeSpan(time17.Day - 1, 0, 0, 0);
                            DateTime time18 = time17 - span7;
                            DateTime time19 = time18.AddMonths(-1);
                            DateTime time20 = time18.AddDays(-1.0);
                            nullable = new DateTime(time19.Year, time19.Month, time19.Day, 0, 0, 0);
                            nullable2 = new DateTime(time20.Year, time20.Month, time20.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.NextMonth:
                        {
                            DateTime time25 = DateTime.Now;
                            TimeSpan span9 = new TimeSpan(time25.Day - 1, 0, 0, 0);
                            DateTime time27 = (time25 - span9).AddMonths(1);
                            DateTime time29 = time27.AddMonths(1).AddDays(-1.0);
                            nullable = new DateTime(time27.Year, time27.Month, time27.Day, 0, 0, 0);
                            nullable2 = new DateTime(time29.Year, time29.Month, time29.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.ThisWeek:
                        {
                            DateTime time10 = DateTime.Now;
                            TimeSpan span5 = new TimeSpan((int) time10.DayOfWeek, 0, 0, 0);
                            DateTime time11 = time10 - span5;
                            DateTime time12 = time11 + new TimeSpan(6, 0, 0, 0);
                            nullable = new DateTime(time11.Year, time11.Month, time11.Day, 0, 0, 0);
                            nullable2 = new DateTime(time12.Year, time12.Month, time12.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.LastWeek:
                        {
                            DateTime time6 = DateTime.Now;
                            TimeSpan span4 = new TimeSpan((int) time6.DayOfWeek, 0, 0, 0);
                            DateTime time7 = time6 - span4;
                            DateTime time8 = time7 - new TimeSpan(7, 0, 0, 0);
                            DateTime time9 = time8 + new TimeSpan(6, 0, 0, 0);
                            nullable = new DateTime(time8.Year, time8.Month, time8.Day, 0, 0, 0);
                            nullable2 = new DateTime(time9.Year, time9.Month, time9.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                        case DateOccurringType.NextWeek:
                        {
                            DateTime time13 = DateTime.Now;
                            TimeSpan span6 = new TimeSpan((int) time13.DayOfWeek, 0, 0, 0);
                            DateTime time14 = time13 - span6;
                            DateTime time15 = time14 + new TimeSpan(7, 0, 0, 0);
                            DateTime time16 = time15 + new TimeSpan(6, 0, 0, 0);
                            nullable = new DateTime(time15.Year, time15.Month, time15.Day, 0, 0, 0);
                            nullable2 = new DateTime(time16.Year, time16.Month, time16.Day, 0x17, 0x3b, 0x3b);
                            break;
                        }
                    }
                    if (nullable.HasValue && nullable2.HasValue)
                    {
                        DateCondition condition = new DateCondition(DateCompareType.AfterEqualsTo, nullable.Value, null);
                        DateCondition condition2 = new DateCondition(DateCompareType.BeforeEqualsTo, nullable2.Value, null);
                        RelationCondition condition3 = new RelationCondition(RelationCompareType.And, condition, condition2);
                        return condition3.Evaluate(null, 0, 0, actualValue);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            DateExCondition condition = base.Clone() as DateExCondition;
            condition.expectTypeId = this.expectTypeId;
            return condition;
        }

        /// <summary>
        /// Evaluates using the specified evaluator.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actualObj">The actual value object.</param>
        /// <returns><c>true</c> if the result is successful; otherwise, <c>false</c>.</returns>
        public override bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actualObj)
        {
            int? nullable = base.GetExpectedInt(null, 0, 0);
            return (nullable.HasValue && this.CheckCondition(nullable.Value, actualObj));
        }

        /// <summary>
        /// Creates a DateExCondition object from the day.
        /// </summary>
        /// <param name="day">The day.</param>
        /// <returns>The DateExCondition object.</returns>
        public static DateExCondition FromDay(int day)
        {
            return new DateExCondition((int) day) { expectTypeId = 5 };
        }

        /// <summary>
        /// Creates a DateExCondition object from the month.
        /// </summary>
        /// <param name="month">The month.</param>
        /// <returns>The DateExCondition object.</returns>
        public static DateExCondition FromMonth(MonthOfYearType month)
        {
            return new DateExCondition(month) { expectTypeId = 3 };
        }

        /// <summary>
        /// Creates a DateExCondition object from the quarter.
        /// </summary>
        /// <param name="quarter">The quarter.</param>
        /// <returns>The DateExCondition object.</returns>
        public static DateExCondition FromQuarter(QuarterType quarter)
        {
            return new DateExCondition(quarter) { expectTypeId = 2 };
        }

        /// <summary>
        /// Creates a DateExCondition object from the week.
        /// </summary>
        /// <param name="week">The week.</param>
        /// <returns>The DateExCondition object.</returns>
        public static DateExCondition FromWeek(DayOfWeek week)
        {
            return new DateExCondition(week) { expectTypeId = 4 };
        }

        /// <summary>
        /// Creates a DateExCondition object from the year.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <returns>The DateExCondition object.</returns>
        public static DateExCondition FromYear(int year)
        {
            return new DateExCondition((int) year) { expectTypeId = 1 };
        }

        bool IsEqualsDay(int expected, DateTime actualDateTime)
        {
            return (expected == actualDateTime.Day);
        }

        bool IsEqualsMonth(int expected, DateTime actualDateTime)
        {
            return (expected == actualDateTime.Month);
        }

        bool IsEqualsQuarter(int expected, DateTime actualDateTime)
        {
            switch (((QuarterType) expected))
            {
                case QuarterType.Quarter1:
                    if (actualDateTime.Month < 1)
                    {
                        return false;
                    }
                    return (actualDateTime.Month <= 3);

                case QuarterType.Quarter2:
                    if (actualDateTime.Month < 4)
                    {
                        return false;
                    }
                    return (actualDateTime.Month <= 6);

                case QuarterType.Quarter3:
                    if (actualDateTime.Month < 7)
                    {
                        return false;
                    }
                    return (actualDateTime.Month <= 9);

                case QuarterType.Quarter4:
                    if (actualDateTime.Month < 10)
                    {
                        return false;
                    }
                    return (actualDateTime.Month <= 12);
            }
            return false;
        }

        bool IsEqualsWeek(int expected, DateTime actualDateTime)
        {
            return (expected == (int) actualDateTime.DayOfWeek);
        }

        bool IsEqualsYear(int expected, DateTime actualDateTime)
        {
            return (expected == actualDateTime.Year);
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Type"))
            {
                this.expectTypeId = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            Serializer.SerializeObj((int) this.expectTypeId, "Type", writer);
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.expectTypeId = 0;
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get
            {
                switch (this.expectTypeId)
                {
                    case 0:
                        return typeof(DateOccurringType);

                    case 1:
                        return typeof(int);

                    case 2:
                        return typeof(QuarterType);

                    case 3:
                        return typeof(MonthOfYearType);

                    case 4:
                        return typeof(DayOfWeek);

                    case 5:
                        return typeof(int);
                }
                return typeof(int);
            }
        }
    }
}

