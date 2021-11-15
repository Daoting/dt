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
    /// Specifies where the search string is found.
    /// </summary>
    public enum SearchFoundFlags
    {
        /// <summary>
        /// [8] Indicates that the string is found in the cell formula.
        /// </summary>
        CellFormula = 8,
        /// <summary>
        /// [4] Indicates that the string is found in the cell tag.
        /// </summary>
        CellTag = 4,
        /// <summary>
        /// [1] Indicates that the string is found in the cell text.
        /// </summary>
        CellText = 1,
        /// <summary>
        /// [0] Indicates that no string is found.
        /// </summary>
        None = 0
    }
}

