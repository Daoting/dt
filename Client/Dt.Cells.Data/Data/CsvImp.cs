#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the serializer object for opening from and saving to a file 
    /// the entire Spread component and all its data.
    /// </summary>
    internal static class CsvImp
    {
        /// <summary>
        /// Gets the cell data.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="opt">The opt</param>
        /// <returns></returns>
        static object GetCellData(Worksheet worksheet, SheetArea area, int rowIndex, int columnIndex, ImportExportOptions opt)
        {
            if (opt.Formula && (area == SheetArea.Cells))
            {
                string formula = worksheet.GetFormula(rowIndex, columnIndex);
                if (!string.IsNullOrEmpty(formula))
                {
                    return string.Format("={0}", (object[]) new object[] { formula });
                }
            }
            if (!opt.UnFormatted)
            {
                IFormatter formatter = worksheet.GetActualFormatter(rowIndex, columnIndex, area);
                if (formatter != null)
                {
                    return worksheet.Value2Text(worksheet.GetValue(rowIndex, columnIndex, area), formatter);
                }
            }
            object columnLabel = worksheet.GetValue(rowIndex, columnIndex, area);
            if ((columnLabel == null) && (area == SheetArea.ColumnHeader))
            {
                columnLabel = worksheet.GetColumnLabel(rowIndex, columnIndex);
            }
            if ((columnLabel == null) && (area == (SheetArea.CornerHeader | SheetArea.RowHeader)))
            {
                columnLabel = worksheet.GetRowLabel(rowIndex, columnIndex);
            }
            return columnLabel;
        }

        /// <summary>
        /// Gets the length of the max.
        /// </summary>
        /// <param name="data">The data</param>
        /// <returns></returns>
        internal static int GetMaxLength(List<List<string>> data)
        {
            if (data == null)
            {
                return 0;
            }
            int num = 0;
            using (List<List<string>>.Enumerator enumerator = data.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    num = Math.Max(enumerator.Current.Count, num);
                }
            }
            return num;
        }

        /// <summary>
        /// Gets the part data.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="rowStartIndex">Start index of the row</param>
        /// <param name="rowEndIndex">End index of the row</param>
        /// <param name="columnStartIndex">Start index of the column</param>
        /// <param name="columnEndIndex">End index of the column</param>
        /// <param name="area">The area</param>
        /// <param name="opt">The opt</param>
        /// <returns></returns>
        static List<List<object>> GetPartData(Worksheet worksheet, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, SheetArea area, ImportExportOptions opt)
        {
            if (!opt.IncludeSheetArea(area))
            {
                return null;
            }
            List<List<object>> list = new List<List<object>>();
            bool actualVisible = true;
            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                if (area == SheetArea.ColumnHeader)
                {
                    actualVisible = worksheet.ColumnHeader.Rows[i].ActualVisible;
                }
                else
                {
                    actualVisible = worksheet.Rows[i].ActualVisible;
                }
                if (actualVisible || !opt.AsViewed)
                {
                    List<object> list2 = new List<object>();
                    if (opt.RowHeader && (area == SheetArea.Cells))
                    {
                        for (int k = 0; k < worksheet.GetColumnCount(SheetArea.CornerHeader | SheetArea.RowHeader); k++)
                        {
                            if (worksheet.GetActualColumnVisible(k, SheetArea.CornerHeader | SheetArea.RowHeader) || !opt.AsViewed)
                            {
                                list2.Add(GetCellData(worksheet, SheetArea.CornerHeader | SheetArea.RowHeader, i, k, opt));
                            }
                        }
                    }
                    for (int j = columnStartIndex; j <= columnEndIndex; j++)
                    {
                        if (worksheet.GetActualColumnVisible(j, SheetArea.Cells) || !opt.AsViewed)
                        {
                            list2.Add(GetCellData(worksheet, area, i, j, opt));
                        }
                    }
                    list.Add(list2);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the range text.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="row">The row index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="column">The column index.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="rowDelimiter">The row delimiter.</param>
        /// <param name="columnDelimiter">The column delimiter.</param>
        /// <param name="cellDelimiter">The cell delimiter.</param>
        /// <param name="forceCellDelimiter">If set to <c>true</c>, [force cell delimiter]</param>
        /// <param name="flags">The export flags.</param>
        /// <returns></returns>
        public static string GetRangeText(Worksheet worksheet, int row, int rowCount, int column, int columnCount, string rowDelimiter, string columnDelimiter, string cellDelimiter, bool forceCellDelimiter, TextFileSaveFlags flags)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            if ((row < -1) || (row >= worksheet.RowCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvaildRowIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) row), ((int) (worksheet.RowCount - 1)) }));
            }
            if ((rowCount < -1) || ((row + rowCount) > worksheet.RowCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidRowCountWithAllowedRowCountBehind, (object[]) new object[] { ((int) rowCount), ((int) (worksheet.RowCount - 1)) }));
            }
            if ((column < -1) || (column >= worksheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) column), ((int) (worksheet.ColumnCount - 1)) }));
            }
            if ((columnCount < -1) || ((column + columnCount) > worksheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnCountWithAllowedColumnCountBehind, (object[]) new object[] { ((int) columnCount), ((int) (worksheet.ColumnCount - columnCount)) }));
            }
            int rowEndIndex = -1;
            int columnEndIndex = -1;
            if (((row == -1) && (column == -1)) && ((rowCount == -1) && (columnCount == -1)))
            {
                row = 0;
                column = 0;
                rowEndIndex = worksheet.GetLastDirtyRow(StorageType.Data);
                columnEndIndex = worksheet.GetLastDirtyColumn(StorageType.Data);
            }
            else
            {
                if (row == -1)
                {
                    row = 0;
                }
                if (column == -1)
                {
                    column = 0;
                }
                if (rowCount == -1)
                {
                    rowCount = worksheet.RowCount - row;
                }
                if (columnCount == -1)
                {
                    columnCount = worksheet.ColumnCount - column;
                }
                rowEndIndex = (row + rowCount) - 1;
                columnEndIndex = (column + columnCount) - 1;
            }
            if (string.IsNullOrEmpty(rowDelimiter))
            {
                rowDelimiter = "\r\n";
            }
            if (string.IsNullOrEmpty(columnDelimiter))
            {
                columnDelimiter = "\t";
            }
            if (string.IsNullOrEmpty(cellDelimiter))
            {
                cellDelimiter = "\"";
            }
            List<List<object>> list = GetSheetData(worksheet, row, rowEndIndex, column, columnEndIndex, flags);
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                List<object> list2 = list[i];
                bool flag = true;
                foreach (object obj2 in list2)
                {
                    if (!flag)
                    {
                        builder.Append(columnDelimiter);
                    }
                    flag = false;
                    string str = string.Empty;
                    if (obj2 != null)
                    {
                        str = obj2.ToString().Replace(cellDelimiter, cellDelimiter + cellDelimiter);
                    }
                    if ((forceCellDelimiter || (str.IndexOf(cellDelimiter) != -1)) || ((str.IndexOf(columnDelimiter) != -1) || (str.IndexOf(rowDelimiter) != -1)))
                    {
                        builder.AppendFormat("{0}{1}{0}", (object[]) new object[] { cellDelimiter, str });
                    }
                    else
                    {
                        builder.Append(str);
                    }
                }
                if (i != (list.Count - 1))
                {
                    builder.Append(rowDelimiter);
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Gets the sheet data.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="flags">The save flags.</param>
        /// <returns></returns>
        public static List<List<object>> GetSheetData(Worksheet worksheet, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, TextFileSaveFlags flags)
        {
            List<List<object>> list = new List<List<object>>();
            ImportExportOptions opt = new ImportExportOptions(flags);
            opt.FixOptions(worksheet);
            if (opt.ColumnHeader)
            {
                list.AddRange((IEnumerable<List<object>>) GetPartData(worksheet, 0, worksheet.GetRowCount(SheetArea.ColumnHeader) - 1, columnStartIndex, columnEndIndex, SheetArea.ColumnHeader, opt));
            }
            list.AddRange((IEnumerable<List<object>>) GetPartData(worksheet, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, SheetArea.Cells, opt));
            return list;
        }

        public static List<List<string>> ParseText(string data, string rowDelimiter, string columnDelimiter, string cellDelimiter)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            if (string.IsNullOrEmpty(rowDelimiter))
            {
                rowDelimiter = "\r\n";
            }
            if (string.IsNullOrEmpty(columnDelimiter))
            {
                columnDelimiter = "\t";
            }
            if (string.IsNullOrEmpty(cellDelimiter))
            {
                cellDelimiter = "\"";
            }
            if (!data.EndsWith(rowDelimiter))
            {
                data = data + rowDelimiter;
            }
            List<List<string>> list = new List<List<string>>();
            List<string> list2 = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            int length = cellDelimiter.Length;
            int num2 = rowDelimiter.Length;
            int num3 = columnDelimiter.Length;
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i]);
                if ((builder.Length >= length) && cellDelimiter.Equals(builder.ToString(builder.Length - length, length)))
                {
                    if ((flag && (data.Length >= ((i + 1) + length))) && cellDelimiter.Equals(data.Substring(i + 1, length)))
                    {
                        i += length;
                    }
                    else if (!flag)
                    {
                        if (builder.ToString().IndexOf(cellDelimiter) == 0)
                        {
                            builder.Remove(builder.Length - length, length);
                            flag = !flag;
                        }
                    }
                    else
                    {
                        builder.Remove(builder.Length - length, length);
                        flag = !flag;
                    }
                }
                else if ((!flag && (builder.Length >= num3)) && columnDelimiter.Equals(builder.ToString(builder.Length - num3, num3)))
                {
                    builder.Remove(builder.Length - num3, num3);
                    list2.Add(builder.ToString());
                    builder.Remove(0, builder.Length);
                }
                else if ((!flag && (builder.Length >= num2)) && rowDelimiter.Equals(builder.ToString(builder.Length - num2, num2)))
                {
                    builder.Remove(builder.Length - num2, num2);
                    list2.Add(builder.ToString());
                    list.Add(list2);
                    list2 = new List<string>();
                    builder.Remove(0, builder.Length);
                }
                else if ((flag && (data.Length >= ((i + 1) + num3))) && (columnDelimiter.Equals(data.Substring(i + 1, num3)) && (columnDelimiter != CultureInfo.InvariantCulture.TextInfo.ListSeparator)))
                {
                    i += length;
                }
            }
            if (flag)
            {
                if ((list2.Count > 0) && !list.Contains(list2))
                {
                    list.Add(list2);
                }
                string str2 = builder.ToString();
                if (!string.IsNullOrEmpty(str2))
                {
                    str2 = str2.Replace(columnDelimiter, "");
                    if (str2.EndsWith(rowDelimiter))
                    {
                        str2 = str2.Substring(0, str2.Length - rowDelimiter.Length);
                    }
                    if (list.Count >= 1)
                    {
                        list[list.Count - 1].Add(str2);
                    }
                    else
                    {
                        list.Add(new List<string>(new string[] { str2 }));
                    }
                }
            }
            if ((list.Count == 0) && !string.IsNullOrWhiteSpace(data))
            {
                string str3 = data;
                if (data.EndsWith(rowDelimiter))
                {
                    str3 = str3.Substring(0, str3.Length - rowDelimiter.Length);
                }
                list.Add(new List<string>(new string[] { str3 }));
            }
            return list;
        }

        /// <summary>
        /// Sets the cell data.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="area">The area</param>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="value">The value</param>
        /// <param name="opt">The opt</param>
        static void SetCellData(Worksheet worksheet, SheetArea area, int rowIndex, int columnIndex, string value, ImportExportOptions opt)
        {
            object obj2 = value;
            GeneralFormatter gformatter = null;
            if (!opt.UnFormatted)
            {
                gformatter = new GeneralFormatter().GetPreferredDisplayFormatter(value, out obj2) as GeneralFormatter;
            }
            if (obj2 == null)
            {
                worksheet.SetValue(rowIndex, columnIndex, area, obj2);
            }
            else if (!object.Equals(value, ""))
            {
                if (!opt.Formula || !value.StartsWith("="))
                {
                    StyleInfo info = worksheet.GetActualStyleInfo(rowIndex, columnIndex, area);
                    if (info != null)
                    {
                        if (!opt.UnFormatted)
                        {
                            if (info.Formatter == null)
                            {
                                SetFormatter(worksheet, rowIndex, columnIndex, area, gformatter);
                            }
                            else if (info.Formatter.FormatString == "@")
                            {
                                obj2 = value.ToString();
                            }
                        }
                        else if (info.Formatter != null)
                        {
                            if (area == SheetArea.Cells)
                            {
                                worksheet.Cells[rowIndex, columnIndex].ResetFormatter();
                            }
                            if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
                            {
                                worksheet.RowHeader.Cells[rowIndex, columnIndex].ResetFormatter();
                            }
                            if (area == SheetArea.ColumnHeader)
                            {
                                worksheet.ColumnHeader.Cells[rowIndex, columnIndex].ResetFormatter();
                            }
                        }
                    }
                    worksheet.SetValue(rowIndex, columnIndex, area, obj2);
                }
                else
                {
                    try
                    {
                        worksheet.SetFormula(rowIndex, columnIndex, area, value.Substring(1));
                    }
                    catch
                    {
                        worksheet.SetText(rowIndex, columnIndex, area, value);
                    }
                }
            }
            else
            {
                worksheet.SetValue(rowIndex, columnIndex, area, null);
            }
        }

        public static void SetFormatter(Worksheet worksheet, int row, int column, SheetArea area, GeneralFormatter gformatter)
        {
            if (area == SheetArea.Cells)
            {
                worksheet.Cells[row, column].Formatter = new AutoFormatter(gformatter);
            }
            if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                worksheet.RowHeader.Cells[row, column].Formatter = new AutoFormatter(gformatter);
            }
            if (area == SheetArea.ColumnHeader)
            {
                worksheet.ColumnHeader.Cells[row, column].Formatter = new AutoFormatter(gformatter);
            }
        }

        /// <summary>
        /// Sets the range text.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="data">The data.</param>
        /// <param name="rowDelimiter">The row delimiter.</param>
        /// <param name="columnDelimiter">The column delimiter.</param>
        /// <param name="cellDelimiter">The cell delimiter.</param>
        /// <param name="flags">The load flags.</param>
        public static void SetRangeText(Worksheet worksheet, int row, int column, string data, string rowDelimiter, string columnDelimiter, string cellDelimiter, TextFileOpenFlags flags)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            if ((row < -1) || (row >= worksheet.RowCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvaildRowIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) row), ((int) (worksheet.RowCount - 1)) }));
            }
            if ((column < -1) || (column >= worksheet.ColumnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) column), ((int) (worksheet.ColumnCount - 1)) }));
            }
            if (!string.IsNullOrEmpty(data))
            {
                if (row == -1)
                {
                    row = 0;
                }
                if (column == -1)
                {
                    column = 0;
                }
                List<List<string>> list = ParseText(data, rowDelimiter, columnDelimiter, cellDelimiter);
                if ((list != null) && (list.Count > 0))
                {
                    SetSheetData(worksheet, row, column, list, flags);
                }
            }
        }

        /// <summary>
        /// Sets the row data.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="rowData">The row data</param>
        /// <param name="sheetRowIndex">Index of the sheet row</param>
        /// <param name="columnIndex">Index of the column</param>
        /// <param name="columnCount">The column count</param>
        /// <param name="area">The area</param>
        /// <param name="opt">The opt</param>
        static void SetRowData(Worksheet worksheet, List<string> rowData, int sheetRowIndex, int columnIndex, int columnCount, SheetArea area, ImportExportOptions opt)
        {
            int num = 0;
            for (int i = columnIndex; num < rowData.Count; i++)
            {
                if ((columnCount > 0) && (i < worksheet.GetColumnCount(area)))
                {
                    SetCellData(worksheet, area, sheetRowIndex, i, rowData[num], opt);
                }
                num++;
            }
        }

        /// <summary>
        /// Sets the sheet data.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="rowIndex">The row index.</param>
        /// <param name="columnIndex">The column index.</param>
        /// <param name="data">The data.</param>
        /// <param name="flags">The import flags.</param>
        public static void SetSheetData(Worksheet worksheet, int rowIndex, int columnIndex, List<List<string>> data, TextFileOpenFlags flags)
        {
            int num = data.Count;
            int maxLength = GetMaxLength(data);
            if ((num != 0) && (maxLength != 0))
            {
                ImportExportOptions opt = new ImportExportOptions(flags);
                opt.FixOptions(worksheet);
                int columnCount = opt.RowHeader ? worksheet.RowHeaderColumnCount : 0;
                int num4 = opt.ColumnHeader ? worksheet.ColumnHeaderRowCount : 0;
                int num5 = 0;
                maxLength -= columnCount;
                if (maxLength <= 0)
                {
                    maxLength = 0;
                }
                num -= num4;
                if (num <= 0)
                {
                    num5 = 0;
                }
                num -= num5;
                if (num <= 0)
                {
                    num = 0;
                }
                if (opt.ExpandRows && ((rowIndex + num) > worksheet.RowCount))
                {
                    worksheet.RowCount = rowIndex + num;
                }
                if (opt.ExpandColumns && ((columnIndex + maxLength) > worksheet.ColumnCount))
                {
                    worksheet.ColumnCount = columnIndex + maxLength;
                }
                int num6 = 0;
                for (int i = 0; num6 < data.Count; i++)
                {
                    List<string> rowData = data[num6];
                    if (rowData.Count > 0)
                    {
                        if ((num4 > 0) && (num6 < num4))
                        {
                            SetRowData(worksheet, rowData, i, columnIndex, maxLength, SheetArea.ColumnHeader, opt);
                        }
                        else if ((num > 0) && (i < worksheet.GetRowCount(SheetArea.Cells)))
                        {
                            if (num6 == num4)
                            {
                                i = rowIndex;
                            }
                            SetRowData(worksheet, rowData, i, 0, columnCount, SheetArea.CornerHeader | SheetArea.RowHeader, opt);
                            rowData.RemoveRange(0, columnCount);
                            SetRowData(worksheet, rowData, i, columnIndex, maxLength, SheetArea.Cells, opt);
                        }
                    }
                    num6++;
                }
            }
        }

        internal class ImportExportOptions
        {
            bool asViewed;
            bool columnHeader;
            bool expandColumns;
            bool expandRows;
            bool formula;
            bool rowHeader;
            bool unFormatted;

            public ImportExportOptions(TextFileOpenFlags flags)
            {
                this.rowHeader = (flags & TextFileOpenFlags.IncludeRowHeader) == TextFileOpenFlags.IncludeRowHeader;
                this.columnHeader = (flags & TextFileOpenFlags.IncludeColumnHeader) == TextFileOpenFlags.IncludeColumnHeader;
                this.unFormatted = (flags & TextFileOpenFlags.UnFormatted) == TextFileOpenFlags.UnFormatted;
                this.formula = (flags & TextFileOpenFlags.ImportFormula) == TextFileOpenFlags.ImportFormula;
                this.expandRows = true;
                this.expandColumns = true;
            }

            public ImportExportOptions(TextFileSaveFlags flags)
            {
                this.rowHeader = (flags & TextFileSaveFlags.IncludeRowHeader) == TextFileSaveFlags.IncludeRowHeader;
                this.columnHeader = (flags & TextFileSaveFlags.IncludeColumnHeader) == TextFileSaveFlags.IncludeColumnHeader;
                this.unFormatted = (flags & TextFileSaveFlags.UnFormatted) == TextFileSaveFlags.UnFormatted;
                this.formula = (flags & TextFileSaveFlags.ExportFormula) == TextFileSaveFlags.ExportFormula;
                this.asViewed = (flags & TextFileSaveFlags.AsViewed) == TextFileSaveFlags.AsViewed;
            }

            public void FixOptions(Worksheet worksheet)
            {
                if (worksheet != null)
                {
                    if (worksheet.RowHeaderColumnCount <= 0)
                    {
                        this.rowHeader = false;
                    }
                    if (worksheet.ColumnHeaderRowCount <= 0)
                    {
                        this.columnHeader = false;
                    }
                }
            }

            public bool IncludeSheetArea(SheetArea area)
            {
                switch (area)
                {
                    case SheetArea.Cells:
                        return true;

                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        return this.RowHeader;

                    case SheetArea.ColumnHeader:
                        return this.ColumnHeader;
                }
                throw new ArgumentOutOfRangeException("area");
            }

            public bool AsViewed
            {
                get { return  this.asViewed; }
            }

            public bool ColumnHeader
            {
                get { return  this.columnHeader; }
            }

            public bool ExpandColumns
            {
                get { return  this.expandColumns; }
            }

            public bool ExpandRows
            {
                get { return  this.expandRows; }
            }

            public bool Formula
            {
                get { return  this.formula; }
            }

            public bool RowHeader
            {
                get { return  this.rowHeader; }
            }

            public bool UnFormatted
            {
                get { return  this.unFormatted; }
            }
        }
    }
}

