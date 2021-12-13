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
    /// Specifies what part of the Excel-compatible file to load into the spreadsheet. 
    /// </summary>
    [Flags]
    public enum ExcelOpenFlags
    {
        /// <summary>
        /// Loads column headers from frozen rows in the Excel-compatible file into the spreadsheet. 
        /// </summary>
        ColumnHeaders = 8,
        /// <summary>
        /// Loads formulas from the Excel-compatible file into the spreadsheet. 
        /// </summary>
        DataAndFormulasOnly = 3,
        /// <summary>
        /// Loads only the data from the Excel-compatible file into the spreadsheet.  
        /// </summary>
        DataOnly = 1,
        /// <summary>
        /// Avoids recalculation after loading the Excel-compatible file (by not setting the SheetView.AutoCalculation property to true and not calling SheetView.Recalculate()).  
        /// </summary>
        DoNotRecalculateAfterLoad = 0x400,
        /// <summary>
        /// Opens the spreadsheet from the Excel-compatible file with no special options.
        /// </summary>
        NoFlagsSet = 0,
        /// <summary>
        /// Loads row headers from frozen columns and column headers from frozen rows.  
        /// </summary>
        RowAndColumnHeaders = 12,
        /// <summary>
        /// Loads row headers from frozen columns in the Excel-compatible file into the spreadsheet.  
        /// </summary>
        RowHeaders = 4
    }
}

