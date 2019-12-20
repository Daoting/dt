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
            this._sheetView = sheetView;
        }

        public override void BringCellToVisible(TabularPosition position)
        {
            if ((!position.IsEmpty && (position.Area == SheetArea.Cells)) && (this._sheetView.Worksheet != null))
            {
                NavigatorHelper.BringCellToVisible(this._sheetView, position.Row, position.Column);
            }
        }

        public override bool CanHorizontalScroll(bool isBackward)
        {
            if (this._sheetView == null)
            {
                return base.CanHorizontalScroll(isBackward);
            }
            if (!this._sheetView.HorizontalScrollable)
            {
                return false;
            }
            int activeColumnViewportIndex = this._sheetView.GetActiveColumnViewportIndex();
            if (isBackward)
            {
                return (this._sheetView.GetNextPageColumnCount(activeColumnViewportIndex) > 0);
            }
            return (this._sheetView.GetPrePageColumnCount(activeColumnViewportIndex) > 0);
        }

        public override bool CanMoveCurrentTo(TabularPosition cellPosition)
        {
            return (((((this._sheetView.Worksheet != null) && (cellPosition.Row >= 0)) && ((cellPosition.Row < this._sheetView.Worksheet.RowCount) && (cellPosition.Column >= 0))) && (((cellPosition.Column < this._sheetView.Worksheet.ColumnCount) && this._sheetView.Worksheet.Cells[cellPosition.Row, cellPosition.Column].ActualFocusable) && this.GetRowIsVisible(cellPosition.Row))) && this.GetColumnIsVisible(cellPosition.Column));
        }

        public override bool CanVerticalScroll(bool isBackward)
        {
            if (this._sheetView == null)
            {
                return base.CanVerticalScroll(isBackward);
            }
            if (!this._sheetView.VerticalScrollable)
            {
                return false;
            }
            int activeRowViewportIndex = this._sheetView.GetActiveRowViewportIndex();
            if (isBackward)
            {
                return (this._sheetView.GetNextPageRowCount(activeRowViewportIndex) > 0);
            }
            return (this._sheetView.GetPrePageRowCount(activeRowViewportIndex) > 0);
        }

        public override bool GetColumnIsVisible(int columnIndex)
        {
            if ((this._sheetView == null) || (this._sheetView.Worksheet == null))
            {
                return base.GetColumnIsVisible(columnIndex);
            }
            return (this._sheetView.Worksheet.GetActualColumnVisible(columnIndex, SheetArea.Cells) && (this._sheetView.Worksheet.GetActualColumnWidth(columnIndex, SheetArea.Cells) > 0.0));
        }

        public override bool GetRowIsVisible(int rowIndex)
        {
            if ((this._sheetView == null) || (this._sheetView.Worksheet == null))
            {
                return base.GetRowIsVisible(rowIndex);
            }
            return (this._sheetView.Worksheet.GetActualRowVisible(rowIndex, SheetArea.Cells) && (this._sheetView.Worksheet.GetActualRowHeight(rowIndex, SheetArea.Cells) > 0.0));
        }

        public override bool IsMerged(TabularPosition position, out TabularRange range)
        {
            range = new TabularRange(position, 1, 1);
            if ((this._sheetView.Worksheet != null) && (this._sheetView.Worksheet.SpanModel != null))
            {
                CellRange range2 = this._sheetView.Worksheet.SpanModel.Find(position.Row, position.Column);
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
            NavigatorHelper.ScrollToNextPageOfColumns(this._sheetView);
        }

        public override void ScrollToNextPageOfRows()
        {
            NavigatorHelper.ScrollToNextPageOfRows(this._sheetView);
        }

        public override void ScrollToPreviousPageOfColumns()
        {
            NavigatorHelper.ScrollToPreviousPageOfColumns(this._sheetView);
        }

        public override void ScrollToPreviousPageOfRows()
        {
            NavigatorHelper.ScrollToPreviousPageOfRows(this._sheetView);
        }

        public override TabularRange ContentBounds
        {
            get
            {
                if ((this._sheetView == null) || (this._sheetView.Worksheet == null))
                {
                    return base.ContentBounds;
                }
                Worksheet worksheet = this._sheetView.Worksheet;
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
                int activeColumnViewportIndex = this._sheetView.GetActiveColumnViewportIndex();
                int activeRowViewportIndex = this._sheetView.GetActiveRowViewportIndex();
                if (activeColumnViewportIndex == -1)
                {
                    activeColumnViewportIndex = 0;
                }
                else if (activeColumnViewportIndex == this._sheetView.Worksheet.GetViewportInfo().ColumnViewportCount)
                {
                    activeColumnViewportIndex = this._sheetView.Worksheet.GetViewportInfo().ColumnViewportCount - 1;
                }
                if (activeRowViewportIndex == -1)
                {
                    activeRowViewportIndex = 0;
                }
                else if (activeRowViewportIndex == this._sheetView.Worksheet.GetViewportInfo().RowViewportCount)
                {
                    activeRowViewportIndex = this._sheetView.Worksheet.GetViewportInfo().RowViewportCount - 1;
                }
                int viewportLeftColumn = this._sheetView.GetViewportLeftColumn(activeColumnViewportIndex);
                int viewportRightColumn = this._sheetView.GetViewportRightColumn(activeColumnViewportIndex);
                int viewportTopRow = this._sheetView.GetViewportTopRow(activeRowViewportIndex);
                int viewportBottomRow = this._sheetView.GetViewportBottomRow(activeRowViewportIndex);
                double viewportWidth = this._sheetView.GetViewportWidth(activeColumnViewportIndex);
                double viewportHeight = this._sheetView.GetViewportHeight(activeRowViewportIndex);
                if (NavigatorHelper.GetColumnWidth(this._sheetView.Worksheet, viewportLeftColumn, viewportRightColumn) > viewportWidth)
                {
                    viewportRightColumn--;
                }
                if (NavigatorHelper.GetRowHeight(this._sheetView.Worksheet, viewportTopRow, viewportBottomRow) > viewportHeight)
                {
                    viewportBottomRow--;
                }
                return new TabularRange(SheetArea.Cells, viewportTopRow, viewportLeftColumn, Math.Max(1, (viewportBottomRow - viewportTopRow) + 1), Math.Max(1, (viewportRightColumn - viewportLeftColumn) + 1));
            }
        }

        public override int TotalColumnCount
        {
            get { return  this._sheetView.Worksheet.ColumnCount; }
        }

        public override int TotalRowCount
        {
            get { return  this._sheetView.Worksheet.RowCount; }
        }
    }
}

