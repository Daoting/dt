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
    /// Specifies the type of date value.
    /// </summary>
    public enum SpecialDateType
    {
        /// <summary>
        /// Determines whether the value represents a day of the month.
        /// </summary>
        Day = 0,
        /// <summary>
        /// Determines whether the value represents a month.
        /// </summary>
        Month = 0,
        /// <summary>
        /// Determines whether the value represents a quarter.
        /// </summary>
        Quarter = 0,
        /// <summary>
        /// Determines whether the value represents a week in the month.
        /// </summary>
        Week = 0
    }
}

