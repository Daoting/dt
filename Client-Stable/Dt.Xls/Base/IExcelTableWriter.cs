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
    /// Represents a collection of generalized method which be used to write excel tables.
    /// </summary>
    public interface IExcelTableWriter
    {
        /// <summary>
        /// Get the name of the default pivot table style used by the current workbook
        /// </summary>
        /// <returns>The name of the default pivot table style.</returns>
        string GetDefaultPivotTableStyleName();
        /// <summary>
        /// Get the name of the default table style used by the current workbook
        /// </summary>
        /// <returns>The name of the default table style</returns>
        string GetDefaultTableStyleName();
        /// <summary>
        /// Get all tables of the specified sheet.
        /// </summary>
        /// <param name="sheetIndex">zero-based sheet index.</param>
        /// <returns>A collection of <see cref="T:Dt.Xls.IExcelTable" /> instances represents tables</returns>
        List<IExcelTable> GetSheetTables(int sheetIndex);
        /// <summary>
        /// Get all table styles defined in the current workbook
        /// </summary>
        /// <returns>A collection of <see cref="T:Dt.Xls.IExcelTableStyle" /> instances represents table styles.</returns>
        List<IExcelTableStyle> GetTableStyles();
    }
}

