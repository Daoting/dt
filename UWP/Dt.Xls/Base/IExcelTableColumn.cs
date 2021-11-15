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
    /// An element representing a single column for the table.
    /// </summary>
    public interface IExcelTableColumn
    {
        /// <summary>
        /// Represents the formula that is used to perform the calculation for each cell in this column.
        /// </summary>
        string CalculatedColumnFormula { get; set; }

        /// <summary>
        /// A flag indicate whether the CalculatedColumnFormula is array formula or not.
        /// </summary>
        bool CalculatedColumnFormulaIsArrayFormula { get; set; }

        /// <summary>
        /// An integer representing the unique identifier of this column. This should be unique per table.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// A string representing the unique caption of the table column. This is what shall be displayed in the header row in the UI,
        /// and is referenced through functions. This name shall be unique per table.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Represents the formula that is used to perform the calculation the totals cell of the column
        /// </summary>
        /// <remarks>It should be only exists when the TotlsRowFunction is custom</remarks>
        string TotalsRowCustomFunction { get; set; }

        /// <summary>
        /// An enumeration indicating which type of aggregation to show in the totals row cell for this column
        /// </summary>
        ExcelTableTotalsRowFunction TotalsRowFunction { get; set; }

        /// <summary>
        /// A flag indicate whether the TotalsRowFunction is array formula or not.
        /// </summary>
        bool TotalsRowFunctionIsArrayFormula { get; set; }

        /// <summary>
        /// A string to show in the totals row cell for this column.
        /// </summary>
        /// <remarks>This setting should be ignored unless the totalsRowFunction equals to none for this column, in which case it is displayed in the totals row.</remarks>
        string TotalsRowLabel { get; set; }
    }
}

