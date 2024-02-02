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
    /// Dynamic filter types
    /// </summary>
    public enum ExcelDynamicFilterType
    {
        /// <summary>
        /// Shows values that are above average.
        /// </summary>
        AboveAverage = 1,
        /// <summary>
        /// Shows values that are below average.
        /// </summary>
        BelowAverage = 2,
        /// <summary>
        /// Shows Last month's dates.
        /// </summary>
        LastMonth = 0x10,
        /// <summary>
        /// Shows last quarter's dates.
        /// </summary>
        LastQuarter = 0x13,
        /// <summary>
        /// Shows last week's dates.
        /// </summary>
        LastWeek = 13,
        /// <summary>
        /// Shows last year's dates.
        /// </summary>
        LastYear = 0x16,
        /// <summary>
        /// Shows the dates that are in January, regardless of year
        /// </summary>
        M1 = 0x1c,
        /// <summary>
        /// Shows the dates that are in October, regardless of year
        /// </summary>
        M10 = 0x25,
        /// <summary>
        /// Shows the dates that are in November, regardless of year
        /// </summary>
        M11 = 0x26,
        /// <summary>
        /// Shows the dates that are in December, regardless of year
        /// </summary>
        M12 = 0x27,
        /// <summary>
        /// Shows the dates that are in February, regardless of year
        /// </summary>
        M2 = 0x1d,
        /// <summary>
        /// Shows the dates that are in March, regardless of year
        /// </summary>
        M3 = 30,
        /// <summary>
        /// Shows the dates that are in April, regardless of year
        /// </summary>
        M4 = 0x1f,
        /// <summary>
        /// Shows the dates that are in May, regardless of year 
        /// </summary>
        M5 = 0x20,
        /// <summary>
        /// Shows the dates that are in June, regardless of year 
        /// </summary>
        M6 = 0x21,
        /// <summary>
        /// Shows the dates that are in July, regardless of year 
        /// </summary>
        M7 = 0x22,
        /// <summary>
        /// Shows the dates that are in August, regardless of year
        /// </summary>
        M8 = 0x23,
        /// <summary>
        /// Shows the dates that are in September, regardless of year
        /// </summary>
        M9 = 0x24,
        /// <summary>
        /// Shows next month's dates.
        /// </summary>
        NextMonth = 14,
        /// <summary>
        /// Shows next quarter's dates.
        /// </summary>
        NextQuarter = 0x11,
        /// <summary>
        /// Shows next week's dates.
        /// </summary>
        NextWeek = 11,
        /// <summary>
        /// Shows next year's dates.
        /// </summary>
        NextYear = 20,
        /// <summary>
        /// The type has not been specified.
        /// </summary>
        Null = 0,
        /// <summary>
        /// Shows the dates that are in the 1st quarter, regardless
        /// of year.
        /// </summary>
        Q1 = 0x18,
        /// <summary>
        /// Shows the dates that are in the 2nd quarter, regardless
        /// of year.
        /// </summary>
        Q2 = 0x19,
        /// <summary>
        /// Shows the dates that are in the 3rd quarter, regardless
        /// of year. 
        /// </summary>
        Q3 = 0x1a,
        /// <summary>
        /// Shows the dates that are in the 4th quarter, regardless
        /// of year.
        /// </summary>
        Q4 = 0x1b,
        /// <summary>
        /// Shows this month's dates.
        /// </summary>
        ThisMonth = 15,
        /// <summary>
        /// Shows this quarter's dates.
        /// </summary>
        ThisQuarter = 0x12,
        /// <summary>
        /// Shows this week's dates.
        /// </summary>
        ThisWeek = 12,
        /// <summary>
        /// Shows this year's dates.
        /// </summary>
        ThisYear = 0x15,
        /// <summary>
        /// Shows today's dates.
        /// </summary>
        Today = 9,
        /// <summary>
        /// Shows tomorrow's dates.
        /// </summary>
        Tomorrow = 8,
        /// <summary>
        /// Shows the dates between the beginning of the year
        /// and today, inclusive.
        /// </summary>
        YearToDate = 0x17,
        /// <summary>
        /// Shows yesterday's dates
        /// </summary>
        Yesterday = 10
    }
}

