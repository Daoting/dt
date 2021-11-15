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
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Defines formatting for all non-cell formatting in the workbook. It specifies
    /// incremental (or differential) aspects of formatting directly inline within dxf element.
    /// The dxf formatting is to be applied on top of or in addition to any formatting already 
    /// presents on the object using the dxf record.
    /// </summary>
    public interface IDifferentialFormatting
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        IDifferentialFormatting Clone();

        /// <summary>
        /// Formatting information pertaining to text alignment in cells. These are a variety of choices for how 
        /// text is aligned both horizontally and vertically, as well as indentation settings, and so on.
        /// </summary>
        /// <value>The alignment.</value>
        IAlignmentBlock Alignment { get; set; }

        /// <summary>
        /// Express a single set of cell border formats. (left, right, top, bottom). Color is optional, when missing,
        /// 'automatic' is applied.
        /// </summary>
        /// <value>The border.</value>
        IExcelBorder Border { get; set; }

        /// <summary>
        /// Gets the extended property list.
        /// </summary>
        /// <value>The extended property list.</value>
        /// <remarks>This property is a secondary attribute, it used mainly for writing file to excel97-2003. Return an empty collection
        /// is enough for the implementer.</remarks>
        List<Tuple<string, object>> ExtendedPropertyList { get; }

        /// <summary>
        /// Defines the fill formatting.
        /// </summary>
        /// <value>The fill formatting.</value>
        /// <remarks>The first item is the PatternType, the second item is the PatternColor and the last the FillBackground</remarks>
        Tuple<FillPatternType, IExcelColor, IExcelColor> Fill { get; set; }

        /// <summary>
        /// Defines the properties for one of the fonts used in the workbook
        /// </summary>
        /// <value>The font.</value>
        IExcelFont Font { get; set; }

        /// <summary>
        /// Gets or sets the format id if the number format the dxf reference is an excel built-in number format.
        /// </summary>
        /// <value>The id of the number format.</value>
        int FormatId { get; set; }

        /// <summary>
        /// Flag indicate whether it's a extended different formatting record.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it;s a extended record; otherwise, <see langword="false" />.
        /// </value>
        bool IsDFXExten { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is hidden; otherwise, <see langword="false" />.
        /// </value>
        bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is locked; otherwise, <see langword="false" />.
        /// </value>
        bool IsLocked { get; set; }

        /// <summary>
        /// Gets or sets the number format
        /// </summary>
        /// <value>The number format</value>
        IExcelNumberFormat NumberFormat { get; set; }
    }
}

