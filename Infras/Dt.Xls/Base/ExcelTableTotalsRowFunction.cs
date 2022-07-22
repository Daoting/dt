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
    /// An enumeration that specifies what function is used to aggregate the data in a column before it is displayed in the totals row.
    /// </summary>
    public enum ExcelTableTotalsRowFunction
    {
        None,
        Average,
        Count,
        CountNums,
        Max,
        Min,
        Sum,
        StdDev,
        Var,
        Custom
    }
}

