#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal sealed class DocumentProperties : IDocumentProperties
    {
        public IPropertyAccess DocumentSummaryInfomation { get; set; }

        public IPropertyAccess SummaryInformation { get; set; }
    }
}

