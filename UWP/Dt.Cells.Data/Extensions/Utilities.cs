#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.CalcEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Internal only.
    /// Utilities
    /// </summary>
    internal class Utilities
    {
        internal static readonly Font HtmlDefaultFont = new Font(DefaultStyleCollection.DefaultFontName, DefaultStyleCollection.DefaultFontSize, UnitType.Pixel);
        static IMeasureable htmlMeasure;
        static readonly object PaddingCellTag = new object();
        static readonly object PaddingFooterCellTag = new object();
        static Regex pageRangeRegex;
        const string pageRangeRegexEnd = "end";
        const string pageRangeRegexStart = "start";
        public const int ROW_GROUP_MIN_INDENT = 10;

        /// <summary>
        /// Calculates the count.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        public static double CalcCount(IEnumerable<double> list)
        {
            double num = 0.0;
            foreach (double num2 in list)
            {
                num += Math.Max(0.0, num2);
            }
            return num;
        }

        /// <summary>
        /// Checks the page range.
        /// </summary>
        /// <param name="pageRange">The page range.</param>
        /// <returns></returns>
        public static bool CheckPageRange(string pageRange)
        {
            if (!string.IsNullOrEmpty(pageRange))
            {
                string[] strArray = pageRange.Split(new char[] { ',' });
                if (strArray.Length <= 0)
                {
                    return false;
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    string input = strArray[i].Trim();
                    if (!PageRangeRegex.IsMatch(input))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the cell range contains the specified cell range.
        /// </summary>
        /// <param name="cr">The cell range.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        public static void ContainsCellRange(CellRange cr, ref int rowStartIndex, ref int rowEndIndex, ref int columnStartIndex, ref int columnEndIndex)
        {
            if (((cr != null) && (cr.RowCount != 0)) && (cr.ColumnCount != 0))
            {
                if (cr.Row < rowStartIndex)
                {
                    rowStartIndex = cr.Row;
                }
                if (((cr.Row + cr.RowCount) - 1) > rowEndIndex)
                {
                    rowEndIndex = (cr.Row + cr.RowCount) - 1;
                }
                if (cr.Column < columnStartIndex)
                {
                    columnStartIndex = cr.Column;
                }
                if (((cr.Column + cr.ColumnCount) - 1) > columnEndIndex)
                {
                    columnEndIndex = (cr.Column + cr.ColumnCount) - 1;
                }
            }
        }

        /// <summary>
        /// Finds the automatically merged column.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="spanModel">The span model.</param>
        /// <param name="area">The area.</param>
        /// <param name="mergeModel">The merge model.</param>
        /// <param name="topRow">The top row.</param>
        /// <param name="leftColumn">The left column.</param>
        /// <param name="bottomRow">The bottom row.</param>
        /// <param name="rightColumn">The right column.</param>
        public static void FindAutoMergeColumn(Worksheet worksheet, SheetSpanModelBase spanModel, SheetArea area, SheetSpanModelBase mergeModel, int topRow, int leftColumn, int bottomRow, int rightColumn)
        {
        }

        /// <summary>
        /// Finds the automatically merged row.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="spanModel">The span model.</param>
        /// <param name="area">The area.</param>
        /// <param name="mergeModel">The merge model.</param>
        /// <param name="topRow">The top row.</param>
        /// <param name="leftColumn">The left column.</param>
        /// <param name="bottomRow">The bottom row.</param>
        /// <param name="rightColumn">The right column.</param>
        public static void FindAutoMergeRow(Worksheet worksheet, SheetSpanModelBase spanModel, SheetArea area, SheetSpanModelBase mergeModel, int topRow, int leftColumn, int bottomRow, int rightColumn)
        {
        }

        /// <summary>
        /// Finds the overflow.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="spanModel">The span model</param>
        /// <param name="mergeModel">The merge model</param>
        /// <param name="area">The area</param>
        /// <param name="overFlowModel">The overflow model</param>
        /// <param name="topRow">The top row</param>
        /// <param name="leftColumn">The left column</param>
        /// <param name="bottomRow">The bottom row</param>
        /// <param name="rightColumn">The right column</param>
        /// <param name="columnWidths">The column widths</param>
        /// <param name="measure">The measure</param>
        /// <param name="state">The state.</param>
        static void FindOverFlow(Worksheet worksheet, SheetSpanModelBase spanModel, SheetSpanModelBase mergeModel, SheetArea area, SheetSpanModelBase overFlowModel, int topRow, int leftColumn, int bottomRow, int rightColumn, PartLayoutData columnWidths, IMeasureable measure, GcSheetSection.SheetState state)
        {
            if (measure == null)
            {
                throw new ArgumentNullException("measure");
            }
            for (int i = topRow; i <= bottomRow; i++)
            {
                if (worksheet.GetActualRowVisible(i, SheetArea.Cells))
                {
                    if ((state != null) && ((area == SheetArea.Cells) || (area == (SheetArea.CornerHeader | SheetArea.RowHeader))))
                    {
                        bool flag = false;
                        if ((i >= state.RowStartIndex) && (i <= state.RowEndIndex))
                        {
                            flag = true;
                        }
                        if ((!flag && state.HasRepeatRow) && ((i >= state.RepeatRowStartIndex) && (i <= state.RepeatRowEndIndex)))
                        {
                            flag = true;
                        }
                        if ((!flag && (worksheet.FrozenRowCount > 0)) && (i < worksheet.FrozenRowCount))
                        {
                            flag = true;
                        }
                        if ((!flag && (worksheet.FrozenTrailingRowCount > 0)) && (i >= (worksheet.GetRowCount(area) - worksheet.FrozenTrailingRowCount)))
                        {
                            flag = true;
                        }
                        if (!flag)
                        {
                            continue;
                        }
                    }
                    for (int j = leftColumn; j <= rightColumn; j++)
                    {
                        double num5;
                        if (!worksheet.GetActualColumnVisible(j, SheetArea.Cells))
                        {
                            continue;
                        }
                        if ((state != null) && ((area == SheetArea.Cells) || (area == SheetArea.ColumnHeader)))
                        {
                            bool flag2 = false;
                            if ((j >= state.ColumnStartIndex) && (j <= state.ColumnEndIndex))
                            {
                                flag2 = true;
                            }
                            if ((!flag2 && state.HasRepeatColumn) && ((j >= state.RepeatColumnStartIndex) && (j <= state.RepeatColumnEndIndex)))
                            {
                                flag2 = true;
                            }
                            if ((!flag2 && (worksheet.FrozenColumnCount > 0)) && (j < worksheet.FrozenColumnCount))
                            {
                                flag2 = true;
                            }
                            if ((!flag2 && (worksheet.FrozenTrailingColumnCount > 0)) && (j >= (worksheet.GetColumnCount(area) - worksheet.FrozenTrailingColumnCount)))
                            {
                                flag2 = true;
                            }
                            if (!flag2)
                            {
                                continue;
                            }
                        }
                        string str = worksheet.GetText(i, j, area);
                        StyleInfo style = worksheet.GetActualStyleInfo(i, j, area);
                        if (((string.IsNullOrEmpty(str) || style.WordWrap) || ((spanModel.Find(i, j) != null) || (mergeModel.Find(i, j) != null))) || (overFlowModel.Find(i, j) != null))
                        {
                            continue;
                        }
                        double num3 = GetColumnWidth(worksheet, columnWidths, area, j);
                        Font font = Font.Create(style) ?? measure.DefaultFont;
                        double width = measure.MeasureString(str, font, false, 0).Width;
                        if (width <= num3)
                        {
                            continue;
                        }
                        object obj2 = worksheet.GetValue(i, j, area);
                        bool flag3 = false;
                        bool flag4 = false;
                        switch (style.HorizontalAlignment)
                        {
                            case CellHorizontalAlignment.Left:
                                flag3 = true;
                                goto Label_024F;

                            case CellHorizontalAlignment.Center:
                                flag4 = flag3 = true;
                                goto Label_024F;

                            case CellHorizontalAlignment.Right:
                                flag4 = true;
                                goto Label_024F;

                            case CellHorizontalAlignment.General:
                                if (area == SheetArea.Cells)
                                {
                                    break;
                                }
                                flag4 = flag3 = true;
                                goto Label_024F;

                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        if (IsExcelRightAlignmentValue(obj2))
                        {
                            flag4 = true;
                        }
                        else if (IsExcelCenterAlignmentValue(obj2))
                        {
                            flag4 = flag3 = true;
                        }
                        else
                        {
                            flag3 = true;
                        }
                    Label_024F:
                        num5 = 0.0;
                        double num6 = 0.0;
                        if (flag4 && flag3)
                        {
                            num5 = num6 = (width - num3) / 2.0;
                        }
                        else if (flag4)
                        {
                            num5 = width - num3;
                        }
                        else
                        {
                            num6 = width - num3;
                        }
                        int column = j;
                        int columnCount = 1;
                        if (flag4 && (num5 > 0.0))
                        {
                            for (int k = j - 1; (k >= 0) && (num5 > 0.0); k--)
                            {
                                if ((!string.IsNullOrEmpty(worksheet.GetText(i, k, area)) || (spanModel.Find(i, k) != null)) || ((mergeModel.Find(i, k) != null) || (overFlowModel.Find(i, k) != null)))
                                {
                                    break;
                                }
                                num5 -= GetColumnWidth(worksheet, columnWidths, area, k);
                                column = k;
                                columnCount++;
                            }
                        }
                        if (flag3 && (num6 > 0.0))
                        {
                            for (int m = j + 1; (m < worksheet.GetColumnCount(area)) && (num6 > 0.0); m++)
                            {
                                if ((!string.IsNullOrEmpty(worksheet.GetText(i, m, area)) || (spanModel.Find(i, m) != null)) || ((mergeModel.Find(i, m) != null) || (overFlowModel.Find(i, m) != null)))
                                {
                                    break;
                                }
                                num6 -= GetColumnWidth(worksheet, columnWidths, area, m);
                                columnCount++;
                            }
                        }
                        if ((columnCount > 1) && !worksheet.Cells[i, column].ShrinkToFit)
                        {
                            overFlowModel.Add(i, column, 1, columnCount);
                            j += columnCount - 1;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the bottom side sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public static SheetArea GetBottomSideSheetArea(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                    return (SheetArea.CornerHeader | SheetArea.RowHeader);

                case SheetArea.Cells:
                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return area;

                case SheetArea.ColumnHeader:
                    return SheetArea.Cells;
            }
            throw new ArgumentOutOfRangeException("area");
        }

        /// <summary>
        /// Gets the color by fill mode.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        static Windows.UI.Color GetColorByFillMode(Windows.UI.Color color, ShapePathFillMode mode)
        {
            switch (mode)
            {
                case ShapePathFillMode.None:
                case ShapePathFillMode.Normal:
                    return color;

                case ShapePathFillMode.Lighten:
                    color.R = (byte) Math.Min((float) 255f, (float) (color.R * 1.4f));
                    color.G = (byte) Math.Min((float) 255f, (float) (color.G * 1.4f));
                    color.B = (byte) Math.Min((float) 255f, (float) (color.B * 1.4f));
                    return color;

                case ShapePathFillMode.LightenLess:
                    color.R = (byte) Math.Min((float) 255f, (float) (color.R * 1.2f));
                    color.G = (byte) Math.Min((float) 255f, (float) (color.G * 1.2f));
                    color.B = (byte) Math.Min((float) 255f, (float) (color.B * 1.2f));
                    return color;

                case ShapePathFillMode.Darken:
                    color.R = (byte) Math.Max((float) 0f, (float) (color.R * 0.6f));
                    color.G = (byte) Math.Max((float) 0f, (float) (color.G * 0.6f));
                    color.B = (byte) Math.Max((float) 0f, (float) (color.B * 0.6f));
                    return color;

                case ShapePathFillMode.DarkenLess:
                    color.R = (byte) Math.Max((float) 0f, (float) (color.R * 0.8f));
                    color.G = (byte) Math.Max((float) 0f, (float) (color.G * 0.8f));
                    color.B = (byte) Math.Max((float) 0f, (float) (color.B * 0.8f));
                    return color;
            }
            throw new ArgumentOutOfRangeException("mode");
        }

        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="columnWidths">The column widths.</param>
        /// <param name="area">The area.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        public static double GetColumnWidth(Worksheet worksheet, PartLayoutData columnWidths, SheetArea area, int columnIndex)
        {
            double num = 0.0;
            if (columnWidths.HasSize(columnIndex))
            {
                return columnWidths.GetSize(columnIndex);
            }
            num = worksheet.GetColumnWidth(columnIndex, area, UnitType.CentileInch);
            columnWidths.SetSize(columnIndex, num);
            return num;
        }

        /// <summary>
        /// Gets the width of the column.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="columnWidths">The column widths.</param>
        /// <param name="area">The area.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static double GetColumnWidth(Worksheet worksheet, PartLayoutData columnWidths, SheetArea area, int columnIndex, int count)
        {
            double num = 0.0;
            for (int i = columnIndex; i < (columnIndex + count); i++)
            {
                num += GetColumnWidth(worksheet, columnWidths, area, i);
            }
            return num;
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="hasTopPart">if set to <c>true</c> [has top part].</param>
        /// <param name="hasBottomPart">if set to <c>true</c> [has bottom part].</param>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static List<double> GetColumnWidths(Worksheet worksheet, SheetArea area, int startIndex, int count, bool hasTopPart, bool hasBottomPart, UnitType unit)
        {
            return GetColumnWidths(worksheet, area, startIndex, count, null, false, -1, -1, -1, -1, hasTopPart, hasBottomPart, unit, true, BorderCollapse.Collapse);
        }

        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="measure">The measurement.</param>
        /// <param name="autoFit">if set to <c>true</c> [auto fit].</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="repeatStartIndex">Start index of the repeat.</param>
        /// <param name="repeatEndIndex">End index of the repeat.</param>
        /// <param name="hasTopPart">if set to <c>true</c> [has top part].</param>
        /// <param name="hasBottomPart">if set to <c>true</c> [has bottom part].</param>
        /// <param name="unit">The unit.</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollapse">The collapsed border.</param>
        /// <returns></returns>
        public static List<double> GetColumnWidths(Worksheet worksheet, SheetArea area, int startIndex, int count, IMeasureable measure, bool autoFit, int rowStartIndex, int rowEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasTopPart, bool hasBottomPart, UnitType unit, bool showGridline, BorderCollapse borderCollapse)
        {
            List<double> list = new List<double>();
            SheetSpanModelBase spanModel = null;
            SheetSpanModelBase base3 = null;
            SheetSpanModelBase base4 = null;
            SheetArea topSideSheetArea = GetTopSideSheetArea(area);
            int num = -1;
            SheetArea bottomSideSheetArea = GetBottomSideSheetArea(area);
            int num2 = -1;
            int specStartIndex = -1;
            double hGridLineTopHeight = 0.0;
            double hGridLineBottomHeight = 0.0;
            double vGridLineLeftWidth = 0.0;
            double vGridLineRightWidth = 0.0;
            double num8 = 0.0;
            double num9 = 0.0;
            double num10 = 0.0;
            double num11 = 0.0;
            double num12 = 0.0;
            double num13 = 0.0;
            double num14 = 0.0;
            double num15 = 0.0;
            bool isColumnSorted = worksheet.IsColumnSorted;
            bool isRowSorted = worksheet.IsRowSorted;
            double num16 = 64.0;
            SheetSpanModel gSpans = null;
            if (autoFit)
            {
                spanModel = GetSpanModel(worksheet, area);
            }
            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (!worksheet.GetActualColumnVisible(i, area))
                {
                    list.Add(0.0);
                }
                else if (autoFit && (spanModel != null))
                {
                    double width = 0.0;
                    bool flag3 = false;
                    if (hasTopPart && (topSideSheetArea != SheetArea.CornerHeader))
                    {
                        flag3 = GetSheetAreaMinWidth(worksheet, topSideSheetArea, 0, num - 1, specStartIndex, specStartIndex, base3, i, measure, unit, ref width, false, num8, num9, num10, num11, isColumnSorted, isRowSorted, null, borderCollapse);
                    }
                    flag3 = GetSheetAreaMinWidth(worksheet, area, rowStartIndex, rowEndIndex, repeatStartIndex, repeatEndIndex, spanModel, i, measure, unit, ref width, true, hGridLineTopHeight, hGridLineBottomHeight, vGridLineLeftWidth, vGridLineRightWidth, isColumnSorted, isRowSorted, gSpans, borderCollapse);
                    if (hasBottomPart && (bottomSideSheetArea != SheetArea.CornerHeader))
                    {
                        flag3 = GetSheetAreaMinWidth(worksheet, bottomSideSheetArea, 0, num2 - 1, -1, -1, base4, i, measure, unit, ref width, false, num12, num13, num14, num15, isColumnSorted, isRowSorted, null, borderCollapse);
                    }
                    if (flag3 && (width == 0.0))
                    {
                        list.Add(worksheet.GetColumnWidth(i, area, unit));
                    }
                    else if (width == 0.0)
                    {
                        list.Add(num16);
                    }
                    else
                    {
                        list.Add(width);
                    }
                }
                else
                {
                    list.Add(worksheet.GetColumnWidth(i, area, unit));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the fill effect by using the fill mode.
        /// </summary>
        /// <param name="background">The background.</param>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public static Brush GetFillEffectByFillMode(Brush background, ShapePathFillMode mode)
        {
            if (((background != null) && !(background is ImageBrush)) && (mode != ShapePathFillMode.Normal))
            {
                if (background is SolidColorBrush brush)
                {
                    return new SolidColorBrush(GetColorByFillMode(brush.Color, mode));
                }
                if (background is GradientBrush)
                {
                    return null;
                }
            }
            return background;
        }

        /// <summary>
        /// Gets the gray color.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns></returns>
        public static Windows.UI.Color GetGrayColor(Windows.UI.Color color)
        {
            byte r = Convert.ToByte((int) (((((Convert.ToInt32(color.R) * 30) + (Convert.ToInt32(color.G) * 0x3b)) + (Convert.ToInt32(color.B) * 11)) + 50) / 100));
            return Windows.UI.Color.FromArgb(color.A, r, r, r);
        }

        /// <summary>
        /// Gets the left side sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public static SheetArea GetLeftSideSheetArea(SheetArea area)
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
        /// Gets the lines.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="font">The font.</param>
        /// <param name="allowWrap">if set to <c>true</c> [allow wrap].</param>
        /// <param name="width">The width.</param>
        /// <param name="measure">The measurement.</param>
        /// <returns></returns>
        public static List<string> GetLines(string str, Font font, bool allowWrap, double width, IMeasureable measure)
        {
            string str3;
            List<string> list = new List<string>();
            if (string.IsNullOrEmpty(str) || (str.Length <= 1))
            {
                list.Add(str);
                return list;
            }
            if (!allowWrap)
            {
                string str2;
                StringReader reader = new StringReader(str);
                while ((str2 = reader.ReadLine()) != null)
                {
                    list.Add(str2);
                }
                return list;
            }
            StringReader reader2 = new StringReader(str);
            while ((str3 = reader2.ReadLine()) != null)
            {
                for (int i = 1; i <= str3.Length; i++)
                {
                    string str4 = str3.Substring(0, i);
                    if (Math.Ceiling(measure.MeasureNoWrapString(str4, font).Width) > width)
                    {
                        bool flag = false;
                        if (str3[i - 1] != ' ')
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                if (str3[j] == ' ')
                                {
                                    flag = true;
                                    i = j + 1;
                                    break;
                                }
                            }
                        }
                        if (!flag && (i > 1))
                        {
                            i--;
                        }
                        str4 = str3.Substring(0, i);
                        while ((i < str3.Length) && (str3[i] == ' '))
                        {
                            i++;
                        }
                        str3 = str3.Substring(i);
                        i = 0;
                        if (str4 != null)
                        {
                            str4 = str4.Trim();
                            list.Add(str4);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(str3))
                {
                    list.Add(str3);
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the page range.
        /// </summary>
        /// <param name="pageRange">The page range.</param>
        /// <param name="lastPage">The last page.</param>
        /// <returns></returns>
        public static List<int> GetPageRange(string pageRange, int lastPage)
        {
            if (!CheckPageRange(pageRange))
            {
                return null;
            }
            List<int> list = new List<int>();
            if (!string.IsNullOrEmpty(pageRange))
            {
                string[] strArray = pageRange.Split(new char[] { ',' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    string str = strArray[i].Trim();
                    if (!string.IsNullOrEmpty(str))
                    {
                        Match match = PageRangeRegex.Match(str);
                        int num2 = -1;
                        int num3 = -1;
                        if (match.Groups["start"].Success && !string.IsNullOrEmpty(match.Groups["start"].Value))
                        {
                            try
                            {
                                num2 = int.Parse(match.Groups["start"].Value);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        if (match.Groups["end"].Success && !string.IsNullOrEmpty(match.Groups["end"].Value))
                        {
                            try
                            {
                                num3 = int.Parse(match.Groups["end"].Value);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        if (str.IndexOf('-') >= 0)
                        {
                            if (((num2 <= lastPage) || (num3 <= lastPage)) && ((num2 > 0) || (num3 > 0)))
                            {
                                if (num2 <= 0)
                                {
                                    num2 = 1;
                                }
                                if ((num2 > lastPage) && (num3 < lastPage))
                                {
                                    num2 = lastPage;
                                }
                                if ((num3 <= 0) || (num3 > lastPage))
                                {
                                    num3 = lastPage;
                                }
                                int num4 = (num3 >= num2) ? 1 : -1;
                                for (int j = num2; j != num3; j += num4)
                                {
                                    list.Add(j);
                                }
                                list.Add(num3);
                            }
                        }
                        else if ((num2 > 0) && (num2 <= lastPage))
                        {
                            list.Add(num2);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <returns></returns>
        public static Windows.Foundation.Size GetPageSize(PaperSize paperSize, PrintPageOrientation orientation)
        {
            paperSize = paperSize ?? new PaperSize();
            Windows.Foundation.Size size = new Windows.Foundation.Size();
            if (orientation == PrintPageOrientation.Landscape)
            {
                size.Height = paperSize.Width;
                size.Width = paperSize.Height;
                return size;
            }
            size.Width = paperSize.Width;
            size.Height = paperSize.Height;
            return size;
        }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="fAngle">The angle.</param>
        /// <returns></returns>
        public static Windows.Foundation.Point GetPos(double left, double top, double width, double height, double fAngle)
        {
            if (height == 0.0)
            {
                height = 0.001;
            }
            if (width == 0.0)
            {
                width = 0.001;
            }
            fAngle = (3.1415926535897931 * fAngle) / 180.0;
            double num = width / 2.0;
            double num2 = height / 2.0;
            double num3 = left + (width / 2.0);
            double num4 = top + (height / 2.0);
            double num5 = 0.0;
            double num6 = 0.0;
            if (fAngle <= 1.5707963267948966)
            {
                double num7 = Math.Tan(fAngle);
                double d = 1.0 / ((1.0 / (num * num)) + ((num7 * num7) / (num2 * num2)));
                num5 = Math.Sqrt(d);
                num6 = num5 * num7;
            }
            else if (fAngle <= 3.1415926535897931)
            {
                fAngle = 3.1415926535897931 - fAngle;
                double num9 = Math.Tan(fAngle);
                double num10 = 1.0 / ((1.0 / (num * num)) + ((num9 * num9) / (num2 * num2)));
                num5 = Math.Sqrt(num10);
                num6 = num5 * num9;
                num5 = -num5;
            }
            else if (fAngle <= 4.71238898038469)
            {
                double num11 = Math.Tan(fAngle);
                double num12 = 1.0 / ((1.0 / (num * num)) + ((num11 * num11) / (num2 * num2)));
                num5 = -Math.Sqrt(num12);
                num6 = num5 * num11;
            }
            else if (fAngle <= 6.2831853071795862)
            {
                fAngle = 6.2831853071795862 - fAngle;
                double num13 = Math.Tan(fAngle);
                double num14 = 1.0 / ((1.0 / (num * num)) + ((num13 * num13) / (num2 * num2)));
                num5 = Math.Sqrt(num14);
                num6 = -num5 * num13;
            }
            return new Windows.Foundation.Point { X = num5 + num3, Y = num6 + num4 };
        }

        /// <summary>
        /// Gets the right side sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public static SheetArea GetRightSideSheetArea(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                    return SheetArea.ColumnHeader;

                case SheetArea.Cells:
                case SheetArea.ColumnHeader:
                    return area;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return SheetArea.Cells;
            }
            throw new ArgumentOutOfRangeException("area");
        }

        /// <summary>
        /// Gets the height of the row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowHeights">The row heights.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns></returns>
        public static double GetRowHeight(Worksheet worksheet, PartLayoutData rowHeights, SheetArea area, int rowIndex)
        {
            double rowHeight = 0.0;
            if (rowHeights.HasSize(rowIndex))
            {
                return rowHeights.GetSize(rowIndex);
            }
            rowHeight = worksheet.GetRowHeight(rowIndex, area);
            rowHeights.SetSize(rowIndex, rowHeight);
            return rowHeight;
        }

        /// <summary>
        /// Gets the height of the row.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="rowHeights">The row heights.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static double GetRowHeight(Worksheet worksheet, PartLayoutData rowHeights, SheetArea area, int rowIndex, int count)
        {
            double num = 0.0;
            for (int i = rowIndex; i < (rowIndex + count); i++)
            {
                num += GetRowHeight(worksheet, rowHeights, area, i);
            }
            return num;
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="hasLeftPart">if set to <c>true</c> [has left part].</param>
        /// <param name="hasRightPart">if set to <c>true</c> [has right part].</param>
        /// <param name="unit">The unit.</param>
        /// <returns></returns>
        public static List<double> GetRowHeights(Worksheet worksheet, SheetArea area, int startIndex, int count, bool hasLeftPart, bool hasRightPart, UnitType unit)
        {
            return GetRowHeights(worksheet, area, startIndex, count, null, false, -1, -1, -1, -1, hasLeftPart, hasRightPart, null, null, null, unit, true, BorderCollapse.Collapse);
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <param name="measure">The measurement.</param>
        /// <param name="autoFit">If set to <c>true</c>, [auto fit].</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="repeatStartIndex">Start index of the repeated index.</param>
        /// <param name="repeatEndIndex">End index of the repeated index.</param>
        /// <param name="hasLeftPart">if set to <c>true</c> [has left part].</param>
        /// <param name="hasRightPart">if set to <c>true</c> [has right part].</param>
        /// <param name="colWidths">The column widths.</param>
        /// <param name="lColWidths">The column widths.</param>
        /// <param name="rColWidths">The column widths.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="showGridline">if set to <c>true</c> [show gridline].</param>
        /// <param name="borderCollpase">The collapsed border.</param>
        /// <returns></returns>
        public static List<double> GetRowHeights(Worksheet worksheet, SheetArea area, int startIndex, int count, IMeasureable measure, bool autoFit, int columnStartIndex, int columnEndIndex, int repeatStartIndex, int repeatEndIndex, bool hasLeftPart, bool hasRightPart, PartLayoutData colWidths, PartLayoutData lColWidths, PartLayoutData rColWidths, UnitType unit, bool showGridline, BorderCollapse borderCollpase)
        {
            List<double> list = new List<double>();
            SheetSpanModelBase spanModel = null;
            SheetSpanModelBase base3 = null;
            SheetSpanModelBase base4 = null;
            SheetArea leftSideSheetArea = GetLeftSideSheetArea(area);
            int num = -1;
            SheetArea rightSideSheetArea = GetRightSideSheetArea(area);
            int num2 = -1;
            int specStartIndex = -1;
            double hGridLineTopHeight = 0.0;
            double hGridLineBottomHeight = 0.0;
            double vGridLineLeftWidth = 0.0;
            double vGridLineRightWidth = 0.0;
            double num8 = 0.0;
            double num9 = 0.0;
            double num10 = 0.0;
            double num11 = 0.0;
            double num12 = 0.0;
            double num13 = 0.0;
            double num14 = 0.0;
            double num15 = 0.0;
            bool isColumnSorted = worksheet.IsColumnSorted;
            bool isRowSorted = worksheet.IsRowSorted;
            double num16 = 24.0;
            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (!worksheet.GetActualRowVisible(i, area))
                {
                    list.Add(0.0);
                }
                else if (autoFit && (area != SheetArea.CornerHeader))
                {
                    spanModel = GetSpanModel(worksheet, area);
                    double height = 0.0;
                    bool flag3 = false;
                    if (hasLeftPart && (leftSideSheetArea != SheetArea.CornerHeader))
                    {
                        flag3 = GetSheetAreaMinHeight(worksheet, leftSideSheetArea, 0, num - 1, specStartIndex, specStartIndex, base3, i, measure, unit, ref height, lColWidths, false, num8, num9, num10, num11, isColumnSorted, isRowSorted, borderCollpase);
                    }
                    flag3 = GetSheetAreaMinHeight(worksheet, area, columnStartIndex, columnEndIndex, repeatStartIndex, repeatEndIndex, spanModel, i, measure, unit, ref height, colWidths, true, hGridLineTopHeight, hGridLineBottomHeight, vGridLineLeftWidth, vGridLineRightWidth, isColumnSorted, isRowSorted, borderCollpase);
                    if (hasRightPart)
                    {
                        flag3 = GetSheetAreaMinHeight(worksheet, rightSideSheetArea, 0, num2 - 1, -1, -1, base4, i, measure, unit, ref height, rColWidths, false, num12, num13, num14, num15, isColumnSorted, isRowSorted, borderCollpase);
                    }
                    if (flag3 && (height == 0.0))
                    {
                        list.Add(worksheet.GetRowHeight(i, area, unit));
                    }
                    else if (height == 0.0)
                    {
                        list.Add(num16);
                    }
                    else
                    {
                        list.Add(height);
                    }
                }
                else
                {
                    list.Add(worksheet.GetRowHeight(i, area, unit));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the height of the sheet area min.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="specStartIndex">Start index of the spec.</param>
        /// <param name="specEndIndex">End index of the spec.</param>
        /// <param name="spanModel">The span model.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="measure">The measure.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="height">The height.</param>
        /// <param name="colWidths">The col widths.</param>
        /// <param name="calcFrozen">if set to <c>true</c> [calc frozen].</param>
        /// <param name="hGridLineTopHeight">Height of the h grid line top.</param>
        /// <param name="hGridLineBottomHeight">Height of the h grid line bottom.</param>
        /// <param name="vGridLineLeftWidth">Width of the v grid line left.</param>
        /// <param name="vGridLineRightWidth">Width of the v grid line right.</param>
        /// <param name="isColumnSorted">if set to <c>true</c> [is column sorted].</param>
        /// <param name="isRowSorted">if set to <c>true</c> [is row sorted].</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static bool GetSheetAreaMinHeight(Worksheet worksheet, SheetArea area, int columnStartIndex, int columnEndIndex, int specStartIndex, int specEndIndex, SheetSpanModelBase spanModel, int rowIndex, IMeasureable measure, UnitType unit, ref double height, PartLayoutData colWidths, bool calcFrozen, double hGridLineTopHeight, double hGridLineBottomHeight, double vGridLineLeftWidth, double vGridLineRightWidth, bool isColumnSorted, bool isRowSorted, BorderCollapse borderCollapse)
        {
            int row = isRowSorted ? worksheet.GetModelRowFromViewRow(rowIndex, area) : rowIndex;
            int columnCount = worksheet.GetColumnCount(area);
            bool flag = false;
            double num3 = 24.0;
            for (int i = 0; i < columnCount; i++)
            {
                double num5 = 0.0;
                bool flag2 = false;
                if ((i >= columnStartIndex) && (i <= columnEndIndex))
                {
                    flag2 = true;
                }
                if (calcFrozen)
                {
                    if ((!flag2 && (worksheet.FrozenColumnCount > 0)) && (i < worksheet.FrozenColumnCount))
                    {
                        flag2 = true;
                    }
                    if ((!flag2 && (worksheet.FrozenTrailingColumnCount > 0)) && (i >= (columnCount - worksheet.FrozenTrailingColumnCount)))
                    {
                        flag2 = true;
                    }
                }
                if (((!flag2 && (specStartIndex > -1)) && ((specEndIndex > -1) && (i >= specStartIndex))) && (i <= specEndIndex))
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    int column = isColumnSorted ? worksheet.GetModelColumnFromViewColumn(i, area) : i;
                    CellRange range = spanModel.Find(row, column);
                    bool flag3 = true;
                    if (range != null)
                    {
                        if ((range.Row != row) || (range.Column != column))
                        {
                            flag3 = false;
                        }
                        else if (range.RowCount > 1)
                        {
                            flag = true;
                            flag3 = false;
                        }
                    }
                    if (flag3)
                    {
                        string str = worksheet.GetText(rowIndex, i, area);
                        if (!string.IsNullOrEmpty(str))
                        {
                            StyleInfo style = worksheet.GetActualStyleInfo(rowIndex, i, area);
                            int w = (int) Math.Floor(GetColumnWidth(worksheet, colWidths, area, i));
                            double a = vGridLineRightWidth;
                            double num9 = hGridLineBottomHeight;
                            if ((!style.IsBorderLeftSet() && !style.IsBorderTopSet()) && (!style.IsBorderRightSet() && !style.IsBorderBottomSet()))
                            {
                                a += vGridLineLeftWidth;
                                num9 += hGridLineTopHeight;
                            }
                            w -= (int) Math.Ceiling(a);
                            if (((range != null) && (range.ColumnCount > 1)) && !worksheet.IsColumnSorted)
                            {
                                for (int j = 1; (j < range.ColumnCount) && ((i + j) < columnCount); j++)
                                {
                                    w += (int) Math.Floor(GetColumnWidth(worksheet, colWidths, area, i + j));
                                }
                            }
                            ContentAlignment alignment = (ContentAlignment.Create(style) == null) ? new ContentAlignment() : ContentAlignment.Create(style);
                            num5 = MeasureStringByAlignment(measure, str, Font.Create(style), alignment, w).Height + ((int) Math.Ceiling(num9));
                        }
                        else
                        {
                            num5 = num3;
                        }
                    }
                    height = Math.Max(height, num5);
                }
            }
            return flag;
        }

        /// <summary>
        /// Gets the width of the sheet area min.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="specStartIndex">Start index of the spec.</param>
        /// <param name="specEndIndex">End index of the spec.</param>
        /// <param name="spanModel">The span model.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <param name="measure">The measure.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="width">The width.</param>
        /// <param name="calcFrozen">if set to <c>true</c> [calc frozen].</param>
        /// <param name="hGridLineTopHeight">Height of the h grid line top.</param>
        /// <param name="hGridLineBottomHeight">Height of the h grid line bottom.</param>
        /// <param name="vGridLineLeftWidth">Width of the v grid line left.</param>
        /// <param name="vGridLineRightWidth">Width of the v grid line right.</param>
        /// <param name="isColumnSorted">if set to <c>true</c> [is column sorted].</param>
        /// <param name="isRowSorted">if set to <c>true</c> [is row sorted].</param>
        /// <param name="gSpans">The g spans.</param>
        /// <param name="borderCollapse">The border collapse.</param>
        /// <returns></returns>
        static bool GetSheetAreaMinWidth(Worksheet worksheet, SheetArea area, int rowStartIndex, int rowEndIndex, int specStartIndex, int specEndIndex, SheetSpanModelBase spanModel, int columnIndex, IMeasureable measure, UnitType unit, ref double width, bool calcFrozen, double hGridLineTopHeight, double hGridLineBottomHeight, double vGridLineLeftWidth, double vGridLineRightWidth, bool isColumnSorted, bool isRowSorted, SheetSpanModel gSpans, BorderCollapse borderCollapse)
        {
            int column = isColumnSorted ? worksheet.GetModelColumnFromViewColumn(columnIndex, area) : columnIndex;
            int rowCount = worksheet.GetRowCount(area);
            bool flag = false;
            for (int i = 0; i < rowCount; i++)
            {
                double num4 = 0.0;
                bool flag2 = false;
                if ((i >= rowStartIndex) && (i <= rowEndIndex))
                {
                    flag2 = true;
                }
                if (calcFrozen)
                {
                    if ((!flag2 && (worksheet.FrozenRowCount > 0)) && (i < worksheet.FrozenRowCount))
                    {
                        flag2 = true;
                    }
                    if ((!flag2 && (worksheet.FrozenTrailingRowCount > 0)) && (i >= (rowCount - worksheet.FrozenTrailingRowCount)))
                    {
                        flag2 = true;
                    }
                }
                if (((!flag2 && (specStartIndex > -1)) && ((specEndIndex > -1) && (i >= specStartIndex))) && (i <= specEndIndex))
                {
                    flag2 = true;
                }
                if (flag2)
                {
                    int row = isRowSorted ? worksheet.GetModelRowFromViewRow(i, area) : i;
                    CellRange range = spanModel.Find(row, column);
                    bool flag3 = true;
                    if ((range == null) && (gSpans != null))
                    {
                        range = gSpans.Find(i, columnIndex);
                    }
                    if (range != null)
                    {
                        if ((range.Row != row) || (range.Column != column))
                        {
                            flag3 = false;
                        }
                        else if (range.ColumnCount > 1)
                        {
                            flag = true;
                            flag3 = false;
                        }
                    }
                    if (flag3)
                    {
                        string str = worksheet.GetText(i, columnIndex, area);
                        if (!string.IsNullOrEmpty(str))
                        {
                            if ((range != null) && (range.ColumnCount > 1))
                            {
                                num4 = worksheet.GetColumnWidth(columnIndex, area, unit);
                            }
                            else
                            {
                                StyleInfo style = worksheet.GetActualStyleInfo(i, columnIndex, area);
                                num4 = ((int) Math.Ceiling(MeasureStringByAlignment(measure, str, Font.Create(style), ContentAlignment.Create(style), (int) Math.Floor(worksheet.GetColumnWidth(columnIndex, area, unit))).Width)) + 4;
                                double a = vGridLineRightWidth;
                                if ((!style.IsBorderLeftSet() && !style.IsBorderTopSet()) && (!style.IsBorderRightSet() && !style.IsBorderBottomSet()))
                                {
                                    a += vGridLineLeftWidth;
                                }
                                if (style.TextIndent != 0)
                                {
                                    num4 += style.TextIndent;
                                }
                                num4 += Math.Ceiling(a);
                            }
                        }
                    }
                    width = Math.Max(width, num4);
                }
            }
            return flag;
        }

        /// <summary>
        /// Gets the span layout data.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="columnWidths">The column widths.</param>
        /// <param name="measure">The measurement.</param>
        /// <param name="onlyRow">if set to <c>true</c> [only row].</param>
        /// <param name="calcOverflow">if set to <c>true</c> [calc overflow].</param>
        /// <returns></returns>
        public static SpanLayoutData GetSpanLayoutData(Worksheet worksheet, SheetArea area, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, PartLayoutData columnWidths, IMeasureable measure, bool onlyRow, bool calcOverflow)
        {
            return GetSpanLayoutData(worksheet, area, rowStartIndex, rowEndIndex, columnStartIndex, columnEndIndex, columnWidths, measure, onlyRow, calcOverflow, null);
        }

        /// <summary>
        /// Gets the span layout data.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="rowStartIndex">Start index of the row.</param>
        /// <param name="rowEndIndex">End index of the row.</param>
        /// <param name="columnStartIndex">Start index of the column.</param>
        /// <param name="columnEndIndex">End index of the column.</param>
        /// <param name="columnWidths">The column widths.</param>
        /// <param name="measure">The measurement.</param>
        /// <param name="onlyRow">If set to <c>true</c>, [only row]</param>
        /// <param name="calcOverflow">if set to <c>true</c> [calc overflow].</param>
        /// <param name="state">The state.</param>
        /// <returns></returns>
        public static SpanLayoutData GetSpanLayoutData(Worksheet worksheet, SheetArea area, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex, PartLayoutData columnWidths, IMeasureable measure, bool onlyRow, bool calcOverflow, GcSheetSection.SheetState state)
        {
            if ((rowEndIndex < rowStartIndex) || (columnEndIndex < columnStartIndex))
            {
                return null;
            }
            bool flag = worksheet.IsRowSorted || onlyRow;
            bool isColumnSorted = worksheet.IsColumnSorted;
            SheetSpanModelBase spanModel = GetSpanModel(worksheet, area);
            SheetSpanModelBase mergeModel = new SheetSpanModel();
            SheetSpanModelBase overFlowModel = new SheetSpanModel();
            bool canCellOverflow = worksheet.Workbook.CanCellOverflow;
            FindAutoMergeRow(worksheet, spanModel, area, mergeModel, rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex);
            if (!onlyRow)
            {
                FindAutoMergeColumn(worksheet, spanModel, area, mergeModel, rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex);
            }
            if (area != SheetArea.Cells)
            {
                canCellOverflow = false;
            }
            if (calcOverflow && canCellOverflow)
            {
                FindOverFlow(worksheet, spanModel, mergeModel, area, overFlowModel, rowStartIndex, columnStartIndex, rowEndIndex, columnEndIndex, columnWidths, measure, state);
            }
            SpanLayoutData data = new SpanLayoutData();
            if (!isColumnSorted || !flag)
            {
                IEnumerator enumerator = spanModel.GetEnumerator(worksheet.IsRowSorted ? -1 : rowStartIndex, worksheet.IsColumnSorted ? -1 : columnStartIndex, worksheet.IsRowSorted ? -1 : ((rowEndIndex - rowStartIndex) + 1), worksheet.IsColumnSorted ? -1 : ((columnEndIndex - columnStartIndex) + 1));
                while (enumerator.MoveNext())
                {
                    CellRange current = (CellRange) enumerator.Current;
                    current = new CellRange(current.Row, current.Column, current.RowCount, current.ColumnCount);
                    if (current.Intersects(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1))
                    {
                        bool flag4 = false;
                        if (area == SheetArea.ColumnHeader)
                        {
                            flag4 = IsPaddingCell(worksheet.GetTag(current.Row, current.Column, area));
                        }
                        data.Add(current.Row, current.Column, (flag && !flag4) ? 1 : current.RowCount, isColumnSorted ? 1 : current.ColumnCount, true);
                    }
                }
            }
            bool flag5 = false;
            IEnumerator enumerator2 = mergeModel.GetEnumerator(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1);
            while (enumerator2.MoveNext())
            {
                CellRange range2 = (CellRange) enumerator2.Current;
                if ((!flag5 || (data.Find(range2.Row, range2.Column) == null)) && range2.Intersects(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1))
                {
                    data.Add(range2.Row, range2.Column, range2.RowCount, range2.ColumnCount);
                }
            }
            IEnumerator enumerator3 = overFlowModel.GetEnumerator(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1);
            while (enumerator3.MoveNext())
            {
                CellRange range3 = (CellRange) enumerator3.Current;
                if ((!flag5 || (data.Find(range3.Row, range3.Column) == null)) && range3.Intersects(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1))
                {
                    data.Add(range3.Row, range3.Column, range3.RowCount, range3.ColumnCount);
                }
            }
            return data;
        }

        /// <summary>
        /// Gets the span model.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public static SheetSpanModelBase GetSpanModel(Worksheet worksheet, SheetArea area)
        {
            return worksheet.FindSpanModel(area);
        }

        /// <summary>
        /// Gets the top side sheet area.
        /// </summary>
        /// <param name="area">The area.</param>
        /// <returns></returns>
        public static SheetArea GetTopSideSheetArea(SheetArea area)
        {
            switch (area)
            {
                case SheetArea.CornerHeader:
                case SheetArea.ColumnHeader:
                    return area;

                case SheetArea.Cells:
                    return SheetArea.ColumnHeader;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    return SheetArea.CornerHeader;
            }
            throw new ArgumentOutOfRangeException("area");
        }

        /// <summary>
        /// Determines whether [has fill effect] [the specified effect].
        /// </summary>
        /// <param name="effect">The effect.</param>
        /// <returns>
        /// <c>true</c> if [has fill effect] [the specified effect]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasFillEffect(Brush effect)
        {
            if (effect == null)
            {
                return false;
            }
            if (effect is SolidColorBrush)
            {
                SolidColorBrush sfe = effect as SolidColorBrush;
                bool _hasFillEffect = true;
                if (sfe.Color.A == 0)
                {
                    _hasFillEffect = false;
                }
                return _hasFillEffect;
            }
            return true;
        }

        /// <summary>
        /// Determines whether [is excel center alignment value] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if [is excel center alignment value] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExcelCenterAlignmentValue(object value)
        {
            if (value == null)
            {
                return false;
            }
            return ((value is bool) || (value is CalcError));
        }

        /// <summary>
        /// Determines whether [is excel right alignment value] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if [is excel right alignment value] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExcelRightAlignmentValue(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (!IsNumber(value) && !(value is DateTime))
            {
                return (value is TimeSpan);
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified value is a numeric data type.
        /// </summary>
        /// <param name="value">The value for which to determine the data type.</param>
        /// <returns>True if the value is numeric; otherwise false.</returns>
        public static bool IsNumber(object value)
        {
            return ((((((value is double) || (value is float)) || ((value is decimal) || (value is long))) || (((value is int) || (value is short)) || ((value is sbyte) || (value is ulong)))) || ((value is uint) || (value is ushort))) || (value is byte));
        }

        /// <summary>
        /// Determines whether [is padding cell] [the specified tag].
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <c>true</c> if [is padding cell] [the specified tag]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPaddingCell(object tag)
        {
            return ((tag != null) && tag.Equals(PaddingCellTag));
        }

        /// <summary>
        /// Determines whether [is padding footer cell] [the specified tag].
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>
        /// <c>true</c> if [is padding footer cell] [the specified tag]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPaddingFooterCell(object tag)
        {
            return ((tag != null) && tag.Equals(PaddingFooterCellTag));
        }

        /// <summary>
        /// Determines whether the specified value is zero.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// <c>true</c> if [is zero value] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsZeroValue(object value)
        {
            if (IsNumber(value))
            {
                try
                {
                    if (Convert.ToDouble(value) == 0.0)
                    {
                        return true;
                    }
                }
                catch
                {
                    try
                    {
                        if (Convert.ToDecimal(value) == 0.0M)
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Measures the string using the alignment.
        /// </summary>
        /// <param name="measure">The measurement.</param>
        /// <param name="text">The text.</param>
        /// <param name="font">The font.</param>
        /// <param name="alignment">The alignment.</param>
        /// <param name="w">The w.</param>
        /// <returns></returns>
        public static Windows.Foundation.Size MeasureStringByAlignment(IMeasureable measure, string text, Font font, ContentAlignment alignment, int w)
        {
            Windows.Foundation.Size size = measure.MeasureString(text, font, (alignment != null) && alignment.WordWrap, w);
            if ((size.Width == 0.0) || (size.Height == 0.0))
            {
                return new Windows.Foundation.Size(0.0, 0.0);
            }
            switch (((alignment == null) ? TextOrientation.TextHorizontal : alignment.TextOrientation))
            {
                case TextOrientation.TextHorizontal:
                case TextOrientation.TextHorizontalFlipped:
                    return size;

                case TextOrientation.TextVertical:
                case TextOrientation.TextVerticalFlipped:
                case TextOrientation.TextTopDown:
                case TextOrientation.TextTopDownRTL:
                    return new Windows.Foundation.Size(size.Height, size.Width);

                case TextOrientation.TextRotateCustom:
                    if (alignment.TextRotationAngle != 0.0)
                    {
                        Windows.Foundation.Rect rect = Shape.GetRotatedBounds(0.0, 0.0, size.Width, size.Height, alignment.TextRotationAngle, size.Width / 2.0, size.Height / 2.0);
                        size = new Windows.Foundation.Size(rect.Width, rect.Height);
                    }
                    return size;
            }
            throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// Creates a union with the specified rectangle.
        /// </summary>
        /// <param name="rect1">The first rectangle.</param>
        /// <param name="rect2">The second rectangle.</param>
        /// <returns></returns>
        public static Windows.Foundation.Rect Union(Windows.Foundation.Rect rect1, Windows.Foundation.Rect rect2)
        {
            if (rect1.IsEmpty)
            {
                rect1 = rect2;
                return rect1;
            }
            if (!rect2.IsEmpty)
            {
                double num = Math.Min(rect1.Left, rect2.Left);
                double num2 = Math.Min(rect1.Top, rect2.Top);
                if ((rect2.Width == double.PositiveInfinity) || (rect1.Width == double.PositiveInfinity))
                {
                    rect1.Width = double.PositiveInfinity;
                }
                else
                {
                    double num3 = Math.Max(rect1.Right, rect2.Right);
                    rect1.Width = Math.Max((double) (num3 - num), (double) 0.0);
                }
                if ((rect2.Height == double.PositiveInfinity) || (rect1.Height == double.PositiveInfinity))
                {
                    rect1.Height = double.PositiveInfinity;
                }
                else
                {
                    double num4 = Math.Max(rect1.Bottom, rect2.Bottom);
                    rect1.Height = Math.Max((double) (num4 - num2), (double) 0.0);
                }
                rect1.X = num;
                rect1.Y = num2;
            }
            return rect1;
        }

        /// <summary>
        /// Gets the HTML measure.
        /// </summary>
        /// <value>The HTML measure.</value>
        public static IMeasureable HtmlMeasure
        {
            get
            {
                if (htmlMeasure == null)
                {
                    htmlMeasure = new MeasureUtility(UnitType.Pixel, HtmlDefaultFont);
                }
                return htmlMeasure;
            }
        }

        /// <summary>
        /// Gets the page range regex.
        /// </summary>
        /// <value>The page range regex.</value>
        public static Regex PageRangeRegex
        {
            get
            {
                if (pageRangeRegex == null)
                {
                    pageRangeRegex = new Regex(string.Format(@"^(?<{0}>[0-9]*)\s*[-]?\s*(?<{1}>[0-9]*)$", (object[]) new object[] { "start", "end" }));
                }
                return pageRangeRegex;
            }
        }

        /// <summary>
        /// Internal only.
        /// Group Span Cache
        /// </summary>
        internal class GroupSpanCache : List<CellRange>
        {
        }

        /// <summary>
        /// Internal only.
        /// MeasureUtility
        /// </summary>
        internal class MeasureUtility : IMeasureable
        {
            TextBlock _internalTextBlock;
            readonly Font defaultFont;
            readonly UnitType unit;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Utilities.MeasureUtility" /> class.
            /// </summary>
            /// <param name="unit">The unit.</param>
            public MeasureUtility(UnitType unit)
            {
                this.defaultFont = new Font(DefaultStyleCollection.DefaultFontName, DefaultStyleCollection.DefaultFontSize);
                this.unit = unit;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.Utilities.MeasureUtility" /> class.
            /// </summary>
            /// <param name="unit">The unit.</param>
            /// <param name="defaultFont">The default font.</param>
            public MeasureUtility(UnitType unit, Font defaultFont) : this(unit)
            {
                this.defaultFont = defaultFont;
            }

            /// <summary>
            /// Measures the string without wrapping.
            /// </summary>
            /// <param name="str">The string.</param>
            /// <param name="font">The font.</param>
            /// <returns></returns>
            public Windows.Foundation.Size MeasureNoWrapString(string str, Font font)
            {
                return this.MeasureString(str, font, false, 0);
            }

            /// <summary>
            /// Measures the string.
            /// </summary>
            /// <param name="str">The string.</param>
            /// <param name="font">The font.</param>
            /// <param name="allowWrap">if set to <c>true</c> [allow wrap].</param>
            /// <param name="width">The width.</param>
            /// <returns></returns>
            public Windows.Foundation.Size MeasureString(string str, Font font, bool allowWrap, int width)
            {
                if (string.IsNullOrEmpty(str))
                {
                    return Windows.Foundation.Size.Empty;
                }
                if (font == null)
                {
                    font = this.defaultFont;
                }
                this.InternalTextBlock.Text = str;
                this.InternalTextBlock.TextWrapping = allowWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
                this.InternalTextBlock.Width = allowWrap ? UnitManager.ConvertTo((double)width, this.unit, UnitType.Pixel, UnitManager.Dpi) : 3.4028234663852886E+38;
                if (font.FontFamily == null)
                {
                    this.InternalTextBlock.FontFamily = this.defaultFont.FontFamily;
                }
                else
                {
                    this.InternalTextBlock.FontFamily = font.FontFamily;
                }
                this.InternalTextBlock.FontSize = (double)font.GetFontSize(UnitType.Pixel, (int)UnitManager.Dpi);
                this.InternalTextBlock.FontStretch = font.FontStretch;
                this.InternalTextBlock.FontStyle = font.FontStyle;
                this.InternalTextBlock.FontWeight = font.FontWeight;
                return new Windows.Foundation.Size(UnitManager.ConvertTo(this.InternalTextBlock.ActualWidth, UnitType.Pixel, this.unit, UnitManager.Dpi), UnitManager.ConvertTo(this.InternalTextBlock.ActualHeight, UnitType.Pixel, this.unit, UnitManager.Dpi));
            }

            /// <summary>
            /// Gets the default font.
            /// </summary>
            /// <value>The default font.</value>
            public Font DefaultFont
            {
                get { return  this.defaultFont; }
            }

            public TextBlock InternalTextBlock
            {
                get
                {
                    if (this._internalTextBlock == null)
                    {
                        this._internalTextBlock = new TextBlock();
                    }
                    return this._internalTextBlock;
                }
            }
        }
    }
}

