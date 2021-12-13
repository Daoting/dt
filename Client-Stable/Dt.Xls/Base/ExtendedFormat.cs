#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents the cell formats and styles in a workbook.
    /// </summary>
    public class ExtendedFormat : IExtendedFormat, IEquatable<IExtendedFormat>
    {
        private IExcelBorder _excelBorder;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Xls.ExtendedFormat" /> class.
        /// </summary>
        public ExtendedFormat()
        {
            this.Font = new ExcelFont().Default;
            this.Border = new ExcelBorder();
        }

        /// <summary>
        /// Clone and create a new instance of the <see cref="T:Dt.Xls.IExtendedFormat" />
        /// </summary>
        /// <returns></returns>
        public IExtendedFormat Clone()
        {
            ExtendedFormat format = new ExtendedFormat();
            format.CopyFrom(this);
            return format;
        }

        internal void CopyFrom(ExtendedFormat source)
        {
            this.ApplyAlignment = source.ApplyAlignment;
            this.ApplyBorder = source.ApplyBorder;
            this.ApplyFill = source.ApplyFill;
            this.ApplyFont = source.ApplyFont;
            this.ApplyNumberFormat = source.ApplyNumberFormat;
            this.ApplyProtection = source.ApplyProtection;
            this.PatternBackgroundColor = source.PatternBackgroundColor;
            this.PatternColor = source.PatternColor;
            this.FillPattern = source.FillPattern;
            this.Border = (source.Border != null) ? source.Border.Clone() : null;
            this.Font = (source.Font != null) ? source.Font.Clone() : null;
            this.NumberFormatIndex = source.NumberFormatIndex;
            this.NumberFormat = source.NumberFormat;
            this.IsStyleFormat = source.IsStyleFormat;
            this.ParentFormatID = source.ParentFormatID;
            this.HorizontalAlign = source.HorizontalAlign;
            this.VerticalAlign = source.VerticalAlign;
            this.IsLocked = source.IsLocked;
            this.Rotation = source.Rotation;
            this.IsWordWrap = source.IsWordWrap;
            this.IsJustfyLastLine = source.IsJustfyLastLine;
            this.IsShrinkToFit = source.IsShrinkToFit;
            this.IsFirstSymbolApostrophe = source.IsFirstSymbolApostrophe;
            this.ReadingOrder = source.ReadingOrder;
            this.IsHidden = source.IsHidden;
            this.Indent = source.Indent;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:Dt.Xls.ExtendedFormat" /> is equals to the current item
        /// </summary>
        /// <param name="excelStyle">The specified <see cref="T:Dt.Xls.ExtendedFormat" /> used to compared with the current item</param>
        /// <returns>
        /// <see langword="true" /> if the specified item has the same settings, otherwise, <c>False</c>
        /// </returns>
        public bool Equals(ExtendedFormat excelStyle)
        {
            int? parentFormatID = null;
            int? nullable14 = null;
            if (object.ReferenceEquals(this, excelStyle))
            {
                return true;
            }
            if (excelStyle == null)
            {
                return false;
            }
            if ((((this.Border.Equals(excelStyle.Border) && (this.FillPattern == excelStyle.FillPattern)) && ((this.NumberFormatIndex == excelStyle.NumberFormatIndex) && (this.HorizontalAlign == excelStyle.HorizontalAlign))) && (((this.VerticalAlign == excelStyle.VerticalAlign) && (this.IsLocked == excelStyle.IsLocked)) && ((this.Rotation == excelStyle.Rotation) && (this.IsWordWrap == excelStyle.IsWordWrap)))) && ((((this.IsJustfyLastLine == excelStyle.IsJustfyLastLine) && (this.IsShrinkToFit == excelStyle.IsShrinkToFit)) && ((this.IsFirstSymbolApostrophe == excelStyle.IsFirstSymbolApostrophe) && (this.ReadingOrder == excelStyle.ReadingOrder))) && ((this.IsHidden == excelStyle.IsHidden) && (this.Indent == excelStyle.Indent))))
            {
                bool? applyAlignment = this.ApplyAlignment;
                bool? nullable2 = excelStyle.ApplyAlignment;
                if ((applyAlignment.GetValueOrDefault() == nullable2.GetValueOrDefault()) && (applyAlignment.HasValue == nullable2.HasValue))
                {
                    bool? applyBorder = this.ApplyBorder;
                    bool? nullable4 = excelStyle.ApplyBorder;
                    if ((applyBorder.GetValueOrDefault() == nullable4.GetValueOrDefault()) && (applyBorder.HasValue == nullable4.HasValue))
                    {
                        bool? applyFill = this.ApplyFill;
                        bool? nullable6 = excelStyle.ApplyFill;
                        if ((applyFill.GetValueOrDefault() == nullable6.GetValueOrDefault()) && (applyFill.HasValue == nullable6.HasValue))
                        {
                            bool? applyFont = this.ApplyFont;
                            bool? nullable8 = excelStyle.ApplyFont;
                            if ((applyFont.GetValueOrDefault() == nullable8.GetValueOrDefault()) && (applyFont.HasValue == nullable8.HasValue))
                            {
                                bool? applyNumberFormat = this.ApplyNumberFormat;
                                bool? nullable10 = excelStyle.ApplyNumberFormat;
                                if ((applyNumberFormat.GetValueOrDefault() == nullable10.GetValueOrDefault()) && (applyNumberFormat.HasValue == nullable10.HasValue))
                                {
                                    bool? applyProtection = this.ApplyProtection;
                                    bool? nullable12 = excelStyle.ApplyProtection;
                                    if ((applyProtection.GetValueOrDefault() == nullable12.GetValueOrDefault()) && (applyProtection.HasValue == nullable12.HasValue))
                                    {
                                        parentFormatID = this.ParentFormatID;
                                        nullable14 = excelStyle.ParentFormatID;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            bool flag = (parentFormatID.GetValueOrDefault() == nullable14.GetValueOrDefault()) && (parentFormatID.HasValue == nullable14.HasValue);
            if (this.Font == null)
            {
                if (excelStyle.Font != null)
                {
                    return false;
                }
            }
            else
            {
                flag = flag && this.Font.Equals(excelStyle.Font);
            }
            if ((this.NumberFormat == null) && (excelStyle.NumberFormat != null))
            {
                return false;
            }
            if (this.NumberFormat != null)
            {
                flag = flag && this.NumberFormat.Equals(excelStyle.NumberFormat);
            }
            if ((this.PatternColor == null) && (excelStyle.PatternColor != null))
            {
                return false;
            }
            if ((this.PatternBackgroundColor == null) && (excelStyle.PatternBackgroundColor != null))
            {
                return false;
            }
            if (this.PatternBackgroundColor != null)
            {
                flag = flag && this.PatternBackgroundColor.Equals(excelStyle.PatternBackgroundColor);
            }
            if (this.PatternColor != null)
            {
                flag = flag && this.PatternColor.Equals(excelStyle.PatternColor);
            }
            return flag;
        }

        /// <summary>
        /// Determines whether the current instance is equals to the specific <see cref="T:Dt.Xls.IExtendedFormat" /> instance.
        /// </summary>
        /// <param name="format">The other <see cref="T:Dt.Xls.IExtendedFormat" /> instance used to compare with.</param>
        /// <returns></returns>
        public bool Equals(IExtendedFormat format)
        {
            return this.Equals(format as ExtendedFormat);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> used to compared with the current item</param>
        /// <returns>
        /// <see langword="true" /> if the specified <see cref="T:System.Object" /> has the same settings; otherwise, <c>False</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExtendedFormat);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int num = ((this.FillPattern.GetHashCode() ^ (this.IsWordWrap.GetHashCode() << 4)) ^ (((ExcelVerticalAlignment) this.VerticalAlign).GetHashCode() << 8)) ^ (((ExcelHorizontalAlignment) this.HorizontalAlign).GetHashCode() << 12);
            if (this.Border != null)
            {
                num ^= this.Border.GetHashCode() << 0x10;
            }
            if (this.Font != null)
            {
                num ^= this.Font.GetHashCode() << 0x16;
            }
            if (this.PatternBackgroundColor != null)
            {
                num ^= this.PatternBackgroundColor.GetHashCode() << 0x1a;
            }
            return num;
        }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the alignment
        /// </summary>
        /// <value>Apply alignment or not.</value>
        public bool? ApplyAlignment { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the border.
        /// </summary>
        /// <value>Apply border or not</value>
        public bool? ApplyBorder { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the fill
        /// </summary>
        /// <value>Apply fill or not.</value>
        public bool? ApplyFill { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the font
        /// </summary>
        /// <value>Apply font or not.</value>
        public bool? ApplyFont { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the number format.
        /// </summary>
        /// <value>Apply number format or not.</value>
        public bool? ApplyNumberFormat { get; set; }

        /// <summary>
        /// Gets or sets the flag indicate whether apply the protection
        /// </summary>
        /// <value>Apply protection or not.</value>
        public bool? ApplyProtection { get; set; }

        /// <summary>
        /// Gets or sets the border of the ExtendedFormat.
        /// </summary>
        /// <value>The border of the ExtendedFormat</value>
        public IExcelBorder Border
        {
            get
            {
                if (this._excelBorder == null)
                {
                    this._excelBorder = new ExcelBorder();
                }
                return this._excelBorder;
            }
            set { this._excelBorder = value; }
        }

        /// <summary>
        /// Gets the default format.
        /// </summary>
        /// <value>The default format</value>
        public static ExtendedFormat Default
        {
            get { return  new ExtendedFormat { Border = new ExcelBorder(), Font = new ExcelFont().Default, NumberFormatIndex = 0, IsLocked = true, VerticalAlign = ExcelVerticalAlignment.Bottom, HorizontalAlign = ExcelHorizontalAlignment.General }; }
        }

        /// <summary>
        /// Gets or sets the fill pattern.
        /// </summary>
        /// <value>The fill pattern.</value>
        public FillPatternType FillPattern { get; set; }

        /// <summary>
        /// Gets or sets the font of the ExtendedFormat.
        /// </summary>
        /// <value>The font of the ExtendedFormat.</value>
        public IExcelFont Font { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelHorizontalAlignment" /> property value
        /// </summary>
        /// <value>The horizontal alignment setting.</value>
        public ExcelHorizontalAlignment HorizontalAlign { get; set; }

        /// <summary>
        /// Gets or sets the indent level of the alignment.
        /// </summary>
        /// <value>The indent level of the alignment</value>
        public byte Indent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is first symbol apostrophe.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is first symbol apostrophe; otherwise, <see langword="false" />.
        /// </value>
        public bool IsFirstSymbolApostrophe { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is hidden; otherwise, <see langword="false" />.
        /// </value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the justify distributed option is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsJustfyLastLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Lock property is true or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the lock property is true; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>Locking cells or hiding formulas has no effect until you protect the <see cref="T:Dt.Xls.IExcelWorksheet" /> instance</remarks>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Shrink to fit of  the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsShrinkToFit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is style format.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is style format; otherwise, <see langword="false" />.
        /// </value>
        public bool IsStyleFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Wrap text of the text control is checked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it's checked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsWordWrap { get; set; }

        /// <summary>
        /// Gets or sets the number format of the ExtendedFormat.
        /// </summary>
        /// <value>The number format of the ExtendedFormat.</value>
        public IExcelNumberFormat NumberFormat { get; set; }

        /// <summary>
        /// Gets or sets the index of the number format.
        /// </summary>
        /// <value>The index of the number format.</value>
        /// <remarks>
        /// If the number format is a excel built in number format, used this property to refers to
        /// that number format and ExcelNumberFormat property will be null in this case
        /// </remarks>
        public int NumberFormatIndex { get; set; }

        /// <summary>
        /// Gets or sets the parent format ID.
        /// </summary>
        /// <value>The parent format ID.</value>
        public int? ParentFormatID { get; set; }

        /// <summary>
        /// Gets or sets the color of the pattern background.
        /// </summary>
        /// <value>The color of the pattern background.</value>
        public IExcelColor PatternBackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the color of the pattern.
        /// </summary>
        /// <value>The color of the pattern.</value>
        public IExcelColor PatternColor { get; set; }

        /// <summary>
        /// Gets or sets the text direction of the alignment.
        /// </summary>
        /// <value>The test direction of the alignment.</value>
        public TextDirection ReadingOrder { get; set; }

        /// <summary>
        /// Gets or sets the text orientation.
        /// </summary>
        /// <value>The orientation of the text.</value>
        public int Rotation { get; set; }

        /// <summary>
        /// Get or set the <see cref="T:Dt.Xls.ExcelVerticalAlignment" /> property value
        /// </summary>
        /// <value>The vertical alignment setting.</value>
        public ExcelVerticalAlignment VerticalAlign { get; set; }
    }
}

