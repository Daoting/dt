#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls.OOXml
{
    internal class SharedFormula
    {
        internal List<ParsedSharedFormulaStruct> ParsedSharedFormulaStructs;

        internal int BaseColumn { get; set; }

        internal string BaseFormula { get; set; }

        internal int BaseRow { get; set; }

        internal int Count { get; set; }

        internal bool IsRowShared { get; set; }
    }
}

