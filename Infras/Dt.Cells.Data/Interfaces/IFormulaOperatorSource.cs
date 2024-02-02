#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using Dt.CalcEngine.Expressions;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal interface IFormulaOperatorSource
    {
        void ClearExpression(CalcLocalIdentity id);
        void ClearNode(CalcLocalIdentity id);
        IEnumerable<CalcLocalIdentity> GetAllLocalIdentities();
        CalcExpression GetExpression(CalcLocalIdentity id);
        IFormulaOperatorSource GetExternalManager(ICalcSource source);
        CalcNode GetNode(CalcIdentity id);
        void Invalidate(CalcLocalIdentity id, bool autoCalculate);
        void Invalidate(IEnumerable<CalcLocalIdentity> ids, bool autoCalculate);
        void SetExpression(CalcLocalIdentity id, CalcExpression expr);

        IMultiSourceProvider MultiSourceProvider { get; }

        ICalcSource Source { get; }
    }
}

