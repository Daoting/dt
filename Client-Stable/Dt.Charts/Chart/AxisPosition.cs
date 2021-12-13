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

namespace Dt.Charts
{
    [Flags]
    public enum AxisPosition
    {
        DisableLastLabelOverflow = 4,
        Far = 1,
        Inner = 8,
        Near = 0,
        OverData = 2
    }
}

