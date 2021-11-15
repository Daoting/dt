#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using Dt.Xls.OOXml;
using Dt.Xls.Utils;
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Implement the interface IErrorBars
    /// </summary>
    public class ExcelErrorBars : IExcelErrorBars
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelErrorBars" /> class.
        /// </summary>
        public ExcelErrorBars()
        {
            this.ErrorBarValueType = ExcelErrorBarValueType.FixedValue;
            this.ErrorBarDireciton = ExcelErrorBarDireciton.None;
            this.ErrorBarType = ExcelErrorBarType.Both;
            this.NoEndCap = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "errDir")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "none"))
                    {
                        case "x":
                            this.ErrorBarDireciton = ExcelErrorBarDireciton.X;
                            break;

                        case "y":
                            this.ErrorBarDireciton = ExcelErrorBarDireciton.Y;
                            break;
                    }
                }
                else if (element.Name.LocalName == "errBarType")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "both"))
                    {
                        case "both":
                            this.ErrorBarType = ExcelErrorBarType.Both;
                            break;

                        case "minus":
                            this.ErrorBarType = ExcelErrorBarType.Minus;
                            break;

                        case "plus":
                            this.ErrorBarType = ExcelErrorBarType.Plus;
                            break;
                    }
                }
                else if (element.Name.LocalName == "errValType")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "fixedVal"))
                    {
                        case "cust":
                            this.ErrorBarValueType = ExcelErrorBarValueType.CustomErrorBar;
                            break;

                        case "fixedVal":
                            this.ErrorBarValueType = ExcelErrorBarValueType.FixedValue;
                            break;

                        case "percentage":
                            this.ErrorBarValueType = ExcelErrorBarValueType.Percentage;
                            break;

                        case "stdDev":
                            this.ErrorBarValueType = ExcelErrorBarValueType.StandardDeviation;
                            break;

                        case "stdErr":
                            this.ErrorBarValueType = ExcelErrorBarValueType.StandardError;
                            break;
                    }
                }
                else if (element.Name.LocalName == "noEndCap")
                {
                    this.NoEndCap = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.ErrorBarsFormat = format;
                }
                else if (element.Name.LocalName == "plus")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "numLit")
                        {
                            this.Plus = new NumericDataLiterals();
                            this.Plus.ReadXml(element2);
                        }
                        else if (element2.Name.LocalName == "numRef")
                        {
                            this.PlusReferenceFormula = element2.GetChildElementValue("f");
                            if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                            {
                                this.PlusReferenceFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.PlusReferenceFormula, 0, 0);
                            }
                        }
                    }
                }
                else if (element.Name.LocalName == "minus")
                {
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName == "numLit")
                        {
                            this.Minus = new NumericDataLiterals();
                            this.Minus.ReadXml(element3);
                        }
                        else if (element3.Name.LocalName == "numRef")
                        {
                            this.MinusReferenceFormula = element3.GetChildElementValue("f");
                            if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                            {
                                this.MinusReferenceFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.MinusReferenceFormula, 0, 0);
                            }
                        }
                    }
                }
                else if (element.Name.LocalName == "val")
                {
                    this.Value = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "errBars", null);
            if (this.ErrorBarDireciton != ExcelErrorBarDireciton.None)
            {
                writer.WriteStartElement("c", "errDir", null);
                writer.WriteAttributeString("val", (this.ErrorBarDireciton == ExcelErrorBarDireciton.X) ? "x" : "y");
                writer.WriteEndElement();
            }
            string str = "both";
            switch (this.ErrorBarType)
            {
                case ExcelErrorBarType.Both:
                    str = "both";
                    break;

                case ExcelErrorBarType.Minus:
                    str = "minus";
                    break;

                case ExcelErrorBarType.Plus:
                    str = "plus";
                    break;
            }
            writer.WriteStartElement("c", "errBarType", null);
            writer.WriteAttributeString("val", str);
            writer.WriteEndElement();
            string str2 = "fixedVal";
            switch (this.ErrorBarValueType)
            {
                case ExcelErrorBarValueType.FixedValue:
                    str2 = "fixedVal";
                    break;

                case ExcelErrorBarValueType.CustomErrorBar:
                    str2 = "cust";
                    break;

                case ExcelErrorBarValueType.Percentage:
                    str2 = "percentage";
                    break;

                case ExcelErrorBarValueType.StandardDeviation:
                    str2 = "stdDev";
                    break;

                case ExcelErrorBarValueType.StandardError:
                    str2 = "stdErr";
                    break;
            }
            writer.WriteStartElement("c", "errValType", null);
            writer.WriteAttributeString("val", str2);
            writer.WriteEndElement();
            if (!this.NoEndCap)
            {
                writer.WriteStartElement("c", "noEndCap", null);
                writer.WriteAttributeString("val", "0");
                writer.WriteEndElement();
            }
            if (this.ErrorBarValueType == ExcelErrorBarValueType.CustomErrorBar)
            {
                if (this.Plus != null)
                {
                    writer.WriteStartElement("c", "plus", null);
                    this.Plus.WriteXml(writer, mFolder, chartFile);
                    writer.WriteEndElement();
                }
                else if (!string.IsNullOrWhiteSpace(this.PlusReferenceFormula))
                {
                    writer.WriteStartElement("c", "plus", null);
                    writer.WriteStartElement("c", "numRef", null);
                    string plusReferenceFormula = this.PlusReferenceFormula;
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        plusReferenceFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(plusReferenceFormula, 0, 0);
                    }
                    writer.WriteElementString("c", "f", null, plusReferenceFormula);
                    using (writer.WriteElement("numCache", null, "c"))
                    {
                        writer.WriteElementString("c", "formatCode", null, "General");
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                if (this.Minus != null)
                {
                    writer.WriteStartElement("c", "minus", null);
                    this.Minus.WriteXml(writer, mFolder, chartFile);
                    writer.WriteEndElement();
                }
                else if (!string.IsNullOrWhiteSpace(this.MinusReferenceFormula))
                {
                    writer.WriteStartElement("c", "minus", null);
                    writer.WriteStartElement("c", "numRef", null);
                    string minusReferenceFormula = this.MinusReferenceFormula;
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        minusReferenceFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(minusReferenceFormula, 0, 0);
                    }
                    writer.WriteElementString("c", "f", null, minusReferenceFormula);
                    using (writer.WriteElement("numCache", null, "c"))
                    {
                        writer.WriteElementString("c", "formatCode", null, "General");
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            if (this.Value != 0.0)
            {
                writer.WriteStartElement("c", "val", null);
                writer.WriteAttributeString("val", ((double) this.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteEndElement();
            }
            if (this.ErrorBarsFormat != null)
            {
                (this.ErrorBarsFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the error bar direciton.
        /// </summary>
        /// <value>
        /// The error bar direciton.
        /// </value>
        public ExcelErrorBarDireciton ErrorBarDireciton { get; set; }

        /// <summary>
        /// Represents the error bar format.
        /// </summary>
        public IExcelChartFormat ErrorBarsFormat { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar.
        /// </summary>
        /// <value>
        /// The type of the error bar.
        /// </value>
        public ExcelErrorBarType ErrorBarType { get; set; }

        /// <summary>
        /// Gets or sets the type of the error bar value.
        /// </summary>
        /// <value>
        /// The type of the error bar value.
        /// </value>
        public ExcelErrorBarValueType ErrorBarValueType { get; set; }

        /// <summary>
        /// Specifies the error bar value in the negtive direction.
        /// </summary>
        public NumericDataLiterals Minus { get; set; }

        /// <summary>
        /// Specifies the error bar formula in the negtive direction.
        /// </summary>
        public string MinusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies an end cap is not drawn on the error bars.
        /// </summary>
        public bool NoEndCap { get; set; }

        /// <summary>
        /// Specifies the error bar value in the positive direction.
        /// </summary>
        public NumericDataLiterals Plus { get; set; }

        /// <summary>
        /// Specifies the error bar formula in the positive direction.
        /// </summary>
        public string PlusReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a value which is used with the errorBar element to determine the length of the error bars.
        /// </summary>
        public double Value { get; set; }
    }
}

