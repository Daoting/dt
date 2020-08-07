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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal class SpreadXTabularNavigator : TabularNavigator
    {
        internal SheetView _sheetView;

        public SpreadXTabularNavigator(SheetView sheetView)
        {
            _sheetView = sheetView;
        }

        public override void BringCellToVisible(TabularPosition position)
        {
            if ((!position.IsEmpty && (position.Area == SheetArea.Cells)) && (_sheetView.ActiveSheet != null))
            {
                NavigatorHelper.BringCellToVisible(_sheetView, position.Row, position.Column);
            }
        }

        public override bool CanHorizontalScroll(bool isBackward)
        {
            if (_sheetView == null)
            {
                return base.CanHorizontalScroll(isBackward);
            }
            if (!_sheetView.HorizontalScrollable)
            {
                return false;
            }
            int activeColumnViewportIndex = _sheetView.GetActiveColumnViewportIndex();
            if (isBackward)
            {
                return (_sheetView.GetNextPageColumnCount(activeColumnViewportIndex) > 0);
            }
            return (_sheetView.GetPrePageColumnCount(activeColumnViewportIndex) > 0);
        }

        public override bool CanMoveCurrentTo(TabularPosition cellPosition)
        {
            return (((((_sheetView.ActiveSheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < _sheetView.ActiveSheet.RowCount) && (cellPosition.Column >= 0))) && (((cellPosition.Column < _sheetView.ActiveSheet.ColumnCount) && _sheetView.ActiveSheet.Cells[cellPosition.Row, cellPosition.Column].ActualFocusable) && GetRowIsVisible(cellPosition.Row))) && GetColumnIsVisible(cellPosition.Column));
        }

        public override bool CanVerticalScroll(bool isBackward)
        {
            if (_sheetView == null)
            {
                return base.CanVerticalScroll(isBackward);
            }
            if (!_sheetView.VerticalScrollable)
            {
                return false;
            }
            int activeRowViewportIndex = _sheetView.GetActiveRowViewportIndex();
            if (isBackward)
            {
                return (_sheetView.GetNextPageRowCount(activeRowViewportIndex) > 0);
            }
            return (_sheetView.GetPrePageRowCount(activeRowViewportIndex) > 0);
        }

        public override bool GetColumnIsVisible(int columnIndex)
        {
            if ((_sheetView == null) || (_sheetView.ActiveSheet == null))
            {
                return base.GetColumnIsVisible(columnIndex);
            }
            return (_sheetView.ActiveSheet.GetActualColumnVisible(columnIndex, SheetArea.Cells) && (_sheetView.ActiveSheet.GetActualColumnWidth(columnIndex, SheetArea.Cells) > 0.0));
        }

        public override bool GetRowIsVisible(int rowIndex)
        {
            if ((_sheetView == null) || (_sheetView.ActiveSheet == null))
            {
                return base.GetRowIsVisible(rowIndex);
            }
            return (_sheetView.ActiveSheet.GetActualRowVisible(rowIndex, SheetArea.Cells) && (_sheetView.ActiveSheet.GetActualRowHeight(rowIndex, SheetArea.Cells) > 0.0));
        }

        public override bool IsMerged(TabularPosition position, out TabularRange range)
        {
            range = new TabularRange(position, 1, 1);
            if ((_sheetView.ActiveSheet != null) && (_sheetView.ActiveSheet.SpanModel != null))
            {
                CellRange range2 = _sheetView.ActiveSheet.SpanModel.Find(position.Row, position.Column);
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
            NavigatorHelper.ScrollToNextPageOfColumns(_sheetView);
        }

        public override void ScrollToNextPageOfRows()
        {
            NavigatorHelper.ScrollToNextPageOfRows(_sheetView);
        }

        public override void ScrollToPreviousPageOfColumns()
        {
            NavigatorHelper.ScrollToPreviousPageOfColumns(_sheetView);
        }

        public override void ScrollToPreviousPageOfRows()
        {
            NavigatorHelper.ScrollToPreviousPageOfRows(_sheetView);
        }

        public override TabularRange ContentBounds
        {
            get
            {
                if ((_sheetView == null) || (_sheetView.ActiveSheet == null))
                {
                    return base.ContentBounds;
                }
                var worksheet = _sheetView.ActiveSheet;
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
                int activeColumnViewportIndex = _sheetView.GetActiveColumnViewportIndex();
                int activeRowViewportIndex = _sheetView.GetActiveRowViewportIndex();
                if (activeColumnViewportIndex == -1)
                {
                    activeColumnViewportIndex = 0;
                }
                else if (activeColumnViewportIndex == _sheetView.ActiveSheet.GetViewportInfo().ColumnViewportCount)
                {
                    activeColumnViewportIndex = _sheetView.ActiveSheet.GetViewportInfo().ColumnViewportCount - 1;
                }
                if (activeRowViewportIndex == -1)
                {
                    activeRowViewportIndex = 0;
                }
                else if (activeRowViewportIndex == _sheetView.ActiveSheet.GetViewportInfo().RowViewportCount)
                {
                    activeRowViewportIndex = _sheetView.ActiveSheet.GetViewportInfo().RowViewportCount - 1;
                }
                int viewportLeftColumn = _sheetView.GetViewportLeftColumn(activeColumnViewportIndex);
                int viewportRightColumn = _sheetView.GetViewportRightColumn(activeColumnViewportIndex);
                int viewportTopRow = _sheetView.GetViewportTopRow(activeRowViewportIndex);
                int viewportBottomRow = _sheetView.GetViewportBottomRow(activeRowViewportIndex);
                double viewportWidth = _sheetView.GetViewportWidth(activeColumnViewportIndex);
                double viewportHeight = _sheetView.GetViewportHeight(activeRowViewportIndex);
                if (NavigatorHelper.GetColumnWidth(_sheetView.ActiveSheet, viewportLeftColumn, viewportRightColumn) > viewportWidth)
                {
                    viewportRightColumn--;
                }
                if (NavigatorHelper.GetRowHeight(_sheetView.ActiveSheet, viewportTopRow, viewportBottomRow) > viewportHeight)
                {
                    viewportBottomRow--;
                }
                return new TabularRange(SheetArea.Cells, viewportTopRow, viewportLeftColumn, Math.Max(1, (viewportBottomRow - viewportTopRow) + 1), Math.Max(1, (viewportRightColumn - viewportLeftColumn) + 1));
            }
        }

        public override int TotalColumnCount
        {
            get { return  _sheetView.ActiveSheet.ColumnCount; }
        }

        public override int TotalRowCount
        {
            get { return  _sheetView.ActiveSheet.RowCount; }
        }
    }
}

