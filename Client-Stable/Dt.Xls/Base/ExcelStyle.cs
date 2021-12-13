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
    /// An class implements <see cref="T:Dt.Xls.IBuiltInExcelStyle" /> used to represent Excel built-in cell style.
    /// </summary>
    public class ExcelStyle : IBuiltInExcelStyle, IExcelStyle
    {
        private ExtendedFormat _format;

        /// <summary>
        /// Copy and create a new <see cref="T:Dt.Xls.ExcelStyle" /> instance
        /// </summary>
        /// <returns></returns>
        public ExcelStyle Copy()
        {
            return new ExcelStyle { BuiltInStyle = this.BuiltInStyle, OutLineLevel = this.OutLineLevel, IsCustomBuiltIn = this.IsCustomBuiltIn, Category = this.Category, Name = this.Name, Format = this.Format.Clone() };
        }

        /// <summary>
        /// Gets the built in style category.
        /// </summary>
        /// <returns></returns>
        public int GetBuiltInStyleCategory()
        {
            if (this.IsBuiltInStyle)
            {
                switch (this.BuiltInStyle)
                {
                    case BuiltInStyleIndex.Normal:
                        return 1;

                    case BuiltInStyleIndex.RowLevel:
                    case BuiltInStyleIndex.ColumnLevel:
                    case BuiltInStyleIndex.Accent1:
                    case BuiltInStyleIndex.Accent1_20:
                    case BuiltInStyleIndex.Accent1_40:
                    case BuiltInStyleIndex.Accent1_60:
                    case BuiltInStyleIndex.Accent2:
                    case BuiltInStyleIndex.Accent2_20:
                    case BuiltInStyleIndex.Accent2_40:
                    case BuiltInStyleIndex.Accent2_60:
                    case BuiltInStyleIndex.Accent3:
                    case BuiltInStyleIndex.Accent3_20:
                    case BuiltInStyleIndex.Accent3_40:
                    case BuiltInStyleIndex.Accent3_60:
                    case BuiltInStyleIndex.Accent4:
                    case BuiltInStyleIndex.Accent4_20:
                    case BuiltInStyleIndex.Accent4_40:
                    case BuiltInStyleIndex.Accent4_60:
                    case BuiltInStyleIndex.Accent5:
                    case BuiltInStyleIndex.Accent5_20:
                    case BuiltInStyleIndex.Accent5_40:
                    case BuiltInStyleIndex.Accent5_60:
                    case BuiltInStyleIndex.Accent6:
                    case BuiltInStyleIndex.Accent6_20:
                    case BuiltInStyleIndex.Accent6_40:
                    case BuiltInStyleIndex.Accent6_60:
                        return 4;

                    case BuiltInStyleIndex.Comma:
                    case BuiltInStyleIndex.Currency:
                    case BuiltInStyleIndex.Comma0:
                    case BuiltInStyleIndex.Currency0:
                        return 5;

                    case BuiltInStyleIndex.Percent:
                        return 5;

                    case BuiltInStyleIndex.Note:
                    case BuiltInStyleIndex.Output:
                        return 2;

                    case BuiltInStyleIndex.WarningText:
                        return 2;

                    case BuiltInStyleIndex.Title:
                    case BuiltInStyleIndex.Total:
                        return 3;

                    case BuiltInStyleIndex.Heading1:
                    case BuiltInStyleIndex.Heading2:
                    case BuiltInStyleIndex.Heading3:
                    case BuiltInStyleIndex.Heading4:
                        return 3;

                    case BuiltInStyleIndex.Input:
                        return 2;

                    case BuiltInStyleIndex.Calculation:
                        return 2;

                    case BuiltInStyleIndex.CheckCell:
                        return 2;

                    case BuiltInStyleIndex.LinkedCell:
                        return 2;

                    case BuiltInStyleIndex.Good:
                        return 1;

                    case BuiltInStyleIndex.Bad:
                        return 1;

                    case BuiltInStyleIndex.Neutral:
                        return 1;

                    case BuiltInStyleIndex.ExplanatoryText:
                        return 2;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets or sets the built in style index.
        /// </summary>
        /// <value>The built in style index</value>
        public BuiltInStyleIndex BuiltInStyle { get; set; }

        /// <summary>
        /// Gets or sets the category of the built-in style
        /// </summary>
        /// <value>The category of the built-in style</value>
        public int Category { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="T:Dt.Xls.IExtendedFormat" /> used in the cell style.
        /// </summary>
        /// <value>
        /// An <see cref="T:Dt.Xls.IExtendedFormat" /> represents the cell style format setting
        /// </value>
        public IExtendedFormat Format
        {
            get
            {
                if (this._format == null)
                {
                    this._format = new ExtendedFormat();
                }
                return this._format;
            }
            set { this._format = value as ExtendedFormat; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is built in style.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is built in style; otherwise, <see langword="false" />.
        /// </value>
        public bool IsBuiltInStyle
        {
            get { return  true; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the style has been modified
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the style has been modified; otherwise, <see langword="false" />.
        /// </value>
        public bool IsCustomBuiltIn { get; set; }

        /// <summary>
        /// Gets the name of the style
        /// </summary>
        /// <value>The name of the style</value>
        public string Name { get; set; }

        /// <summary>
        /// An unsigned integer that specifies the depth level of row/column automatic outlining.
        /// </summary>
        /// <value>The out line level value</value>
        /// <remarks>If <see cref="P:Dt.Xls.ExcelStyle.Category" /> equals 0x01 or 0x02, the value must between 0x00 and 0x06, otherwise, the value must be 0xFF.</remarks>
        public byte OutLineLevel { get; set; }
    }
}

