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
using System;
#endregion

namespace Dt.Cells.UI
{
    internal class NavigatorHelper
    {
        public static bool ActiveCellInSelection(Worksheet sheet)
        {
            if (sheet.Selections.Count == 0)
            {
                return false;
            }
            CellRange spanCell = sheet.GetSpanCell(sheet.ActiveRowIndex, sheet.ActiveColumnIndex);
            if (spanCell == null)
            {
                spanCell = new CellRange(sheet.ActiveRowIndex, sheet.ActiveColumnIndex, 1, 1);
            }
            foreach (CellRange range2 in sheet.Selections)
            {
                if (range2.Contains(spanCell))
                {
                    return true;
                }
                bool flag2 = (range2.Row == -1) && (range2.RowCount == -1);
                bool flag3 = (range2.Column == -1) && (range2.ColumnCount == -1);
                if ((flag2 || flag3) && range2.Contains(sheet.ActiveRowIndex, sheet.ActiveColumnIndex))
                {
                    return true;
                }
            }
            return false;
        }

        public static void BringCellToVisible(Excel excel, int viewCellRow, int viewCellColumn)
        {
            if ((excel != null) && (excel.ActiveSheet != null))
            {
                ViewportInfo viewportInfo = excel.GetViewportInfo();
                int activeRowViewportIndex = excel.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = excel.GetActiveColumnViewportIndex();
                int rowViewportIndex = activeRowViewportIndex;
                int columnViewportIndex = activeColumnViewportIndex;
                if (activeRowViewportIndex == -1)
                {
                    if (viewCellRow >= excel.ActiveSheet.FrozenRowCount)
                    {
                        rowViewportIndex = 0;
                    }
                }
                else if ((activeRowViewportIndex == viewportInfo.RowViewportCount) && (viewCellRow < (excel.ActiveSheet.RowCount - excel.ActiveSheet.FrozenTrailingRowCount)))
                {
                    rowViewportIndex = viewportInfo.RowViewportCount - 1;
                }
                if (activeColumnViewportIndex == -1)
                {
                    if (viewCellColumn >= excel.ActiveSheet.FrozenColumnCount)
                    {
                        columnViewportIndex = 0;
                    }
                }
                else if ((activeColumnViewportIndex == viewportInfo.ColumnViewportCount) && (viewCellColumn < (excel.ActiveSheet.ColumnCount - excel.ActiveSheet.FrozenTrailingColumnCount)))
                {
                    columnViewportIndex = viewportInfo.ColumnViewportCount - 1;
                }
                BringCellToVisible(excel, rowViewportIndex, columnViewportIndex, viewCellRow, viewCellColumn);
            }
        }

        public static void BringCellToVisible(Excel excel, int rowViewportIndex, int columnViewportIndex, int viewCellRow, int viewCellColumn)
        {
            if ((excel != null) && (excel.ActiveSheet != null))
            {
                var sheet = excel.ActiveSheet;
                double viewportWidth = excel.GetViewportWidth(columnViewportIndex);
                double viewportHeight = excel.GetViewportHeight(rowViewportIndex);
                int viewportLeftColumn = excel.GetViewportLeftColumn(columnViewportIndex);
                int viewportRightColumn = excel.GetViewportRightColumn(columnViewportIndex);
                int viewportTopRow = excel.GetViewportTopRow(rowViewportIndex);
                int viewportBottomRow = excel.GetViewportBottomRow(rowViewportIndex);
                int row = viewCellRow;
                int column = viewCellColumn;
                int rowCount = 1;
                int columnCount = 1;
                if (sheet.SpanModel != null)
                {
                    CellRange range = sheet.SpanModel.Find(row, column);
                    if (range != null)
                    {
                        row = range.Row;
                        column = range.Column;
                        rowCount = range.RowCount;
                        columnCount = range.ColumnCount;
                    }
                }
                if (excel.HorizontalScrollable)
                {
                    if (column < viewportLeftColumn)
                    {
                        excel.SetViewportLeftColumn(columnViewportIndex, column);
                    }
                    else if ((column > viewportLeftColumn) && (column <= viewportRightColumn))
                    {
                        if (((column + columnCount) - 1) >= viewportRightColumn)
                        {
                            double increasedWidth = GetColumnWidth(sheet, viewportLeftColumn, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) - viewportWidth;
                            if (increasedWidth > 0.0)
                            {
                                excel.SetViewportLeftColumn(columnViewportIndex, GetNewLeftColumn(sheet, viewportLeftColumn, increasedWidth));
                            }
                        }
                    }
                    else if (column > viewportRightColumn)
                    {
                        if (GetColumnWidth(sheet, column, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) >= viewportWidth)
                        {
                            excel.SetViewportLeftColumn(columnViewportIndex, column);
                        }
                        else
                        {
                            double num12 = GetColumnWidth(sheet, viewportLeftColumn, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) - viewportWidth;
                            if (num12 > 0.0)
                            {
                                excel.SetViewportLeftColumn(columnViewportIndex, GetNewLeftColumn(sheet, viewportLeftColumn, num12));
                            }
                        }
                    }
                }
                if (excel.VerticalScrollable)
                {
                    if (row < viewportTopRow)
                    {
                        excel.SetViewportTopRow(rowViewportIndex, row);
                    }
                    else if ((row > viewportTopRow) && (row <= viewportBottomRow))
                    {
                        if (((row + rowCount) - 1) >= viewportBottomRow)
                        {
                            double increasedHeight = GetRowHeight(sheet, viewportTopRow, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) - viewportHeight;
                            if (increasedHeight > 0.0)
                            {
                                excel.SetViewportTopRow(rowViewportIndex, GetNewTopRow(sheet, viewportTopRow, increasedHeight));
                            }
                        }
                    }
                    else if (row > viewportBottomRow)
                    {
                        if (GetRowHeight(sheet, row, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) >= viewportHeight)
                        {
                            excel.SetViewportTopRow(rowViewportIndex, row);
                        }
                        else
                        {
                            double num14 = GetRowHeight(sheet, viewportTopRow, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) - viewportHeight;
                            if (num14 > 0.0)
                            {
                                excel.SetViewportTopRow(rowViewportIndex, GetNewTopRow(sheet, viewportTopRow, num14));
                            }
                        }
                    }
                }
            }
        }

        public static double GetColumnWidth(Worksheet sheet, int leftColumn, int rightColumn)
        {
            double num = 0.0;
            for (int i = leftColumn; (i <= rightColumn) && (i < sheet.ColumnCount); i++)
            {
                num += sheet.Columns[i].ActualWidth * sheet.ZoomFactor;
            }
            return num;
        }

        public static int GetNewLeftColumn(Worksheet sheet, int oldLeftColumn, double increasedWidth)
        {
            double num = 0.0;
            for (int i = oldLeftColumn; (i < sheet.ColumnCount) && (num < increasedWidth); i++)
            {
                oldLeftColumn++;
                num += sheet.Columns[i].ActualWidth * sheet.ZoomFactor;
            }
            if (oldLeftColumn < sheet.ColumnCount)
            {
                return oldLeftColumn;
            }
            return (sheet.ColumnCount - 1);
        }

        public static int GetNewTopRow(Worksheet sheet, int oldTopRow, double increasedHeight)
        {
            double num = 0.0;
            for (int i = oldTopRow; (i <= sheet.RowCount) && (num < increasedHeight); i++)
            {
                oldTopRow++;
                num += sheet.Rows[i].ActualHeight * sheet.ZoomFactor;
            }
            if (oldTopRow < sheet.RowCount)
            {
                return oldTopRow;
            }
            return (sheet.RowCount - 1);
        }

        public static double GetRowHeight(Worksheet sheet, int topRow, int bottonRow)
        {
            double num = 0.0;
            for (int i = topRow; (i <= bottonRow) && (i < sheet.RowCount); i++)
            {
                num += sheet.Rows[i].ActualHeight * sheet.ZoomFactor;
            }
            return num;
        }

        public static bool ScrollToNextPageOfColumns(Excel excel)
        {
            if (!excel.HorizontalScrollable || (excel.ActiveSheet == null))
            {
                return false;
            }
            int activeColumnViewportIndex = excel.GetActiveColumnViewportIndex();
            if (activeColumnViewportIndex == -1)
            {
                activeColumnViewportIndex = 0;
            }
            return ScrollToNextPageOfColumns(excel, activeColumnViewportIndex);
        }

        public static bool ScrollToNextPageOfColumns(Excel excel, int columViewportIndex)
        {
            if (!excel.HorizontalScrollable || (excel.ActiveSheet == null))
            {
                return false;
            }
            if (columViewportIndex == -1)
            {
                columViewportIndex = 0;
            }
            int nextPageColumnCount = excel.GetNextPageColumnCount(columViewportIndex);
            if (nextPageColumnCount == 0)
            {
                return false;
            }
            int viewportLeftColumn = excel.GetViewportLeftColumn(columViewportIndex);
            excel.SetViewportLeftColumn(columViewportIndex, viewportLeftColumn + nextPageColumnCount);
            return true;
        }

        public static bool ScrollToNextPageOfRows(Excel excel)
        {
            if (!excel.VerticalScrollable || (excel.ActiveSheet == null))
            {
                return false;
            }
            int activeRowViewportIndex = excel.GetActiveRowViewportIndex();
            if (activeRowViewportIndex == -1)
            {
                activeRowViewportIndex = 0;
            }
            return ScrollToNextPageOfRows(excel, activeRowViewportIndex);
        }

        public static bool ScrollToNextPageOfRows(Excel excel, int rowViewportIndex)
        {
            if (!excel.VerticalScrollable || (excel.ActiveSheet == null))
            {
                return false;
            }
            if (rowViewportIndex == -1)
            {
                rowViewportIndex = 0;
            }
            int nextPageRowCount = excel.GetNextPageRowCount(rowViewportIndex);
            if (nextPageRowCount == 0)
            {
                return false;
            }
            int viewportTopRow = excel.GetViewportTopRow(rowViewportIndex);
            excel.SetViewportTopRow(rowViewportIndex, viewportTopRow + nextPageRowCount);
            return true;
        }

        public static bool ScrollToPreviousPageOfColumns(Excel excel)
        {
            if (excel.HorizontalScrollable)
            {
                var worksheet = excel.ActiveSheet;
                if (worksheet != null)
                {
                    int activeColumnViewportIndex = excel.GetActiveColumnViewportIndex();
                    if (activeColumnViewportIndex == worksheet.GetViewportInfo().ColumnViewportCount)
                    {
                        activeColumnViewportIndex = worksheet.GetViewportInfo().ColumnViewportCount - 1;
                    }
                    return ScrollToPreviousPageOfColumns(excel, activeColumnViewportIndex);
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfColumns(Excel excel, int columViewportIndex)
        {
            if (excel.HorizontalScrollable)
            {
                var worksheet = excel.ActiveSheet;
                if (worksheet != null)
                {
                    if (columViewportIndex == worksheet.GetViewportInfo().ColumnViewportCount)
                    {
                        columViewportIndex = worksheet.GetViewportInfo().ColumnViewportCount - 1;
                    }
                    int prePageColumnCount = excel.GetPrePageColumnCount(columViewportIndex);
                    if (prePageColumnCount == 0)
                    {
                        return false;
                    }
                    int viewportLeftColumn = excel.GetViewportLeftColumn(columViewportIndex);
                    excel.SetViewportLeftColumn(columViewportIndex, viewportLeftColumn - prePageColumnCount);
                    return true;
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfRows(Excel excel)
        {
            if (excel.VerticalScrollable)
            {
                var worksheet = excel.ActiveSheet;
                if (worksheet != null)
                {
                    int activeRowViewportIndex = excel.GetActiveRowViewportIndex();
                    if (activeRowViewportIndex == worksheet.GetViewportInfo().RowViewportCount)
                    {
                        activeRowViewportIndex = worksheet.GetViewportInfo().RowViewportCount - 1;
                    }
                    return ScrollToPreviousPageOfRows(excel, activeRowViewportIndex);
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfRows(Excel excel, int rowViewportIndex)
        {
            if (excel.VerticalScrollable)
            {
                var worksheet = excel.ActiveSheet;
                if (worksheet != null)
                {
                    if (rowViewportIndex == worksheet.GetViewportInfo().RowViewportCount)
                    {
                        rowViewportIndex = worksheet.GetViewportInfo().RowViewportCount - 1;
                    }
                    int prePageRowCount = excel.GetPrePageRowCount(rowViewportIndex);
                    if (prePageRowCount == 0)
                    {
                        return false;
                    }
                    int viewportTopRow = excel.GetViewportTopRow(rowViewportIndex);
                    excel.SetViewportTopRow(rowViewportIndex, viewportTopRow - prePageRowCount);
                    return true;
                }
            }
            return false;
        }
    }
}

