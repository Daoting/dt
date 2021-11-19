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
    /// Specifies what part of the spreadsheet to export to an Excel-compatible file. 
    /// </summary>
    [Flags]
    public enum ExcelSaveFlags
    {
        /// <summary>
        /// The default row height is not saved. Excel automatically determines row heights based on the largest font that is set in each row.  
        /// </summary>
        AutoRowHeight = 0x1000,
        /// <summary>
        /// Saves only the data to the Excel-compatible file.  
        /// </summary>
        DataOnly = 0x20,
        /// <summary>
        /// Saves the spreadsheet to the Excel-compatible file with no special options.  
        /// </summary>
        NoFlagsSet = 0,
        /// <summary>
        /// Saves the displayed data, but not the formulas, to the Excel-compatible file.  
        /// </summary>
        NoFormulas = 1,
        /// <summary>
        /// Saves the filtered row results to the Excel-compatible file.  
        /// </summary>
        SaveAsFiltered = 8,
        /// <summary>
        /// Saves the spreadsheet as viewed to the Excel-compatible file.  
        /// </summary>
        SaveAsViewed = 0x88,
        /// <summary>
        /// Saves both the custom row headers and the custom column headers to the Excel-compatible file.  
        /// </summary>
        SaveBothCustomRowAndColumnHeaders = 12,
        /// <summary>
        /// Saves the custom column headers to the Excel-compatible file.  
        /// </summary>
        SaveCustomColumnHeaders = 4,
        /// <summary>
        /// Saves the custom row headers to the Excel-compatible file. 
        /// </summary>
        SaveCustomRowHeaders = 2
    }
}

