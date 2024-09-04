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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    internal static class CalcExpressionHelper
    {
        public const int MAX_COLUMN_INDEX = 0x3fff;
        public const int MAX_ROW_INDEX = 0xfffff;

        public static CalcExternalRangeExpression CreateExternalRangeExpressionByCount(ICalcSource source, int row, int column, int rowCount, int columnCount, bool startRowRelative = false, bool startColumnRelative = false, bool endRowRelative = false, bool endColumnRelative = false)
        {
            new CalcCellIdentity(row, column);
            if ((rowCount == -1) && (rowCount == -1))
            {
                return new CalcExternalRangeExpression(source);
            }
            if (columnCount == -1)
            {
                return new CalcExternalRangeExpression(source, row, (row + rowCount) - 1, startRowRelative, endRowRelative, true);
            }
            if (rowCount == -1)
            {
                return new CalcExternalRangeExpression(source, column, (column + columnCount) - 1, startColumnRelative, endColumnRelative, false);
            }
            return new CalcExternalRangeExpression(source, row, column, (row + rowCount) - 1, (column + columnCount) - 1, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
        }

        public static CalcRangeExpression CreateRangeExpressionByCount(int row, int column, int rowCount, int columnCount, bool startRowRelative = false, bool startColumnRelative = false, bool endRowRelative = false, bool endColumnRelative = false)
        {
            new CalcCellIdentity(row, column);
            if (((rowCount == -1) && (columnCount == -1)) || ((row == -1) && (column == -1)))
            {
                return new CalcRangeExpression();
            }
            if ((columnCount == -1) || (column == -1))
            {
                return new CalcRangeExpression(row, (row + rowCount) - 1, startRowRelative, endRowRelative, true);
            }
            if ((rowCount != -1) && (row != -1))
            {
                return new CalcRangeExpression(row, column, (row + rowCount) - 1, (column + columnCount) - 1, startRowRelative, startColumnRelative, endRowRelative, endColumnRelative);
            }
            return new CalcRangeExpression(column, (column + columnCount) - 1, startColumnRelative, endColumnRelative, false);
        }

        public static void SetExpression(CalcCalculationManager mgr, CalcLocalIdentity id, CalcExpression expr, bool? isArrayFormula = new bool?())
        {
            if (id is CalcCellIdentity)
            {
                mgr.SetExpression(id as CalcCellIdentity, expr, false);
            }
            if (id is CalcRangeIdentity)
            {
                if (!isArrayFormula.HasValue)
                {
                    isArrayFormula = new bool?(!(expr is CalcSharedExpression));
                }
                else
                {
                    CalcSharedExpression expression1 = expr as CalcSharedExpression;
                }
                mgr.SetExpression(id as CalcRangeIdentity, expr, isArrayFormula.Value);
            }
        }

        public static void SetFormula(CalcCalculationManager mgr, CalcLocalIdentity id, string fromula, bool isArrayFormual)
        {
            if (id is CalcCellIdentity)
            {
                mgr.SetFormula(id as CalcCellIdentity, fromula, isArrayFormual);
            }
            if (id is CalcRangeIdentity)
            {
                mgr.SetFormula(id as CalcRangeIdentity, fromula, isArrayFormual);
            }
        }

        public static void ValidateIntersectedArrayFormula(CalcCalculationManager mgr, int row, int column, int rowCount, int columnCount, bool isInsertRowOrColumn)
        {
            List<CalcRangeIdentity> list;
            List<CalcRangeIdentity> list2;
            CalcRangeIdentity objA = FormulaOperatorHelper.Indexs2Identity(row, column, (row + rowCount) - 1, (column + columnCount) - 1) as CalcRangeIdentity;
            if (object.ReferenceEquals(objA, null))
            {
                objA = new CalcRangeIdentity(row, column, 1, 1);
            }
            mgr.GetArrayFormulaByRange(objA, out list, out list2);
            if ((list2 != null) && (list2.Count > 0))
            {
                foreach (CalcRangeIdentity identity2 in list2)
                {
                    if (!isInsertRowOrColumn || (((!identity2.IsFullRow && !objA.IsFullRow) || (identity2.RowIndex < row)) && ((!identity2.IsFullColumn && !objA.IsFullColumn) || (identity2.ColumnIndex < column))))
                    {
                        throw new InvalidOperationException(ResourceStrings.FormulaChangePartOfArrayFormulaError);
                    }
                }
            }
        }
    }
}

