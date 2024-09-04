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
using System.Collections.Generic;
using Windows.Foundation;
#endregion

namespace Dt.Cells.UI
{
    internal class PrintPaginator
    {
        double _horZoomFactor = 1.0;
        double _verZoomFactor = 1.0;
        Excel _excel;
        Worksheet _sheet;
        Size _pageSize;
        PrintInfo _info;
        List<PageInfo> _columnPages;
        int _pageCount = -1;
        List<PageInfo> _rowPages;

        public PrintPaginator(Excel p_excel, PrintInfo p_printSettings, Size p_paperSize)
        {
            _excel = p_excel;
            _sheet = _excel.ActiveSheet;
            _pageSize = p_paperSize;
            _info = p_printSettings;
        }

        public SheetPageInfo GetPage(int p_pageIndex)
        {
            int num = 0;
            int num2 = 0;
            if ((_info.PageOrder == PrintPageOrder.DownThenOver)
                || (_info.PageOrder == PrintPageOrder.Auto && VerticalPageCount >= HorizontalPageCount))
            {
                num = p_pageIndex % _rowPages.Count;
                num2 = p_pageIndex / _rowPages.Count;
            }
            else
            {
                num2 = p_pageIndex % _columnPages.Count;
                num = p_pageIndex / _columnPages.Count;
            }
            return new SheetPageInfo { RowPageIndex = num, ColumnPageIndex = num2, RowPage = _rowPages[num], ColumnPage = _columnPages[num2] };
        }

        public SheetPageInfo GetPage(int p_rowIndex, int p_colIndex)
        {
            return new SheetPageInfo { RowPageIndex = p_rowIndex, ColumnPageIndex = p_colIndex, RowPage = _rowPages[p_rowIndex], ColumnPage = _columnPages[p_colIndex] };
        }

        /// <summary>
        /// 区域打印时的位置
        /// </summary>
        /// <param name="p_info"></param>
        /// <returns></returns>
        public Point GetPageLocation(SheetPageInfo p_info)
        {
            double x = 0;
            double y = 0;
            if (_sheet.RowHeader.IsVisible)
            {
                for (int j = 0; j < _sheet.RowHeader.ColumnCount; j++)
                {
                    x += GetColumnWidth(j, SheetArea.RowHeader);
                }
            }

            for (int i = 0; i < p_info.ColumnPage.ItemStart; i++)
            {
                x += GetColumnWidth(i, SheetArea.Cells);
            }

            if (_sheet.ColumnHeader.IsVisible)
            {
                for (int j = 0; j < _sheet.ColumnHeader.RowCount; j++)
                {
                    y += GetRowHeight(j, SheetArea.ColumnHeader);
                }
            }

            for (int i = 0; i < p_info.RowPage.ItemStart; i++)
            {
                y += GetRowHeight(i, SheetArea.Cells);
            }
            return new Point(x, y);
        }
        
        /// <summary>
        /// 执行分页
        /// </summary>
        public void Paginate()
        {
            if (ContentWidth <= 0.0 || ContentHeight <= 0.0)
            {
                _pageCount = 0;
                return;
            }

            if (_info.FitPagesTall == -1 && _info.FitPagesWide == -1)
            {
                HorZoomFactor = _info.ZoomFactor;
                VerZoomFactor = _info.ZoomFactor;
            }
            PaginateCore();
            if ((_info.FitPagesTall >= 1) || (_info.FitPagesWide >= 1))
            {
                double totalContentWidth = 0.0;
                double totalContentHeight = 0.0;
                for (int i = 0; i < _columnPages.Count; i++)
                {
                    totalContentWidth += _columnPages[i].ContentSize;
                }
                for (int j = 0; j < _rowPages.Count; j++)
                {
                    totalContentHeight += _rowPages[j].ContentSize;
                }
                if ((_info.FitPagesTall < VerticalPageCount) && (_info.FitPagesTall >= 1))
                {
                    VerticalFitPaginate(totalContentHeight, VerticalPageCount);
                }
                if ((_info.FitPagesWide < HorizontalPageCount) && (_info.FitPagesWide >= 1))
                {
                    HorizontalFitPaginate(totalContentWidth, HorizontalPageCount);
                }
                _pageCount = _rowPages.Count * _columnPages.Count;
            }
        }

        public int HorizontalPageCount
        {
            get { return _columnPages.Count; }
        }

        public double HorZoomFactor
        {
            get { return _horZoomFactor; }
            set { _horZoomFactor = value; }
        }

        public int PageCount
        {
            get { return _pageCount; }
        }

        public int VerticalPageCount
        {
            get { return _rowPages.Count; }
        }

        public double VerZoomFactor
        {
            get { return _verZoomFactor; }
            set { _verZoomFactor = value; }
        }

        void PaginateCore()
        {
            VerticalPaginate();
            HorizontalPaginate();
            _pageCount = _rowPages.Count * _columnPages.Count;
        }

        void VerticalPaginate()
        {
            int rowStart = (_info.RowStart != -1) ? _info.RowStart : 0;
            int rowEnd = (_info.RowEnd != -1) ? _info.RowEnd : _info.UseMax ? _sheet.GetLastDirtyRow() : (_sheet.RowCount - 1);
            _rowPages = VerticalPaginateCore(rowStart, rowEnd, _info.RepeatRowStart, _info.RepeatRowEnd, SheetArea.Cells, ContentHeight);
            ModifyPageInfo(_rowPages, rowStart);
        }

        void VerticalFitPaginate(double totalContentHeight, int vPageCount)
        {
            double num = 1.0;
            if (_info.FitPagesTall < vPageCount)
            {
                num = ((double)_info.FitPagesTall) / ((double)vPageCount);
            }
            double num2 = (_info.FitPagesTall * ContentHeight) / totalContentHeight;
            double num3 = num2;
            int num4 = 15;
            double num5 = (num2 - num) / ((double)num4);
            for (int i = 0; i <= num4; i++)
            {
                VerZoomFactor = num3;
                VerticalPaginate();
                if (_rowPages.Count == _info.FitPagesTall)
                {
                    return;
                }
                num3 -= num5;
            }
        }

        List<PageInfo> VerticalPaginateCore(int rowStart, int rowEnd, int repeatRowStart, int repetRowEnd, SheetArea sheetArea, double pageHeight)
        {
            bool repeat = repeatRowStart < rowStart;
            double curHeight = 0.0;
            double contentHeight = 0.0;
            double repeatHeight = 0.0;
            double leaveHeight = pageHeight;
            double yEnd = 0.0;
            PageInfo info = null;
            List<PageInfo> list = new List<PageInfo>();
            if (rowStart > rowEnd)
            {
                rowStart = rowEnd;
            }

            for (int i = rowStart; i <= rowEnd; i++)
            {
                // 首页且显示列头，扣除列头高度
                if (i == rowStart && _sheet.ColumnHeader.IsVisible)
                {
                    for (int k = 0; k < _sheet.ColumnHeader.RowCount; k++)
                    {
                        leaveHeight -= _sheet.GetActualRowHeight(k, SheetArea.ColumnHeader);
                    }
                }

                // 需要重复每页顶部时，扣除其高度
                if (repeat
                    && sheetArea == SheetArea.Cells
                    && repeatRowStart != -1
                    && repeatRowStart < i)
                {

                    int leaveRow = repetRowEnd;
                    if (leaveRow >= i)
                    {
                        leaveRow = i - 1;
                    }
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.RepeatItemStart = repeatRowStart;
                    info.RepeatItemEnd = leaveRow;
                    for (int j = repeatRowStart; j <= leaveRow; j++)
                    {
                        double rowHeight = GetRowHeight(j, sheetArea);
                        repeatHeight += rowHeight;
                    }
                    leaveHeight -= repeatHeight;
                    repeat = false;
                }

                double rh = GetRowHeight(i, sheetArea);
                curHeight += rh;
                yEnd += rh;
                if ((curHeight > leaveHeight) || (curHeight == leaveHeight))
                {
                    if (curHeight > leaveHeight)
                    {
                        contentHeight = (curHeight - GetRowHeight(i, sheetArea)) + repeatHeight;
                        yEnd = (yEnd - GetRowHeight(i, sheetArea)) + repeatHeight;
                        i--;
                    }
                    else
                    {
                        contentHeight = curHeight + repeatHeight;
                        yEnd += repeatHeight;
                    }

                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.ItemEnd = i;
                    info.ContentSize = contentHeight;
                    info.YEnd = yEnd;
                    list.Add(info);
                    info = null;
                    curHeight = 0.0;
                    contentHeight = 0.0;
                    repeatHeight = 0.0;
                    leaveHeight = pageHeight;
                    repeat = true;
                }
                else if (i == rowEnd && curHeight > 0)
                {
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    contentHeight = curHeight + repeatHeight;
                    info.ItemEnd = i;
                    info.ContentSize = contentHeight;
                    info.YEnd = yEnd;
                    list.Add(info);
                    info = null;
                    curHeight = 0.0;
                    contentHeight = 0.0;
                    repeatHeight = 0.0;
                    leaveHeight = pageHeight;
                    repeat = true;
                }
            }
            return list;
        }

        void HorizontalPaginate()
        {
            int columnStart = (_info.ColumnStart != -1) ? _info.ColumnStart : 0;
            int columnEnd = (_info.ColumnEnd != -1) ? _info.ColumnEnd : _info.UseMax ? _sheet.GetLastDirtyColumn() : (_sheet.ColumnCount - 1);
            _columnPages = HorizontalPaginatorCore(columnStart, columnEnd, _info.RepeatColumnStart, _info.RepeatColumnEnd, SheetArea.Cells, ContentWidth);
            ModifyPageInfo(_columnPages, columnStart);
        }

        void HorizontalFitPaginate(double totalContentWidth, int hPageCount)
        {
            double num = 1.0;
            if (_info.FitPagesWide < hPageCount)
            {
                num = ((double)_info.FitPagesWide) / ((double)hPageCount);
            }
            double num2 = (_info.FitPagesWide * ContentWidth) / totalContentWidth;
            double num3 = num2;
            int num4 = 15;
            double num5 = (num2 - num) / ((double)num4);
            for (int i = 0; i <= num4; i++)
            {
                HorZoomFactor = num3;
                HorizontalPaginate();
                if (_columnPages.Count == _info.FitPagesWide)
                {
                    return;
                }
                num3 -= num5;
            }
        }

        List<PageInfo> HorizontalPaginatorCore(int columnStart, int columnEnd, int repeatColumnStart, int repeatColumnEnd, SheetArea sheetArea, double pageWidth)
        {
            bool repeat = repeatColumnStart < columnStart;
            double curWidth = 0.0;
            double contentWidth = 0.0;
            double repeatWidth = 0.0;
            double leaveWidth = pageWidth;
            double xEnd = 0.0;
            PageInfo info = null;
            List<PageInfo> list = new List<PageInfo>();
            if (columnStart > columnEnd)
            {
                columnStart = columnEnd;
            }

            for (int i = columnStart; i <= columnEnd; i++)
            {
                // 首页且显示行头，扣除行头宽度
                if (i == columnStart && _sheet.RowHeader.IsVisible)
                {
                    for (int k = 0; k < _sheet.RowHeader.ColumnCount; k++)
                    {
                        leaveWidth -= _sheet.GetActualColumnWidth(k, SheetArea.RowHeader);
                    }
                }
                
                // 需要重复每页左侧时，扣除其宽度
                if (repeat
                    && sheetArea == SheetArea.Cells
                    && repeatColumnStart != -1
                    && repeatColumnStart < i)
                {
                    int leaveCol = repeatColumnEnd;
                    if (leaveCol >= i)
                    {
                        leaveCol = i - 1;
                    }
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.RepeatItemStart = repeatColumnStart;
                    info.RepeatItemEnd = leaveCol;
                    for (int j = repeatColumnStart; j <= leaveCol; j++)
                    {
                        double columnWidth = GetColumnWidth(j, sheetArea);
                        repeatWidth += columnWidth;
                    }
                    leaveWidth -= repeatWidth;
                    repeat = false;
                }
                
                double cw = GetColumnWidth(i, sheetArea);
                curWidth += cw;
                xEnd += cw;
                if ((curWidth > leaveWidth) || (curWidth == leaveWidth))
                {
                    if (curWidth > leaveWidth)
                    {
                        contentWidth = (curWidth - GetColumnWidth(i, sheetArea)) + repeatWidth;
                        xEnd = (xEnd - GetColumnWidth(i, sheetArea)) + repeatWidth;
                        i--;
                    }
                    else
                    {
                        contentWidth = curWidth + repeatWidth;
                        xEnd += repeatWidth;
                    }
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.ItemEnd = i;
                    info.ContentSize = contentWidth;
                    info.XEnd = xEnd;
                    list.Add(info);
                    info = null;
                    curWidth = 0.0;
                    contentWidth = 0.0;
                    repeatWidth = 0.0;
                    leaveWidth = pageWidth;
                    repeat = true;
                }
                else if (i == columnEnd && curWidth > 0)
                {
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    contentWidth = curWidth + repeatWidth;
                    info.ItemEnd = i;
                    info.ContentSize = contentWidth;
                    info.XEnd = xEnd;
                    list.Add(info);
                    info = null;
                    curWidth = 0.0;
                    contentWidth = 0.0;
                    repeatWidth = 0.0;
                    leaveWidth = pageWidth;
                    repeat = true;
                }
            }
            return list;
        }

        double GetColumnWidth(int c, SheetArea sheetArea)
        {
            bool columnVisible = _sheet.GetColumnVisible(c, sheetArea);
            if ((sheetArea == SheetArea.Cells) && (c < _sheet.ColumnCount))
            {
                columnVisible = columnVisible && !_sheet.ColumnRangeGroup.IsCollapsed(c);
            }
            if (!columnVisible)
            {
                return 0.0;
            }
            double columnWidth = _sheet.GetColumnWidth(c, sheetArea);
            if (!_info.BestFitColumns)
            {
                return columnWidth;
            }
            bool rowHeader = sheetArea == (SheetArea.CornerHeader | SheetArea.RowHeader);
            double num2 = Math.Ceiling(_excel.GetColumnAutoFitValue(c, rowHeader));
            if (num2 <= 0.0)
            {
                return columnWidth;
            }
            return ((num2 + 1.0) + 4.0);
        }

        double GetRowHeight(int r, SheetArea sheetArea)
        {
            bool rowVisible = _sheet.GetRowVisible(r, sheetArea);
            if ((sheetArea == SheetArea.Cells) && (r < _sheet.RowCount))
            {
                rowVisible = (rowVisible && !_sheet.Rows[r].IsFilteredOut) && !_sheet.RowRangeGroup.IsCollapsed(r);
            }
            if (!rowVisible)
            {
                return 0.0;
            }
            double rowHeight = _sheet.GetRowHeight(r, sheetArea);
            if (!_info.BestFitRows)
            {
                return rowHeight;
            }
            bool columnHeader = sheetArea == SheetArea.ColumnHeader;
            double num2 = Math.Ceiling(_excel.GetRowAutoFitValue(r, columnHeader));
            if (num2 <= 0.0)
            {
                return rowHeight;
            }
            double defaultRowHeight = (num2 + 1.0) + 2.0;
            if (columnHeader)
            {
                if (defaultRowHeight < _sheet.ColumnHeader.DefaultRowHeight)
                {
                    defaultRowHeight = _sheet.ColumnHeader.DefaultRowHeight;
                }
                return defaultRowHeight;
            }
            if (defaultRowHeight < _sheet.DefaultRowHeight)
            {
                defaultRowHeight = _sheet.DefaultRowHeight;
            }
            return defaultRowHeight;
        }

        void ModifyPageInfo(List<PageInfo> pages, int itemStart)
        {
            PageInfo info = null;
            for (int i = 0; i < pages.Count; i++)
            {
                if ((i - 1) >= 0)
                {
                    info = pages[i - 1];
                }
                else
                {
                    pages[i].ItemStart = itemStart;
                    pages[i].XStart = 0;
                    pages[i].YStart = 0;
                }
                if (info != null)
                {
                    pages[i].ItemStart = info.ItemEnd + 1;
                    pages[i].XStart = info.XEnd;
                    pages[i].YStart = info.YEnd;
                }
            }
        }

        double ContentHeight
        {
            get { return (_pageSize.Height / VerZoomFactor); }
        }

        double ContentWidth
        {
            get { return (_pageSize.Width / HorZoomFactor); }
        }
    }
}

