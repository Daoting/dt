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
#endregion

namespace Dt.Cells.Data
{
    internal static class CellRangUtility
    {
        public static string FormatCellRange(Worksheet worksheet, CellRange cellRange)
        {
            CalcReferenceExpression expressionFromCellRange = GetExpressionFromCellRange(worksheet, cellRange);
            List<CalcExpression> expressions = new List<CalcExpression> {
                expressionFromCellRange
            };
            return FormulaUtility.BuildFormula(worksheet, expressions);
        }

        public static CalcReferenceExpression GetExpressionFromCellRange(Worksheet worksheet, CellRange range)
        {
            if (IsSheetRange(range))
            {
                return new CalcRangeExpression(0, (worksheet != null) ? (worksheet.RowCount - 1) : 0, false, false, true);
            }
            if (IsColumnRange(range))
            {
                return new CalcRangeExpression(range.Column, (range.Column + range.ColumnCount) - 1, false, false, false);
            }
            if (IsRowRange(range))
            {
                return new CalcRangeExpression(range.Row, (range.Row + range.RowCount) - 1, false, false, true);
            }
            if (IsSigleCellRange(range))
            {
                return new CalcCellExpression(range.Row, range.Column);
            }
            return new CalcRangeExpression(range.Row, range.Column, (range.Row + range.RowCount) - 1, (range.Column + range.ColumnCount) - 1);
        }

        public static CellRange GetRangeFromExpression(CalcReferenceExpression reference)
        {
            int row = 0;
            int column = 0;
            int rowCount = 0;
            int columnCount = 0;
            if (reference is CalcCellExpression)
            {
                CalcCellExpression expression = reference as CalcCellExpression;
                row = expression.Row;
                column = expression.Column;
                rowCount = 1;
                columnCount = 1;
            }
            else if (reference is CalcRangeExpression)
            {
                CalcRangeExpression expression2 = reference as CalcRangeExpression;
                if (expression2.IsFullColumn)
                {
                    row = -1;
                    column = expression2.StartColumn;
                    rowCount = -1;
                    columnCount = (expression2.EndColumn - expression2.StartColumn) + 1;
                }
                else if (expression2.IsFullRow)
                {
                    row = expression2.StartRow;
                    column = -1;
                    rowCount = (expression2.EndRow - expression2.StartRow) + 1;
                    columnCount = -1;
                }
                else
                {
                    row = expression2.StartRow;
                    column = expression2.StartColumn;
                    rowCount = (expression2.EndRow - expression2.StartRow) + 1;
                    columnCount = (expression2.EndColumn - expression2.StartColumn) + 1;
                }
            }
            else if (reference is CalcExternalCellExpression)
            {
                CalcExternalCellExpression expression3 = reference as CalcExternalCellExpression;
                row = expression3.Row;
                column = expression3.Column;
                rowCount = 1;
                columnCount = 1;
            }
            else if (reference is CalcExternalRangeExpression)
            {
                CalcExternalRangeExpression expression4 = reference as CalcExternalRangeExpression;
                if (expression4.IsFullColumn)
                {
                    row = -1;
                    column = expression4.StartColumn;
                    rowCount = -1;
                    columnCount = (expression4.EndColumn - expression4.StartColumn) + 1;
                }
                else if (expression4.IsFullRow)
                {
                    row = expression4.StartRow;
                    column = -1;
                    rowCount = (expression4.EndRow - expression4.StartRow) + 1;
                    columnCount = -1;
                }
                else
                {
                    row = expression4.StartRow;
                    column = expression4.StartColumn;
                    rowCount = (expression4.EndRow - expression4.StartRow) + 1;
                    columnCount = (expression4.EndColumn - expression4.StartColumn) + 1;
                }
            }
            return new CellRange(row, column, rowCount, columnCount);
        }

        public static bool IsColumnRange(CellRange cellRange)
        {
            return ((((cellRange.Row == -1) && (cellRange.RowCount == -1)) && (cellRange.Column != -1)) && (cellRange.ColumnCount != -1));
        }

        public static bool IsRowRange(CellRange cellRange)
        {
            return ((((cellRange.Column == -1) && (cellRange.ColumnCount == -1)) && (cellRange.Row != -1)) && (cellRange.RowCount != -1));
        }

        public static bool IsSheetRange(CellRange cellRange)
        {
            return ((((cellRange.Row == -1) && (cellRange.RowCount == -1)) && (cellRange.Column == -1)) && (cellRange.ColumnCount == -1));
        }

        public static bool IsSigleCellRange(CellRange cellRange)
        {
            return ((((cellRange.Row > -1) && (cellRange.Column > -1)) && (cellRange.RowCount == 1)) && (cellRange.ColumnCount == 1));
        }

        public static CellRange ParseCellRange(Worksheet worksheet, string cellRange)
        {
            CalcExpression expression = FormulaUtility.Formula2Expression(worksheet, cellRange);
            if (expression is CalcReferenceExpression)
            {
                return GetRangeFromExpression(expression as CalcReferenceExpression);
            }
            if (expression is CalcNameExpression)
            {
                CalcNameExpression expression2 = expression as CalcNameExpression;
                NameInfo customName = worksheet.GetCustomName(expression2.Name);
                if ((customName != null) && (customName.Expression is CalcReferenceExpression))
                {
                    return GetRangeFromExpression(customName.Expression as CalcReferenceExpression);
                }
            }
            else if (expression is CalcExternalNameExpression)
            {
                CalcExternalNameExpression expression3 = expression as CalcExternalNameExpression;
                NameInfo info2 = worksheet.GetCustomName(expression3.Name);
                if ((info2 != null) && (info2.Expression is CalcReferenceExpression))
                {
                    return GetRangeFromExpression(info2.Expression as CalcReferenceExpression);
                }
            }
            return null;
        }

        public static CellRange[] ParseRanges(Worksheet worksheet, string rangeString)
        {
            if (rangeString.StartsWith("="))
            {
                rangeString = rangeString.Substring(1);
            }
            if (string.IsNullOrEmpty(rangeString))
            {
                return null;
            }
            List<CellRange> list = new List<CellRange>();
            foreach (string str in rangeString.Split(new char[] { ',' }))
            {
                CellRange range = ParseCellRange(worksheet, str);
                if (range != null)
                {
                    list.Add(range);
                }
            }
            return list.ToArray();
        }
    }
}

