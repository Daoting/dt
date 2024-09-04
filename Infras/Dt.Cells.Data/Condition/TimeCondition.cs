#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a time condition.
    /// </summary>
    public sealed class TimeCondition : ConditionBase
    {
        DateCompareType compareType;

        /// <summary>
        /// Creates a new time condition.
        /// </summary>
        public TimeCondition() : this(DateCompareType.EqualsTo, DateTime.Now, null)
        {
        }

        /// <summary>
        /// Creates a new time condition with a specified date comparison type and expected date-time value.
        /// </summary>
        /// <param name="compareType">The comparison type.</param>
        /// <param name="expected">The expected date-time value.</param>
        /// <param name="formula">The expected formula value.</param>
        internal TimeCondition(DateCompareType compareType, object expected, string formula) : base(expected, formula)
        {
            this.compareType = compareType;
        }

        /// <summary>
        /// Checks the condition.
        /// </summary>
        /// <param name="expectedValue">Expected value for the condition</param>
        /// <param name="actualValue">Actual value for the condition</param>
        /// <returns>
        /// <c>true</c> if the value satisfy the condition; otherwise, <c>false</c>.
        /// </returns>
        bool CheckCondition(object expectedValue, object actualValue)
        {
            TimeSpan? nullable = null;
            if (actualValue is DateTime)
            {
                DateTime time = (DateTime) actualValue;
                DateTime time2 = (DateTime) actualValue;
                DateTime time3 = (DateTime) actualValue;
                DateTime time4 = (DateTime) actualValue;
                nullable = new TimeSpan(0, time.Hour, time2.Minute, time3.Second, time4.Millisecond);
            }
            else if (actualValue is TimeSpan)
            {
                nullable = new TimeSpan?((TimeSpan) actualValue);
            }
            if ((actualValue == null) || actualValue.Equals(string.Empty))
            {
                return this.IgnoreBlank;
            }
            if (actualValue is string)
            {
                try
                {
                    nullable = new TimeSpan?(ConditionValueConverter.ToTimeSpan(actualValue));
                }
                catch (InvalidCastException)
                {
                    return false;
                }
            }
            if (nullable.HasValue)
            {
                if (!nullable.HasValue && (expectedValue != null))
                {
                    return false;
                }
                try
                {
                    if (nullable.HasValue)
                    {
                        TimeSpan span;
                        if (expectedValue is TimeSpan)
                        {
                            span = (TimeSpan) expectedValue;
                        }
                        else
                        {
                            DateTime? nullable2 = ConditionValueConverter.TryDateTime(expectedValue);
                            if (!nullable2.HasValue)
                            {
                                return false;
                            }
                            span = new TimeSpan(0, nullable2.Value.Hour, nullable2.Value.Minute, nullable2.Value.Second, nullable2.Value.Millisecond);
                        }
                        switch (this.CompareType)
                        {
                            case DateCompareType.EqualsTo:
                                return this.IsEquals(span, nullable.Value);

                            case DateCompareType.NotEqualsTo:
                                return !this.IsEquals(span, nullable.Value);

                            case DateCompareType.Before:
                                return this.IsBefore(span, nullable.Value);

                            case DateCompareType.BeforeEqualsTo:
                                return (this.IsBefore(span, nullable.Value) || this.IsEquals(span, nullable.Value));

                            case DateCompareType.After:
                                return this.IsAfter(span, nullable.Value);

                            case DateCompareType.AfterEqualsTo:
                                return (this.IsAfter(span, nullable.Value) || this.IsEquals(span, nullable.Value));
                        }
                    }
                }
                catch
                {
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
            TimeCondition condition = base.Clone() as TimeCondition;
            condition.compareType = this.compareType;
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
            object expectedValue = base.GetExpected(evaluator, baseRow, baseColumn);
            object actualValue = actualObj.GetValue();
            return this.CheckCondition(expectedValue, actualValue);
        }

        /// <summary>
        /// Creates a TimeCondition object from the DateTime object.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected DateTime object.</param>
        /// <returns>The TimeCondition object.</returns>
        public static TimeCondition FromDateTime(DateCompareType compareType, DateTime expected)
        {
            return new TimeCondition(compareType, expected, null);
        }

        /// <summary>
        /// Creates a TimeCondition object from the formula.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="formula">The formula.</param>
        /// <returns>The TimeCondition object.</returns>
        public static TimeCondition FromFormula(DateCompareType compareType, string formula)
        {
            return new TimeCondition(compareType, DateTime.Now, formula);
        }

        /// <summary>
        /// Determines whether the specified time is after another time.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified time value is after the other time; otherwise, <c>false</c>.
        /// </returns>
        bool IsAfter(TimeSpan expectedValue, TimeSpan value)
        {
            return (value > expectedValue);
        }

        /// <summary>
        /// Determines whether the specified time is before another time.
        /// </summary>
        /// <param name="expectedValue">The expected value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified time value is before the other time; otherwise, <c>false</c>.
        /// </returns>
        bool IsBefore(TimeSpan expectedValue, TimeSpan value)
        {
            return (value < expectedValue);
        }

        /// <summary>
        /// Determines whether the specified time equals another time.
        /// </summary>
        /// <param name="expectedValue">The expected time span</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified time equals the other; otherwise, <c>false</c>.
        /// </returns>
        bool IsEquals(TimeSpan expectedValue, TimeSpan value)
        {
            return ((((expectedValue.Hours == value.Hours) && (expectedValue.Minutes == value.Minutes)) && (expectedValue.Seconds == value.Seconds)) && (expectedValue.Milliseconds == value.Milliseconds));
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
                this.compareType = (DateCompareType) Serializer.DeserializeObj(typeof(DateCompareType), reader);
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.compareType != DateCompareType.EqualsTo)
            {
                Serializer.SerializeObj(this.compareType, "Type", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = DateCompareType.EqualsTo;
        }

        /// <summary>
        /// Gets the time comparison type.
        /// </summary>
        /// <value>The time comparison type. The default value is <see cref="T:Dt.Cells.Data.DateCompareType">Equals</see>.</value>
        [DefaultValue(0)]
        public DateCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }
    }
}

