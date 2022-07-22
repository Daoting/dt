#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine.Expressions;
using System;
#endregion

namespace Dt.CalcEngine
{
    internal class FullBandMappingVisitor : ExpressionVisitor
    {
        private int _index;
        private bool _isRow;

        public FullBandMappingVisitor(bool isRow, int index)
        {
            this._isRow = isRow;
            this._index = index;
        }

        protected override CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
        {
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return expr;
            }
            if (this._isRow && expr.IsFullRow)
            {
                if (expr.StartRow == expr.EndRow)
                {
                    return new CalcExternalCellExpression(expr.Source, expr.StartRow, this._index);
                }
                return new CalcExternalRangeExpression(expr.Source, expr.StartRow, this._index, expr.EndRow, this._index);
            }
            if (this._isRow || !expr.IsFullColumn)
            {
                return base.VisitExternalRangeExpression(expr, baseRow, baseColumn);
            }
            if (expr.StartColumn == expr.EndColumn)
            {
                return new CalcExternalCellExpression(expr.Source, this._index, expr.StartColumn);
            }
            return new CalcExternalRangeExpression(expr.Source, this._index, expr.StartColumn, this._index, expr.EndColumn);
        }

        protected override CalcExpression VisitFunctionExpression(CalcFunctionExpression expr, int baseRow, int baseColumn)
        {
            return expr;
        }

        protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return expr;
            }
            if (this._isRow && expr.IsFullRow)
            {
                if (expr.StartRow == expr.EndRow)
                {
                    return new CalcCellExpression(expr.StartRow, this._index);
                }
                return new CalcRangeExpression(expr.StartRow, this._index, expr.EndRow, this._index);
            }
            if (this._isRow || !expr.IsFullColumn)
            {
                return base.VisitRangeExpression(expr, baseRow, baseColumn);
            }
            if (expr.StartColumn == expr.EndColumn)
            {
                return new CalcCellExpression(this._index, expr.StartColumn);
            }
            return new CalcRangeExpression(this._index, expr.StartColumn, this._index, expr.EndColumn);
        }
    }
}

