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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies text for a series name.
    /// </summary>
    public class ExcelSeriesName : IExcelSeriesName
    {
        private string referenceFormula;
        private string textValue;

        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "strRef")
                {
                    this.ReferenceFormula = element.GetChildElementValue("f");
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        this.ReferenceFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.ReferenceFormula, 0, 0);
                    }
                    this.IsReferenceFormula = true;
                }
                else if (element.Name.LocalName == "v")
                {
                    this.TextValue = element.Value;
                    this.IsReferenceFormula = false;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "tx", null);
            if (this.IsReferenceFormula && !string.IsNullOrWhiteSpace(this.ReferenceFormula))
            {
                using (writer.WriteElement("strRef", null, "c"))
                {
                    string referenceFormula = this.ReferenceFormula;
                    if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                    {
                        referenceFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(referenceFormula, 0, 0);
                    }
                    writer.WriteElementString("c", "f", null, referenceFormula);
                    goto Label_0092;
                }
            }
            if (!string.IsNullOrWhiteSpace(this.TextValue))
            {
                writer.WriteElementString("c", "v", null, this.TextValue);
            }
        Label_0092:
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies whethere the text is reference formula or text value.
        /// </summary>
        public bool IsReferenceFormula { get; set; }

        /// <summary>
        /// Specifies a reference to data for a single datable or title.
        /// </summary>
        public string ReferenceFormula
        {
            get { return  this.referenceFormula; }
            set
            {
                this.referenceFormula = value;
                this.IsReferenceFormula = true;
            }
        }

        /// <summary>
        /// Specifies a text value for a category axis lable or a series name.
        /// </summary>
        public string TextValue
        {
            get { return  this.textValue; }
            set
            {
                this.textValue = value;
                this.IsReferenceFormula = false;
            }
        }
    }
}

