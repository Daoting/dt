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
    /// Represents a date condition.
    /// </summary>
    public sealed class DateCondition : ConditionBase
    {
        DateCompareType compareType;

        /// <summary>
        /// Creates a new date condition.
        /// </summary>
        public DateCondition() : this(DateCompareType.EqualsTo, DateTime.Now, null)
        {
        }

        /// <summary>
        /// Creates a new date condition with the specified comparison type and expected date condition.
        /// </summary>
        /// <param name="compareType">The date comparison type.</param>
        /// <param name="expected">The expected date condition.</param>
        /// <param name="formula"></param>
        internal DateCondition(DateCompareType compareType, object expected, string formula) : base(expected, formula)
        {
            this.compareType = compareType;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            DateCondition condition = base.Clone() as DateCondition;
            condition.compareType = this.compareType;
            return condition;
        }

        /// <summary>
        /// Creates the day beginning.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Returns the date time.</returns>
        DateTime CreateDayBeginning(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0, 0, 0);
        }

        /// <summary>
        /// Creates the day ending.
        /// </summary>
        /// <param name="datetime">The datetime</param>
        /// <returns>Returns the date time.</returns>
        DateTime CreateDayEnding(DateTime datetime)
        {
            return new DateTime(datetime.Year, datetime.Month, datetime.Day, 0x17, 0x3b, 0x3b);
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
            if ((actualObj.GetValue() == null) && this.IgnoreBlank)
            {
                return true;
            }
            object obj2 = actualObj.GetValue(NumberFormatType.DateTime);
            if (obj2 is DateTime)
            {
                DateTime? nullable = base.GetExpectedDateTime(evaluator, baseRow, baseColumn);
                if (!nullable.HasValue && this.IgnoreBlank)
                {
                    return true;
                }
                if (!nullable.HasValue)
                {
                    return false;
                }
                switch (this.CompareType)
                {
                    case DateCompareType.EqualsTo:
                        return this.IsEquals(nullable.Value, (DateTime) obj2);

                    case DateCompareType.NotEqualsTo:
                        return !this.IsEquals(nullable.Value, (DateTime) obj2);

                    case DateCompareType.Before:
                        return this.IsBefore(nullable.Value, (DateTime) obj2);

                    case DateCompareType.BeforeEqualsTo:
                        return (this.IsBefore(nullable.Value, (DateTime) obj2) || this.IsEquals(nullable.Value, (DateTime) obj2));

                    case DateCompareType.After:
                        return this.IsAfter(nullable.Value, (DateTime) obj2);

                    case DateCompareType.AfterEqualsTo:
                        return (this.IsAfter(nullable.Value, (DateTime) obj2) || this.IsEquals(nullable.Value, (DateTime) obj2));
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a DateCondition object from the dateTime object.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected dateTime object.</param>
        /// <returns>The DateCondition object.</returns>
        public static DateCondition FromDateTime(DateCompareType compareType, DateTime expected)
        {
            return new DateCondition(compareType, expected, null);
        }

        /// <summary>
        /// Creates a DateCondition object from the formula.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="formula">The formula.</param>
        /// <returns>The DateCondition object.</returns>
        public static DateCondition FromFormula(DateCompareType compareType, string formula)
        {
            return new DateCondition(compareType, DateTime.Now, formula);
        }

        /// <summary>
        /// Determines whether the specified date is after another date.
        /// </summary>
        /// <param name="expectedValue">The expected date value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified date is after another date; otherwise, <c>false</c>.
        /// </returns>
        bool IsAfter(DateTime expectedValue, DateTime value)
        {
            DateTime time = this.CreateDayEnding(expectedValue);
            return (value > time);
        }

        /// <summary>
        /// Determines whether the specified date is before another date.
        /// </summary>
        /// <param name="expectedValue">The expected date value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified date is before another date; otherwise, <c>false</c>.
        /// </returns>
        bool IsBefore(DateTime expectedValue, DateTime value)
        {
            DateTime time = this.CreateDayBeginning(expectedValue);
            return (value < time);
        }

        /// <summary>
        /// Determines whether the specified date equals another date.
        /// </summary>
        /// <param name="expectedValue">The expected date value</param>
        /// <param name="value">The value</param>
        /// <returns>
        /// <c>true</c> if the specified date equals the other date; otherwise, <c>false</c>.
        /// </returns>
        bool IsEquals(DateTime expectedValue, DateTime value)
        {
            return (((expectedValue.Year == value.Year) && (expectedValue.Month == value.Month)) && (expectedValue.Day == value.Day));
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
        /// Gets or sets the type of the compare.
        /// </summary>
        /// <value>The type of the compare.</value>
        [DefaultValue(0)]
        public DateCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get { return  typeof(DateTime); }
        }
    }
}

