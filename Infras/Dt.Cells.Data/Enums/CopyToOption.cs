#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents which type of cell data is copied when invoking the worksheet's Copy or Move method.
    /// </summary>
    [Flags]
    public enum CopyToOption
    {
        /// <summary>
        /// Indicates to copy values, formulas, range groups, sparklines, spans, styles, and tags.
        /// </summary>
        All = 0x1fb,
        /// <summary>
        /// The floating object
        /// </summary>
        FloatingObject = 0x100,
        /// <summary>
        /// Indicates the type of data is a formula.
        /// </summary>
        Formula = 2,
        /// <summary>
        /// Indicates to copy a range group.
        /// </summary>
        RangeGroup = 8,
        /// <summary>
        /// Indicates to copy a span.
        /// </summary>
        Span = 0x20,
        /// <summary>
        /// Indicates the type of data is a sparkline.
        /// </summary>
        Sparkline = 0x10,
        /// <summary>
        /// Indicates the type of data is a style.
        /// </summary>
        Style = 0x40,
        /// <summary>
        /// Indicates the type of data is a tag.
        /// </summary>
        Tag = 0x80,
        /// <summary>
        /// Indicates the type of data is pure data.
        /// </summary>
        Value = 1
    }
}

