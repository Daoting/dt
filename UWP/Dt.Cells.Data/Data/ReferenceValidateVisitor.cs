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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal class ReferenceValidateVisitor : Dt.Cells.Data.ExpressionVisitor
    {
        int? _maxColIndex;
        int? _maxRowIndex;

        public ReferenceValidateVisitor(int? maxRowIndex = new int?(), int? maxColCount = new int?())
        {
            this._maxRowIndex = maxRowIndex;
            this._maxColIndex = maxColCount;
        }

        public CalcIdentity GetId(CalcCellExpression cellExp, int row, int column)
        {
            return new CalcCellIdentity(cellExp.RowRelative ? (cellExp.Row + row) : cellExp.Row, cellExp.ColumnRelative ? (cellExp.Column + column) : cellExp.Column);
        }

        public CalcIdentity GetId(CalcExternalCellExpression cellExp, int row, int column)
        {
            return new CalcExternalCellIdentity(cellExp.Source, cellExp.RowRelative ? (cellExp.Row + row) : cellExp.Row, cellExp.ColumnRelative ? (cellExp.Column + column) : cellExp.Column);
        }

        public CalcIdentity GetId(CalcExternalRangeExpression expr, int row, int column)
        {
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return new CalcExternalRangeIdentity(expr.Source);
            }
            if (expr.IsFullRow)
            {
                int num = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
                int num2 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
                this.Sort(ref num, ref num2);
                return new CalcExternalRangeIdentity(expr.Source, num, (num2 - num) + 1, true);
            }
            if (expr.IsFullColumn)
            {
                int num3 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
                int num4 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
                this.Sort(ref num3, ref num4);
                return new CalcExternalRangeIdentity(expr.Source, num3, (num4 - num3) + 1, false);
            }
            int num5 = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
            int num6 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
            int num7 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
            int num8 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
            this.Sort(ref num5, ref num6);
            this.Sort(ref num7, ref num8);
            return new CalcExternalRangeIdentity(expr.Source, num5, num7, (num6 - num5) + 1, (num8 - num7) + 1);
        }

        public CalcIdentity GetId(CalcRangeExpression expr, int row, int column)
        {
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return new CalcRangeIdentity();
            }
            if (expr.IsFullRow)
            {
                int num = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
                int num2 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
                this.Sort(ref num, ref num2);
                return new CalcRangeIdentity(num, (num2 - num) + 1, true);
            }
            if (expr.IsFullColumn)
            {
                int num3 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
                int num4 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
                this.Sort(ref num3, ref num4);
                return new CalcRangeIdentity(num3, (num4 - num3) + 1, false);
            }
            int num5 = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
            int num6 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
            int num7 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
            int num8 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
            this.Sort(ref num5, ref num6);
            this.Sort(ref num7, ref num8);
            return new CalcRangeIdentity(num5, num7, (num6 - num5) + 1, (num8 - num7) + 1);
        }

        public CalcIdentity GetId(CalcSheetRangeExpression expr, int row, int column)
        {
            if (expr.IsFullRow && expr.IsFullColumn)
            {
                return new CalcSheetRangeIdentity(expr.StartSource, expr.EndSource);
            }
            if (expr.IsFullRow)
            {
                int num = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
                int num2 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
                this.Sort(ref num, ref num2);
                return new CalcSheetRangeIdentity(expr.StartSource, expr.EndSource, num, (num2 - num) + 1, true);
            }
            if (expr.IsFullColumn)
            {
                int num3 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
                int num4 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
                this.Sort(ref num3, ref num4);
                return new CalcSheetRangeIdentity(expr.StartSource, expr.EndSource, num3, (num4 - num3) + 1, false);
            }
            int num5 = expr.StartRowRelative ? (expr.StartRow + row) : expr.StartRow;
            int num6 = expr.EndRowRelative ? (expr.EndRow + row) : expr.EndRow;
            int num7 = expr.StartColumnRelative ? (expr.StartColumn + column) : expr.StartColumn;
            int num8 = expr.EndColumnRelative ? (expr.EndColumn + column) : expr.EndColumn;
            this.Sort(ref num5, ref num6);
            this.Sort(ref num7, ref num8);
            return new CalcSheetRangeIdentity(expr.StartSource, expr.EndSource, num5, num7, (num6 - num5) + 1, (num8 - num7) + 1);
        }

        internal void Sort(ref int index1, ref int index2)
        {
            if (index1 > index2)
            {
                int num = index1;
                index1 = index2;
                index2 = num;
            }
        }

        protected override CalcExpression VisitCellExpression(CalcCellExpression expr, int baseRow, int baseColumn)
        {
            bool flag = this._maxRowIndex.HasValue && expr.RowRelative;
            bool flag2 = this._maxColIndex.HasValue && expr.ColumnRelative;
            if (!flag && !flag2)
            {
                return expr;
            }
            CalcCellIdentity identity = this.GetId(expr, baseRow, baseColumn) as CalcCellIdentity;
            if ((!flag || ((identity.RowIndex >= 0) && (identity.RowIndex <= this._maxRowIndex.Value))) && (!flag2 || ((identity.ColumnIndex >= 0) && (identity.ColumnIndex <= this._maxColIndex.Value))))
            {
                return expr;
            }
            return new CalcErrorExpression(CalcErrors.Reference);
        }

        protected override CalcExpression VisitExternalCellExpression(CalcExternalCellExpression expr, int baseRow, int baseColumn)
        {
            bool rowRelative = expr.RowRelative;
            bool columnRelative = expr.ColumnRelative;
            if (!rowRelative && !columnRelative)
            {
                return expr;
            }
            int num = expr.Source.GetRowCount() - 1;
            int num2 = expr.Source.GetColumnCount() - 1;
            CalcExternalCellIdentity identity = this.GetId(expr, baseRow, baseColumn) as CalcExternalCellIdentity;
            if ((!rowRelative || ((identity.RowIndex >= 0) && (identity.RowIndex <= num))) && (!columnRelative || ((identity.ColumnIndex >= 0) && (identity.ColumnIndex <= num2))))
            {
                return expr;
            }
            return new CalcErrorExpression(CalcErrors.Reference);
        }

        protected override CalcExpression VisitExternalRangeExpression(CalcExternalRangeExpression expr, int baseRow, int baseColumn)
        {
            bool flag = !expr.IsFullColumn && expr.StartRowRelative;
            bool flag2 = !expr.IsFullRow && expr.StartColumnRelative;
            bool flag3 = !expr.IsFullColumn && expr.EndRowRelative;
            bool flag4 = !expr.IsFullRow && expr.EndColumnRelative;
            if ((!flag && !flag2) && (!flag3 && !flag4))
            {
                return expr;
            }
            int num = expr.Source.GetRowCount() - 1;
            int num2 = expr.Source.GetColumnCount() - 1;
            CalcExternalRangeIdentity identity = this.GetId(expr, baseRow, baseColumn) as CalcExternalRangeIdentity;
            int num3 = (identity.RowIndex + identity.RowCount) - 1;
            int num4 = (identity.ColumnIndex + identity.ColumnCount) - 1;
            if (((!flag || ((identity.RowIndex >= 0) && (identity.RowIndex <= num))) && (!flag2 || ((identity.ColumnIndex >= 0) && (identity.ColumnIndex <= num2)))) && ((!flag3 || ((num3 >= 0) && (num3 <= num))) && (!flag4 || ((num4 >= 0) && (num4 <= num2)))))
            {
                return base.VisitExternalRangeExpression(expr, baseRow, baseColumn);
            }
            return new CalcErrorExpression(CalcErrors.Reference);
        }

        protected override CalcExpression VisitRangeExpression(CalcRangeExpression expr, int baseRow, int baseColumn)
        {
            bool flag = (this._maxRowIndex.HasValue && !expr.IsFullColumn) && expr.StartRowRelative;
            bool flag2 = (this._maxColIndex.HasValue && !expr.IsFullRow) && expr.StartColumnRelative;
            bool flag3 = (this._maxRowIndex.HasValue && !expr.IsFullColumn) && expr.EndRowRelative;
            bool flag4 = (this._maxColIndex.HasValue && !expr.IsFullRow) && expr.EndColumnRelative;
            if ((!flag && !flag2) && (!flag3 && !flag4))
            {
                return expr;
            }
            CalcRangeIdentity identity = this.GetId(expr, baseRow, baseColumn) as CalcRangeIdentity;
            int num = (identity.RowIndex + identity.RowCount) - 1;
            int num2 = (identity.ColumnIndex + identity.ColumnCount) - 1;
            if (((!flag || ((identity.RowIndex >= 0) && (identity.RowIndex <= this._maxRowIndex.Value))) && (!flag2 || ((identity.ColumnIndex >= 0) && (identity.ColumnIndex <= this._maxColIndex.Value)))) && ((!flag3 || ((num >= 0) && (num <= this._maxRowIndex.Value))) && (!flag4 || ((num2 >= 0) && (num2 <= this._maxColIndex.Value)))))
            {
                return base.VisitRangeExpression(expr, baseRow, baseColumn);
            }
            return new CalcErrorExpression(CalcErrors.Reference);
        }

        protected override CalcExpression VisitSheetRangeExpression(CalcSheetRangeExpression expr, int baseRow, int baseColumn)
        {
            bool flag = (this._maxRowIndex.HasValue && !expr.IsFullColumn) && expr.StartRowRelative;
            bool flag2 = (this._maxColIndex.HasValue && !expr.IsFullRow) && expr.StartColumnRelative;
            bool flag3 = (this._maxRowIndex.HasValue && !expr.IsFullColumn) && expr.EndRowRelative;
            bool flag4 = (this._maxColIndex.HasValue && !expr.IsFullRow) && expr.EndColumnRelative;
            if ((!flag && !flag2) && (!flag3 && !flag4))
            {
                return expr;
            }
            int num = 0x7fffffff;
            int num2 = 0x7fffffff;
            IMultiSourceProvider startSource = expr.StartSource as IMultiSourceProvider;
            if (startSource != null)
            {
                foreach (ICalcSource source in startSource.GetCalcSources(expr.StartSource, expr.EndSource))
                {
                    num = (num >= source.GetRowCount()) ? (source.GetRowCount() - 1) : num;
                    num2 = (num2 >= source.GetColumnCount()) ? (source.GetColumnCount() - 1) : num2;
                }
            }
            else
            {
                num = expr.StartSource.GetRowCount() - 1;
                num2 = expr.EndSource.GetRowCount() - 1;
                num = (num >= expr.EndSource.GetRowCount()) ? (expr.EndSource.GetRowCount() - 1) : num;
                num2 = (num2 >= expr.EndSource.GetColumnCount()) ? (expr.EndSource.GetColumnCount() - 1) : num2;
            }
            CalcSheetRangeIdentity identity = this.GetId(expr, baseRow, baseColumn) as CalcSheetRangeIdentity;
            int num3 = (identity.RowIndex + identity.RowCount) - 1;
            int num4 = (identity.ColumnIndex + identity.ColumnCount) - 1;
            if (((!flag || ((identity.RowIndex >= 0) && (identity.RowIndex <= num))) && (!flag2 || ((identity.ColumnIndex >= 0) && (identity.ColumnIndex <= num2)))) && ((!flag3 || ((num3 >= 0) && (num3 <= num))) && (!flag4 || ((num4 >= 0) && (num4 <= num2)))))
            {
                return base.VisitSheetRangeExpression(expr, baseRow, baseColumn);
            }
            return new CalcErrorExpression(CalcErrors.Reference);
        }
    }
}

