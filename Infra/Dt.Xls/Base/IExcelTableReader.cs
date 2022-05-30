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
    /// Represents a collection of generalized method which be used to read Excel table settings.
    /// </summary>
    public interface IExcelTableReader
    {
        /// <summary>
        /// Add a table to the specified sheet.
        /// </summary>
        /// <param name="sheetIndex">Zero-based worksheet index.</param>
        /// <param name="table">An <see cref="T:Dt.Xls.IExcelTable" /> instance which represents a table defined in the specified worksheet.</param>
        void SetTable(int sheetIndex, IExcelTable table);
        /// <summary>
        /// Set the set table and pivot table style.
        /// </summary>
        /// <param name="defaultTableStyleName">The default table style name.</param>
        /// <param name="defaultPivotTableStyleName">The default pivot table style name.</param>
        void SetTableDefaultStyle(string defaultTableStyleName, string defaultPivotTableStyleName);
        /// <summary>
        /// Set a table style which used by one of table of the current workbook.
        /// </summary>
        /// <param name="tableStyle">
        /// a table style definition that indicates how the workbook
        /// should format and display a table.
        /// </param>
        void SetTableStyle(IExcelTableStyle tableStyle);
    }
}

