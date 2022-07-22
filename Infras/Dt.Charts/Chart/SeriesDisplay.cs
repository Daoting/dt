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
    public enum SeriesDisplay : byte
    {
        HideLegend = 2,
        ShowNaNGap = 1,
        SkipNaN = 0
    }
}

