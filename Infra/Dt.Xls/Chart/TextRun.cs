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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the presence of a run of text within the containing text doay.
    /// </summary>
    public class TextRun
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "rPr")
                {
                    int num = element.GetAttributeValueOrDefaultOfInt32Type("sz", -1);
                    if ((num >= 100) && (num <= 0x61a80))
                    {
                        this.FontSize = new double?(((double) num) / 100.0);
                    }
                    foreach (XAttribute attribute in element.Attributes())
                    {
                        if (attribute.Name.LocalName == "b")
                        {
                            this.Bold = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("b", true));
                        }
                        if (attribute.Name.LocalName == "i")
                        {
                            this.Italics = new bool?(element.GetAttributeValueOrDefaultOfBooleanType("i", true));
                        }
                    }
                    using (IEnumerator<XElement> enumerator3 = element.Elements().GetEnumerator())
                    {
                        while (enumerator3.MoveNext())
                        {
                            IFillFormat format = enumerator3.Current.ReadFillFormat(mFolder, xFile);
                            if (format != null)
                            {
                                this.FillFormat = format;
                            }
                        }
                        continue;
                    }
                }
                if (element.Name.LocalName == "t")
                {
                    this.Text = element.Value;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("r", null, "a"))
            {
                if ((this.FontSize.HasValue || this.Bold.HasValue) || (this.Italics.HasValue || (this.FillFormat != null)))
                {
                    using (writer.WriteElement("rPr", null, "a"))
                    {
                        if (this.FontSize.HasValue)
                        {
                            double num = this.FontSize.Value * 100.0;
                            writer.WriteAttributeString("sz", ((double) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (this.Bold.HasValue)
                        {
                            writer.WriteAttributeString("b", this.Bold.Value ? "1" : "0");
                        }
                        if (this.Italics.HasValue)
                        {
                            writer.WriteAttributeString("i", this.Italics.Value ? "1" : "0");
                        }
                        if (this.FillFormat != null)
                        {
                            this.FillFormat.WriteXml(writer, mFolder, chartFile);
                        }
                    }
                }
                writer.WriteElementString("a", "t", null, this.Text);
            }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as bold text.
        /// </summary>
        public bool? Bold { get; set; }

        /// <summary>
        /// Specifies the fill format used for this text run.
        /// </summary>
        public IFillFormat FillFormat { get; set; }

        /// <summary>
        /// Specifies the size of text within a text run in Point
        /// </summary>
        public double? FontSize { get; set; }

        /// <summary>
        /// Specifies whether a run of text is formatted as italic text.
        /// </summary>
        public bool? Italics { get; set; }

        /// <summary>
        /// The content of the text run.
        /// </summary>
        public string Text { get; set; }
    }
}

