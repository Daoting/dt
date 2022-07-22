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
    /// Represents a formula rule.
    /// </summary>
    public sealed class FormulaRule : FormattingRuleBase, IConditionalFormula
    {
        string formula;

        /// <summary>
        /// Constructs a formula rule.
        /// </summary>
        public FormulaRule() : this(null, null)
        {
        }

        /// <summary>
        /// Creates a new text rule with the specified comparison operator, text, and style.
        /// </summary>
        /// <param name="formula">Formula.</param>
        /// <param name="style">Cell style.</param>
        internal FormulaRule(string formula, StyleInfo style) : base(style)
        {
            this.formula = formula;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            FormulaRule rule = base.Clone() as FormulaRule;
            rule.formula = this.formula;
            return rule;
        }

        /// <summary>
        /// Creates a new formula rule with specified parameters.
        /// </summary>
        /// <param name="formula">The condition formula.</param>
        /// <param name="style">The style that is set by the rule.</param>
        /// <returns>The new formula rule.</returns>
        public static FormulaRule Create(string formula, StyleInfo style)
        {
            return new FormulaRule(formula, style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            return new FormulaCondition(CustomValueType.Formula, string.IsNullOrEmpty(this.formula) ? null : this.formula.TrimStart(new char[] { '=' }));
        }

        static bool IsFormula(object val)
        {
            return ((val is string) && val.ToString().StartsWith("="));
        }

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        /// <param name="reader">The XML reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Formula"))
            {
                this.formula = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (!string.IsNullOrEmpty(this.formula))
            {
                Serializer.SerializeObj(this.formula, "Formula", writer);
            }
        }

        /// <summary>
        /// Resets the rule
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.formula = null;
        }

        /// <summary>
        /// Gets the comparison formula of the text rule.
        /// </summary>
        /// <value>The comparison formula of the text rule. </value>
        [DefaultValue("")]
        public string Formula
        {
            get { return  this.formula; }
            set
            {
                this.formula = value;
                this.OnPropertyChanged("Formula");
            }
        }
    }
}

