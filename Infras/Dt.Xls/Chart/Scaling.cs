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
    /// Inplement the IScaling interface.
    /// </summary>
    public class Scaling : IScaling
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.Scaling" /> class.
        /// </summary>
        public Scaling()
        {
            this.LogBase = double.NaN;
            this.Orientation = AxisOrientation.MinMax;
            this.Max = double.NaN;
            this.Min = double.NaN;
        }

        internal void ReadXml(XElement node)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "logBase")
                {
                    this.LogBase = element.GetAttributeValueOrDefaultOfDoubleType("val", double.NaN);
                }
                else if (element.Name.LocalName == "min")
                {
                    this.Min = element.GetAttributeValueOrDefaultOfDoubleType("val", double.NaN);
                }
                else if (element.Name.LocalName == "max")
                {
                    this.Max = element.GetAttributeValueOrDefaultOfDoubleType("val", double.NaN);
                }
                else if (element.Name.LocalName == "orientation")
                {
                    if (element.GetAttributeValueOrDefaultOfStringType("val", "minMax") == "minMax")
                    {
                        this.Orientation = AxisOrientation.MinMax;
                    }
                    else
                    {
                        this.Orientation = AxisOrientation.MaxMin;
                    }
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            writer.WriteStartElement("c", "scaling", null);
            if (!double.IsNaN(this.LogBase))
            {
                writer.WriteLeafElementWithAttribute("logBase", null, "c", "val", ((double) this.LogBase).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (this.Orientation == AxisOrientation.MinMax)
            {
                writer.WriteLeafElementWithAttribute("orientation", null, "c", "val", "minMax");
            }
            else
            {
                writer.WriteLeafElementWithAttribute("orientation", null, "c", "val", "maxMin");
            }
            if (!double.IsNaN(this.Max))
            {
                writer.WriteLeafElementWithAttribute("max", null, "c", "val", ((double) this.Max).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            if (!double.IsNaN(this.Min))
            {
                writer.WriteLeafElementWithAttribute("min", null, "c", "val", ((double) this.Min).ToString((IFormatProvider) CultureInfo.InvariantCulture));
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Specifies the logarithmic base for a logarithmic axis.
        /// </summary>
        /// <remarks>
        /// The value should be between 2 and 1000.
        /// </remarks>
        public double LogBase { get; set; }

        /// <summary>
        /// Specifies the maximun value of the axis.
        /// </summary>
        public double Max { get; set; }

        /// <summary>
        /// Specifies the minimun value of the axis.
        /// </summary>
        public double Min { get; set; }

        /// <summary>
        /// Specifies the possible ways to place a picture on a data point, series, wall of floor.
        /// </summary>
        public AxisOrientation Orientation { get; set; }
    }
}

