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
    internal interface IDocumentProperties
    {
        IPropertyAccess DocumentSummaryInfomation { get; set; }

        IPropertyAccess SummaryInformation { get; set; }
    }
}

