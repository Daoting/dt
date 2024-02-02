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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
#endregion

namespace Dt.Cells.Data
{
    internal class WorksheetSparklineGroupManager : IRangeSupport, ICrossSheetRangeSupport, IDisposable, IXmlSerializable
    {
        ICalcEvaluator evaluator;
        List<SparklineGroup> groups;
        ISparklineSheet sheet;

        /// <summary>
        /// This method is only used for XML deserialization. Do not use it in other cases.
        /// </summary>
        public WorksheetSparklineGroupManager()
        {
            this.Init();
        }

        public WorksheetSparklineGroupManager(ISparklineSheet sheet, ICalcEvaluator calcEvaluator)
        {
            this.Init();
            this.Sheet = sheet;
            this.CalcEvaluator = calcEvaluator;
        }

        public void Add(SparklineGroup group)
        {
            this.Groups.Add(group);
            group.SparklineGroupManager = this;
        }

        static CalcExpression AddColumnRange(int column, int columnCount, CalcExpression dataRange)
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

        public void AddColumns(int column, int count)
        {
            foreach (SparklineGroup group in this.Groups)
            {
                if (group.DisplayDateAxis)
                {
                    CalcExpression expression = AddColumnRange(column, count, group.DateAxisReference);
                    if (expression != group.DateAxisReference)
                    {
                        group.DateAxisReference = expression;
                    }
                }
                foreach (Sparkline sparkline in group)
                {
                    if (column <= sparkline.Column)
                    {
                        sparkline.Column += count;
                    }
                    CalcExpression expression2 = AddColumnRange(column, count, sparkline.DataReference);
                    if (expression2 != sparkline.DataReference)
                    {
                        sparkline.DataReference = expression2;
                    }
                }
            }
        }

        static CalcExpression AddRowRange(int row, int rowCount, CalcExpression dataRange)
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

        public void AddRows(int row, int count)
        {
            foreach (SparklineGroup group in this.Groups)
            {
                if (group.DisplayDateAxis)
                {
                    CalcExpression expression = AddRowRange(row, count, group.DateAxisReference);
                    if (expression != group.DateAxisReference)
                    {
                        group.DateAxisReference = expression;
                    }
                }
                foreach (Sparkline sparkline in group)
                {
                    if (row <= sparkline.Row)
                    {
                        sparkline.Row += count;
                    }
                    CalcExpression expression2 = AddRowRange(row, count, sparkline.DataReference);
                    if (expression2 != sparkline.DataReference)
                    {
                        sparkline.DataReference = expression2;
                    }
                }
            }
        }

        internal void AtachEvents()
        {
            if (this.sheet != null)
            {
                this.sheet.CellChanged += new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
            }
        }

        static bool CanOffset(CalcExpression exp, int offsetRow, int offsetColumn, int MAX_ROW_COUNT, int MAX_COLUMN_COUNT)
        {
            CellRange expressionRange = GetExpressionRange(exp);
            if (expressionRange == null)
            {
                return false;
            }
            int num = (expressionRange.Row < 0) ? 0 : expressionRange.Row;
            int num2 = (expressionRange.Column < 0) ? 0 : expressionRange.Column;
            int num3 = (expressionRange.Row < 0) ? MAX_ROW_COUNT : expressionRange.RowCount;
            int num4 = (expressionRange.Column < 0) ? MAX_COLUMN_COUNT : expressionRange.ColumnCount;
            return ((((num + offsetRow) >= 0) && ((num2 + offsetColumn) >= 0)) && ((((num + num3) + offsetRow) <= MAX_ROW_COUNT) && (((num2 + num4) + offsetColumn) <= MAX_COLUMN_COUNT)));
        }

        public void Clear()
        {
            foreach (SparklineGroup group in this.Groups)
            {
                if (group != null)
                {
                    group.SparklineGroupManager = null;
                }
            }
            this.Groups.Clear();
        }

        public void Clear(int row, int column, int rowCount, int columnCount)
        {
            for (int i = row; i < (row + rowCount); i++)
            {
                for (int j = column; j < (column + columnCount); j++)
                {
                    Sparkline item = this.Find(i, j);
                    if (item != null)
                    {
                        SparklineGroup group = item.Group;
                        group.Remove(item);
                        if (group.Count == 0)
                        {
                            this.Remove(group);
                        }
                    }
                }
            }
        }

        public bool Contains(SparklineGroup group)
        {
            return this.Groups.Contains(group);
        }

        static bool ContainsCell(CalcExpression exp, int row, int column)
        {
            CalcRangeExpression expression = exp as CalcRangeExpression;
            if (expression != null)
            {
                return ((((row >= expression.StartRow) && (row <= expression.EndRow)) && (column >= expression.StartColumn)) && (column <= expression.EndColumn));
            }
            CalcExternalRangeExpression expression2 = exp as CalcExternalRangeExpression;
            if (expression2 == null)
            {
                return false;
            }
            return ((((row >= expression2.StartRow) && (row <= expression2.EndRow)) && (column >= expression2.StartColumn)) && (column <= expression2.EndColumn));
        }

        static CalcExpression ConvertToExternal(ICalcSource extSource, CalcExpression exp)
        {
            if (exp is CalcRangeExpression)
            {
                CalcRangeExpression expression = (CalcRangeExpression) exp;
                if (expression.IsFullRow && expression.IsFullColumn)
                {
                    return new CalcExternalRangeExpression(extSource);
                }
                if (expression.IsFullRow)
                {
                    return new CalcExternalRangeExpression(extSource, expression.StartRow, expression.EndRow, expression.StartRowRelative, expression.EndRowRelative, true);
                }
                if (expression.IsFullColumn)
                {
                    return new CalcExternalRangeExpression(extSource, expression.StartColumn, expression.EndColumn, expression.StartColumnRelative, expression.EndColumnRelative, false);
                }
                return new CalcExternalRangeExpression(extSource, expression.StartRow, expression.StartColumn, expression.EndRow, expression.EndColumn, expression.StartRowRelative, expression.StartColumnRelative, expression.EndRowRelative, expression.EndColumnRelative);
            }
            if (exp is CalcCellExpression)
            {
                CalcCellExpression expression2 = (CalcCellExpression) exp;
                return new CalcExternalCellExpression(extSource, expression2.Row, expression2.Column, expression2.RowRelative, expression2.ColumnRelative);
            }
            if (exp is CalcExternalRangeExpression)
            {
                CalcExternalRangeExpression expression3 = (CalcExternalRangeExpression) exp;
                if (expression3.Source != extSource)
                {
                    return new CalcExternalRangeExpression(extSource, expression3.StartRow, expression3.StartColumn, expression3.EndRow, expression3.EndColumn, expression3.StartRowRelative, expression3.StartColumnRelative, expression3.EndRowRelative, expression3.EndColumnRelative);
                }
                return exp;
            }
            if (exp is CalcExternalCellExpression)
            {
                CalcExternalCellExpression expression4 = (CalcExternalCellExpression) exp;
                if (expression4.Source != extSource)
                {
                    return new CalcExternalCellExpression(extSource, expression4.Row, expression4.Column, expression4.RowRelative, expression4.ColumnRelative);
                }
            }
            return exp;
        }

        public void Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            DataMatrix<Sparkline> matrix = new DataMatrix<Sparkline>(rowCount, columnCount);
            int offsetRow = toRow - fromRow;
            int offsetColumn = toColumn - fromColumn;
            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    Sparkline sparkline = this.Sheet.GetSparkline(fromRow + i, fromColumn + k);
                    if (sparkline != null)
                    {
                        Sparkline sparkline2 = sparkline.Clone();
                        sparkline2.Row = toRow + i;
                        sparkline2.Column = toColumn + k;
                        CalcExpression dateAxisReference = sparkline2.DateAxisReference;
                        if (dateAxisReference != null)
                        {
                            if (CanOffset(dateAxisReference, offsetRow, offsetColumn, ((Worksheet) this.Sheet).RowCount, ((Worksheet) this.Sheet).ColumnCount))
                            {
                                sparkline2.DateAxisReference = dateAxisReference.Offset(offsetRow, offsetColumn, false, true);
                            }
                            else
                            {
                                sparkline2.DateAxisReference = new CalcErrorExpression(CalcErrors.Reference);
                            }
                        }
                        CalcExpression dataReference = sparkline2.DataReference;
                        if (dataReference != null)
                        {
                            if (CanOffset(dataReference, offsetRow, offsetColumn, ((Worksheet) this.Sheet).RowCount, ((Worksheet) this.Sheet).ColumnCount))
                            {
                                sparkline2.DataReference = dataReference.Offset(offsetRow, offsetColumn, false, true);
                            }
                            else
                            {
                                sparkline2.DataReference = new CalcErrorExpression(CalcErrors.Reference);
                            }
                        }
                        this.Add(sparkline2.Group);
                        matrix.SetValue(i, k, sparkline2);
                    }
                }
            }
            for (int j = 0; j < rowCount; j++)
            {
                for (int m = 0; m < columnCount; m++)
                {
                    this.Sheet.SetSparkline(toRow + j, toColumn + m, matrix.GetValue(j, m));
                }
            }
        }

        public void Copy(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (object.ReferenceEquals(src, this.Sheet))
            {
                this.Copy(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
            else
            {
                int offsetRow = toRow - fromRow;
                int offsetColumn = toColumn - fromColumn;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        Sparkline sparkline = src.GetSparkline(fromRow + i, fromColumn + j);
                        if (sparkline != null)
                        {
                            Sparkline sparkline2 = sparkline.Clone();
                            sparkline2.Row = toRow + i;
                            sparkline2.Column = toColumn + j;
                            CalcExpression dateAxisReference = sparkline2.DateAxisReference;
                            if (dateAxisReference != null)
                            {
                                if (CanOffset(dateAxisReference, offsetRow, offsetColumn, ((Worksheet) this.Sheet).RowCount, ((Worksheet) this.Sheet).ColumnCount))
                                {
                                    sparkline2.DateAxisReference = dateAxisReference.Offset(offsetRow, offsetColumn, false, true);
                                }
                                else
                                {
                                    sparkline2.DateAxisReference = new CalcErrorExpression(CalcErrors.Reference);
                                }
                            }
                            CalcExpression dataReference = sparkline2.DataReference;
                            if (dataReference != null)
                            {
                                if (CanOffset(dataReference, offsetRow, offsetColumn, ((Worksheet) this.Sheet).RowCount, ((Worksheet) this.Sheet).ColumnCount))
                                {
                                    sparkline2.DataReference = dataReference.Offset(offsetRow, offsetColumn, false, true);
                                }
                                else
                                {
                                    sparkline2.DataReference = new CalcErrorExpression(CalcErrors.Reference);
                                }
                            }
                            this.Add(sparkline2.Group);
                            this.Sheet.SetSparkline(toRow + i, toColumn + j, sparkline2);
                        }
                        else
                        {
                            this.Sheet.SetSparkline(toRow + i, toColumn + j, null);
                        }
                    }
                }
            }
        }

        internal void DetachEvents()
        {
            if (this.sheet != null)
            {
                this.sheet.CellChanged -= new EventHandler<CellChangedEventArgs>(this.Sheet_CellChanged);
            }
        }

        public void Dispose()
        {
            this.DetachEvents();
            this.Groups.Clear();
        }

        Sparkline Find(int row, int column)
        {
            foreach (SparklineGroup group in this.Groups)
            {
                for (int i = 0; i < group.Count; i++)
                {
                    Sparkline sparkline = group[i];
                    if ((sparkline.Row == row) && (sparkline.Column == column))
                    {
                        return sparkline;
                    }
                }
            }
            return null;
        }

        static CellRange GetExpressionRange(CalcExpression exp)
        {
            if (exp is CalcRangeExpression)
            {
                CalcRangeExpression expression = (CalcRangeExpression) exp;
                if (expression.IsFullRow && expression.IsFullColumn)
                {
                    return new CellRange(-1, -1, -1, -1);
                }
                if (expression.IsFullRow)
                {
                    return new CellRange(-1, expression.StartColumn, -1, (expression.EndColumn - expression.StartColumn) + 1);
                }
                if (expression.IsFullColumn)
                {
                    return new CellRange(expression.StartRow, -1, (expression.EndRow - expression.StartRow) + 1, -1);
                }
                return new CellRange(expression.StartRow, expression.StartColumn, (expression.EndRow - expression.StartRow) + 1, (expression.EndColumn - expression.StartColumn) + 1);
            }
            if (!(exp is CalcExternalRangeExpression))
            {
                return null;
            }
            CalcExternalRangeExpression expression2 = (CalcExternalRangeExpression) exp;
            if (expression2.IsFullRow && expression2.IsFullColumn)
            {
                return new CellRange(-1, -1, -1, -1);
            }
            if (expression2.IsFullRow)
            {
                return new CellRange(-1, expression2.StartColumn, -1, (expression2.EndColumn - expression2.StartColumn) + 1);
            }
            if (expression2.IsFullColumn)
            {
                return new CellRange(expression2.StartRow, -1, (expression2.EndRow - expression2.StartRow) + 1, -1);
            }
            return new CellRange(expression2.StartRow, expression2.StartColumn, (expression2.EndRow - expression2.StartRow) + 1, (expression2.EndColumn - expression2.StartColumn) + 1);
        }

        internal void Init()
        {
            this.DetachEvents();
            this.groups = null;
            this.sheet = null;
            this.evaluator = null;
        }

        public void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            this.MoveDataRange(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            DataMatrix<Sparkline> matrix = new DataMatrix<Sparkline>(rowCount, columnCount);
            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    Sparkline sparkline = this.Sheet.GetSparkline(fromRow + i, fromColumn + k);
                    if (sparkline != null)
                    {
                        sparkline.Row = toRow + i;
                        sparkline.Column = toColumn + k;
                        matrix.SetValue(i, k, sparkline);
                    }
                    this.Sheet.SetSparkline(fromRow + i, fromColumn + k, null);
                }
            }
            for (int j = 0; j < rowCount; j++)
            {
                for (int m = 0; m < columnCount; m++)
                {
                    this.Sheet.SetSparkline(toRow + j, toColumn + m, matrix.GetValue(j, m));
                }
            }
        }

        public void Move(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (object.ReferenceEquals(src, this.Sheet))
            {
                this.Move(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
            else
            {
                this.MoveDataRange(src, fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        Sparkline item = src.GetSparkline(fromRow + i, fromColumn + j);
                        if (item != null)
                        {
                            item.Row = toRow + i;
                            item.Column = toColumn + j;
                            SparklineGroup group = item.Group;
                            SparklineGroup group2 = group.Clone();
                            group.Remove(item);
                            if (group.Count <= 0)
                            {
                                src.SheetSparklineGroupManager.Remove(group);
                            }
                            group2.Add(item);
                            this.Add(group2);
                            this.Sheet.SetSparkline(toRow + i, toColumn + j, item);
                        }
                        else
                        {
                            this.Sheet.SetSparkline(toRow + i, toColumn + j, null);
                        }
                        ((ISparklineSheet) src).SetSparkline(fromRow + i, fromColumn + j, null);
                    }
                }
            }
        }

        void MoveDataRange(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            CellRange range = new CellRange(fromRow, fromColumn, rowCount, columnCount);
            int row = toRow - fromRow;
            int column = toColumn - fromColumn;
            using (List<SparklineGroup>.Enumerator enumerator = this.Groups.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    foreach (Sparkline sparkline in enumerator.Current)
                    {
                        if (sparkline != null)
                        {
                            CalcExpression dateAxisReference = sparkline.DateAxisReference;
                            CellRange expressionRange = GetExpressionRange(dateAxisReference);
                            if (((expressionRange != null) && SameSource((ICalcSource) this.Sheet, dateAxisReference)) && (range.Contains(expressionRange) && range.Contains(sparkline.Row, sparkline.Column)))
                            {
                                sparkline.DateAxisReference = dateAxisReference.Offset(row, column, false, true);
                            }
                            CalcExpression dataReference = sparkline.DataReference;
                            CellRange range3 = GetExpressionRange(dataReference);
                            if (((range3 != null) && SameSource(this.Sheet as ICalcSource, dataReference)) && (range.Contains(range3) && range.Contains(sparkline.Row, sparkline.Column)))
                            {
                                sparkline.DataReference = dataReference.Offset(row, column, false, true);
                            }
                        }
                    }
                }
            }
        }

        void MoveDataRange(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            if (src == (this.Sheet as Worksheet))
            {
                this.MoveDataRange(fromRow, fromColumn, toRow, toColumn, rowCount, columnCount);
            }
            else
            {
                CellRange range = new CellRange(fromRow, fromColumn, rowCount, columnCount);
                int row = toRow - fromRow;
                int column = toColumn - fromColumn;
                using (List<SparklineGroup>.Enumerator enumerator = src.SheetSparklineGroupManager.Groups.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        foreach (Sparkline sparkline in enumerator.Current)
                        {
                            if (sparkline != null)
                            {
                                CalcExpression dateAxisReference = sparkline.DateAxisReference;
                                CellRange expressionRange = GetExpressionRange(dateAxisReference);
                                if (((expressionRange != null) && SameSource(src, dateAxisReference)) && (range.Contains(expressionRange) && range.Contains(sparkline.Row, sparkline.Column)))
                                {
                                    sparkline.DateAxisReference = dateAxisReference.Offset(row, column, false, true);
                                }
                                CalcExpression dataReference = sparkline.DataReference;
                                CellRange range3 = GetExpressionRange(dataReference);
                                if (((range3 != null) && SameSource(src, dataReference)) && (range.Contains(range3) && range.Contains(sparkline.Row, sparkline.Column)))
                                {
                                    sparkline.DataReference = dataReference.Offset(row, column, false, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        void NotifyCellValueChanged(int row, int column)
        {
            foreach (SparklineGroup group in this.Groups)
            {
                foreach (Sparkline sparkline in group)
                {
                    if (ContainsCell(sparkline.DataReference, row, column))
                    {
                        sparkline.OnSparklineChanged();
                    }
                }
                if (ContainsCell(group.DateAxisReference, row, column))
                {
                    group.OnGroupChanged();
                }
            }
        }

        public void Remove(SparklineGroup group)
        {
            this.Groups.Remove(group);
            group.SparklineGroupManager = null;
        }

        static CalcExpression RemoveColumnRange(int column, int columnCount, CalcExpression dataRange)
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

        public void RemoveColumns(int column, int count)
        {
            foreach (SparklineGroup group in this.Groups.ToArray())
            {
                if (group.DisplayDateAxis)
                {
                    CalcExpression expression = RemoveColumnRange(column, count, group.DateAxisReference);
                    if (expression != null)
                    {
                        if (group.DateAxisReference != expression)
                        {
                            group.DateAxisReference = expression;
                        }
                    }
                    else
                    {
                        group.Clear();
                        this.Remove(group);
                        continue;
                    }
                }
                Sparkline[] array = new Sparkline[group.Count];
                group.CopyTo(array, 0);
                foreach (Sparkline sparkline in array)
                {
                    if ((sparkline.Column >= column) && (sparkline.Column < (column + count)))
                    {
                        group.Remove(sparkline);
                    }
                    else
                    {
                        if (column <= sparkline.Column)
                        {
                            sparkline.Column -= count;
                        }
                        CalcExpression expression2 = RemoveColumnRange(column, count, sparkline.DataReference);
                        if (expression2 != null)
                        {
                            if (sparkline.DataReference != expression2)
                            {
                                sparkline.DataReference = expression2;
                            }
                        }
                        else
                        {
                            group.Remove(sparkline);
                        }
                    }
                }
                if (group.Count <= 0)
                {
                    this.Remove(group);
                }
            }
        }

        static CalcExpression RemoveRowRange(int row, int rowCount, CalcExpression dataRange)
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

        public void RemoveRows(int row, int count)
        {
            foreach (SparklineGroup group in this.Groups.ToArray())
            {
                if (group.DisplayDateAxis)
                {
                    CalcExpression expression = RemoveRowRange(row, count, group.DateAxisReference);
                    if (expression != null)
                    {
                        if (group.DateAxisReference != expression)
                        {
                            group.DateAxisReference = expression;
                        }
                    }
                    else
                    {
                        group.Clear();
                        this.Remove(group);
                        continue;
                    }
                }
                Sparkline[] array = new Sparkline[group.Count];
                group.CopyTo(array, 0);
                foreach (Sparkline sparkline in array)
                {
                    if ((sparkline.Row >= row) && (sparkline.Row < (row + count)))
                    {
                        group.Remove(sparkline);
                    }
                    else
                    {
                        if (row <= sparkline.Row)
                        {
                            sparkline.Row -= count;
                        }
                        CalcExpression expression2 = RemoveRowRange(row, count, sparkline.DataReference);
                        if (expression2 != null)
                        {
                            if (sparkline.DataReference != expression2)
                            {
                                sparkline.DataReference = expression2;
                            }
                        }
                        else
                        {
                            group.Remove(sparkline);
                        }
                    }
                }
                if (group.Count <= 0)
                {
                    this.Remove(group);
                }
            }
        }

        internal void ResumeAfterDeserialization()
        {
            if (this.evaluator != null)
            {
                using (List<SparklineGroup>.Enumerator enumerator = this.Groups.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.ResumeAfterDeserialization();
                    }
                }
            }
        }

        static bool SameSource(ICalcSource source, CalcExpression exp)
        {
            if (exp is CalcExternalExpression)
            {
                return (source == ((CalcExternalExpression) exp).Source);
            }
            return true;
        }

        void Sheet_CellChanged(object sender, CellChangedEventArgs e)
        {
            if (e.PropertyName == "Value")
            {
                this.NotifyCellValueChanged(e.Row, e.Column);
            }
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

        public void Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    Sparkline sparkline = this.Sheet.GetSparkline(fromRow + i, fromColumn + j);
                    sparkline.Row = toRow + i;
                    sparkline.Column = toColumn + j;
                    Sparkline sparkline2 = this.Sheet.GetSparkline(toRow + i, toColumn + j);
                    sparkline2.Row = fromRow + i;
                    sparkline2.Column = fromColumn + j;
                    this.Sheet.SetSparkline(fromRow + i, fromColumn + j, sparkline2);
                    this.Sheet.SetSparkline(toRow + i, toColumn + j, sparkline);
                }
            }
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Init();
            Serializer.InitReader(reader);
            while (reader.Read())
            {
                string str;
                if (((reader.NodeType == ((XmlNodeType) ((int) XmlNodeType.Element))) && ((str = reader.Name) != null)) && (str == "SparklineGroupList"))
                {
                    XmlReader reader2 = Serializer.ExtractNode(reader);
                    List<SparklineGroup> list = new List<SparklineGroup>();
                    Serializer.DeserializeList((IList) list, reader2);
                    reader2.Close();
                    foreach (SparklineGroup group in list)
                    {
                        this.Add(group);
                    }
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            Serializer.InitWriter(writer);
            if (this.groups != null)
            {
                writer.WriteStartElement("SparklineGroupList");
                Serializer.SerializeList((IList) this.groups, writer);
                writer.WriteEndElement();
            }
        }

        public ICalcEvaluator CalcEvaluator
        {
            get { return  this.evaluator; }
            internal set
            {
                if (value != this.evaluator)
                {
                    this.evaluator = value;
                }
            }
        }

        public int Count
        {
            get { return  this.Groups.Count; }
        }

        internal List<SparklineGroup> Groups
        {
            get
            {
                if (this.groups == null)
                {
                    this.groups = new List<SparklineGroup>();
                }
                return this.groups;
            }
            set
            {
                if (this.groups != value)
                {
                    this.groups = value;
                }
            }
        }

        public SparklineGroup this[int index]
        {
            get { return  this.Groups[index]; }
        }

        public ISparklineSheet Sheet
        {
            get { return  this.sheet; }
            internal set
            {
                if (this.sheet != value)
                {
                    this.DetachEvents();
                    this.sheet = value;
                    this.AtachEvents();
                }
            }
        }
    }
}

