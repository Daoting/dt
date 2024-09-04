#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
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
    /// Specifies the lable for the trendline
    /// </summary>
    public class ExcelTrendLineLabel
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "layout")
                {
                    Dt.Xls.Chart.Layout layout = new Dt.Xls.Chart.Layout();
                    layout.ReadXml(element, mFolder, xFile);
                    this.Layout = layout;
                }
                else if (element.Name.LocalName == "numFmt")
                {
                    Dt.Xls.Chart.NumberFormat format = ChartCommonSimpleNodeHelper.ReadNumberFormatNode(element);
                    this.NumberFormat = format.NumberFormatCode;
                    this.NumberFormatLinked = format.LinkToSource;
                }
                else if (element.Name.LocalName == "tx")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "rich")
                        {
                            Dt.Xls.Chart.RichText text = new Dt.Xls.Chart.RichText();
                            text.ReadXml(element2, mFolder, xFile);
                            this.RichText = text;
                        }
                        else if (element2.Name.LocalName == "strRef")
                        {
                            this.TextStringReference = element2.GetChildElementValue("f");
                        }
                    }
                }
            }
        }

        private void WriteElement(XmlWriter writer, string prefix, string name, string attribleName, string attribleValue)
        {
            writer.WriteStartElement(prefix, name, null);
            writer.WriteAttributeString(attribleName, attribleValue);
            writer.WriteEndElement();
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "trendlineLbl", null);
            if (this.Layout != null)
            {
                this.Layout.WriteXml(writer, mFolder, chartFile);
            }
            if (!string.IsNullOrWhiteSpace(this.NumberFormat) || !this.NumberFormatLinked)
            {
                Dt.Xls.Chart.NumberFormat numberFormat = new Dt.Xls.Chart.NumberFormat {
                    LinkToSource = this.NumberFormatLinked,
                    NumberFormatCode = this.NumberFormat
                };
                ChartCommonSimpleNodeHelper.WriteNummberFormatNode(writer, numberFormat);
            }
            if (this.RichText != null)
            {
                using (writer.WriteElement("tx", null, "c"))
                {
                    this.RichText.WriteXml(writer, mFolder, chartFile);
                    goto Label_00F5;
                }
            }
            if (!string.IsNullOrWhiteSpace(this.TextStringReference))
            {
                using (writer.WriteElement("tx", null, "c"))
                {
                    using (writer.WriteElement("strRef", null, "c"))
                    {
                        writer.WriteElementString("c", "f", null, this.TextStringReference);
                    }
                }
            }
        Label_00F5:
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the layout.
        /// </summary>
        /// <value>
        /// The layout.
        /// </value>
        public Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies the number format code.
        /// </summary>
        public string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whether Linked to source
        /// </summary>
        public bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Gets or sets the rich text.
        /// </summary>
        /// <value>
        /// The rich text.
        /// </value>
        public Dt.Xls.Chart.RichText RichText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TextStringReference { get; set; }
    }
}

