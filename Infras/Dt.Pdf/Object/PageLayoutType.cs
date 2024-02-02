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
    /// Specifying the page layout to be used when the document is opened
    /// </summary>
    public enum PageLayoutType
    {
        Auto,
        SinglePage,
        OneColumn,
        TwoColumnLeft,
        TwoColumnRight,
        TwoPageLeft,
        TwoPageRight
    }
}

