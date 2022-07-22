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
    /// Represents the chart format
    /// </summary>
    public class ExcelChartFormat : IExcelChartFormat
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "ln")
                {
                    Dt.Xls.Chart.LineFormat format = new Dt.Xls.Chart.LineFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.LineFormat = format;
                }
                IFillFormat format2 = element.ReadFillFormat(mFolder, xFile);
                if (format2 != null)
                {
                    this.FillFormat = format2;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("spPr", null, "c"))
            {
                if (this.FillFormat != null)
                {
                    this.FillFormat.WriteXml(writer, mFolder, chartFile);
                }
                if (this.LineFormat != null)
                {
                    (this.LineFormat as Dt.Xls.Chart.LineFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Gets or sets the fill format.
        /// </summary>
        /// <value>
        /// The fill format.
        /// </value>
        public IFillFormat FillFormat { get; set; }

        /// <summary>
        /// Gets or sets the line format.
        /// </summary>
        /// <value>
        /// The line format.
        /// </value>
        public ILineFormat LineFormat { get; set; }
    }
}

