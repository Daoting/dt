#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the Excel Font settings.
    /// </summary>
    public sealed class ExcelFont : IExcelFont, IEquatable<IExcelFont>
    {
        private ExcelFont _default;
        private double _fontSize;

        internal ExcelFont()
        {
            this._fontSize = 11.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExcelFont" /> class.
        /// </summary>
        /// <param name="name">The font name</param>
        /// 
        /// <param name="fontColor">The font color.</param>
        /// <param name="fontFamily">The font family.</param>
        public ExcelFont(string name, ExcelColor fontColor, ExcelFontFamily fontFamily)
        {
            this._fontSize = 11.0;
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name");
            }
            this.FontColor = fontColor;
            this.FontName = name;
            this.FontFamily = this.FontFamily;
        }

        /// <summary>
        /// create a new <see cref="T:Dt.Xls.ExcelFont" /> based on the current instance.
        /// </summary>
        /// <returns>
        /// a new <see cref="T:Dt.Xls.ExcelFont" /> with the same property compared with the current instance.
        /// </returns>
        public IExcelFont Clone()
        {
            return new ExcelFont { IsAutoColor = this.IsAutoColor, IsBold = this.IsBold, IsItalic = this.IsItalic, IsOutlineStyle = this.IsOutlineStyle, IsShadowStyle = this.IsShadowStyle, IsStrikeOut = this.IsStrikeOut, FontName = this.FontName, FontFamily = this.FontFamily, FontScheme = this.FontScheme, FontColor = this.FontColor, FontSize = this.FontSize, CharSetIndex = this.CharSetIndex, UnderLineStyle = this.UnderLineStyle, VerticalAlignRun = this.VerticalAlignRun };
        }

        /// <summary>
        /// Determines whether this instance is equals with the specific <see cref="T:Dt.Xls.IExcelFont" /> instance
        /// </summary>
        /// <param name="right">The other instance used to compared with the current instance.</param>
        /// <returns></returns>
        public bool Equals(IExcelFont right)
        {
            if (object.ReferenceEquals(this, right))
            {
                return true;
            }
            if (right == null)
            {
                return false;
            }
            if ((((((this.IsBold != right.IsBold) || (this.IsItalic != right.IsItalic)) || ((this.IsOutlineStyle != right.IsOutlineStyle) || (this.IsShadowStyle != right.IsShadowStyle))) || (((this.IsStrikeOut != right.IsStrikeOut) || (this.FontName != right.FontName)) || ((this.FontFamily != right.FontFamily) || (this.FontSize != right.FontSize)))) || (((this.CharSetIndex != right.CharSetIndex) || (this.UnderLineStyle != right.UnderLineStyle)) || (this.VerticalAlignRun != right.VerticalAlignRun))) || (this.FontScheme != right.FontScheme))
            {
                return false;
            }
            if ((this.FontColor == null) && (right.FontColor == null))
            {
                return true;
            }
            if ((this.FontColor == null) && (right.FontColor != null))
            {
                return false;
            }
            if ((this.FontColor != null) && (right.FontColor == null))
            {
                return false;
            }
            return this.FontColor.Equals(right.FontColor);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with this instance.</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                if (object.ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj is ExcelFont)
                {
                    ExcelFont font = obj as ExcelFont;
                    return (this == font);
                }
            }
            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int num = (((double) this.FontSize).GetHashCode() ^ (this.IsBold.GetHashCode() << 4)) ^ (this.IsItalic.GetHashCode() << 8);
            if (this.FontName != null)
            {
                num ^= this.FontName.GetHashCode() << 12;
            }
            if (this.FontColor != null)
            {
                num ^= this.FontColor.GetHashCode() << 0x10;
            }
            return num;
        }

        private void InitOrResetLanguageRelatedFontInfo(ExcelFont font, string language)
        {
            if (language == "ja-Jp")
            {
                font.FontSize = 11.0;
                font.FontFamily = ExcelFontFamily.Modern;
                font.FontColor = new ExcelColor(ExcelPaletteColor.Black);
                font.FontName = "lr oSVbN";
                font.CharSetIndex = 0x80;
            }
            else
            {
                font.FontSize = 11.0;
                font.FontFamily = ExcelFontFamily.Swiss;
                font.FontColor = new ExcelColor(ExcelPaletteColor.Black);
                font.FontName = "Calibri";
                font.CharSetIndex = 0;
            }
        }

        /// <summary>
        /// Gets or sets the index of the char set.
        /// </summary>
        /// <value>
        /// The index of the char set.
        /// </value>
        public byte CharSetIndex { get; set; }

        internal ExcelFont Default
        {
            get
            {
                if (this._default == null)
                {
                    this._default = new ExcelFont();
                    if (CultureInfo.CurrentCulture.Name == "ja-JP")
                    {
                        this.InitOrResetLanguageRelatedFontInfo(this._default, "ja-Jp");
                    }
                    else
                    {
                        this.InitOrResetLanguageRelatedFontInfo(this._default, "en-Us");
                    }
                }
                return this._default;
            }
            set { this._default = value; }
        }

        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        /// <value>The color of the font.</value>
        public IExcelColor FontColor { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public ExcelFontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        public string FontName { get; set; }

        /// <summary>
        /// Gets or sets the scheme of the font.
        /// </summary>
        /// <value>The font scheme.</value>
        public FontSchemeCategory FontScheme { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>
        /// The size of the font.
        /// </value>
        /// <exception cref="T:System.InvalidOperationException"> throw when set the FontSize with a negative value</exception>
        public double FontSize
        {
            get { return  this._fontSize; }
            set
            {
                if (value < 0.0)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("fontSizeError"));
                }
                if ((value * 20.0) > 0x1fff)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("fontSizeError"));
                }
                this._fontSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the font color is auto color
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this font color is auto color; otherwise, <see langword="false" />.
        /// </value>
        public bool IsAutoColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font style is bold
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this font style is bold; otherwise, <see langword="false" />.
        /// </value>
        public bool IsBold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font style is italic.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font style is italic; otherwise, <see langword="false" />.
        /// </value>
        public bool IsItalic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is outline style effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in outline style effects; otherwise, <see langword="false" />.
        /// </value>
        public bool IsOutlineStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is in shadow style effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in shadow style effects; otherwise, <see langword="false" />.
        /// </value>
        public bool IsShadowStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is in strikeout effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in strikeout effect; otherwise, <see langword="false" />.
        /// </value>
        public bool IsStrikeOut { get; set; }

        /// <summary>
        /// Gets or sets the under line style.
        /// </summary>
        /// <value>The under line style.</value>
        public Dt.Xls.UnderLineStyle UnderLineStyle { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="P:Dt.Xls.ExcelFont.VerticalAlignRun" /> for the font
        /// </summary>
        /// <value>
        /// The <see cref="P:Dt.Xls.ExcelFont.VerticalAlignRun" /> associate with the font.
        /// </value>
        public Dt.Xls.VerticalAlignRun VerticalAlignRun { get; set; }
    }
}

