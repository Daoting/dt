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
    /// Specifies the import flags.
    /// </summary>
    [Flags]
    public enum TextFileOpenFlags
    {
        /// <summary>
        /// Imports formulas.
        /// </summary>
        ImportFormula = 0x10,
        /// <summary>
        /// Includes column headers.
        /// </summary>
        IncludeColumnHeader = 2,
        /// <summary>
        /// Includes row headers.
        /// </summary>
        IncludeRowHeader = 1,
        /// <summary>
        /// Imports with no special options.
        /// </summary>
        None = 0,
        /// <summary>
        /// Leaves the data unformatted.
        /// </summary>
        /// <remarks>
        /// The unformatted setting bypasses the IFormatter object in the CellStyleInfo object for the cell so the cell data is unformatted.
        /// </remarks>
        UnFormatted = 8
    }
}

