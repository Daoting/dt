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
    public sealed class TextLengthCondition : ConditionBase
    {
        GeneralCompareType compareType;

        /// <summary>
        /// Creates a new number condition.
        /// </summary>
        public TextLengthCondition() : this(GeneralCompareType.EqualsTo, (int) 0, null)
        {
        }

        /// <summary>
        /// Creates a new number condition of the specified type with the specified expected value.
        /// </summary>
        /// <param name="compareType">The number comparison type.</param>
        /// <param name="expected">The expected number.</param>
        /// <param name="formula">The expected formula.</param>
        internal TextLengthCondition(GeneralCompareType compareType, object expected, string formula) : base(expected, formula)
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
            TextLengthCondition condition = base.Clone() as TextLengthCondition;
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
            string text = actualObj.GetText();
            if (string.IsNullOrEmpty(text))
            {
                return this.IgnoreBlank;
            }
            int num = (text == null) ? 0 : text.Length;
            int? nullable = base.GetExpectedInt(evaluator, baseRow, baseColumn);
            if (nullable.HasValue)
            {
                switch (this.compareType)
                {
                    case GeneralCompareType.EqualsTo:
                        return (num == nullable.Value);

                    case GeneralCompareType.NotEqualsTo:
                        return (num != nullable.Value);

                    case GeneralCompareType.GreaterThan:
                        return (num > nullable.Value);

                    case GeneralCompareType.GreaterThanOrEqualsTo:
                        return (num >= nullable.Value);

                    case GeneralCompareType.LessThan:
                        return (num < nullable.Value);

                    case GeneralCompareType.LessThanOrEqualsTo:
                        return (num <= nullable.Value);
                }
            }
            return false;
        }

        /// <summary>
        /// Creates a TextLengthCondition object from the formula.
        /// </summary>
        /// <param name="compareType">The compare type.</param>
        /// <param name="formula">The formula.</param>
        /// <returns>The TextLengthCondition object.</returns>
        public static TextLengthCondition FromFormula(GeneralCompareType compareType, string formula)
        {
            return new TextLengthCondition(compareType, (int) 0, formula);
        }

        /// <summary>
        /// Creates a TextLengthCondition object from an integer value.
        /// </summary>
        /// <param name="compareType">The comparison type.</param>
        /// <param name="expected">The expected value.</param>
        /// <returns>The TextLengthCondition object.</returns>
        public static TextLengthCondition FromInt(GeneralCompareType compareType, int expected)
        {
            return new TextLengthCondition(compareType, (int) expected, null);
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
                this.compareType = (GeneralCompareType) Serializer.DeserializeObj(typeof(GeneralCompareType), reader);
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
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.compareType = GeneralCompareType.EqualsTo;
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
            get { return  typeof(int); }
        }
    }
}

