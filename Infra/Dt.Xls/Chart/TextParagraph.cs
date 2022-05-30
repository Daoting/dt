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
    /// Specifies the presence of a paragraph of text within the containing text body.
    /// </summary>
    public class TextParagraph
    {
        private List<TextRun> _textRuns = new List<TextRun>();

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "pPr")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "defRPr")
                        {
                            int num = element2.GetAttributeValueOrDefaultOfInt32Type("sz", 0);
                            if ((num >= 100) && (num <= 0x61a80))
                            {
                                this.FontSize = new double?(((double) num) / 100.0);
                            }
                            foreach (XAttribute attribute in element2.Attributes())
                            {
                                if (attribute.Name.LocalName == "b")
                                {
                                    this.Bold = new bool?(element2.GetAttributeValueOrDefaultOfBooleanType("b", false));
                                }
                                if (attribute.Name.LocalName == "i")
                                {
                                    this.Italics = new bool?(element2.GetAttributeValueOrDefaultOfBooleanType("i", false));
                                }
                            }
                            foreach (XElement element3 in element2.Elements())
                            {
                                IFillFormat format = element3.ReadFillFormat(mFolder, xFile);
                                if (format != null)
                                {
                                    this.FillFormat = format;
                                }
                                if (element3.Name.LocalName == "latin")
                                {
                                    this.LatinFontFamily = element3.GetAttributeValueOrDefaultOfStringType("typeface", "Microsoft YaHei");
                                }
                                else if (element3.Name.LocalName == "ea")
                                {
                                    this.EastAsianFontFamily = element3.GetAttributeValueOrDefaultOfStringType("typeface", "Microsoft YaHei");
                                }
                                else if (element3.Name.LocalName == "cs")
                                {
                                    this.ComplexFontFamily = element3.GetAttributeValueOrDefaultOfStringType("typeface", "Microsoft YaHei");
                                }
                                else if (element3.Name.LocalName == "sym")
                                {
                                    this.SymbolFontFamily = element3.GetAttributeValueOrDefaultOfStringType("typeface", "Microsoft YaHei");
                                }
                            }
                        }
                    }
                }
                else if (element.Name.LocalName == "r")
                {
                    TextRun run = new TextRun();
                    run.ReadXml(element, mFolder, xFile);
                    this.TextRuns.Add(run);
                }
            }
        }

        private void WriteFont(XmlWriter writer, string node, string fontfamily)
        {
            if ((!string.IsNullOrWhiteSpace(node) && !string.IsNullOrWhiteSpace(fontfamily)) && (fontfamily.ToUpperInvariant() != "Microsoft YaHei"))
            {
                using (writer.WriteElement(node, null, "a"))
                {
                    writer.WriteAttributeString("typeface", fontfamily);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("p", null, "a"))
            {
                using (writer.WriteElement("pPr", null, "a"))
                {
                    using (writer.WriteElement("defRPr", null, "a"))
                    {
                        if (this.FontSize.HasValue && ((this.FontSize.Value - 1.0) > 0.0))
                        {
                            int num = (int) Math.Ceiling((double) (this.FontSize.Value * 100.0));
                            writer.WriteAttributeString("sz", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (this.Bold.HasValue)
                        {
                            writer.WriteAttributeString("b", this.Bold.Value ? "1" : "0");
                        }
                        if (this.Italics.HasValue)
                        {
                            writer.WriteAttributeString("i", this.Italics.Value ? "1" : "0");
                        }
                        else
                        {
                            writer.WriteAttributeString("i", "0");
                        }
                        if (this.FillFormat != null)
                        {
                            this.FillFormat.WriteXml(writer, mFolder, chartFile);
                        }
                        this.WriteFont(writer, "latin", this.LatinFontFamily);
                        this.WriteFont(writer, "ea", this.EastAsianFontFamily);
                        this.WriteFont(writer, "cs", this.ComplexFontFamily);
                        this.WriteFont(writer, "sym", this.SymbolFontFamily);
                    }
                }
                using (List<TextRun>.Enumerator enumerator = this.TextRuns.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies whether a run of text is formatted as bold text.
        /// </summary>
        public bool? Bold { get; set; }

        /// <summary>
        /// Specifies the complex script font 
        /// </summary>
        public string ComplexFontFamily { get; set; }

        /// <summary>
        /// Sepcifies the East Asian Font
        /// </summary>
        public string EastAsianFontFamily { get; set; }

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
        /// Specifies the Latin font 
        /// </summary>
        public string LatinFontFamily { get; set; }

        /// <summary>
        /// Specifies the symbol font
        /// </summary>
        public string SymbolFontFamily { get; set; }

        /// <summary>
        /// List of TextRuns defined in this paragraph.
        /// </summary>
        public List<TextRun> TextRuns
        {
            get { return  this._textRuns; }
        }
    }
}

