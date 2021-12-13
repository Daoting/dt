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
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Defines a gradient stop. A gradient stop consists of a position where the stop appears in the color band.
    /// </summary>
    public class ExcelGradientStop
    {
        internal void ReadXml(XElement node)
        {
            double num = node.GetAttributeValueOrDefaultOfDoubleType("pos", 100000.0);
            this.Position = num / 100000.0;
            foreach (XElement element in node.Elements())
            {
                this.DrawingColorSettings = new ExcelDrawingColorSettings();
                this.Color = ChartColorHelper.ReadColor(element, this.DrawingColorSettings);
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("gs", null, "a"))
            {
                double num = Math.Round(this.Position, 5) * 100000.0;
                writer.WriteAttributeString("pos", ((double) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                if (this.Color != null)
                {
                    ChartColorHelper.WriteColor(writer, this.Color, this.DrawingColorSettings);
                }
            }
        }

        /// <summary>
        /// Specifies the stop color.
        /// </summary>
        public IExcelColor Color { get; set; }

        /// <summary>
        /// Represents additional color settings used by this gradient stop.
        /// </summary>
        public ExcelDrawingColorSettings DrawingColorSettings { get; set; }

        /// <summary>
        /// Specifies where this gradient stop should appear in the color band.
        /// </summary>
        public double Position { get; set; }
    }
}

