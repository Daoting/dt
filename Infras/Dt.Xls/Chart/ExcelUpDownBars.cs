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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the up and down bars
    /// </summary>
    public class ExcelUpDownBars
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelUpDownBars" /> class.
        /// </summary>
        public ExcelUpDownBars()
        {
            this.GapWidth = 150;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "downBars")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        if (element2.Name.LocalName == "spPr")
                        {
                            ExcelChartFormat format = new ExcelChartFormat();
                            format.ReadXml(element2, mFolder, xFile);
                            this.DownBars = format;
                        }
                    }
                }
                else if (element.Name.LocalName == "upBars")
                {
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName == "spPr")
                        {
                            ExcelChartFormat format2 = new ExcelChartFormat();
                            format2.ReadXml(element3, mFolder, xFile);
                            this.UpBars = format2;
                        }
                    }
                }
                else if (element.Name.LocalName == "gapWidth")
                {
                    this.GapWidth = element.GetAttributeValueOrDefaultOfInt32Type("val", 150);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("upDownBars", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("gapWidth", null, "c", "val", ((int) this.GapWidth).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                using (writer.WriteElement("upBars", null, "c"))
                {
                    if (this.UpBars != null)
                    {
                        (this.UpBars as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                    }
                }
                using (writer.WriteElement("downBars", null, "c"))
                {
                    if (this.DownBars != null)
                    {
                        (this.DownBars as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the Down bars.
        /// </summary>
        public IExcelChartFormat DownBars { get; set; }

        /// <summary>
        /// Specifies the gap width between column or bars.
        /// </summary>
        public int GapWidth { get; set; }

        /// <summary>
        /// Specifies the Up bars.
        /// </summary>
        public IExcelChartFormat UpBars { get; set; }
    }
}

