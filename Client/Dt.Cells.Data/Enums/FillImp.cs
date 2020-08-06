#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents an implementation for Fill feature.
    /// </summary>
    internal class FillImp
    {
        FillCachePool _fillCache;
        Worksheet _worksheet;

        public FillImp(Worksheet worksheet)
        {
            this._worksheet = worksheet;
            this._fillCache = new FillCachePool(worksheet);
        }

        void AutoFillColumnTrendValues(CellRange sourceRange, CellRange targetRange, int sourceColumn, int targetColumn, NumberSource trendData)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (targetRange.Row - sourceRange.Row) / sourceRange.RowCount;
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int fromRow = trendData.Indexes[j];
                        int toRow = fromRow + (num * sourceRange.RowCount);
                        if (toRow < (targetRange.Row + targetRange.RowCount))
                        {
                            this.CopyCell(this._worksheet, fromRow, sourceColumn, toRow, targetColumn, trendData.ToActualValue(num5), FillType.Auto);
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        if (trendData.DataType == typeof(TimeSpan))
                        {
                            TimeSpan span2 = TimeSpan.FromDays(trendData.InnerValues[0]).Add(new TimeSpan(1, 0, 0));
                            trendData.Add(trendData.Indexes[0] + 1, span2);
                        }
                        else
                        {
                            trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                        }
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        if (trendData.DataType == typeof(TimeSpan))
                        {
                            TimeSpan newValue = TimeSpan.FromDays(trendData.InnerValues[0]).Add(new TimeSpan((num9 * num) + k, 0, 0));
                            int num11 = num8 + k;
                            int num12 = num11 + (num * sourceRange.RowCount);
                            if (num12 < (targetRange.Row + targetRange.RowCount))
                            {
                                this.CopyCell(this._worksheet, num8, sourceColumn, num12, targetColumn, newValue, FillType.Auto);
                            }
                        }
                        else
                        {
                            double num13 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                            int num14 = num8 + k;
                            int num15 = num14 + (num * sourceRange.RowCount);
                            if (num15 < (targetRange.Row + targetRange.RowCount))
                            {
                                this.CopyCell(this._worksheet, num8, sourceColumn, num15, targetColumn, trendData.ToActualValue(num13), FillType.Auto);
                            }
                        }
                    }
                }
            }
        }

        void AutoFillRange(CellRange sourceRange, int rowCount, int columnCount, FillSeries fillSeries)
        {
            int num;
            int num2;
            this._fillCache.Clear();
            switch (fillSeries)
            {
                case FillSeries.Column:
                {
                    int num4 = rowCount / sourceRange.RowCount;
                    int num5 = rowCount % sourceRange.RowCount;
                    if (num4 > 1)
                    {
                        for (int i = 1; i < num4; i++)
                        {
                            CellRange targetRange = new CellRange(sourceRange.Row + (i * sourceRange.RowCount), sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, targetRange, fillSeries, FillType.Auto);
                        }
                    }
                    if ((num4 > 0) && (num5 > 0))
                    {
                        CellRange range4 = new CellRange(sourceRange.Row + (num4 * sourceRange.RowCount), sourceRange.Column, num5, sourceRange.ColumnCount);
                        this.CopyRange(sourceRange, range4, fillSeries, FillType.Auto);
                    }
                    goto Label_0153;
                }
                case FillSeries.Row:
                    num = columnCount / sourceRange.ColumnCount;
                    num2 = columnCount % sourceRange.ColumnCount;
                    if (num > 1)
                    {
                        for (int j = 1; j < num; j++)
                        {
                            CellRange range = new CellRange(sourceRange.Row, sourceRange.Column + (j * sourceRange.ColumnCount), sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, range, fillSeries, FillType.Auto);
                        }
                    }
                    break;

                default:
                    goto Label_0153;
            }
            if ((num > 0) && (num2 > 0))
            {
                CellRange range2 = new CellRange(sourceRange.Row, sourceRange.Column + (num * sourceRange.ColumnCount), sourceRange.RowCount, num2);
                this.CopyRange(sourceRange, range2, fillSeries, FillType.Auto);
            }
        Label_0153:
            this._fillCache.Clear();
        }

        void AutoFillRowTrendValues(CellRange sourceRange, CellRange targetRange, int sourceRow, int targetRow, NumberSource trendData)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (targetRange.Column - sourceRange.Column) / sourceRange.ColumnCount;
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int fromColumn = trendData.Indexes[j];
                        int toColumn = fromColumn + (num * sourceRange.ColumnCount);
                        if (toColumn < (targetRange.Column + targetRange.ColumnCount))
                        {
                            this.CopyCell(this._worksheet, sourceRow, fromColumn, targetRow, toColumn, trendData.ToActualValue(num5), FillType.Auto);
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        if (trendData.DataType == typeof(TimeSpan))
                        {
                            TimeSpan span2 = TimeSpan.FromDays(trendData.InnerValues[0]).Add(new TimeSpan(1, 0, 0));
                            trendData.Add(trendData.Indexes[0] + 1, span2);
                        }
                        else
                        {
                            trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                        }
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        if (trendData.DataType == typeof(TimeSpan))
                        {
                            TimeSpan newValue = TimeSpan.FromDays(trendData.InnerValues[0]).Add(new TimeSpan((num9 * num) + k, 0, 0));
                            int num11 = num8 + k;
                            int num12 = num11 + (num * sourceRange.ColumnCount);
                            if (targetRow < (targetRange.Row + targetRange.RowCount))
                            {
                                this.CopyCell(this._worksheet, sourceRow, num11, targetRow, num12, newValue, FillType.Auto);
                            }
                        }
                        else
                        {
                            double num13 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                            int num14 = num8 + k;
                            int num15 = num14 + (num * sourceRange.ColumnCount);
                            if (num15 < (targetRange.Column + targetRange.ColumnCount))
                            {
                                this.CopyCell(this._worksheet, sourceRow, num14, targetRow, num15, trendData.ToActualValue(num13), FillType.Auto);
                            }
                        }
                    }
                }
            }
        }

        List<double> CalcSeriesData(NumberSource sourceData, int count, FillType type, double stepValue, double? stopValue, FillDateUnit? dateUnit)
        {
            if ((sourceData == null) || (sourceData.DataCount <= 0))
            {
                return null;
            }
            List<double> list = new List<double>();
            double initValue = sourceData.InnerValues[0];
            double currentValue = initValue;
            for (int i = 0; i < count; i++)
            {
                if (stopValue.HasValue)
                {
                    double num4 = currentValue;
                    double? nullable = stopValue;
                    if (!((num4 <= ((double) nullable.GetValueOrDefault())) && nullable.HasValue))
                    {
                        continue;
                    }
                }
                list.Add(currentValue);
                if (type == FillType.Linear)
                {
                    currentValue += stepValue;
                }
                else if (type == FillType.Growth)
                {
                    currentValue *= stepValue;
                }
                else if ((type == FillType.Date) && dateUnit.HasValue)
                {
                    currentValue = this.GetNextDateValue(dateUnit.Value, initValue, currentValue, stepValue, i + 1);
                }
            }
            return list;
        }

        List<double> CalcSeriesTrendData(NumberSource sourceData, int count, FillType type)
        {
            if (((sourceData == null) || (sourceData.DataCount <= 0)) || ((type != FillType.Linear) && (type != FillType.Growth)))
            {
                return null;
            }
            List<double> list = new List<double>();
            if (sourceData.DataCount == 1)
            {
                if (type == FillType.Linear)
                {
                    if (sourceData.DataType == typeof(TimeSpan))
                    {
                        TimeSpan span2 = TimeSpan.FromDays(sourceData.InnerValues[0]).Add(new TimeSpan(1, 0, 0));
                        sourceData.Add(sourceData.Indexes[0] + 1, (double) span2.TotalDays);
                    }
                    else
                    {
                        sourceData.Add(sourceData.Indexes[0] + 1, sourceData.ToActualValue(sourceData.InnerValues[0] + 1.0));
                    }
                }
                else if (type == FillType.Growth)
                {
                    sourceData.Add(sourceData.Indexes[0] + 1, sourceData.Values[0]);
                }
            }
            for (int i = 0; i < count; i++)
            {
                if (type == FillType.Linear)
                {
                    list.Add(FormulaEvaluator.TREND(this._worksheet, sourceData.Values2, sourceData.Indexes2, i + 1));
                }
                else if (type == FillType.Growth)
                {
                    list.Add(FormulaEvaluator.GROWTH(this._worksheet, sourceData.Values2, sourceData.Indexes2, i + 1));
                }
            }
            if (sourceData.DataType == typeof(TimeSpan))
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j] < 0.0)
                    {
                        while (list[j] < 0.0)
                        {
                            List<double> list2;
                            int num3;
                            (list2 = list)[num3 = j] = list2[num3] + 1.0;
                        }
                    }
                }
            }
            return list;
        }

        void CopyCell(Worksheet sheet, int fromRow, int fromColumn, int toRow, int toColumn, object newValue, FillType type)
        {
            if (sheet.HasFormula)
            {
                sheet.SetFormula(toRow, toColumn, null);
                if ((type == FillType.Direction) || (type == FillType.Auto))
                {
                    IRangeSupport calcAxial = sheet.CalcAxial;
                    if (calcAxial != null)
                    {
                        calcAxial.Copy(fromRow, fromColumn, toRow, toColumn, 1, 1);
                    }
                }
            }
            sheet.SetValue(toRow, toColumn, newValue);
            StyleInfo cachedStyle = this._fillCache.GetCachedStyle(fromRow, fromColumn);
            if (cachedStyle == null)
            {
                sheet.SetStyleObject(toRow, toColumn, SheetArea.Cells, null);
            }
            else
            {
                StyleInfo style = new StyleInfo();
                style.CopyFromInternal(cachedStyle, true, true);
                sheet.SetStyleObject(toRow, toColumn, SheetArea.Cells, style);
            }
            if (!sheet.SpanModel.IsEmpty())
            {
                sheet.RemoveSpanCell(toRow, toColumn);
                CellRange spanCell = sheet.GetSpanCell(fromRow, fromColumn);
                if (spanCell != null)
                {
                    sheet.Cells[toRow, toColumn].RowSpan = spanCell.RowCount;
                    sheet.Cells[toRow, toColumn].ColumnSpan = spanCell.ColumnCount;
                }
            }
        }

        void CopyRange(CellRange sourceRange, CellRange targetRange, FillSeries fillSeries, FillType fillType)
        {
            bool hasFormula;
            int num7;
            switch (fillSeries)
            {
                case FillSeries.Column:
                    hasFormula = this._worksheet.HasFormula;
                    num7 = 0;
                    break;

                case FillSeries.Row:
                {
                    bool flag = this._worksheet.HasFormula;
                    for (int i = 0; i < sourceRange.RowCount; i++)
                    {
                        NumberSource trendData = null;
                        Type type = null;
                        int row = sourceRange.Row + i;
                        int targetRow = targetRange.Row + i;
                        int num4 = 0;
                        while (num4 < sourceRange.ColumnCount)
                        {
                            int column = sourceRange.Column + num4;
                            int toColumn = targetRange.Column + num4;
                            object obj2 = null;
                            if (!flag || string.IsNullOrEmpty(this._worksheet.GetFormula(row, column)))
                            {
                                obj2 = this._worksheet.GetValue(row, column);
                            }
                            if ((fillType == FillType.Auto) && FormatConverter.IsNumber(obj2))
                            {
                                if (trendData == null)
                                {
                                    trendData = new NumberSource();
                                }
                                Type type2 = null;
                                if (obj2 is DateTime)
                                {
                                    type2 = typeof(DateTime);
                                }
                                else if (obj2 is TimeSpan)
                                {
                                    type2 = typeof(TimeSpan);
                                }
                                else
                                {
                                    type2 = typeof(double);
                                }
                                if (type == null)
                                {
                                    type = type2;
                                }
                                if (type == type2)
                                {
                                    trendData.Add(column, obj2);
                                    num4++;
                                    continue;
                                }
                            }
                            if (((obj2 != null) && (trendData != null)) && (trendData.DataCount > 0))
                            {
                                this.AutoFillRowTrendValues(sourceRange, targetRange, row, targetRow, trendData);
                                trendData = null;
                                type = null;
                            }
                            else
                            {
                                CellRange spanCell = this._worksheet.GetSpanCell(row, column);
                                if (spanCell != null)
                                {
                                    if ((spanCell.Row == row) && (toColumn < (targetRange.Column + targetRange.ColumnCount)))
                                    {
                                        this.CopyCell(this._worksheet, row, column, targetRow, toColumn, obj2, fillType);
                                    }
                                    num4 += spanCell.ColumnCount;
                                    continue;
                                }
                                if (toColumn < (targetRange.Column + targetRange.ColumnCount))
                                {
                                    this.CopyCell(this._worksheet, row, column, targetRow, toColumn, obj2, fillType);
                                }
                                num4++;
                            }
                        }
                        if ((trendData != null) && (trendData.DataCount > 0))
                        {
                            this.AutoFillRowTrendValues(sourceRange, targetRange, row, targetRow, trendData);
                        }
                    }
                    return;
                }
                default:
                    return;
            }
            while (num7 < sourceRange.ColumnCount)
            {
                NumberSource source2 = null;
                Type type3 = null;
                int num8 = sourceRange.Column + num7;
                int targetColumn = targetRange.Column + num7;
                int num10 = 0;
                while (num10 < sourceRange.RowCount)
                {
                    int num11 = sourceRange.Row + num10;
                    int toRow = targetRange.Row + num10;
                    object obj3 = null;
                    if (!hasFormula || string.IsNullOrEmpty(this._worksheet.GetFormula(num11, num8)))
                    {
                        obj3 = this._worksheet.GetValue(num11, num8);
                    }
                    if ((fillType == FillType.Auto) && FormatConverter.IsNumber(obj3))
                    {
                        if (source2 == null)
                        {
                            source2 = new NumberSource();
                        }
                        Type type4 = null;
                        if (obj3 is DateTime)
                        {
                            type4 = typeof(DateTime);
                        }
                        else if (obj3 is TimeSpan)
                        {
                            type4 = typeof(TimeSpan);
                        }
                        else
                        {
                            type4 = typeof(double);
                        }
                        if (type3 == null)
                        {
                            type3 = type4;
                        }
                        if (type3 == type4)
                        {
                            source2.Add(num11, obj3);
                            num10++;
                            continue;
                        }
                    }
                    if (((obj3 != null) && (source2 != null)) && (source2.DataCount > 0))
                    {
                        this.AutoFillColumnTrendValues(sourceRange, targetRange, num8, targetColumn, source2);
                        source2 = null;
                        type3 = null;
                    }
                    else
                    {
                        CellRange range2 = this._worksheet.GetSpanCell(num11, num8);
                        if (range2 != null)
                        {
                            if ((range2.Column == num8) && (toRow < (targetRange.Row + targetRange.RowCount)))
                            {
                                this.CopyCell(this._worksheet, num11, num8, toRow, targetColumn, obj3, fillType);
                            }
                            num10 += range2.RowCount;
                            continue;
                        }
                        if (toRow < (targetRange.Row + targetRange.RowCount))
                        {
                            this.CopyCell(this._worksheet, num11, num8, toRow, targetColumn, obj3, fillType);
                        }
                        num10++;
                    }
                }
                if ((source2 != null) && (source2.DataCount > 0))
                {
                    this.AutoFillColumnTrendValues(sourceRange, targetRange, num8, targetColumn, source2);
                }
                num7++;
            }
        }

        void DirectionFillRange(CellRange sourceRange, int row, int column, int rowCount, int columnCount, FillDirection direction)
        {
            this._fillCache.Clear();
            switch (direction)
            {
                case FillDirection.Left:
                {
                    if (this.HasSpans(row, column, rowCount, columnCount - sourceRange.ColumnCount))
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHasMergedCell);
                    }
                    int num = columnCount / sourceRange.ColumnCount;
                    if ((columnCount % sourceRange.ColumnCount) != 0)
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHaveDifferentSize);
                    }
                    if (num > 1)
                    {
                        for (int i = 1; i < num; i++)
                        {
                            CellRange targetRange = new CellRange(sourceRange.Row, sourceRange.Column - (i * sourceRange.ColumnCount), sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, targetRange, FillSeries.Row, FillType.Direction);
                        }
                    }
                    break;
                }
                case FillDirection.Right:
                {
                    if (this.HasSpans(row, column + sourceRange.ColumnCount, rowCount, columnCount - sourceRange.ColumnCount))
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHasMergedCell);
                    }
                    int num4 = columnCount / sourceRange.ColumnCount;
                    if ((columnCount % sourceRange.ColumnCount) != 0)
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHaveDifferentSize);
                    }
                    if (num4 > 1)
                    {
                        for (int j = 1; j < num4; j++)
                        {
                            CellRange range2 = new CellRange(sourceRange.Row, sourceRange.Column + (j * sourceRange.ColumnCount), sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, range2, FillSeries.Row, FillType.Direction);
                        }
                    }
                    break;
                }
                case FillDirection.Up:
                {
                    if (this.HasSpans(row, column, rowCount - sourceRange.RowCount, columnCount))
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHasMergedCell);
                    }
                    int num7 = rowCount / sourceRange.RowCount;
                    if ((rowCount % sourceRange.RowCount) != 0)
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHaveDifferentSize);
                    }
                    if (num7 > 1)
                    {
                        for (int k = 1; k < num7; k++)
                        {
                            CellRange range3 = new CellRange(sourceRange.Row - (k * sourceRange.RowCount), sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, range3, FillSeries.Column, FillType.Direction);
                        }
                    }
                    break;
                }
                case FillDirection.Down:
                {
                    if (this.HasSpans(row + sourceRange.RowCount, column, rowCount - sourceRange.RowCount, columnCount))
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHasMergedCell);
                    }
                    int num10 = rowCount / sourceRange.RowCount;
                    if ((rowCount % sourceRange.RowCount) != 0)
                    {
                        throw new ArgumentException(ResourceStrings.FillRangeHaveDifferentSize);
                    }
                    if (num10 > 1)
                    {
                        for (int m = 1; m < num10; m++)
                        {
                            CellRange range4 = new CellRange(sourceRange.Row + (m * sourceRange.RowCount), sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount);
                            this.CopyRange(sourceRange, range4, FillSeries.Column, FillType.Direction);
                        }
                    }
                    break;
                }
            }
            this._fillCache.Clear();
        }

        public void FillAuto(CellRange range, FillDirection direction)
        {
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            CellRange sourceRange = this.GetDirectionFillSourceRange(row, column, rowCount, columnCount, direction);
            if (sourceRange != null)
            {
                this.DirectionFillRange(sourceRange, row, column, rowCount, columnCount, direction);
            }
        }

        public void FillAuto(CellRange range, FillSeries series)
        {
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            CellRange sourceRange = this.GetAutoFillSourceRange(row, column, rowCount, columnCount, series);
            if (sourceRange != null)
            {
                if (this.HasPartSpans(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount) || this.HasPartSpans(row, column, rowCount, columnCount))
                {
                    throw new ArgumentException(ResourceStrings.AutoFillChangedPartOfMergedCell);
                }
                this.AutoFillRange(sourceRange, rowCount, columnCount, series);
            }
        }

        public void FillDate(CellRange range, FillSeries series, FillDateUnit unit, double step)
        {
            this.FillDate(range, series, unit, step, null);
        }

        public void FillDate(CellRange range, FillSeries series, FillDateUnit unit, double step, double? stop)
        {
            this.SeriesFillRange(range, series, FillType.Date, step, stop, new FillDateUnit?(unit));
        }

        public void FillGrowth(CellRange range, FillSeries series)
        {
            this.SeriesTrendFillRange(range, series, FillType.Growth);
        }

        public void FillGrowth(CellRange range, FillSeries series, double step)
        {
            this.FillGrowth(range, series, step, null);
        }

        public void FillGrowth(CellRange range, FillSeries series, double step, double? stop)
        {
            this.SeriesFillRange(range, series, FillType.Growth, step, stop, null);
        }

        public void FillLinear(CellRange range, FillSeries series)
        {
            this.SeriesTrendFillRange(range, series, FillType.Linear);
        }

        public void FillLinear(CellRange range, FillSeries series, double step)
        {
            this.FillLinear(range, series, step, null);
        }

        public void FillLinear(CellRange range, FillSeries series, double step, double? stop)
        {
            this.SeriesFillRange(range, series, FillType.Linear, step, stop, null);
        }

        CellRange FixRange(CellRange range)
        {
            int row = range.Row;
            int column = range.Column;
            int rowCount = range.RowCount;
            int columnCount = range.ColumnCount;
            if (row == -1)
            {
                row = 0;
                rowCount = this._worksheet.RowCount;
            }
            if (column == -1)
            {
                column = 0;
                columnCount = this._worksheet.ColumnCount;
            }
            return new CellRange(row, column, rowCount, columnCount);
        }

        string GetAutoFillColumnTreadValue(CellRange sourceRange, CellRange targetRange, int souceCoulumn, int targetColumn, NumberSource trendData)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (int) Math.Floor((double) (((targetRange.Row - sourceRange.Row) * 1.0) / ((double) sourceRange.RowCount)));
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int num7 = trendData.Indexes[j] + (num * sourceRange.RowCount);
                        if (num7 == targetRange.Row)
                        {
                            return trendData.ToActualValue(num5).ToString();
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        double num11 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                        int num12 = num8 + k;
                        int num13 = num12 + (num * sourceRange.RowCount);
                        if (num13 == targetRange.Row)
                        {
                            return trendData.ToActualValue(num11).ToString();
                        }
                    }
                }
            }
            return null;
        }

        void GetAutoFillColumnTrendValues(CellRange sourceRange, CellRange targetRange, int sourceColumn, int targetColumn, NumberSource trendData, Dictionary<long, object> result)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (targetRange.Row - sourceRange.Row) / sourceRange.RowCount;
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int row = trendData.Indexes[j];
                        int num7 = row + (num * sourceRange.RowCount);
                        if (num7 < (targetRange.Row + targetRange.RowCount))
                        {
                            StyleInfo styleInfo = this._worksheet.GetStyleInfo(row, sourceColumn);
                            if ((styleInfo != null) && (styleInfo.Formatter != null))
                            {
                                result[(long) ((num7 << 4) | targetColumn)] = styleInfo.Formatter.Format(trendData.ToActualValue(num5));
                            }
                            else
                            {
                                result[(long) ((num7 << 4) | targetColumn)] = trendData.ToActualValue(num5);
                            }
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        double num11 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                        int num12 = num8 + k;
                        int num13 = num12 + (num * sourceRange.RowCount);
                        if (num13 < (targetRange.Row + targetRange.RowCount))
                        {
                            StyleInfo info2 = this._worksheet.GetStyleInfo(num12, sourceColumn);
                            if ((info2 != null) && (info2.Formatter != null))
                            {
                                result[(long) ((num13 << 4) | targetColumn)] = info2.Formatter.Format(trendData.ToActualValue(num11));
                            }
                            else
                            {
                                result[(long) ((num13 << 4) | targetColumn)] = trendData.ToActualValue(num11);
                            }
                        }
                    }
                }
            }
        }

        object GetAutoFillRange(CellRange sourceRange, int rowCount, int columnCount, FillSeries fillSeries)
        {
            int num;
            int num2;
            Dictionary<long, object> result = new Dictionary<long, object>();
            this._fillCache.Clear();
            switch (fillSeries)
            {
                case FillSeries.Column:
                {
                    int num4 = rowCount / sourceRange.RowCount;
                    int num5 = rowCount % sourceRange.RowCount;
                    if (num4 > 1)
                    {
                        for (int i = 1; i < num4; i++)
                        {
                            CellRange targetRange = new CellRange(sourceRange.Row + (i * sourceRange.RowCount), sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount);
                            this.GetCopyRangeValue(sourceRange, targetRange, fillSeries, FillType.Auto, result);
                        }
                    }
                    if ((num4 > 0) && (num5 > 0))
                    {
                        CellRange range4 = new CellRange(sourceRange.Row + (num4 * sourceRange.RowCount), sourceRange.Column, num5, sourceRange.ColumnCount);
                        this.GetCopyRangeValue(sourceRange, range4, fillSeries, FillType.Auto, result);
                    }
                    goto Label_015F;
                }
                case FillSeries.Row:
                    num = columnCount / sourceRange.ColumnCount;
                    num2 = columnCount % sourceRange.ColumnCount;
                    if (num > 1)
                    {
                        for (int j = 1; j < num; j++)
                        {
                            CellRange range = new CellRange(sourceRange.Row, sourceRange.Column + (j * sourceRange.ColumnCount), sourceRange.RowCount, sourceRange.ColumnCount);
                            this.GetCopyRangeValue(sourceRange, range, fillSeries, FillType.Auto, result);
                        }
                    }
                    break;

                default:
                    goto Label_015F;
            }
            if ((num > 0) && (num2 > 0))
            {
                CellRange range2 = new CellRange(sourceRange.Row, sourceRange.Column + (num * sourceRange.ColumnCount), sourceRange.RowCount, num2);
                this.GetCopyRangeValue(sourceRange, range2, fillSeries, FillType.Auto, result);
            }
        Label_015F:
            this._fillCache.Clear();
            return result;
        }

        string GetAutoFillRowTreadValue(CellRange sourceRange, CellRange targetRange, int souceRow, int targetRow, NumberSource trendData)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (int) Math.Floor((double) (((targetRange.Column - sourceRange.Column) * 1.0) / ((double) sourceRange.ColumnCount)));
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int num7 = trendData.Indexes[j] + (num * sourceRange.ColumnCount);
                        if (num7 == targetRange.Column)
                        {
                            return trendData.ToActualValue(num5).ToString();
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        double num11 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                        int num12 = num8 + k;
                        int num13 = num12 + (num * sourceRange.ColumnCount);
                        if (num13 == targetRange.Column)
                        {
                            return trendData.ToActualValue(num11).ToString();
                        }
                    }
                }
            }
            return null;
        }

        void GetAutoFillRowTrendValues(CellRange sourceRange, CellRange targetRange, int sourceRow, int targetRow, NumberSource trendData, Dictionary<long, object> result)
        {
            if ((trendData != null) && (trendData.DataCount > 0))
            {
                int num = (targetRange.Column - sourceRange.Column) / sourceRange.ColumnCount;
                if (this.IsArithmeticProgression(trendData.Indexes, trendData.InnerValues))
                {
                    object[,] values = trendData.Values2;
                    object[,] indexes = trendData.Indexes2;
                    int dataCount = trendData.DataCount;
                    indexes = new object[1, dataCount];
                    for (int i = 0; i < dataCount; i++)
                    {
                        indexes[0, i] = i + 1;
                    }
                    for (int j = 0; j < dataCount; j++)
                    {
                        double num5 = FormulaEvaluator.TREND(this._worksheet, values, indexes, ((dataCount * num) + j) + 1);
                        int column = trendData.Indexes[j];
                        int num7 = column + (num * sourceRange.ColumnCount);
                        if (num7 < (targetRange.Column + targetRange.ColumnCount))
                        {
                            StyleInfo styleInfo = this._worksheet.GetStyleInfo(sourceRow, column);
                            if ((styleInfo != null) && (styleInfo.Formatter != null))
                            {
                                result[(long) ((targetRow << 4) | num7)] = styleInfo.Formatter.Format(trendData.ToActualValue(num5));
                            }
                            else
                            {
                                result[(long) ((targetRow << 4) | num7)] = trendData.ToActualValue(num5);
                            }
                        }
                    }
                }
                else
                {
                    int num8 = trendData.Indexes[0];
                    int num9 = (trendData.Indexes[trendData.DataCount - 1] - num8) + 1;
                    if (trendData.DataCount == 1)
                    {
                        trendData.Add(trendData.Indexes[0] + 1, trendData.ToActualValue(trendData.InnerValues[0] + 1.0));
                    }
                    for (int k = 0; k < num9; k++)
                    {
                        double num11 = FormulaEvaluator.TREND(this._worksheet, trendData.Values2, trendData.Indexes2, ((num9 * num) + k) + 1);
                        int num12 = num8 + k;
                        int num13 = num12 + (num * sourceRange.ColumnCount);
                        if (num13 < (targetRange.Column + targetRange.ColumnCount))
                        {
                            StyleInfo info2 = this._worksheet.GetStyleInfo(sourceRow, num12);
                            if ((info2 != null) && (info2.Formatter != null))
                            {
                                result[(long) ((targetRow << 4) | num13)] = info2.Formatter.Format(trendData.ToActualValue(num11));
                            }
                            else
                            {
                                result[(long) ((targetRow << 4) | num13)] = trendData.ToActualValue(num11);
                            }
                        }
                    }
                }
            }
        }

        CellRange GetAutoFillSourceRange(int row, int column, int rowCount, int columnCount, FillSeries fillSeries)
        {
            switch (fillSeries)
            {
                case FillSeries.Column:
                {
                    object obj3 = null;
                    int num4 = -1;
                    bool flag2 = this._worksheet.SpanModel.IsEmpty();
                    for (int i = column; i < (column + columnCount); i++)
                    {
                        for (int j = (row + rowCount) - 1; j >= row; j--)
                        {
                            if (flag2)
                            {
                                obj3 = this._worksheet.GetValue(j, i);
                            }
                            else
                            {
                                CellRange spanCell = this._worksheet.GetSpanCell(j, i);
                                if (spanCell == null)
                                {
                                    obj3 = this._worksheet.GetValue(j, i);
                                }
                                else
                                {
                                    if ((spanCell.Row == j) && (spanCell.Column == i))
                                    {
                                        obj3 = this._worksheet.GetValue(spanCell.Row, spanCell.Column);
                                    }
                                    j = spanCell.Row;
                                }
                            }
                            if (obj3 != null)
                            {
                                num4 = Math.Max(num4, j);
                            }
                        }
                    }
                    if (num4 >= 0)
                    {
                        return new CellRange(row, column, (num4 - row) + 1, columnCount);
                    }
                    break;
                }
                case FillSeries.Row:
                {
                    object obj2 = null;
                    int num = -1;
                    bool flag = this._worksheet.SpanModel.IsEmpty();
                    for (int k = row; k < (row + rowCount); k++)
                    {
                        for (int m = (column + columnCount) - 1; m >= column; m--)
                        {
                            if (flag)
                            {
                                obj2 = this._worksheet.GetValue(k, m);
                            }
                            else
                            {
                                CellRange range = this._worksheet.GetSpanCell(k, m);
                                if (range == null)
                                {
                                    obj2 = this._worksheet.GetValue(k, m);
                                }
                                else
                                {
                                    if ((range.Row == row) && (range.Column == column))
                                    {
                                        obj2 = this._worksheet.GetValue(range.Row, range.Column);
                                    }
                                    m = range.Column;
                                }
                            }
                            if (obj2 != null)
                            {
                                num = Math.Max(num, m);
                            }
                        }
                    }
                    if (num < 0)
                    {
                        break;
                    }
                    return new CellRange(row, column, rowCount, (num - column) + 1);
                }
            }
            return null;
        }

        void GetCopyRangeValue(CellRange sourceRange, CellRange targetRange, FillSeries fillSeries, FillType fillType, Dictionary<long, object> result)
        {
            bool hasFormula;
            int num7;
            switch (fillSeries)
            {
                case FillSeries.Column:
                    hasFormula = this._worksheet.HasFormula;
                    num7 = 0;
                    break;

                case FillSeries.Row:
                {
                    bool flag = this._worksheet.HasFormula;
                    for (int i = 0; i < sourceRange.RowCount; i++)
                    {
                        NumberSource trendData = null;
                        Type type = null;
                        int row = sourceRange.Row + i;
                        int targetRow = targetRange.Row + i;
                        int num4 = 0;
                        while (num4 < sourceRange.ColumnCount)
                        {
                            int column = sourceRange.Column + num4;
                            int num6 = targetRange.Column + num4;
                            object obj2 = null;
                            if (!flag || string.IsNullOrEmpty(this._worksheet.GetFormula(row, column)))
                            {
                                obj2 = this._worksheet.GetValue(row, column);
                            }
                            if ((fillType == FillType.Auto) && FormatConverter.IsNumber(obj2))
                            {
                                if (trendData == null)
                                {
                                    trendData = new NumberSource();
                                }
                                Type type2 = null;
                                if (obj2 is DateTime)
                                {
                                    type2 = typeof(DateTime);
                                }
                                else if (obj2 is TimeSpan)
                                {
                                    type2 = typeof(TimeSpan);
                                }
                                else
                                {
                                    type2 = typeof(double);
                                }
                                if (type == null)
                                {
                                    type = type2;
                                }
                                if (type == type2)
                                {
                                    trendData.Add(column, obj2);
                                    num4++;
                                    continue;
                                }
                            }
                            if (((obj2 != null) && (trendData != null)) && (trendData.DataCount > 0))
                            {
                                this.GetAutoFillRowTrendValues(sourceRange, targetRange, row, targetRow, trendData, result);
                                trendData = null;
                                type = null;
                            }
                            else
                            {
                                CellRange spanCell = this._worksheet.GetSpanCell(row, column);
                                if (spanCell != null)
                                {
                                    if ((spanCell.Row == row) && (num6 < (targetRange.Column + targetRange.ColumnCount)))
                                    {
                                        result[(long) ((targetRow << 4) | num6)] = obj2;
                                    }
                                    num4 += spanCell.ColumnCount;
                                    continue;
                                }
                                if (num6 < (targetRange.Column + targetRange.ColumnCount))
                                {
                                    result[(long) ((targetRow << 4) | num6)] = obj2;
                                }
                                num4++;
                            }
                        }
                        if ((trendData != null) && (trendData.DataCount > 0))
                        {
                            this.GetAutoFillRowTrendValues(sourceRange, targetRange, row, targetRow, trendData, result);
                        }
                    }
                    return;
                }
                default:
                    return;
            }
            while (num7 < sourceRange.ColumnCount)
            {
                NumberSource source2 = null;
                Type type3 = null;
                int num8 = sourceRange.Column + num7;
                int targetColumn = targetRange.Column + num7;
                int num10 = 0;
                while (num10 < sourceRange.RowCount)
                {
                    int num11 = sourceRange.Row + num10;
                    int num12 = targetRange.Row + num10;
                    object obj3 = null;
                    if (!hasFormula || string.IsNullOrEmpty(this._worksheet.GetFormula(num11, num8)))
                    {
                        obj3 = this._worksheet.GetValue(num11, num8);
                    }
                    if ((fillType == FillType.Auto) && FormatConverter.IsNumber(obj3))
                    {
                        if (source2 == null)
                        {
                            source2 = new NumberSource();
                        }
                        Type type4 = null;
                        if (obj3 is DateTime)
                        {
                            type4 = typeof(DateTime);
                        }
                        else if (obj3 is TimeSpan)
                        {
                            type4 = typeof(TimeSpan);
                        }
                        else
                        {
                            type4 = typeof(double);
                        }
                        if (type3 == null)
                        {
                            type3 = type4;
                        }
                        if (type3 == type4)
                        {
                            source2.Add(num11, obj3);
                            num10++;
                            continue;
                        }
                    }
                    if (((obj3 != null) && (source2 != null)) && (source2.DataCount > 0))
                    {
                        Debugger.Break();
                        this.GetAutoFillColumnTrendValues(sourceRange, targetRange, num8, targetColumn, source2, result);
                        source2 = null;
                        type3 = null;
                    }
                    else
                    {
                        CellRange range2 = this._worksheet.GetSpanCell(num11, num8);
                        if (range2 != null)
                        {
                            if ((range2.Column == num8) && (num12 < (targetRange.Row + targetRange.RowCount)))
                            {
                                result[(long) ((num12 << 4) | targetColumn)] = obj3;
                            }
                            num10 += range2.RowCount;
                            continue;
                        }
                        if (num12 < (targetRange.Row + targetRange.RowCount))
                        {
                            result[(long) ((num12 << 4) | targetColumn)] = obj3;
                        }
                        num10++;
                    }
                }
                if ((source2 != null) && (source2.DataCount > 0))
                {
                    this.GetAutoFillColumnTrendValues(sourceRange, targetRange, num8, targetColumn, source2, result);
                }
                num7++;
            }
        }

        CellRange GetDirectionFillSourceRange(int row, int column, int rowCount, int columnCount, FillDirection direction)
        {
            CellRange range = null;
            switch (direction)
            {
                case FillDirection.Left:
                    range = new CellRange(row, (column + columnCount) - 1, rowCount, 1);
                    break;

                case FillDirection.Right:
                    range = new CellRange(row, column, rowCount, 1);
                    break;

                case FillDirection.Up:
                    range = new CellRange((row + rowCount) - 1, column, 1, columnCount);
                    break;

                case FillDirection.Down:
                    range = new CellRange(row, column, 1, columnCount);
                    break;
            }
            return this.InflateCellRange(range);
        }

        internal object GetFillAutoValue(CellRange range, FillSeries series)
        {
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            CellRange sourceRange = this.GetAutoFillSourceRange(row, column, rowCount, columnCount, series);
            if ((sourceRange != null) && (!this.HasPartSpans(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, sourceRange.ColumnCount) && !this.HasPartSpans(row, column, rowCount, columnCount)))
            {
                return this.GetAutoFillRange(sourceRange, rowCount, columnCount, series);
            }
            return null;
        }

        internal object GetFillLinearValue(CellRange range, FillSeries series)
        {
            return this.GetSeriesTrendFillRangeValue(range, series, FillType.Linear);
        }

        public string GetFillText(int row, int column, AutoFillType fillType, FillDirection fillDirection)
        {
            CellRange sourceRange = null;
            if (this._worksheet.Selection.Count > 0)
            {
                sourceRange = this._worksheet.Selection[0];
            }
            else if (this._worksheet.ActiveCell != null)
            {
                sourceRange = new CellRange(this._worksheet.ActiveRowIndex, this._worksheet.ActiveColumnIndex, 1, 1);
            }
            else
            {
                return null;
            }
            if ((fillDirection == FillDirection.Down) || (fillDirection == FillDirection.Up))
            {
                NumberSource source = this.GetSeriesSource(sourceRange.Row, sourceRange.Column, sourceRange.RowCount, 1, FillSeries.Column);
                if (source != null)
                {
                    if (fillType == AutoFillType.CopyCells)
                    {
                        return this._worksheet.GetText(sourceRange.Row, column);
                    }
                    if (fillType == AutoFillType.FillSeries)
                    {
                        return this.GetAutoFillColumnTreadValue(sourceRange, new CellRange(row, column, 1, 1), column, column, source);
                    }
                    return null;
                }
                int num = (row - sourceRange.Row) % sourceRange.RowCount;
                if (num < 0)
                {
                    num += sourceRange.RowCount;
                }
                return this._worksheet.GetText(sourceRange.Row + num, sourceRange.Column);
            }
            if ((fillDirection != FillDirection.Right) && (fillDirection != FillDirection.Left))
            {
                return null;
            }
            NumberSource trendData = this.GetSeriesSource(sourceRange.Row, sourceRange.Column, 1, sourceRange.ColumnCount, FillSeries.Row);
            if (trendData != null)
            {
                if (fillType == AutoFillType.CopyCells)
                {
                    return this._worksheet.GetText(row, sourceRange.Column);
                }
                if (fillType == AutoFillType.FillSeries)
                {
                    return this.GetAutoFillRowTreadValue(sourceRange, new CellRange(row, column, 1, 1), column, column, trendData);
                }
                return null;
            }
            int num2 = (column - sourceRange.Column) % sourceRange.ColumnCount;
            if (num2 < 0)
            {
                num2 += sourceRange.ColumnCount;
            }
            return this._worksheet.GetText(sourceRange.Row, sourceRange.Column + num2);
        }

        double GetNextDateValue(FillDateUnit dateUnit, double initValue, double currentValue, double stepValue, int nextIndex)
        {
            double num = currentValue;
            switch (dateUnit)
            {
                case FillDateUnit.Day:
                    return DateTimeExtension.FromOADate(currentValue).AddDays(stepValue).ToOADate();

                case FillDateUnit.Weekday:
                {
                    DateTime @this = DateTimeExtension.FromOADate(currentValue);
                    double num2 = Math.Abs(stepValue);
                    while (num2 > 0.0)
                    {
                        if (stepValue > 0.0)
                        {
                            @this = @this.AddDays(Math.Min(1.0, num2));
                        }
                        else
                        {
                            @this = @this.AddDays(-Math.Min(1.0, num2));
                        }
                        if ((@this.DayOfWeek != ((DayOfWeek) ((int) DayOfWeek.Saturday))) && (@this.DayOfWeek != ((DayOfWeek) ((int) DayOfWeek.Sunday))))
                        {
                            num2--;
                        }
                    }
                    return @this.ToOADate();
                }
                case FillDateUnit.Month:
                    return DateTimeExtension.FromOADate(initValue).AddMonths((int) (nextIndex * stepValue)).ToOADate();

                case FillDateUnit.Year:
                    return DateTimeExtension.FromOADate(initValue).AddYears((int) (nextIndex * stepValue)).ToOADate();
            }
            return num;
        }

        NumberSource GetSeriesSource(int row, int column, int rowCount, int columnCount, FillSeries fillSeries)
        {
            NumberSource source = null;
            switch (fillSeries)
            {
                case FillSeries.Column:
                    for (int i = (row + rowCount) - 1; i >= row; i--)
                    {
                        object obj3 = this._worksheet.GetValue(i, column);
                        if (FormatConverter.IsNumber(obj3))
                        {
                            if (source == null)
                            {
                                source = new NumberSource(row);
                            }
                            source.Insert(0, i, obj3);
                        }
                    }
                    return source;

                case FillSeries.Row:
                    int num;
                    for (num = num = (column + columnCount) - 1; num >= column; num--)
                    {
                        object obj2 = this._worksheet.GetValue(row, num);
                        if (FormatConverter.IsNumber(obj2))
                        {
                            if (source == null)
                            {
                                source = new NumberSource(column);
                            }
                            source.Insert(0, num, obj2);
                        }
                    }
                    return source;
            }
            return source;
        }

        object GetSeriesTrendFillRangeValue(CellRange range, FillSeries fillSeries, FillType fillType)
        {
            Dictionary<long, object> dictionary = new Dictionary<long, object>();
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            if (!this.HasSpans(row, column, rowCount, columnCount))
            {
                this._fillCache.Clear();
                switch (fillSeries)
                {
                    case FillSeries.Column:
                        for (int i = column; i < (column + columnCount); i++)
                        {
                            NumberSource sourceData = this.GetSeriesSource(row, i, rowCount, 1, fillSeries);
                            if ((sourceData != null) && (sourceData.DataCount > 0))
                            {
                                List<double> list2 = this.CalcSeriesTrendData(sourceData, rowCount, fillType);
                                if ((list2 != null) && (list2.Count > 0))
                                {
                                    for (int j = 0; j < list2.Count; j++)
                                    {
                                        int num10 = ((row + j) << 4) | i;
                                        dictionary[(long) num10] = sourceData.ToActualValue(list2[j]);
                                    }
                                }
                            }
                        }
                        break;

                    case FillSeries.Row:
                        for (int k = row; k < (row + rowCount); k++)
                        {
                            NumberSource source = this.GetSeriesSource(k, column, 1, columnCount, fillSeries);
                            if ((source != null) && (source.DataCount > 0))
                            {
                                List<double> list = this.CalcSeriesTrendData(source, columnCount, fillType);
                                if ((list != null) && (list.Count > 0))
                                {
                                    for (int m = 0; m < list.Count; m++)
                                    {
                                        int num7 = (k << 4) | (column + m);
                                        dictionary[(long) num7] = source.ToActualValue(list[m]);
                                    }
                                }
                            }
                        }
                        break;
                }
                this._fillCache.Clear();
            }
            return dictionary;
        }

        bool HasPartSpans(int row, int column, int rowCount, int columnCount)
        {
            IEnumerator enumerator = this._worksheet.SpanModel.GetEnumerator(row, column, rowCount, columnCount);
            while (enumerator.MoveNext())
            {
                CellRange current = (CellRange) enumerator.Current;
                if ((current.Row < row) || ((current.Row + current.RowCount) > (row + rowCount)))
                {
                    return true;
                }
                if ((current.Column < column) || ((current.Column + current.ColumnCount) > (column + columnCount)))
                {
                    return true;
                }
            }
            return false;
        }

        bool HasSpans(int row, int column, int rowCount, int columnCount)
        {
            return this._worksheet.SpanModel.GetEnumerator(row, column, rowCount, columnCount).MoveNext();
        }

        CellRange InflateCellRange(CellRange range)
        {
            List<CellRange> spans = this._worksheet.GetSpans();
            if ((spans != null) && (range != null))
            {
                range = this._worksheet.CellRangeInflate(spans, range);
            }
            return range;
        }

        bool IsArithmeticProgression(List<int> indexes, List<double> values)
        {
            if (indexes.Count != values.Count)
            {
                return false;
            }
            int num = values.Count;
            if (num <= 1)
            {
                return false;
            }
            if (num != 2)
            {
                int num2 = indexes[1] - indexes[0];
                double num3 = values[1] - values[0];
                for (int i = 2; i < num; i++)
                {
                    if ((indexes[i] - indexes[i - 1]) != num2)
                    {
                        return false;
                    }
                    if ((values[i] - values[i - 1]) != num3)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        void SeriesFillRange(CellRange range, FillSeries fillSeries, FillType type, double stepValue, double? stopValue, FillDateUnit? dateUnit)
        {
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            if (this.HasSpans(row, column, rowCount, columnCount))
            {
                throw new ArgumentException(ResourceStrings.RangeShouldNotHaveMergedCell);
            }
            this._fillCache.Clear();
            switch (fillSeries)
            {
                case FillSeries.Column:
                    for (int i = column; i < (column + columnCount); i++)
                    {
                        NumberSource sourceData = this.GetSeriesSource(row, i, 1, 1, fillSeries);
                        if ((sourceData != null) && (sourceData.DataCount > 0))
                        {
                            List<double> list2 = this.CalcSeriesData(sourceData, rowCount, type, stepValue, stopValue, dateUnit);
                            if ((list2 != null) && (list2.Count > 0))
                            {
                                for (int j = 0; j < list2.Count; j++)
                                {
                                    this.CopyCell(this._worksheet, sourceData.Indexes[0], i, row + j, i, sourceData.ToActualValue(list2[j]), type);
                                }
                            }
                        }
                    }
                    break;

                case FillSeries.Row:
                    for (int k = row; k < (row + rowCount); k++)
                    {
                        NumberSource source = this.GetSeriesSource(k, column, 1, 1, fillSeries);
                        if ((source != null) && (source.DataCount > 0))
                        {
                            List<double> list = this.CalcSeriesData(source, columnCount, type, stepValue, stopValue, dateUnit);
                            if ((list != null) && (list.Count > 0))
                            {
                                for (int m = 0; m < list.Count; m++)
                                {
                                    this.CopyCell(this._worksheet, k, source.Indexes[0], k, column + m, source.ToActualValue(list[m]), type);
                                }
                            }
                        }
                    }
                    break;
            }
            this._fillCache.Clear();
        }

        void SeriesTrendFillRange(CellRange range, FillSeries fillSeries, FillType fillType)
        {
            CellRange range2 = this.FixRange(range);
            int row = range2.Row;
            int column = range2.Column;
            int rowCount = range2.RowCount;
            int columnCount = range2.ColumnCount;
            if (this.HasSpans(row, column, rowCount, columnCount))
            {
                throw new ArgumentException(ResourceStrings.RangeShouldNotHaveMergedCell);
            }
            this._fillCache.Clear();
            switch (fillSeries)
            {
                case FillSeries.Column:
                    for (int i = column; i < (column + columnCount); i++)
                    {
                        NumberSource sourceData = this.GetSeriesSource(row, i, rowCount, 1, fillSeries);
                        if ((sourceData != null) && (sourceData.DataCount > 0))
                        {
                            List<double> list2 = this.CalcSeriesTrendData(sourceData, rowCount, fillType);
                            if ((list2 != null) && (list2.Count > 0))
                            {
                                for (int j = 0; j < list2.Count; j++)
                                {
                                    this.CopyCell(this._worksheet, sourceData.Indexes[0], i, row + j, i, sourceData.ToActualValue(list2[j]), fillType);
                                }
                            }
                        }
                    }
                    break;

                case FillSeries.Row:
                    for (int k = row; k < (row + rowCount); k++)
                    {
                        NumberSource source = this.GetSeriesSource(k, column, 1, columnCount, fillSeries);
                        if ((source != null) && (source.DataCount > 0))
                        {
                            List<double> list = this.CalcSeriesTrendData(source, columnCount, fillType);
                            if ((list != null) && (list.Count > 0))
                            {
                                for (int m = 0; m < list.Count; m++)
                                {
                                    this.CopyCell(this._worksheet, k, source.Indexes[0], k, column + m, source.ToActualValue(list[m]), fillType);
                                }
                            }
                        }
                    }
                    break;
            }
            this._fillCache.Clear();
        }

        class FillCachePool
        {
            Dictionary<ulong, StyleInfo> _cacheStyle = new Dictionary<ulong, StyleInfo>();
            Worksheet _sheet;

            public FillCachePool(Worksheet sheet)
            {
                this._sheet = sheet;
            }

            public void Clear()
            {
                this._cacheStyle.Clear();
            }

            public StyleInfo GetCachedStyle(int row, int column)
            {
                StyleInfo info;
                ulong num = (ulong)row;
                num = num << 0x20;
                num += (ulong)column;
                if (this._cacheStyle.TryGetValue(num, out info))
                {
                    return info;
                }
                StyleInfo info2 = this._sheet.GetCompositeStyle(row, column, SheetArea.Cells, this._sheet.GetStorage(SheetArea.Cells));
                this._cacheStyle.Add(num, info2);
                return info2;
            }
        }

        enum FillType
        {
            Direction,
            Linear,
            Growth,
            Date,
            Auto
        }

        class NumberSource
        {
            List<int> _indexes;
            List<double> _innerValues;
            int _startIndex;
            Type _type;

            public NumberSource() : this(-1)
            {
            }

            public NumberSource(int startIndex)
            {
                this._startIndex = startIndex;
                this._indexes = new List<int>();
                this._innerValues = new List<double>();
                this._type = null;
            }

            public void Add(int index, object value)
            {
                if (this._type == null)
                {
                    if (value is DateTime)
                    {
                        this._type = typeof(DateTime);
                    }
                    else if (value is TimeSpan)
                    {
                        this._type = typeof(TimeSpan);
                    }
                    else
                    {
                        this._type = typeof(double);
                    }
                }
                this._indexes.Add(index);
                this._innerValues.Add(this.ToDouble(value));
            }

            public void Insert(int insertIndex, int index, object value)
            {
                if (!FormatConverter.IsNumber(value))
                {
                    throw new NotSupportedException(ResourceStrings.NumberSourceOnlyWorkedWithNumbers);
                }
                if (this._type == null)
                {
                    if (value is DateTime)
                    {
                        this._type = typeof(DateTime);
                    }
                    else if (value is TimeSpan)
                    {
                        this._type = typeof(TimeSpan);
                    }
                    else
                    {
                        this._type = typeof(double);
                    }
                }
                this._indexes.Insert(insertIndex, index);
                this._innerValues.Insert(insertIndex, this.ToDouble(value));
            }

            public object ToActualValue(double value)
            {
                if (this.DataType == typeof(DateTime))
                {
                    return this.ToDateTime((double) value);
                }
                if (this.DataType == typeof(TimeSpan))
                {
                    return this.ToTimeSpan((double) value);
                }
                return (double) value;
            }

            DateTime ToDateTime(object value)
            {
                DateTime? nullable = FormatConverter.TryDateTime(value, false);
                if (!nullable.HasValue)
                {
                    return DateTimeExtension.FromOADate(0.0);
                }
                return nullable.Value;
            }

            double ToDouble(object value)
            {
                double? nullable = FormatConverter.TryDouble(value, false);
                return (nullable.HasValue ? ((double) nullable.GetValueOrDefault()) : 0.0);
            }

            TimeSpan ToTimeSpan(object value)
            {
                TimeSpan? nullable = FormatConverter.TryTimeSpan(value, false);
                if (!nullable.HasValue)
                {
                    return TimeSpan.Zero;
                }
                return nullable.Value;
            }

            public int DataCount
            {
                get { return  this._innerValues.Count; }
            }

            public Type DataType
            {
                get { return  this._type; }
            }

            public List<int> Indexes
            {
                get { return  this._indexes; }
            }

            public object[,] Indexes2
            {
                get
                {
                    int num = this._indexes.Count;
                    if (num <= 0)
                    {
                        return null;
                    }
                    int startIndex = this.StartIndex;
                    if (startIndex == -1)
                    {
                        startIndex = this._indexes[0];
                    }
                    object[,] objArray = new object[1, num];
                    for (int i = 0; i < num; i++)
                    {
                        objArray[0, i] = (int) ((this._indexes[i] - startIndex) + 1);
                    }
                    return objArray;
                }
            }

            public List<double> InnerValues
            {
                get { return  this._innerValues; }
            }

            public int StartIndex
            {
                get { return  this._startIndex; }
            }

            public List<object> Values
            {
                get
                {
                    if (this._innerValues.Count <= 0)
                    {
                        return new List<object>();
                    }
                    List<object> list = new List<object>();
                    foreach (double num in this._innerValues)
                    {
                        object obj2;
                        if (this.DataType == typeof(DateTime))
                        {
                            obj2 = this.ToDateTime((double) num);
                        }
                        else if (this.DataType == typeof(TimeSpan))
                        {
                            obj2 = this.ToTimeSpan((double) num);
                        }
                        else
                        {
                            obj2 = (double) num;
                        }
                        list.Add(obj2);
                    }
                    return list;
                }
            }

            public object[,] Values2
            {
                get
                {
                    int num = this._innerValues.Count;
                    if (num <= 0)
                    {
                        return null;
                    }
                    object[,] objArray = new object[1, num];
                    for (int i = 0; i < num; i++)
                    {
                        objArray[0, i] = (double) this._innerValues[i];
                    }
                    return objArray;
                }
            }
        }
    }
}

