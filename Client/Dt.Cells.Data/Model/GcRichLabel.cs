#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Pdf.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a rich label control.
    /// </summary>
    internal class GcRichLabel : GcPrintableControl
    {
        Brush background;
        const string Bold = "B";
        const string BoldString = "Bold";
        const string CenterPart = "C";
        string centerSource;
        Image cImage;
        const string ColorPrefix = "K";
        const string CommandPrefix = "&";
        const string CurrentPage = "P";
        const string Date = "D";
        const string FontPrefix = "\"";
        static Regex fontRegex;
        const string fontRegexName = "fontName";
        const string fontRegexStyle = "fontStyle";
        static Regex fontSizeRegex;
        const string fontSizeRegexNextIsNumber = "nextIsNumber";
        const string fontSizeRegexSize = "fontSize";
        Brush foreground;
        const string Italic = "I";
        const string ItalicString = "Italic";
        const string LeftPart = "L";
        string leftSource;
        Image lImage;
        const string NoneSpecified = "-";
        const string PageCount = "N";
        const string Picture = "G";
        const string RegularString = "Regular";
        static Regex rgbColorRegex;
        const string rgbColorRegexBlue = "blue";
        const string rgbColorRegexGreen = "green";
        const string rgbColorRegexRed = "red";
        const string RightPart = "R";
        string rightSource;
        Image rImage;
        const string Strikethrough = "S";
        static Regex themeColorRegex;
        const string themeColorRegexMethod = "method";
        const string themeColorRegexTheme = "theme";
        const string themeColorRegexValue = "value";
        const string Time = "T";
        const string Underline = "U";
        TextVerticalAlignment vAlignment;
        string workbookName;
        const string WorkbookNameCmd = "F";
        string worksheetName;
        const string WorksheetNameCmd = "A";

        /// <summary>
        /// Creates a new rich label.
        /// </summary>
        public GcRichLabel()
        {
            this.leftSource = string.Empty;
            this.centerSource = string.Empty;
            this.rightSource = string.Empty;
            this.foreground = FillEffects.Black;
        }

        /// <summary>
        /// Creates a new rich label with the specified <i>x</i> and <i>y</i> values, and width and height.
        /// </summary>
        /// <param name="x">The <i>x</i> value, in hundredths of an inch.</param>
        /// <param name="y">The <i>y</i> value, in hundredths of an inch.</param>
        /// <param name="width">The width, in hundredths of an inch.</param>
        /// <param name="height">The height, in hundredths of an inch.</param>
        public GcRichLabel(int x, int y, int width, int height) : base(x, y, width, height)
        {
            this.leftSource = string.Empty;
            this.centerSource = string.Empty;
            this.rightSource = string.Empty;
            this.foreground = FillEffects.Black;
        }

        /// <summary>
        /// Adjusts the line by part.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="context">The context.</param>
        /// <param name="part">The part.</param>
        /// <param name="includeY">if set to <c>true</c> [include Y].</param>
        /// <param name="allHeight">All height.</param>
        /// <param name="rect">The rect.</param>
        /// <returns></returns>
        internal static int AdjustLineByPart(List<GcPrintableControl> lists, GcReportContext context, int part, bool includeY, int allHeight, Windows.Foundation.Rect rect)
        {
            int num = 0;
            int num2 = 0;
            foreach (GcPrintableControl control in lists)
            {
                if (control is GcLabel)
                {
                    ((GcLabel)control).AutoSize(context);
                }
                control.X = num2;
                num2 += control.Width;
                if (includeY)
                {
                    control.Y = allHeight;
                    num = Math.Max(control.Height, num);
                }
            }
            switch (part)
            {
                case -1:
                    num2 = 0;
                    break;

                case 0:
                    num2 = (int)((rect.Width - num2) / 2.0);
                    break;

                case 1:
                    num2 = (int)(rect.Width - num2);
                    break;
            }
            foreach (GcPrintableControl control2 in lists)
            {
                control2.X += num2;
                if (includeY)
                {
                    control2.Y += num - control2.Height;
                }
            }
            return num;
        }

        /// <summary>
        /// Generates the range block.
        /// </summary>
        /// <param name="rLabel">The r label.</param>
        /// <param name="leftSource">The left source.</param>
        /// <param name="centerSource">The center source.</param>
        /// <param name="rightSource">The right source.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        static List<List<List<GcPrintableControl>>> GenerateRangeBlock(GcRichLabel rLabel, string leftSource, string centerSource, string rightSource, GcReportContext context)
        {
            int width = rLabel.Width;
            int height = rLabel.Height;
            if (rLabel.Padding.Horizontal > 0)
            {
                width -= rLabel.Padding.Horizontal;
            }
            if (rLabel.Padding.Vertical > 0)
            {
                width -= rLabel.Padding.Vertical;
            }
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect(0.0, 0.0, (double)width, (double)height);
            List<List<List<GcPrintableControl>>> list = new List<List<List<GcPrintableControl>>> { null, null, null };
            if ((!string.IsNullOrEmpty(leftSource) || !string.IsNullOrEmpty(centerSource)) || !string.IsNullOrEmpty(rightSource))
            {
                if ((rect.IsEmpty || (rect.Width == 0.0)) || (rect.Height == 0.0))
                {
                    return list;
                }
                string str = leftSource;
                string str2 = centerSource;
                string str3 = rightSource;
                if (!string.IsNullOrEmpty(str))
                {
                    list[0] = GenerateRangeBlockPart(rLabel, str, rect, -1, context);
                }
                if (!string.IsNullOrEmpty(str2))
                {
                    list[1] = GenerateRangeBlockPart(rLabel, str2, rect, 0, context);
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    list[2] = GenerateRangeBlockPart(rLabel, str3, rect, 1, context);
                }
            }
            return list;
        }

        /// <summary>
        /// Generates the range block line.
        /// </summary>
        /// <param name="rLabel">The r label.</param>
        /// <param name="source">The source.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="image">The image.</param>
        /// <param name="font">The font.</param>
        /// <param name="fontName">Name of the font.</param>
        /// <param name="isStrikethrough">if set to <c>true</c> [is strikethrough].</param>
        /// <param name="fontSize">Size of the font.</param>
        /// <param name="underlineType">Type of the underline.</param>
        /// <param name="isBold">if set to <c>true</c> [is bold].</param>
        /// <param name="isItalic">if set to <c>true</c> [is italic].</param>
        /// <param name="foreground">The foreground.</param>
        /// <returns></returns>
        static List<GcPrintableControl> GenerateRangeBlockLine(GcRichLabel rLabel, string source, Windows.Foundation.Rect rect, Image image, ref Font font, ref string fontName, ref bool isStrikethrough, ref double fontSize, ref UnderlineType underlineType, ref bool isBold, ref bool isItalic, ref Brush foreground)
        {
            List<GcPrintableControl> list = new List<GcPrintableControl>();
            if ((!string.IsNullOrEmpty(source) && !rect.IsEmpty) && ((rect.Width != 0.0) && (rect.Height != 0.0)))
            {
                string str = string.Empty;
                while (!string.IsNullOrEmpty(source))
                {
                    string str2 = fontName;
                    bool flag = isStrikethrough;
                    double num = fontSize;
                    UnderlineType single = underlineType;
                    bool flag2 = isBold;
                    bool flag3 = isItalic;
                    Brush foregroundNext = foreground;
                    int length = -1;
                    length = source.IndexOf("&");
                    if (length < 0)
                    {
                        length = source.Length;
                    }
                    str = str + source.Substring(0, length);
                    string str3 = ((length + 1) < source.Length) ? source.Substring(length + 1, 1) : string.Empty;
                    int startIndex = -1;
                    bool flag4 = false;
                    PageInfoType pageNumber = PageInfoType.PageNumber;
                    bool flag5 = false;
                    int num4 = 0;
                    try
                    {
                        Match match;
                        switch (str3)
                        {
                            case "1":
                            case "2":
                            case "3":
                            case "4":
                            case "5":
                            case "6":
                            case "7":
                            case "8":
                            case "9":
                            case "0":
                                match = FontSizeRegex.Match(source.Substring(length));
                                if ((!match.Success || !match.Groups["fontSize"].Success) || string.IsNullOrEmpty(match.Groups["fontSize"].Value))
                                {
                                    goto Label_079D;
                                }
                                num = int.Parse(match.Groups["fontSize"].Value);
                                flag4 = true;
                                num4 = 0;
                                startIndex = (length + "&".Length) + match.Groups["fontSize"].Value.Length;
                                if (match.Groups["nextIsNumber"].Success)
                                {
                                    startIndex++;
                                }
                                goto Label_07C0;

                            case "K":
                                {
                                    match = RgbColorRegex.Match(source.Substring(length));
                                    if (!match.Success)
                                    {
                                        break;
                                    }
                                    byte r = byte.Parse(match.Groups["red"].Value, (NumberStyles)NumberStyles.AllowHexSpecifier);
                                    byte g = byte.Parse(match.Groups["green"].Value, (NumberStyles)NumberStyles.AllowHexSpecifier);
                                    byte b = byte.Parse(match.Groups["blue"].Value, (NumberStyles)NumberStyles.AllowHexSpecifier);
                                    foregroundNext = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, r, g, b));
                                    flag4 = true;
                                    num4 = 0;
                                    startIndex = ((length + "&".Length) + "K".Length) + 6;
                                    goto Label_07C0;
                                }
                            case "S":
                                flag = !flag;
                                flag4 = true;
                                num4 = 0;
                                startIndex = (length + "&".Length) + "S".Length;
                                goto Label_07C0;

                            case "U":
                                if (single != UnderlineType.None)
                                {
                                    goto Label_04DA;
                                }
                                single = UnderlineType.Single;
                                goto Label_04E2;

                            case "\"":
                                match = FontRegex.Match(source.Substring(length));
                                if (match.Success)
                                {
                                    string str4 = match.Groups["fontName"].Value;
                                    string str5 = match.Groups["fontStyle"].Value;
                                    if (str4.IndexOf("-") < 0)
                                    {
                                        str2 = str4;
                                        flag4 = true;
                                        num4 = 0;
                                    }
                                    if (str5.IndexOf("-") < 0)
                                    {
                                        if (str5.IndexOf("Bold") >= 0)
                                        {
                                            flag2 = true;
                                        }
                                        if (str5.IndexOf("Italic") >= 0)
                                        {
                                            flag3 = true;
                                        }
                                        if (str5.IndexOf("Regular") >= 0)
                                        {
                                            flag2 = flag3 = false;
                                        }
                                        flag4 = true;
                                        num4 = 0;
                                    }
                                    startIndex = length + match.Value.Length;
                                }
                                goto Label_07C0;

                            case "B":
                                flag2 = !flag2;
                                flag4 = true;
                                num4 = 0;
                                startIndex = (length + "&".Length) + "B".Length;
                                goto Label_07C0;

                            case "I":
                                flag3 = !flag3;
                                flag4 = true;
                                num4 = 0;
                                startIndex = (length + "&".Length) + "I".Length;
                                goto Label_07C0;

                            case "&":
                                str = str + "&";
                                flag4 = true;
                                num4 = 0;
                                startIndex = length + ("&".Length * 2);
                                goto Label_07C0;

                            case "D":
                                str = str + "{0}";
                                flag4 = true;
                                num4 = 1;
                                pageNumber = PageInfoType.Date;
                                startIndex = (length + "&".Length) + "D".Length;
                                goto Label_07C0;

                            case "T":
                                str = str + "{0}";
                                flag4 = true;
                                num4 = 1;
                                pageNumber = PageInfoType.Time;
                                startIndex = (length + "&".Length) + "T".Length;
                                goto Label_07C0;

                            case "P":
                                str = str + "{0}";
                                flag4 = true;
                                num4 = 1;
                                pageNumber = PageInfoType.PageNumber;
                                startIndex = (length + "&".Length) + "P".Length;
                                goto Label_07C0;

                            case "N":
                                str = str + "{0}";
                                flag4 = true;
                                num4 = 1;
                                pageNumber = PageInfoType.PageCount;
                                startIndex = (length + "&".Length) + "N".Length;
                                goto Label_07C0;

                            case "G":
                                if (image != null)
                                {
                                    flag4 = true;
                                    num4 = 0;
                                    flag5 = true;
                                }
                                startIndex = (length + "&".Length) + "G".Length;
                                goto Label_07C0;

                            case "F":
                                str = str + rLabel.WorkbookName;
                                startIndex = (length + "&".Length) + "F".Length;
                                goto Label_07C0;

                            case "A":
                                str = str + rLabel.WorksheetName;
                                startIndex = (length + "&".Length) + "A".Length;
                                goto Label_07C0;

                            default:
                                goto Label_079D;
                        }
                        if (!ThemeColorRegex.Match(source.Substring(length)).Success)
                        {
                            goto Label_079D;
                        }
                        foregroundNext = rLabel.Foreground;
                        flag4 = true;
                        num4 = 0;
                        startIndex = ((length + "&".Length) + "K".Length) + 5;
                        goto Label_07C0;
                    Label_04DA:
                        if (single == UnderlineType.Single)
                        {
                            single = UnderlineType.None;
                        }
                    Label_04E2:
                        flag4 = true;
                        num4 = 0;
                        startIndex = (length + "&".Length) + "U".Length;
                        goto Label_07C0;
                    Label_079D:
                        startIndex = length + "&".Length;
                    }
                    catch
                    {
                        startIndex = length + "&".Length;
                    }
                Label_07C0:
                    if (startIndex >= source.Length)
                    {
                        source = string.Empty;
                    }
                    else
                    {
                        source = source.Substring(startIndex);
                    }
                    if (!string.IsNullOrEmpty(str) && (flag4 || string.IsNullOrEmpty(source)))
                    {
                        GcLabel label = null;
                        switch (num4)
                        {
                            case 0:
                                label = new GcLabel(str);
                                break;

                            case 1:
                                {
                                    GcPageInfo info = new GcPageInfo
                                    {
                                        Format = str,
                                        Type = pageNumber
                                    };
                                    label = info;
                                    break;
                                }
                        }
                        label.CanGrow = false;
                        label.CanShrink = false;
                        label.Foreground = foreground;
                        if ((((font.FontFamilyName != fontName) || (font.Strikeout != isStrikethrough)) || ((font.FontSize != fontSize) || (font.Underline != underlineType))) || ((font.Bold != isBold) || (font.Italic != isItalic)))
                        {
                            font = new Font(fontName, fontSize);
                            font.Strikeout = isStrikethrough;
                            font.Bold = isBold;
                            font.Italic = isItalic;
                            font.Underline = underlineType;
                        }
                        label.Alignment.WordWrap = false;
                        label.Font = font;
                        list.Add(label);
                        str = string.Empty;
                    }
                    if (flag5 && (image != null))
                    {
                        GcImage image2 = new GcImage(image);
                        list.Add(image2);
                        flag5 = false;
                    }
                    fontName = str2;
                    isStrikethrough = flag;
                    fontSize = num;
                    underlineType = single;
                    isBold = flag2;
                    isItalic = flag3;
                    foreground = foregroundNext;
                }
            }
            return list;
        }

        /// <summary>
        /// Generates the range block part.
        /// </summary>
        /// <param name="rLabel">The r label.</param>
        /// <param name="source">The source.</param>
        /// <param name="rect">The rect.</param>
        /// <param name="part">The part.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        static List<List<GcPrintableControl>> GenerateRangeBlockPart(GcRichLabel rLabel, string source, Windows.Foundation.Rect rect, int part, GcReportContext context)
        {
            List<List<GcPrintableControl>> list = new List<List<GcPrintableControl>>();
            if ((!string.IsNullOrEmpty(source) && !rect.IsEmpty) && ((rect.Width != 0.0) && (rect.Height != 0.0)))
            {
                string str;
                StringReader reader = new StringReader(source);
                int allHeight = 0;
                Font font = rLabel.Font ?? context.DefaultFont;
                string fontFamilyName = font.FontFamilyName;
                bool strikeout = font.Strikeout;
                double fontSize = font.FontSize;
                UnderlineType underline = font.Underline;
                bool bold = font.Bold;
                bool italic = font.Italic;
                Brush foreground = rLabel.Foreground;
                Image leftImage = null;
                switch (part)
                {
                    case -1:
                        leftImage = rLabel.LeftImage;
                        break;

                    case 0:
                        leftImage = rLabel.CenterImage;
                        break;

                    case 1:
                        leftImage = rLabel.RightImage;
                        break;
                }
                while ((str = reader.ReadLine()) != null)
                {
                    List<GcPrintableControl> lists = GenerateRangeBlockLine(rLabel, str, rect, leftImage, ref font, ref fontFamilyName, ref strikeout, ref fontSize, ref underline, ref bold, ref italic, ref foreground);
                    int num3 = AdjustLineByPart(lists, context, part, true, allHeight, rect);
                    list.Add(lists);
                    if ((num3 <= 0) && (lists.Count <= 0))
                    {
                        num3 = (int)Math.Ceiling(context.MeasureNoWrapString("X", font).Height);
                    }
                    allHeight += num3;
                }
                int num4 = 0;
                switch (rLabel.VerticalAlignment)
                {
                    case TextVerticalAlignment.General:
                    case TextVerticalAlignment.Top:
                    case TextVerticalAlignment.Justify:
                        break;

                    case TextVerticalAlignment.Center:
                    case TextVerticalAlignment.Distributed:
                        num4 = (int)((rect.Height - allHeight) / 2.0);
                        break;

                    case TextVerticalAlignment.Bottom:
                        num4 = (int)(rect.Height - allHeight);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if (num4 != 0)
                {
                    using (List<List<GcPrintableControl>>.Enumerator enumerator = list.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            foreach (GcPrintableControl local1 in enumerator.Current)
                            {
                                local1.Y += num4;
                            }
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Internal only.
        /// Gets the block.
        /// </summary>
        /// <param name="context">The context</param>
        /// <returns></returns>
        internal override GcBlock GetBlock(GcReportContext context)
        {
            GcBlock block = base.GetBlock(context);
            block.Cache = GenerateRangeBlock(this, this.leftSource, this.centerSource, this.rightSource, context);
            return block;
        }

        /// <summary>
        /// Gets or sets the background of the rich label.
        /// </summary>
        /// <value>
        /// A <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that specifies the background for the rich label.
        /// The default value is null.
        /// </value>
        [DefaultValue((string)null)]
        public Brush Background
        {
            get { return this.background; }
            set { this.background = value; }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcControl" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanGrow
        {
            get { return false; }
            set
            {
            }
        }

        /// <summary>
        /// Overrides the <see cref="T:Dt.Cells.Data.GcControl" /> property.
        /// </summary>
        /// <value>
        /// This property is always <c>false</c>.
        /// </value>
        /// <remarks>This property is read-only.</remarks>
        public override bool CanShrink
        {
            get { return false; }
            set
            {
            }
        }

        /// <summary>
        /// Gets or sets the center part of the image.
        /// </summary>
        /// <value>The image for the center section of the rich label. The default value is null.</value>
        [DefaultValue((string)null)]
        public Image CenterImage
        {
            get { return this.cImage; }
            set { this.cImage = value; }
        }

        /// <summary>
        /// Gets or sets the middle source of the rich label.
        /// </summary>
        /// <value>The middle source. The default value is an empty string.</value>
        [DefaultValue("")]
        public string CenterSource
        {
            get { return this.centerSource; }
            set { this.centerSource = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets the font regex.
        /// </summary>
        /// <value>The font regex.</value>
        static Regex FontRegex
        {
            get
            {
                if (fontRegex == null)
                {
                    fontRegex = new Regex(string.Format("^{5}\"\\s*(?<{3}>-|[^-,\"]+),\\s*(?<{4}>((({0}|{1}|{2})\\s+)*({0}|{1}|{2}))*|-)\\s*\"", (object[])new object[] { "Regular", "Bold", "Italic", "fontName", "fontStyle", "&" }));
                }
                return fontRegex;
            }
        }

        /// <summary>
        /// Internal only.
        /// Gets the font size regex.
        /// </summary>
        /// <value>The font size regex.</value>
        static Regex FontSizeRegex
        {
            get
            {
                if (fontSizeRegex == null)
                {
                    fontSizeRegex = new Regex(string.Format(@"^{0}(?<{1}>[0-9]+)(?<{2}>\s[0-9])?", (object[])new object[] { "&", "fontSize", "nextIsNumber" }));
                }
                return fontSizeRegex;
            }
        }

        /// <summary>
        /// Gets or sets the foreground of the rich label.
        /// </summary>
        /// <value>
        /// A <see cref="T:Windows.UI.Xaml.Media.Brush" /> object that specifies the foreground for the rich label.
        /// The default value is black.
        /// </value>
        public Brush Foreground
        {
            get { return this.foreground; }
            set { this.foreground = value; }
        }

        /// <summary>
        /// Gets or sets the left part of the image.
        /// </summary>
        /// <value>The image for the left section of the rich label. The default value is null.</value>
        [DefaultValue((string)null)]
        public Image LeftImage
        {
            get { return this.lImage; }
            set { this.lImage = value; }
        }

        /// <summary>
        /// Gets or sets the left source of the rich label.
        /// </summary>
        /// <value>The left source. The default value is an empty string.</value>
        [DefaultValue("")]
        public string LeftSource
        {
            get { return this.leftSource; }
            set { this.leftSource = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets the font regex.
        /// </summary>
        /// <value>The font regex.</value>
        static Regex RgbColorRegex
        {
            get
            {
                if (rgbColorRegex == null)
                {
                    rgbColorRegex = new Regex(string.Format("^{0}{1}(?<{2}>[0-9ABCDEFabcdef][0-9ABCDEFabcdef])(?<{3}>[0-9ABCDEFabcdef][0-9ABCDEFabcdef])(?<{4}>[0-9ABCDEFabcdef][0-9ABCDEFabcdef])", (object[])new object[] { "&", "K", "red", "green", "blue" }));
                }
                return rgbColorRegex;
            }
        }

        /// <summary>
        /// Gets or sets the right part of the image.
        /// </summary>
        /// <value>The image for the right section of the rich label. The default value is null.</value>
        [DefaultValue((string)null)]
        public Image RightImage
        {
            get { return this.rImage; }
            set { this.rImage = value; }
        }

        /// <summary>
        /// Gets or sets the right source of the rich label.
        /// </summary>
        /// <value>The right source. The default value is an empty string.</value>
        [DefaultValue("")]
        public string RightSource
        {
            get { return this.rightSource; }
            set { this.rightSource = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets the font regex.
        /// </summary>
        /// <value>The font regex.</value>
        static Regex ThemeColorRegex
        {
            get
            {
                if (themeColorRegex == null)
                {
                    themeColorRegex = new Regex(string.Format(@"^{0}{1}(?<{2}>[0-9ABCDEFabcdef][0-9ABCDEFabcdef])(?<{3}>(\+|\-))(?<{4}>[0-9ABCDEFabcdef][0-9ABCDEFabcdef])", (object[])new object[] { "&", "K", "theme", "method", "value" }));
                }
                return themeColorRegex;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the text in the rich label.
        /// </summary>
        /// <value>
        /// The vertical alignment of the text.
        /// The default value is <see cref="T:Dt.Cells.Data.TextVerticalAlignment">General</see>.
        /// </value>
        [DefaultValue(0)]
        public TextVerticalAlignment VerticalAlignment
        {
            get { return this.vAlignment; }
            set { this.vAlignment = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the name of the workbook.
        /// </summary>
        /// <value>The name of the workbook.</value>
        internal string WorkbookName
        {
            get { return this.workbookName; }
            set { this.workbookName = value; }
        }

        /// <summary>
        /// Internal only.
        /// Gets or sets the name of the worksheet.
        /// </summary>
        /// <value>The name of the worksheet.</value>
        internal string WorksheetName
        {
            get { return this.worksheetName; }
            set { this.worksheetName = value; }
        }
    }
}

