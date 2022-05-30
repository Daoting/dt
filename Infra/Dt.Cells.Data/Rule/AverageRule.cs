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
    /// Represents an average condition rule.
    /// </summary>
    public sealed class AverageRule : FormattingRuleBase
    {
        AverageConditionType op;

        /// <summary>
        /// Constructs an average rule.
        /// </summary>
        public AverageRule() : this(AverageConditionType.Above, null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="op">Comparison operator.</param>
        /// <param name="style">Cell style.</param>
        internal AverageRule(AverageConditionType op, StyleInfo style) : base(style)
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
            AverageRule rule = base.Clone() as AverageRule;
            rule.op = this.op;
            return rule;
        }

        /// <summary>
        /// Creates an average rule with the specified average condition and style information.
        /// </summary>
        /// <param name="op">
        /// The  <see cref="T:Dt.Cells.Data.AverageConditionType" /> average condition type.
        /// </param>
        /// <param name="style"> The style.</param>
        /// <returns>The new average rule.</returns>
        public static AverageRule Create(AverageConditionType op, StyleInfo style)
        {
            return new AverageRule(op, style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return new AverageCondition(this.op, base.Ranges);
        }

        /// <summary>
        /// Initial condition for the rule.
        /// </summary>
        protected override void InitCondition()
        {
            base.InitCondition();
            AverageCondition condition = base.condition as AverageCondition;
            if (condition != null)
            {
                condition.Ranges = base.Ranges;
            }
        }

        /// <summary>
        /// Reads XML from the XML reader.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Operator"))
            {
                this.op = (AverageConditionType) Serializer.DeserializeObj(typeof(AverageConditionType), reader);
            }
        }

        /// <summary>
        /// Writes the rule to the XML writer.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.op != AverageConditionType.Above)
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
            this.op = AverageConditionType.Above;
        }

        /// <summary>
        /// Gets the comparison operator for the text rule.
        /// </summary>
        /// <value>
        /// The comparison operator for the text rule.
        /// The default value is <see cref="T:Dt.Cells.Data.TextComparisonOperator">Containing</see>.
        /// </value>
        [DefaultValue(0)]
        public AverageConditionType Operator
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

