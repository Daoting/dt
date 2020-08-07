#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
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

        public static void BringCellToVisible(SheetView sheetView, int viewCellRow, int viewCellColumn)
        {
            if ((sheetView != null) && (sheetView.ActiveSheet != null))
            {
                ViewportInfo viewportInfo = sheetView.GetViewportInfo();
                int activeRowViewportIndex = sheetView.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = sheetView.GetActiveColumnViewportIndex();
                int rowViewportIndex = activeRowViewportIndex;
                int columnViewportIndex = activeColumnViewportIndex;
                if (activeRowViewportIndex == -1)
                {
                    if (viewCellRow >= sheetView.ActiveSheet.FrozenRowCount)
                    {
                        rowViewportIndex = 0;
                    }
                }
                else if ((activeRowViewportIndex == viewportInfo.RowViewportCount) && (viewCellRow < (sheetView.ActiveSheet.RowCount - sheetView.ActiveSheet.FrozenTrailingRowCount)))
                {
                    rowViewportIndex = viewportInfo.RowViewportCount - 1;
                }
                if (activeColumnViewportIndex == -1)
                {
                    if (viewCellColumn >= sheetView.ActiveSheet.FrozenColumnCount)
                    {
                        columnViewportIndex = 0;
                    }
                }
                else if ((activeColumnViewportIndex == viewportInfo.ColumnViewportCount) && (viewCellColumn < (sheetView.ActiveSheet.ColumnCount - sheetView.ActiveSheet.FrozenTrailingColumnCount)))
                {
                    columnViewportIndex = viewportInfo.ColumnViewportCount - 1;
                }
                BringCellToVisible(sheetView, rowViewportIndex, columnViewportIndex, viewCellRow, viewCellColumn);
            }
        }

        public static void BringCellToVisible(SheetView sheetView, int rowViewportIndex, int columnViewportIndex, int viewCellRow, int viewCellColumn)
        {
            if ((sheetView != null) && (sheetView.ActiveSheet != null))
            {
                var sheet = sheetView.ActiveSheet;
                double viewportWidth = sheetView.GetViewportWidth(columnViewportIndex);
                double viewportHeight = sheetView.GetViewportHeight(rowViewportIndex);
                int viewportLeftColumn = sheetView.GetViewportLeftColumn(columnViewportIndex);
                int viewportRightColumn = sheetView.GetViewportRightColumn(columnViewportIndex);
                int viewportTopRow = sheetView.GetViewportTopRow(rowViewportIndex);
                int viewportBottomRow = sheetView.GetViewportBottomRow(rowViewportIndex);
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
                if (sheetView.HorizontalScrollable)
                {
                    if (column < viewportLeftColumn)
                    {
                        sheetView.SetViewportLeftColumn(columnViewportIndex, column);
                    }
                    else if ((column > viewportLeftColumn) && (column <= viewportRightColumn))
                    {
                        if (((column + columnCount) - 1) >= viewportRightColumn)
                        {
                            double increasedWidth = GetColumnWidth(sheet, viewportLeftColumn, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) - viewportWidth;
                            if (increasedWidth > 0.0)
                            {
                                sheetView.SetViewportLeftColumn(columnViewportIndex, GetNewLeftColumn(sheet, viewportLeftColumn, increasedWidth));
                            }
                        }
                    }
                    else if (column > viewportRightColumn)
                    {
                        if (GetColumnWidth(sheet, column, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) >= viewportWidth)
                        {
                            sheetView.SetViewportLeftColumn(columnViewportIndex, column);
                        }
                        else
                        {
                            double num12 = GetColumnWidth(sheet, viewportLeftColumn, Math.Min((int) ((sheet.ColumnCount - sheet.FrozenTrailingColumnCount) - 1), (int) ((column + columnCount) - 1))) - viewportWidth;
                            if (num12 > 0.0)
                            {
                                sheetView.SetViewportLeftColumn(columnViewportIndex, GetNewLeftColumn(sheet, viewportLeftColumn, num12));
                            }
                        }
                    }
                }
                if (sheetView.VerticalScrollable)
                {
                    if (row < viewportTopRow)
                    {
                        sheetView.SetViewportTopRow(rowViewportIndex, row);
                    }
                    else if ((row > viewportTopRow) && (row <= viewportBottomRow))
                    {
                        if (((row + rowCount) - 1) >= viewportBottomRow)
                        {
                            double increasedHeight = GetRowHeight(sheet, viewportTopRow, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) - viewportHeight;
                            if (increasedHeight > 0.0)
                            {
                                sheetView.SetViewportTopRow(rowViewportIndex, GetNewTopRow(sheet, viewportTopRow, increasedHeight));
                            }
                        }
                    }
                    else if (row > viewportBottomRow)
                    {
                        if (GetRowHeight(sheet, row, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) >= viewportHeight)
                        {
                            sheetView.SetViewportTopRow(rowViewportIndex, row);
                        }
                        else
                        {
                            double num14 = GetRowHeight(sheet, viewportTopRow, Math.Min((int) ((sheet.RowCount - sheet.FrozenTrailingRowCount) - 1), (int) ((row + rowCount) - 1))) - viewportHeight;
                            if (num14 > 0.0)
                            {
                                sheetView.SetViewportTopRow(rowViewportIndex, GetNewTopRow(sheet, viewportTopRow, num14));
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

        public static bool ScrollToNextPageOfColumns(SheetView sheetView)
        {
            if (!sheetView.HorizontalScrollable || (sheetView.ActiveSheet == null))
            {
                return false;
            }
            int activeColumnViewportIndex = sheetView.GetActiveColumnViewportIndex();
            if (activeColumnViewportIndex == -1)
            {
                activeColumnViewportIndex = 0;
            }
            return ScrollToNextPageOfColumns(sheetView, activeColumnViewportIndex);
        }

        public static bool ScrollToNextPageOfColumns(SheetView sheetView, int columViewportIndex)
        {
            if (!sheetView.HorizontalScrollable || (sheetView.ActiveSheet == null))
            {
                return false;
            }
            if (columViewportIndex == -1)
            {
                columViewportIndex = 0;
            }
            int nextPageColumnCount = sheetView.GetNextPageColumnCount(columViewportIndex);
            if (nextPageColumnCount == 0)
            {
                return false;
            }
            int viewportLeftColumn = sheetView.GetViewportLeftColumn(columViewportIndex);
            sheetView.SetViewportLeftColumn(columViewportIndex, viewportLeftColumn + nextPageColumnCount);
            return true;
        }

        public static bool ScrollToNextPageOfRows(SheetView sheetView)
        {
            if (!sheetView.VerticalScrollable || (sheetView.ActiveSheet == null))
            {
                return false;
            }
            int activeRowViewportIndex = sheetView.GetActiveRowViewportIndex();
            if (activeRowViewportIndex == -1)
            {
                activeRowViewportIndex = 0;
            }
            return ScrollToNextPageOfRows(sheetView, activeRowViewportIndex);
        }

        public static bool ScrollToNextPageOfRows(SheetView sheetView, int rowViewportIndex)
        {
            if (!sheetView.VerticalScrollable || (sheetView.ActiveSheet == null))
            {
                return false;
            }
            if (rowViewportIndex == -1)
            {
                rowViewportIndex = 0;
            }
            int nextPageRowCount = sheetView.GetNextPageRowCount(rowViewportIndex);
            if (nextPageRowCount == 0)
            {
                return false;
            }
            int viewportTopRow = sheetView.GetViewportTopRow(rowViewportIndex);
            sheetView.SetViewportTopRow(rowViewportIndex, viewportTopRow + nextPageRowCount);
            return true;
        }

        public static bool ScrollToPreviousPageOfColumns(SheetView sheetView)
        {
            if (sheetView.HorizontalScrollable)
            {
                var worksheet = sheetView.ActiveSheet;
                if (worksheet != null)
                {
                    int activeColumnViewportIndex = sheetView.GetActiveColumnViewportIndex();
                    if (activeColumnViewportIndex == worksheet.GetViewportInfo().ColumnViewportCount)
                    {
                        activeColumnViewportIndex = worksheet.GetViewportInfo().ColumnViewportCount - 1;
                    }
                    return ScrollToPreviousPageOfColumns(sheetView, activeColumnViewportIndex);
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfColumns(SheetView sheetView, int columViewportIndex)
        {
            if (sheetView.HorizontalScrollable)
            {
                var worksheet = sheetView.ActiveSheet;
                if (worksheet != null)
                {
                    if (columViewportIndex == worksheet.GetViewportInfo().ColumnViewportCount)
                    {
                        columViewportIndex = worksheet.GetViewportInfo().ColumnViewportCount - 1;
                    }
                    int prePageColumnCount = sheetView.GetPrePageColumnCount(columViewportIndex);
                    if (prePageColumnCount == 0)
                    {
                        return false;
                    }
                    int viewportLeftColumn = sheetView.GetViewportLeftColumn(columViewportIndex);
                    sheetView.SetViewportLeftColumn(columViewportIndex, viewportLeftColumn - prePageColumnCount);
                    return true;
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfRows(SheetView sheetView)
        {
            if (sheetView.VerticalScrollable)
            {
                var worksheet = sheetView.ActiveSheet;
                if (worksheet != null)
                {
                    int activeRowViewportIndex = sheetView.GetActiveRowViewportIndex();
                    if (activeRowViewportIndex == worksheet.GetViewportInfo().RowViewportCount)
                    {
                        activeRowViewportIndex = worksheet.GetViewportInfo().RowViewportCount - 1;
                    }
                    return ScrollToPreviousPageOfRows(sheetView, activeRowViewportIndex);
                }
            }
            return false;
        }

        public static bool ScrollToPreviousPageOfRows(SheetView sheetView, int rowViewportIndex)
        {
            if (sheetView.VerticalScrollable)
            {
                var worksheet = sheetView.ActiveSheet;
                if (worksheet != null)
                {
                    if (rowViewportIndex == worksheet.GetViewportInfo().RowViewportCount)
                    {
                        rowViewportIndex = worksheet.GetViewportInfo().RowViewportCount - 1;
                    }
                    int prePageRowCount = sheetView.GetPrePageRowCount(rowViewportIndex);
                    if (prePageRowCount == 0)
                    {
                        return false;
                    }
                    int viewportTopRow = sheetView.GetViewportTopRow(rowViewportIndex);
                    sheetView.SetViewportTopRow(rowViewportIndex, viewportTopRow - prePageRowCount);
                    return true;
                }
            }
            return false;
        }
    }
}

