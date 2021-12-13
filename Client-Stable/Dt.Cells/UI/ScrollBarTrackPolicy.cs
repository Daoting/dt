#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.UI
{
    /// <summary>
    /// Specifies whether the component scrolls the sheet when the user moves the scroll box.
    /// </summary>
    [Flags]
    public enum ScrollBarTrackPolicy
    {
        Off,
        Vertical,
        Horizontal,
        Both
    }
}

