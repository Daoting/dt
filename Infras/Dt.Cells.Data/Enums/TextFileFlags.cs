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
    /// Specifies how to process the data when saving to a text file. 
    /// </summary>
    [Flags]
    public enum TextFileFlags
    {
        /// <summary>
        /// Specifies whether to filter the rows with a row filter.  
        /// </summary>
        FilterRows = 8,
        /// <summary>
        /// Specifies whether to force the cell delimiter. 
        /// </summary>
        ForceCellDelimiter = 0x10,
        /// <summary>
        /// Specifies to save the column footer. 
        /// </summary>
        IncludeFooter = 4,
        /// <summary>
        /// Specifies no special settings.  
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies whether to save formulas.
        /// </summary>
        SaveFormulas = 2,
        /// <summary>
        /// Specifies whether to bypass the IFormatter object in the StyleInfo object for the cell. 
        /// </summary>
        Unformatted = 1
    }
}

