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
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
#endregion

namespace Dt.Cells.Data
{
    internal static class HtmlUtilities
    {
        /// <summary>
        /// Gets the column widths.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static List<double> GetColumnWidths(Worksheet worksheet, SheetArea area, int startIndex, int count)
        {
            List<double> list = new List<double>();
            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (!worksheet.GetActualColumnVisible(i, area))
                {
                    list.Add(0.0);
                }
                else
                {
                    list.Add(worksheet.GetColumnWidth(i, area));
                }
            }
            return list;
        }

        /// <summary>
        /// Gets the row heights.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="area">The area.</param>
        /// <param name="startIndex">The start index.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        public static List<double> GetRowHeights(Worksheet worksheet, SheetArea area, int startIndex, int count)
        {
            List<double> list = new List<double>();
            for (int i = startIndex; i < (startIndex + count); i++)
            {
                if (!worksheet.GetActualRowVisible(i, area))
                {
                    list.Add(0.0);
                }
                else
                {
                    list.Add(worksheet.GetRowHeight(i, area));
                }
            }
            return list;
        }

        public static SpanLayoutData GetSpanLayoutData(Worksheet worksheet, SheetArea area, int rowStartIndex, int rowEndIndex, int columnStartIndex, int columnEndIndex)
        {
            if ((rowEndIndex < rowStartIndex) || (columnEndIndex < columnStartIndex))
            {
                return null;
            }
            SpanLayoutData data = new SpanLayoutData();
            SheetSpanModel spanModel = null;
            switch (area)
            {
                case SheetArea.Cells:
                    spanModel = worksheet.SpanModel;
                    break;

                case (SheetArea.CornerHeader | SheetArea.RowHeader):
                    spanModel = worksheet.RowHeaderSpanModel;
                    break;

                case SheetArea.ColumnHeader:
                    spanModel = worksheet.ColumnHeaderSpanModel;
                    break;

                default:
                    throw new ArgumentException("area");
            }
            if (spanModel != null)
            {
                IEnumerator enumerator = spanModel.GetEnumerator(rowStartIndex, columnStartIndex, (rowEndIndex - rowStartIndex) + 1, (columnEndIndex - columnStartIndex) + 1);
                while (enumerator.MoveNext())
                {
                    CellRange current = (CellRange)enumerator.Current;
                    data.Add(current.Row, current.Column, current.RowCount, current.ColumnCount, true);
                }
            }
            return data;
        }

        static bool IsExcelRightAlignmentValue(object value)
        {
            return ((((((value is DateTime) || (value is double)) || ((value is float) || (value is decimal))) || (((value is long) || (value is int)) || ((value is short) || (value is sbyte)))) || (((value is ulong) || (value is uint)) || (value is ushort))) || (value is byte));
        }

        public static HorizontalAlignment ToHorizontalAlignment(this Cell cell)
        {
            switch (cell.ActualHorizontalAlignment)
            {
                case CellHorizontalAlignment.Left:
                    return HorizontalAlignment.Left;

                case CellHorizontalAlignment.Center:
                    return HorizontalAlignment.Center;

                case CellHorizontalAlignment.Right:
                    return HorizontalAlignment.Right;

                case CellHorizontalAlignment.General:
                    {
                        object val = cell.Value;
                        if (val == null)
                            return HorizontalAlignment.Stretch;

                        if (IsExcelRightAlignmentValue(val))
                            return HorizontalAlignment.Right;

                        if (val is bool)
                            return HorizontalAlignment.Center;
                        return HorizontalAlignment.Stretch;
                    }
            }
            return HorizontalAlignment.Stretch;
        }
    }
}

