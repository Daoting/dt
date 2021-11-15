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
    internal class MoveVisitor : OperatorExpressionVisistorBase
    {
        int _columnCount;
        int _columnOffset;
        bool _convertToExternal;
        ICalcSource _extSource;
        CellRange _fromRange;
        bool _isCopy;
        bool _offsetForDependency;
        int _rowCount;
        int _rowOffset;

        public MoveVisitor(CellRange fromRange, int rowOffset, int columnOffset, int rowCount, int columnCount, bool offsetForDependency, ICalcSource extSource, bool convertToExternal, ICalcSource currentSource, bool isCopy) : base(currentSource)
        {
            this._fromRange = fromRange;
            this._rowOffset = rowOffset;
            this._columnOffset = columnOffset;
            this._rowCount = rowCount;
            this._columnCount = columnCount;
            this._offsetForDependency = offsetForDependency;
            this._extSource = extSource;
            this._convertToExternal = convertToExternal;
            this._isCopy = isCopy;
        }

        public CalcExpression FirstVisit(CalcExpression expr, int baseRow, int baseColumn)
        {
            if (this._isCopy)
            {
                CalcExpression expression = expr;
                if (this._convertToExternal)
                {
                    expression = this.Visit(expr, baseRow, baseColumn);
                }
                if (!(expression is CalcSharedExpression))
                {
                    return new CalcSharedExpression(expression);
                }
            }
            return this.Visit(expr, baseRow, baseColumn);
        }

        bool NeedMove(int row, int column, int endRow, int endColumn)
        {
            if (((this._rowOffset != 0) || (this._columnOffset != 0)) && ((((row >= this._fromRange.Row) && (row < (this._fromRange.Row + this._fromRange.RowCount))) || ((this._fromRange.Row == -1) || (row == -1))) && (((endRow < (this._fromRange.Row + this._rowCount)) || (this._fromRange.Row == -1)) || ((endRow == -1) || (this._rowCount == 0x7fffffff)))))
            {
                if (((column < this._fromRange.Column) || (column >= (this._fromRange.Column + this._fromRange.ColumnCount))) && (this._fromRange.Column != -1))
                {
                    if (column != -1)
                    {
                        return false;
                    }
                    if (((endColumn >= (this._fromRange.Column + this._columnCount)) && (this._fromRange.Column != -1)) && (endColumn != -1))
                    {
                        return (this._columnCount == 0x7fffffff);
                    }
                    return true;
                }
                return true;
            }
            return false;
        }

        protected override CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
        {
            CalcCellIdentity id = expr.GetId(baseRow, baseColumn) as CalcCellIdentity;
            if (!this._isCopy || !this._offsetForDependency)
            {
                if (this._offsetForDependency)
                {
                    if ((((id.RowIndex + this._rowOffset) < 0) || ((id.ColumnIndex + this._columnOffset) < 0)) || (((id.RowIndex + this._rowOffset) >= this._rowCount) || ((id.ColumnIndex + this._columnOffset) >= this._columnCount)))
                    {
                        return new CalcErrorExpression(CalcErrors.Reference);
                    }
                    if (this.NeedMove(id.RowIndex, id.ColumnIndex, -1, -1))
                    {
                        expr = expr.Offset(this._rowOffset, this._columnOffset, this._offsetForDependency, true) as CalcCellExpression;
                    }
                }
                else if (!this._isCopy)
                {
                    if (!expr.RowRelative)
                    {
                        this._rowOffset = 0;
                    }
                    if (!expr.ColumnRelative)
                    {
                        this._columnOffset = 0;
                    }
                    if (this.NeedMove(baseRow, baseColumn, -1, -1))
                    {
                        expr = expr.Offset(-this._rowOffset, -this._columnOffset, false, true) as CalcCellExpression;
                    }
                }
                if (this._convertToExternal)
                {
                    return new CalcExternalCellExpression(this._extSource, expr.Row, expr.Column, expr.RowRelative, expr.ColumnRelative);
                }
            }
            return expr;
        }

        protected override CalcExpression VisitExternalCellExpression(CalcExternalCellExpression expr, int baseRow, int baseColumn)
        {
            CalcExternalCellIdentity id = expr.GetId(baseRow, baseColumn) as CalcExternalCellIdentity;
            if (!this._isCopy || !this._offsetForDependency)
            {
                if (this._offsetForDependency)
                {
                    if ((((id.RowIndex + this._rowOffset) < 0) || ((id.ColumnIndex + this._columnOffset) < 0)) || (((id.RowIndex + this._rowOffset) >= this._rowCount) || ((id.ColumnIndex + this._columnOffset) >= this._columnCount)))
                    {
                        return new CalcErrorExpression(CalcErrors.Reference);
                    }
                    if (this.NeedMove(id.RowIndex, id.ColumnIndex, -1, -1))
                    {
                        expr = expr.Offset(this._rowOffset, this._columnOffset, this._offsetForDependency, true) as CalcExternalCellExpression;
                    }
                }
                else if (!this._isCopy)
                {
                    if (!expr.RowRelative)
                    {
                        this._rowOffset = 0;
                    }
                    if (!expr.ColumnRelative)
                    {
                        this._columnOffset = 0;
                    }
                    if (this.NeedMove(baseRow, baseColumn, -1, -1))
                    {
                        expr = expr.Offset(-this._rowOffset, -this._columnOffset, false, true) as CalcExternalCellExpression;
                    }
                }
                if (this._convertToExternal && (expr.Source != this._extSource))
                {
                    return new CalcExternalCellExpression(this._extSource, expr.Row, expr.Column, expr.RowRelative, expr.ColumnRelative);
                }
            }
            return expr;
        }

        protected override CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
        {
            CalcExternalRangeIdentity id = expr.GetId(baseRow, baseColumn) as CalcExternalRangeIdentity;
            if (this._isCopy && this._offsetForDependency)
            {
                return expr;
            }
            if (this._offsetForDependency)
            {
                if ((!id.IsFullColumn && (((id.RowIndex + this._rowOffset) < 0) || ((((id.RowIndex + id.RowCount) - 1) + this._rowOffset) >= this._rowCount))) || (!id.IsFullRow && (((id.ColumnIndex + this._columnOffset) < 0) || ((((id.ColumnIndex + id.ColumnCount) - 1) + this._columnOffset) >= this._columnCount))))
                {
                    return new CalcErrorExpression(CalcErrors.Reference);
                }
                if (this.NeedMove(id.RowIndex, id.ColumnIndex, (id.RowIndex + id.RowCount) - 1, (id.ColumnIndex + id.ColumnCount) - 1))
                {
                    expr = expr.Offset(this._rowOffset, this._columnOffset, this._offsetForDependency, true) as CalcExternalRangeExpression;
                }
            }
            else if (!this._isCopy)
            {
                if (!expr.StartRowRelative && !expr.EndRowRelative)
                {
                    this._rowOffset = 0;
                }
                if (!expr.StartColumnRelative && !expr.EndColumnRelative)
                {
                    this._columnOffset = 0;
                }
                if (this.NeedMove(baseRow, baseColumn, baseRow + id.RowCount, baseColumn + id.ColumnCount))
                {
                    expr = expr.Offset(-this._rowOffset, -this._columnOffset, false, true) as CalcExternalRangeExpression;
                }
            }
            if (!this._convertToExternal || (expr.Source == this._extSource))
            {
                return expr;
            }
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return new CalcExternalRangeExpression(this._extSource);
            }
            if (expr.IsFullRow)
            {
                return new CalcExternalRangeExpression(this._extSource, expr.StartRow, expr.EndRow, expr.StartRowRelative, expr.EndRowRelative, true);
            }
            if (expr.IsFullColumn)
            {
                return new CalcExternalRangeExpression(this._extSource, expr.StartColumn, expr.EndColumn, expr.StartColumnRelative, expr.EndColumnRelative, false);
            }
            return new CalcExternalRangeExpression(this._extSource, expr.StartRow, expr.StartColumn, expr.EndRow, expr.EndColumn, expr.StartRowRelative, expr.StartColumnRelative, expr.EndRowRelative, expr.EndColumnRelative);
        }

        protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            CalcRangeIdentity id = expr.GetId(baseRow, baseColumn) as CalcRangeIdentity;
            if (this._isCopy && this._offsetForDependency)
            {
                return expr;
            }
            if (this._offsetForDependency)
            {
                if ((!id.IsFullColumn && (((id.RowIndex + this._rowOffset) < 0) || ((((id.RowIndex + id.RowCount) - 1) + this._rowOffset) >= this._rowCount))) || (!id.IsFullRow && (((id.ColumnIndex + this._columnOffset) < 0) || ((((id.ColumnIndex + id.ColumnCount) - 1) + this._columnOffset) >= this._columnCount))))
                {
                    return new CalcErrorExpression(CalcErrors.Reference);
                }
                if (this.NeedMove(id.RowIndex, id.ColumnIndex, id.RowCount, id.ColumnCount))
                {
                    expr = expr.Offset(this._rowOffset, this._columnOffset, this._offsetForDependency, true) as CalcRangeExpression;
                }
            }
            else if (!this._isCopy)
            {
                if (!expr.StartRowRelative && !expr.EndRowRelative)
                {
                    this._rowOffset = 0;
                }
                if (!expr.StartColumnRelative && !expr.EndColumnRelative)
                {
                    this._columnOffset = 0;
                }
                if (this.NeedMove(baseRow, baseColumn, baseRow + id.RowCount, baseColumn + id.ColumnCount))
                {
                    expr = expr.Offset(-this._rowOffset, -this._columnOffset, false, true) as CalcRangeExpression;
                }
            }
            if (!this._convertToExternal)
            {
                return expr;
            }
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return new CalcExternalRangeExpression(this._extSource);
            }
            if (expr.IsFullRow)
            {
                return new CalcExternalRangeExpression(this._extSource, expr.StartRow, expr.EndRow, expr.StartRowRelative, expr.EndRowRelative, true);
            }
            if (expr.IsFullColumn)
            {
                return new CalcExternalRangeExpression(this._extSource, expr.StartColumn, expr.EndColumn, expr.StartColumnRelative, expr.EndColumnRelative, false);
            }
            return new CalcExternalRangeExpression(this._extSource, expr.StartRow, expr.StartColumn, expr.EndRow, expr.EndColumn, expr.StartRowRelative, expr.StartColumnRelative, expr.EndRowRelative, expr.EndColumnRelative);
        }

        protected override CalcExpression VisitSheetRangeExpression(CalcSheetRangeExpression expr, int baseRow, int baseColumn)
        {
            CalcSheetRangeIdentity id = expr.GetId(baseRow, baseColumn) as CalcSheetRangeIdentity;
            if (!this._isCopy || !this._offsetForDependency)
            {
                if (this._offsetForDependency)
                {
                    if ((!id.IsFullColumn && (((id.RowIndex + this._rowOffset) < 0) || ((((id.RowIndex + id.RowCount) - 1) + this._rowOffset) >= this._rowCount))) || (!id.IsFullRow && (((id.ColumnIndex + this._columnOffset) < 0) || ((((id.ColumnIndex + id.ColumnCount) - 1) + this._columnOffset) >= this._columnCount))))
                    {
                        return new CalcErrorExpression(CalcErrors.Reference);
                    }
                    if (this.NeedMove(id.RowIndex, id.ColumnIndex, id.RowCount, id.ColumnCount))
                    {
                        expr = expr.Offset(this._rowOffset, this._columnOffset, this._offsetForDependency, true) as CalcSheetRangeExpression;
                    }
                    return expr;
                }
                if (!this._isCopy)
                {
                    if (!expr.StartRowRelative && !expr.EndRowRelative)
                    {
                        this._rowOffset = 0;
                    }
                    if (!expr.StartColumnRelative && !expr.EndColumnRelative)
                    {
                        this._columnOffset = 0;
                    }
                    if (this.NeedMove(baseRow, baseColumn, baseRow + id.RowCount, baseColumn + id.ColumnCount))
                    {
                        expr = expr.Offset(-this._rowOffset, -this._columnOffset, false, true) as CalcSheetRangeExpression;
                    }
                }
            }
            return expr;
        }
    }
}

