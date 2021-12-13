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
    /// Represents the line format settings for the chart.
    /// </summary>
    public class LineFormat : ILineFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.LineFormat" /> class.
        /// </summary>
        public LineFormat()
        {
            this.Width = 0.0;
            this.LineEndingCap = EndLineCap.Square;
            this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.SingleLines;
            this.PenAlignment = Dt.Xls.Chart.PenAlignment.CenterAlignment;
            this.LineDashType = Dt.Xls.Chart.LineDashType.Solid;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            int num = node.GetAttributeValueOrDefaultOfInt32Type("w", 0);
            this.Width = (((double) num) / 12700.0) * 1.3333333333333333;
            string str5 = node.GetAttributeValueOrDefaultOfStringType("cmpd", "sng");
            if (str5 != null)
            {
                if (str5 == "dbl")
                {
                    this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.DoubleLines;
                }
                else if (str5 == "sng")
                {
                    this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.SingleLines;
                }
                else if (str5 == "thickThin")
                {
                    this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.ThickThinDoubleLines;
                }
                else if (str5 == "thinThick")
                {
                    this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.ThinThickDoubleLines;
                }
                else if (str5 == "tri")
                {
                    this.CompoundLineType = Dt.Xls.Chart.CompoundLineType.ThinThickThinTripleLines;
                }
            }
            string str6 = node.GetAttributeValueOrDefaultOfStringType("cap", "sq");
            if (str6 != null)
            {
                if (str6 == "flat")
                {
                    this.LineEndingCap = EndLineCap.Flat;
                }
                else if (str6 == "rnd")
                {
                    this.LineEndingCap = EndLineCap.Round;
                }
                else if (str6 == "sq")
                {
                    this.LineEndingCap = EndLineCap.Square;
                }
            }
            switch (node.GetAttributeValueOrDefaultOfStringType("algn", "ctr"))
            {
                case "ctr":
                    this.PenAlignment = Dt.Xls.Chart.PenAlignment.CenterAlignment;
                    break;

                case "in":
                    this.PenAlignment = Dt.Xls.Chart.PenAlignment.InsetAlignment;
                    break;
            }
            foreach (XElement element in node.Elements())
            {
                IFillFormat format = element.ReadFillFormat(mFolder, xFile);
                if (format != null)
                {
                    this.FillFormat = format;
                }
                if (element.Name.LocalName == "headEnd")
                {
                    LineEndStyle style = new LineEndStyle();
                    style.ReadXml(element);
                    this.HeadLineEndStyle = style;
                }
                else if (element.Name.LocalName == "tailEnd")
                {
                    LineEndStyle style2 = new LineEndStyle();
                    style2.ReadXml(element);
                    this.TailLineEndStyle = style2;
                }
                else if (element.Name.LocalName == "bevel")
                {
                    this.JoinType = EndLineJoinType.Bevel;
                }
                else if (element.Name.LocalName == "miter")
                {
                    this.JoinType = EndLineJoinType.Miter;
                }
                else if (element.Name.LocalName == "round")
                {
                    this.JoinType = EndLineJoinType.Round;
                }
                else if (element.Name.LocalName == "prstDash")
                {
                    switch (element.GetAttributeValueOrDefaultOfStringType("val", "solid"))
                    {
                        case "dash":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.Dash;
                            break;

                        case "dashDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.DashDot;
                            break;

                        case "dot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.Dot;
                            break;

                        case "lgDash":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.LongDash;
                            break;

                        case "lgDashDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.LongDashDot;
                            break;

                        case "lgDashDotDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.LongDashDotDot;
                            break;

                        case "solid":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.Solid;
                            break;

                        case "sysDash":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.SystemDash;
                            break;

                        case "sysDashDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.SystemDashDot;
                            break;

                        case "sysDashDotDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.SystemDashDotDot;
                            break;

                        case "sysDot":
                            this.LineDashType = Dt.Xls.Chart.LineDashType.SystemDot;
                            break;
                    }
                }
                else
                {
                    bool flag1 = element.Name.LocalName == "custDash";
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("ln", null, "a"))
            {
                if (!this.Width.IsZero())
                {
                    double a = ((this.Width * 12700.0) * 72.0) / 96.0;
                    int num2 = (int) Math.Ceiling(a);
                    writer.WriteAttributeString("w", ((int) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                }
                if (this.CompoundLineType != Dt.Xls.Chart.CompoundLineType.SingleLines)
                {
                    switch (this.CompoundLineType)
                    {
                        case Dt.Xls.Chart.CompoundLineType.DoubleLines:
                            writer.WriteAttributeString("cmpd", "dbl");
                            break;

                        case Dt.Xls.Chart.CompoundLineType.SingleLines:
                            writer.WriteAttributeString("cmpd", "sng");
                            break;

                        case Dt.Xls.Chart.CompoundLineType.ThickThinDoubleLines:
                            writer.WriteAttributeString("cmpd", "thickThin");
                            break;

                        case Dt.Xls.Chart.CompoundLineType.ThinThickDoubleLines:
                            writer.WriteAttributeString("cmpd", "thinThick");
                            break;

                        case Dt.Xls.Chart.CompoundLineType.ThinThickThinTripleLines:
                            writer.WriteAttributeString("cmpd", "tri");
                            break;
                    }
                }
                if (this.LineEndingCap != EndLineCap.Square)
                {
                    switch (this.LineEndingCap)
                    {
                        case EndLineCap.Flat:
                            writer.WriteAttributeString("cap", "flat");
                            break;

                        case EndLineCap.Round:
                            writer.WriteAttributeString("cap", "rnd");
                            break;

                        case EndLineCap.Square:
                            writer.WriteAttributeString("cap", "sq");
                            break;
                    }
                }
                switch (this.PenAlignment)
                {
                    case Dt.Xls.Chart.PenAlignment.CenterAlignment:
                        writer.WriteAttributeString("algn", "ctr");
                        break;

                    case Dt.Xls.Chart.PenAlignment.InsetAlignment:
                        writer.WriteAttributeString("algn", "in");
                        break;
                }
                if (this.FillFormat != null)
                {
                    this.FillFormat.WriteXml(writer, mFolder, chartFile);
                }
                using (writer.WriteElement("prstDash", null, "a"))
                {
                    switch (this.LineDashType)
                    {
                        case Dt.Xls.Chart.LineDashType.Solid:
                            writer.WriteAttributeString("val", "solid");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.Dash:
                            writer.WriteAttributeString("val", "dash");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.DashDot:
                            writer.WriteAttributeString("val", "dashDot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.Dot:
                            writer.WriteAttributeString("val", "dot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.LongDash:
                            writer.WriteAttributeString("val", "lgDash");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.LongDashDot:
                            writer.WriteAttributeString("val", "lgDashDot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.LongDashDotDot:
                            writer.WriteAttributeString("val", "lgDashDotDot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.SystemDash:
                            writer.WriteAttributeString("val", "sysDash");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.SystemDashDot:
                            writer.WriteAttributeString("val", "sysDashDot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.SystemDashDotDot:
                            writer.WriteAttributeString("val", "sysDashDotDot");
                            goto Label_02C2;

                        case Dt.Xls.Chart.LineDashType.SystemDot:
                            writer.WriteAttributeString("val", "sysDot");
                            goto Label_02C2;
                    }
                }
            Label_02C2:
                switch (this.JoinType)
                {
                    case EndLineJoinType.Miter:
                    {
                        using (writer.WriteElement("miter", null, "a"))
                        {
                            writer.WriteAttributeString("lim", "800000");
                            break;
                        }
                    }
                    case EndLineJoinType.Bevel:
                    {
                        using (writer.WriteElement("bevel", null, "a"))
                        {
                            break;
                        }
                    }
                    case EndLineJoinType.Round:
                        using (writer.WriteElement("round", null, "a"))
                        {
                        }
                        break;
                }
                if (this.HeadLineEndStyle != null)
                {
                    using (writer.WriteElement("headEnd", null, "a"))
                    {
                        this.HeadLineEndStyle.WriteXml(writer, mFolder, chartFile);
                    }
                }
                if (this.TailLineEndStyle != null)
                {
                    using (writer.WriteElement("tailEnd", null, "a"))
                    {
                        this.TailLineEndStyle.WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the compound line type that is to be used for lines with text such as underlines.
        /// </summary>
        public Dt.Xls.Chart.CompoundLineType CompoundLineType { get; set; }

        /// <summary>
        /// Specifies the line color fill format.
        /// </summary>
        public IFillFormat FillFormat { get; set; }

        /// <summary>
        /// Specifies decorations which can be added to the head of a line
        /// </summary>
        public LineEndStyle HeadLineEndStyle { get; set; }

        /// <summary>
        /// </summary>
        public EndLineJoinType JoinType { get; set; }

        /// <summary>
        /// Represents the line dash type
        /// </summary>
        public Dt.Xls.Chart.LineDashType LineDashType { get; set; }

        /// <summary>
        /// Specifies the ending caps that should be used for this line.
        /// </summary>
        /// <remarks>
        /// The default value is Square.
        /// </remarks>
        public EndLineCap LineEndingCap { get; set; }

        /// <summary>
        /// Specifies the Pen Alignment type.
        /// </summary>
        public Dt.Xls.Chart.PenAlignment PenAlignment { get; set; }

        /// <summary>
        /// Specifies decorations which can be added to the tail of a line
        /// </summary>
        public LineEndStyle TailLineEndStyle { get; set; }

        /// <summary>
        /// Specifies the width to be used for the underline stroke. Default value is 0.
        /// </summary>
        public double Width { get; set; }
    }
}

