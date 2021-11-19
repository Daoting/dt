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
#endregion

namespace Dt.Cells.Data
{
    internal class RemoveSheetVisitor : Dt.Cells.Data.ExpressionVisitor
    {
        ICalcSource _currentSource;
        ICalcSource _replacedSource;

        public RemoveSheetVisitor(ICalcSource currentSource, ICalcSource replacedSource)
        {
            this._currentSource = currentSource;
            this._replacedSource = replacedSource;
        }

        protected override CalcExpression VisitSheetRangeExpression(CalcSheetRangeExpression sheetRangeExpr, int baseRow, int baseColumn)
        {
            if ((sheetRangeExpr.StartSource == sheetRangeExpr.EndSource) && (sheetRangeExpr.StartSource == this._currentSource))
            {
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            ICalcSource newStartSource = (sheetRangeExpr.StartSource == this._currentSource) ? this._replacedSource : sheetRangeExpr.StartSource;
            ICalcSource newEndSource = (sheetRangeExpr.EndSource == this._currentSource) ? this._replacedSource : sheetRangeExpr.EndSource;
            return sheetRangeExpr.ResetSheetRanges(newStartSource, newEndSource);
        }
    }
}

