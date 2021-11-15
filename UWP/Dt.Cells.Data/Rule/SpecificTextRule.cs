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
    /// Represents a text condition rule.
    /// </summary>
    public sealed class SpecificTextRule : FormattingRuleBase
    {
        TextComparisonOperator op;
        string text;

        /// <summary>
        /// Creates a new text rule.
        /// </summary>
        public SpecificTextRule() : this(TextComparisonOperator.Contains, string.Empty, null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="op">The comparison operator.</param>
        /// <param name="text">The text for comparison.</param>
        /// <param name="style">Cell style.</param>
        internal SpecificTextRule(TextComparisonOperator op, string text, StyleInfo style) : base(style)
        {
            this.op = op;
            this.text = text;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            SpecificTextRule rule = base.Clone() as SpecificTextRule;
            rule.op = this.op;
            rule.text = this.text;
            return rule;
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="op">The comparison operator.</param>
        /// <param name="text">The text for comparison.</param>
        /// <param name="style">The cell style.</param>
        /// <returns>The new text rule that is created. </returns>
        public static SpecificTextRule Create(TextComparisonOperator op, string text, StyleInfo style)
        {
            return new SpecificTextRule(op, text, style);
        }

        /// <summary>
        /// Creates a condition for the current rule.
        /// </summary>
        /// <returns>The condition base of the rule.</returns>
        protected override ConditionBase CreateCondition()
        {
            TextCompareType contains;
            switch (this.Operator)
            {
                case TextComparisonOperator.Contains:
                    contains = TextCompareType.Contains;
                    break;

                case TextComparisonOperator.DoesNotContain:
                    contains = TextCompareType.DoesNotContain;
                    break;

                case TextComparisonOperator.BeginsWith:
                    contains = TextCompareType.BeginsWith;
                    break;

                case TextComparisonOperator.EndsWith:
                    contains = TextCompareType.EndsWith;
                    break;

                default:
                    contains = TextCompareType.EqualsTo;
                    break;
            }
            TextCondition condition = TextCondition.FromString(contains, this.text);
            condition.IgnoreCase = true;
            return condition;
        }

        /// <summary>
        /// Reads xml from the xml reader.
        /// </summary>
        /// <param name="reader">The xml reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "Operator")
                {
                    if (name != "Text")
                    {
                        return;
                    }
                }
                else
                {
                    this.op = (TextComparisonOperator) Serializer.DeserializeObj(typeof(TextComparisonOperator), reader);
                    return;
                }
                this.text = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
            }
        }

        /// <summary>
        /// Writes xml to the xml writer.
        /// </summary>
        /// <param name="writer">The xml writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.op != TextComparisonOperator.Contains)
            {
                Serializer.SerializeObj(this.op, "Operator", writer);
            }
            if (!string.IsNullOrEmpty(this.text))
            {
                Serializer.SerializeObj(this.text, "Text", writer);
            }
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.op = TextComparisonOperator.Contains;
            this.text = string.Empty;
        }

        /// <summary>
        /// Gets the comparison operator for the text rule.
        /// </summary>
        /// <value>
        /// The comparison operator for the text rule.
        /// The default value is <see cref="T:Dt.Cells.Data.TextComparisonOperator">Containing</see>.
        /// </value>
        [DefaultValue(0)]
        public TextComparisonOperator Operator
        {
            get { return  this.op; }
            set
            {
                this.op = value;
                this.OnPropertyChanged("Operator");
            }
        }

        /// <summary>
        /// Gets the comparison formula for the text rule.
        /// </summary>
        /// <value>The comparison formula of the text rule. </value>
        [DefaultValue("")]
        public string Text
        {
            get { return  this.text; }
            set
            {
                this.text = value;
                this.OnPropertyChanged("Text");
            }
        }
    }
}

