#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    internal class Font : ICloneable
    {
        FontFamily fontFamily;
        string fontFamilyName;
        double? fontSize;
        UnitType? fontSizeUnit;
        FontStretch? fontStretch;
        Windows.UI.Text.FontStyle? fontStyle;
        FontWeight? fontWeight;
        byte? gdiCharSet;
        bool? strikeout;
        UnderlineType? underline;

        public Font()
        {
            this.fontSize = null;
            this.fontSizeUnit = null;
            this.gdiCharSet = null;
            this.fontStretch = null;
            this.fontWeight = null;
            this.fontStyle = null;
            this.underline = null;
            this.strikeout = null;
            this.fontFamily = null;
        }

        public Font(string familyName, double fontSize)
        {
            this.underline = null;
            this.strikeout = null;
            this.fontFamilyName = familyName;
            this.fontSize = new double?(fontSize);
            this.fontSizeUnit = null;
            this.fontStyle = null;
            this.fontWeight = null;
            this.fontStretch = null;
            this.gdiCharSet = null;
        }

        public Font(string familyName, double fontSize, UnitType fontSizeUnit)
        {
            this.underline = null;
            this.strikeout = null;
            this.fontFamilyName = familyName;
            this.fontSize = new double?(fontSize);
            this.fontSizeUnit = new UnitType?(fontSizeUnit);
            this.fontStyle = null;
            this.fontWeight = null;
            this.fontStretch = null;
            this.gdiCharSet = null;
        }

        public Font(string familyName, double fontSize, OldFontStyle fontStyleType)
        {
            this.underline = null;
            this.strikeout = null;
            this.fontFamilyName = familyName;
            this.fontSize = new double?(fontSize);
            this.fontSizeUnit = null;
            this.fontStyle = null;
            this.fontWeight = null;
            this.fontStretch = null;
            this.gdiCharSet = null;
            this.FontStyleType = fontStyleType;
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Bold
        {
            get
            {
                if (this.fontWeight.HasValue)
                {
                    return ((this.fontWeight.Value.Equals(FontWeights.Black) || this.fontWeight.Value.Equals(FontWeights.Bold)) || (this.fontWeight.Value.Equals(FontWeights.ExtraBlack) || this.fontWeight.Value.Equals(FontWeights.ExtraBold))) || this.fontWeight.Value.Equals(FontWeights.SemiBold);
                }
                return false;
            }
            set
            {
                if (this.Bold != value)
                {
                    if (value)
                    {
                        this.fontWeight = new FontWeight?(FontWeights.Bold);
                    }
                    else
                    {
                        this.fontWeight = new FontWeight?(FontWeights.Normal);
                    }
                    this.ClearFontCached();
                }
            }
        }
        
        public FontFamily FontFamily
        {
            get
            {
                if (this.fontFamily == null)
                {
                    string actualName = (this.fontFamilyName != null) ? this.fontFamilyName : DefaultStyleCollection.DefaultFontName;
                    this.fontFamily = new FontFamily(actualName);
                }
                return this.fontFamily;
            }
            set
            {
                if (this.fontFamily != value)
                {
                    this.fontFamily = value;
                    if (this.fontFamily != null)
                    {
                        this.fontFamilyName = this.fontFamily.Source;
                    }
                    this.ClearFontCached();
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        [DefaultValue("")]
        public string FontFamilyName
        {
            get
            {
                string fontFamilyName = string.Empty;
                if (this.fontFamily != null)
                {
                    fontFamilyName = this.fontFamily.Source;
                }
                if (!string.IsNullOrEmpty(fontFamilyName))
                {
                    return fontFamilyName;
                }
                if (this.fontFamilyName != null)
                {
                    fontFamilyName = this.fontFamilyName;
                }
                if (!string.IsNullOrEmpty(fontFamilyName))
                {
                    return fontFamilyName;
                }
                return DefaultStyleCollection.DefaultFontName;
            }
            set
            {
                this.fontFamilyName = value;
                this.fontFamily = null;
                this.ClearFontCached();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double FontSize
        {
            get
            {
                if (this.fontSize.HasValue)
                {
                    return this.fontSize.Value;
                }
                return DefaultStyleCollection.DefaultFontSize;
            }
            set
            {
                if (value < 1.0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                double? fontSize = this.fontSize;
                double num = value;
                if ((((double)fontSize.GetValueOrDefault()) != num) || !fontSize.HasValue)
                {
                    this.fontSize = new double?(value);
                    this.ClearFontCached();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public UnitType FontSizeUnit
        {
            get
            {
                if (this.fontSizeUnit.HasValue)
                {
                    return this.fontSizeUnit.Value;
                }
                return UnitType.Point;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public OldFontStyle FontStyleType
        {
            get
            {
                OldFontStyle regular = OldFontStyle.Regular;
                if (this.Bold)
                {
                    regular |= OldFontStyle.Bold;
                }
                if (this.Italic)
                {
                    regular |= OldFontStyle.Italic;
                }
                if (this.Strikeout)
                {
                    regular |= OldFontStyle.Strikeout;
                }
                if (this.Underline == UnderlineType.Single)
                {
                    regular |= OldFontStyle.Underline;
                }
                return regular;
            }
            set
            {
                this.Bold = (value & OldFontStyle.Bold) > OldFontStyle.Regular;
                this.Italic = (value & OldFontStyle.Italic) > OldFontStyle.Regular;
                this.Strikeout = (value & OldFontStyle.Strikeout) > OldFontStyle.Regular;
                this.Underline = ((value & OldFontStyle.Underline) > OldFontStyle.Regular) ? UnderlineType.Single : UnderlineType.None;
                this.ClearFontCached();
            }
        }

        public FontStretch FontStretch
        {
            get
            {
                if (this.fontStretch.HasValue)
                {
                    return this.fontStretch.Value;
                }
                return FontStretch.Normal;
            }
            set
            {
                FontStretch? stretch = this.fontStretch;
                if ((((FontStretch)stretch.GetValueOrDefault()) != value) || !stretch.HasValue)
                {
                    this.fontStretch = value;
                    this.ClearFontCached();
                }
            }
        }

        public Windows.UI.Text.FontStyle FontStyle
        {
            get
            {
                if (this.fontStyle.HasValue)
                {
                    return this.fontStyle.Value;
                }
                return Windows.UI.Text.FontStyle.Normal;
            }
            set
            {
                Windows.UI.Text.FontStyle? style = this.fontStyle;
                if ((((Windows.UI.Text.FontStyle)style.GetValueOrDefault()) != value) || !style.HasValue)
                {
                    this.fontStyle = value;
                    this.ClearFontCached();
                }
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                if (this.fontWeight.HasValue)
                {
                    return this.fontWeight.Value;
                }
                return FontWeights.Normal;
            }
            set
            {
                FontWeight? weight = this.fontWeight;
                if (!weight.HasValue || !weight.Value.Equals(value))
                {
                    this.fontWeight = value;
                    this.ClearFontCached();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(1)]
        public byte GdiCharSet
        {
            get
            {
                if (this.gdiCharSet.HasValue)
                {
                    return this.gdiCharSet.Value;
                }
                return 1;
            }
            set
            {
                byte? gdiCharSet = this.gdiCharSet;
                int num = value;
                if ((gdiCharSet.GetValueOrDefault() != num) || !gdiCharSet.HasValue)
                {
                    this.gdiCharSet = new byte?(value);
                    this.ClearFontCached();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Italic
        {
            get { return  (this.fontStyle.HasValue && (this.fontStyle.Value == Windows.UI.Text.FontStyle.Italic)); }
            set
            {
                if (this.Italic != value)
                {
                    if (value)
                    {
                        this.fontStyle = Windows.UI.Text.FontStyle.Italic;
                    }
                    else
                    {
                        this.fontStyle = Windows.UI.Text.FontStyle.Normal;
                    }
                    this.ClearFontCached();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(false)]
        public bool Strikeout
        {
            get
            {
                if (this.strikeout.HasValue)
                {
                    return this.strikeout.Value;
                }
                return false;
            }
            set
            {
                this.strikeout = new bool?(value);
                this.ClearFontCached();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DefaultValue(0)]
        public UnderlineType Underline
        {
            get
            {
                if (this.underline.HasValue)
                {
                    return this.underline.Value;
                }
                return UnderlineType.None;
            }
            set
            {
                this.underline = new UnderlineType?(value);
                this.ClearFontCached();
            }
        }

        void ClearFontCached()
        {
        }

        public object Clone()
        {
            return new Font { fontFamily = this.fontFamily, fontFamilyName = this.fontFamilyName, fontSize = this.fontSize, fontSizeUnit = this.fontSizeUnit, fontStyle = this.fontStyle, fontWeight = this.fontWeight, fontStretch = this.fontStretch, underline = this.underline, gdiCharSet = this.gdiCharSet, strikeout = this.strikeout };
        }

        public static Font Create(StyleInfo style)
        {
            Font font = new Font();
            if ((!style.IsFontFamilySet() && !style.IsFontSizeSet()) && ((!style.IsFontStretchSet() && !style.IsFontStyleSet()) && !style.IsFontWeightSet()))
            {
                return null;
            }
            if ((style != null) && (style.FontFamily != null))
            {
                string familyName = FontHelper.GetEquivalentEnglishFontName(style.FontFamily.Source);
                font.FontFamily = new FontFamily(familyName);
            }
            font.FontSize = style.FontSize;
            font.FontStretch = style.FontStretch;
            font.FontStyle = style.FontStyle;
            font.FontWeight = style.FontWeight;
            font.gdiCharSet = 0;
            return font;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Font))
            {
                return false;
            }
            Font font = obj as Font;
            if (font.FontFamilyName != this.FontFamilyName)
            {
                return false;
            }
            double? fontSize = font.fontSize;
            double? nullable2 = this.fontSize;
            if ((((double)fontSize.GetValueOrDefault()) != ((double)nullable2.GetValueOrDefault())) || (fontSize.HasValue != nullable2.HasValue))
            {
                return false;
            }
            UnitType? fontSizeUnit = font.fontSizeUnit;
            UnitType? nullable4 = this.fontSizeUnit;
            if ((fontSizeUnit.GetValueOrDefault() != nullable4.GetValueOrDefault()) || (fontSizeUnit.HasValue != nullable4.HasValue))
            {
                return false;
            }
            Windows.UI.Text.FontStyle? fontStyle = font.fontStyle;
            Windows.UI.Text.FontStyle? nullable6 = this.fontStyle;
            if ((fontStyle.GetValueOrDefault() != nullable6.GetValueOrDefault()) || (fontStyle.HasValue != nullable6.HasValue))
            {
                return false;
            }
            if (!font.fontWeight.Equals(this.fontWeight))
            {
                return false;
            }
            FontStretch? fontStretch = font.fontStretch;
            FontStretch? nullable8 = this.fontStretch;
            if ((fontStretch.GetValueOrDefault() != nullable8.GetValueOrDefault()) || (fontStretch.HasValue != nullable8.HasValue))
            {
                return false;
            }
            byte? gdiCharSet = font.gdiCharSet;
            byte? nullable10 = this.gdiCharSet;
            if ((gdiCharSet.GetValueOrDefault() != nullable10.GetValueOrDefault()) || (gdiCharSet.HasValue != nullable10.HasValue))
            {
                return false;
            }
            bool? strikeout = font.strikeout;
            bool? nullable12 = this.strikeout;
            if ((strikeout.GetValueOrDefault() != nullable12.GetValueOrDefault()) || (strikeout.HasValue != nullable12.HasValue))
            {
                return false;
            }
            UnderlineType? underline = font.underline;
            UnderlineType? nullable14 = this.underline;
            if ((underline.GetValueOrDefault() != nullable14.GetValueOrDefault()) || (underline.HasValue != nullable14.HasValue))
            {
                return false;
            }
            return true;
        }

        public float GetFontSize(UnitType unitType, int dpi)
        {
            return (float)UnitManager.ConvertTo(this.FontSize, UnitType.Pixel, unitType, (float)dpi);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void SetFontSize(UnitType unitType, int dpi, double size)
        {
            this.FontSize = (float)UnitManager.ConvertTo(size, unitType, this.FontSizeUnit, (float)dpi);
        }
    }

    /// <summary>
    /// Specifies style information applied to text.
    /// </summary>
    [Flags]
    internal enum OldFontStyle
    {
        /// <summary>
        /// [1] Shows the bold text.
        /// </summary>
        Bold = 1,
        /// <summary>
        /// [2] Shows the italic text.
        /// </summary>
        Italic = 2,
        /// <summary>
        /// [0] Shows the normal text.
        /// </summary>
        Regular = 0,
        /// <summary>
        /// [8] Shows the text with a line through the middle.
        /// </summary>
        Strikeout = 8,
        /// <summary>
        /// [4] Shows the text with an underline.
        /// </summary>
        Underline = 4
    }
}