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
    /// the Calculation mode saved with the workbook.
    /// </summary>
    /// <remarks>
    /// Changing the calculation mode for one workbook will changes the mode for all workbook.
    /// </remarks>
    public enum ExcelCalculationMode
    {
        Manual,
        Automatic,
        AutomaticExceptTables
    }
}

