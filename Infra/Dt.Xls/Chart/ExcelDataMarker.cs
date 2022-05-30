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
    /// Specifies a data marker
    /// </summary>
    public class ExcelDataMarker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelDataMarker" /> class.
        /// </summary>
        public ExcelDataMarker()
        {
            this.MarkerSize = 5;
            this.MarkerStyle = DataPointMarkStyle.None;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "size")
                {
                    this.MarkerSize = element.GetAttributeValueOrDefaultOfInt32Type("val", 5);
                }
                else if (element.Name.LocalName == "spPr")
                {
                    ExcelChartFormat format = new ExcelChartFormat();
                    format.ReadXml(element, mFolder, xFile);
                    this.Format = format;
                }
                else if (element.Name.LocalName == "symbol")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "none"))
                    {
                        case "circle":
                            this.MarkerStyle = DataPointMarkStyle.Circle;
                            break;

                        case "dash":
                            this.MarkerStyle = DataPointMarkStyle.Dash;
                            break;

                        case "diamond":
                            this.MarkerStyle = DataPointMarkStyle.Diamond;
                            break;

                        case "dot":
                            this.MarkerStyle = DataPointMarkStyle.Dot;
                            break;

                        case "none":
                            this.MarkerStyle = DataPointMarkStyle.None;
                            break;

                        case "picture":
                            this.MarkerStyle = DataPointMarkStyle.Picture;
                            break;

                        case "plus":
                            this.MarkerStyle = DataPointMarkStyle.Plus;
                            break;

                        case "square":
                            this.MarkerStyle = DataPointMarkStyle.Square;
                            break;

                        case "star":
                            this.MarkerStyle = DataPointMarkStyle.Star;
                            break;

                        case "triangle":
                            this.MarkerStyle = DataPointMarkStyle.Triangle;
                            break;

                        case "x":
                            this.MarkerStyle = DataPointMarkStyle.X;
                            break;
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("marker", null, "c"))
            {
                if (this.MarkerStyle == DataPointMarkStyle.None)
                {
                    writer.WriteLeafElementWithAttribute("symbol", null, "c", "val", "none");
                }
                else
                {
                    string str = "none";
                    switch (this.MarkerStyle)
                    {
                        case DataPointMarkStyle.None:
                            str = "none";
                            break;

                        case DataPointMarkStyle.Circle:
                            str = "circle";
                            break;

                        case DataPointMarkStyle.Dash:
                            str = "dash";
                            break;

                        case DataPointMarkStyle.Diamond:
                            str = "diamond";
                            break;

                        case DataPointMarkStyle.Dot:
                            str = "dot";
                            break;

                        case DataPointMarkStyle.Picture:
                            str = "picture";
                            break;

                        case DataPointMarkStyle.Plus:
                            str = "plus";
                            break;

                        case DataPointMarkStyle.Square:
                            str = "square";
                            break;

                        case DataPointMarkStyle.Star:
                            str = "star";
                            break;

                        case DataPointMarkStyle.Triangle:
                            str = "triangle";
                            break;

                        case DataPointMarkStyle.X:
                            str = "x";
                            break;
                    }
                    writer.WriteLeafElementWithAttribute("symbol", null, "c", "val", str);
                    writer.WriteLeafElementWithAttribute("size", null, "c", "val", ((int) this.MarkerSize).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    if (this.Format != null)
                    {
                        (this.Format as ExcelChartFormat).WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the data marker style.
        /// </summary>
        public IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Specifies the size of the marker in points
        /// </summary>
        public int MarkerSize { get; set; }

        /// <summary>
        /// Specifies the style of the marker.
        /// </summary>
        public DataPointMarkStyle MarkerStyle { get; set; }
    }
}

