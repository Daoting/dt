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
    /// Specifies the data values which shall be used to define the location of data markers on a charts.
    /// </summary>
    public class ExcelSeriesValue : IExcelSeriesValue
    {
        private NumericDataLiterals _numericLiterals;
        private string _referenceFormula;
        private string numberReferenceFormat = "General";

        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "numRef")
                {
                    string childElementValue = element.GetChildElementValue("f");
                    if (!string.IsNullOrWhiteSpace(childElementValue))
                    {
                        this.ReferenceFormula = childElementValue;
                        if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                        {
                            this.ReferenceFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.ReferenceFormula, 0, 0);
                        }
                    }
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "numCache")
                        {
                            this.numberReferenceFormat = element2.GetChildElementValue("formatCode");
                        }
                    }
                }
                else if (element.Name.LocalName == "numLit")
                {
                    NumericDataLiterals literals = new NumericDataLiterals();
                    literals.ReadXml(element);
                    this.NumericLiterals = literals;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            this.WriteXml(writer, mFolder, chartFile, "val");
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile, string startElement)
        {
            writer.WriteStartElement("c", startElement, null);
            if (!string.IsNullOrWhiteSpace(this.ReferenceFormula))
            {
                writer.WriteStartElement("c", "numRef", null);
                string referenceFormula = this.ReferenceFormula;
                if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                {
                    referenceFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(referenceFormula, 0, 0);
                }
                writer.WriteElementString("c", "f", null, referenceFormula);
                using (writer.WriteElement("numCache", null, "c"))
                {
                    writer.WriteElementString("c", "formatCode", null, this.FormatCode);
                }
                writer.WriteEndElement();
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
        /// Specifies a set of numbers.
        /// </summary>
        public NumericDataLiterals NumericLiterals
        {
            get { return  this._numericLiterals; }
            set
            {
                this._numericLiterals = value;
                this._referenceFormula = null;
            }
        }

        /// <summary>
        /// Specifies a reference formula to numeric data.
        /// </summary>
        public string ReferenceFormula
        {
            get { return  this._referenceFormula; }
            set
            {
                this._referenceFormula = value;
                this._numericLiterals = null;
            }
        }
    }
}

