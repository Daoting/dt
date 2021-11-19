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
    /// Represents a data table.
    /// </summary>
    public class ExcelChartDataTable : IExcelChartDataTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelChartDataTable" /> class.
        /// </summary>
        public ExcelChartDataTable()
        {
            this.ShowHorizontalBorder = true;
            this.ShowVerticalBorder = true;
            this.ShowOutlineBorder = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "showHorzBorder")
                {
                    this.ShowHorizontalBorder = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                if (element.Name.LocalName == "showVertBorder")
                {
                    this.ShowVerticalBorder = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                if (element.Name.LocalName == "showOutline")
                {
                    this.ShowOutlineBorder = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                if (element.Name.LocalName == "showKeys")
                {
                    this.ShowLegendKeys = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Format = format;
                }
                if (element.Name.LocalName == "txPr")
                {
                    ExcelTextFormat format2 = new ExcelTextFormat();
                    format2.ReadXml(element, mFolder, xFile);
                    this.TextFormat = format2;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("dTable", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("showHorzBorder", null, "c", "val", this.ShowHorizontalBorder ? "1" : "0");
                writer.WriteLeafElementWithAttribute("showVertBorder", null, "c", "val", this.ShowVerticalBorder ? "1" : "0");
                writer.WriteLeafElementWithAttribute("showOutline", null, "c", "val", this.ShowOutlineBorder ? "1" : "0");
                writer.WriteLeafElementWithAttribute("showKeys", null, "c", "val", this.ShowLegendKeys ? "1" : "0");
                if (this.Format != null)
                {
                    (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
                if (this.TextFormat != null)
                {
                    (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Represent the data tale format settings
        /// </summary>
        public IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Indicating whether show Horizontal border in data table.
        /// </summary>
        public bool ShowHorizontalBorder { get; set; }

        /// <summary>
        /// Indicating whether show legend keys in data tale
        /// </summary>
        public bool ShowLegendKeys { get; set; }

        /// <summary>
        /// Indicating whether show outline border in data table.
        /// </summary>
        public bool ShowOutlineBorder { get; set; }

        /// <summary>
        /// Indicating whether show vertical border in data table
        /// </summary>
        public bool ShowVerticalBorder { get; set; }

        /// <summary>
        /// Represents the data table text formattings
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }
    }
}

