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

        public PrintPaginator(Excel p_spread, int p_sheetIndex, PrintInfo p_printSettings, Size p_paperSize)
        {
            _excel = p_spread;
            _sheet = p_spread.Sheets[p_sheetIndex];
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
            bool flag = repeatRowStart < rowStart;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = pageHeight;
            int num5 = repeatRowStart;
            int num6 = repetRowEnd;
            double yEnd = 0.0;
            PageInfo info = null;
            List<PageInfo> list = new List<PageInfo>();
            if (rowStart > rowEnd)
            {
                rowStart = rowEnd;
            }
            for (int i = rowStart; i <= rowEnd; i++)
            {
                if (flag && (sheetArea == SheetArea.Cells))
                {
                    num5 = repeatRowStart;
                    num6 = repetRowEnd;
                    if ((num5 != -1) && (num5 < i))
                    {
                        if (num6 >= i)
                        {
                            num6 = i - 1;
                        }
                        if (info == null)
                        {
                            info = new PageInfo();
                        }
                        info.RepeatItemStart = num5;
                        info.RepeatItemEnd = num6;
                        for (int j = num5; j <= num6; j++)
                        {
                            double rowHeight = GetRowHeight(j, sheetArea);
                            num3 += rowHeight;
                        }
                        num4 -= num3;
                        flag = false;
                    }
                }
                double rh = GetRowHeight(i, sheetArea);
                num += rh;
                yEnd += rh;
                if ((num > num4) || (num == num4))
                {
                    if (num > num4)
                    {
                        num2 = (num - GetRowHeight(i, sheetArea)) + num3;
                        yEnd = (yEnd - GetRowHeight(i, sheetArea)) + num3;
                        i--;
                    }
                    else
                    {
                        num2 = num + num3;
                        yEnd += num3;
                    }
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.ItemEnd = i;
                    info.ContentSize = num2;
                    info.YEnd = yEnd;
                    list.Add(info);
                    info = null;
                    num = 0.0;
                    num2 = 0.0;
                    num3 = 0.0;
                    num4 = pageHeight;
                    flag = true;
                }
                else if (i == rowEnd)
                {
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    num2 = num + num3;
                    info.ItemEnd = i;
                    info.ContentSize = num2;
                    info.YEnd = yEnd;
                    list.Add(info);
                    info = null;
                    num = 0.0;
                    num2 = 0.0;
                    num3 = 0.0;
                    num4 = pageHeight;
                    flag = true;
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
            bool flag = repeatColumnStart < columnStart;
            double num = 0.0;
            double num2 = 0.0;
            double num3 = 0.0;
            double num4 = pageWidth;
            int num5 = repeatColumnStart;
            int num6 = repeatColumnEnd;
            double xEnd = 0.0;
            PageInfo info = null;
            List<PageInfo> list = new List<PageInfo>();
            if (columnStart > columnEnd)
            {
                columnStart = columnEnd;
            }
            for (int i = columnStart; i <= columnEnd; i++)
            {
                if (flag && (sheetArea == SheetArea.Cells))
                {
                    num5 = repeatColumnStart;
                    num6 = repeatColumnEnd;
                    if ((num5 != -1) && (num5 < i))
                    {
                        if (num6 >= i)
                        {
                            num6 = i - 1;
                        }
                        if (info == null)
                        {
                            info = new PageInfo();
                        }
                        info.RepeatItemStart = repeatColumnStart;
                        info.RepeatItemEnd = repeatColumnEnd;
                        for (int j = num5; j <= num6; j++)
                        {
                            double columnWidth = GetColumnWidth(j, sheetArea);
                            num3 += columnWidth;
                        }
                        num4 -= num3;
                        flag = false;
                    }
                }
                double cw = GetColumnWidth(i, sheetArea);
                num += cw;
                xEnd += cw;
                if ((num > num4) || (num == num4))
                {
                    if (num > num4)
                    {
                        num2 = (num - GetColumnWidth(i, sheetArea)) + num3;
                        xEnd = (xEnd - GetColumnWidth(i, sheetArea)) + num3;
                        i--;
                    }
                    else
                    {
                        num2 = num + num3;
                        xEnd += num3;
                    }
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    info.ItemEnd = i;
                    info.ContentSize = num2;
                    info.XEnd = xEnd;
                    list.Add(info);
                    info = null;
                    num = 0.0;
                    num2 = 0.0;
                    num3 = 0.0;
                    num4 = pageWidth;
                    flag = true;
                }
                else if (i == columnEnd)
                {
                    if (info == null)
                    {
                        info = new PageInfo();
                    }
                    num2 = num + num3;
                    info.ItemEnd = i;
                    info.ContentSize = num2;
                    info.XEnd = xEnd;
                    list.Add(info);
                    info = null;
                    num = 0.0;
                    num2 = 0.0;
                    num3 = 0.0;
                    num4 = pageWidth;
                    flag = true;
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

