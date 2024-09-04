#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent formatting properties that are used in various features to modify cell formatting
    /// </summary>
    public class DifferentialFormatting : IDifferentialFormatting
    {
        private List<Tuple<string, object>> _extendedPropertyList;

        /// <summary>
        /// Clone this instance.
        /// </summary>
        /// <returns></returns>
        public IDifferentialFormatting Clone()
        {
            DifferentialFormatting formatting = new DifferentialFormatting {
                Fill = (this.Fill != null) ? new Tuple<FillPatternType, IExcelColor, IExcelColor>(this.Fill.Item1, this.Fill.Item2, this.Fill.Item3) : null,
                Border = (this.Border != null) ? this.Border.Clone() : null,
                Font = (this.Font != null) ? this.Font.Clone() : null,
                FormatId = this.FormatId,
                NumberFormat = this.NumberFormat,
                Alignment = (this.Alignment != null) ? this.Alignment.Clone() : null,
                IsHidden = this.IsHidden,
                IsLocked = this.IsLocked,
                IsDFXExten = this.IsDFXExten
            };
            if (formatting.IsDFXExten)
            {
                formatting.ExtendedPropertyList = (this.ExtendedPropertyList != null) ? new List<Tuple<string, object>>((IEnumerable<Tuple<string, object>>) this.ExtendedPropertyList) : null;
            }
            return formatting;
        }

        /// <summary>
        /// Formatting information pertaining to text alignment in cells. These are a variety of choices for how
        /// text is aligned both horizontally and vertically, as well as indentation settings, and so on.
        /// </summary>
        /// <value>The alignment.</value>
        public IAlignmentBlock Alignment { get; set; }

        /// <summary>
        /// Express a single set of cell border formats. (left, right, top, bottom). Color is optional, when missing,
        /// 'automatic' is applied.
        /// </summary>
        /// <value>The border.</value>
        public IExcelBorder Border { get; set; }

        /// <summary>
        /// Gets the extended property list.
        /// </summary>
        /// <value>The extended property list.</value>
        /// <remarks>This property is a secondary attribute, it used mainly for writing file to excel97-2003.</remarks>
        public List<Tuple<string, object>> ExtendedPropertyList
        {
            get
            {
                if (!this.IsDFXExten)
                {
                    throw new InvalidOperationException(ResourceHelper.GetResourceString("dxfExtendedPropertyError"));
                }
                if (this._extendedPropertyList == null)
                {
                    this._extendedPropertyList = new List<Tuple<string, object>>();
                }
                return this._extendedPropertyList;
            }
            internal set { this._extendedPropertyList = value; }
        }

        /// <summary>
        /// Defines the fill formatting.
        /// </summary>
        /// <value>The fill formatting.</value>
        /// <remarks>The first item is the PatternType, the second item is the PatternColor and the last the FillBackground</remarks>
        public Tuple<FillPatternType, IExcelColor, IExcelColor> Fill { get; set; }

        /// <summary>
        /// Defines the properties for one of the fonts used in the workbook
        /// </summary>
        /// <value>The font.</value>
        public IExcelFont Font { get; set; }

        /// <summary>
        /// Gets or sets the format id if the number format the dxf references is an excel built-in number format.
        /// </summary>
        /// <value>The id of the number format.</value>
        public int FormatId { get; set; }

        /// <summary>
        /// Flag indicate whether it's a extended different formatting record.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it;s a extended record; otherwise, <see langword="false" />.
        /// </value>
        public bool IsDFXExten { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is hidden; otherwise, <see langword="false" />.
        /// </value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is locked; otherwise, <see langword="false" />.
        /// </value>
        public bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the number format
        /// </summary>
        /// <value>The number format</value>
        public IExcelNumberFormat NumberFormat { get; set; }
    }
}

