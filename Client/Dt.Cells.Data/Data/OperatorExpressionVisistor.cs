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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal abstract class OperatorExpressionVisistor : OperatorExpressionVisistorBase
    {
        bool _isFullBand;
        bool _isRow;

        public OperatorExpressionVisistor(bool isRow, bool isFullBand, ICalcSource currentCalcSource) : base(currentCalcSource)
        {
            this._isRow = isRow;
            this._isFullBand = isFullBand;
        }

        public abstract void GetCellOffset(int oldIndex, out int newIndex);
        public abstract void GetRangeOffset(int oldStart, int oldEnd, out int newStart, out int newEnd);
        protected override CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
        {
            int num2;
            int num4;
            if (base.OffsetExternalOnly)
            {
                return expr;
            }
            if (this._isFullBand && ((this._isRow && expr.RowRelative) || (!this._isRow && expr.ColumnRelative)))
            {
                return expr;
            }
            CalcCellIdentity id = expr.GetId(baseRow, baseColumn) as CalcCellIdentity;
            int oldIndex = this._isRow ? id.RowIndex : id.ColumnIndex;
            this.GetCellOffset(oldIndex, out num2);
            int num3 = this._isRow ? baseRow : baseColumn;
            this.GetCellOffset(num3, out num4);
            if (num2 == -2147483648)
            {
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            int row = num2 - oldIndex;
            if ((this._isRow && expr.RowRelative) || (!this._isRow && expr.ColumnRelative))
            {
                row -= num4 - num3;
            }
            if (row == 0)
            {
                return base.VisitCellExpression(expr, baseRow, baseColumn);
            }
            if (this._isRow)
            {
                return expr.Offset(row, 0, true, !this.OffsetAbsoluteOnly);
            }
            return expr.Offset(0, row, true, !this.OffsetAbsoluteOnly);
        }

        protected override CalcExpression VisitExternalCellExpression(CalcExternalCellExpression expr, int baseRow, int baseColumn)
        {
            int num2;
            int num4;
            int oldIndex = this._isRow ? baseRow : baseColumn;
            this.GetCellOffset(oldIndex, out num2);
            if ((base.CurrentCalcSource == null) || (base.CurrentCalcSource != expr.Source))
            {
                if (oldIndex == num2)
                {
                    return expr;
                }
                if (this._isRow)
                {
                    return expr.Offset(oldIndex - num2, 0, false, true);
                }
                return expr.Offset(0, oldIndex - num2, false, true);
            }
            if (this._isFullBand && ((this._isRow && expr.RowRelative) || (!this._isRow && expr.ColumnRelative)))
            {
                return expr;
            }
            CalcExternalCellIdentity id = expr.GetId(baseRow, baseColumn) as CalcExternalCellIdentity;
            int num3 = this._isRow ? id.RowIndex : id.ColumnIndex;
            this.GetCellOffset(num3, out num4);
            if (num4 == -2147483648)
            {
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            int row = num4 - num3;
            if ((this._isRow && expr.RowRelative) || (!this._isRow && expr.ColumnRelative))
            {
                row -= num2 - oldIndex;
            }
            if (row == 0)
            {
                return base.VisitExternalCellExpression(expr, baseRow, baseColumn);
            }
            if (this._isRow)
            {
                return expr.Offset(row, 0, true, !this.OffsetAbsoluteOnly);
            }
            return expr.Offset(0, row, true, !this.OffsetAbsoluteOnly);
        }

        protected override CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
        {
            int num3;
            int num4;
            int num6;
            int num8;
            if ((base.CurrentCalcSource == null) || (base.CurrentCalcSource != expr.Source))
            {
                return expr;
            }
            if (this._isFullBand && ((this._isRow && expr.StartRowRelative) || (!this._isRow && expr.StartColumnRelative)))
            {
                return expr;
            }
            CalcExternalRangeIdentity id = expr.GetId(baseRow, baseColumn) as CalcExternalRangeIdentity;
            int oldStart = this._isRow ? id.RowIndex : id.ColumnIndex;
            int oldEnd = this._isRow ? ((id.RowIndex + id.RowCount) - 1) : ((id.ColumnIndex + id.ColumnCount) - 1);
            this.GetRangeOffset(oldStart, oldEnd, out num3, out num4);
            int oldIndex = this._isRow ? baseRow : baseColumn;
            this.GetCellOffset(oldIndex, out num6);
            int num7 = this._isRow ? (baseRow + id.RowCount) : (baseColumn + id.ColumnCount);
            this.GetCellOffset(num7, out num8);
            if ((num3 == -2147483648) && (num4 == -2147483648))
            {
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            if ((num3 == -2147483648) || (num4 == -2147483648))
            {
                return expr;
            }
            int row = num3 - oldStart;
            if ((this._isRow && expr.StartRowRelative) || (!this._isRow && expr.StartColumnRelative))
            {
                row -= num6 - oldIndex;
            }
            int endRow = num4 - oldEnd;
            if ((this._isRow && expr.EndRowRelative) || (!this._isRow && expr.EndColumnRelative))
            {
                endRow -= num8 - num7;
            }
            if ((row == 0) && (endRow == 0))
            {
                return base.VisitExternalRangeExpression(expr, baseRow, baseColumn);
            }
            if (this._isRow)
            {
                return expr.Offset(row, 0, endRow, 0, true, !this.OffsetAbsoluteOnly);
            }
            return expr.Offset(0, row, 0, endRow, true, !this.OffsetAbsoluteOnly);
        }

        protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            int num3;
            int num4;
            int num6;
            if (base.OffsetExternalOnly)
            {
                return expr;
            }
            if (this._isFullBand && ((this._isRow && expr.StartRowRelative) || (!this._isRow && expr.StartColumnRelative)))
            {
                return expr;
            }
            if ((this._isRow && expr.IsFullColumn) || (!this._isRow && expr.IsFullRow))
            {
                return expr;
            }
            CalcRangeIdentity id = expr.GetId(baseRow, baseColumn) as CalcRangeIdentity;
            int oldStart = this._isRow ? id.RowIndex : id.ColumnIndex;
            int oldEnd = this._isRow ? ((id.RowIndex + id.RowCount) - 1) : ((id.ColumnIndex + id.ColumnCount) - 1);
            this.GetRangeOffset(oldStart, oldEnd, out num3, out num4);
            int oldIndex = this._isRow ? baseRow : baseColumn;
            this.GetCellOffset(oldIndex, out num6);
            if ((num3 == -2147483648) && (num4 == -2147483648))
            {
                return new CalcErrorExpression(CalcErrors.Reference);
            }
            if ((num3 == -2147483648) || (num4 == -2147483648))
            {
                return expr;
            }
            int row = num3 - oldStart;
            if ((this._isRow && expr.StartRowRelative) || (!this._isRow && expr.StartColumnRelative))
            {
                row -= num6 - oldIndex;
            }
            int endRow = num4 - oldEnd;
            if ((this._isRow && expr.EndRowRelative) || (!this._isRow && expr.EndColumnRelative))
            {
                endRow -= num6 - oldIndex;
            }
            if ((row == 0) && (endRow == 0))
            {
                return base.VisitRangeExpression(expr, baseRow, baseColumn);
            }
            if (this._isRow)
            {
                return expr.Offset(row, 0, endRow, 0, true, !this.OffsetAbsoluteOnly);
            }
            return expr.Offset(0, row, 0, endRow, true, !this.OffsetAbsoluteOnly);
        }

        public bool OffsetAbsoluteOnly { get; set; }
    }
}

