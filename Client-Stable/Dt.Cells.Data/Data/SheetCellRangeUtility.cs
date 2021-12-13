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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    internal static class SheetCellRangeUtility
    {
        public static string BuildFormula(ICalcEvaluator evaluator, List<SheetCellRange> ranges)
        {
            if ((ranges == null) || (ranges.Count == 0))
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            foreach (SheetCellRange range in ranges)
            {
                if (ranges != null)
                {
                    string str = SheetRange2Formula(evaluator, range);
                    builder.Append(str);
                    builder.Append(CultureInfo.InvariantCulture.TextInfo.ListSeparator);
                }
            }
            string str2 = builder.ToString();
            if (str2 != string.Empty)
            {
                int startIndex = str2.LastIndexOf(CultureInfo.InvariantCulture.TextInfo.ListSeparator);
                if (startIndex != -1)
                {
                    str2 = str2.Remove(startIndex);
                }
            }
            return str2;
        }

        public static string BuildFormula(ICalcEvaluator evaluator, List<List<SheetCellRange>> ranges)
        {
            if ((ranges == null) || (ranges.Count == 0))
            {
                return null;
            }
            string str = string.Empty;
            foreach (List<SheetCellRange> list in ranges)
            {
                string str2 = BuildFormula(evaluator, list);
                str = str + str2 + CultureInfo.InvariantCulture.TextInfo.ListSeparator;
            }
            if (str != string.Empty)
            {
                int startIndex = str.LastIndexOf(CultureInfo.InvariantCulture.TextInfo.ListSeparator);
                if (startIndex != -1)
                {
                    str = str.Remove(startIndex);
                }
            }
            return str;
        }

        static CalcExternalRangeExpression CreateExternalRangeExpressionByCount(ICalcSource source, int row, int column, int rowCount, int columnCount, bool startRowRelative = false, bool startColumnRelative = false, bool endRowRelative = false, bool endColumnRelative = false)
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

        public static SheetCellRange[] ExtractAllExternalReference(ICalcEvaluator evaluator, CalcExpression expression)
        {
            if (expression != null)
            {
                List<CalcReferenceExpression> nodes = new List<CalcReferenceExpression>();
                FormulaUtility.ExtractAllReferenceExpression(evaluator, expression, nodes);
                if (nodes.Count > 0)
                {
                    List<SheetCellRange> list2 = new List<SheetCellRange>();
                    foreach (CalcReferenceExpression expression2 in nodes)
                    {
                        if (expression2 is CalcExternalRangeExpression)
                        {
                            CalcExternalRangeExpression expression3 = expression2 as CalcExternalRangeExpression;
                            SheetCellRange range = new SheetCellRange(expression3.Source as Worksheet, expression3.StartRow, expression3.StartColumn, (expression3.EndRow - expression3.StartRow) + 1, (expression3.EndColumn - expression3.StartColumn) + 1);
                            list2.Add(range);
                        }
                        else if (expression2 is CalcExternalCellExpression)
                        {
                            CalcExternalCellExpression expression4 = expression2 as CalcExternalCellExpression;
                            SheetCellRange range2 = new SheetCellRange(expression4.Source as Worksheet, expression4.Row, expression4.Column, 1, 1);
                            list2.Add(range2);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    return list2.ToArray();
                }
            }
            return null;
        }

        public static SheetCellRange[] ExtractAllExternalReference(ICalcEvaluator evaluator, string formula)
        {
            CalcExpression expression = FormulaUtility.Formula2Expression(evaluator, formula);
            if (expression == null)
            {
                return null;
            }
            return ExtractAllExternalReference(evaluator, expression);
        }

        public static SheetCellRange Formula2SheetRange(ICalcEvaluator evaluator, string formula)
        {
            CalcExternalRangeExpression expression = FormulaUtility.Formula2Expression(evaluator, formula) as CalcExternalRangeExpression;
            if (expression == null)
            {
                return null;
            }
            return new SheetCellRange(expression.Source as Worksheet, expression.StartRow, expression.StartColumn, (expression.EndRow - expression.StartRow) + 1, (expression.EndColumn - expression.StartColumn) + 1);
        }

        public static object[] GetColumnValues(SheetCellRange valuesRange, int column)
        {
            object[,] rangeValues = GetRangeValues(valuesRange);
            int length = rangeValues.GetLength(0);
            List<object> list = new List<object>(length);
            for (int i = 0; i < length; i++)
            {
                object obj2 = rangeValues[i, column];
                list.Add(obj2);
            }
            return list.ToArray();
        }

        public static object[,] GetRangeValues(SheetCellRange valuesRange)
        {
            if ((valuesRange != null) && (valuesRange.Sheet != null))
            {
                CalcExternalExpression expression = CreateExternalRangeExpressionByCount(valuesRange.Sheet, valuesRange.Row, valuesRange.Column, valuesRange.RowCount, valuesRange.ColumnCount, true, true, true, true);
                return (((ICalcEvaluator) valuesRange.Sheet).EvaluateExpression(expression, 0, 0, 0, 0, true) as object[,]);
            }
            return null;
        }

        public static object[] GetRowValues(SheetCellRange valuesRange, int row)
        {
            object[,] rangeValues = GetRangeValues(valuesRange);
            int length = rangeValues.GetLength(1);
            List<object> list = new List<object>(length);
            for (int i = 0; i < length; i++)
            {
                object obj2 = rangeValues[row, i];
                list.Add(obj2);
            }
            return list.ToArray();
        }

        public static string SheetRange2Formula(ICalcEvaluator evaluator, SheetCellRange range)
        {
            if (evaluator == null)
            {
                return null;
            }
            string str = null;
            try
            {
                CalcExternalRangeExpression expression = new CalcExternalRangeExpression(range.Sheet, range.Row, range.Column, (range.Row + range.RowCount) - 1, (range.Column + range.ColumnCount) - 1, false, false, false, false);
                str = evaluator.Expression2Formula(expression, 0, 0);
            }
            catch (Exception)
            {
            }
            return str;
        }

        public static List<SheetCellRange> SplitToColumnRanges(SheetCellRange[] ranges, int columnStripSize)
        {
            List<SheetCellRange> list = new List<SheetCellRange>();
            if (ranges != null)
            {
                foreach (SheetCellRange range in ranges)
                {
                    int num = (range.Column + range.ColumnCount) - 1;
                    for (int i = range.Column; i < (range.Column + range.ColumnCount); i += columnStripSize)
                    {
                        int num3 = (i + columnStripSize) - 1;
                        if (num3 <= num)
                        {
                            list.Add(new SheetCellRange(range.Sheet, range.Row, i, range.RowCount, columnStripSize));
                        }
                        else
                        {
                            int columnCount = (num - i) + 1;
                            list.Add(new SheetCellRange(range.Sheet, range.Row, i, range.RowCount, columnCount));
                        }
                    }
                }
            }
            return list;
        }

        public static List<SheetCellRange> SplitToRowRanges(SheetCellRange[] ranges, int rowStripSize)
        {
            List<SheetCellRange> list = new List<SheetCellRange>();
            if (ranges != null)
            {
                foreach (SheetCellRange range in ranges)
                {
                    int num = (range.Row + range.RowCount) - 1;
                    for (int i = range.Row; i < (range.Row + range.RowCount); i += rowStripSize)
                    {
                        int num3 = (i + rowStripSize) - 1;
                        if (num3 <= num)
                        {
                            list.Add(new SheetCellRange(range.Sheet, i, range.Column, rowStripSize, range.ColumnCount));
                        }
                        else
                        {
                            int columnCount = (num - i) + 1;
                            list.Add(new SheetCellRange(range.Sheet, i, range.Column, rowStripSize, columnCount));
                        }
                    }
                }
            }
            return list;
        }
    }
}

