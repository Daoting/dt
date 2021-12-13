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
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents a gradient fill format.
    /// </summary>
    public class GradientFillFormat : IFillFormat
    {
        private List<ExcelGradientStop> gradientStops = new List<ExcelGradientStop>();

        internal void ReadXml(XElement node)
        {
            switch (node.GetAttributeValueOrDefaultOfStringType("flip", "none"))
            {
                case "x":
                    this.FlipMode = TileFilpMode.Horizontal;
                    break;

                case "y":
                    this.FlipMode = TileFilpMode.Vertical;
                    break;

                case "xy":
                    this.FlipMode = TileFilpMode.Both;
                    break;

                default:
                    this.FlipMode = TileFilpMode.None;
                    break;
            }
            this.RotateWithShape = node.GetAttributeValueOrDefaultOfBooleanType("rotWithShape", false);
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "gsLst")
                {
                    foreach (XElement element2 in element.Elements())
                    {
                        ExcelGradientStop stop = new ExcelGradientStop();
                        stop.ReadXml(element2);
                        this.GradientStops.Add(stop);
                    }
                }
                else if (element.Name.LocalName == "lin")
                {
                    this.GradientFillType = Dt.Xls.Chart.GradientFillType.Linear;
                    this.Angle = ((double) element.GetAttributeValueOrDefaultOfInt32Type("ang", 0)) / 60000.0;
                    this.Scaled = element.GetAttributeValueOrDefaultOfBooleanType("scaled", false);
                }
                else if (element.Name.LocalName == "path")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("path", "shape"))
                    {
                        case "shape":
                            this.GradientFillType = Dt.Xls.Chart.GradientFillType.Shape;
                            break;

                        case "circle":
                            this.GradientFillType = Dt.Xls.Chart.GradientFillType.Circle;
                            break;

                        case "rect":
                            this.GradientFillType = Dt.Xls.Chart.GradientFillType.Rectange;
                            break;
                    }
                    foreach (XElement element3 in element.Elements())
                    {
                        if (element3.Name.LocalName == "fillToRect")
                        {
                            this.FillToRectLeftOffset = ((double) element3.GetAttributeValueOrDefaultOfInt32Type("l", 0)) / 100000.0;
                            this.FillToRectRightOffset = ((double) element3.GetAttributeValueOrDefaultOfInt32Type("r", 0)) / 100000.0;
                            this.FillToRectBottomOffset = ((double) element3.GetAttributeValueOrDefaultOfInt32Type("t", 0)) / 100000.0;
                            this.FillToRectTopOffset = ((double) element3.GetAttributeValueOrDefaultOfInt32Type("b", 0)) / 100000.0;
                        }
                    }
                }
                else if (element.Name.LocalName == "tileRect")
                {
                    this.TileRectLeftOffset = ((double) element.GetAttributeValueOrDefaultOfInt32Type("l", 0)) / 100000.0;
                    this.TileRectRightOffset = ((double) element.GetAttributeValueOrDefaultOfInt32Type("r", 0)) / 100000.0;
                    this.TileRectBottomOffset = ((double) element.GetAttributeValueOrDefaultOfInt32Type("t", 0)) / 100000.0;
                    this.TileRectTopOffset = ((double) element.GetAttributeValueOrDefaultOfInt32Type("b", 0)) / 100000.0;
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("gradFill", null, "a"))
            {
                if (this.FlipMode != TileFilpMode.None)
                {
                    string str = "none";
                    switch (this.FlipMode)
                    {
                        case TileFilpMode.None:
                            str = "none";
                            break;

                        case TileFilpMode.Horizontal:
                            str = "x";
                            break;

                        case TileFilpMode.Vertical:
                            str = "y";
                            break;

                        case TileFilpMode.Both:
                            str = "xy";
                            break;
                    }
                    writer.WriteAttributeString("flip", str);
                }
                if (this.RotateWithShape)
                {
                    writer.WriteAttributeString("rotWithShape", "1");
                }
                if (this.GradientStops.Count > 0)
                {
                    using (writer.WriteElement("gsLst", null, "a"))
                    {
                        using (List<ExcelGradientStop>.Enumerator enumerator = this.GradientStops.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                            {
                                enumerator.Current.WriteXml(writer, mFolder, chartFile);
                            }
                        }
                    }
                }
                if (this.GradientFillType == Dt.Xls.Chart.GradientFillType.Linear)
                {
                    using (writer.WriteElement("lin", null, "a"))
                    {
                        double num = this.Angle * 60000.0;
                        writer.WriteAttributeString("ang", ((double) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("scaled", this.Scaled ? "1" : "0");
                        goto Label_02E2;
                    }
                }
                using (writer.WriteElement("path", null, "a"))
                {
                    string str2 = "shape";
                    if (this.GradientFillType == Dt.Xls.Chart.GradientFillType.Circle)
                    {
                        str2 = "circle";
                    }
                    else if (this.GradientFillType == Dt.Xls.Chart.GradientFillType.Rectange)
                    {
                        str2 = "rect";
                    }
                    writer.WriteAttributeString("path", str2);
                    if ((!this.FillToRectBottomOffset.IsZero() || !this.FillToRectLeftOffset.IsZero()) || (!this.FillToRectRightOffset.IsZero() || !this.FillToRectTopOffset.IsZero()))
                    {
                        using (writer.WriteElement("fillToRect", null, "a"))
                        {
                            if (!this.FillToRectBottomOffset.IsZero())
                            {
                                double num2 = this.FillToRectBottomOffset * 100000.0;
                                writer.WriteAttributeString("b", ((double) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.FillToRectLeftOffset.IsZero())
                            {
                                double num3 = this.FillToRectLeftOffset * 100000.0;
                                writer.WriteAttributeString("l", ((double) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.FillToRectRightOffset.IsZero())
                            {
                                double num4 = this.FillToRectRightOffset * 100000.0;
                                writer.WriteAttributeString("r", ((double) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                            if (!this.FillToRectTopOffset.IsZero())
                            {
                                double num5 = this.FillToRectTopOffset * 100000.0;
                                writer.WriteAttributeString("t", ((double) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            Label_02E2:
                if ((!this.TileRectBottomOffset.IsZero() || !this.TileRectLeftOffset.IsZero()) || (!this.TileRectRightOffset.IsZero() || !this.TileRectTopOffset.IsZero()))
                {
                    using (writer.WriteElement("tileRect", null, "a"))
                    {
                        if (!this.TileRectBottomOffset.IsZero())
                        {
                            double num6 = this.TileRectBottomOffset * 100000.0;
                            writer.WriteAttributeString("b", ((double) num6).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (!this.TileRectLeftOffset.IsZero())
                        {
                            double num7 = this.TileRectLeftOffset * 100000.0;
                            writer.WriteAttributeString("l", ((double) num7).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (!this.TileRectRightOffset.IsZero())
                        {
                            double num8 = this.TileRectRightOffset * 100000.0;
                            writer.WriteAttributeString("r", ((double) num8).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                        if (!this.TileRectTopOffset.IsZero())
                        {
                            double num9 = this.TileRectTopOffset * 100000.0;
                            writer.WriteAttributeString("t", ((double) num9).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the direction of color change for the grandient. 
        /// </summary>
        /// <remarks>
        /// The value should be between 0 (inclusive) and 21600000 (exclusive).
        /// </remarks>
        public double Angle { get; set; }

        /// <summary>
        /// specifies the fill format type.
        /// </summary>
        public Dt.Xls.Chart.FillFormatType FillFormatType
        {
            get { return  Dt.Xls.Chart.FillFormatType.GradientFill; }
        }

        /// <summary>
        /// Specifies the bottom edge of the rectange
        /// </summary>
        public double FillToRectBottomOffset { get; set; }

        /// <summary>
        /// Specifies the left edge of the rectange
        /// </summary>
        public double FillToRectLeftOffset { get; set; }

        /// <summary>
        /// Specifies the right edge of the rectange
        /// </summary>
        public double FillToRectRightOffset { get; set; }

        /// <summary>
        /// Specifies the top edge of the rectange
        /// </summary>
        public double FillToRectTopOffset { get; set; }

        /// <summary>
        /// Specifies the direction in which to flip the gradient while tiling. 
        /// </summary>
        /// <remarks>
        /// Normally a gradient fill encompasses the entire bounding box of the shape which contains the fill. However, with the tileRect element, it's
        /// possible to difine a tile rectange which is smaller than the bounding box. In this situation, the gradient fill is encompassed withing the tile rectange
        /// and the tile rectange is tiles across the bounding box to fill the entire area.
        /// </remarks>
        public TileFilpMode FlipMode { get; set; }

        /// <summary>
        /// Describes the  of path to follow for a path gradient shade.
        /// </summary>
        public Dt.Xls.Chart.GradientFillType GradientFillType { get; set; }

        /// <summary>
        /// The list of gradient stops that specifies the gradient colors and their relative position in the color band.
        /// </summary>
        public List<ExcelGradientStop> GradientStops
        {
            get { return  this.gradientStops; }
        }

        /// <summary>
        /// Specifies if a fill rotates along with a shape when the shape is rotate.
        /// </summary>
        public bool RotateWithShape { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Scaled { get; set; }

        /// <summary>
        /// Specifies the bottom edge of the rectange
        /// </summary>
        public double TileRectBottomOffset { get; set; }

        /// <summary>
        /// Specifies the left edge of the rectange
        /// </summary>
        public double TileRectLeftOffset { get; set; }

        /// <summary>
        /// Specifies the right edge of the rectange
        /// </summary>
        public double TileRectRightOffset { get; set; }

        /// <summary>
        /// Specifies the top edge of the rectange
        /// </summary>
        public double TileRectTopOffset { get; set; }
    }
}

