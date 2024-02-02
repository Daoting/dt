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
    /// Represent conditional formatting type.
    /// </summary>
    public enum ExcelConditionalFormatType
    {
        /// <summary>
        /// This conditional formatting rule highlights cells that are above the average for all values in the range.
        /// </summary>
        AboveAverage = 0x19,
        /// <summary>
        /// This conditional formatting rule highlights cells that are above or equals to the average for all values in the range.
        /// </summary>
        AboveOrEqualToAverage = 0x1d,
        /// <summary>
        /// This conditional formatting rule highlights cells in the range that begin with the given text. Equivalent to
        /// using the LEFT() sheet function and comparing values
        /// </summary>
        BeginsWith = 0x1f,
        /// <summary>
        /// This conditional formatting rule highlights cells that are below the average for all values in the range.
        /// </summary>
        BelowAverage = 0x1a,
        /// <summary>
        /// This conditional formatting rule highlights cells that are below or equals the average for all values in the range.
        /// </summary>
        BelowOrEqualToAverage = 30,
        /// <summary>
        /// This conditional formatting rule compares a cell value to a formula calculated result, using an operator.
        /// </summary>
        CellIs = 0,
        /// <summary>
        /// This conditional formatting rule creates a gradated color scale on the cells.
        /// </summary>
        ColorScale = 2,
        /// <summary>
        /// This conditional formatting rule highlights cells that are completely blank. Equivalent of using LEN(TRIM()).
        /// This means that if the cell contains only characters that TRIM() would remove, then it is considered blank.
        /// </summary>
        /// <remarks> An empty cell is also considered blank.</remarks>
        ContainsBlanks = 9,
        /// <summary>
        /// This conditional formatting rule highlights cells with  formula errors. Equivalent to using ISERROR() sheet
        /// function to determine if there is a formula error.
        /// </summary>
        ContainsErrors = 11,
        /// <summary>
        /// This conditional formatting rule highlights cells containing given text. Equivalent to using the SEARCH()
        /// sheet function to determine whether the cell contains the text.
        /// </summary>
        ContainsText = 8,
        /// <summary>
        /// This conditional formatting rule displays a gradated data bar in the range of cells.
        /// </summary>
        DataBar = 3,
        /// <summary>
        /// This conditional formatting rule highlights duplicated values.
        /// </summary>
        DuplicateValues = 0x1b,
        /// <summary>
        /// This conditional formatting rule highlights cells ending with given text. Equivalent to using the RIGHT() sheet function
        /// and comparing values. 
        /// </summary>
        EndsWith = 0x20,
        /// <summary>
        /// This conditional formatting rule contains a formula to evaluate. When the formula result is true, the cell is highlighted.
        /// </summary>
        Expression = 1,
        /// <summary>
        /// This conditional formatting rule applies icons to cells according to their values.
        /// </summary>
        IconSet = 4,
        /// <summary>
        /// A date in the last seven days.
        /// </summary>
        Last7Days = 0x12,
        /// <summary>
        /// A date occurring in the last calendar month.
        /// </summary>
        LastMonth = 0x13,
        /// <summary>
        /// A date occurring last week.
        /// </summary>
        LastWeek = 0x17,
        /// <summary>
        /// A date occurring in the next calendar month.
        /// </summary>
        NextMonth = 20,
        /// <summary>
        /// A date occurring next week.
        /// </summary>
        NextWeek = 0x16,
        /// <summary>
        /// This conditional formatting rule highlights cells that are not blank. Equivalent of using LEN(TRIM()). This
        /// means that if the cell contains only characters that TRIM() would remove, then it is considered blank
        /// </summary>
        NotContainsBlanks = 10,
        /// <summary>
        /// This conditional formatting rule highlights cells without formula errors. Equivalent to using ISERROR()
        /// sheet function to determine if there is a formula error.
        /// </summary>
        NotContainsErrors = 12,
        /// <summary>
        /// This conditional formatting rule highlights cells that do not contain given text. Equivalent to using the SEARCH() sheet function.
        /// </summary>
        NotContainsText = 0x21,
        /// <summary>
        /// A date occurring in this calendar month.
        /// </summary>
        ThisMonth = 0x18,
        /// <summary>
        /// A date occurring this week.
        /// </summary>
        ThisWeek = 0x15,
        /// <summary>
        /// This conditional formatting rule highlights cells containing dates in the specified time period. The
        /// underlying value of the cell is evaluated, therefore the cell does not need to be formatted as a date to be
        /// evaluated.
        /// </summary>
        TimePeriod = 0x22,
        /// <summary>
        /// Today's date.
        /// </summary>
        Today = 15,
        /// <summary>
        /// Tomorrow's date.
        /// </summary>
        Tomorrow = 0x10,
        /// <summary>
        /// This conditional formatting rule highlights cells whose values fall in the top N or bottom N bracket, as specified.
        /// </summary>
        Top10 = 5,
        /// <summary>
        /// This conditional formatting rule highlights unique values in the range.
        /// </summary>
        UniqueValues = 7,
        /// <summary>
        /// Yesterday's date.
        /// </summary>
        Yesterday = 0x11
    }
}

