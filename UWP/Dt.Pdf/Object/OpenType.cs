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
    /// Specifying how the document should be displayed when opened
    /// </summary>
    public enum OpenType
    {
        Auto,
        UseNone,
        UseOutlines,
        UseThumbs,
        FullScreen,
        UseOC,
        UseAttachments
    }
}

