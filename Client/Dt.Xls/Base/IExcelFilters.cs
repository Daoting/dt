#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// When multiple values are chosen to filter by, or when a group of date values are chosen to filter by, this element
    /// groups those criteria together.
    /// </summary>
    public interface IExcelFilters : IExcelFilter
    {
        /// <summary>
        /// Flag indicating whether to filter by blank.
        /// </summary>
        bool Blank { get; set; }

        /// <summary>
        /// Calendar type for date grouped items. Used to interpret the values in dateGroupItem.
        /// </summary>
        /// <remarks>
        /// This is the calendar type used to evaluate all dates in the filter column, even when those
        /// dates are not using the same calendar system / date formatting.
        /// </remarks>
        ExcelCalendarType CalendarType { get; set; }

        /// <summary>
        /// A collection is used to express a group of dates or times which are used in an AutoFilter criteria
        /// </summary>
        List<IExcelDateGroupItem> DateGroupItem { get; set; }

        /// <summary>
        /// Represents a filter criteria value.
        /// </summary>
        List<string> Filter { get; set; }
    }
}

