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
    internal class SpreadXTabularNavigator : TabularNavigator
    {
        internal Excel _excel;

        public SpreadXTabularNavigator(Excel p_excel)
        {
            _excel = p_excel;
        }

        public override void BringCellToVisible(TabularPosition position)
        {
            if ((!position.IsEmpty && (position.Area == SheetArea.Cells)) && (_excel.ActiveSheet != null))
            {
                NavigatorHelper.BringCellToVisible(_excel, position.Row, position.Column);
            }
        }

        public override bool CanHorizontalScroll(bool isBackward)
        {
            if (_excel == null)
            {
                return base.CanHorizontalScroll(isBackward);
            }
            if (!_excel.HorizontalScrollable)
            {
                return false;
            }
            int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
            if (isBackward)
            {
                return (_excel.GetNextPageColumnCount(activeColumnViewportIndex) > 0);
            }
            return (_excel.GetPrePageColumnCount(activeColumnViewportIndex) > 0);
        }

        public override bool CanMoveCurrentTo(TabularPosition cellPosition)
        {
            return (((((_excel.ActiveSheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < _excel.ActiveSheet.RowCount) && (cellPosition.Column >= 0))) && (((cellPosition.Column < _excel.ActiveSheet.ColumnCount) && _excel.ActiveSheet.Cells[cellPosition.Row, cellPosition.Column].ActualFocusable) && GetRowIsVisible(cellPosition.Row))) && GetColumnIsVisible(cellPosition.Column));
        }

        public override bool CanVerticalScroll(bool isBackward)
        {
            if (_excel == null)
            {
                return base.CanVerticalScroll(isBackward);
            }
            if (!_excel.VerticalScrollable)
            {
                return false;
            }
            int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
            if (isBackward)
            {
                return (_excel.GetNextPageRowCount(activeRowViewportIndex) > 0);
            }
            return (_excel.GetPrePageRowCount(activeRowViewportIndex) > 0);
        }

        public override bool GetColumnIsVisible(int columnIndex)
        {
            if ((_excel == null) || (_excel.ActiveSheet == null))
            {
                return base.GetColumnIsVisible(columnIndex);
            }
            return (_excel.ActiveSheet.GetActualColumnVisible(columnIndex, SheetArea.Cells) && (_excel.ActiveSheet.GetActualColumnWidth(columnIndex, SheetArea.Cells) > 0.0));
        }

        public override bool GetRowIsVisible(int rowIndex)
        {
            if ((_excel == null) || (_excel.ActiveSheet == null))
            {
                return base.GetRowIsVisible(rowIndex);
            }
            return (_excel.ActiveSheet.GetActualRowVisible(rowIndex, SheetArea.Cells) && (_excel.ActiveSheet.GetActualRowHeight(rowIndex, SheetArea.Cells) > 0.0));
        }

        public override bool IsMerged(TabularPosition position, out TabularRange range)
        {
            range = new TabularRange(position, 1, 1);
            if ((_excel.ActiveSheet != null) && (_excel.ActiveSheet.SpanModel != null))
            {
                CellRange range2 = _excel.ActiveSheet.SpanModel.Find(position.Row, position.Column);
                if (range2 != null)
                {
                    range = new TabularRange(position.Area, range2.Row, range2.Column, range2.RowCount, range2.ColumnCount);
                    return true;
                }
            }
            return false;
        }

        public override void ScrollToNextPageOfColumns()
        {
            NavigatorHelper.ScrollToNextPageOfColumns(_excel);
        }

        public override void ScrollToNextPageOfRows()
        {
            NavigatorHelper.ScrollToNextPageOfRows(_excel);
        }

        public override void ScrollToPreviousPageOfColumns()
        {
            NavigatorHelper.ScrollToPreviousPageOfColumns(_excel);
        }

        public override void ScrollToPreviousPageOfRows()
        {
            NavigatorHelper.ScrollToPreviousPageOfRows(_excel);
        }

        public override TabularRange ContentBounds
        {
            get
            {
                if ((_excel == null) || (_excel.ActiveSheet == null))
                {
                    return base.ContentBounds;
                }
                var worksheet = _excel.ActiveSheet;
                ViewportInfo viewportInfo = worksheet.GetViewportInfo();
                int activeRowViewportIndex = worksheet.GetActiveRowViewportIndex();
                int activeColumnViewportIndex = worksheet.GetActiveColumnViewportIndex();
                int row = 0;
                int column = 0;
                int rowCount = worksheet.RowCount;
                int columnCount = worksheet.ColumnCount;
                if (viewportInfo.RowViewportCount > 1)
                {
                    if (activeRowViewportIndex > 0)
                    {
                        row = worksheet.FrozenRowCount;
                        rowCount -= worksheet.FrozenRowCount;
                    }
                    if (activeRowViewportIndex < (viewportInfo.RowViewportCount - 1))
                    {
                        rowCount -= worksheet.FrozenTrailingRowCount;
                    }
                }
                if (viewportInfo.ColumnViewportCount > 1)
                {
                    if (activeColumnViewportIndex > 0)
                    {
                        column = worksheet.FrozenColumnCount;
                        columnCount -= worksheet.FrozenColumnCount;
                    }
                    if (activeColumnViewportIndex < (viewportInfo.ColumnViewportCount - 1))
                    {
                        columnCount -= worksheet.FrozenTrailingColumnCount;
                    }
                }
                return new TabularRange(SheetArea.Cells, row, column, rowCount, columnCount);
            }
        }

        public override TabularRange CurrentViewport
        {
            get
            {
                int activeColumnViewportIndex = _excel.GetActiveColumnViewportIndex();
                int activeRowViewportIndex = _excel.GetActiveRowViewportIndex();
                if (activeColumnViewportIndex == -1)
                {
                    activeColumnViewportIndex = 0;
                }
                else if (activeColumnViewportIndex == _excel.ActiveSheet.GetViewportInfo().ColumnViewportCount)
                {
                    activeColumnViewportIndex = _excel.ActiveSheet.GetViewportInfo().ColumnViewportCount - 1;
                }
                if (activeRowViewportIndex == -1)
                {
                    activeRowViewportIndex = 0;
                }
                else if (activeRowViewportIndex == _excel.ActiveSheet.GetViewportInfo().RowViewportCount)
                {
                    activeRowViewportIndex = _excel.ActiveSheet.GetViewportInfo().RowViewportCount - 1;
                }
                int viewportLeftColumn = _excel.GetViewportLeftColumn(activeColumnViewportIndex);
                int viewportRightColumn = _excel.GetViewportRightColumn(activeColumnViewportIndex);
                int viewportTopRow = _excel.GetViewportTopRow(activeRowViewportIndex);
                int viewportBottomRow = _excel.GetViewportBottomRow(activeRowViewportIndex);
                double viewportWidth = _excel.GetViewportWidth(activeColumnViewportIndex);
                double viewportHeight = _excel.GetViewportHeight(activeRowViewportIndex);
                if (NavigatorHelper.GetColumnWidth(_excel.ActiveSheet, viewportLeftColumn, viewportRightColumn) > viewportWidth)
                {
                    viewportRightColumn--;
                }
                if (NavigatorHelper.GetRowHeight(_excel.ActiveSheet, viewportTopRow, viewportBottomRow) > viewportHeight)
                {
                    viewportBottomRow--;
                }
                return new TabularRange(SheetArea.Cells, viewportTopRow, viewportLeftColumn, Math.Max(1, (viewportBottomRow - viewportTopRow) + 1), Math.Max(1, (viewportRightColumn - viewportLeftColumn) + 1));
            }
        }

        public override int TotalColumnCount
        {
            get { return  _excel.ActiveSheet.ColumnCount; }
        }

        public override int TotalRowCount
        {
            get { return  _excel.ActiveSheet.RowCount; }
        }
    }
}

