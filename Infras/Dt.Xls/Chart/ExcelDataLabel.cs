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
    /// Represents a single data label of a data point or trendline.
    /// </summary>
    public class ExcelDataLabel : IExcelDataLabel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelDataLabel" /> class.
        /// </summary>
        public ExcelDataLabel()
        {
            this.Delete = true;
            this.NumberFormatLinked = true;
            this.Position = DataLabelPosition.BestFit;
            this.ShowBubbleSize = true;
            this.ShowCategoryName = true;
            this.ShowLegendKey = true;
            this.ShowPercentage = true;
            this.ShowSeriesName = true;
            this.ShowValue = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "idx")
                {
                    this.Index = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "layout")
                {
                    Dt.Xls.Chart.Layout layout = new Dt.Xls.Chart.Layout();
                    layout.ReadXml(element, mFolder, xFile);
                    this.Layout = layout;
                }
                else if (element.Name.LocalName == "delete")
                {
                    this.Delete = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "numFmt")
                {
                    Dt.Xls.Chart.NumberFormat format = ChartCommonSimpleNodeHelper.ReadNumberFormatNode(element);
                    this.NumberFormat = format.NumberFormatCode;
                    this.NumberFormatLinked = format.LinkToSource;
                }
                else if (element.Name.LocalName == "dLblPos")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "bestFit"))
                    {
                        case "b":
                            this.Position = DataLabelPosition.Bottom;
                            break;

                        case "bestFit":
                            this.Position = DataLabelPosition.BestFit;
                            break;

                        case "ctr":
                            this.Position = DataLabelPosition.Center;
                            break;

                        case "inBase":
                            this.Position = DataLabelPosition.InsideBase;
                            break;

                        case "inEnd":
                            this.Position = DataLabelPosition.InsideEnd;
                            break;

                        case "l":
                            this.Position = DataLabelPosition.Left;
                            break;

                        case "outEnd":
                            this.Position = DataLabelPosition.OutsideEnd;
                            break;

                        case "r":
                            this.Position = DataLabelPosition.Right;
                            break;

                        case "t":
                            this.Position = DataLabelPosition.Top;
                            break;
                    }
                }
                else if (element.Name.LocalName == "separator")
                {
                    this.Separator = element.Value;
                }
                else if (element.Name.LocalName == "showLegendKey")
                {
                    this.ShowLegendKey = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showVal")
                {
                    this.ShowValue = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showCatName")
                {
                    this.ShowCategoryName = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showSerName")
                {
                    this.ShowSeriesName = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showPercent")
                {
                    this.ShowPercentage = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "showBubbleSize")
                {
                    this.ShowBubbleSize = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
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
                            this.TextFormula = element2.GetChildElementValue("f");
                            if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                            {
                                this.TextFormula = ParsingContext.ConvertA1FormulaToR1C1Formula(this.TextFormula, 0, 0);
                            }
                        }
                    }
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format2 = new ExcelChartFormat();
                    format2.ReadXml(element, mFolder, xFile);
                    this.ShapeFormat = format2;
                }
                else if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format3 = new ExcelTextFormat();
                    format3.ReadXml(element, mFolder, xFile);
                    this.TextFormat = format3;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "dLbl", null);
            writer.WriteLeafElementWithAttribute("idx", null, "c", "val", ((int) this.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
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
            if (this.ShapeFormat != null)
            {
                (this.ShapeFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.TextFormat != null)
            {
                (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.Position != DataLabelPosition.BestFit)
            {
                string str = "bestFit";
                switch (this.Position)
                {
                    case DataLabelPosition.Bottom:
                        str = "b";
                        break;

                    case DataLabelPosition.BestFit:
                        str = "bestFit";
                        break;

                    case DataLabelPosition.Center:
                        str = "ctr";
                        break;

                    case DataLabelPosition.InsideBase:
                        str = "inBase";
                        break;

                    case DataLabelPosition.InsideEnd:
                        str = "inEnd";
                        break;

                    case DataLabelPosition.Left:
                        str = "l";
                        break;

                    case DataLabelPosition.OutsideEnd:
                        str = "outEnd";
                        break;

                    case DataLabelPosition.Right:
                        str = "r";
                        break;

                    case DataLabelPosition.Top:
                        str = "t";
                        break;
                }
                writer.WriteLeafElementWithAttribute("dLblPos", null, "c", "val", str);
            }
            if (!this.Delete)
            {
                writer.WriteLeafElementWithAttribute("delete", null, "c", "val", "0");
            }
            writer.WriteLeafElementWithAttribute("showLegendKey", null, "c", "val", this.ShowLegendKey ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showVal", null, "c", "val", this.ShowValue ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showCatName", null, "c", "val", this.ShowCategoryName ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showSerName", null, "c", "val", this.ShowSeriesName ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showPercent", null, "c", "val", this.ShowPercentage ? "1" : "0");
            writer.WriteLeafElementWithAttribute("showBubbleSize", null, "c", "val", this.ShowBubbleSize ? "1" : "0");
            if ((this.Separator != CultureInfo.CurrentCulture.TextInfo.ListSeparator) && !string.IsNullOrEmpty(this.Separator))
            {
                writer.WriteElementString("c", "separator", null, this.Separator);
            }
            if (this.RichText != null)
            {
                using (writer.WriteElement("tx", null, "c"))
                {
                    this.RichText.WriteXml(writer, mFolder, chartFile);
                    goto Label_035F;
                }
            }
            if (!string.IsNullOrWhiteSpace(this.TextFormula))
            {
                writer.WriteStartElement("c", "tx", null);
                writer.WriteStartElement("c", "strRef", null);
                string textFormula = this.TextFormula;
                if (ParsingContext.ReferenceStyle == ExcelReferenceStyle.R1C1)
                {
                    textFormula = ParsingContext.ConvertR1C1FormulaToA1Formula(textFormula, 0, 0);
                }
                writer.WriteElementString("c", "f", null, textFormula);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        Label_035F:
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies that the chart element specifies by its containing element shall be deleted from the chart.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Specifies the index of the containing element.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Specifies how the chart element is placed on the chart
        /// </summary>
        public Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies the number format for the data label.
        /// </summary>
        public string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whethere the data label use the same number formats as the cells that contain the data for the associated data point.
        /// </summary>
        public bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Specifies the text orientation of the data lable.
        /// </summary>
        /// <remarks>
        /// The value should be between -90 and 90.
        /// </remarks>
        public int Orientation { get; set; }

        /// <summary>
        /// Specifies the position of the data label.
        /// </summary>
        public DataLabelPosition Position { get; set; }

        /// <summary>
        /// Specifies the text of the data label in the rich text format
        /// </summary>
        public Dt.Xls.Chart.RichText RichText { get; set; }

        /// <summary>
        /// Specifies text that shall be used to separate the parts of the data label, the default is comma.
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Specifies the formatting options for the data label.
        /// </summary>
        public IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies whether the data label show the bubble size.
        /// </summary>
        public bool ShowBubbleSize { get; set; }

        /// <summary>
        /// Specifies whether the data label show the category name.
        /// </summary>
        public bool ShowCategoryName { get; set; }

        /// <summary>
        /// Specifies whether the data label show the legend key.
        /// </summary>
        public bool ShowLegendKey { get; set; }

        /// <summary>
        /// Specifies whether the data label show the percentage.
        /// </summary>
        public bool ShowPercentage { get; set; }

        /// <summary>
        /// Specifies whether the data label show the series name.
        /// </summary>
        public bool ShowSeriesName { get; set; }

        /// <summary>
        /// Specifies whether the data label show the value.
        /// </summary>
        public bool ShowValue { get; set; }

        /// <summary>
        /// Specifies the text formatting options for the data label.
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }

        /// <summary>
        /// Specifies the text of the data label.
        /// </summary>
        public string TextFormula { get; set; }
    }
}

