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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the legend
    /// </summary>
    public class ExcelChartLegend : IExcelChartLegend
    {
        private List<IExcelLegendEntry> legendEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelChartLegend" /> class.
        /// </summary>
        public ExcelChartLegend()
        {
            this.Position = ExcelLegendPositon.Right;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "legendPos")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "r"))
                    {
                        case "r":
                            this.Position = ExcelLegendPositon.Right;
                            break;

                        case "l":
                            this.Position = ExcelLegendPositon.Left;
                            break;

                        case "t":
                            this.Position = ExcelLegendPositon.Top;
                            break;

                        case "b":
                            this.Position = ExcelLegendPositon.Bottom;
                            break;

                        case "tr":
                            this.Position = ExcelLegendPositon.TopRight;
                            break;
                    }
                }
                else if (element.Name.LocalName == "legendEntry")
                {
                    ExcelLegendEntry entry = new ExcelLegendEntry();
                    entry.ReadXml(element, mFolder, xFile);
                    this.LegendEntries.Add(entry);
                }
                else if (element.Name.LocalName == "layout")
                {
                    Dt.Xls.Chart.Layout layout = new Dt.Xls.Chart.Layout();
                    layout.ReadXml(element, mFolder, xFile);
                    this.Layout = layout;
                }
                else if (element.Name.LocalName == "overlay")
                {
                    this.Overlay = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
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
                    this.TextFormat = format2;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("legend", null, "c"))
            {
                string str = "r";
                switch (this.Position)
                {
                    case ExcelLegendPositon.Left:
                        str = "l";
                        break;

                    case ExcelLegendPositon.Top:
                        str = "t";
                        break;

                    case ExcelLegendPositon.Bottom:
                        str = "b";
                        break;

                    case ExcelLegendPositon.TopRight:
                        str = "tr";
                        break;

                    default:
                        str = "r";
                        break;
                }
                writer.WriteLeafElementWithAttribute("legendPos", null, "c", "val", str);
                if ((this.LegendEntries != null) && (this.LegendEntries.Count > 0))
                {
                    using (List<IExcelLegendEntry>.Enumerator enumerator = this.LegendEntries.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            (enumerator.Current as ExcelLegendEntry).WriteXml(writer, mFolder, chartFile);
                        }
                    }
                }
                if (this.Layout != null)
                {
                    this.Layout.WriteXml(writer, mFolder, chartFile);
                }
                writer.WriteLeafElementWithAttribute("overlay", null, "c", "val", this.Overlay ? "1" : "0");
                if (this.ShapeFormat != null)
                {
                    (this.ShapeFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
                if (this.TextFormat != null)
                {
                    (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Sepcifies the Legend layout.
        /// </summary>
        public Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies the legend entries.
        /// </summary>
        public List<IExcelLegendEntry> LegendEntries
        {
            get
            {
                if (this.legendEntries == null)
                {
                    this.legendEntries = new List<IExcelLegendEntry>();
                }
                return this.legendEntries;
            }
            set { this.legendEntries = value; }
        }

        /// <summary>
        /// Specifies that other chart elements shall be allowed to overlap this chart element.
        /// </summary>
        public bool Overlay { get; set; }

        /// <summary>
        /// Specifies the position of the legend.
        /// </summary>
        public ExcelLegendPositon Position { get; set; }

        /// <summary>
        /// Specifies the format info.
        /// </summary>
        public IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }
    }
}

