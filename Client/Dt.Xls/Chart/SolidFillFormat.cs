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
    /// Represents a solid fill format.
    /// </summary>
    public class SolidFillFormat : IFillFormat
    {
        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                this.DrawingColorSettings = new ExcelDrawingColorSettings();
                this.Color = ChartColorHelper.ReadColor(element, this.DrawingColorSettings);
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("solidFill", null, "a"))
            {
                ChartColorHelper.WriteColor(writer, this.Color, this.DrawingColorSettings);
            }
        }

        /// <summary>
        /// Specifies the solid fill color.
        /// </summary>
        public IExcelColor Color { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ExcelDrawingColorSettings DrawingColorSettings { get; set; }

        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get { return  Dt.Xls.Chart.FillFormatType.SolidFill; }
        }
    }
}

