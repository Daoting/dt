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
    /// Represents lines used in Chart, such as series line, drop line and high-low lines.
    /// </summary>
    public class ExcelChartLines
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "spPr")
                {
                    this.Format = new ExcelChartFormat();
                    (this.Format as ExcelChartFormat).ReadXml(element, mFolder, xFile);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile, string root)
        {
            using (writer.WriteElement(root, null, "c"))
            {
                if (this.Format != null)
                {
                    (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Specifies the dropLine format.
        /// </summary>
        public IExcelChartFormat Format { get; set; }
    }
}

