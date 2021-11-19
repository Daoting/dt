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
    /// Specifies the export flags.
    /// </summary>
    [Flags]
    public enum TextFileSaveFlags
    {
        /// <summary>
        /// Exports the cell text area including the visible rows and columns of filtered-in rows.
        /// </summary>
        AsViewed = 0x20,
        /// <summary>
        /// Exports formulas.
        /// </summary>
        ExportFormula = 0x10,
        /// <summary>
        /// Includes column headers.
        /// </summary>
        IncludeColumnHeader = 2,
        /// <summary>
        /// Includes row headers.
        /// </summary>
        IncludeRowHeader = 1,
        /// <summary>
        /// Exports with no special options.
        /// </summary>
        None = 0,
        /// <summary>
        /// Exports without formatting.
        /// </summary>
        UnFormatted = 8
    }
}

