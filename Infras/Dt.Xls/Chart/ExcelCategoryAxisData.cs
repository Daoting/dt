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
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Implement the interface of ICategoryAxisData
    /// </summary>
    public class ExcelCategoryAxisData : IExcelChartCategoryAxisData
    {
        private string _multiLevelStringReferenceFormula;
        private string _numberReferenceFormula;
        private NumericDataLiterals _numericLiterals;
        private StringLiteralData _stringLiterals;
        private string _stringReferenceFormula;
        private string numberReferenceFormat = "General";

        internal void ReadXml(XElement element)
        {
            foreach (XElement element2 in element.Elements())
            {
                if (element2.Name.LocalName == "numRef")
                {
                    this.NumberReferencesFormula = element2.GetChildElementValue("f");
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        this.NumberReferencesFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.NumberReferencesFormula, 0, 0);
                    }
                    foreach (XElement element3 in element2.Elements())
                    {
                        if (element3.Name.LocalName == "numCache")
                        {
                            this.numberReferenceFormat = element3.GetChildElementValue("formatCode");
                        }
                    }
                }
                else if (element2.Name.LocalName == "numLit")
                {
                    if (element2.HasElements)
                    {
                        NumericDataLiterals literals = new NumericDataLiterals();
                        literals.ReadXml(element2);
                        this.NumericLiterals = literals;
                    }
                }
                else if (element2.Name.LocalName == "strRef")
                {
                    this.StringReferencedFormula = element2.GetChildElementValue("f");
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        this.StringReferencedFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.StringReferencedFormula, 0, 0);
                    }
                }
                else if (element2.Name.LocalName == "strLit")
                {
                    if (element2.HasElements)
                    {
                        StringLiteralData data = new StringLiteralData();
                        data.ReadXml(element2);
                        this.StringLiterals = data;
                    }
                }
                else if (element2.Name.LocalName == "multiLvlStrRef")
                {
                    this.MultiLevelStringReferenceFormula = element2.GetChildElementValue("f");
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        this.MultiLevelStringReferenceFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.MultiLevelStringReferenceFormula, 0, 0);
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            this.WriteXml(writer, mFolder, chartFile, "cat");
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile, string startElement)
        {
            writer.WriteStartElement("c", startElement, null);
            if (!string.IsNullOrWhiteSpace(this.MultiLevelStringReferenceFormula))
            {
                writer.WriteStartElement("c", "multiLvlStrRef", null);
                string multiLevelStringReferenceFormula = this.MultiLevelStringReferenceFormula;
                if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                {
                    multiLevelStringReferenceFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(multiLevelStringReferenceFormula, 0, 0);
                }
                writer.WriteElementString("c", "f", null, multiLevelStringReferenceFormula);
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(this.NumberReferencesFormula))
            {
                writer.WriteStartElement("c", "numRef", null);
                string numberReferencesFormula = this.NumberReferencesFormula;
                if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                {
                    numberReferencesFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(numberReferencesFormula, 0, 0);
                }
                writer.WriteElementString("c", "f", null, numberReferencesFormula);
                using (writer.WriteElement("numCache", null, "c"))
                {
                    writer.WriteElementString("c", "formatCode", null, this.FormatCode);
                }
                writer.WriteEndElement();
            }
            else if (!string.IsNullOrWhiteSpace(this.StringReferencedFormula))
            {
                writer.WriteStartElement("c", "strRef", null);
                string stringReferencedFormula = this.StringReferencedFormula;
                if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                {
                    stringReferencedFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(stringReferencedFormula, 0, 0);
                }
                writer.WriteElementString("c", "f", null, stringReferencedFormula);
                writer.WriteEndElement();
            }
            else if (this.StringLiterals != null)
            {
                this.StringLiterals.WriteXml(writer, mFolder, chartFile);
            }
            else if (this.NumericLiterals != null)
            {
                this.NumericLiterals.WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Represents the number format code used for the NumberReferencesFormula 
        /// </summary>
        public string FormatCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.numberReferenceFormat))
                {
                    return "General";
                }
                return this.numberReferenceFormat;
            }
            set { this.numberReferenceFormat = value; }
        }

        /// <summary>
        /// Specifies a reference to data for the category axis
        /// </summary>
        public string MultiLevelStringReferenceFormula
        {
            get { return  this._multiLevelStringReferenceFormula; }
            set
            {
                this._stringReferenceFormula = null;
                this._numberReferenceFormula = null;
                this._stringLiterals = null;
                this._numericLiterals = null;
                this._multiLevelStringReferenceFormula = value;
            }
        }

        /// <summary>
        /// Specifies a set of numbers used for the parent element.
        /// </summary>
        public string NumberReferencesFormula
        {
            get { return  this._numberReferenceFormula; }
            set
            {
                this._stringReferenceFormula = null;
                this._numberReferenceFormula = value;
                this._stringLiterals = null;
                this._numericLiterals = null;
                this._multiLevelStringReferenceFormula = null;
            }
        }

        /// <summary>
        /// Specifies a reference to numeric data with a cache of the last valuse used.
        /// </summary>
        public NumericDataLiterals NumericLiterals
        {
            get { return  this._numericLiterals; }
            set
            {
                this._stringReferenceFormula = null;
                this._numberReferenceFormula = null;
                this._stringLiterals = null;
                this._numericLiterals = value;
                this._multiLevelStringReferenceFormula = null;
            }
        }

        /// <summary>
        /// Specifies a set of string s used for a chart.
        /// </summary>
        public StringLiteralData StringLiterals
        {
            get { return  this._stringLiterals; }
            set
            {
                this._stringReferenceFormula = null;
                this._numberReferenceFormula = null;
                this._stringLiterals = value;
                this._numericLiterals = null;
                this._multiLevelStringReferenceFormula = null;
            }
        }

        /// <summary>
        /// Specifies a reference to data for a single data label or title with a cache of the last values used.
        /// </summary>
        public string StringReferencedFormula
        {
            get { return  this._stringReferenceFormula; }
            set
            {
                this._stringReferenceFormula = value;
                this._numberReferenceFormula = null;
                this._stringLiterals = null;
                this._numericLiterals = null;
                this._multiLevelStringReferenceFormula = null;
            }
        }
    }
}

