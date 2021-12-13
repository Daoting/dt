#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Describes which style is used to display the table, and specifies which portions of the table 
    /// have the style applied.
    /// </summary>
    public interface IExcelTableStyleInfo
    {
        /// <summary>
        /// Represents the name of the table style to use with the table
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// A boolean indicating whether column stripe formatting is applied. 
        /// </summary>
        bool ShowColumnStripes { get; set; }

        /// <summary>
        /// A boolean indicating whether the first column in the table should have style applied.
        /// </summary>
        bool ShowFirstColumn { get; set; }

        /// <summary>
        /// A boolean indicating whether the last column in the table should have style applied.
        /// </summary>
        bool ShowLastColumn { get; set; }

        /// <summary>
        /// A boolean indicating whether row stripe formatting is applied. 
        /// </summary>
        bool ShowRowStripes { get; set; }
    }
}

