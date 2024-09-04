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
    /// Specifies the type of search flags.
    /// </summary>
    public enum SearchFlags
    {
        /// <summary>
        /// Determines whether to search within a cell range.
        /// </summary>
        BlockRange = 8,
        /// <summary>
        /// Determines whether the search considers only an exact match.
        /// </summary>
        ExactMatch = 2,
        /// <summary>
        /// Determines whether the search considers the case of the letters in the search string. 
        /// </summary>
        IgnoreCase = 1,
        /// <summary>
        /// Determines whether the search considers wildcard characters (*, ?) in the search string.
        /// </summary>
        UseWildCards = 4
    }
}

