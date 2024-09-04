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
    /// Specifies what data is pasted from the Clipboard.
    /// </summary>
    [Flags]
    public enum ClipboardPasteOptions
    {
        /// <summary>
        /// [0] Pastes all data objects, including values, formatting, and formulas.
        /// </summary>
        All = 0xff,
        /// <summary>
        /// The floating objects
        /// </summary>
        FloatingObjects = 8,
        /// <summary>
        /// [2] Pastes only formatting.
        /// </summary>
        Formatting = 2,
        /// <summary>
        /// [3] Pastes only formulas.
        /// </summary>
        Formulas = 4,
        /// <summary>
        /// The range group
        /// </summary>
        RangeGroup = 0x10,
        /// <summary>
        /// Indicates to copy a span.
        /// </summary>
        Span = 0x40,
        /// <summary>
        /// Indicates the type of data is a sparkline.
        /// </summary>
        Sparkline = 0x20,
        /// <summary>
        /// The tags
        /// </summary>
        Tags = 0x80,
        /// <summary>
        /// [1] Pastes only values.
        /// </summary>
        Values = 1
    }
}

