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
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Windows.UI;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a condition base object. 
    /// </summary>
    public abstract class ConditionBase : ICloneable, IConditionalFormula, IXmlSerializable
    {
        int columnOffset;
        ValueObject expected;
        bool ignoreBlank = false;
        int rowOffset;

        /// <summary>
        /// Creates a new base condition with the specified expected content for the specified cell.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="formula">The expected formula.</param>
        protected ConditionBase(object expected, string formula)
        {
            if (string.IsNullOrEmpty(formula))
            {
                this.expected = ValueObject.FromValue(expected);
            }
            else
            {
                this.expected = ValueObject.FromFormula(formula);
            }
            this.rowOffset = 0;
            this.columnOffset = 0;
        }

        internal virtual void AdjustOffset(int rowOffset, int columnOffset, bool overrideOld = true)
        {
            this.rowOffset = overrideOld ? rowOffset : (this.rowOffset + rowOffset);
            this.columnOffset = overrideOld ? columnOffset : (this.columnOffset + columnOffset);
            if (this.expected != null)
            {
                this.expected.RowOffset = this.rowOffset;
                this.expected.ColumnOffset = this.columnOffset;
            }
        }

        /// <summary>
        /// Clears the expected value.
        /// </summary>
        protected void ClearExpected()
        {
            this.expected = null;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public virtual object Clone()
        {
            ConditionBase base2 = Activator.CreateInstance(base.GetType()) as ConditionBase;
            base2.expected = (this.expected == null) ? null : (this.expected.Clone() as ValueObject);
            base2.ignoreBlank = this.ignoreBlank;
            base2.rowOffset = this.rowOffset;
            base2.columnOffset = this.columnOffset;
            return base2;
        }

        /// <summary>
        /// Clones the ranges.
        /// </summary>
        /// <param name="ranges">The source ranges.</param>
        /// <returns>The new ranges.</returns>
        protected static ICellRange[] CloneRanges(ICellRange[] ranges)
        {
            List<ICellRange> list = new List<ICellRange>();
            foreach (ICellRange range in ranges)
            {
                list.Add(range.Clone() as ICellRange);
            }
            return list.ToArray();
        }

        /// <summary>
        /// Evaluates by the specified evaluator.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="actual">The actual value object.</param>
        /// <returns><c>true</c> if the result is successful, otherwise <c>false</c>.</returns>
        public abstract bool Evaluate(ICalcEvaluator evaluator, int baseRow, int baseColumn, IActualValue actual);
        /// <summary>
        /// Gets the expected value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>The expected value.</returns>
        public object GetExpected(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return this.GetExpected(evaluator, baseRow, baseColumn, true);
        }

        /// <summary>
        /// Gets the expected value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row.</param>
        /// <param name="baseColumn">The base column.</param>
        /// <param name="isArrayFormula">if set to <c>true</c> is an array formula.</param>
        /// <returns>The expected value.</returns>
        public object GetExpected(ICalcEvaluator evaluator, int baseRow, int baseColumn, bool isArrayFormula)
        {
            return this.expected.GetValue(evaluator, baseRow, baseColumn, isArrayFormula);
        }

        /// <summary>
        /// Gets the expected bool value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected bool value.</returns>
        protected bool? GetExpectedBoolean(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return ConditionValueConverter.TryBool(this.GetExpected(evaluator, baseRow, baseColumn));
        }

        /// <summary>
        /// Gets the expected color value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected color value.</returns>
        protected Color? GetExpectedColor(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return ConditionValueConverter.TryColor(this.GetExpected(evaluator, baseRow, baseColumn));
        }

        /// <summary>
        /// Gets the expected date and time string.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected date and time string.</returns>
        protected DateTime? GetExpectedDateTime(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return ConditionValueConverter.TryDateTime(this.GetExpected(evaluator, baseRow, baseColumn));
        }

        /// <summary>
        /// Gets the expected double value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected double value.</returns>
        protected double? GetExpectedDouble(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return ConditionValueConverter.TryDouble(this.GetExpected(evaluator, baseRow, baseColumn));
        }

        /// <summary>
        /// Gets the expected integer value.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected integer value.</returns>
        protected int? GetExpectedInt(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            return ConditionValueConverter.TryInt(this.GetExpected(evaluator, baseRow, baseColumn));
        }

        /// <summary>
        /// Gets the expected string.
        /// </summary>
        /// <param name="evaluator">The evaluator.</param>
        /// <param name="baseRow">The base row index.</param>
        /// <param name="baseColumn">The base column index.</param>
        /// <returns>Returns the expected string.</returns>
        protected string GetExpectedString(ICalcEvaluator evaluator, int baseRow, int baseColumn)
        {
            object obj2 = this.GetExpected(evaluator, baseRow, baseColumn);
            if (obj2 != null)
            {
                return obj2.ToString();
            }
            return null;
        }

        /// <summary>
        /// Called when reading XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        protected virtual void OnReadXml(XmlReader reader)
        {
            string name = reader.Name;
            if (name != null)
            {
                if (name != "IgnoreBlank")
                {
                    if (name != "Formula")
                    {
                        if (name == "Value")
                        {
                            this.expected = ValueObject.FromValue(Serializer.DeserializeObj(null, reader));
                            return;
                        }
                        if (name == "OffsetY")
                        {
                            this.rowOffset = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                            return;
                        }
                        if (name == "OffsetX")
                        {
                            this.columnOffset = (int) ((int) Serializer.DeserializeObj(typeof(int), reader));
                        }
                        return;
                    }
                }
                else
                {
                    this.ignoreBlank = (bool) ((bool) Serializer.DeserializeObj(typeof(bool), reader));
                    return;
                }
                this.expected = ValueObject.FromFormula((string) (Serializer.DeserializeObj(typeof(string), reader) as string));
            }
        }

        /// <summary>
        /// Called after the end of reading XML.
        /// </summary>
        protected virtual void OnReadXmlEnd()
        {
            this.AdjustOffset(this.rowOffset, this.columnOffset, true);
        }

        /// <summary>
        /// Called when writing XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected virtual void OnWriteXml(XmlWriter writer)
        {
            if (this.ignoreBlank)
            {
                Serializer.SerializeObj((bool) this.ignoreBlank, "IgnoreBlank", writer);
            }
            if (this.expected != null)
            {
                if (!string.IsNullOrEmpty(this.ExpectedFormula))
                {
                    Serializer.SerializeObj(this.ExpectedFormula, "Formula", writer);
                }
                else if (this.ExpectedValue != null)
                {
                    Serializer.WriteStartObj("Value", writer);
                    Serializer.WriteTypeAttr(this.ExpectedValue, writer);
                    Serializer.SerializeObj(this.ExpectedValue, null, writer);
                    Serializer.WriteEndObj(writer);
                }
            }
            if (this.rowOffset != 0)
            {
                Serializer.SerializeObj((int) this.rowOffset, "OffsetY", writer);
            }
            if (this.columnOffset != 0)
            {
                Serializer.SerializeObj((int) this.columnOffset, "OffsetX", writer);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        protected virtual void Reset()
        {
            this.expected = null;
            this.ignoreBlank = false;
        }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface, you should return null (Nothing in Visual Basic) from this method, and instead, if specifying a custom schema is required, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute" /> to the class.
        /// </summary>
        /// <returns>A <see cref="T:System.Xml.Schema.XmlSchema" /> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)" /> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)" /> method.</returns>
        /// <remarks></remarks>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader" /> stream from which the object is deserialized.</param>
        /// <remarks></remarks>
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
            this.OnReadXmlEnd();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter" /> stream to which the object is serialized.</param>
        /// <remarks></remarks>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.OnWriteXml(writer);
        }

        /// <summary>
        /// Gets or sets the expected expression.
        /// </summary>
        /// <value>The expected expression.</value>
        public object ExpectedExpression
        {
            get
            {
                if (this.expected != null)
                {
                    return this.expected.Expression;
                }
                return null;
            }
            set { this.expected = ValueObject.FromExpression(value); }
        }

        /// <summary>
        /// Gets or sets the expected formula.
        /// </summary>
        /// <value>The expected formula.</value>
        public string ExpectedFormula
        {
            get
            {
                if (this.expected != null)
                {
                    return this.expected.Formula;
                }
                return null;
            }
            set { this.expected = ValueObject.FromFormula(value); }
        }

        /// <summary>
        /// Gets or sets the expected value.
        /// </summary>
        /// <value>The expected value.</value>
        public object ExpectedValue
        {
            get
            {
                if (this.expected != null)
                {
                    return this.expected.Value;
                }
                return null;
            }
            set { this.expected = ValueObject.FromValue(value); }
        }

        internal ValueObject ExpectedValueObject
        {
            get { return  this.expected; }
        }

        /// <summary>
        /// Gets the expected type of the value.
        /// </summary>
        public virtual Type ExpectedValueType
        {
            get { return  null; }
        }

        string IConditionalFormula.Formula
        {
            get { return  this.ExpectedFormula; }
            set { this.ExpectedFormula = value; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to ignore the null value.
        /// </summary>
        /// <value>
        /// <c>true</c> if this condition ignores the null value; otherwise, <c>false</c>.
        /// The default value is <c>false</c>, which means it does not ignore null values.
        /// </value>
        [DefaultValue(false)]
        public virtual bool IgnoreBlank
        {
            get { return  this.ignoreBlank; }
            set { this.ignoreBlank = value; }
        }
    }
}

