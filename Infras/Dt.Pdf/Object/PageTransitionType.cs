#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Pdf.Object
{
    /// <summary>
    /// The transition style to use when moving to this page from another during a presentation.
    /// </summary>
    public enum PageTransitionType
    {
        Default,
        Split,
        Blinds,
        Box,
        Wipe,
        Dissolve,
        Glitter,
        Fly,
        Push,
        Cover,
        Uncover,
        Fade
    }
}

