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
    /// Represents a cell value condition rule.
    /// </summary>
    public sealed class CellValueRule : FormattingRuleBase
    {
        ComparisonOperator op;
        object value1;
        object value2;

        /// <summary>
        /// Creates a new text rule.
        /// </summary>
        public CellValueRule() : this(ComparisonOperator.Between, null, null, null)
        {
        }

        internal CellValueRule(ComparisonOperator op, object value1, object value2, StyleInfo style) : base(style)
        {
            this.op = op;
            this.value1 = value1;
            this.value2 = value2;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            CellValueRule rule = base.Clone() as CellValueRule;
            rule.value1 = this.value1;
            rule.value2 = this.value2;
            return rule;
        }

        /// <summary>
        /// Creates a cell value data rule with specified parameters.
        /// </summary>
        /// <param name="op">
        /// The comparison operator <see cref="T:Dt.Cells.Data.ComparisonOperator" /> for the text rule.
        /// </param>
        /// <param name="value1">The first object.</param>
        /// <param name="value2">The second object.</param>
        /// <param name="style">The style that is set by the rule.</param>
        /// <returns>The new cell value rule.</returns>
        public static CellValueRule Create(ComparisonOperator op, object value1, object value2, StyleInfo style)
        {
            return new CellValueRule(op, value1, value2, style);
        }

        /// <summary>
        /// Creates conditions for the rule.
        /// </summary>
        /// <returns>
        /// The  condition.
        /// </returns>
        protected override ConditionBase CreateCondition()
        {
            GeneralCompareType equalsTo;
            CellValueCondition condition;
            string formula = IsFormula(this.value1) ? (this.value1 as string).TrimStart(new char[] { '=' }) : null;
            IsFormula(this.value1);
            string str2 = IsFormula(this.value2) ? (this.value2 as string).TrimStart(new char[] { '=' }) : null;
            IsFormula(this.value2);
            switch (this.op)
            {
                case ComparisonOperator.Between:
                {
                    CellValueCondition condition4 = new CellValueCondition(GeneralCompareType.GreaterThanOrEqualsTo, this.value1, formula) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base2 = condition4;
                    CellValueCondition condition5 = new CellValueCondition(GeneralCompareType.LessThanOrEqualsTo, this.value2, str2) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base3 = condition5;
                    CellValueCondition condition6 = new CellValueCondition(GeneralCompareType.LessThanOrEqualsTo, this.value1, formula) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base4 = condition6;
                    CellValueCondition condition7 = new CellValueCondition(GeneralCompareType.GreaterThanOrEqualsTo, this.value2, str2) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base5 = condition7;
                    RelationCondition condition2 = new RelationCondition(RelationCompareType.And, base2, base3);
                    return new RelationCondition(RelationCompareType.Or, condition2, new RelationCondition(RelationCompareType.And, base4, base5));
                }
                case ComparisonOperator.NotBetween:
                {
                    CellValueCondition condition10 = new CellValueCondition(GeneralCompareType.LessThan, this.value1, formula) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base6 = condition10;
                    CellValueCondition condition11 = new CellValueCondition(GeneralCompareType.GreaterThan, this.value2, str2) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base7 = condition11;
                    CellValueCondition condition12 = new CellValueCondition(GeneralCompareType.GreaterThan, this.value1, formula) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base8 = condition12;
                    CellValueCondition condition13 = new CellValueCondition(GeneralCompareType.LessThan, this.value2, str2) {
                        TreatNullValueAsZero = true
                    };
                    ConditionBase base9 = condition13;
                    RelationCondition condition8 = new RelationCondition(RelationCompareType.Or, base6, base7);
                    return new RelationCondition(RelationCompareType.And, condition8, new RelationCondition(RelationCompareType.Or, base8, base9));
                }
                case ComparisonOperator.EqualTo:
                case ComparisonOperator.NotEqualTo:
                case ComparisonOperator.GreaterThan:
                case ComparisonOperator.LessThan:
                case ComparisonOperator.GreaterThanOrEqualTo:
                case ComparisonOperator.LessThanOrEqualTo:
                    equalsTo = GeneralCompareType.EqualsTo;
                    switch (this.op)
                    {
                        case ComparisonOperator.EqualTo:
                            equalsTo = GeneralCompareType.EqualsTo;
                            goto Label_00F3;

                        case ComparisonOperator.NotEqualTo:
                            equalsTo = GeneralCompareType.NotEqualsTo;
                            goto Label_00F3;

                        case ComparisonOperator.GreaterThan:
                            equalsTo = GeneralCompareType.GreaterThan;
                            goto Label_00F3;

                        case ComparisonOperator.LessThan:
                            equalsTo = GeneralCompareType.LessThan;
                            goto Label_00F3;

                        case ComparisonOperator.GreaterThanOrEqualTo:
                            equalsTo = GeneralCompareType.GreaterThanOrEqualsTo;
                            goto Label_00F3;

                        case ComparisonOperator.LessThanOrEqualTo:
                            equalsTo = GeneralCompareType.LessThanOrEqualsTo;
                            goto Label_00F3;
                    }
                    break;

                default:
                    return null;
            }
        Label_00F3:
            condition = new CellValueCondition(equalsTo, this.value1, formula);
            condition.TreatNullValueAsZero = true;
            return condition;
        }

        static bool IsFormula(object val)
        {
            return (((val is string) && val.ToString().StartsWith("=")) && (val.ToString().Length > 1));
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
                    if (name != "Value1")
                    {
                        if (name == "Value2")
                        {
                            this.value2 = Serializer.DeserializeObj(null, reader);
                        }
                        return;
                    }
                }
                else
                {
                    this.op = (ComparisonOperator) Serializer.DeserializeObj(typeof(ComparisonOperator), reader);
                    return;
                }
                this.value1 = Serializer.DeserializeObj(null, reader);
            }
        }

        /// <summary>
        /// Converts the object into its XML representation.
        /// </summary>
        /// <param name="writer">The XML writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.op != ComparisonOperator.Between)
            {
                Serializer.SerializeObj(this.op, "Operator", writer);
            }
            if (this.value1 != null)
            {
                Serializer.WriteStartObj("Value1", writer);
                Serializer.WriteTypeAttr(this.value1, writer);
                Serializer.SerializeObj(this.value1, null, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.value2 != null)
            {
                Serializer.WriteStartObj("Value2", writer);
                Serializer.WriteTypeAttr(this.value2, writer);
                Serializer.SerializeObj(this.value2, null, writer);
                Serializer.WriteEndObj(writer);
            }
        }

        /// <summary>
        /// Resets the rule.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.op = ComparisonOperator.Between;
            this.value1 = null;
            this.value2 = null;
        }

        /// <summary>
        /// Gets the comparison operator for the text rule.
        /// </summary>
        /// <value>
        /// The comparison operator for the text rule.
        /// The default value is <see cref="T:Dt.Cells.Data.TextComparisonOperator">Containing</see>.
        /// </value>
        [DefaultValue(1)]
        public ComparisonOperator Operator
        {
            get { return  this.op; }
            set
            {
                this.op = value;
                this.OnPropertyChanged("Operator");
            }
        }

        /// <summary>
        /// Gets or sets the value of the first object.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        [DefaultValue((string) null)]
        public object Value1
        {
            get { return  this.value1; }
            set
            {
                this.value1 = value;
                this.OnPropertyChanged("Value1");
            }
        }

        /// <summary>
        /// Gets or sets the value of the second object.
        /// </summary>
        /// <value>
        /// The default is null.
        /// </value>
        [DefaultValue((string) null)]
        public object Value2
        {
            get { return  this.value2; }
            set
            {
                this.value2 = value;
                this.OnPropertyChanged("Value2");
            }
        }
    }
}

