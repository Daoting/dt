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
using System.Runtime.InteropServices;
using System.Xml;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a number condition.
    /// </summary>
    internal sealed class CellValueCondition : ConditionBase
    {
        GeneralCompareType compareType;
        bool treatNullValueAsZero;

        /// <summary>
        /// Creates a new number condition.
        /// </summary>
        public CellValueCondition() : this(GeneralCompareType.EqualsTo, null, null)
        {
        }

        /// <summary>
        /// Creates a new number condition with the specified type for the specified cell range.
        /// </summary>
        /// <param name="compareType">Type of the compare</param>
        /// <param name="expected">The expected value</param>
        /// <param name="formula">The expected formula</param>
        internal CellValueCondition(GeneralCompareType compareType, object expected, string formula) : base(expected, formula)
        {
            this.compareType = compareType;
        }

        /// <summary>
        /// Checks the condition.
        /// </summary>
        /// <param name="expectedValue">Expected value for the condition.</param>
        /// <param name="actualValue">Actual value for the condition.</param>
        /// <returns>
        /// <c>true</c> if the value satisfies the condition; otherwise, <c>false</c>.
        /// </returns>
        bool CheckCondition(object expectedValue, object actualValue)
        {
            double dValue = 0.0;
            bool flag = false;
            if (expectedValue is bool)
            {
                bool @this = (bool) ((bool) expectedValue);
                switch (this.compareType)
                {
                    case GeneralCompareType.EqualsTo:
                        return object.Equals(actualValue, expectedValue);

                    case GeneralCompareType.NotEqualsTo:
                        return !object.Equals(actualValue, expectedValue);

                    case GeneralCompareType.GreaterThan:
                        if (@this.CompareTo(actualValue) >= 0)
                        {
                            return false;
                        }
                        return true;

                    case GeneralCompareType.GreaterThanOrEqualsTo:
                        if (@this.CompareTo(actualValue) > 0)
                        {
                            return false;
                        }
                        return true;

                    case GeneralCompareType.LessThan:
                        if (@this.CompareTo(actualValue) <= 0)
                        {
                            return false;
                        }
                        return true;

                    case GeneralCompareType.LessThanOrEqualsTo:
                        if (@this.CompareTo(actualValue) < 0)
                        {
                            return false;
                        }
                        return true;
                }
            }
            if (actualValue is TimeSpan)
            {
                TimeSpan span = (TimeSpan) actualValue;
                dValue = span.TotalSeconds;
                flag = true;
            }
            else
            {
                flag = TryConvertToNumber(actualValue, out dValue);
            }
            if ((actualValue == null) && (expectedValue == null))
            {
                switch (this.compareType)
                {
                    case GeneralCompareType.EqualsTo:
                    case GeneralCompareType.GreaterThanOrEqualsTo:
                    case GeneralCompareType.LessThanOrEqualsTo:
                        return true;

                    case GeneralCompareType.NotEqualsTo:
                    case GeneralCompareType.GreaterThan:
                    case GeneralCompareType.LessThan:
                        return false;
                }
                return false;
            }
            if (this.TreatNullValueAsZero && (actualValue == null))
            {
                flag = true;
                dValue = 0.0;
            }
            if (flag)
            {
                double totalSeconds = 0.0;
                try
                {
                    if (expectedValue is DateTime)
                    {
                        totalSeconds = ((DateTime) expectedValue).ToOADate();
                    }
                    else if (expectedValue is TimeSpan)
                    {
                        TimeSpan span2 = (TimeSpan) expectedValue;
                        totalSeconds = span2.TotalSeconds;
                    }
                    else
                    {
                        totalSeconds = ConditionValueConverter.ToDouble(expectedValue);
                    }
                }
                catch
                {
                    switch (this.CompareType)
                    {
                        case GeneralCompareType.EqualsTo:
                            return false;

                        case GeneralCompareType.NotEqualsTo:
                            return true;
                    }
                    return false;
                }
                switch (this.CompareType)
                {
                    case GeneralCompareType.EqualsTo:
                        return (dValue == totalSeconds);

                    case GeneralCompareType.NotEqualsTo:
                        return !(dValue == totalSeconds);

                    case GeneralCompareType.GreaterThan:
                        return (dValue > totalSeconds);

                    case GeneralCompareType.GreaterThanOrEqualsTo:
                        return (dValue >= totalSeconds);

                    case GeneralCompareType.LessThan:
                        return (dValue < totalSeconds);

                    case GeneralCompareType.LessThanOrEqualsTo:
                        return (dValue <= totalSeconds);
                }
            }
            else if (actualValue is string)
            {
                string str = null;
                if (expectedValue is string)
                {
                    str = (string) (expectedValue as string);
                }
                else
                {
                    switch (this.CompareType)
                    {
                        case GeneralCompareType.EqualsTo:
                            return false;

                        case GeneralCompareType.NotEqualsTo:
                            return true;
                    }
                    return false;
                }
                string strB = (string) ((string) actualValue);
                switch (this.CompareType)
                {
                    case GeneralCompareType.EqualsTo:
                        return (strB == str);

                    case GeneralCompareType.NotEqualsTo:
                        return (strB != str);

                    case GeneralCompareType.GreaterThan:
                        return (str.CompareTo(strB) < 0);

                    case GeneralCompareType.GreaterThanOrEqualsTo:
                        return (str.CompareTo(strB) <= 0);

                    case GeneralCompareType.LessThan:
                        return (str.CompareTo(strB) > 0);

                    case GeneralCompareType.LessThanOrEqualsTo:
                        return (str.CompareTo(strB) >= 0);
                }
            }
            return false;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>Returns the cloned object.</returns>
        public override object Clone()
        {
            CellValueCondition condition = base.Clone() as CellValueCondition;
            condition.compareType = this.compareType;
            condition.treatNullValueAsZero = this.treatNullValueAsZero;
            return condition;
        }

        public override bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actualObj)
        {
            object expectedValue = base.GetExpected(evaluator, baseRow, baseColumn);
            object actualValue = actualObj.GetValue();
            return this.CheckCondition(expectedValue, actualValue);
        }

        /// <summary>
        /// Determines whether the specified value meets the condition.
        /// </summary>
        /// <param name="value">The value.</param>
        public bool IsSatisfyingCondition(object value)
        {
            object expectedValue = base.GetExpected(null, 0, 0);
            return this.CheckCondition(expectedValue, value);
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "Type")
                {
                    if (name != "TreatNullValueAsZero")
                    {
                        return;
                    }
                }
                else
                {
                    this.compareType = (GeneralCompareType) Serializer.DeserializeObj(typeof(GeneralCompareType), reader);
                    return;
                }
                this.treatNullValueAsZero = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.compareType != GeneralCompareType.EqualsTo)
            {
                Serializer.SerializeObj(this.compareType, "Type", writer);
            }
            Serializer.SerializeObj((bool) this.treatNullValueAsZero, "TreatNullValueAsZero", writer);
        }

        static bool TryConvertToNumber(object value, out double dValue)
        {
            dValue = 0.0;
            if (ConditionValueConverter.IsNumber(value))
            {
                try
                {
                    dValue = ConditionValueConverter.ToDouble(value);
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return ((value is string) && double.TryParse((string)(value as string), (NumberStyles)NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out dValue));
        }

        /// <summary>
        /// Gets the type of the compare.
        /// </summary>
        /// <value>The type of the compare.</value>
        public GeneralCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to treat the null value in a cell as zero.
        /// </summary>
        public bool TreatNullValueAsZero
        {
            get { return  this.treatNullValueAsZero; }
            set { this.treatNullValueAsZero = value; }
        }
    }
}

