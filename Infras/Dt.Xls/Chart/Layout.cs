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
    /// Specifies how the chart element is placed on the chart.
    /// </summary>
    public class Layout
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "manualLayout")
                {
                    ExcelManualLayout layout = new ExcelManualLayout();
                    layout.ReadXml(element);
                    this.ManualLayout = layout;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("layout", null, "c"))
            {
                if (this.ManualLayout != null)
                {
                    this.ManualLayout.WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Specifies that the layout is manual layout
        /// </summary>
        public ExcelManualLayout ManualLayout { get; set; }
    }
}

