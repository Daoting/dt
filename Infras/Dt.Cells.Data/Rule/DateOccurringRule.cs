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
    /// Represents a date occurring rule.
    /// </summary>
    public sealed class DateOccurringRule : FormattingRuleBase
    {
        DateOccurringType op;

        /// <summary>
        /// Constructs a data occurring  rule.
        /// </summary>
        public DateOccurringRule() : this(DateOccurringType.Today, null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="op">The comparison operator.</param>
        /// <param name="style">The cell style.</param>
        internal DateOccurringRule(DateOccurringType op, StyleInfo style) : base(style)
        {
            this.op = op;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            DateOccurringRule rule = base.Clone() as DateOccurringRule;
            rule.op = this.op;
            return rule;
        }

        /// <summary>
        /// Creates a data occurring rule with specified parameters.
        /// </summary>
        /// <param name="op">
        /// The <see cref="T:Dt.Cells.Data.DateOccurringType" /> data occurring rule.
        /// </param>
        /// <param name="style">The style that is set by the rule.</param>
        /// <returns>The new data occurring rule.</returns>
        public static DateOccurringRule Create(DateOccurringType op, StyleInfo style)
        {
            return new DateOccurringRule(op, style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return new DateExCondition(this.op);
        }

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Operator"))
            {
                this.op = (DateOccurringType) Serializer.DeserializeObj(typeof(DateOccurringType), reader);
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.op != DateOccurringType.Today)
            {
                Serializer.SerializeObj(this.op, "Operator", writer);
            }
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.op = DateOccurringType.Today;
        }

        /// <summary>
        /// Gets the comparison operator for the text rule.
        /// </summary>
        /// <value>
        /// The comparison operator for the text rule.
        /// The default value is <see cref="T:Dt.Cells.Data.TextComparisonOperator">Containing</see>.
        /// </value>
        [DefaultValue(0)]
        public DateOccurringType Operator
        {
            get { return  this.op; }
            set
            {
                this.op = value;
                this.OnPropertyChanged("Operator");
            }
        }
    }
}

