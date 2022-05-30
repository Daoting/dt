#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines properties used for representing excel font settings
    /// </summary>
    public interface IExcelFont : IEquatable<IExcelFont>
    {
        /// <summary>
        /// Clones and create a new <see cref="T:Dt.Xls.IExcelFont" /> instance 
        /// </summary>
        /// <returns></returns>
        IExcelFont Clone();

        /// <summary>
        /// Gets or sets the index of the char set.
        /// </summary>
        /// <value>The index of the char set.</value>
        byte CharSetIndex { get; set; }

        /// <summary>
        /// Gets or sets the color of the font.
        /// </summary>
        /// <value>The color of the font.</value>
        IExcelColor FontColor { get; set; }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        ExcelFontFamily FontFamily { get; set; }

        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        /// <value>The name of the font.</value>
        string FontName { get; set; }

        /// <summary>
        /// Gets or sets the scheme of the font.
        /// </summary>
        /// <value>The font scheme.</value>
        FontSchemeCategory FontScheme { get; set; }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        double FontSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font color is auto color
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this font color is auto color; otherwise, <see langword="false" />.
        /// </value>
        bool IsAutoColor { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font style is bold
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this font style is bold; otherwise, <see langword="false" />.
        /// </value>
        bool IsBold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font style is italic.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font style is italic; otherwise, <see langword="false" />.
        /// </value>
        bool IsItalic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is outline style effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in outline style effects; otherwise, <see langword="false" />.
        /// </value>
        bool IsOutlineStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is in shadow style effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in shadow style effects; otherwise, <see langword="false" />.
        /// </value>
        bool IsShadowStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the font is in strikeout effects
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the font is in strikeout effect; otherwise, <see langword="false" />.
        /// </value>
        bool IsStrikeOut { get; set; }

        /// <summary>
        /// Gets or sets the under line style.
        /// </summary>
        /// <value>The under line style.</value>
        Dt.Xls.UnderLineStyle UnderLineStyle { get; set; }

        /// <summary>
        /// Gets or sets the vertical align run.
        /// </summary>
        /// <value>The vertical align run.</value>
        Dt.Xls.VerticalAlignRun VerticalAlignRun { get; set; }
    }
}

