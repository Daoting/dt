#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using System.Collections;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.UndoRedo
{
    internal static class CopyMoveHelper
    {
        public static CellRange AdjustRange(CellRange range, int totalRowCount, int totalColumnCount)
        {
            int row = (range.Row != -1) ? range.Row : 0;
            int column = (range.Column != -1) ? range.Column : 0;
            int rowCount = (range.RowCount != -1) ? range.RowCount : totalRowCount;
            return new CellRange(row, column, rowCount, (range.ColumnCount != -1) ? range.ColumnCount : totalColumnCount);
        }

        internal static ulong ConvertToKey(int row, int column)
        {
            ulong num = 0L;
            num = (ulong)row;
            num = num << 0x20;
            return (num | (uint)column);
        }

        public static FloatingObject[] GetFloatingObjectsInRange(CellRange range, Worksheet sheet)
        {
            List<FloatingObject> list = new List<FloatingObject>();
            foreach (SpreadChart chart in sheet.Charts)
            {
                list.Add(chart);
            }
            foreach (Picture picture in sheet.Pictures)
            {
                list.Add(picture);
            }
            foreach (FloatingObject obj2 in sheet.FloatingObjects)
            {
                list.Add(obj2);
            }
            List<FloatingObject> list2 = new List<FloatingObject>();
            foreach (FloatingObject obj3 in list)
            {
                CellRange range2 = new CellRange(obj3.StartRow, obj3.StartColumn, (obj3.EndRow - obj3.StartRow) + 1, (obj3.EndColumn - obj3.StartColumn) - 1);
                if (range.Contains(range2))
                {
                    list2.Add(obj3);
                }
            }
            return list2.ToArray();
        }

        public static object GetStyleObject(Worksheet sheet, int row, int column, SheetArea area)
        {
            StyleInfo objA = sheet.GetStyleInfo(row, column, area);
            if (objA != null)
            {
                string name = objA.Name;
                StyleInfo objB = null;
                if (sheet.NamedStyles != null)
                {
                    objB = sheet.NamedStyles.Find(name);
                }
                if (((objB == null) && (sheet.Workbook != null)) && (sheet.Workbook.NamedStyles != null))
                {
                    objB = sheet.Workbook.NamedStyles.Find(name);
                }
                if ((objB != null) && object.ReferenceEquals(objA, objB))
                {
                    return name;
                }
            }
            return objA;
        }

        public static List<CellData> GetValues(Worksheet sheet, int row, int column, int rowCount, int columnCount)
        {
            if (row < 0)
            {
                row = 0;
                rowCount = sheet.RowCount;
            }
            if (column < 0)
            {
                column = 0;
                columnCount = sheet.ColumnCount;
            }
            List<CellData> list = new List<CellData>();
            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < columnCount; j++)
                {
                    object obj2 = sheet.GetValue(row + i, column + j);
                    if (obj2 != null)
                    {
                        list.Add(new CellData(i, j, obj2));
                    }
                }
            }
            return list;
        }

        public static void RaiseValueChanged(Excel excel, int row, int column, int rowCount, int columnCount, List<CellData> oldValues)
        {
            if (((excel != null) && (excel._eventSuspended <= 0)) && (oldValues != null))
            {
                if (row < 0)
                {
                    row = 0;
                    rowCount = excel.ActiveSheet.RowCount;
                }
                if (column < 0)
                {
                    column = 0;
                    columnCount = excel.ActiveSheet.ColumnCount;
                }
                List<CellData> cellDatas = new List<CellData>((IEnumerable<CellData>)oldValues);
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        object obj3;
                        object objA = excel.ActiveSheet.GetValue(row + i, column + j);
                        CellData? nullable = Remove(cellDatas, i, j);
                        if (!nullable.HasValue)
                        {
                            obj3 = null;
                        }
                        else
                        {
                            obj3 = nullable.Value;
                        }
                        if (!object.Equals(objA, obj3))
                        {
                            excel.RaiseValueChanged(row + i, column + j);
                        }
                    }
                }
            }
        }

        static CellData? Remove(List<CellData> cellDatas, int row, int column)
        {
            CellData? nullable = null;
            int num = -1;
            int num2 = cellDatas.Count;
            if (num2 > 0)
            {
                for (int i = 0; i < num2; i++)
                {
                    if ((cellDatas[i].Row == row) && (cellDatas[i].Column == column))
                    {
                        num = i;
                        break;
                    }
                }
            }
            if (num != -1)
            {
                nullable = new CellData?(cellDatas[num]);
                cellDatas.RemoveAt(num);
            }
            return nullable;
        }

        public static void SaveColumnHeaderInfo(Worksheet sheet, CopyMoveCellsInfo headerCellsInfo, CopyMoveColumnsInfo columnsInfo, int baseColumn, CopyToOption option)
        {
            int rowCount = headerCellsInfo.RowCount;
            int columnCount = headerCellsInfo.ColumnCount;
            for (int i = 0; i < rowCount; i++)
            {
                for (int k = 0; k < columnCount; k++)
                {
                    if ((option & CopyToOption.Value) > ((CopyToOption)0))
                    {
                        headerCellsInfo.SaveValue(i, k, sheet.GetValue(i, baseColumn + k, SheetArea.ColumnHeader));
                    }
                    if ((option & CopyToOption.Style) > ((CopyToOption)0))
                    {
                        headerCellsInfo.SaveStyle(i, k, GetStyleObject(sheet, i, baseColumn + k, SheetArea.ColumnHeader));
                    }
                    if ((option & CopyToOption.Tag) > ((CopyToOption)0))
                    {
                        headerCellsInfo.SaveTag(i, k, sheet.GetTag(i, baseColumn + k, SheetArea.ColumnHeader));
                    }
                }
            }
            if ((option & CopyToOption.Value) > ((CopyToOption)0))
            {
                for (int m = 0; m < columnCount; m++)
                {
                    if (sheet.IsColumnBound(baseColumn + m))
                    {
                        columnsInfo.SaveBindingField(m, sheet.GetDataColumnName(baseColumn + m));
                    }
                }
            }
            if ((option & CopyToOption.Span) > ((CopyToOption)0))
            {
                IEnumerator enumerator = sheet.ColumnHeaderSpanModel.GetEnumerator(0, baseColumn, rowCount, columnCount);
                while (enumerator.MoveNext())
                {
                    headerCellsInfo.SaveSpan((CellRange)enumerator.Current);
                }
            }
            columnCount = columnsInfo.ColumnCount;
            for (int j = 0; j < columnCount; j++)
            {
                columnsInfo.SaveWidth(j, sheet.GetColumnWidth(baseColumn + j));
                columnsInfo.SaveVisible(j, sheet.GetColumnVisible(baseColumn + j));
                columnsInfo.SaveResizable(j, sheet.GetColumnResizable(baseColumn + j));
                columnsInfo.SaveTag(j, sheet.GetTag(-1, baseColumn + j));
            }
            if ((option & CopyToOption.Style) > ((CopyToOption)0))
            {
                for (int n = 0; n < columnCount; n++)
                {
                    columnsInfo.SaveViewportColumnStyle(n, GetStyleObject(sheet, -1, baseColumn + n, SheetArea.Cells));
                    columnsInfo.SaveHeaderColumnStyle(n, GetStyleObject(sheet, -1, baseColumn + n, SheetArea.ColumnHeader));
                }
            }
            if ((option & CopyToOption.RangeGroup) > ((CopyToOption)0))
            {
                RangeGroup columnRangeGroup = sheet.ColumnRangeGroup;
                if ((columnRangeGroup != null) && !columnRangeGroup.IsEmpty())
                {
                    for (int num8 = 0; num8 < columnCount; num8++)
                    {
                        columnsInfo.SaveRangeGroup(num8, columnRangeGroup.Data.GetLevel(baseColumn + num8), columnRangeGroup.Data.GetCollapsed(baseColumn + num8));
                    }
                }
            }
        }

        public static void SaveRowHeaderInfo(Worksheet sheet, CopyMoveCellsInfo headerCellsInfo, CopyMoveRowsInfo rowsInfo, int baseRow, CopyToOption option)
        {
            if ((option & CopyToOption.All) > ((CopyToOption)0))
            {
                int rowCount = headerCellsInfo.RowCount;
                int columnCount = headerCellsInfo.ColumnCount;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int k = 0; k < columnCount; k++)
                    {
                        if ((option & CopyToOption.Value) > ((CopyToOption)0))
                        {
                            headerCellsInfo.SaveValue(i, k, sheet.GetValue(baseRow + i, k, SheetArea.CornerHeader | SheetArea.RowHeader));
                        }
                        if ((option & CopyToOption.Style) > ((CopyToOption)0))
                        {
                            headerCellsInfo.SaveStyle(i, k, GetStyleObject(sheet, baseRow + i, k, SheetArea.CornerHeader | SheetArea.RowHeader));
                        }
                        if ((option & CopyToOption.Tag) > ((CopyToOption)0))
                        {
                            headerCellsInfo.SaveTag(i, k, sheet.GetTag(baseRow + i, k, SheetArea.CornerHeader | SheetArea.RowHeader));
                        }
                    }
                }
                if ((option & CopyToOption.Span) > ((CopyToOption)0))
                {
                    IEnumerator enumerator = sheet.RowHeaderSpanModel.GetEnumerator(baseRow, 0, rowCount, columnCount);
                    while (enumerator.MoveNext())
                    {
                        headerCellsInfo.SaveSpan((CellRange)enumerator.Current);
                    }
                }
                rowCount = rowsInfo.RowCount;
                for (int j = 0; j < rowCount; j++)
                {
                    rowsInfo.SaveHeight(j, sheet.GetRowHeight(baseRow + j));
                    rowsInfo.SaveVisible(j, sheet.GetRowVisible(baseRow + j));
                    rowsInfo.SaveResizable(j, sheet.GetRowResizable(baseRow + j));
                    rowsInfo.SaveTag(j, sheet.GetTag(baseRow + j, -1));
                }
                if ((option & CopyToOption.Style) > ((CopyToOption)0))
                {
                    for (int m = 0; m < rowCount; m++)
                    {
                        rowsInfo.SaveViewportRowStyle(m, GetStyleObject(sheet, baseRow + m, -1, SheetArea.Cells));
                        rowsInfo.SaveHeaderRowStyle(m, GetStyleObject(sheet, baseRow + m, -1, SheetArea.CornerHeader | SheetArea.RowHeader));
                    }
                }
                if ((option & CopyToOption.RangeGroup) > ((CopyToOption)0))
                {
                    RangeGroup rowRangeGroup = sheet.RowRangeGroup;
                    if ((rowRangeGroup != null) && !rowRangeGroup.IsEmpty())
                    {
                        for (int n = 0; n < rowCount; n++)
                        {
                            rowsInfo.SaveRangeGroup(n, rowRangeGroup.Data.GetLevel(baseRow + n), rowRangeGroup.Data.GetCollapsed(baseRow + n));
                        }
                    }
                }
            }
        }

        public static void SaveSheetInfo(Worksheet sheet, CopyMoveSheetInfo sheetInfo, CopyToOption option)
        {
            if ((option & CopyToOption.Style) > ((CopyToOption)0))
            {
                sheetInfo.SaveDefaultStyle(sheet.DefaultStyle);
                sheetInfo.SaveColumnHeaderDefaultStyle(sheet.ColumnHeader.DefaultStyle);
                sheetInfo.SaveRowHeaderDefaultStyle(sheet.RowHeader.DefaultStyle);
            }
            sheetInfo.SaveDefaultColumnWidth(sheet.DefaultColumnWidth);
            sheetInfo.SaveDefaultRowHeight(sheet.DefaultRowHeight);
            sheetInfo.SaveColumnHeaderDefaultRowHeight(sheet.ColumnHeader.DefaultRowHeight);
            sheetInfo.SaveRowHeaderDefaultColumnWidth(sheet.RowHeader.DefaultColumnWidth);
        }

        public static void SaveViewportInfo(Worksheet sheet, CopyMoveCellsInfo cellsInfo, int baseRow, int baseColumn, CopyToOption option)
        {
            if ((option & CopyToOption.All) > ((CopyToOption)0))
            {
                int rowCount = cellsInfo.RowCount;
                int columnCount = cellsInfo.ColumnCount;
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if ((option & CopyToOption.Value) > ((CopyToOption)0))
                        {
                            cellsInfo.SaveValue(i, j, sheet.GetValue(baseRow + i, baseColumn + j, SheetArea.Cells));
                        }
                        if (((option & CopyToOption.Value) > ((CopyToOption)0)) || ((option & CopyToOption.Formula) > ((CopyToOption)0)))
                        {
                            cellsInfo.SaveFormula(i, j, sheet.GetFormula(baseRow + i, baseColumn + j));
                            object[,] arrayFormulas = Excel.GetsArrayFormulas(sheet, baseRow, baseColumn, rowCount, columnCount);
                            cellsInfo.SaveArrayFormula(arrayFormulas);
                        }
                        if ((option & CopyToOption.Sparkline) > ((CopyToOption)0))
                        {
                            Sparkline sparkline = sheet.GetSparkline(baseRow + i, baseColumn + j);
                            CellRange sparklineDataRange = sheet.Cells[baseRow + i, baseColumn + j].SparklineDataRange;
                            CellRange sparklineDateAxisRange = sheet.Cells[baseRow + i, baseColumn + j].SparklineDateAxisRange;
                            if ((sparkline != null) && (sparklineDataRange != null))
                            {
                                cellsInfo.SaveSparkline(i, j, new SparklineInfo(sparkline, sparklineDataRange, sparklineDateAxisRange));
                            }
                            else
                            {
                                cellsInfo.SaveSparkline(i, j, null);
                            }
                        }
                        if ((option & CopyToOption.Style) > ((CopyToOption)0))
                        {
                            cellsInfo.SaveStyle(i, j, GetStyleObject(sheet, baseRow + i, baseColumn + j, SheetArea.Cells));
                        }
                        if ((option & CopyToOption.Tag) > ((CopyToOption)0))
                        {
                            cellsInfo.SaveTag(i, j, sheet.GetTag(baseRow + i, baseColumn + j, SheetArea.Cells));
                        }
                    }
                }
                if ((option & CopyToOption.Span) > ((CopyToOption)0))
                {
                    IEnumerator enumerator = sheet.SpanModel.GetEnumerator(baseRow, baseColumn, rowCount, columnCount);
                    while (enumerator.MoveNext())
                    {
                        cellsInfo.SaveSpan((CellRange)enumerator.Current);
                    }
                }
            }
        }

        public static void SetStyleObject(Worksheet sheet, int row, int column, SheetArea area, object style)
        {
            if (style is string)
            {
                sheet.SetStyleName(row, column, area, (string)((string)style));
            }
            else
            {
                sheet.SetStyleInfo(row, column, area, style as StyleInfo);
            }
        }

        public static void UndoCellsInfo(Worksheet sheet, CopyMoveCellsInfo cellsInfo, int baseRow, int baseColumn, SheetArea area)
        {
            int rowCount = cellsInfo.RowCount;
            int columnCount = cellsInfo.ColumnCount;
            sheet.SuspendCalcService();
            try
            {
                for (int i = 0; i < rowCount; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        if (cellsInfo.IsFormulaSaved() && (area == SheetArea.Cells))
                        {
                            sheet.SetFormula(baseRow + i, baseColumn + j, null);
                        }
                        if (cellsInfo.IsSparklineSaved() && (area == SheetArea.Cells))
                        {
                            sheet.RemoveSparkline(baseRow + i, baseColumn + j);
                        }
                        if (cellsInfo.IsValueSaved())
                        {
                            sheet.SetValue(baseRow + i, baseColumn + j, area, null);
                        }
                        if (cellsInfo.IsStyleSaved())
                        {
                            SetStyleObject(sheet, baseRow + i, baseColumn + j, area, null);
                        }
                        if (cellsInfo.IsTagSaved())
                        {
                            sheet.SetTag(baseRow + i, baseColumn + j, area, null);
                        }
                    }
                }
                if (cellsInfo.IsFormulaSaved() && (area == SheetArea.Cells))
                {
                    foreach (CellData data in cellsInfo.GetFormulas())
                    {
                        sheet.SetFormula(baseRow + data.Row, baseColumn + data.Column, (string)(data.Value as string));
                    }
                }
                if (cellsInfo.IsSparklineSaved() && (area == SheetArea.Cells))
                {
                    foreach (CellData data2 in cellsInfo.GetSparklines())
                    {
                        SparklineInfo info = data2.Value as SparklineInfo;
                        if (info == null)
                        {
                            ((ISparklineSheet)sheet).SetSparkline(baseRow + data2.Row, baseColumn + data2.Column, null);
                        }
                        else if (info.DataAxisRange == null)
                        {
                            sheet.SetSparkline(baseRow + data2.Row, baseColumn + data2.Column, info.DataRange, info.Sparkline.DataOrientation, info.Sparkline.SparklineType, info.Sparkline.Setting);
                        }
                        else
                        {
                            sheet.SetSparkline(baseRow + data2.Row, baseColumn + data2.Column, info.DataRange, info.Sparkline.DataOrientation, info.Sparkline.SparklineType, info.DataAxisRange, info.Sparkline.DateAxisOrientation, info.Sparkline.Setting);
                        }
                    }
                }
                if (cellsInfo.IsValueSaved())
                {
                    foreach (CellData data3 in cellsInfo.GetValues())
                    {
                        sheet.SetValue(baseRow + data3.Row, baseColumn + data3.Column, area, data3.Value);
                    }
                }
                if (cellsInfo.IsStyleSaved())
                {
                    foreach (CellData data4 in cellsInfo.GetStyles())
                    {
                        SetStyleObject(sheet, baseRow + data4.Row, baseColumn + data4.Column, area, data4.Value);
                    }
                }
                if (cellsInfo.IsTagSaved())
                {
                    foreach (CellData data5 in cellsInfo.GetTags())
                    {
                        sheet.SetTag(baseRow + data5.Row, baseColumn + data5.Column, area, data5.Value);
                    }
                }
                if (cellsInfo.IsArrayFormulaSaved() && (area == SheetArea.Cells))
                {
                    object[,] arrayFormula = Excel.GetsArrayFormulas(sheet, baseRow, baseColumn, rowCount, columnCount);
                    if ((arrayFormula != null) && (arrayFormula.Length > 0))
                    {
                        int length = arrayFormula.GetLength(0);
                        for (int k = 0; k < length; k++)
                        {
                            CellRange range = (CellRange)arrayFormula[k, 0];
                            sheet.SetArrayFormula(range.Row, range.Column, range.RowCount, range.ColumnCount, null);
                        }
                    }
                    arrayFormula = cellsInfo.GetArrayFormula();
                    if ((arrayFormula != null) && (arrayFormula.Length > 0))
                    {
                        int num7 = arrayFormula.GetLength(0);
                        for (int m = 0; m < num7; m++)
                        {
                            CellRange range2 = (CellRange)arrayFormula[m, 0];
                            string formula = (string)((string)arrayFormula[m, 1]);
                            if (formula.StartsWith("{") && formula.EndsWith("}"))
                            {
                                formula = formula.Substring(1, formula.Length - 2);
                            }
                            sheet.SetArrayFormula(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount, formula);
                        }
                    }
                }
            }
            finally
            {
                sheet.ResumeCalcService();
            }
            SheetSpanModel spanModel = null;
            if (area == SheetArea.Cells)
            {
                spanModel = sheet.SpanModel;
            }
            else if (area == SheetArea.ColumnHeader)
            {
                spanModel = sheet.ColumnHeaderSpanModel;
            }
            else if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                spanModel = sheet.RowHeaderSpanModel;
            }
            if ((spanModel != null) && !spanModel.IsEmpty())
            {
                List<CellRange> list6 = new List<CellRange>();
                IEnumerator enumerator = spanModel.GetEnumerator(baseRow, baseColumn, rowCount, columnCount);
                while (enumerator.MoveNext())
                {
                    CellRange current = enumerator.Current as CellRange;
                    if (current != null)
                    {
                        list6.Add(current);
                    }
                }
                foreach (CellRange range4 in list6)
                {
                    spanModel.Remove(range4.Row, range4.Column);
                }
            }
            if (cellsInfo.IsSpanSaved() && (spanModel != null))
            {
                foreach (CellRange range5 in cellsInfo.Spans)
                {
                    spanModel.Add(range5.Row, range5.Column, range5.RowCount, range5.ColumnCount);
                }
            }
        }

        public static void UndoColumnsInfo(Worksheet sheet, CopyMoveColumnsInfo columnsInfo, int baseColumn)
        {
            int columnCount = columnsInfo.ColumnCount;
            if (columnsInfo.IsBindingFieldSaved())
            {
                for (int i = 0; i < columnCount; i++)
                {
                    string str;
                    if (columnsInfo.GetBindingField(i, out str))
                    {
                        sheet.BindDataColumn(baseColumn + i, str);
                    }
                }
            }
            else
            {
                for (int j = 0; j < columnCount; j++)
                {
                    sheet.BindDataColumn(baseColumn + j, null);
                }
            }
            if (columnsInfo.IsWidthSaved())
            {
                for (int k = 0; k < columnCount; k++)
                {
                    sheet.SetColumnWidth(baseColumn + k, SheetArea.Cells, columnsInfo.GetWidth(k));
                }
            }
            if (columnsInfo.IsVisibleSaved())
            {
                for (int m = 0; m < columnCount; m++)
                {
                    sheet.SetColumnVisible(baseColumn + m, SheetArea.Cells, columnsInfo.GetVisible(m));
                }
            }
            if (columnsInfo.IsResizableSaved())
            {
                for (int n = 0; n < columnCount; n++)
                {
                    sheet.SetColumnResizable(baseColumn + n, SheetArea.Cells, columnsInfo.GetResizable(n));
                }
            }
            if (columnsInfo.IsTagSaved())
            {
                for (int num7 = 0; num7 < columnCount; num7++)
                {
                    sheet.SetTag(-1, baseColumn + num7, SheetArea.Cells, columnsInfo.GetTag(num7));
                }
            }
            if (columnsInfo.IsViewportColumnStyleSaved())
            {
                for (int num8 = 0; num8 < columnCount; num8++)
                {
                    SetStyleObject(sheet, -1, baseColumn + num8, SheetArea.Cells, columnsInfo.GetViewportColumnStyle(num8));
                }
            }
            if (columnsInfo.IsHeaderColumnStyleSaved())
            {
                for (int num9 = 0; num9 < columnCount; num9++)
                {
                    SetStyleObject(sheet, -1, baseColumn + num9, SheetArea.ColumnHeader, columnsInfo.GetHeaderColumnStyle(num9));
                }
            }
            if (columnsInfo.IsRangeGroupSaved())
            {
                RangeGroup columnRangeGroup = sheet.ColumnRangeGroup;
                if (columnRangeGroup != null)
                {
                    for (int num11 = 0; num11 < columnCount; num11++)
                    {
                        int num10;
                        bool flag;
                        columnsInfo.GetRangeGroup(num11, out num10, out flag);
                        columnRangeGroup.Data.SetLevel(baseColumn + num11, num10);
                        columnRangeGroup.Data.SetCollapsed(baseColumn + num11, flag);
                    }
                }
            }
        }

        public static void UndoFloatingObjectsInfo(Worksheet sheet, CopyMoveFloatingObjectsInfo floatingObjectsInfo)
        {
            sheet.Workbook.SuspendEvent();
            try
            {
                foreach (FloatingObject obj2 in GetFloatingObjectsInRange(floatingObjectsInfo.Range, sheet))
                {
                    if (obj2 is SpreadChart)
                    {
                        sheet.Charts.Remove(obj2 as SpreadChart);
                    }
                    else if (obj2 is Picture)
                    {
                        sheet.Pictures.Remove(obj2 as Picture);
                    }
                    else
                    {
                        sheet.FloatingObjects.Remove(obj2);
                    }
                }
                foreach (FloatingObject obj3 in floatingObjectsInfo.SavedFloatingObjects)
                {
                    if (obj3 is SpreadChart)
                    {
                        sheet.Charts.Add(obj3 as SpreadChart);
                    }
                    else if (obj3 is Picture)
                    {
                        sheet.Pictures.Add(obj3 as Picture);
                    }
                    else
                    {
                        sheet.FloatingObjects.Add(obj3);
                    }
                }
            }
            finally
            {
                sheet.Workbook.ResumeEvent();
            }
        }

        public static void UndoRowsInfo(Worksheet sheet, CopyMoveRowsInfo rowsInfo, int baseRow)
        {
            int rowCount = rowsInfo.RowCount;
            if (rowsInfo.IsHeightSaved())
            {
                for (int i = 0; i < rowCount; i++)
                {
                    sheet.SetRowHeight(baseRow + i, SheetArea.Cells, rowsInfo.GetHeight(i));
                }
            }
            if (rowsInfo.IsVisibleSaved())
            {
                for (int j = 0; j < rowCount; j++)
                {
                    sheet.SetRowVisible(baseRow + j, SheetArea.Cells, rowsInfo.GetVisible(j));
                }
            }
            if (rowsInfo.IsResizableSaved())
            {
                for (int k = 0; k < rowCount; k++)
                {
                    sheet.SetRowResizable(baseRow + k, SheetArea.Cells, rowsInfo.GetResizable(k));
                }
            }
            if (rowsInfo.IsTagSaved())
            {
                for (int m = 0; m < rowCount; m++)
                {
                    sheet.SetTag(baseRow + m, -1, SheetArea.Cells, rowsInfo.GetTag(m));
                }
            }
            if (rowsInfo.IsViewportRowStyleSaved())
            {
                for (int n = 0; n < rowCount; n++)
                {
                    SetStyleObject(sheet, baseRow + n, -1, SheetArea.Cells, rowsInfo.GetViewportRowStyle(n));
                }
            }
            if (rowsInfo.IsHeaderRowStyleSaved())
            {
                for (int num7 = 0; num7 < rowCount; num7++)
                {
                    SetStyleObject(sheet, baseRow + num7, -1, SheetArea.CornerHeader | SheetArea.RowHeader, rowsInfo.GetHeaderRowStyle(num7));
                }
            }
            if (rowsInfo.IsRangeGroupSaved())
            {
                RangeGroup rowRangeGroup = sheet.RowRangeGroup;
                if (rowRangeGroup != null)
                {
                    for (int num9 = 0; num9 < rowCount; num9++)
                    {
                        int num8;
                        bool flag;
                        rowsInfo.GetRangeGroup(num9, out num8, out flag);
                        rowRangeGroup.Data.SetLevel(baseRow + num9, num8);
                        rowRangeGroup.Data.SetCollapsed(baseRow + num9, flag);
                    }
                }
            }
        }

        public static void UndoSheetInfo(Worksheet sheet, CopyMoveSheetInfo sheetInfo)
        {
            if (sheetInfo.IsDefaultStyleSaved())
            {
                sheet.DefaultStyle = sheetInfo.GetDefaultStyle();
            }
            if (sheetInfo.IsDefaultColumnWidthSaved())
            {
                sheet.DefaultColumnWidth = sheetInfo.GetDefaultColumnWidth();
            }
            if (sheetInfo.IsDefaultRowHeightSaved())
            {
                sheet.DefaultRowHeight = sheetInfo.GetDefaultRowHeight();
            }
            if (sheetInfo.IsColumnHeaderDefaultStyleSaved())
            {
                sheet.ColumnHeader.DefaultStyle = sheetInfo.GetColumnHeaderDefaultStyle();
            }
            if (sheetInfo.IsColumnHeaderDefaultRowHeightSaved())
            {
                sheet.ColumnHeader.DefaultRowHeight = sheetInfo.GetColumnHeaderDefaultRowHeight();
            }
            if (sheetInfo.IsRowHeaderDefaultStyleSaved())
            {
                sheet.RowHeader.DefaultStyle = sheetInfo.GetRowHeaderDefaultStyle();
            }
            if (sheetInfo.IsRowHeaderDefaultColumnWidthSaved())
            {
                sheet.RowHeader.DefaultColumnWidth = sheetInfo.GetRowHeaderDefaultColumnWidth();
            }
        }
    }
}

