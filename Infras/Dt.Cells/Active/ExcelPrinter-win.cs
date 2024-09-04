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
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Printing;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Graphics.Printing;
#endregion

namespace Dt.Cells.UI
{
#if WIN
    /// <summary>
    /// 打印管理类
    /// 0. PrintManager.ShowPrintUIAsync()显示打印对话框
    /// 1. PrintTaskSourceRequestedHandler设置打印参数和内容
    /// 2. PrintDocument.Paginate生成预览页面，设置页数
    /// 3. PrintDocument.GetPreviewPage显示特定预览页面
    /// 4. 按下打印PrintDocument.AddPages输出最终打印页集合
    /// </summary>
    internal class ExcelPrinter
    {
        #region 成员变量
        const string _rangeRegexStart = "start";
        const string _rangeRegexEnd = "end";
        static Regex _rangeRegex;

        Canvas _pnl;
        Excel _excel;
        Rectangle _maskRight;
        Rectangle _maskBottom;
        Rectangle _border;
        PrintInfo _info;
        readonly nint _hWnd;
        readonly PrintDocument _printDoc;
        PrintPaginator _paginator;
        double _footerMargin;
        double _headerMargin;
        Rect _printArea;
        List<int> _pageIndexs;
        Size _pageSize;
        string _jobName;
        readonly List<Rect> _pages;
        #endregion

        #region 构造方法
        public ExcelPrinter()
        {
            // 打印请求时
            _hWnd = WinRT.Interop.WindowNative.GetWindowHandle(ExcelKit.MainWin);
            var pr = PrintManagerInterop.GetForWindow(_hWnd);
            pr.PrintTaskRequested += OnPrintTaskRequested;

            _printDoc = new PrintDocument();
            // 打印参数发生变化
            _printDoc.Paginate += OnPaginate;
            // 预览页面
            _printDoc.GetPreviewPage += OnGetPreviewPage;
            // 最终打印页集合
            _printDoc.AddPages += OnAddPages;

            _pages = new List<Rect>();
        }
        #endregion

        #region 打印
        /// <summary>
        /// 显示打印预览对话框
        /// </summary>
        /// <param name="p_excel"></param>
        /// <param name="p_printInfo"></param>
        /// <param name="p_sheetIndex"></param>
        /// <param name="p_jobName"></param>
        public void Print(Excel p_excel, PrintInfo p_printInfo, int p_sheetIndex, string p_jobName)
        {
            _info = p_printInfo;
            _jobName = p_jobName;

            ExcelKit.RunAsync(async () =>
            {
                try
                {
                    CopyExcel(p_excel, p_sheetIndex);
                    await PrintManagerInterop.ShowPrintUIForWindowAsync(_hWnd);
                }
                catch
                { }
            });
        }

        void CopyExcel(Excel p_excel, int p_sheetIndex)
        {
            Worksheet sheet = p_excel.Sheets[p_sheetIndex];
            int endRow = sheet.GetLastDirtyRow();
            int endCol = sheet.GetLastDirtyColumn();

            double height = 0.0;
            if (sheet.ColumnHeader.IsVisible)
                height += GetTotalHeight(sheet, sheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
            height += GetTotalHeight(sheet, endRow + 1, SheetArea.Cells);

            double width = 0.0;
            if (sheet.RowHeader.IsVisible)
                width += GetTotalWidth(sheet, sheet.RowHeader.ColumnCount, SheetArea.RowHeader);
            width += GetTotalWidth(sheet, endCol + 1, SheetArea.Cells);

            // 为提供性能，只复制要打印的Sheet，所以跨Sheet引用时数据无效！！！
            MemoryStream ms = new MemoryStream();
            sheet.SaveXml(ms);
            ms.Seek(0L, SeekOrigin.Begin);
            Worksheet ws = new Worksheet();
            Workbook wb = new Workbook();
            // 先增加到Workbook再加载xml，否则Chart无数据源，因内部根据Worksheet名在Workbook中查找Worksheet！！！
            wb.Sheets.Add(ws);
            ws.OpenXml(ms);

            _excel = new Excel(wb);
            using (_excel.Defer())
            {
                _excel.Width = width;
                _excel.Height = height;
                _excel.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                _excel.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                _excel.TabStripVisibility = Visibility.Collapsed;
                _excel.ShowRowRangeGroup = false;
                _excel.ShowColumnRangeGroup = false;
                _excel.ShowSelection = false;
                _excel.ShowFreezeLine = false;

                // PrintInfo控制行头列头网格是否显示
                ws.PrintInfo = _info;
                if (_info.ShowRowHeader == VisibilityType.Hide)
                    ws.RowHeader.IsVisible = false;
                if (_info.ShowColumnHeader == VisibilityType.Hide)
                    ws.ColumnHeader.IsVisible = false;
                if (!_info.ShowGridLine)
                    ws.ShowGridLine = false;
                
                ws.ZoomFactor = 1f;
                ViewportInfo info = new ViewportInfo(ws, 1, 1);
                ws.SetViewportInfo(info);
            }
            _pnl = new Canvas { Width = width, Height = height };
            _pnl.Children.Add(_excel);
        }
        #endregion

        #region 打印事件处理
        /// <summary>
        /// 处理打印请求
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            sender.PrintTaskRequested -= OnPrintTaskRequested;
            PrintTask printTask = null;
            printTask = e.Request.CreatePrintTask(_jobName, (src) =>
            {
                // 初始化打印预览时的方向和纸张大小
                printTask.Options.Orientation = (_info.Orientation == PrintPageOrientation.Portrait) ? PrintOrientation.Portrait : PrintOrientation.Landscape;
                PrintMediaSize size = _info.PaperSize.MediaSize;
                if (size != PrintMediaSize.PrinterCustom)
                    printTask.Options.MediaSize = size;

                ExcelKit.RunSync(() => src.SetSource(_printDoc.DocumentSource));
            });

            // 打印任务完成时释放资源
            //printTask.Completed += (s, args) => ExcelKit.Msg("打印任务提交成功");
        }

        /// <summary>
        /// 打印参数变化时生成预览页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPaginate(object sender, PaginateEventArgs e)
        {
            var desc = e.PrintTaskOptions.GetPageDescription(0);

            // 页面大小
            _pageSize = new Size();
            if (_info.Orientation == PrintPageOrientation.Landscape)
            {
                // 横向
                _pageSize.Width = _info.PaperSize.PxHeight;
                _pageSize.Height = _info.PaperSize.PxWidth;
            }
            else
            {
                // 纵向
                _pageSize.Width = _info.PaperSize.PxWidth;
                _pageSize.Height = _info.PaperSize.PxHeight;
            }

            // 内容区域及边距
            double x = Math.Max(desc.ImageableRect.Left, _info.Margin.PxLeft);
            double y = Math.Max(desc.ImageableRect.Top, _info.Margin.PxTop);
            double right = Math.Max(desc.PageSize.Width - desc.ImageableRect.Right, _info.Margin.PxRight);
            double bottom = Math.Max(desc.PageSize.Height - desc.ImageableRect.Bottom, _info.Margin.PxBottom);
            _printArea = new Rect(x, y, (_pageSize.Width - x) - right, (_pageSize.Height - y) - bottom);
            _headerMargin = Math.Max(desc.ImageableRect.Top, _info.Margin.PxHeader);
            _footerMargin = Math.Max(desc.ImageableRect.Top, _info.Margin.PxFooter);

            // 添加边距遮罩
            SolidColorBrush bg = new SolidColorBrush(Colors.White);
            var mask = new Rectangle { Width = Math.Max(x - 1, 0), Height = _pageSize.Height, Fill = bg };
            Canvas.SetZIndex(mask, 10);
            _pnl.Children.Add(mask);
            mask = new Rectangle { Width = _pageSize.Width, Height = Math.Max(y - 1, 0), Fill = bg };
            Canvas.SetZIndex(mask, 11);
            _pnl.Children.Add(mask);
            _maskRight = new Rectangle { Height = _pageSize.Height, Fill = bg };
            Canvas.SetZIndex(_maskRight, 12);
            _pnl.Children.Add(_maskRight);
            _maskBottom = new Rectangle { Width = _pageSize.Width, Fill = bg };
            Canvas.SetZIndex(_maskBottom, 13);
            _pnl.Children.Add(_maskBottom);
            
            // 添加外框
            if (_info.ShowBorder)
            {
                _border = new Rectangle { Stroke = new SolidColorBrush(Colors.Black), StrokeThickness = 1 };
                Canvas.SetZIndex(_border, 14);
                Canvas.SetLeft(_border, x);
                Canvas.SetTop(_border, y);
                _pnl.Children.Add(_border);
            }
            
            // 分页计算
            _paginator = new PrintPaginator(
                _excel,
                _info,
                new Size(_printArea.Width, _printArea.Height));
            _paginator.Paginate();

            // 获取PrintInfo中指定的页码列表
            _pageIndexs = GetPageRange();
            if (_pageIndexs == null)
            {
                // 记录所有页码
                _pageIndexs = new List<int>();
                for (int i = 0; i < _paginator.PageCount; i++)
                {
                    _pageIndexs.Add(i);
                }
            }
            else
            {
                // 排序整理要打印的页码列表
                _pageIndexs.Sort();
                List<int> list = new List<int>();
                for (int i = 0; i < _pageIndexs.Count; i++)
                {
                    _pageIndexs[i] = _pageIndexs[i] - 1;
                    if (_pageIndexs[i] >= _paginator.PageCount)
                    {
                        list.Add(_pageIndexs[i]);
                    }
                }
                // 移除超出范围的页码
                for (int j = 0; j < list.Count; j++)
                {
                    _pageIndexs.Remove(list[j]);
                }
            }

            _pages.Clear();
            _excel.RenderTransform = null;
            if (_pageIndexs.Count > 0)
            {
                CreatePages();
                _printDoc.SetPreviewPageCount(_pageIndexs.Count, PreviewPageCountType.Intermediate);
            }
        }

        /// <summary>
        /// 显示特定预览页面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnGetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            try
            {
                ArrangePage(e.PageNumber - 1);
                _printDoc.SetPreviewPage(e.PageNumber, _pnl);
            }
            catch { }
        }

        /// <summary>
        /// 发送最终打印页集合
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnAddPages(object sender, AddPagesEventArgs e)
        {
            for (int i = 0; i < _pages.Count; i++)
            {
                ArrangePage(i);
                _printDoc.AddPage(_pnl);
            }
            _printDoc.AddPagesComplete();
        }
        #endregion

        #region 内部方法
        /// <summary>
        /// 获取PrintInfo中设置的页面范围
        /// </summary>
        /// <returns></returns>
        List<int> GetPageRange()
        {
            string pageRange = _info.PageRange;
            if (string.IsNullOrEmpty(pageRange))
                return null;

            if (!CheckPageRange(pageRange))
                return null;

            List<int> list = new List<int>();
            string[] strArray = pageRange.Split(new char[] { ',' });
            for (int i = 0; i < strArray.Length; i++)
            {
                string str = strArray[i].Trim();
                if (string.IsNullOrEmpty(str))
                    continue;

                Match match = RangeRegex.Match(str);
                int start = -1;
                int end = -1;
                if (match.Groups[_rangeRegexStart].Success && !string.IsNullOrEmpty(match.Groups[_rangeRegexStart].Value))
                {
                    try
                    {
                        start = int.Parse(match.Groups[_rangeRegexStart].Value);
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (match.Groups[_rangeRegexEnd].Success && !string.IsNullOrEmpty(match.Groups[_rangeRegexEnd].Value))
                {
                    try
                    {
                        end = int.Parse(match.Groups[_rangeRegexEnd].Value);
                    }
                    catch
                    {
                        continue;
                    }
                }

                if (str.IndexOf('-') >= 0)
                {
                    int num4 = (end >= start) ? 1 : -1;
                    for (int j = start; j != end; j += num4)
                    {
                        list.Add(j);
                    }
                    list.Add(end);
                }
                else if (start > 0)
                {
                    list.Add(start);
                }
            }
            return list;
        }

        static bool CheckPageRange(string p_pageRange)
        {
            if (!string.IsNullOrEmpty(p_pageRange))
            {
                string[] strArray = p_pageRange.Split(new char[] { ',' });
                if (strArray.Length <= 0)
                    return false;

                for (int i = 0; i < strArray.Length; i++)
                {
                    string input = strArray[i].Trim();
                    if (!RangeRegex.IsMatch(input))
                        return false;
                }
            }
            return true;
        }

        static Regex RangeRegex
        {
            get
            {
                if (_rangeRegex == null)
                    _rangeRegex = new Regex(string.Format(@"^(?<{0}>[0-9]*)\s*[-]?\s*(?<{1}>[0-9]*)$", new object[] { _rangeRegexStart, _rangeRegexEnd }));
                return _rangeRegex;
            }
        }

        /// <summary>
        /// 获得列头或Excel当前表总高
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_count"></param>
        /// <param name="p_area"></param>
        /// <returns></returns>
        internal static double GetTotalHeight(Worksheet p_sheet, int p_count, SheetArea p_area)
        {
            double height = 0.0;
            for (int i = 0; i < p_count; i++)
            {
                height += p_sheet.GetActualRowHeight(i, p_area);
            }
            return height;
        }

        /// <summary>
        /// 获得行头或Excel当前表总宽
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_count"></param>
        /// <param name="p_area"></param>
        /// <returns></returns>
        internal static double GetTotalWidth(Worksheet p_sheet, int p_count, SheetArea p_area)
        {
            double width = 0.0;
            for (int i = 0; i < p_count; i++)
            {
                width += p_sheet.GetActualColumnWidth(i, p_area);
            }
            return width;
        }

        /// <summary>
        /// 生成所有页
        /// </summary>
        void CreatePages()
        {
            Worksheet sheet = _excel.Sheets[0];
            if ((_info.RowStart > -1 && _info.RowEnd > 0)
                || (_info.ColumnStart > -1 && _info.ColumnEnd > 0))
            {
                // 区域打印
                for (int i = 0; i < _pageIndexs.Count; i++)
                {
                    SheetPageInfo info = _paginator.GetPage(i);
                    _pages.Add(new Rect(
                        _paginator.GetPageLocation(info),
                        new Size(
                            Math.Min(info.ColumnPage.ContentSize, _pageSize.Width),
                            Math.Min(info.RowPage.ContentSize, _pageSize.Height))));
                }
            }
            else
            {
                double rowHeaderWidth = 0.0;
                double colHeaderHeight = 0.0;
                if (sheet.RowHeader.IsVisible)
                    rowHeaderWidth = GetTotalWidth(sheet, sheet.RowHeader.ColumnCount, SheetArea.RowHeader);
                if (sheet.ColumnHeader.IsVisible)
                    colHeaderHeight = GetTotalHeight(sheet, sheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
                for (int i = 0; i < _pageIndexs.Count; i++)
                {
                    SheetPageInfo info = _paginator.GetPage(i);
                    double chw = sheet.RowHeader.IsVisible ? (info.ColumnPage.XStart == 0 ? rowHeaderWidth : 0) : 0;
                    double rhh = sheet.ColumnHeader.IsVisible ? (info.RowPage.YStart == 0 ? colHeaderHeight : 0) : 0;
                    _pages.Add(new Rect
                    {
                        X = info.ColumnPage.XStart == 0 ? 0 : info.ColumnPage.XStart + rowHeaderWidth,
                        Y = info.RowPage.YStart == 0 ? 0 : info.RowPage.YStart + colHeaderHeight,
                        Width = Math.Min(info.ColumnPage.ContentSize + chw, _pageSize.Width),
                        Height = Math.Min(info.RowPage.ContentSize + rhh, _pageSize.Height)
                    });
                }
            }
        }

        /// <summary>
        /// 放置页面
        /// </summary>
        /// <param name="p_index"></param>
        void ArrangePage(int p_index)
        {
            var page = _pages[p_index];

            // 计算平移位置
            Canvas.SetLeft(_excel, -page.Left + _printArea.Left);
            Canvas.SetTop(_excel, -page.Top + _printArea.Top);

            var right = _pageSize.Width - _printArea.Left - page.Width;
            _maskRight.Width = right;
            Canvas.SetLeft(_maskRight, _printArea.Left + page.Width);
            
            _maskBottom.Height = _pageSize.Height - _printArea.Top - page.Height;
            Canvas.SetTop(_maskBottom, _printArea.Top + page.Height);
            
            if (_border != null)
            {
                _border.Width = page.Width;
                _border.Height = page.Height;
            }
            // 不在可视树上时Clip方式无效，无裁剪效果！
            //_excel.Clip = new RectangleGeometry { Rect = page };
        }
        #endregion
    }
#endif
}