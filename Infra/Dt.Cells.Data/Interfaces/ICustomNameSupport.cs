#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal interface ICustomNameSupport
    {
        void EnumerateAllNames(out Dictionary<string, CalcExpression> workbookNames, out Dictionary<ICustomNameSupport, Dictionary<string, CalcExpression>> worksheetNames);
        void SetDefinedName(string name, CalcExpression expression, bool isWorkbookName);
    }
}

