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
    /// Specifies the picture to be used on the data point, series, wall, or floor.
    /// </summary>
    public class PictureOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.PictureOptions" /> class.
        /// </summary>
        public PictureOptions()
        {
            this.ApplyToEnd = true;
            this.ApplyToFront = true;
            this.ApplyToSides = true;
            this.PictureFormat = Dt.Xls.Chart.PictureFormat.Stretch;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile file)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "applyToFront")
                {
                    this.ApplyToFront = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "applyToEnd")
                {
                    this.ApplyToEnd = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "applyToSides")
                {
                    this.ApplyToSides = element.GetAttributeValueOrDefaultOfBooleanType("val", true);
                }
                else if (element.Name.LocalName == "pictureFormat")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "stretch"))
                    {
                        case "stretch":
                            this.PictureFormat = Dt.Xls.Chart.PictureFormat.Stretch;
                            break;

                        case "stack":
                            this.PictureFormat = Dt.Xls.Chart.PictureFormat.Stack;
                            break;

                        case "stackScale":
                            this.PictureFormat = Dt.Xls.Chart.PictureFormat.StackAndScale;
                            break;
                    }
                }
                else if (element.Name.LocalName == "pictureStackUnit")
                {
                    this.PictureStackUnit = element.GetAttributeValueOrDefaultOfDoubleType("val", 1.0);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("pictureOptions", null, "c"))
            {
                if (!this.ApplyToEnd)
                {
                    writer.WriteLeafElementWithAttribute("applyToEnd", null, "c", "val", "0");
                }
                if (!this.ApplyToFront)
                {
                    writer.WriteLeafElementWithAttribute("applyFront", null, "c", "val", "0");
                }
                if (!this.ApplyToSides)
                {
                    writer.WriteLeafElementWithAttribute("applySides", null, "c", "val", "0");
                }
                string str = "stretch";
                switch (this.PictureFormat)
                {
                    case Dt.Xls.Chart.PictureFormat.Stretch:
                        str = "stretch";
                        break;

                    case Dt.Xls.Chart.PictureFormat.Stack:
                        str = "stack";
                        break;

                    case Dt.Xls.Chart.PictureFormat.StackAndScale:
                        str = "stackScale";
                        break;
                }
                writer.WriteLeafElementWithAttribute("pictureFormat", null, "c", "val", str);
                if ((this.PictureFormat == Dt.Xls.Chart.PictureFormat.StackAndScale) && (this.PictureStackUnit > 0.0))
                {
                    writer.WriteLeafElementWithAttribute("pictureStackUnit", null, "c", "val", ((double) this.PictureStackUnit).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
            }
        }

        /// <summary>
        /// Specifies the picture shall be applied to the end of the point or series.
        /// </summary>
        public bool ApplyToEnd { get; set; }

        /// <summary>
        /// Specifies the picture shall be applied to the front of the point or series.
        /// </summary>
        public bool ApplyToFront { get; set; }

        /// <summary>
        /// Specifies the picture shall be applied to the sides of the point or series.
        /// </summary>
        public bool ApplyToSides { get; set; }

        /// <summary>
        /// Gets or sets the picture format.
        /// </summary>
        /// <value>
        /// The picture format.
        /// </value>
        public Dt.Xls.Chart.PictureFormat PictureFormat { get; set; }

        /// <summary>
        /// Gets or sets the picture stack unit.
        /// </summary>
        /// <value>
        /// The picture stack unit.
        /// </value>
        public double PictureStackUnit { get; set; }
    }
}

