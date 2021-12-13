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
    /// Specifies a single data point
    /// </summary>
    public class ExcelDataPoint : IExcelDataPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelDataPoint" /> class.
        /// </summary>
        public ExcelDataPoint()
        {
            this.IsBubble3D = true;
            this.InvertIfNegative = true;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "bubble3D")
                {
                    this.IsBubble3D = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "idx")
                {
                    this.Index = element.GetAttributeValueOrDefaultOfInt32Type("val", 0);
                }
                else if (element.Name.LocalName == "explosion")
                {
                    this.Explosion = new int?(element.GetAttributeValueOrDefaultOfInt32Type("val", 0));
                }
                else if (element.Name.LocalName == "invertIfNegative")
                {
                    this.InvertIfNegative = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "marker")
                {
                    ExcelDataMarker marker = new ExcelDataMarker();
                    marker.ReadXml(element, mFolder, xFile);
                    this.Marker = marker;
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.DataPointFormat = format;
                }
                else if (element.Name.LocalName == "pictureOptions")
                {
                    Dt.Xls.Chart.PictureOptions options = new Dt.Xls.Chart.PictureOptions();
                    options.ReadXml(element, mFolder, xFile);
                    this.PictureOptions = options;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "dPt", null);
            writer.WriteLeafElementWithAttribute("idx", null, "c", "val", ((int) this.Index).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            writer.WriteLeafElementWithAttribute("invertIfNegative", null, "c", "val", this.InvertIfNegative ? "1" : "0");
            if (!this.IsBubble3D)
            {
                writer.WriteLeafElementWithAttribute("bubble3D", null, "c", "val", "0");
            }
            if (this.PictureOptions != null)
            {
                this.PictureOptions.WriteXml(writer, mFolder, chartFile);
            }
            if (this.DataPointFormat != null)
            {
                (this.DataPointFormat as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
            }
            if (this.Explosion.HasValue)
            {
                writer.WriteLeafElementWithAttribute("explosion", null, "c", "val", ((int) this.Explosion.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.Marker != null)
            {
                this.Marker.WriteXml(writer, mFolder, chartFile);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets or sets the data point format.
        /// </summary>
        /// <value>
        /// The data point format.
        /// </value>
        public IExcelChartFormat DataPointFormat { get; set; }

        /// <summary>
        /// Specifies the amount the data points shall be moved from the center of the pie.
        /// </summary>
        public int? Explosion { get; set; }

        /// <summary>
        /// The index of the datapoint
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [invert if negative].
        /// </summary>
        /// <value>
        /// <c>true</c> if [invert if negative]; otherwise, <c>false</c>.
        /// </value>
        public bool InvertIfNegative { get; set; }

        /// <summary>
        /// Specifies that the bubbles have a 3-D effect applied to them.
        /// </summary>
        public bool IsBubble3D { get; set; }

        /// <summary>
        /// Represents the data marker settings.
        /// </summary>
        public ExcelDataMarker Marker { get; set; }

        /// <summary>
        /// Represents the picture option settings when the fill is blip fill.
        /// </summary>
        public Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }
    }
}

