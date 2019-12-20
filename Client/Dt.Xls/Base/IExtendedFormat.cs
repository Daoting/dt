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
    /// An interface used to defined format used in cell or style
    /// </summary>
    public interface IExtendedFormat : IEquatable<IExtendedFormat>
    {
        /// <summary>
        /// Clone and create a new instance of the <see cref="T:Dt.Xls.IExtendedFormat" />
        /// </summary>
        /// <returns></returns>
        IExtendedFormat Clone();

        /// <summary>
        /// Gets or sets the flag indicate whether apply the alignment
        /// </summary>
        /// <value>Apply alignment or not.</value>
        bool? ApplyAlignment { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the border.
        /// </summary>
        /// <value>Apply border or not</value>
        bool? ApplyBorder { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the fill
        /// </summary>
        /// <value>Apply fill or not.</value>
        bool? ApplyFill { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the font
        /// </summary>
        /// <value>Apply font or not.</value>
        bool? ApplyFont { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the number format.
        /// </summary>
        /// <value>Apply number format or not.</value>
        bool? ApplyNumberFormat { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the protection
        /// </summary>
        /// <value>Apply protection or not.</value>
        bool? ApplyProtection { get; set; }

        /// <summary>
        /// Gets or sets the border of the ExtendedFormat.
        /// </summary>
        /// <value>The border of the ExtendedFormat</value>
        IExcelBorder Border { get; set; }

        /// <summary>
        /// Gets or sets the fill pattern.
        /// </summary>
        /// <value>The fill pattern.</value>
        FillPatternType FillPattern { get; set; }

        /// <summary>
        /// Gets or sets the font of the ExtendedFormat.
        /// </summary>
        /// <value>The font of the ExtendedFormat.</value>
        IExcelFont Font { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelHorizontalAlignment" /> property value
        /// </summary>
        /// <value>The horizontal alignment setting.</value>
        ExcelHorizontalAlignment HorizontalAlign { get; set; }

        /// <summary>
        /// Gets or sets the indent level of the alignment.
        /// </summary>
        /// <value>The indent level of the alignment</value>
        byte Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first symbol apostrophe.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is first symbol apostrophe; otherwise, <see langword="false" />.
        /// </value>
        bool IsFirstSymbolApostrophe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is hidden; otherwise, <see langword="false" />.
        /// </value>
        bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the justify distributed option is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsJustfyLastLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Lock property is true or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the lock property is true; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>Locking cells or hiding formulas has no effect until you protect the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</remarks>
        bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Shrink to fit of  the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsShrinkToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is style format.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is style format; otherwise, <see langword="false" />.
        /// </value>
        bool IsStyleFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Wrap text of the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        bool IsWordWrap { get; set; }

        /// <summary>
        /// Gets or sets the number format of the ExtendedFormat.
        /// </summary>
        /// <value>The number format of the ExtendedFormat.</value>
        IExcelNumberFormat NumberFormat { get; set; }

        /// <summary>
        /// Gets or sets the index of the number format.
        /// </summary>
        /// <value>The index of the number format.</value>
        /// <remarks>
        /// If the number format is a excel built in number format, used this property to refers to
        /// that number format and the property  ExcelNumberFormat  will be null in this case
        /// </remarks>
        int NumberFormatIndex { get; set; }

        /// <summary>
        /// Gets or sets the parent format ID.
        /// </summary>
        /// <value>The parent format ID.</value>
        int? ParentFormatID { get; set; }

        /// <summary>
        /// Gets or sets the color of the pattern background.
        /// </summary>
        /// <value>The color of the pattern background.</value>
        IExcelColor PatternBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the pattern.
        /// </summary>
        /// <value>The color of the pattern.</value>
        IExcelColor PatternColor { get; set; }

        /// <summary>
        /// Gets or sets the text direction of the alignment.
        /// </summary>
        /// <value>The test direction of the alignment.</value>
        TextDirection ReadingOrder { get; set; }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The orientation of the text.</value>
        int Rotation { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelVerticalAlignment" /> property value
        /// </summary>
        /// <value>The vertical alignment setting.</value>
        ExcelVerticalAlignment VerticalAlign { get; set; }
    }
}

