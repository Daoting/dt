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
    /// Represents the text format.
    /// </summary>
    public class ExcelTextFormat : IExcelTextFormat
    {
        private List<TextParagraph> textParagraphs;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.ExcelTextFormat" /> class.
        /// </summary>
        public ExcelTextFormat()
        {
            this.LeftInset = 7.2;
            this.RightInset = 7.2;
            this.TopInset = 3.6;
            this.BottomInset = 3.6;
            this.textParagraphs = new List<TextParagraph>();
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                string str8;
                if (element.Name.LocalName != "bodyPr")
                {
                    goto Label_039A;
                }
                string str6 = element.GetAttributeValueOrDefaultOfStringType("anchor", "t");
                if (str6 != null)
                {
                    if (str6 != "b")
                    {
                        if (str6 == "ctr")
                        {
                            goto Label_009C;
                        }
                        if (str6 == "dist")
                        {
                            goto Label_00A5;
                        }
                        if (str6 == "just")
                        {
                            goto Label_00AE;
                        }
                        if (str6 == "t")
                        {
                            goto Label_00B7;
                        }
                    }
                    else
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Bottom;
                    }
                }
                goto Label_00BE;
            Label_009C:
                this.TextAnchoringType = TextAnchoringTypes.Center;
                goto Label_00BE;
            Label_00A5:
                this.TextAnchoringType = TextAnchoringTypes.Distributed;
                goto Label_00BE;
            Label_00AE:
                this.TextAnchoringType = TextAnchoringTypes.Justified;
                goto Label_00BE;
            Label_00B7:
                this.TextAnchoringType = TextAnchoringTypes.Top;
            Label_00BE:
                this.IsTextBoxAnchorCenter = element.GetAttributeValueOrDefaultOfBooleanType("anchorCtr", false);
                string str7 = element.GetAttributeValueOrDefaultOfStringType("horzOverflow", "overflow");
                if (str7 != null)
                {
                    if (str7 == "overflow")
                    {
                        this.TextHorizontalOverflow = TextOverflowTypes.Overflow;
                    }
                    else if (str7 == "clip")
                    {
                        goto Label_010E;
                    }
                }
                goto Label_0115;
            Label_010E:
                this.TextHorizontalOverflow = TextOverflowTypes.Clip;
            Label_0115:
                if ((str8 = element.GetAttributeValueOrDefaultOfStringType("vertOverflow", "overflow")) != null)
                {
                    if (str8 != "overflow")
                    {
                        if (str8 == "clip")
                        {
                            goto Label_0161;
                        }
                        if (str8 == "ellipsis")
                        {
                            goto Label_016A;
                        }
                    }
                    else
                    {
                        this.TextVerticalOverflow = TextOverflowTypes.Overflow;
                    }
                }
                goto Label_0171;
            Label_0161:
                this.TextVerticalOverflow = TextOverflowTypes.Clip;
                goto Label_0171;
            Label_016A:
                this.TextVerticalOverflow = TextOverflowTypes.Ellipsis;
            Label_0171:
                switch (element.GetAttributeValueOrDefaultOfStringType("vert", "horz"))
                {
                    case "horz":
                        this.VerticalText = TextVerticalTypes.Horizontal;
                        break;

                    case "vert":
                        this.VerticalText = TextVerticalTypes.Vertical;
                        break;

                    case "vert270":
                        this.VerticalText = TextVerticalTypes.Vertical270;
                        break;

                    case "wordArtVert":
                        this.VerticalText = TextVerticalTypes.WordArtVertical;
                        break;

                    case "wordArtVertRtl":
                        this.VerticalText = TextVerticalTypes.VerticalWordArtRightToLeft;
                        break;

                    case "eaVert":
                        this.VerticalText = TextVerticalTypes.EastAsianVertical;
                        break;

                    case "mongolianVert":
                        this.VerticalText = TextVerticalTypes.MongolianVertical;
                        break;
                }
                int num = element.GetAttributeValueOrDefaultOfInt32Type("tIns", 0xb298);
                int num2 = element.GetAttributeValueOrDefaultOfInt32Type("bIns", 0xb298);
                int num3 = element.GetAttributeValueOrDefaultOfInt32Type("lIns", 0x16530);
                int num4 = element.GetAttributeValueOrDefaultOfInt32Type("rIns", 0x16530);
                this.TopInset = (((double) num) / 914400.0) * 72.0;
                this.BottomInset = (((double) num2) / 914400.0) * 72.0;
                this.LeftInset = (((double) num3) / 914400.0) * 72.0;
                this.RightInset = (((double) num4) / 914400.0) * 72.0;
                int num5 = element.GetAttributeValueOrDefaultOfInt32Type("rot", 0);
                this.Rotation = ((double) num5) / 60000.0;
                this.UpRight = element.GetAttributeValueOrDefaultOfBooleanType("upRight", false);
                switch (element.GetAttributeValueOrDefaultOfStringType("wrap", "square"))
                {
                    case "square":
                        this.TextWrappingType = TextWrappingTypes.Square;
                        break;

                    case "none":
                        this.TextWrappingType = TextWrappingTypes.None;
                        break;
                }
                continue;
            Label_039A:
                if ((element.Name.LocalName != "lstStyle") && (element.Name.LocalName == "p"))
                {
                    TextParagraph paragraph = new TextParagraph();
                    paragraph.ReadXml(element, mFolder, xFile);
                    this.TextParagraphs.Add(paragraph);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("txPr", null, "c"))
            {
                using (writer.WriteElement("bodyPr", null, "a"))
                {
                    if (!this.Rotation.IsZero())
                    {
                        double num = this.Rotation * 60000.0;
                        writer.WriteAttributeString("rot", ((double) num).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (this.TextHorizontalOverflow != TextOverflowTypes.Overflow)
                    {
                        string str = "overflow";
                        switch (this.TextHorizontalOverflow)
                        {
                            case TextOverflowTypes.Overflow:
                                str = "overflow";
                                break;

                            case TextOverflowTypes.Clip:
                                str = "clip";
                                break;

                            case TextOverflowTypes.Ellipsis:
                                str = "ellipsis";
                                break;
                        }
                        writer.WriteAttributeString("horzOverflow", str);
                    }
                    if (this.TextVerticalOverflow != TextOverflowTypes.Overflow)
                    {
                        string str2 = "overflow";
                        switch (this.TextVerticalOverflow)
                        {
                            case TextOverflowTypes.Overflow:
                                str2 = "overflow";
                                break;

                            case TextOverflowTypes.Clip:
                                str2 = "clip";
                                break;

                            case TextOverflowTypes.Ellipsis:
                                str2 = "ellipsis";
                                break;
                        }
                        writer.WriteAttributeString("vertOverflow", str2);
                    }
                    if (this.VerticalText != TextVerticalTypes.Horizontal)
                    {
                        string str3 = "horz";
                        switch (this.VerticalText)
                        {
                            case TextVerticalTypes.Horizontal:
                                str3 = "horz";
                                break;

                            case TextVerticalTypes.Vertical:
                                str3 = "vert";
                                break;

                            case TextVerticalTypes.Vertical270:
                                str3 = "vert270";
                                break;

                            case TextVerticalTypes.WordArtVertical:
                                str3 = "wordArtVert";
                                break;

                            case TextVerticalTypes.VerticalWordArtRightToLeft:
                                str3 = "wordArtVertRtl";
                                break;

                            case TextVerticalTypes.EastAsianVertical:
                                str3 = "eaVert";
                                break;

                            case TextVerticalTypes.MongolianVertical:
                                str3 = "mongolianVert";
                                break;
                        }
                        writer.WriteAttributeString("vert", str3);
                    }
                    if (this.TextWrappingType != TextWrappingTypes.Square)
                    {
                        writer.WriteAttributeString("wrap", (this.TextWrappingType == TextWrappingTypes.Square) ? "square" : "none");
                    }
                    if (!(this.LeftInset - 7.2).IsZero())
                    {
                        double num2 = (this.LeftInset * 914400.0) / 72.0;
                        writer.WriteAttributeString("lIns", ((double) num2).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (!(this.RightInset - 7.2).IsZero())
                    {
                        double num3 = (this.RightInset * 914400.0) / 72.0;
                        writer.WriteAttributeString("rIns", ((double) num3).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (!(this.TopInset - 3.6).IsZero())
                    {
                        double num4 = (this.TopInset * 914400.0) / 72.0;
                        writer.WriteAttributeString("tIns", ((double) num4).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    if (!(this.BottomInset - 3.6).IsZero())
                    {
                        double num5 = (this.BottomInset * 914400.0) / 72.0;
                        writer.WriteAttributeString("bIns", ((double) num5).ToString((IFormatProvider) CultureInfo.InvariantCulture));
                    }
                    switch (this.TextAnchoringType)
                    {
                        case TextAnchoringTypes.Top:
                            writer.WriteAttributeString("anchor", "t");
                            break;

                        case TextAnchoringTypes.Bottom:
                            writer.WriteAttributeString("anchor", "b");
                            break;

                        case TextAnchoringTypes.Center:
                            writer.WriteAttributeString("anchor", "ctr");
                            break;

                        case TextAnchoringTypes.Distributed:
                            writer.WriteAttributeString("anchor", "dist");
                            break;

                        case TextAnchoringTypes.Justified:
                            writer.WriteAttributeString("anchor", "just");
                            break;
                    }
                    if (this.IsTextBoxAnchorCenter)
                    {
                        writer.WriteAttributeString("anchorCtr", "1");
                    }
                }
                using (writer.WriteElement("lstStyle", null, "a"))
                {
                }
                using (List<TextParagraph>.Enumerator enumerator = this.TextParagraphs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.WriteXml(writer, mFolder, chartFile);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies the bottom inset of the bounding rectangle.
        /// </summary>
        public double BottomInset { get; set; }

        /// <summary>
        /// Specifies the centering of the text box.
        /// </summary>
        public bool IsTextBoxAnchorCenter { get; set; }

        /// <summary>
        /// Specifies the left inset of the bounding rectangle.
        /// </summary>
        public double LeftInset { get; set; }

        /// <summary>
        /// Specifies the Right inset of the bounding rectangle.
        /// </summary>
        public double RightInset { get; set; }

        /// <summary>
        /// Specifies the rotation that is being applied to the text within the bounding box.
        /// </summary>
        public double Rotation { get; set; }

        /// <summary>
        /// Gets or sets the type of the text anchoring.
        /// </summary>
        /// <value>
        /// The type of the text anchoring.
        /// </value>
        public TextAnchoringTypes TextAnchoringType { get; set; }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box horizontally.
        /// </summary>
        public TextOverflowTypes TextHorizontalOverflow { get; set; }

        /// <summary>
        /// Specifies the text paragraph settings.
        /// </summary>
        public List<TextParagraph> TextParagraphs
        {
            get { return  this.textParagraphs; }
        }

        /// <summary>
        /// Determines whether the text can flow out of the bounding box vertically.
        /// </summary>
        public TextOverflowTypes TextVerticalOverflow { get; set; }

        /// <summary>
        /// Specifies the wrapping options to be used for this text body.
        /// </summary>
        public TextWrappingTypes TextWrappingType { get; set; }

        /// <summary>
        /// Specifies the top inset of the bounding rectangle.
        /// </summary>
        public double TopInset { get; set; }

        /// <summary>
        /// Specifies whether text should remain upright, regardless of the transform applied to it.
        /// </summary>
        public bool UpRight { get; set; }

        /// <summary>
        /// Determines if the text within the given text body should be displayed vertically.
        /// </summary>
        public TextVerticalTypes VerticalText { get; set; }
    }
}

