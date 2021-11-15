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
    /// Represents a top 10 rule.
    /// </summary>
    public sealed class Top10Rule : FormattingRuleBase
    {
        Top10ConditionType op;
        int rank;

        /// <summary>
        /// Constructs a top 10 rule.
        /// </summary>
        public Top10Rule() : this(Top10ConditionType.Top, 10, null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="op">Comparison operator.</param>
        /// <param name="rank">The count of top or bottom items</param>
        /// <param name="style">Cell style.</param>
        internal Top10Rule(Top10ConditionType op, int rank, StyleInfo style) : base(style)
        {
            this.op = op;
            this.rank = rank;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            Top10Rule rule = base.Clone() as Top10Rule;
            rule.op = this.op;
            rule.rank = this.rank;
            return rule;
        }

        /// <summary>
        /// Creates a top 10 rule with specified parameters.
        /// </summary>
        /// <param name="op">
        /// The <see cref="T:Dt.Cells.Data.Top10ConditionType" /> top 10 condition type.
        /// </param>
        /// <param name="rank">&gt;The number of top or bottom items </param>
        /// <param name="style">The style that is set by the rule.</param>
        /// <returns>The new top 10 rule.</returns>
        public static Top10Rule Create(Top10ConditionType op, int rank, StyleInfo style)
        {
            return new Top10Rule(op, rank, style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return new Top10Condition(this.op, this.rank, base.Ranges);
        }

        /// <summary>
        /// Initial condition for the rule.
        /// </summary>
        protected override void InitCondition()
        {
            base.InitCondition();
            Top10Condition condition = base.condition as Top10Condition;
            if (condition != null)
            {
                condition.Ranges = base.Ranges;
            }
        }

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);
            string name = reader.Name;
            if (name != null)
            {
                if (name != "Operator")
                {
                    if (name != "Rank")
                    {
                        return;
                    }
                }
                else
                {
                    this.op = (Top10ConditionType) Serializer.DeserializeObj(typeof(Top10ConditionType), reader);
                    return;
                }
                this.rank = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.op != Top10ConditionType.Top)
            {
                Serializer.SerializeObj(this.op, "Operator", writer);
            }
            if (this.rank != 10)
            {
                Serializer.SerializeObj((int) this.rank, "Rank", writer);
            }
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.op = Top10ConditionType.Top;
            this.rank = 10;
        }

        /// <summary>
        /// Not support for V1
        /// </summary>
        internal bool IsPercent
        {
            get { return  false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets the comparison operator for the text rule.
        /// </summary>
        /// <value>
        /// The comparison operator for the text rule.
        /// The default value is <see cref="T:Dt.Cells.Data.TextComparisonOperator">Containing</see>.
        /// </value>
        [DefaultValue(0)]
        public Top10ConditionType Operator
        {
            get { return  this.op; }
            set
            {
                this.op = value;
                this.OnPropertyChanged("Operator");
            }
        }

        /// <summary>
        /// Gets the rank of the Top10Rule.
        /// </summary>
        /// <value>The rank of the Top10Rule. The default is 10.</value>
        [DefaultValue(10)]
        public int Rank
        {
            get { return  this.rank; }
            set
            {
                this.rank = value;
                this.OnPropertyChanged("Rank");
            }
        }
    }
}

