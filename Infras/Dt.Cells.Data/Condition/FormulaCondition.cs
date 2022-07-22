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
    /// Represents a formula condition.
    /// </summary>
    /// <remarks>
    /// The condition is true when the result of a formula calculation is true.
    /// </remarks>
    public sealed class FormulaCondition : ConditionBase
    {
        /// <summary>
        /// the custom value type.
        /// </summary>
        CustomValueType customValueType;

        /// <summary>
        /// Creates a new custom condition.
        /// </summary>
        public FormulaCondition() : this(CustomValueType.Empty, null)
        {
        }

        /// <summary>
        /// Creates a new custom condition with a specified formula or expression.
        /// </summary>
        /// <param name="customValueType"></param>
        /// <param name="formula">Formula string or expression.</param>
        internal FormulaCondition(CustomValueType customValueType, string formula) : base(null, formula)
        {
            this.customValueType = customValueType;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override object Clone()
        {
            FormulaCondition condition = base.Clone() as FormulaCondition;
            condition.customValueType = this.customValueType;
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
            if (this.customValueType == CustomValueType.Formula)
            {
                object val = base.GetExpected(evaluator, baseRow, baseColumn);
                if (this.IgnoreBlank && (val == null))
                {
                    return true;
                }
                bool? nullable = ConditionValueConverter.TryBool(val);
                if (!nullable.HasValue)
                {
                    return false;
                }
                return nullable.Value;
            }
            object objA = actualObj.GetValue();
            switch (this.customValueType)
            {
                case CustomValueType.Empty:
                    return ((objA == null) || object.Equals(objA, string.Empty));

                case CustomValueType.NonEmpty:
                    if (objA == null)
                    {
                        return false;
                    }
                    return !object.Equals(objA, string.Empty);

                case CustomValueType.Error:
                    return evaluator.IsCalcError(objA);

                case CustomValueType.NonError:
                    return !evaluator.IsCalcError(objA);
            }
            return false;
        }

        /// <summary>
        /// Creates a formula condition from the formula.
        /// </summary>
        /// <param name="formula">The formula.</param>
        /// <returns>The FormulaCondition object.</returns>
        public static FormulaCondition FromFormula(string formula)
        {
            return new FormulaCondition(CustomValueType.Formula, formula);
        }

        /// <summary>
        /// Creates a formula condition from the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The FormulaCondition object.</returns>
        public static FormulaCondition FromType(CustomValueType type)
        {
            return new FormulaCondition(type, null);
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected override void OnReadXml(XmlReader reader)
        {
            string str;
            base.OnReadXml(reader);
            if (((str = reader.Name) != null) && (str == "Formula1"))
            {
                this.customValueType = (CustomValueType) Serializer.DeserializeObj(typeof(CustomValueType), reader);
            }
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void OnWriteXml(XmlWriter writer)
        {
            base.OnWriteXml(writer);
            if (this.customValueType != CustomValueType.Empty)
            {
                Serializer.SerializeObj(this.customValueType, "Formula1", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected override void Reset()
        {
            base.Reset();
            this.customValueType = CustomValueType.Empty;
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public override System.Type ExpectedValueType
        {
            get { return  typeof(string); }
        }

        /// <summary>
        /// Gets or sets the type of the custom value.
        /// </summary>
        /// <value>The type of the custom value. The default value is null.</value>
        [DefaultValue((string) null)]
        public CustomValueType Type
        {
            get { return  this.customValueType; }
            set { this.customValueType = value; }
        }
    }
}

