#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a data validator base.
    /// </summary>
    public class DataValidator : ICloneable, IXmlSerializable
    {
        Dt.Cells.Data.ComparisonOperator comparisonOperator;
        ConditionBase condition;
        string errorMessage;
        Dt.Cells.Data.ErrorStyle errorStyle;
        string errorTitle;
        bool ignoreBlank;
        bool inCellDropdown;
        string inputMessage;
        string inputTitle;
        bool showErrorMessage;
        bool showInputMessage;
        CriteriaType type;

        /// <summary>
        /// Constructs a data validator.
        /// </summary>
        public DataValidator() : this(null)
        {
        }

        DataValidator(ConditionBase condition)
        {
            this.Init();
            this.condition = condition;
            if (this.condition != null)
            {
                this.condition.IgnoreBlank = this.IgnoreBlank;
            }
            this.type = CriteriaType.AnyValue;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            DataValidator validator = Activator.CreateInstance(base.GetType()) as DataValidator;
            validator.errorStyle = this.errorStyle;
            validator.ignoreBlank = this.ignoreBlank;
            validator.inCellDropdown = this.inCellDropdown;
            validator.showInputMessage = this.showInputMessage;
            validator.showErrorMessage = this.showErrorMessage;
            validator.inputTitle = this.inputTitle;
            validator.errorTitle = this.errorTitle;
            validator.inputMessage = this.inputMessage;
            validator.errorMessage = this.errorMessage;
            validator.comparisonOperator = this.comparisonOperator;
            validator.condition = (this.condition == null) ? null : (this.condition.Clone() as ConditionBase);
            validator.type = this.type;
            return validator;
        }

        /// <summary>
        /// Creates a new data validator based on the specified parameters.
        /// </summary>
        /// <param name="rowOffset">The row offset.</param>
        /// <param name="columnOffset">The column offset.</param>
        /// <returns>A new data validator.</returns>
        public virtual DataValidator Clone(int rowOffset, int columnOffset)
        {
            DataValidator validator = this.Clone() as DataValidator;
            validator.condition = (this.condition == null) ? null : (this.condition.Clone() as ConditionBase);
            validator.type = this.type;
            if (validator.condition != null)
            {
                validator.condition.AdjustOffset(rowOffset, columnOffset, false);
            }
            return validator;
        }

        /// <summary>
        /// Creates a validator based on the data.
        /// </summary>
        /// <param name="typeOperator">
        /// The type of <see cref="P:Dt.Cells.Data.DataValidator.ComparisonOperator" /> compare operator.
        /// </param>
        /// <param name="v1">The first object.</param>
        /// <param name="v2">The second object.</param>
        /// <param name="isTime">
        /// <c>true</c> if the validator is set to time; otherwise, <c>false</c>.
        /// </param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateDateValidator(Dt.Cells.Data.ComparisonOperator typeOperator, object v1, object v2, bool isTime)
        {
            string formula = IsFormula(v1) ? (v1 as string).TrimStart(new char[] { '=' }) : null;
            object expected = IsFormula(v1) ? null : v1;
            string str2 = IsFormula(v2) ? (v2 as string).TrimStart(new char[] { '=' }) : null;
            object obj3 = IsFormula(v2) ? null : v2;
            ConditionBase condition = null;
            if (isTime)
            {
                switch (typeOperator)
                {
                    case Dt.Cells.Data.ComparisonOperator.Between:
                        condition = new RelationCondition(RelationCompareType.And, new TimeCondition(DateCompareType.AfterEqualsTo, expected, formula), new TimeCondition(DateCompareType.BeforeEqualsTo, obj3, str2));
                        break;

                    case Dt.Cells.Data.ComparisonOperator.NotBetween:
                        condition = new RelationCondition(RelationCompareType.Or, new TimeCondition(DateCompareType.Before, expected, formula), new TimeCondition(DateCompareType.After, obj3, str2));
                        break;

                    case Dt.Cells.Data.ComparisonOperator.EqualTo:
                        condition = new TimeCondition(DateCompareType.EqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.NotEqualTo:
                        condition = new TimeCondition(DateCompareType.NotEqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.GreaterThan:
                        condition = new TimeCondition(DateCompareType.After, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.LessThan:
                        condition = new TimeCondition(DateCompareType.Before, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.GreaterThanOrEqualTo:
                        condition = new TimeCondition(DateCompareType.AfterEqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.LessThanOrEqualTo:
                        condition = new TimeCondition(DateCompareType.BeforeEqualsTo, expected, formula);
                        break;
                }
            }
            else
            {
                switch (typeOperator)
                {
                    case Dt.Cells.Data.ComparisonOperator.Between:
                        condition = new RelationCondition(RelationCompareType.And, new DateCondition(DateCompareType.AfterEqualsTo, expected, formula), new DateCondition(DateCompareType.BeforeEqualsTo, obj3, str2));
                        break;

                    case Dt.Cells.Data.ComparisonOperator.NotBetween:
                        condition = new RelationCondition(RelationCompareType.Or, new DateCondition(DateCompareType.Before, expected, formula), new DateCondition(DateCompareType.After, obj3, str2));
                        break;

                    case Dt.Cells.Data.ComparisonOperator.EqualTo:
                        condition = new DateCondition(DateCompareType.EqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.NotEqualTo:
                        condition = new DateCondition(DateCompareType.NotEqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.GreaterThan:
                        condition = new DateCondition(DateCompareType.After, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.LessThan:
                        condition = new DateCondition(DateCompareType.Before, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.GreaterThanOrEqualTo:
                        condition = new DateCondition(DateCompareType.AfterEqualsTo, expected, formula);
                        break;

                    case Dt.Cells.Data.ComparisonOperator.LessThanOrEqualTo:
                        condition = new DateCondition(DateCompareType.BeforeEqualsTo, expected, formula);
                        break;
                }
            }
            return new DataValidator(condition) { type = isTime ? CriteriaType.Time : CriteriaType.Date, ComparisonOperator = typeOperator };
        }

        /// <summary>
        /// Creates a validator based on a formula list.
        /// </summary>
        /// <param name="formula">The formula condition.</param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateFormulaListValidator(string formula)
        {
            return new DataValidator(AreaCondition.FromFormula(formula)) { type = CriteriaType.List };
        }

        /// <summary>
        /// Creates a validator based on a formula.
        /// </summary>
        /// <param name="formula">The formula condition.</param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateFormulaValidator(string formula)
        {
            string str = formula;
            return new DataValidator(FormulaCondition.FromFormula(str.TrimStart(new char[] { '=' }))) { type = CriteriaType.Custom };
        }

        /// <summary>
        /// Creates a validator based on a list.
        /// </summary>
        /// <param name="source">The list value.</param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateListValidator(string source)
        {
            return new DataValidator(AreaCondition.FromSource(source)) { type = CriteriaType.List };
        }

        /// <summary>
        /// Creates a validator based on numbers.
        /// </summary>
        /// <param name="typeOperator">
        /// The type of <see cref="P:Dt.Cells.Data.DataValidator.ComparisonOperator" /> compare operator.
        /// </param>
        /// <param name="v1">The first object.</param>
        /// <param name="v2">The second object.</param>
        /// <param name="isIntegerValue">
        /// <c>true</c> if the validator is set to a number; otherwise, <c>false</c>.
        /// </param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateNumberValidator(Dt.Cells.Data.ComparisonOperator typeOperator, object v1, object v2, bool isIntegerValue)
        {
            string formula = IsFormula(v1) ? (v1 as string).TrimStart(new char[] { '=' }) : null;
            object expected = IsFormula(v1) ? null : v1;
            string str2 = IsFormula(v2) ? (v2 as string).TrimStart(new char[] { '=' }) : null;
            object obj3 = IsFormula(v2) ? null : v2;
            ConditionBase base2 = null;
            switch (typeOperator)
            {
                case Dt.Cells.Data.ComparisonOperator.Between:
                {
                    NumberCondition condition = new NumberCondition(GeneralCompareType.GreaterThanOrEqualsTo, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    NumberCondition condition2 = new NumberCondition(GeneralCompareType.LessThanOrEqualsTo, obj3, str2) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = new RelationCondition(RelationCompareType.And, condition, condition2);
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.NotBetween:
                {
                    NumberCondition condition3 = new NumberCondition(GeneralCompareType.LessThan, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    NumberCondition condition4 = new NumberCondition(GeneralCompareType.GreaterThan, obj3, str2) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = new RelationCondition(RelationCompareType.Or, condition3, condition4);
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.EqualTo:
                {
                    NumberCondition condition5 = new NumberCondition(GeneralCompareType.EqualsTo, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition5;
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.NotEqualTo:
                {
                    NumberCondition condition6 = new NumberCondition(GeneralCompareType.NotEqualsTo, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition6;
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.GreaterThan:
                {
                    NumberCondition condition7 = new NumberCondition(GeneralCompareType.GreaterThan, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition7;
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.LessThan:
                {
                    NumberCondition condition9 = new NumberCondition(GeneralCompareType.LessThan, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition9;
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.GreaterThanOrEqualTo:
                {
                    NumberCondition condition8 = new NumberCondition(GeneralCompareType.GreaterThanOrEqualsTo, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition8;
                    break;
                }
                case Dt.Cells.Data.ComparisonOperator.LessThanOrEqualTo:
                {
                    NumberCondition condition10 = new NumberCondition(GeneralCompareType.LessThanOrEqualsTo, expected, formula) {
                        IntegerValue = isIntegerValue
                    };
                    base2 = condition10;
                    break;
                }
            }
            return new DataValidator(base2) { type = isIntegerValue ? CriteriaType.WholeNumber : CriteriaType.DecimalValues, ComparisonOperator = typeOperator };
        }

        /// <summary>
        /// Creates a validator based on text length.
        /// </summary>
        /// <param name="typeOperator">
        /// The type of <see cref="P:Dt.Cells.Data.DataValidator.ComparisonOperator" /> compare operator.
        /// </param>
        /// <param name="v1">The first object.</param>
        /// <param name="v2">The second object.</param>
        /// <returns>Returns a new validator.</returns>
        public static DataValidator CreateTextLengthValidator(Dt.Cells.Data.ComparisonOperator typeOperator, object v1, object v2)
        {
            string formula = IsFormula(v1) ? (v1 as string).TrimStart(new char[] { '=' }) : null;
            object expected = IsFormula(v1) ? null : v1;
            string str2 = IsFormula(v2) ? (v2 as string).TrimStart(new char[] { '=' }) : null;
            object obj3 = IsFormula(v2) ? null : v2;
            ConditionBase condition = null;
            switch (typeOperator)
            {
                case Dt.Cells.Data.ComparisonOperator.Between:
                    condition = new RelationCondition(RelationCompareType.And, new TextLengthCondition(GeneralCompareType.GreaterThanOrEqualsTo, expected, formula), new TextLengthCondition(GeneralCompareType.LessThanOrEqualsTo, obj3, str2));
                    break;

                case Dt.Cells.Data.ComparisonOperator.NotBetween:
                    condition = new RelationCondition(RelationCompareType.Or, new TextLengthCondition(GeneralCompareType.LessThan, expected, formula), new TextLengthCondition(GeneralCompareType.GreaterThan, obj3, str2));
                    break;

                case Dt.Cells.Data.ComparisonOperator.EqualTo:
                    condition = new TextLengthCondition(GeneralCompareType.EqualsTo, expected, formula);
                    break;

                case Dt.Cells.Data.ComparisonOperator.NotEqualTo:
                    condition = new TextLengthCondition(GeneralCompareType.NotEqualsTo, expected, formula);
                    break;

                case Dt.Cells.Data.ComparisonOperator.GreaterThan:
                    condition = new TextLengthCondition(GeneralCompareType.GreaterThan, expected, formula);
                    break;

                case Dt.Cells.Data.ComparisonOperator.LessThan:
                    condition = new TextLengthCondition(GeneralCompareType.LessThan, expected, formula);
                    break;

                case Dt.Cells.Data.ComparisonOperator.GreaterThanOrEqualTo:
                    condition = new TextLengthCondition(GeneralCompareType.GreaterThanOrEqualsTo, expected, formula);
                    break;

                case Dt.Cells.Data.ComparisonOperator.LessThanOrEqualTo:
                    condition = new TextLengthCondition(GeneralCompareType.LessThanOrEqualsTo, expected, formula);
                    break;
            }
            return new DataValidator(condition) { type = CriteriaType.TextLength, ComparisonOperator = typeOperator };
        }

        /// <summary>
        /// Returns the valid data lists if the data validation type is list; otherwise, returns null.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <returns>The valid list.</returns>
        public object[] GetValidList(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            if (((this.Condition != null) && (this.Type == CriteriaType.List)) && (this.Condition is AreaCondition))
            {
                return (this.Condition as AreaCondition).GetValidList(evaluator, baseRow, baseColumn);
            }
            return null;
        }

        void Init()
        {
            this.errorStyle = Dt.Cells.Data.ErrorStyle.Stop;
            this.ignoreBlank = true;
            this.inCellDropdown = true;
            this.showInputMessage = true;
            this.showErrorMessage = true;
            this.inputTitle = string.Empty;
            this.errorTitle = string.Empty;
            this.inputMessage = string.Empty;
            this.errorMessage = string.Empty;
            this.comparisonOperator = Dt.Cells.Data.ComparisonOperator.Between;
        }

        static bool IsFormula(object val)
        {
            return ((val is string) && val.ToString().StartsWith("="));
        }

        /// <summary>
        /// Determines whether the current value is valid.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actual">The current value.</param>
        /// <returns>
        /// <c>true</c> The value is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValid(ICalcEvaluator evaluator, int baseRow, int baseColumn, object actual)
        {
            if (this.condition == null)
            {
                return true;
            }
            if (this.condition.IgnoreBlank && ((actual == null) || ((actual != null) && string.IsNullOrEmpty(actual.ToString()))))
            {
                return true;
            }
            object obj2 = actual;
            if (actual != null)
            {
                switch (this.type)
                {
                    case CriteriaType.AnyValue:
                        return true;

                    case CriteriaType.WholeNumber:
                    case CriteriaType.DecimalValues:
                    {
                        double? nullable = ConditionValueConverter.TryDouble(actual);
                        if (nullable.HasValue)
                        {
                            obj2 = nullable;
                        }
                        break;
                    }
                    case CriteriaType.Date:
                    case CriteriaType.Time:
                    {
                        DateTime? nullable2 = ConditionValueConverter.TryDateTime(actual);
                        if (nullable2.HasValue)
                        {
                            obj2 = nullable2;
                        }
                        break;
                    }
                }
            }
            return this.condition.Evaluate(evaluator, baseRow, baseColumn, new ActualValue(obj2));
        }

        /// <summary>
        /// Generates the rule from its XML representation.
        /// </summary>
        protected virtual void OnReadXml(XmlReader reader)
        {
            switch (reader.Name)
            {
                case "ErrorStyle":
                    this.errorStyle = (Dt.Cells.Data.ErrorStyle) Serializer.DeserializeObj(typeof(Dt.Cells.Data.ErrorStyle), reader);
                    return;

                case "IgnoreBlank":
                    this.ignoreBlank = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "InCellDropdown":
                    this.inCellDropdown = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ShowInputMessage":
                    this.showInputMessage = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "ShowErrorMessage":
                    this.showErrorMessage = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;

                case "InputTitle":
                    this.inputTitle = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;

                case "ErrorTitle":
                    this.errorTitle = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;

                case "InputMessage":
                    this.inputMessage = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;

                case "ErrorMessage":
                    this.errorMessage = (string) (Serializer.DeserializeObj(typeof(string), reader) as string);
                    return;

                case "Operator":
                    this.comparisonOperator = (Dt.Cells.Data.ComparisonOperator) Serializer.DeserializeObj(typeof(Dt.Cells.Data.ComparisonOperator), reader);
                    return;

                case "Condition":
                    this.condition = Serializer.DeserializeObj(null, reader) as ConditionBase;
                    return;

                case "Type":
                    this.type = (CriteriaType) Serializer.DeserializeObj(typeof(CriteriaType), reader);
                    return;
            }
        }

        /// <summary>
        /// Converts the rule into its XML representation.
        /// </summary>
        protected virtual void OnWriteXml(XmlWriter writer)
        {
            if (this.errorStyle != Dt.Cells.Data.ErrorStyle.Stop)
            {
                Serializer.SerializeObj(this.errorStyle, "ErrorStyle", writer);
            }
            if (!this.ignoreBlank)
            {
                Serializer.SerializeObj((bool) this.ignoreBlank, "IgnoreBlank", writer);
            }
            if (!this.inCellDropdown)
            {
                Serializer.SerializeObj((bool) this.inCellDropdown, "InCellDropdown", writer);
            }
            if (!this.showInputMessage)
            {
                Serializer.SerializeObj((bool) this.showInputMessage, "ShowInputMessage", writer);
            }
            if (!this.showErrorMessage)
            {
                Serializer.SerializeObj((bool) this.showErrorMessage, "ShowErrorMessage", writer);
            }
            if (!string.IsNullOrEmpty(this.inputTitle))
            {
                Serializer.SerializeObj(this.inputTitle, "InputTitle", writer);
            }
            if (!string.IsNullOrEmpty(this.errorTitle))
            {
                Serializer.SerializeObj(this.errorTitle, "ErrorTitle", writer);
            }
            if (!string.IsNullOrEmpty(this.inputMessage))
            {
                Serializer.SerializeObj(this.inputMessage, "InputMessage", writer);
            }
            if (!string.IsNullOrEmpty(this.errorMessage))
            {
                Serializer.SerializeObj(this.errorMessage, "ErrorMessage", writer);
            }
            Serializer.SerializeObj(this.comparisonOperator, "Operator", writer);
            if (this.condition != null)
            {
                Serializer.WriteStartObj("Condition", writer);
                Serializer.WriteTypeAttr(this.condition, writer);
                Serializer.SerializeObj(this.condition, null, writer);
                Serializer.WriteEndObj(writer);
            }
            if (this.type != CriteriaType.AnyValue)
            {
                Serializer.SerializeObj(this.type, "Type", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected virtual void Reset()
        {
            this.Init();
            this.type = CriteriaType.AnyValue;
            this.condition = null;
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Reset();
            while (reader.Read())
            {
                if (reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element)))
                {
                    this.OnReadXml(reader);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.OnWriteXml(writer);
        }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        /// <value>
        /// The comparison operator.
        /// The default value is <see cref="P:Dt.Cells.Data.DataValidator.ComparisonOperator">Between</see>.
        /// </value>
        [DefaultValue(1)]
        public Dt.Cells.Data.ComparisonOperator ComparisonOperator
        {
            get { return  this.comparisonOperator; }
            internal set { this.comparisonOperator = value; }
        }

        internal ConditionBase Condition
        {
            get { return  this.condition; }
            set { this.condition = value; }
        }

        /// <summary>
        /// Gets or sets the error message string.
        /// </summary>
        /// <value>The error message string. The default value is an empty string.</value>
        [DefaultValue("")]
        public string ErrorMessage
        {
            get { return  this.errorMessage; }
            set { this.errorMessage = value; }
        }

        /// <summary>
        /// Gets the error style.
        /// </summary>
        /// <value>
        /// An <see cref="P:Dt.Cells.Data.DataValidator.ErrorStyle" /> enumeration that specifies the icon type in the error alert.
        /// The default value is <see cref="P:Dt.Cells.Data.DataValidator.ErrorStyle">Stop</see>.
        /// </value>
        [DefaultValue(0)]
        public Dt.Cells.Data.ErrorStyle ErrorStyle
        {
            get { return  this.errorStyle; }
            set { this.errorStyle = value; }
        }

        /// <summary>
        /// Gets or sets the error title string.
        /// </summary>
        /// <value>The error title string. The default value is an empty string.</value>
        [DefaultValue("")]
        public string ErrorTitle
        {
            get { return  this.errorTitle; }
            set { this.errorTitle = value; }
        }

        internal IConditionalFormula[] FormulaConditions
        {
            get
            {
                if (this.condition == null)
                {
                    return null;
                }
                ConditionBase base2 = this.condition;
                Stack<ConditionBase> stack = new Stack<ConditionBase>();
                stack.Push(base2);
                List<IConditionalFormula> list = new List<IConditionalFormula>();
                while (stack.Count > 0)
                {
                    base2 = stack.Pop();
                    RelationCondition condition = base2 as RelationCondition;
                    if (condition != null)
                    {
                        if (condition.Item1 != null)
                        {
                            if (!string.IsNullOrEmpty(condition.Item1.ExpectedFormula))
                            {
                                list.Add(condition.Item1);
                            }
                            else
                            {
                                stack.Push(condition.Item1);
                            }
                        }
                        if (condition.Item2 != null)
                        {
                            if (!string.IsNullOrEmpty(condition.Item2.ExpectedFormula))
                            {
                                list.Add(condition.Item2);
                            }
                            else
                            {
                                stack.Push(condition.Item2);
                            }
                        }
                    }
                    else if ((base2 != null) && !string.IsNullOrEmpty(base2.ExpectedFormula))
                    {
                        list.Add(base2);
                    }
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets whether to ignore an empty value.
        /// </summary>
        /// <value>
        /// <c>true</c> if the validator ignores the empty value; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public virtual bool IgnoreBlank
        {
            get { return  this.ignoreBlank; }
            set
            {
                this.ignoreBlank = value;
                if (this.condition != null)
                {
                    this.condition.IgnoreBlank = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to show the candidate legal data in a drop down list.
        /// </summary>
        /// <value>
        /// <c>true</c> if the data validator shows the candidate legal data in a drop down list; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool InCellDropdown
        {
            get { return  this.inCellDropdown; }
            set { this.inCellDropdown = value; }
        }

        /// <summary>
        /// Gets or sets the input message string.
        /// </summary>
        /// <value>The input message string. The default value is an empty string.</value>
        [DefaultValue("")]
        public string InputMessage
        {
            get { return  this.inputMessage; }
            set { this.inputMessage = value; }
        }

        /// <summary>
        /// Gets or sets the input title string.
        /// </summary>
        /// <value>The input title string. The default value is an empty string.</value>
        [DefaultValue("")]
        public string InputTitle
        {
            get { return  this.inputTitle; }
            set { this.inputTitle = value; }
        }

        /// <summary>
        /// Gets or sets whether to show the error message.
        /// </summary>
        /// <value>
        /// <c>true</c> if the data validator shows the error message; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowErrorMessage
        {
            get { return  this.showErrorMessage; }
            set { this.showErrorMessage = value; }
        }

        /// <summary>
        /// Gets or sets whether to show the input message.
        /// </summary>
        /// <value>
        /// <c>true</c> if the data validator shows the input message; otherwise, <c>false</c>.
        /// The default value is <c>true</c>.
        /// </value>
        [DefaultValue(true)]
        public bool ShowInputMessage
        {
            get { return  this.showInputMessage; }
            set { this.showInputMessage = value; }
        }

        /// <summary>
        /// Gets the criteria type of this data validator.
        /// </summary>
        /// <value>
        /// The criteria type of this data validator.
        /// The default value is <see cref="T:Dt.Cells.Data.CriteriaType">AnyValue</see>.
        /// </value>
        [DefaultValue(0)]
        public virtual CriteriaType Type
        {
            get { return  this.type; }
        }

        /// <summary>
        /// Gets  the first value of the data validation.
        /// </summary>
        public object Value1
        {
            get
            {
                ConditionBase base2 = (this.condition is RelationCondition) ? ((RelationCondition) this.condition).Item1 : this.condition;
                if (base2 == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(base2.ExpectedFormula))
                {
                    return base2.ExpectedValue;
                }
                if (base2.ExpectedFormula.StartsWith("="))
                {
                    return base2.ExpectedFormula.ToUpper();
                }
                return ("=" + base2.ExpectedFormula.ToUpper());
            }
        }

        /// <summary>
        /// Gets the second value of the data validation.
        /// </summary>
        public object Value2
        {
            get
            {
                ConditionBase base2 = (this.condition is RelationCondition) ? ((RelationCondition) this.condition).Item2 : null;
                if (base2 == null)
                {
                    return null;
                }
                if (string.IsNullOrEmpty(base2.ExpectedFormula))
                {
                    return base2.ExpectedValue;
                }
                if (base2.ExpectedFormula.StartsWith("="))
                {
                    return base2.ExpectedFormula.ToUpper();
                }
                return ("=" + base2.ExpectedFormula.ToUpper());
            }
        }
    }
}

