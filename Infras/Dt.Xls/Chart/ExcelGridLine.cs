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
    /// Defines gridlines
    /// </summary>
    public class ExcelGridLine : IExcelGridLines
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Format = format;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile, string name)
        {
            using (writer.WriteElement(name, null, "c"))
            {
                if (this.Format != null)
                {
                    (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Specify the gridline format
        /// </summary>
        public IExcelChartFormat Format { get; set; }
    }
}

