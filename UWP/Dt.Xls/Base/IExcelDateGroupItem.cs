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
    /// Represents a group of dates or times which are used in an AutoFilter criteria.
    /// </summary>
    public interface IExcelDateGroupItem
    {
        /// <summary>
        /// Grouping level.
        /// </summary>
        ExcelDateTimeGrouping DateTimeGrouping { get; set; }

        /// <summary>
        /// Day
        /// </summary>
        ushort Day { get; set; }

        /// <summary>
        /// hour
        /// </summary>
        ushort Hour { get; set; }

        /// <summary>
        /// Minute
        /// </summary>
        ushort Minute { get; set; }

        /// <summary>
        /// Month
        /// </summary>
        ushort Month { get; set; }

        /// <summary>
        /// Second
        /// </summary>
        ushort Second { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        ushort Year { get; set; }
    }
}

