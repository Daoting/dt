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
using System.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies a lengend entry
    /// </summary>
    public class ExcelLegendEntry : IExcelLegendEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelLegendEntry" /> class.
        /// </summary>
        public ExcelLegendEntry()
        {
            this.Delete = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "delete")
                {
                    this.Delete = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "idx")
                {
                    this.Index = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if ((element.Name.LocalName != "extLst") && (element.Name.LocalName == "txPr"))
                {
                    ExcelTextFormat format = new ExcelTextFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.TextFormat = format;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("legendEntry", null, "c"))
            {
                writer.WriteLeafElementWithAttribute("idx", null, "c", "val", ((int) this.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                if (this.TextFormat != null)
                {
                    (this.TextFormat as ExcelTextFormat).WriteXml(writer, mFolder, chartFile);
                }
                else
                {
                    writer.WriteLeafElementWithAttribute("delete", null, "c", "val", this.Delete ? "1" : "0");
                }
            }
        }

        /// <summary>
        /// Specifies that the chart element specified by its containing element shall be deleted from the chart.
        /// </summary>
        public bool Delete { get; set; }

        /// <summary>
        /// Specifies the legend entry index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        public IExcelTextFormat TextFormat { get; set; }
    }
}

