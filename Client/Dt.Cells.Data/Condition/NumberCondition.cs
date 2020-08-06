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
    /// Represents a number condition.
    /// </summary>
    public sealed class NumberCondition : ConditionBase
    {
        GeneralCompareType compareType;
        bool integerValue;

        /// <summary>
        /// Creates a new number condition.
        /// </summary>
        public NumberCondition() : this(GeneralCompareType.EqualsTo, (int) 0, null)
        {
        }

        /// <summary>
        /// Creates a new number condition of the specified type with the specified expected value.
        /// </summary>
        /// <param name="compareType">The number comparison type.</param>
        /// <param name="expected">The expected number.</param>
        /// <param name="formula">The expected formula.</param>
        internal NumberCondition(GeneralCompareType compareType, object expected, string formula) : base(expected, formula)
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
            if (this.IgnoreBlank && ((actualValue == null) || actualValue.Equals(string.Empty)))
            {
                return true;
            }
            if (ConditionValueConverter.IsNumber(actualValue))
            {
                double? nullable = null;
                object obj2 = expectedValue;
                if (obj2 is string)
                {
                    return ((this.CompareType == GeneralCompareType.NotEqualsTo) && ConditionValueConverter.IsNumber(actualValue));
                }
                if (ConditionValueConverter.IsNumber(obj2))
                {
                    nullable = new double?(ConditionValueConverter.ToDouble(obj2));
                }
                if (!nullable.HasValue)
                {
                    if (this.IgnoreBlank)
                    {
                        return true;
                    }
                    nullable = 0.0;
                }
                double d = 0.0;
                try
                {
                    d = ConditionValueConverter.ToDouble(actualValue);
                }
                catch (FormatException)
                {
                    return false;
                }
                if (!this.IntegerValue || ((d - Math.Floor(d)) == 0.0))
                {
                    switch (this.CompareType)
                    {
                        case GeneralCompareType.EqualsTo:
                            return (d == nullable.Value);

                        case GeneralCompareType.NotEqualsTo:
                            return (d != nullable.Value);

                        case GeneralCompareType.GreaterThan:
                            return (d > nullable.Value);

                        case GeneralCompareType.GreaterThanOrEqualsTo:
                            return (d >= nullable.Value);

                        case GeneralCompareType.LessThan:
                            return (d < nullable.Value);

                        case GeneralCompareType.LessThanOrEqualsTo:
                            return (d <= nullable.Value);
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
            NumberCondition condition = base.Clone() as NumberCondition;
            condition.compareType = this.compareType;
            condition.integerValue = this.integerValue;
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
            object expectedValue = null;
            object actualValue = actualObj.GetValue();
            if (this.integerValue)
            {
                int? nullable = base.GetExpectedInt(evaluator, baseRow, baseColumn);
                expectedValue = nullable.HasValue ? ((object) ((int) nullable.Value)) : null;
            }
            else
            {
                expectedValue = base.GetExpected(evaluator, baseRow, baseColumn);
            }
            return this.CheckCondition(expectedValue, actualValue);
        }

        /// <summary>
        /// Creates a number condition from a double value.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>The NumberCondition object.</returns>
        public static NumberCondition FromDouble(GeneralCompareType compareType, double expected)
        {
            return new NumberCondition(compareType, (double) expected, null);
        }

        /// <summary>
        /// Creates a number condition from a formula.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="formula">The formula.</param>
        /// <returns>The NumberCondition object.</returns>
        public static NumberCondition FromFormula(GeneralCompareType compareType, string formula)
        {
            return new NumberCondition(compareType, (int) 0, formula);
        }

        /// <summary>
        /// Creates a number condition from an integer value.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>The NumberCondition object.</returns>
        public static NumberCondition FromInt(GeneralCompareType compareType, int expected)
        {
            return new NumberCondition(compareType, (int) expected, null);
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
                    if (name != "IntegerValue")
                    {
                        return;
                    }
                }
                else
                {
                    this.compareType = (GeneralCompareType) Serializer.DeserializeObj(typeof(GeneralCompareType), reader);
                    return;
                }
                this.integerValue = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
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
            if (this.integerValue)
            {
                Serializer.SerializeObj((bool) this.integerValue, "IntegerValue", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = GeneralCompareType.EqualsTo;
            this.integerValue = false;
        }

        /// <summary>
        /// Gets the type of the comparison.
        /// </summary>
        /// <value>The type of the comparison. The default value is <see cref="T:Dt.Cells.Data.GeneralCompareType">Equals</see>.</value>
        [DefaultValue(0)]
        public GeneralCompareType CompareType
        {
            get { return  this.compareType; }
            set { this.compareType = value; }
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override Type ExpectedValueType
        {
            get { return  typeof(double); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to only check integer values.
        /// </summary>
        /// <value>
        /// <c>true</c> if the comparison only checks integer values; otherwise, <c>false</c>.
        /// The default value is <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IntegerValue
        {
            get { return  this.integerValue; }
            set { this.integerValue = value; }
        }
    }
}

