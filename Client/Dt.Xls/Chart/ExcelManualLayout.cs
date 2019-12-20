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
    /// Specifies how the chart element is placed on the chart
    /// </summary>
    public class ExcelManualLayout
    {
        private ExcelLayoutMode ReadLayoutMode(string value)
        {
            if ((value != "factor") && (value == "edge"))
            {
                return ExcelLayoutMode.Edge;
            }
            return ExcelLayoutMode.Factor;
        }

        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "x")
                {
                    this.Left = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "y")
                {
                    this.Top = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "w")
                {
                    this.Width = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "h")
                {
                    this.Height = element.GetAttributeValueOrDefaultOfDoubleType("val", 0.0);
                }
                else if (element.Name.LocalName == "layoutTarget")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "outer"))
                    {
                        case "outer":
                            this.Target = ExcelLayoutTarget.Outer;
                            break;

                        case "inner":
                            this.Target = ExcelLayoutTarget.Inner;
                            break;
                    }
                }
                else if (element.Name.LocalName == "xMode")
                {
                    this.LeftMode = this.ReadLayoutMode(element.GetAttributeValueOrDefaultOfStringType("val", "factor"));
                }
                else if (element.Name.LocalName == "yMode")
                {
                    this.TopMode = this.ReadLayoutMode(element.GetAttributeValueOrDefaultOfStringType("val", "factor"));
                }
                else if (element.Name.LocalName == "wMode")
                {
                    this.WidthMode = this.ReadLayoutMode(element.GetAttributeValueOrDefaultOfStringType("val", "factor"));
                }
                else if (element.Name.LocalName == "hMode")
                {
                    this.HeightMode = this.ReadLayoutMode(element.GetAttributeValueOrDefaultOfStringType("val", "factor"));
                }
            }
        }

        private void WriteElement(XmlWriter writer, string prefix, string name, string attribleName, string attribleValue)
        {
            writer.WriteStartElement(prefix, name, null);
            writer.WriteAttributeString(attribleName, attribleValue);
            writer.WriteEndElement();
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "manualLayout", null);
            if (this.Target != ExcelLayoutTarget.Outer)
            {
                this.WriteElement(writer, "c", "layoutTarget", "val", "inner");
            }
            if (this.LeftMode != ExcelLayoutMode.Factor)
            {
                this.WriteElement(writer, "c", "xMode", "val", "edge");
            }
            if (this.TopMode != ExcelLayoutMode.Factor)
            {
                this.WriteElement(writer, "c", "yMode", "val", "edge");
            }
            if (this.WidthMode != ExcelLayoutMode.Factor)
            {
                this.WriteElement(writer, "c", "wMode", "val", "edge");
            }
            if (this.HeightMode != ExcelLayoutMode.Factor)
            {
                this.WriteElement(writer, "c", "hMode", "val", "edge");
            }
            if (this.Left >= 0.0)
            {
                this.WriteElement(writer, "c", "x", "val", ((double) this.Left).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.Top >= 0.0)
            {
                this.WriteElement(writer, "c", "y", "val", ((double) this.Top).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!this.Width.IsZero())
            {
                this.WriteElement(writer, "c", "w", "val", ((double) this.Width).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!this.Height.IsZero())
            {
                this.WriteElement(writer, "c", "h", "val", ((double) this.Height).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the height (if height mode is Factor) or bottom (if height mode is edge) of the chart
        /// element as a fraction of the height of the chart.
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Specifies how to interpret the height element for this layout
        /// </summary>
        public ExcelLayoutMode HeightMode { get; set; }

        /// <summary>
        /// Specifies the x location(left) of the chart element as a fractin of the width of the chart
        /// If Left mode is Factor, then the position is reletive to the default position for the chart element
        /// </summary>
        public double Left { get; set; }

        /// <summary>
        /// Specifies how to interpret the left element for this layout
        /// </summary>
        public ExcelLayoutMode LeftMode { get; set; }

        /// <summary>
        /// Specifies the layout target
        /// </summary>
        public ExcelLayoutTarget Target { get; set; }

        /// <summary>
        /// Specifies the top of the chart element as a fraction of the height of the chart. If Top mode is factor
        /// the the position is relative to the default position for the chart element
        /// </summary>
        public double Top { get; set; }

        /// <summary>
        /// Specifies how to interpret the top element for this layout
        /// </summary>
        public ExcelLayoutMode TopMode { get; set; }

        /// <summary>
        /// Specifies the width (if width mode is Factor) or right (if width mode is edge) of the chart
        /// element as a fraction of the width of the chart.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Specifies how to interpret the width element for this layout
        /// </summary>
        public ExcelLayoutMode WidthMode { get; set; }
    }
}

