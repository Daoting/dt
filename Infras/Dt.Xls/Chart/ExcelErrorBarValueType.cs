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

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies the type of values used to determine the length of the error bars.
    /// </summary>
    public enum ExcelErrorBarValueType
    {
        FixedValue,
        CustomErrorBar,
        Percentage,
        StandardDeviation,
        StandardError
    }
}

