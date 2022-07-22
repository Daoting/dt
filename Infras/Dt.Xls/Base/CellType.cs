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
    /// Define the type of the cell
    /// </summary>
    /// <remarks>It not recommended to specify the cell type for a cell manually. Excel will figure out the property cell type during saving the file.</remarks>
    public enum CellType
    {
        Unknown,
        Numeric,
        String,
        FormulaString,
        Blank,
        Boolean,
        Datetime,
        Error,
        Array
    }
}

