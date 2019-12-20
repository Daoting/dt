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
    /// These conditional format operators are used for "Highlight Cells That Contain…" rules. 
    /// For example, "highlight cells that begin with "M2" and contain "Mountain Gear"".
    /// </summary>
    public enum ExcelConditionalFormattingOperator
    {
        NoComparison,
        Between,
        NotBetween,
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        BeginsWith,
        EndsWith,
        ContainsText,
        NotContains
    }
}

