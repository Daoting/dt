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
    /// Specifies the type for the average condition.
    /// </summary>
    public enum AverageConditionType
    {
        Above,
        Below,
        EqualOrAbove,
        EqualOrBelow,
        Above1StdDev,
        Below1StdDev,
        Above2StdDev,
        Below2StdDev,
        Above3StdDev,
        Below3StdDev
    }
}

