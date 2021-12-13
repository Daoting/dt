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
    /// Defines a title for chart.
    /// </summary>
    public class ExcelChartTitle : IExcelChartTitle
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "overlay")
                {
                    this.Overlay = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "layout")
                {
                    Dt.Xls.Chart.Layout layout = new Dt.Xls.Chart.Layout();
                    layout.ReadXml(element, mFolder, xFile);
                    this.Layout = layout;
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.ShapeFormat = format;
                }
                else if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format2 = new ExcelTextFormat();
                    format2.ReadXml(element, mFolder, xFile);
                    this.TextForamt = format2;
                }
                else if (element.Name.LocalName == "tx")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "rich")
                        {
                            RichText text = new RichText();
                            text.ReadXml(element2, mFolder, xFile);
                            this.RichTextTitle = text;
                        }
                        else if (element2.Name.LocalName == "strRef")
                        {
                            this.TitleFormula = element2.GetChildElementValue("f");
                            if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                            {
                                this.TitleFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.TitleFormula, 0, 0);
                            }
                        }
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "title", null);
            if (this.RichTextTitle != null)
            {
                using (writer.WriteElement("tx", null, "c"))
                {
                    this.RichTextTitle.WriteXml(writer, mFolder, chartFile);
                    goto Label_00B8;
                }
            }
            if (!string.IsNullOrWhiteSpace(this.TitleFormula))
            {
                using (writer.WriteElement("tx", null, "c"))
                {
                    using (writer.WriteElement("strRef", null, "c"))
                    {
                        string titleFormula = this.TitleFormula;
                        if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                        {
                            titleFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(titleFormula, 0, 0);
                        }
                        writer.WriteElementString("c", "f", null, titleFormula);
                    }
                }
            }
        Label_00B8:
            if (this.Layout != null)
            {
                this.Layout.WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteLeafElementWithAttribute("overlay", null, "c", "val", this.Overlay ? "1" : "0");
            if (this.ShapeFormat != null)
            {
                (this.ShapeFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.TextForamt != null)
            {
                (this.TextForamt as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies a title.
        /// </summary>
        public Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies that other chart elements shall be allowed to overlap this chart elements
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool Overlay { get; set; }

        /// <summary>
        /// Specifies the text of title in RichText mode
        /// </summary>
        public RichText RichTextTitle { get; set; }

        /// <summary>
        /// Specifies the chart title formatting.
        /// </summary>
        public IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the text of the chart title formatting
        /// </summary>
        public IExcelTextFormat TextForamt { get; set; }

        /// <summary>
        /// Specifies text to use on a chart.
        /// </summary>
        public string TitleFormula { get; set; }
    }
}

