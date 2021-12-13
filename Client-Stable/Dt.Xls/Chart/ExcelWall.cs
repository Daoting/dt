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
    /// Represents the wall of the chart.
    /// </summary>
    public class ExcelWall : IExcelWall
    {
        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "thickness")
                {
                    this.Thickness = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Format = format;
                }
                else if (element.Name.LocalName == "pictureOptions")
                {
                    Dt.Xls.Chart.PictureOptions options = new Dt.Xls.Chart.PictureOptions();
                    options.ReadXml(element, mFolder, xFile);
                    this.PictureOptions = options;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile, string root)
        {
            using (writer.WriteElement(root, null, "c"))
            {
                using (writer.WriteElement("thickness", null, "c"))
                {
                    writer.WriteAttributeString("val", ((int) this.Thickness).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.Format != null)
                {
                    (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                }
                if (this.PictureOptions != null)
                {
                    this.PictureOptions.WriteXml(writer, mFolder, chartFile);
                }
            }
        }

        /// <summary>
        /// Specifies the wall format settings.
        /// </summary>
        public IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Represents the picture options when the fill is a blip fill.
        /// </summary>
        public Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        /// <summary>
        /// Specifies the thickness of the walls
        /// </summary>
        public int Thickness { get; set; }
    }
}

