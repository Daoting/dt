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
    /// Specifies that a blip should be tiled to fill the avilable space.
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.Tile" /> class.
        /// </summary>
        public Tile()
        {
            this.Alignment = TileAlignment.TopLeft;
            this.Flipping = TileFlipping.None;
            this.HorizontalOffset = 0.0;
            this.VerticalOffset = 0.0;
            this.HorizontalRatio = 1.0;
            this.VerticalRatio = 1.0;
        }

        internal void ReadXml(XElement item, MemoryFolder mFolder, XFile xFile)
        {
            string str = item.GetAttributeValueOrDefaultOfStringType("algn", "tl");
            if (str == "tl")
            {
                this.Alignment = TileAlignment.TopLeft;
            }
            else if (str == "b")
            {
                this.Alignment = TileAlignment.Bottom;
            }
            else if (str == "bl")
            {
                this.Alignment = TileAlignment.BottomLeft;
            }
            else if (str == "br")
            {
                this.Alignment = TileAlignment.BottomRight;
            }
            else if (str == "ctr")
            {
                this.Alignment = TileAlignment.Center;
            }
            else if (str == "l")
            {
                this.Alignment = TileAlignment.Left;
            }
            else if (str == "r")
            {
                this.Alignment = TileAlignment.Right;
            }
            else if (str == "t")
            {
                this.Alignment = TileAlignment.Top;
            }
            else if (str == "tr")
            {
                this.Alignment = TileAlignment.TopRight;
            }
            switch (item.GetAttributeValueOrDefaultOfStringType("flip", "none"))
            {
                case "none":
                    this.Flipping = TileFlipping.None;
                    break;

                case "x":
                    this.Flipping = TileFlipping.Horizontal;
                    break;

                case "xy":
                    this.Flipping = TileFlipping.HorizontalVertical;
                    break;

                case "y":
                    this.Flipping = TileFlipping.Vertical;
                    break;
            }
            this.HorizontalRatio = item.GetAttributeValueOrDefaultOfDoubleType("sx", 1.0) / 100000.0;
            this.VerticalRatio = item.GetAttributeValueOrDefaultOfDoubleType("sy", 1.0) / 100000.0;
            this.HorizontalOffset = item.GetAttributeValueOrDefaultOfDoubleType("tx", 0.0) / 100000.0;
            this.VerticalOffset = item.GetAttributeValueOrDefaultOfDoubleType("ty", 0.0) / 100000.0;
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("tile", null, "a"))
            {
                string str = "tl";
                switch (this.Alignment)
                {
                    case TileAlignment.TopLeft:
                        str = "tl";
                        break;

                    case TileAlignment.Bottom:
                        str = "b";
                        break;

                    case TileAlignment.BottomLeft:
                        str = "bl";
                        break;

                    case TileAlignment.BottomRight:
                        str = "br";
                        break;

                    case TileAlignment.Center:
                        str = "ctr";
                        break;

                    case TileAlignment.Left:
                        str = "l";
                        break;

                    case TileAlignment.Right:
                        str = "r";
                        break;

                    case TileAlignment.Top:
                        str = "t";
                        break;

                    case TileAlignment.TopRight:
                        str = "tr";
                        break;
                }
                string str2 = "none";
                switch (this.Flipping)
                {
                    case TileFlipping.None:
                        str2 = "none";
                        break;

                    case TileFlipping.Horizontal:
                        str2 = "x";
                        break;

                    case TileFlipping.HorizontalVertical:
                        str2 = "xy";
                        break;

                    case TileFlipping.Vertical:
                        str2 = "y";
                        break;
                }
                int num = (int)(HorizontalOffset * 100000.0);
                writer.WriteAttributeString("tx", ((int) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                int num2 = (int)(VerticalOffset * 100000.0);
                writer.WriteAttributeString("ty", ((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                int num3 = (int)(HorizontalRatio * 100000.0);
                writer.WriteAttributeString("sx", ((int) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                int num4 = (int)(VerticalRatio * 100000.0);
                writer.WriteAttributeString("sy", ((int) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                writer.WriteAttributeString("flip", str2);
                writer.WriteAttributeString("algn", str);
            }
        }

        /// <summary>
        /// Specifies where to align the first tile with respect to the shape.
        /// </summary>
        public TileAlignment Alignment { get; set; }

        /// <summary>
        /// Specifies the direction in which to flip the source image with tiling.
        /// </summary>
        public TileFlipping Flipping { get; set; }

        /// <summary>
        /// Specifies additional horizontal offset after alignment.
        /// </summary>
        public double HorizontalOffset { get; set; }

        /// <summary>
        /// Specifies the amount to horizontally scale the source rectangle.
        /// </summary>
        public double HorizontalRatio { get; set; }

        /// <summary>
        /// Specifies additional vettial offset after alignment.
        /// </summary>
        public double VerticalOffset { get; set; }

        /// <summary>
        /// Specifies the amount to vertically scale the source rectangle.
        /// </summary>
        public double VerticalRatio { get; set; }
    }
}

