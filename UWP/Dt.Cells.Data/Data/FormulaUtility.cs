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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    internal static class FormulaUtility
    {
        static CalcExpression AddColumnRange(int column, int columnCount, CalcReferenceExpression dataRange)
        {
            CalcRangeExpression expression = dataRange as CalcRangeExpression;
            if (expression != null)
            {
                if (column > expression.EndColumn)
                {
                    return expression;
                }
                if (column > expression.StartColumn)
                {
                    return expression.Offset(0, 0, 0, columnCount, true, true);
                }
                return (expression.Offset(0, columnCount, true, true) as CalcRangeExpression);
            }
            CalcExternalRangeExpression expression2 = dataRange as CalcExternalRangeExpression;
            if (expression2 == null)
            {
                return null;
            }
            if (column > expression2.EndColumn)
            {
                return expression2;
            }
            if (column > expression2.StartColumn)
            {
                return expression2.Offset(0, 0, 0, columnCount, true, true);
            }
            return (expression2.Offset(0, columnCount, true, true) as CalcExternalRangeExpression);
        }

        public static string AddColumnRange(ICalcEvaluator evaluator, string formula, int column, int columnCount)
        {
            CalcExpression expression = evaluator.Formula2Expression(formula, 0, 0) as CalcExpression;
            if (expression != null)
            {
                CalcReferenceExpression[] expressions = null;
                ExtractAllReferenceExpression(evaluator, expression, out expressions);
                List<CalcExpression> list = new List<CalcExpression>();
                foreach (CalcReferenceExpression expression2 in expressions)
                {
                    list.Add(AddColumnRange(column, columnCount, expression2));
                }
                if (list.Count > 0)
                {
                    return BuildFormula(evaluator, list);
                }
            }
            return null;
        }

        static CalcExpression AddRowRange(int row, int rowCount, CalcReferenceExpression dataRange)
        {
            CalcRangeExpression expression = dataRange as CalcRangeExpression;
            if (expression != null)
            {
                if (row > expression.EndRow)
                {
                    return expression;
                }
                if (row > expression.StartRow)
                {
                    return expression.Offset(0, 0, rowCount, 0, true, true);
                }
                return (expression.Offset(rowCount, 0, true, true) as CalcRangeExpression);
            }
            CalcExternalRangeExpression expression2 = dataRange as CalcExternalRangeExpression;
            if (expression2 == null)
            {
                return null;
            }
            if (row > expression2.EndRow)
            {
                return expression2;
            }
            if (row > expression2.StartRow)
            {
                return expression2.Offset(0, 0, rowCount, 0, true, true);
            }
            return (expression2.Offset(rowCount, 0, true, true) as CalcExternalRangeExpression);
        }

        public static string AddRowRange(ICalcEvaluator evaluator, string formula, int row, int rowCount)
        {
            CalcExpression expression = evaluator.Formula2Expression(formula, 0, 0) as CalcExpression;
            if (expression != null)
            {
                CalcReferenceExpression[] expressions = null;
                ExtractAllReferenceExpression(evaluator, expression, out expressions);
                List<CalcExpression> list = new List<CalcExpression>();
                foreach (CalcReferenceExpression expression2 in expressions)
                {
                    list.Add(AddRowRange(row, rowCount, expression2));
                }
                if (list.Count > 0)
                {
                    return BuildFormula(evaluator, list);
                }
            }
            return null;
        }

        static string AppendExternalName(object source)
        {
            StringBuilder builder = new StringBuilder();
            string str = (string) (source as string);
            if ((str != null) && (str.Length > 0))
            {
                bool flag = !char.IsLetter(str[0]) && (str[0] != '_');
                for (int i = 1; !flag && (i < str.Length); i++)
                {
                    flag = !char.IsLetterOrDigit(str[i]) && (str[i] != '_');
                }
                List<string> list = new List<string> { "#NAME?", "#N/A", "#NULL!", "#NUM!", "#REF!", "#VALUE!" };
                if (list.Contains(str.ToUpperInvariant()))
                {
                    flag = false;
                }
                if (flag)
                {
                    builder.Append("'");
                    builder.Append(str.Replace("'", "''"));
                    builder.Append("'");
                }
                else
                {
                    builder.Append(str);
                }
            }
            return builder.ToString();
        }

        public static string BuildFormula(ICalcEvaluator evaluator, List<CalcExpression> expressions)
        {
            if (expressions.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (CalcExpression expression in expressions)
            {
                if (expressions != null)
                {
                    string str = evaluator.Expression2Formula(expression, 0, 0);
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

        public static void ExtractAllReferenceExpression(ICalcEvaluator evaluator, CalcExpression expression, out CalcReferenceExpression[] expressions)
        {
            List<CalcReferenceExpression> nodes = new List<CalcReferenceExpression>();
            ExtractAllReferenceExpression(evaluator, expression, nodes);
            expressions = nodes.ToArray();
        }

        public static void ExtractAllReferenceExpression(ICalcEvaluator evaluator, CalcExpression root, List<CalcReferenceExpression> nodes)
        {
            if (root is CalcBinaryOperatorExpression)
            {
                CalcBinaryOperatorExpression expression = root as CalcBinaryOperatorExpression;
                ExtractAllReferenceExpression(evaluator, expression.Left, nodes);
                ExtractAllReferenceExpression(evaluator, expression.Right, nodes);
            }
            else if (root is CalcParenthesesExpression)
            {
                CalcParenthesesExpression expression2 = root as CalcParenthesesExpression;
                ExtractAllReferenceExpression(evaluator, expression2.Arg, nodes);
            }
            else if (root is CalcExternalNameExpression)
            {
                CalcExternalNameExpression expression3 = root as CalcExternalNameExpression;
                ICalcSource source = expression3.Source;
                if (source != null)
                {
                    CalcExpression expression4 = source.GetDefinedName(expression3.Name, -1, -1);
                    if (expression4 != null)
                    {
                        ExtractAllReferenceExpression(evaluator, expression4, nodes);
                    }
                }
            }
            else if (root is CalcFunctionExpression)
            {
                CalcFunctionExpression expr = root as CalcFunctionExpression;
                Worksheet worksheet = evaluator as Worksheet;
                if (worksheet != null)
                {
                    CalcEvaluatorContext context = new CalcEvaluatorContext(worksheet, false, worksheet.ActiveRowIndex, worksheet.ActiveColumnIndex, 1, 1);
                    object obj2 = new CalcEvaluator().Evaluate(expr, context, true, true);
                    if (obj2 is CalcReference)
                    {
                        CalcReference reference = obj2 as CalcReference;
                        int row = reference.GetRow(0);
                        int rowCount = reference.GetRowCount(0);
                        int column = reference.GetColumn(0);
                        int columnCount = reference.GetColumnCount(0);
                        ICalcSource source2 = null;
                        CalcReference reference2 = reference.GetSource();

                        // hdt
                        MethodInfo info = reference2.GetType().GetRuntimeMethod("GetContext", null);
                       
                        if (info != null)
                        {
                            source2 = info.Invoke(reference2, null) as ICalcSource;
                        }
                        if (source2 == null)
                        {
                            source2 = worksheet;
                        }
                        CalcExternalRangeExpression expression6 = CreateExternalRangeExpressionByCount(source2, row, column, rowCount, columnCount, false, false, false, false);
                        nodes.Add(expression6);
                    }
                }
            }
            else if (root is CalcReferenceExpression)
            {
                nodes.Add(root as CalcReferenceExpression);
            }
        }

        public static CalcExpression Formula2Expression(ICalcEvaluator evaluator, string formula)
        {
            if (!string.IsNullOrEmpty(formula))
            {
                try
                {
                    if (evaluator != null)
                    {
                        return (evaluator.Formula2Expression(formula, 0, 0) as CalcExpression);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }

        public static bool FormulaEquals(string oldFormula, string newFormula)
        {
            if (string.IsNullOrEmpty(oldFormula) && string.IsNullOrEmpty(newFormula))
            {
                return true;
            }
            if (((oldFormula != null) && (oldFormula.Length > 1)) && oldFormula.StartsWith("="))
            {
                oldFormula = oldFormula.Substring(1);
            }
            if (((newFormula != null) && (newFormula.Length > 1)) && newFormula.StartsWith("="))
            {
                newFormula = newFormula.Substring(1);
            }
            return (oldFormula == newFormula);
        }

        public static string ProcessFormula(string formula)
        {
            if (string.IsNullOrEmpty(formula))
            {
                return string.Empty;
            }
            if ((formula.Length > 2) && formula.StartsWith("="))
            {
                formula = formula.Substring(1);
            }
            string[] strArray = formula.Split(new char[] { ',' });
            if (strArray == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < strArray.Length; i++)
            {
                if (!string.IsNullOrEmpty(strArray[i]))
                {
                    builder.Append(QuotesFormula(strArray[i]));
                    builder.Append(",");
                }
            }
            return ("=" + builder.ToString().TrimEnd(new char[] { ',' }));
        }

        static string QuotesFormula(string s)
        {
            bool flag = false;
            string str = string.Empty;
            string str2 = string.Empty;
            if (s[0] == '\'')
            {
                string[] strArray = s.Split(new char[] { '!' });
                if (strArray.Length == 2)
                {
                    str = strArray[0];
                    if (((str != null) && (str.Length >= 2)) && ((str[0] == str[str.Length - 1]) && (str[0] == '\'')))
                    {
                        flag = false;
                    }
                    else
                    {
                        str = str.Substring(1, str.Length - 2);
                        str2 = strArray[1];
                        flag = true;
                    }
                }
            }
            else
            {
                string[] strArray2 = s.Split(new char[] { '!' });
                if (strArray2.Length == 2)
                {
                    str = strArray2[0];
                    str2 = strArray2[1];
                    flag = true;
                }
            }
            if ((flag && !string.IsNullOrEmpty(str)) && !string.IsNullOrEmpty(str2))
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(AppendExternalName(str));
                builder.Append("!");
                builder.Append(str2);
                return builder.ToString();
            }
            return s;
        }

        static CalcExpression RemoveColumnRange(int column, int columnCount, CalcReferenceExpression dataRange)
        {
            int targetEnd = (column + columnCount) - 1;
            CalcRangeExpression expression = dataRange as CalcRangeExpression;
            if (expression != null)
            {
                int resultStart = -1;
                int resultEnd = -1;
                SubCat(expression.StartColumn, expression.EndColumn, column, targetEnd, ref resultStart, ref resultEnd);
                if ((resultStart > -1) && (resultEnd > -1))
                {
                    return expression.Offset(0, resultStart - expression.StartColumn, 0, resultEnd - expression.EndColumn, true, true);
                }
            }
            else
            {
                CalcExternalRangeExpression expression2 = dataRange as CalcExternalRangeExpression;
                if (expression2 != null)
                {
                    int num4 = -1;
                    int num5 = -1;
                    SubCat(expression2.StartColumn, expression2.EndColumn, column, targetEnd, ref num4, ref num5);
                    if ((num4 > -1) && (num5 > -1))
                    {
                        return expression2.Offset(0, num4 - expression2.StartColumn, 0, num5 - expression2.EndColumn, true, true);
                    }
                }
            }
            return null;
        }

        public static string RemoveColumnRange(ICalcEvaluator evaluator, string formula, int column, int columnCount)
        {
            CalcExpression expression = evaluator.Formula2Expression(formula, 0, 0) as CalcExpression;
            if (expression != null)
            {
                CalcReferenceExpression[] expressions = null;
                ExtractAllReferenceExpression(evaluator, expression, out expressions);
                List<CalcExpression> list = new List<CalcExpression>();
                foreach (CalcReferenceExpression expression2 in expressions)
                {
                    CalcExpression expression3 = RemoveColumnRange(column, columnCount, expression2);
                    if (expression3 != null)
                    {
                        list.Add(expression3);
                    }
                }
                if (list.Count > 0)
                {
                    return BuildFormula(evaluator, list);
                }
            }
            return null;
        }

        static CalcExpression RemoveRowRange(int row, int rowCount, CalcReferenceExpression dataRange)
        {
            int targetEnd = (row + rowCount) - 1;
            CalcRangeExpression expression = dataRange as CalcRangeExpression;
            if (expression != null)
            {
                int resultStart = -1;
                int resultEnd = -1;
                SubCat(expression.StartRow, expression.EndRow, row, targetEnd, ref resultStart, ref resultEnd);
                if ((resultStart > -1) && (resultEnd > -1))
                {
                    return expression.Offset(resultStart - expression.StartRow, 0, resultEnd - expression.EndRow, 0, true, true);
                }
            }
            else
            {
                CalcExternalRangeExpression expression2 = dataRange as CalcExternalRangeExpression;
                if (expression2 != null)
                {
                    int num4 = -1;
                    int num5 = -1;
                    SubCat(expression2.StartRow, expression2.EndRow, row, targetEnd, ref num4, ref num5);
                    if ((num4 > -1) && (num5 > -1))
                    {
                        return expression2.Offset(num4 - expression2.StartRow, 0, num5 - expression2.EndRow, 0, true, true);
                    }
                }
            }
            return null;
        }

        public static string RemoveRowRange(ICalcEvaluator evaluator, string formula, int row, int rowCount)
        {
            CalcExpression expression = evaluator.Formula2Expression(formula, 0, 0) as CalcExpression;
            if (expression != null)
            {
                CalcReferenceExpression[] expressions = null;
                ExtractAllReferenceExpression(evaluator, expression, out expressions);
                List<CalcExpression> list = new List<CalcExpression>();
                foreach (CalcReferenceExpression expression2 in expressions)
                {
                    CalcExpression expression3 = RemoveRowRange(row, rowCount, expression2);
                    if (expression3 != null)
                    {
                        list.Add(expression3);
                    }
                }
                if (list.Count > 0)
                {
                    return BuildFormula(evaluator, list);
                }
            }
            return null;
        }

        static void SubCat(int sourceStart, int sourceEnd, int targetStart, int targetEnd, ref int resultStart, ref int resultEnd)
        {
            if (targetEnd < sourceStart)
            {
                int num = (targetEnd - targetStart) + 1;
                resultStart = sourceStart - num;
                resultEnd = sourceEnd - num;
            }
            else if (targetStart > sourceEnd)
            {
                resultStart = sourceStart;
                resultEnd = sourceEnd;
            }
            else if (targetStart > sourceStart)
            {
                int num5 = 0;
                for (int i = sourceStart; i <= sourceEnd; i++)
                {
                    if ((resultStart == -1) && ((i < targetStart) || (i > targetEnd)))
                    {
                        resultStart = i;
                    }
                    if ((i < targetStart) || (i > targetEnd))
                    {
                        num5++;
                    }
                }
                if ((resultStart != -1) && (num5 > 0))
                {
                    resultEnd = (resultStart + num5) - 1;
                }
            }
            else
            {
                int num2 = (sourceEnd - sourceStart) + 1;
                int num3 = 0;
                for (int j = sourceStart; j <= targetEnd; j++)
                {
                    if (j > sourceEnd)
                    {
                        break;
                    }
                    num3++;
                }
                resultStart = targetStart;
                resultEnd = ((resultStart + num2) - num3) - 1;
            }
        }

        public static List<List<SheetCellRange>> TryCombineSeriesRange(List<List<SheetCellRange>> seriesRanges, DataOrientation dataOrientation)
        {
            List<List<SheetCellRange>> list = new List<List<SheetCellRange>>();
            for (int i = 0; i < seriesRanges.Count; i++)
            {
                List<SheetCellRange> list2 = seriesRanges[i];
                if ((list2 != null) && (list2.Count != 0))
                {
                    if (list.Count == 0)
                    {
                        List<SheetCellRange> list3 = new List<SheetCellRange>();
                        for (int k = 0; k < list2.Count; k++)
                        {
                            list3.Add(list2[k].Clone() as SheetCellRange);
                        }
                        list.Add(list3);
                        continue;
                    }
                    List<SheetCellRange> list4 = list[list.Count - 1];
                    if (list2.Count != list4.Count)
                    {
                        List<SheetCellRange> list5 = new List<SheetCellRange>();
                        for (int m = 0; m < list2.Count; m++)
                        {
                            list5.Add(list2[m].Clone() as SheetCellRange);
                        }
                        list.Add(list5);
                        continue;
                    }
                    bool flag = true;
                    for (int j = 0; j < list2.Count; j++)
                    {
                        SheetCellRange range = list2[j];
                        SheetCellRange range2 = list4[j];
                        if (dataOrientation == DataOrientation.Vertical)
                        {
                            if (((range.Row == range2.Row) && (range.RowCount == range2.RowCount)) && (range.Column == (range2.Column + range2.ColumnCount)))
                            {
                                continue;
                            }
                            flag = false;
                            break;
                        }
                        if (((range.Column != range2.Column) || (range.ColumnCount != range2.ColumnCount)) || (range.Row != (range2.Row + range2.RowCount)))
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        list.Remove(list4);
                        List<SheetCellRange> list6 = new List<SheetCellRange>();
                        for (int n = 0; n < list2.Count; n++)
                        {
                            SheetCellRange range3 = list2[n];
                            SheetCellRange range4 = list4[n];
                            if (dataOrientation == DataOrientation.Vertical)
                            {
                                SheetCellRange range5 = new SheetCellRange(range4.Sheet, range4.Row, range4.Column, range4.RowCount, range4.ColumnCount + range3.ColumnCount);
                                list6.Add(range5);
                            }
                            else
                            {
                                SheetCellRange range6 = new SheetCellRange(range4.Sheet, range4.Row, range4.Column, range4.RowCount + range3.RowCount, range4.ColumnCount);
                                list6.Add(range6);
                            }
                        }
                        list.Add(list6);
                    }
                    else
                    {
                        List<SheetCellRange> list7 = new List<SheetCellRange>();
                        for (int num6 = 0; num6 < list2.Count; num6++)
                        {
                            list7.Add(list2[num6].Clone() as SheetCellRange);
                        }
                        list.Add(list7);
                    }
                }
            }
            List<List<SheetCellRange>> list8 = new List<List<SheetCellRange>>();
            foreach (List<SheetCellRange> list9 in list)
            {
                if (list9.Count != 0)
                {
                    List<SheetCellRange> list10 = new List<SheetCellRange> {
                        list9[0]
                    };
                    if (dataOrientation == DataOrientation.Vertical)
                    {
                        for (int num7 = 1; num7 < list9.Count; num7++)
                        {
                            SheetCellRange range7 = list10[list10.Count - 1];
                            SheetCellRange range8 = list9[num7];
                            if ((range7.Row + range7.RowCount) == range8.Row)
                            {
                                list10.Remove(range7);
                                SheetCellRange range9 = new SheetCellRange(range7.Sheet, range7.Row, range7.Column, range7.RowCount + range8.RowCount, range7.ColumnCount);
                                list10.Add(range9);
                            }
                            else
                            {
                                list10.Add(range8);
                            }
                        }
                    }
                    else
                    {
                        for (int num8 = 1; num8 < list9.Count; num8++)
                        {
                            SheetCellRange range10 = list10[list10.Count - 1];
                            SheetCellRange range11 = list9[num8];
                            if ((range10.Column + range10.ColumnCount) == range11.Column)
                            {
                                list10.Remove(range10);
                                SheetCellRange range12 = new SheetCellRange(range10.Sheet, range10.Row, range10.Column, range10.RowCount, range10.ColumnCount + range11.ColumnCount);
                                list10.Add(range12);
                            }
                            else
                            {
                                list10.Add(range11);
                            }
                        }
                    }
                    list8.Add(list10);
                }
            }
            return list8;
        }
    }
}

