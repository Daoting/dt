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
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents a rich text settings.
    /// </summary>
    public class RichText
    {
        private List<TextParagraph> textParagraphs = new List<TextParagraph>();

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.Chart.RichText" /> class.
        /// </summary>
        public RichText()
        {
            this.ForceAntiAlias = false;
            this.FromWordArt = false;
            this.CompatibleLineSpacing = false;
            this.ColumnsRightToLeft = false;
            this.ParagraphSpacing = false;
            this.TextAnchoringType = TextAnchoringTypes.Top;
            this.SpaceBetweenColumns = 1;
            this.LeftInset = 7.2;
            this.RightInset = 7.2;
            this.TopInset = 3.6;
            this.UpRight = false;
            this.BottomInset = 3.6;
            this.NumberOfColumns = 1;
        }

        internal void ReadXml(XElement node, MemoryFolder mFolder, XFile xFile)
        {
            foreach (XElement element in node.Elements())
            {
                if (element.Name.LocalName == "bodyPr")
                {
                    string str = element.GetAttributeValueOrDefaultOfStringType("anchor", "t");
                    if (str == "b")
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Bottom;
                    }
                    else if (str == "ctr")
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Center;
                    }
                    else if (str == "dist")
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Distributed;
                    }
                    else if (str == "just")
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Justified;
                    }
                    else if (str == "t")
                    {
                        this.TextAnchoringType = TextAnchoringTypes.Top;
                    }
                    this.IsTextBoxAnchorCenter = element.GetAttributeValueOrDefaultOfBooleanType("anchorCtr", false);
                    this.CompatibleLineSpacing = element.GetAttributeValueOrDefaultOfBooleanType("compatLnSpc", false);
                    this.ForceAntiAlias = element.GetAttributeValueOrDefaultOfBooleanType("forceAA", false);
                    this.FromWordArt = element.GetAttributeValueOrDefaultOfBooleanType("fromWordArt", false);
                    if (element.GetAttributeValueOrDefaultOfStringType("horzOverflow", "overflow") == "clip")
                    {
                        this.TextHorizontalOverflow = Dt.Xls.Chart.TextHorizontalOverflow.Clip;
                    }
                    else
                    {
                        this.TextHorizontalOverflow = Dt.Xls.Chart.TextHorizontalOverflow.Overflow;
                    }
                    this.NumberOfColumns = element.GetAttributeValueOrDefaultOfInt32Type("numCol", 1);
                    this.Rotation = element.GetAttributeValueOrDefaultOfInt32Type("rot", 0) / 0xea60;
                    this.ColumnsRightToLeft = element.GetAttributeValueOrDefaultOfBooleanType("rtlCol", false);
                    this.SpaceBetweenColumns = element.GetAttributeValueOrDefaultOfInt32Type("spcCol", 1);
                    this.ParagraphSpacing = element.GetAttributeValueOrDefaultOfBooleanType("spcFirstLastPara", false);
                    this.UpRight = element.GetAttributeValueOrDefaultOfBooleanType("upright", false);
                    string str3 = element.GetAttributeValueOrDefaultOfStringType("vert", "horz");
                    if (str3 == "horz")
                    {
                        this.VerticalText = TextVerticalTypes.Horizontal;
                    }
                    else if (str3 == "eaVert")
                    {
                        this.VerticalText = TextVerticalTypes.EastAsianVertical;
                    }
                    else if (str3 == "mongolianVert")
                    {
                        this.VerticalText = TextVerticalTypes.MongolianVertical;
                    }
                    else if (str3 == "vert270")
                    {
                        this.VerticalText = TextVerticalTypes.Vertical270;
                    }
                    else if (str3 == "wordArtVert")
                    {
                        this.VerticalText = TextVerticalTypes.WordArtVertical;
                    }
                    else if (str3 == "wordArtVertRtl")
                    {
                        this.VerticalText = TextVerticalTypes.VerticalWordArtRightToLeft;
                    }
                    switch (element.GetAttributeValueOrDefaultOfStringType("vertOverflow", "overflow"))
                    {
                        case "clip":
                            this.TextVerticalOverflow = TextOverflowTypes.Clip;
                            break;

                        case "ellipsis":
                            this.TextVerticalOverflow = TextOverflowTypes.Ellipsis;
                            break;

                        case "overflow":
                            this.TextVerticalOverflow = TextOverflowTypes.Overflow;
                            break;
                    }
                    string str5 = element.GetAttributeValueOrDefaultOfStringType("wrap", "square");
                    if (str5 == "none")
                    {
                        this.TextWrappingType = TextWrappingTypes.None;
                    }
                    else if (str5 == "square")
                    {
                        this.TextWrappingType = TextWrappingTypes.Square;
                    }
                    this.LeftInset = (element.GetAttributeValueOrDefaultOfDoubleType("lIns", 91440.0) / 914400.0) * 72.0;
                    this.RightInset = (element.GetAttributeValueOrDefaultOfDoubleType("rIns", 91440.0) / 914400.0) * 72.0;
                    this.TopInset = (element.GetAttributeValueOrDefaultOfDoubleType("tIns", 45720.0) / 914400.0) * 72.0;
                    this.BottomInset = (element.GetAttributeValueOrDefaultOfDoubleType("bIns", 45720.0) / 914400.0) * 72.0;
                }
                if (element.Name.LocalName == "p")
                {
                    TextParagraph paragraph = new TextParagraph();
                    paragraph.ReadXml(element, mFolder, xFile);
                    this.TextParagraphs.Add(paragraph);
                }
            }
        }

        internal void WriteXml(XmlWriter writer, MemoryFolder mFolder, XFile chartFile)
        {
            using (writer.WriteElement("rich", null, "c"))
            {
                using (writer.WriteElement("bodyPr", null, "a"))
                {
                    switch (this.TextAnchoringType)
                    {
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
                    if (this.CompatibleLineSpacing)
                    {
                        writer.WriteAttributeString("compatLnSpc", "1");
                    }
                    if (this.ForceAntiAlias)
                    {
                        writer.WriteAttributeString("forceAA", "1");
                    }
                    if (this.FromWordArt)
                    {
                        writer.WriteAttributeString("fromWordArt", "1");
                    }
                    if (this.TextHorizontalOverflow == Dt.Xls.Chart.TextHorizontalOverflow.Clip)
                    {
                        writer.WriteAttributeString("horzOverflow", "clip");
                    }
                    if (this.Rotation != 0)
                    {
                        int num = this.Rotation * 0xea60;
                        writer.WriteAttributeString("rot", ((int) num).ToString());
                    }
                    if (this.ColumnsRightToLeft)
                    {
                        writer.WriteAttributeString("rtlCol", "1");
                    }
                    if (this.NumberOfColumns != 1)
                    {
                        writer.WriteAttributeString("numCol", ((int) this.NumberOfColumns).ToString());
                    }
                    if (this.SpaceBetweenColumns != 1)
                    {
                        writer.WriteAttributeString("spcCol", ((int) this.SpaceBetweenColumns).ToString());
                    }
                    if (this.ParagraphSpacing)
                    {
                        writer.WriteAttributeString("spcFirstLastPara", "1");
                    }
                    if (this.UpRight)
                    {
                        writer.WriteAttributeString("upright", "1");
                    }
                    switch (this.VerticalText)
                    {
                        case TextVerticalTypes.Vertical:
                            writer.WriteAttributeString("vert", "vert");
                            break;

                        case TextVerticalTypes.Vertical270:
                            writer.WriteAttributeString("vert", "vert270");
                            break;

                        case TextVerticalTypes.WordArtVertical:
                            writer.WriteAttributeString("vert", "wordArtVert");
                            break;

                        case TextVerticalTypes.VerticalWordArtRightToLeft:
                            writer.WriteAttributeString("vert", "wordArtVertRtl");
                            break;

                        case TextVerticalTypes.EastAsianVertical:
                            writer.WriteAttributeString("vert", "eaVert");
                            break;

                        case TextVerticalTypes.MongolianVertical:
                            writer.WriteAttributeString("vert", "mongolianVert");
                            break;
                    }
                    switch (this.TextVerticalOverflow)
                    {
                        case TextOverflowTypes.Clip:
                            writer.WriteAttributeString("vertOverflow", "clip");
                            break;

                        case TextOverflowTypes.Ellipsis:
                            writer.WriteAttributeString("vertOverflow", "ellipsis");
                            break;
                    }
                    if (this.TextWrappingType == TextWrappingTypes.None)
                    {
                        writer.WriteAttributeString("wrap", "none");
                    }
                    if (!(this.LeftInset - 7.2).IsZero())
                    {
                        int num4 = (((int) this.LeftInset) * 0xdf3e0) / 0x48;
                        writer.WriteAttributeString("lIns", ((int) num4).ToString());
                    }
                    if (!(this.RightInset - 7.2).IsZero())
                    {
                        int num5 = (((int) this.RightInset) * 0xdf3e0) / 0x48;
                        writer.WriteAttributeString("rIns", ((int) num5).ToString());
                    }
                    if (!(this.TopInset - 3.6).IsZero())
                    {
                        int num6 = (((int) this.TopInset) * 0xdf3e0) / 0x48;
                        writer.WriteAttributeString("tIns", ((int) num6).ToString());
                    }
                    if (!(this.BottomInset - 3.6).IsZero())
                    {
                        int num7 = (((int) this.BottomInset) * 0xdf3e0) / 0x48;
                        writer.WriteAttributeString("bIns", ((int) num7).ToString());
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
        /// Specifies whether columns are used in a right-to-left order.
        /// </summary>
        public bool ColumnsRightToLeft { get; set; }

        /// <summary>
        /// Specifies that the line spacing for this text body is decided in a simplistic manner using the font scene.
        /// </summary>
        public bool CompatibleLineSpacing { get; set; }

        /// <summary>
        /// Specifies whether forces the text to be rendered anti-aliased regardless of font size
        /// </summary>
        public bool ForceAntiAlias { get; set; }

        /// <summary>
        /// Specifies that text within this textbox is converted text from WordArt object.
        /// </summary>
        public bool FromWordArt { get; set; }

        /// <summary>
        /// Specifies the centering of the text box.
        /// </summary>
        public bool IsTextBoxAnchorCenter { get; set; }

        /// <summary>
        /// Specifies the left inset of the bounding rectangle .
        /// </summary>
        public double LeftInset { get; set; }

        /// <summary>
        /// Specifies the number of the columns of the text in the bounding rectangle.
        /// </summary>
        public int NumberOfColumns { get; set; }

        /// <summary>
        /// Specifies whether the before and after paragraph spacing defined by the user is to be respected.
        /// </summary>
        public bool ParagraphSpacing { get; set; }

        /// <summary>
        /// Specifies the right inset of the bounding rectangle.
        /// </summary>
        public double RightInset { get; set; }

        /// <summary>
        /// Specifies the rotation that is being applied to the text within the bounding box.
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Specifies the space between text columns in the text area.
        /// </summary>
        public int SpaceBetweenColumns { get; set; }

        /// <summary>
        /// Specifies the anchoring position of the text body withn the shap.
        /// </summary>
        public TextAnchoringTypes TextAnchoringType { get; set; }

        /// <summary>
        /// Gets or sets the text horizontal overflow.
        /// </summary>
        public Dt.Xls.Chart.TextHorizontalOverflow TextHorizontalOverflow { get; set; }

        /// <summary>
        /// Text paragraphs
        /// </summary>
        public List<TextParagraph> TextParagraphs
        {
            get { return  this.textParagraphs; }
        }

        /// <summary>
        /// Gets or sets the text vertical overflow.
        /// </summary>
        public TextOverflowTypes TextVerticalOverflow { get; set; }

        /// <summary>
        /// Text wrapping types.
        /// </summary>
        public TextWrappingTypes TextWrappingType { get; set; }

        /// <summary>
        /// Specifies the top inset of the bounding rectangle.
        /// </summary>
        public double TopInset { get; set; }

        /// <summary>
        /// Specifies whether text should remain upright, regardless of the transform applied to it and the accompanying shape transform.
        /// </summary>
        public bool UpRight { get; set; }

        /// <summary>
        /// Determines if the text within the given text body should be displayed vertically.
        /// </summary>
        public TextVerticalTypes VerticalText { get; set; }
    }
}

