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
using System.Runtime.InteropServices;
using Windows.Foundation;
#endregion

namespace Dt.Cells.Data
{
    internal static class SpreadChartUtility
    {
        public static bool AreSegmentsAligned(List<List<SheetCellRange>> ranges, DataOrientation dataOrientation)
        {
            if (ranges.Count == 0)
            {
                return false;
            }
            int num = -1;
            int[] numArray = null;
            int[] numArray2 = null;
            bool[] flagArray = null;
            foreach (List<SheetCellRange> list in ranges)
            {
                if ((num != -1) && (list.Count != num))
                {
                    return false;
                }
                num = list.Count;
                if (dataOrientation == DataOrientation.Vertical)
                {
                    if ((numArray != null) && (numArray2 != null))
                    {
                        for (int i = 0; i < num; i++)
                        {
                            int row = list[i].Row;
                            int num4 = (list[i].Row + list[i].RowCount) - 1;
                            if ((row != numArray[i]) || (num4 != numArray2[i]))
                            {
                                flagArray[i] = false;
                                return false;
                            }
                            flagArray[i] = true;
                            numArray[i] = row;
                            numArray2[i] = num4;
                        }
                    }
                    if ((numArray == null) && (numArray2 == null))
                    {
                        numArray = new int[num];
                        numArray2 = new int[num];
                        flagArray = new bool[num];
                        for (int j = 0; j < list.Count; j++)
                        {
                            numArray[j] = list[j].Row;
                            numArray2[j] = (list[j].Row + list[j].RowCount) - 1;
                            flagArray[j] = true;
                        }
                    }
                }
                else
                {
                    if ((numArray != null) && (numArray2 != null))
                    {
                        for (int k = 0; k < num; k++)
                        {
                            int column = list[k].Column;
                            int num8 = (list[k].Column + list[k].ColumnCount) - 1;
                            if ((column != numArray[k]) || (num8 != numArray2[k]))
                            {
                                flagArray[k] = false;
                                return false;
                            }
                            flagArray[k] = true;
                            numArray[k] = column;
                            numArray2[k] = num8;
                        }
                    }
                    if ((numArray == null) && (numArray2 == null))
                    {
                        numArray = new int[num];
                        numArray2 = new int[num];
                        flagArray = new bool[num];
                        for (int m = 0; m < list.Count; m++)
                        {
                            numArray[m] = list[m].Column;
                            numArray2[m] = (list[m].Column + list[m].ColumnCount) - 1;
                            flagArray[m] = true;
                        }
                    }
                }
            }
            return true;
        }

        public static Dictionary<int, List<SheetCellRange>> ArrangeRangesByOrientation(IEnumerable<SheetCellRange> ranges, DataOrientation dataOrientation)
        {
            Dictionary<int, List<SheetCellRange>> dictionary = new Dictionary<int, List<SheetCellRange>>();
            if (dataOrientation == DataOrientation.Vertical)
            {
                foreach (SheetCellRange range in ranges)
                {
                    if (!dictionary.ContainsKey(range.Column))
                    {
                        dictionary.Add(range.Column, new List<SheetCellRange>());
                    }
                    if (!dictionary[range.Column].Contains(range))
                    {
                        dictionary[range.Column].Add(range);
                    }
                }
                return dictionary;
            }
            foreach (SheetCellRange range2 in ranges)
            {
                if (!dictionary.ContainsKey(range2.Row))
                {
                    dictionary.Add(range2.Row, new List<SheetCellRange>());
                }
                if (!dictionary[range2.Row].Contains(range2))
                {
                    dictionary[range2.Row].Add(range2);
                }
            }
            return dictionary;
        }

        public static bool DetectRanges(Worksheet sheetView, int row, int column, int rowCount, int columnCount, out CellRange seriesName, out CellRange category, out CellRange data)
        {
            int num = sheetView.RowCount;
            int num2 = sheetView.ColumnCount;
            if (((rowCount < 1) && (row != -1)) || ((columnCount < 1) && (column != -1)))
            {
                throw new ArgumentException("Cannot detect an empty cell range.");
            }
            TrimCellRange(sheetView, ref row, ref column, ref rowCount, ref columnCount);
            category = null;
            seriesName = null;
            data = null;
            if ((rowCount <= 0) || (columnCount <= 0))
            {
                return false;
            }
            Windows.Foundation.Rect rect = new Windows.Foundation.Rect((double) column, (double) row, (double) columnCount, (double) rowCount);
            Windows.Foundation.Rect rect2 = new Windows.Foundation.Rect(0.0, 0.0, (double) num2, (double) num);
            rect.Intersect(rect2);
            row = (int) rect.Y;
            column = (int) rect.X;
            rowCount = (int) rect.Height;
            columnCount = (int) rect.Width;
            if ((row < 0) || (column < 0))
            {
                throw new ArgumentException("The cell range is invalid.");
            }
            if ((rowCount == 1) && (columnCount == 1))
            {
                if (!IsEmptyCell(sheetView, row, column))
                {
                    data = new CellRange(row, column, rowCount, columnCount);
                }
                return false;
            }
            if (IsEmptyCell(sheetView, row, column))
            {
                int c = column + 1;
                int r = row + 1;
                while ((c < (column + columnCount)) && IsEmptyCell(sheetView, row, c))
                {
                    c++;
                }
                int num3 = c;
                int num4 = (column + columnCount) - num3;
                if (num4 == 0)
                {
                    while ((r < (row + rowCount)) && IsEmptyCell(sheetView, r, column))
                    {
                        r++;
                    }
                    if (r == (row + rowCount))
                    {
                        if (rowCount > 1)
                        {
                            if (columnCount > 1)
                            {
                                category = new CellRange(row, column + 1, 1, columnCount - 1);
                                seriesName = new CellRange(row + 1, column, rowCount - 1, 1);
                                data = new CellRange(row + 1, column + 1, rowCount - 1, columnCount - 1);
                            }
                            else
                            {
                                seriesName = new CellRange(row, column, 1, 1);
                                data = new CellRange(row + 1, column, rowCount - 1, 1);
                            }
                        }
                        else
                        {
                            seriesName = new CellRange(row, column, 1, 1);
                            data = new CellRange(row, column + 1, rowCount, columnCount - 1);
                        }
                    }
                    else if (columnCount > 1)
                    {
                        if ((columnCount > 1) && ((r - row) >= 1))
                        {
                            category = new CellRange(row, column + 1, r - row, columnCount - 1);
                        }
                        if ((rowCount > 1) && ((row + rowCount) > r))
                        {
                            seriesName = new CellRange(r, column, (row + rowCount) - r, 1);
                        }
                        data = new CellRange(r, column + 1, (row + rowCount) - r, columnCount - 1);
                    }
                    else
                    {
                        seriesName = null;
                        if (((rowCount > 1) && (r > row)) && (r < (row + rowCount)))
                        {
                            category = new CellRange(row, column, r - row, columnCount);
                        }
                        if ((r < (row + rowCount)) && (r > row))
                        {
                            data = new CellRange(r, column, (row + rowCount) - r, columnCount);
                        }
                        else
                        {
                            data = new CellRange(row, column, rowCount, columnCount);
                        }
                    }
                }
                else
                {
                    c--;
                    while ((r < (row + rowCount)) && IsEmptyCell(sheetView, r, c))
                    {
                        r++;
                    }
                    if (r == (row + rowCount))
                    {
                        if (rowCount > 1)
                        {
                            if ((columnCount > 1) && ((r - row) >= 1))
                            {
                                category = new CellRange(row, num3, 1, num4);
                            }
                            if ((rowCount > 1) && (columnCount > num4))
                            {
                                seriesName = new CellRange(row + 1, column, rowCount - 1, columnCount - num4);
                            }
                            data = new CellRange(row + 1, num3, rowCount - 1, num4);
                        }
                        else
                        {
                            seriesName = new CellRange(row, column, rowCount, num3 - column);
                            data = new CellRange(row, num3, rowCount, num4);
                        }
                    }
                    else
                    {
                        if ((columnCount > 1) && ((r - row) >= 1))
                        {
                            category = new CellRange(row, num3, r - row, num4);
                        }
                        if (((rowCount > 1) && ((row + rowCount) > r)) && (columnCount > num4))
                        {
                            seriesName = new CellRange(r, column, (row + rowCount) - r, columnCount - num4);
                        }
                        data = new CellRange(r, num3, (row + rowCount) - r, num4);
                    }
                }
            }
            else
            {
                int num7 = (column + columnCount) - 1;
                int num8 = (row + rowCount) - 1;
                if ((columnCount > 1) && (rowCount > 1))
                {
                    int num9 = num7;
                    int num10 = 0;
                    while ((num9 >= column) && IsNumericData(sheetView.GetValue(num8, num9)))
                    {
                        num10++;
                        num9--;
                    }
                    bool flag = num10 == 0;
                    if (flag)
                    {
                        num10 = 1;
                        num9--;
                    }
                    int num11 = num7;
                    if (columnCount > 1)
                    {
                        num11 = num9 + 1;
                        num9 = num8 - 1;
                        int num12 = 1;
                        if (!flag)
                        {
                            while ((num9 >= row) && IsNumericData(sheetView.GetValue(num9, num7)))
                            {
                                num12++;
                                num9--;
                            }
                        }
                        if (((num10 > 0) && (num11 >= column)) && ((num9 + 1) > row))
                        {
                            category = new CellRange(row, num11, (num9 + 1) - row, num10);
                        }
                        if ((num12 > 0) && (num11 > column))
                        {
                            seriesName = new CellRange(num9 + 1, column, num12, num11 - column);
                        }
                        data = new CellRange(num9 + 1, num11, num12, num10);
                    }
                }
                else if (rowCount > 1)
                {
                    if (IsNumericData(sheetView.GetValue(row, column)))
                    {
                        data = new CellRange(row, column, rowCount, columnCount);
                    }
                    else
                    {
                        int num13 = 0;
                        int num14 = num8;
                        while ((num14 >= row) && IsNumericData(sheetView.GetValue(num14, column)))
                        {
                            num14--;
                            num13++;
                        }
                        if ((num14 == num8) && !IsNumericData(sheetView.GetValue(num14, column)))
                        {
                            num14--;
                        }
                        data = new CellRange(num14 + 1, column, num8 - num14, columnCount);
                        if (num14 >= row)
                        {
                            seriesName = new CellRange(row, column, (num14 - row) + 1, columnCount);
                        }
                    }
                }
                else if (IsNumericData(sheetView.GetValue(row, column)))
                {
                    data = new CellRange(row, column, rowCount, columnCount);
                }
                else
                {
                    int num15 = 0;
                    int num16 = num7;
                    while ((num16 >= column) && IsNumericData(sheetView.GetValue(row, num16)))
                    {
                        num16--;
                        num15++;
                    }
                    if ((num16 == num7) && !IsNumericData(sheetView.GetValue(row, num16)))
                    {
                        num16--;
                    }
                    data = new CellRange(row, num16 + 1, rowCount, num7 - num16);
                    if (num16 >= column)
                    {
                        seriesName = new CellRange(row, column, rowCount, (num16 - column) + 1);
                    }
                }
            }
            if ((((data != null) && (data.RowCount > data.ColumnCount)) && (data.RowCount > 1)) && ((data.ColumnCount > 1) || ((data.ColumnCount == 1) && ((seriesName != null) || (category != null)))))
            {
                bool flag2 = false;
                if (seriesName != null)
                {
                    flag2 = seriesName.Row == data.Row;
                }
                else if (category != null)
                {
                    flag2 = category.Column == data.Column;
                }
                if (flag2)
                {
                    CellRange range = category;
                    category = seriesName;
                    seriesName = range;
                }
                return true;
            }
            if (data != null)
            {
                if (data.RowCount == 1)
                {
                    return false;
                }
                if (data.ColumnCount == 1)
                {
                    return true;
                }
            }
            return (((seriesName != null) && (seriesName.Column == data.Column)) || ((category != null) && (category.Row == data.Row)));
        }

        public static SheetCellRange ExtractRange(ICalcEvaluator sheet, CalcExpression value)
        {
            if ((value != null) && !(value is CalcConstantExpression))
            {
                if ((value is CalcExternalCellExpression) || (value is CalcExternalRangeExpression))
                {
                    CalcExternalCellExpression expression = value as CalcExternalCellExpression;
                    if (expression != null)
                    {
                        return new SheetCellRange(expression.Source as Worksheet, expression.Row, expression.Column, 1, 1);
                    }
                    CalcExternalRangeExpression expression2 = value as CalcExternalRangeExpression;
                    if (expression2 != null)
                    {
                        return new SheetCellRange(expression2.Source as Worksheet, expression2.StartRow, expression2.StartColumn, (expression2.EndRow - expression2.StartRow) + 1, (expression2.EndColumn - expression2.StartColumn) + 1);
                    }
                }
                else if (value is CalcExternalNameExpression)
                {
                    SheetCellRange[] rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(sheet, value);
                    if ((rangeArray != null) && (rangeArray.Length > 0))
                    {
                        return rangeArray[0];
                    }
                }
            }
            return null;
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedByColumnRangeGroup(Worksheet worksheet)
        {
            List<SpreadChartBase> list = new List<SpreadChartBase>();
            List<SpreadChartBase> list2 = new List<SpreadChartBase>();
            foreach (SpreadChart chart in worksheet.Charts)
            {
                list.Add(chart);
            }
            if (worksheet.ColumnRangeGroup != null)
            {
                foreach (SpreadChartBase base2 in list)
                {
                    SheetCellRange[] rangeArray = null;
                    if (base2 is SpreadChart)
                    {
                        rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(worksheet, (base2 as SpreadChart).Formula);
                    }
                    if (rangeArray != null)
                    {
                        bool flag = false;
                        foreach (SheetCellRange range in rangeArray)
                        {
                            if (range.Sheet == worksheet)
                            {
                                for (int i = range.Column; i < (range.Column + range.ColumnCount); i++)
                                {
                                    if (worksheet.ColumnRangeGroup.GetLevel(i) >= 0)
                                    {
                                        list2.Add(base2);
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return (IList<SpreadChartBase>) list2;
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedByRowFilter(Worksheet worksheet)
        {
            if (((worksheet == null) || (worksheet.RowFilter == null)) || (worksheet.RowFilter.Range == null))
            {
                return null;
            }
            List<SpreadChartBase> list = new List<SpreadChartBase>();
            List<SpreadChartBase> list2 = new List<SpreadChartBase>();
            foreach (SpreadChart chart in worksheet.Charts)
            {
                list.Add(chart);
            }
            CellRange range = new CellRange(worksheet.RowFilter.Range.Row, -1, worksheet.RowFilter.Range.RowCount, -1);
            foreach (SpreadChartBase base2 in list)
            {
                SheetCellRange[] rangeArray = null;
                if (base2 is SpreadChart)
                {
                    rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(worksheet, (base2 as SpreadChart).Formula);
                }
                if (rangeArray != null)
                {
                    foreach (SheetCellRange range2 in rangeArray)
                    {
                        if ((range2.Sheet == worksheet) && range.Intersects(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount))
                        {
                            list2.Add(base2);
                            break;
                        }
                    }
                }
            }
            return (IList<SpreadChartBase>) list2;
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedByRowRangeGroup(Worksheet worksheet)
        {
            List<SpreadChartBase> list = new List<SpreadChartBase>();
            List<SpreadChartBase> list2 = new List<SpreadChartBase>();
            if (worksheet.RowRangeGroup != null)
            {
                foreach (SpreadChart chart in worksheet.Charts)
                {
                    list.Add(chart);
                }
                foreach (SpreadChartBase base2 in list)
                {
                    SheetCellRange[] rangeArray = null;
                    if (base2 is SpreadChart)
                    {
                        rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(worksheet, (base2 as SpreadChart).Formula);
                    }
                    if (rangeArray != null)
                    {
                        bool flag = false;
                        foreach (SheetCellRange range in rangeArray)
                        {
                            if (range.Sheet == worksheet)
                            {
                                for (int i = range.Row; i < (range.Row + range.RowCount); i++)
                                {
                                    if (worksheet.RowRangeGroup.GetLevel(i) >= 0)
                                    {
                                        list2.Add(base2);
                                        flag = true;
                                        break;
                                    }
                                }
                            }
                            if (flag)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            return (IList<SpreadChartBase>) list2;
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedBySheetTable(Worksheet worksheet, SheetTable table)
        {
            if ((worksheet == null) || (table == null))
            {
                return null;
            }
            List<SpreadChartBase> list = new List<SpreadChartBase>();
            List<SpreadChartBase> list2 = new List<SpreadChartBase>();
            foreach (SpreadChart chart in worksheet.Charts)
            {
                list.Add(chart);
            }
            foreach (SpreadChartBase base2 in list)
            {
                SheetCellRange[] rangeArray = null;
                if (base2 is SpreadChart)
                {
                    rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(worksheet, (base2 as SpreadChart).Formula);
                }
                if (rangeArray != null)
                {
                    foreach (SheetCellRange range in rangeArray)
                    {
                        if ((range.Sheet == worksheet) && table.DataRange.Intersects(range.Row, range.Column, range.RowCount, range.ColumnCount))
                        {
                            list2.Add(base2);
                            break;
                        }
                    }
                }
            }
            return (IList<SpreadChartBase>) list2;
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedCellChanged(Worksheet worksheet, int row, int column)
        {
            return GetChartShapeAffectedCellChanged(worksheet, row, column, 1, 1);
        }

        public static IList<SpreadChartBase> GetChartShapeAffectedCellChanged(Worksheet worksheet, int row, int column, int rowCount, int columnCount)
        {
            List<SpreadChartBase> list = new List<SpreadChartBase>();
            List<SpreadChartBase> list2 = new List<SpreadChartBase>();
            foreach (SpreadChart chart in worksheet.Charts)
            {
                list.Add(chart);
            }
            CellRange range = new CellRange(row, column, rowCount, columnCount);
            foreach (SpreadChartBase base2 in list)
            {
                SheetCellRange[] rangeArray = null;
                if (base2 is SpreadChart)
                {
                    rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(worksheet, (base2 as SpreadChart).Formula);
                }
                if ((rangeArray != null) && (rangeArray != null))
                {
                    foreach (SheetCellRange range2 in rangeArray)
                    {
                        if ((range2.Sheet == worksheet) && range.Intersects(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount))
                        {
                            list2.Add(base2);
                            break;
                        }
                    }
                }
            }
            return (IList<SpreadChartBase>) list2;
        }

        public static int GetDataDimension(SpreadChartType chartType)
        {
            switch (chartType)
            {
                case SpreadChartType.BarClustered:
                case SpreadChartType.BarStacked:
                case SpreadChartType.BarStacked100pc:
                case SpreadChartType.ColumnClustered:
                case SpreadChartType.ColumnStacked:
                case SpreadChartType.ColumnStacked100pc:
                case SpreadChartType.Line:
                case SpreadChartType.LineStacked:
                case SpreadChartType.LineStacked100pc:
                case SpreadChartType.LineWithMarkers:
                case SpreadChartType.LineStackedWithMarkers:
                case SpreadChartType.LineStacked100pcWithMarkers:
                case SpreadChartType.Pie:
                case SpreadChartType.PieExploded:
                case SpreadChartType.PieDoughnut:
                case SpreadChartType.PieExplodedDoughnut:
                case SpreadChartType.Area:
                case SpreadChartType.AreaStacked:
                case SpreadChartType.AreaStacked100pc:
                case SpreadChartType.Radar:
                case SpreadChartType.RadarWithMarkers:
                case SpreadChartType.RadarFilled:
                    return 1;

                case SpreadChartType.Scatter:
                case SpreadChartType.ScatterLines:
                case SpreadChartType.ScatterLinesWithMarkers:
                case SpreadChartType.ScatterLinesSmoothed:
                case SpreadChartType.ScatterLinesSmoothedWithMarkers:
                    return 2;

                case SpreadChartType.Bubble:
                    return 3;

                case SpreadChartType.StockHighLowOpenClose:
                    return 5;
            }
            return 1;
        }

        public static int GetDataDimension(SpreadDataSeries dataSereis)
        {
            if (dataSereis.GetType() == typeof(SpreadDataSeries))
            {
                return 1;
            }
            if (dataSereis.GetType() == typeof(SpreadXYDataSeries))
            {
                return 2;
            }
            if ((dataSereis.GetType() == typeof(SpreadBubbleSeries)) || (dataSereis.GetType() == typeof(SpreadXYZDataSeries)))
            {
                return 3;
            }
            if (dataSereis.GetType() == typeof(SpreadOpenHighLowCloseSeries))
            {
                return 5;
            }
            return GetDataDimension(dataSereis.ChartType);
        }

        public static bool Is100PercentChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("100pc");
        }

        public static bool IsAreaChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("area");
        }

        public static bool IsBarChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("bar");
        }

        public static bool IsBubbleChart(SpreadChartType chartType)
        {
            return chartType.ToString().ToLower().Contains("bubble");
        }

        public static bool IsChartDataVertical(CellRange seriesName, CellRange category, CellRange data)
        {
            if ((((data != null) && (data.RowCount > data.ColumnCount)) && (data.RowCount > 1)) && ((data.ColumnCount > 1) || ((data.ColumnCount == 1) && ((seriesName != null) || (category != null)))))
            {
                return true;
            }
            if (data != null)
            {
                if (data.RowCount == 1)
                {
                    return false;
                }
                if (data.ColumnCount == 1)
                {
                    return true;
                }
            }
            return (((seriesName != null) && (seriesName.Column == data.Column)) || ((category != null) && (category.Row == data.Row)));
        }

        public static bool IsChartWithMarker(SpreadChartType chartType)
        {
            if (!chartType.ToString().ToLower().Contains("marker"))
            {
                return (chartType == SpreadChartType.Scatter);
            }
            return true;
        }

        public static bool IsColumnChart(SpreadChartType chartType)
        {
            return chartType.ToString().ToLower().Contains("column");
        }

        static bool IsEmptyCell(Worksheet sheetView, int r, int c)
        {
            object obj2 = sheetView.GetValue(r, c);
            if ((obj2 != null) && (obj2.ToString().Length > 0))
            {
                return false;
            }
            return true;
        }

        public static bool IsLineOrLineWithMarkerChart(SpreadChartType chartType)
        {
            if (((((chartType != SpreadChartType.Line) && (chartType != SpreadChartType.LineSmoothed)) && ((chartType != SpreadChartType.LineStacked) && (chartType != SpreadChartType.LineStacked100pc))) && (((chartType != SpreadChartType.LineWithMarkers) && (chartType != SpreadChartType.LineWithMarkersSmoothed)) && ((chartType != SpreadChartType.LineStackedWithMarkers) && (chartType != SpreadChartType.LineStacked100pcWithMarkers)))) && ((((chartType != SpreadChartType.Scatter) && (chartType != SpreadChartType.ScatterLines)) && ((chartType != SpreadChartType.ScatterLinesWithMarkers) && (chartType != SpreadChartType.ScatterLinesSmoothed))) && ((chartType != SpreadChartType.ScatterLinesSmoothedWithMarkers) && (chartType != SpreadChartType.Radar))))
            {
                return (chartType == SpreadChartType.RadarWithMarkers);
            }
            return true;
        }

        static bool IsNumericData(object value)
        {
            decimal num;
            if (value == null)
            {
                return true;
            }
            if (((((value is byte) || (value is short)) || ((value is int) || (value is long))) || (((value is ushort) || (value is uint)) || ((value is ulong) || (value is float)))) || (((value is double) || (value is decimal)) || ((value is TimeSpan) || (value is DateTime))))
            {
                return true;
            }
            string str = value.ToString();
            return ((str.Length == 0) || decimal.TryParse(str, out num));
        }

        public static bool IsPieChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("pie");
        }

        public static bool IsRadarChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("radar");
        }

        public static bool IsSeriesHasMarker(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("marker");
        }

        public static bool IsStackedChart(SpreadChartType charType)
        {
            return charType.ToString().ToLower().Contains("stacked");
        }

        public static bool IsSymbolChart(SpreadChartType chartType)
        {
            if (((((chartType != SpreadChartType.BarClustered) && (chartType != SpreadChartType.BarStacked)) && ((chartType != SpreadChartType.BarStacked100pc) && (chartType != SpreadChartType.ColumnClustered))) && (((chartType != SpreadChartType.ColumnStacked) && (chartType != SpreadChartType.ColumnStacked100pc)) && ((chartType != SpreadChartType.Bubble) && (chartType != SpreadChartType.Pie)))) && ((chartType != SpreadChartType.PieExploded) && (chartType != SpreadChartType.PieDoughnut)))
            {
                return (chartType == SpreadChartType.PieExplodedDoughnut);
            }
            return true;
        }

        static void TrimCellRange(Worksheet sheetView, ref int row, ref int column, ref int rowCount, ref int columnCount)
        {
            bool flag2;
            bool flag3;
            int num = sheetView.RowCount;
            int num2 = sheetView.ColumnCount;
            if ((num == 0) || (num2 == 0))
            {
                return;
            }
            if ((row != -1) && ((row != 0) || (rowCount != num)))
            {
                goto Label_00CE;
            }
            bool flag = column == -1;
            if (flag)
            {
                column = 0;
                columnCount = num2;
            }
            row = 0;
            rowCount = num;
        Label_003F:
            flag2 = true;
            int c = column;
            int num4 = column + columnCount;
            while (c < num4)
            {
                if (!IsEmptyCell(sheetView, row, c))
                {
                    flag2 = false;
                    break;
                }
                c++;
            }
            if (flag2)
            {
                row++;
                rowCount--;
                if (rowCount == 0)
                {
                    return;
                }
                goto Label_003F;
            }
            int r = (row + rowCount) - 1;
        Label_0088:
            flag2 = true;
            int num6 = column;
            int num7 = column + columnCount;
            while (num6 < num7)
            {
                if (!IsEmptyCell(sheetView, r, num6))
                {
                    flag2 = false;
                    break;
                }
                num6++;
            }
            if (flag2)
            {
                rowCount--;
                r--;
                if (rowCount == 0)
                {
                    return;
                }
                goto Label_0088;
            }
            if (flag)
            {
                column = -1;
            }
        Label_00CE:
            if ((column != -1) && ((column != 0) || (columnCount != num2)))
            {
                return;
            }
            column = 0;
            columnCount = num2;
        Label_00EA:
            flag3 = true;
            int num8 = row;
            int num9 = row + rowCount;
            while (num8 < num9)
            {
                if (!IsEmptyCell(sheetView, num8, column))
                {
                    flag3 = false;
                    break;
                }
                num8++;
            }
            if (flag3)
            {
                column++;
                columnCount--;
                if (columnCount == 0)
                {
                    return;
                }
                goto Label_00EA;
            }
            int num10 = (column + columnCount) - 1;
        Label_0138:
            flag3 = true;
            int num11 = row;
            int num12 = row + rowCount;
            while (num11 < num12)
            {
                if (!IsEmptyCell(sheetView, num11, num10))
                {
                    flag3 = false;
                    break;
                }
                num11++;
            }
            if (flag3)
            {
                columnCount--;
                num10--;
                if (columnCount != 0)
                {
                    goto Label_0138;
                }
            }
        }

        public static SheetCellRange[] ValidateFormula(ICalcEvaluator evaluator, string formula, bool acceptConstantExpression = false)
        {
            if (string.IsNullOrEmpty(formula) || (evaluator == null))
            {
                return null;
            }
            CalcExpression expression = FormulaUtility.Formula2Expression(evaluator, formula);
            if (expression == null)
            {
                throw new ArgumentException("Formula is invalid.");
            }
            if ((expression is CalcConstantExpression) && acceptConstantExpression)
            {
                return null;
            }
            SheetCellRange[] rangeArray = SheetCellRangeUtility.ExtractAllExternalReference(evaluator, formula);
            if (rangeArray == null)
            {
                throw new ArgumentException("Formula is invalid.");
            }
            if (rangeArray.Length > 0)
            {
                Worksheet sheet = null;
                foreach (SheetCellRange range in rangeArray)
                {
                    if ((sheet != null) && (range.Sheet != sheet))
                    {
                        throw new ArgumentException("Formula is invalid.");
                    }
                    sheet = range.Sheet;
                }
            }
            return rangeArray;
        }
    }
}

