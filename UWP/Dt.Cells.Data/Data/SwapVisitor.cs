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
#endregion

namespace Dt.Cells.Data
{
    internal class SwapVisitor : Dt.Cells.Data.ExpressionVisitor
    {
        int _colOffset;
        int _rowOffset;

        public SwapVisitor(int rowOffset, int colOffset)
        {
            this._rowOffset = rowOffset;
            this._colOffset = colOffset;
        }

        protected override CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
        {
            return expr.Offset(this._rowOffset, this._colOffset, false, true);
        }

        protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            return expr.Offset(this._rowOffset, this._colOffset, false, true);
        }
    }
}

