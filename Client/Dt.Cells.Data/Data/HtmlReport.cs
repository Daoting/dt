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
using System.IO;
using System.Text;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents the HTML report. 
    /// </summary>
    internal static class HtmlReport
    {
        const string _bottomLeftCorner = "tfoot th.l";
        const string _bottomRightCorner = "tfoot th.r";
        const string _cells = "tbody td";
        const string _centerClass = "c";
        const string _classPrefix = ".";
        const string _columnFooter = "tfoot th.c";
        const string _columnHeader = "thead th.c";
        const string _defaultHeaderBKColor = "#9EB6CE";
        const string _defaultSheetName = "sdm";
        const string _idPrefix = "#";
        const string _leftClass = "l";
        const string _noData = "No Data";
        const string _rightClass = "r";
        const string _rowFooter = "tbody th.r";
        const string _rowHeader = "tbody th.l";
        const string _topLeftCorner = "thead th.l";
        const string _topRightCorner = "thead th.r";
        static Dictionary<ulong, StyleInfo> styleCellDictionary = new Dictionary<ulong, StyleInfo>();
        static Dictionary<ulong, StyleInfo> styleColumnHeaderDictionary = new Dictionary<ulong, StyleInfo>();
        static Dictionary<ulong, StyleInfo> styleRowHeaderDicitonary = new Dictionary<ulong, StyleInfo>();

        /// <summary>
        /// Adds to style ex.
        /// </summary>
        /// <param name="styleEx">The style ex.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="td">The td.</param>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        static string AddToStyleEx(StyleEx styleEx, int rowIndex, int columnIndex, TableCellEx td, SheetArea area)
        {
            string str = styleEx.FindEquals(td.Styles);
            if (string.IsNullOrEmpty(str))
            {
                str = string.Format(".{0}{1}r{2}c{3}", (object[]) new object[] { styleEx.SheetName, ((int) GetSheetAreaIndex(area)), ((int) rowIndex), ((int) columnIndex) });
                styleEx.AddStyle(str, td.Styles);
            }
            return str;
        }

        static string ConvertFontWeight(FontWeight fontweight)
        {
            return ((ushort) fontweight.Weight).ToString();
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="rowHeaders">if set to <c>true</c> [row headers].</param>
        /// <param name="columnHeaders">if set to <c>true</c> [column headers].</param>
        /// <param name="styleEx">The style ex.</param>
        /// <returns></returns>
        static TableEx CreateTable(Worksheet worksheet, int row, int column, int rowCount, int columnCount, bool rowHeaders, bool columnHeaders, StyleEx styleEx)
        {
            TableEx table = new TableEx();
            PartsLayoutData layouts = new PartsLayoutData();
            if (columnHeaders)
            {
                layouts.ColumnHeaderHeights = new PartLayoutData(HtmlUtilities.GetRowHeights(worksheet, SheetArea.ColumnHeader, 0, worksheet.ColumnHeaderRowCount));
            }
            layouts.RowHeights = new PartLayoutData(HtmlUtilities.GetRowHeights(worksheet, SheetArea.Cells, 0, worksheet.RowCount));
            if (rowHeaders)
            {
                layouts.RowHeaderWidths = new PartLayoutData(HtmlUtilities.GetColumnWidths(worksheet, SheetArea.CornerHeader | SheetArea.RowHeader, 0, worksheet.RowHeaderColumnCount));
            }
            layouts.ColumnWidths = new PartLayoutData(HtmlUtilities.GetColumnWidths(worksheet, SheetArea.Cells, 0, worksheet.ColumnCount));
            PartsSpanLayoutData spans = new PartsSpanLayoutData();
            int rowStartIndex = row;
            int rowEndIndex = (row + rowCount) - 1;
            int columnStartIndex = column;
            int columnEndIndex = (column + columnCount) - 1;
            if (columnHeaders)
            {
                spans.ColumnHeaderSpans = HtmlUtilities.GetSpanLayoutData(worksheet, SheetArea.ColumnHeader, 0, worksheet.ColumnHeaderRowCount - 1, columnStartIndex, columnEndIndex);
            }
            if (rowHeaders)
            {
                spans.RowHeaderSpans = HtmlUtilities.GetSpanLayoutData(worksheet, SheetArea.CornerHeader | SheetArea.RowHeader, rowStartIndex, rowEndIndex, 0, worksheet.RowHeaderColumnCount - 1);
            }
            spans.CellSpans = HtmlUtilities.GetSpanLayoutData(worksheet, SheetArea.Cells, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex);
            StorageType type = StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data;
            int lastDirtyRow = worksheet.GetLastDirtyRow(type);
            int lastDirtyColumn = worksheet.GetLastDirtyColumn(type);
            if (spans.CellSpans != null)
            {
                int num7 = -1;
                int num8 = -1;
                IEnumerator enumerator = spans.CellSpans.GetEnumerator(-1, -1, -1, -1);
                while (enumerator.MoveNext())
                {
                    CellRange current = (CellRange) enumerator.Current;
                    if (((current.Row + current.RowCount) - 1) > num7)
                    {
                        num7 = (current.Row + current.RowCount) - 1;
                    }
                    if (((current.Column + current.ColumnCount) - 1) > num8)
                    {
                        num8 = (current.Column + current.ColumnCount) - 1;
                    }
                }
                lastDirtyRow = (num7 > lastDirtyRow) ? num7 : lastDirtyRow;
                lastDirtyColumn = (num8 > lastDirtyColumn) ? num8 : lastDirtyColumn;
            }
            SetColGroup(table, worksheet, column, columnCount, rowHeaders, layouts, lastDirtyColumn);
            if (columnHeaders)
            {
                SetTablePart(table.Header, SheetArea.ColumnHeader, worksheet, 0, worksheet.ColumnHeaderRowCount - 1, column, (column + columnCount) - 1, rowHeaders, layouts, spans, styleEx, lastDirtyRow, lastDirtyColumn);
            }
            SetTablePart(table.Body, SheetArea.Cells, worksheet, row, (row + rowCount) - 1, column, (column + columnCount) - 1, rowHeaders, layouts, spans, styleEx, lastDirtyRow, lastDirtyColumn);
            return table;
        }

        static StyleInfo FindStyle(Worksheet worksheet, int r, int c, SheetArea area)
        {
            ulong num = (ulong) r;
            num = num << 0x20;
            num |= (uint)c;
            if (area == SheetArea.ColumnHeader)
            {
                if (styleColumnHeaderDictionary.ContainsKey(num))
                {
                    return styleColumnHeaderDictionary[num];
                }
                StyleInfo info = worksheet.GetActualStyleInfo(r, c, area);
                if (info != null)
                {
                    styleColumnHeaderDictionary.Add(num, info);
                }
                return info;
            }
            if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                if (styleRowHeaderDicitonary.ContainsKey(num))
                {
                    return styleRowHeaderDicitonary[num];
                }
                StyleInfo info2 = worksheet.GetActualStyleInfo(r, c, area);
                if (info2 != null)
                {
                    styleRowHeaderDicitonary.Add(num, info2);
                }
                return info2;
            }
            if (area != SheetArea.Cells)
            {
                return null;
            }
            if (styleCellDictionary.ContainsKey(num))
            {
                return styleCellDictionary[num];
            }
            StyleInfo info3 = worksheet.GetActualStyleInfo(r, c, area);
            if (info3 != null)
            {
                styleCellDictionary.Add(num, info3);
            }
            return info3;
        }

        static BorderLine GetBorderLine(Worksheet worksheet, int r, int c, SheetArea area, int border)
        {
            StyleInfo info;
            if (((r > -1) && (r < worksheet.GetRowCount(area))) && ((c > -1) && (c < worksheet.GetColumnCount(area))))
            {
                info = FindStyle(worksheet, r, c, area);
            }
            else
            {
                info = null;
            }
            if (info != null)
            {
                switch (border)
                {
                    case 0:
                        return info.BorderLeft;

                    case 1:
                        return info.BorderTop;

                    case 2:
                        return info.BorderRight;

                    case 3:
                        return info.BorderBottom;
                }
            }
            return null;
        }

        static string GetBorderString(string lineStyle, Windows.UI.Color color)
        {
            string str = null;
            if (lineStyle == "double")
            {
                str = "3px";
            }
            else
            {
                str = "2px";
            }
            return string.Format("{0} {1} {2}", (object[]) new object[] { str, lineStyle, GetHtmlColor(color) });
        }

        internal static Cell GetCell(Worksheet worksheet, int rowindex, int columnindex, SheetArea area)
        {
            if (area == (SheetArea.CornerHeader | SheetArea.RowHeader))
            {
                return worksheet.RowHeader.Cells[rowindex, columnindex];
            }
            if (area == SheetArea.ColumnHeader)
            {
                return worksheet.ColumnHeader.Cells[rowindex, columnindex];
            }
            if (area != SheetArea.Cells)
            {
                throw new NotSupportedException(ResourceStrings.HtmlGetCellInvalidSheetAreaError);
            }
            return worksheet.Cells[rowindex, columnindex];
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="layouts">The layouts.</param>
        /// <returns></returns>
        static PartLayoutData GetColumnWidths(SheetArea area, PartsLayoutData layouts)
        {
            PartLayoutData data = null;
            switch (area)
            {
                case SheetArea.CornerHeader:
                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return layouts.RowHeaderWidths;

                case SheetArea.Cells:
                case SheetArea.ColumnHeader:
                    return layouts.ColumnWidths;

                case (SheetArea.Cells | SheetArea.RowHeader):
                    return data;
            }
            return data;
        }

        /// <summary>
        /// Gets the name of the CSS friendly.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        static string GetCSSFriendlyName(string name)
        {
            char[] chArray = @" #.\/@+>:{}<".ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                name = name.Replace(chArray[i], '_');
            }
            return name;
        }

        static Dictionary<string, string> GetGridlineStyle(BorderLine Line)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary["overflow"] = "hidden";
            if (Line != null)
            {
                string str = string.Format("1px {0} {1}", (object[]) new object[] { "solid", GetHtmlColor(Line.Color) });
                dictionary["border-left"] = str;
                dictionary["border-top"] = str;
                dictionary["border-right"] = str;
                dictionary["border-bottom"] = str;
            }
            return dictionary;
        }

        static string GetHtmlBorderString(Worksheet sheet, BorderLine border)
        {
            string str = null;
            try
            {
                ((IThemeContextSupport) border).SetContext(sheet);
                if (border == null)
                {
                    return null;
                }
                switch (border.Style)
                {
                    case BorderLineStyle.Thin:
                    case BorderLineStyle.Medium:
                    case BorderLineStyle.Thick:
                        return GetBorderString("solid", border.Color);

                    case BorderLineStyle.Dashed:
                    case BorderLineStyle.Hair:
                    case BorderLineStyle.MediumDashed:
                    case BorderLineStyle.SlantedDashDot:
                        return GetBorderString("dashed", border.Color);

                    case BorderLineStyle.Dotted:
                    case BorderLineStyle.DashDot:
                    case BorderLineStyle.MediumDashDot:
                    case BorderLineStyle.DashDotDot:
                    case BorderLineStyle.MediumDashDotDot:
                        return GetBorderString("dotted", border.Color);

                    case BorderLineStyle.Double:
                        return GetBorderString("double", border.Color);
                }
                return str;
            }
            finally
            {
                ((IThemeContextSupport) border).SetContext(null);
            }
        }

        static string GetHtmlColor(Windows.UI.Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}", (object[]) new object[] { ((byte) color.R), ((byte) color.G), ((byte) color.B) });
        }

        /// <summary>
        /// Gets the HTML fill effect.
        /// </summary>
        /// <param name="fill">The fill.</param>
        /// <returns></returns>
        static string GetHtmlFillEffect(Brush fill)
        {
            if (fill is SolidColorBrush)
            {
                SolidColorBrush solidbrush = fill as SolidColorBrush;
                string fillColor = string.Empty;
                fillColor = GetHtmlColor(solidbrush.Color);
                return fillColor;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the left side sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        static SheetArea GetLeftSideSheetArea(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return area;

                case SheetArea.Cells:
                    return (SheetArea.CornerHeader | SheetArea.RowHeader);

                case SheetArea.ColumnHeader:
                    return SheetArea.CornerHeader;
            }
            throw new ArgumentOutOfRangeException("area");
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <param name="layouts">The layouts.</param>
        /// <returns></returns>
        static PartLayoutData GetRowHeights(SheetArea area, PartsLayoutData layouts)
        {
            PartLayoutData data = null;
            switch (area)
            {
                case SheetArea.CornerHeader:
                case SheetArea.ColumnHeader:
                    return layouts.ColumnHeaderHeights;

                case SheetArea.Cells:
                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return layouts.RowHeights;

                case (SheetArea.Cells | SheetArea.RowHeader):
                    return data;
            }
            return data;
        }

        /// <summary>
        /// Gets the index of the sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        static int GetSheetAreaIndex(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                    return 3;

                case SheetArea.Cells:
                    return 0;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return 1;

                case SheetArea.ColumnHeader:
                    return 2;
            }
            return -1;
        }

        static BorderLine MaxBorderLine(BorderLine l1, BorderLine l2, BorderLine gridLine)
        {
            if ((l1 != null) && (l1.Style == BorderLineStyle.Double))
            {
                return l1;
            }
            if ((l2 != null) && (l2.Style == BorderLineStyle.Double))
            {
                return l2;
            }
            BorderLine line = (l1 > l2) ? l1 : l2;
            if (line != null)
            {
                return line;
            }
            return gridLine;
        }

        /// <summary>
        /// Saves the specified range in the specified worksheet as an HTML table.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">Starting row index of the range to save.</param>
        /// <param name="column">Starting column index of the range to save.</param>
        /// <param name="rowCount">Number of rows in the range to save.</param>
        /// <param name="columnCount">Number of columns in the range to save.</param>
        /// <param name="rowHeaders">Whether to include row headers.</param>
        /// <param name="columnHeaders">Whether to include column headers.</param>
        public static string SaveHtmlRange(Worksheet worksheet, int row, int column, int rowCount, int columnCount, bool rowHeaders, bool columnHeaders)
        {
            if (worksheet == null)
            {
                throw new ArgumentNullException("sheet");
            }
            int num = worksheet.RowCount;
            int num2 = worksheet.ColumnCount;
            if ((row < -1) || (row >= num))
            {
                throw new ArgumentException(string.Format(ResourceStrings.InvaildRowIndexWithAllowedRangeBehind, (object[]) new object[] { ((int) row), ((int) num) }));
            }
            if ((column < -1) || (column >= num2))
            {
                throw new ArgumentException(string.Format(ResourceStrings.InvalidColumnIndex0MustBeBetween1And1, (object[]) new object[] { ((int) column), ((int) num2) }));
            }
            if ((row != -1) && ((row + rowCount) > num))
            {
                throw new ArgumentException(string.Format(ResourceStrings.InvalidRowCountWithAllowedRowCountBehind, (object[]) new object[] { ((int) rowCount), ((int) (num - row)) }));
            }
            if ((column != -1) && ((column + columnCount) > num2))
            {
                throw new ArgumentException(string.Format(ResourceStrings.InvalidColumnCount0MustBeBetween1And1, (object[]) new object[] { ((int) columnCount), ((int) (num2 - column)) }));
            }
            if (row == -1)
            {
                row = 0;
                rowCount = worksheet.RowCount;
            }
            if (column == -1)
            {
                column = 0;
                columnCount = worksheet.ColumnCount;
            }
            if ((columnCount == 0) || (rowCount == 0))
            {
                return "No Data";
            }
            columnHeaders = columnHeaders && (worksheet.ColumnHeaderRowCount > 0);
            rowHeaders = rowHeaders && (worksheet.RowHeaderColumnCount > 0);
            StringBuilder builder = new StringBuilder();
            using (TextWriter writer = (TextWriter) new StringWriter(builder))
            {
                DivEx ex = new DivEx();
                ex.Styles["position"] = "relative";
                string cSSFriendlyName = "sdm";
                if (!string.IsNullOrEmpty(worksheet.Name))
                {
                    cSSFriendlyName = GetCSSFriendlyName(worksheet.Name);
                }
                StyleEx styleEx = new StyleEx {
                    SheetName = cSSFriendlyName
                };
                SetGridline(worksheet, styleEx, rowHeaders, columnHeaders);
                TableEx ex3 = CreateTable(worksheet, row, column, rowCount, columnCount, rowHeaders, columnHeaders, styleEx);
                ex3.Styles["border"] = string.Format("1px {0} {1}", (object[]) new object[] { "solid", "#9EB6CE" });
                ex3.Styles["border-collapse"] = "collapse";
                ex3.Styles["table-layout"] = "automatic";
                ex3.Styles["overflow"] = "visible";
                ex3.Attributes["cellspacing"] = "0";
                ex3.Attributes["cellpadding"] = "0";
                ex3.Attributes["id"] = cSSFriendlyName;
                ex.Childs.Add(ex3);
                ex.Childs.Add(styleEx);
                ex.Render(writer);
                styleRowHeaderDicitonary = new Dictionary<ulong, StyleInfo>();
                styleColumnHeaderDictionary = new Dictionary<ulong, StyleInfo>();
                styleCellDictionary = new Dictionary<ulong, StyleInfo>();
            }
            return builder.ToString();
        }

        /// <summary>
        /// Sets the col group.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="column">The column.</param>
        /// <param name="columnCount">The column count.</param>
        /// <param name="rowHeaders">if set to <c>true</c> [row headers].</param>
        /// <param name="layouts">The layouts.</param>
        /// <param name="lastNonEmptyColumn">The lastNonEmptyColumn index.</param>
        static void SetColGroup(TableEx table, Worksheet worksheet, int column, int columnCount, bool rowHeaders, PartsLayoutData layouts, int lastNonEmptyColumn)
        {
            double num = 0.0;
            PartLayoutData columnWidths = null;
            if (rowHeaders)
            {
                int rowHeaderColumnCount = worksheet.RowHeaderColumnCount;
                columnWidths = layouts.RowHeaderWidths;
                for (int j = 0; j < rowHeaderColumnCount; j++)
                {
                    double size = columnWidths.GetSize(j);
                    if (size < 0.0)
                    {
                        size = 0.0;
                    }
                    table.ColGroup.Cols.Add(new ColEx((int) size));
                    num += size;
                }
            }
            int num5 = column + columnCount;
            if (lastNonEmptyColumn < num5)
            {
                num5 = lastNonEmptyColumn;
            }
            columnWidths = layouts.ColumnWidths;
            for (int i = column; i <= num5; i++)
            {
                double num7 = columnWidths.GetSize(i);
                if (num7 > 0.0)
                {
                    table.ColGroup.Cols.Add(new ColEx((int) num7));
                    num += num7;
                }
            }
            table.Styles["width"] = ElementEx.GetPixcel((int) num);
        }

        static void SetCornerCell(TableCellHeaderEx td, Worksheet worksheet)
        {
            if (worksheet != null)
            {
                td.Attributes["colspan"] = ((int) worksheet.RowHeaderColumnCount).ToString();
                td.Attributes["rowspan"] = ((int) worksheet.ColumnHeaderRowCount).ToString();
                if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    td.Styles["background"] = "#3A3A3A";
                }
                else
                {
                    td.Styles["background"] = "#A29F9F";
                }
                string str = string.Format("1px {0} {1}", (object[]) new object[] { "solid", "#9EB6CE" });
                td.Styles["border-left"] = str;
                td.Styles["border-top"] = str;
                td.Styles["border-right"] = str;
                td.Styles["border-bottom"] = str;
            }
        }

        static void SetGridline(Worksheet worksheet, StyleEx styleEx, bool rowHeaders, bool columnHeaders)
        {
            Dictionary<string, string> gridlineStyle = GetGridlineStyle(worksheet.GetGridLine(SheetArea.Cells));
            if (worksheet.ShowGridLine)
            {
                styleEx.AddStyle("tbody td", gridlineStyle);
            }
            if (columnHeaders)
            {
                gridlineStyle = GetGridlineStyle(worksheet.GetGridLine(SheetArea.ColumnHeader));
                styleEx.AddStyle("thead th.c", gridlineStyle);
            }
            if (rowHeaders)
            {
                gridlineStyle = GetGridlineStyle(worksheet.GetGridLine(SheetArea.CornerHeader | SheetArea.RowHeader));
                styleEx.AddStyle("tbody th.l", gridlineStyle);
            }
            if (columnHeaders && rowHeaders)
            {
                gridlineStyle = GetGridlineStyle(worksheet.GetGridLine(SheetArea.CornerHeader));
                styleEx.AddStyle("thead th.l", gridlineStyle);
            }
        }

        /// <summary>
        /// Sets the table cell.
        /// </summary>
        /// <param name="td">The td.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="styleEx">The style ex.</param>
        static void SetTableCell(TableCellEx td, Worksheet worksheet, SheetArea area, int rowIndex, int columnIndex, StyleEx styleEx)
        {
            Cell cell = GetCell(worksheet, rowIndex, columnIndex, area);
            object obj1 = cell.Value;
            bool flag = true;
            ElementEx ex = td;
            DivEx ex2 = new DivEx();
            td.Childs.Add(ex2);
            ex = ex2;
            if (cell != null)
            {
                HorizontalAlignment alignment = cell.ToHorizontalAlignment();
                switch (alignment)
                {
                    case HorizontalAlignment.Left:
                        td.Styles["text-align"] = "left";
                        break;

                    case HorizontalAlignment.Center:
                        td.Styles["text-align"] = "center";
                        break;

                    case HorizontalAlignment.Right:
                        td.Styles["text-align"] = "right";
                        break;

                    default:
                        td.Styles["text-align"] = "justify";
                        break;
                }
                switch (cell.VerticalAlignment)
                {
                    case CellVerticalAlignment.Top:
                        td.Styles["vertical-align"] = "top";
                        break;

                    case CellVerticalAlignment.Center:
                        td.Styles["vertical-align"] = "middle";
                        break;

                    case CellVerticalAlignment.Bottom:
                        td.Styles["vertical-align"] = "bottom";
                        break;
                }
                if (cell.ActualTextIndent > 0)
                {
                    switch (alignment)
                    {
                        case HorizontalAlignment.Left:
                            td.Styles["padding-left"] = ((int) cell.ActualTextIndent).ToString() + "px";
                            break;

                        case HorizontalAlignment.Right:
                            td.Styles["padding-right"] = ((int) cell.ActualTextIndent).ToString() + "px";
                            break;
                    }
                }
                string htmlFillEffect = GetHtmlFillEffect(cell.ActualBackground);
                if ((htmlFillEffect == string.Empty) && ((area == SheetArea.ColumnHeader) || (area == (SheetArea.CornerHeader | SheetArea.RowHeader))))
                {
                    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        htmlFillEffect = "#3A3A3A";
                    }
                    else
                    {
                        htmlFillEffect = "#CCCCCC";
                    }
                }
                if (!string.IsNullOrEmpty(htmlFillEffect))
                {
                    td.Styles["background"] = htmlFillEffect;
                }
                string str2 = GetHtmlFillEffect(cell.ActualForeground);
                if (!string.IsNullOrEmpty(str2))
                {
                    td.Styles["color"] = str2;
                }
                BorderLine gridLine = worksheet.GetGridLine(area);
                BorderLine border = MaxBorderLine(GetBorderLine(worksheet, rowIndex, columnIndex, area, 0), GetBorderLine(worksheet, rowIndex, columnIndex - 1, area, 2), gridLine);
                if ((border != null) && !border.IsBuiltIn)
                {
                    td.Styles["border-left"] = GetHtmlBorderString(worksheet, border);
                }
                BorderLine line3 = MaxBorderLine(GetBorderLine(worksheet, rowIndex, columnIndex, area, 1), GetBorderLine(worksheet, rowIndex - 1, columnIndex, area, 3), gridLine);
                if ((line3 != null) && !line3.IsBuiltIn)
                {
                    td.Styles["border-top"] = GetHtmlBorderString(worksheet, line3);
                }
                BorderLine line4 = MaxBorderLine(GetBorderLine(worksheet, rowIndex, columnIndex, area, 2), GetBorderLine(worksheet, rowIndex, columnIndex + 1, area, 0), gridLine);
                if ((line4 != null) && !line4.IsBuiltIn)
                {
                    td.Styles["border-right"] = GetHtmlBorderString(worksheet, line4);
                }
                BorderLine line5 = MaxBorderLine(GetBorderLine(worksheet, rowIndex, columnIndex, area, 3), GetBorderLine(worksheet, rowIndex + 1, columnIndex, area, 1), gridLine);
                if ((line5 != null) && !line5.IsBuiltIn)
                {
                    td.Styles["border-bottom"] = GetHtmlBorderString(worksheet, line5);
                }
                if (cell.ActualFontFamily != null)
                {
                    td.Styles["font-family"] = string.Format("\"{0}\"", (object[]) new object[] { cell.ActualFontFamily });
                }
                if (cell.ActualFontSize > 0.0)
                {
                    td.Styles["font-size"] = ((double) cell.ActualFontSize).ToString();
                }
                td.Styles["font-style"] = string.Format("{0}", (object[]) new object[] { cell.ActualFontStyle });
                td.Styles["font-weight"] = ConvertFontWeight(cell.ActualFontWeight);
                if (cell.ActualUnderline && !cell.ActualStrikethrough)
                {
                    td.Styles["text-decoration"] = "underline";
                }
                if (!cell.ActualUnderline && cell.ActualStrikethrough)
                {
                    td.Styles["text-decoration"] = "line-through";
                }
                if (cell.ActualUnderline && cell.ActualStrikethrough)
                {
                    td.Styles["text-decoration"] = "underline line-through";
                }
            }
            string str3 = flag ? worksheet.GetText(rowIndex, columnIndex, area) : string.Empty;
            str3 = (string.IsNullOrEmpty(str3) || str3.Equals(" ")) ? "&nbsp;" : HtmlEncoder.Encode(str3);
            str3 = str3.Replace("\r\n", "<br/>");
            ex.Content = str3;
            if ((styleEx != null) && (td.Styles.Count > 0))
            {
                string str4 = AddToStyleEx(styleEx, rowIndex, columnIndex, td, area);
                if (!string.IsNullOrEmpty(str4))
                {
                    td.IsInlineStyle = false;
                    str4 = str4.Substring(1);
                    if (td.Attributes.ContainsKey("class"))
                    {
                        str4 = str4 + " " + td.Attributes["class"];
                    }
                    td.Attributes["class"] = str4;
                }
            }
            object tag = cell.Tag;
            if (tag != null)
            {
                string str = tag.ToString();
                td.Attributes["Tag"] = HtmlEncoder.Encode(str);
            }
        }

        /// <summary>
        /// Sets the table part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="area">The area.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="hasLeft">if set to <c>true</c> [has left].</param>
        /// <param name="layouts">The layouts.</param>
        /// <param name="spans">The spans.</param>
        /// <param name="styleEx">The style ex.</param>
        /// <param name="lastNonEmptyRow">The last non empty row.</param>
        /// <param name="lastNonEmptyColumn">The last non empty column.</param>
        static void SetTablePart(TablePartEx part, SheetArea area, Worksheet worksheet, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, bool hasLeft, PartsLayoutData layouts, PartsSpanLayoutData spans, StyleEx styleEx, int lastNonEmptyRow, int lastNonEmptyColumn)
        {
            PartLayoutData rowHeights = GetRowHeights(area, layouts);
            List<CellRange> paintedCellRanges = new List<CellRange>();
            List<CellRange> list2 = new List<CellRange>();
            new List<CellRange>();
            if (area == SheetArea.Cells)
            {
                rowEndIndex = (rowEndIndex < lastNonEmptyRow) ? rowEndIndex : lastNonEmptyRow;
            }
            if (lastNonEmptyColumn < columnEndIndex)
            {
                columnEndIndex = lastNonEmptyColumn;
            }
            for (int i = rowStartIndex; i <= rowEndIndex; i++)
            {
                bool actualRowVisible = worksheet.GetActualRowVisible(i, area);
                TableRowEx tr = new TableRowEx();
                if (rowHeights.GetSize(i) > 0.0)
                {
                    tr.Height = (int) rowHeights.GetSize(i);
                }
                if (actualRowVisible)
                {
                    if (hasLeft)
                    {
                        SheetArea leftSideSheetArea = GetLeftSideSheetArea(area);
                        if (leftSideSheetArea == SheetArea.CornerHeader)
                        {
                            if (i == rowStartIndex)
                            {
                                TableCellHeaderEx td = new TableCellHeaderEx();
                                SetCornerCell(td, worksheet);
                                tr.Cells.Add(td);
                            }
                        }
                        else
                        {
                            SetTableRow(tr, worksheet, leftSideSheetArea, i, rowStartIndex, rowEndIndex, 0, worksheet.GetColumnCount(leftSideSheetArea) - 1, GetColumnWidths(leftSideSheetArea, layouts), spans.GetSpanLayoutData(leftSideSheetArea), styleEx, paintedCellRanges);
                        }
                    }
                    SetTableRow(tr, worksheet, area, i, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, GetColumnWidths(area, layouts), spans.GetSpanLayoutData(area), styleEx, list2);
                    part.Rows.Add(tr);
                }
            }
        }

        /// <summary>
        /// Sets the table row.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="columnWidths">The column widths.</param>
        /// <param name="spans">The spans.</param>
        /// <param name="styleEx">The style ex.</param>
        /// <param name="paintedCellRanges">The painted cell ranges.</param>
        static void SetTableRow(TableRowEx tr, Worksheet worksheet, SheetArea area, int rowIndex, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, PartLayoutData columnWidths, SpanLayoutData spans, StyleEx styleEx, List<CellRange> paintedCellRanges)
        {
            if (paintedCellRanges == null)
            {
                throw new ArgumentNullException("paintedCellRanges");
            }
            for (int i = columnStartIndex; i <= columnEndIndex; i++)
            {
                bool actualColumnVisible = worksheet.GetActualColumnVisible(i, area);
                TableCellEx td = (area == SheetArea.Cells) ? new TableCellEx() : new TableCellHeaderEx();
                switch (area)
                {
                    case SheetArea.CornerHeader:
                    case (SheetArea.CornerHeader | SheetArea.RowHeader):
                        td.Attributes["class"] = "l";
                        break;

                    case SheetArea.ColumnHeader:
                        td.Attributes["class"] = "c";
                        break;
                }
                CellRange range = spans.Find(rowIndex, i);
                int column = i;
                int num3 = rowIndex;
                if (range != null)
                {
                    if (paintedCellRanges.Contains(range) || !worksheet.GetActualColumnVisible(column, area))
                    {
                        continue;
                    }
                    int num4 = 0;
                    int num5 = 0;
                    for (int j = num3; j < (range.Row + range.RowCount); j++)
                    {
                        if (worksheet.GetActualRowVisible(j, area))
                        {
                            num4++;
                        }
                    }
                    for (int k = column; k < (range.Column + range.ColumnCount); k++)
                    {
                        if (worksheet.GetColumnVisible(i, area))
                        {
                            num5++;
                        }
                    }
                    column = SpanLayoutData.GetValueColumn(worksheet, area, range.Row, range.Column, spans.PureSpans);
                    num3 = SpanLayoutData.GetValueRow(worksheet, area, range.Row, range.Column, spans.PureSpans);
                    if (num5 > 1)
                    {
                        td.Attributes["colspan"] = ((int) Math.Min((columnEndIndex - range.Column) + 1, num5)).ToString();
                    }
                    if (num4 > 1)
                    {
                        td.Attributes["rowspan"] = ((int) Math.Min((rowEndIndex - range.Row) + 1, num4)).ToString();
                    }
                    paintedCellRanges.Add(range);
                }
                if (!actualColumnVisible)
                {
                    td.Styles["display"] = "none";
                }
                SetTableCell(td, worksheet, area, num3, column, styleEx);
                tr.Cells.Add(td);
            }
        }

        /// <summary>
        /// Internal only.
        /// ColEx
        /// </summary>
        class ColEx : HtmlReport.ElementEx
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.HtmlReport.ColEx" /> class.
            /// </summary>
            public ColEx()
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.HtmlReport.ColEx" /> class.
            /// </summary>
            /// <param name="width">The width.</param>
            public ColEx(int width)
            {
                this.Width = width;
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "col"; }
            }

            /// <summary>
            /// Sets the width.
            /// </summary>
            /// <value>The width.</value>
            public int Width
            {
                set { base.attributes["width"] = HtmlReport.ElementEx.GetPixcel(value); }
            }
        }

        /// <summary>
        /// Internal only.
        /// ColGroupEx
        /// </summary>
        class ColGroupEx : HtmlReport.ElementEx
        {
            readonly List<HtmlReport.ColEx> cols = new List<HtmlReport.ColEx>();

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                using (List<HtmlReport.ColEx>.Enumerator enumerator = this.cols.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Render(writer);
                    }
                }
            }

            /// <summary>
            /// Gets the columns.
            /// </summary>
            /// <value>The columns.</value>
            public List<HtmlReport.ColEx> Cols
            {
                get { return  this.cols; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "colgroup"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// DivEx
        /// </summary>
        class DivEx : HtmlReport.ElementEx
        {
            readonly List<HtmlReport.ElementEx> childs = new List<HtmlReport.ElementEx>();

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                using (List<HtmlReport.ElementEx>.Enumerator enumerator = this.childs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Render(writer);
                    }
                }
                base.RenderContent(writer);
            }

            /// <summary>
            /// Gets the child elements.
            /// </summary>
            /// <value>The child element.</value>
            public List<HtmlReport.ElementEx> Childs
            {
                get { return  this.childs; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "div"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// ElementEx
        /// </summary>
        abstract class ElementEx
        {
            public const string _Automatic = "automatic";
            public const string _Background = "background";
            public const string _Bold = "bold";
            public const string _Border = "border";
            public const string _BorderBottom = "border-bottom";
            public const string _BorderCollapse = "border-collapse";
            public const string _BorderLeft = "border-left";
            public const string _BorderRight = "border-right";
            public const string _BorderTop = "border-top";
            public const string _Bottom = "bottom";
            public const string _BreakAll = "break-all";
            public const string _BreakWord = "break-word";
            public const string _CellPadding = "cellpadding";
            public const string _CellSpacing = "cellspacing";
            public const string _Center = "center";
            public const string _Class = "class";
            public const string _Col = "col";
            public const string _ColGroup = "colgroup";
            public const string _Collapse = "collapse";
            public const string _Color = "color";
            public const string _ColSpan = "colspan";
            public const string _Dashed = "dashed";
            public const string _Direction = "direction";
            public const string _Display = "display";
            public const string _Div = "div";
            public const string _Dotted = "dotted";
            public const string _Double = "double";
            public const string _FontFamily = "font-family";
            public const string _FontSize = "font-size";
            public const string _FontStyle = "font-style";
            public const string _FontWeight = "font-weight";
            public const string _Height = "height";
            public const string _Hidden = "hidden";
            public const string _Id = "id";
            public const string _Inset = "inset";
            public const string _Italic = "italic";
            public const string _Justify = "justify";
            public const string _Left = "left";
            public const string _LineThrough = "line-through";
            public const string _Middle = "middle";
            public const string _None = "none";
            public const string _Normal = "normal";
            public const string _NoWrap = "nowrap";
            public const string _Outset = "outset";
            public const string _Overflow = "overflow";
            public const string _Overline = "overline";
            public const string _PandingLeft = "padding-left";
            public const string _PandingRight = "padding-right";
            public const string _Pixel = "px";
            public const string _Position = "position";
            public const string _Px = "px";
            public const string _Relative = "relative";
            public const string _Right = "right";
            public const string _RightToLeft = "rtl";
            public const string _RowSpan = "rowspan";
            public const string _Solid = "solid";
            public const string _Style = "style";
            public const string _Table = "table";
            public const string _TableBody = "tbody";
            public const string _TableCell = "td";
            public const string _TableCellHeader = "th";
            public const string _TableHeader = "thead";
            public const string _TableLayout = "table-layout";
            public const string _TableRow = "tr";
            public const string _TextAlign = "text-align";
            public const string _TextDecoration = "text-decoration";
            public const string _TextIndent = "text-indent";
            public const string _Title = "title";
            public const string _Top = "top";
            public const string _Underline = "underline";
            public const string _VerticalAlign = "vertical-align";
            public const string _WhiteSpace = "white-space";
            public const string _Width = "width";
            public const string _WordBreak = "word-break";
            public const string _WordWrap = "word-wrap";
            protected readonly Dictionary<string, string> attributes = new Dictionary<string, string>();
            string content;
            bool isInlineStyle = true;
            protected const string Newline = "\r\n";
            protected readonly Dictionary<string, string> styles = new Dictionary<string, string>();

            protected ElementEx()
            {
            }

            /// <summary>
            /// Gets the attribute string.
            /// </summary>
            /// <param name="attributes">The attributes.</param>
            /// <returns></returns>
            protected static string GetAttributeString(ICollection<KeyValuePair<string, string>> attributes)
            {
                return GetDictionaryString(attributes, " {0}=\"{1}\"");
            }

            /// <summary>
            /// Gets the dictionary string.
            /// </summary>
            /// <param name="dic">The dictionary.</param>
            /// <param name="formatter">The formatter.</param>
            /// <returns></returns>
            static string GetDictionaryString(ICollection<KeyValuePair<string, string>> dic, string formatter)
            {
                StringBuilder builder = new StringBuilder();
                if ((dic != null) && (dic.Count > 0))
                {
                    foreach (KeyValuePair<string, string> pair in dic)
                    {
                        builder.AppendFormat(formatter, (object[]) new object[] { pair.Key, pair.Value });
                    }
                }
                return builder.ToString();
            }

            /// <summary>
            /// Gets the pixcel.
            /// </summary>
            /// <param name="number">The number.</param>
            /// <returns></returns>
            public static string GetPixcel(int number)
            {
                return string.Format("{0}px", (object[]) new object[] { ((int) number) });
            }

            /// <summary>
            /// Gets the styles string.
            /// </summary>
            /// <param name="styles">The styles.</param>
            /// <returns></returns>
            protected static string GetStylesString(ICollection<KeyValuePair<string, string>> styles)
            {
                return GetDictionaryString(styles, "{0}:{1}; ");
            }

            /// <summary>
            /// Gets the styles string.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="styles">The styles.</param>
            /// <returns></returns>
            public static string GetStylesString(string selector, ICollection<KeyValuePair<string, string>> styles)
            {
                return string.Format("{0}{{{1}}}", (object[]) new object[] { selector, GetStylesString(styles) });
            }

            /// <summary>
            /// Renders the specified writer.
            /// </summary>
            /// <param name="writer">The writer.</param>
            public void Render(TextWriter writer)
            {
                this.RenderBeforeTag(writer);
                this.RenderStartTag(writer);
                this.RenderContent(writer);
                this.RenderEndTag(writer);
                this.RenderAfterTag(writer);
            }

            /// <summary>
            /// Renders the after tag.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected virtual void RenderAfterTag(TextWriter writer)
            {
            }

            /// <summary>
            /// Renders the before tag.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected virtual void RenderBeforeTag(TextWriter writer)
            {
            }

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected virtual void RenderContent(TextWriter writer)
            {
                if (!string.IsNullOrEmpty(this.Content))
                {
                    writer.Write(this.Content + "\r\n");
                }
            }

            /// <summary>
            /// Renders the end tag.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected virtual void RenderEndTag(TextWriter writer)
            {
                writer.Write("</{0}>\r\n", (object[]) new object[] { this.TagName });
            }

            /// <summary>
            /// Renders the start tag.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected virtual void RenderStartTag(TextWriter writer)
            {
                writer.Write("<{0}", (object[]) new object[] { this.TagName });
                writer.Write(GetAttributeString((ICollection<KeyValuePair<string, string>>) this.Attributes));
                if (this.IsInlineStyle && (this.Styles.Count > 0))
                {
                    writer.Write(" {0}=\"{1}\"", (object[]) new object[] { "style", GetStylesString((ICollection<KeyValuePair<string, string>>) this.Styles) });
                }
                writer.Write(">\r\n");
            }

            /// <summary>
            /// Gets the attributes.
            /// </summary>
            /// <value>The attributes.</value>
            public Dictionary<string, string> Attributes
            {
                get { return  this.attributes; }
            }

            /// <summary>
            /// Gets or sets the content.
            /// </summary>
            /// <value>The content.</value>
            public string Content
            {
                get { return  this.content; }
                set { this.content = value; }
            }

            /// <summary>
            /// Gets or sets a value that indicates whether this instance is an inline style.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is an inline style; otherwise, <c>false</c>.
            /// </value>
            public bool IsInlineStyle
            {
                get { return  this.isInlineStyle; }
                set { this.isInlineStyle = value; }
            }

            /// <summary>
            /// Gets the styles.
            /// </summary>
            /// <value>The styles.</value>
            public Dictionary<string, string> Styles
            {
                get { return  this.styles; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected abstract string TagName { get; }
        }

        /// <summary>
        /// Internal only.
        /// StyleEx
        /// </summary>
        class StyleEx : HtmlReport.ElementEx
        {
            string sheetName;
            new readonly Dictionary<string, Dictionary<string, string>> styles = new Dictionary<string, Dictionary<string, string>>();

            /// <summary>
            /// Adds the style.
            /// </summary>
            /// <param name="selector">The selector.</param>
            /// <param name="style">The style.</param>
            public void AddStyle(string selector, Dictionary<string, string> style)
            {
                if (this.styles.ContainsKey(selector))
                {
                    Dictionary<string, string> dictionary = this.styles[selector];
                    foreach (KeyValuePair<string, string> pair in style)
                    {
                        string introduced3 = pair.Key;
                        dictionary[introduced3] = pair.Value;
                    }
                }
                else
                {
                    this.styles.Add(selector, style);
                }
            }

            /// <summary>
            /// Finds an equal style.
            /// </summary>
            /// <param name="style">The style.</param>
            /// <returns></returns>
            public string FindEquals(Dictionary<string, string> style)
            {
                if (((this.styles.Count > 0) && (style != null)) && (style.Count > 0))
                {
                    foreach (KeyValuePair<string, Dictionary<string, string>> pair in this.styles)
                    {
                        if (pair.Value.Count != style.Count)
                        {
                            continue;
                        }
                        bool flag = true;
                        foreach (KeyValuePair<string, string> pair2 in style)
                        {
                            if (!pair.Value.ContainsKey(pair2.Key))
                            {
                                flag = false;
                                break;
                            }
                            if (pair.Value[pair2.Key] != null)
                            {
                                string introduced6 = pair.Value[pair2.Key];
                                if (!introduced6.Equals(pair2.Value))
                                {
                                    flag = false;
                                    break;
                                }
                            }
                        }
                        if (flag)
                        {
                            return pair.Key;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> pair in this.styles)
                {
                    writer.Write("#{0} {1}{{{2}}}\r\n", (object[]) new object[] { this.sheetName, pair.Key, HtmlReport.ElementEx.GetStylesString(pair.Value) });
                }
            }

            /// <summary>
            /// Gets or sets the name of the sheet.
            /// </summary>
            /// <value>The name of the sheet.</value>
            public string SheetName
            {
                get { return  this.sheetName; }
                set { this.sheetName = value; }
            }

            /// <summary>
            /// Gets the styles.
            /// </summary>
            /// <value>The styles.</value>
            new public Dictionary<string, Dictionary<string, string>> Styles
            {
                get { return  this.styles; }
            }

            /// <summary>
            /// Gets the name of the tag.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "style"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// TableCellEx
        /// </summary>
        class TableCellEx : HtmlReport.ElementEx
        {
            readonly List<HtmlReport.ElementEx> childs = new List<HtmlReport.ElementEx>();

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                using (List<HtmlReport.ElementEx>.Enumerator enumerator = this.childs.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Render(writer);
                    }
                }
                base.RenderContent(writer);
            }

            /// <summary>
            /// Gets the child elements.
            /// </summary>
            /// <value>The child elements.</value>
            public List<HtmlReport.ElementEx> Childs
            {
                get { return  this.childs; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "td"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// TableCellHeaderEx
        /// </summary>
        class TableCellHeaderEx : HtmlReport.TableCellEx
        {
            /// <summary>
            /// Gets the name of the tag.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "th"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// TableEx
        /// </summary>
        class TableEx : HtmlReport.ElementEx
        {
            readonly HtmlReport.TablePartEx body = new HtmlReport.TablePartEx("tbody");
            readonly HtmlReport.ColGroupEx colGroup = new HtmlReport.ColGroupEx();
            readonly HtmlReport.TablePartEx header = new HtmlReport.TablePartEx("thead");

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                this.colGroup.Render(writer);
                this.header.Render(writer);
                this.body.Render(writer);
            }

            /// <summary>
            /// Gets the body.
            /// </summary>
            /// <value>The body.</value>
            public HtmlReport.TablePartEx Body
            {
                get { return  this.body; }
            }

            /// <summary>
            /// Gets the column group.
            /// </summary>
            /// <value>The column group.</value>
            public HtmlReport.ColGroupEx ColGroup
            {
                get { return  this.colGroup; }
            }

            /// <summary>
            /// Gets the header.
            /// </summary>
            /// <value>The header.</value>
            public HtmlReport.TablePartEx Header
            {
                get { return  this.header; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "table"; }
            }
        }

        /// <summary>
        /// Internal only.
        /// TablePartEx
        /// </summary>
        class TablePartEx : HtmlReport.ElementEx
        {
            readonly List<HtmlReport.TableRowEx> rows = new List<HtmlReport.TableRowEx>();
            readonly string tagName;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.HtmlReport.TablePartEx" /> class.
            /// </summary>
            /// <param name="tagName">Name of the tag.</param>
            public TablePartEx(string tagName)
            {
                this.tagName = tagName;
            }

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                using (List<HtmlReport.TableRowEx>.Enumerator enumerator = this.rows.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Render(writer);
                    }
                }
            }

            /// <summary>
            /// Gets the rows.
            /// </summary>
            /// <value>The rows.</value>
            public List<HtmlReport.TableRowEx> Rows
            {
                get { return  this.rows; }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  this.tagName; }
            }
        }

        /// <summary>
        /// Internal only.
        /// TableRowEx
        /// </summary>
        class TableRowEx : HtmlReport.ElementEx
        {
            readonly List<HtmlReport.TableCellEx> cells = new List<HtmlReport.TableCellEx>();

            /// <summary>
            /// Renders the content.
            /// </summary>
            /// <param name="writer">The writer.</param>
            protected override void RenderContent(TextWriter writer)
            {
                using (List<HtmlReport.TableCellEx>.Enumerator enumerator = this.cells.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.Render(writer);
                    }
                }
            }

            /// <summary>
            /// Gets the cells.
            /// </summary>
            /// <value>The cells.</value>
            public List<HtmlReport.TableCellEx> Cells
            {
                get { return  this.cells; }
            }

            /// <summary>
            /// Sets the height.
            /// </summary>
            /// <value>The height.</value>
            public int Height
            {
                set { base.styles["height"] = HtmlReport.ElementEx.GetPixcel(value); }
            }

            /// <summary>
            /// Gets the tag name.
            /// </summary>
            /// <value>The name of the tag.</value>
            protected override string TagName
            {
                get { return  "tr"; }
            }
        }
    }
}

