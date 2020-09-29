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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;
using Windows.UI.Xaml.Shapes;
#endregion

namespace Dt.Cells.UI
{
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

        readonly Excel _excel;
        readonly PrintInfo _info;
        readonly int _sheetIndex;
        readonly PrintDocument _printDoc;
        PrintPaginator _paginator;
        double _footerMargin;
        double _headerMargin;
        Rect _printArea;
        List<int> _pageIndexs;
        Size _pageSize;
        string _jobName;

        MemoryStream _cachedStream;
        readonly List<Rect> _pages;
        bool _hasPictures;
        bool _showDeco;
        #endregion

        public ExcelPrinter(Excel p_excel, PrintInfo p_printInfo, int p_sheetIndex)
        {
            _excel = p_excel;
            _info = p_printInfo;
            _sheetIndex = p_sheetIndex;

            // 打印请求时
            PrintManager.GetForCurrentView().PrintTaskRequested += OnPrintTaskRequested;
            _printDoc = new PrintDocument();
            // 打印参数发生变化
            _printDoc.Paginate += OnPaginate;
            // 预览页面
            _printDoc.GetPreviewPage += OnGetPreviewPage;
            // 最终打印页集合
            _printDoc.AddPages += OnAddPages;

            _pages = new List<Rect>();
            _hasPictures = _excel.Sheets[_sheetIndex].Pictures.Count > 0;
            _showDeco = _excel.ShowDecoration;
        }

        public PrintInfo Info
        {
            get { return _info; }
        }

        public int SheetIndex
        {
            get { return _sheetIndex; }
        }

        public Size PageSize
        {
            get { return _pageSize; }
        }

        public Rect PrintArea
        {
            get { return _printArea; }
        }

        public double HeaderMargin
        {
            get { return _headerMargin; }
        }

        public double FooterMargin
        {
            get { return _footerMargin; }
        }

        public int PageCount
        {
            get
            {
                if (_paginator != null)
                    return _paginator.PageCount;
                return 0;
            }
        }

        /// <summary>
        /// 显示打印预览对话框
        /// </summary>
        /// <param name="p_jobName"></param>
        public async void Print(string p_jobName)
        {
            _jobName = p_jobName;

            // 打印遮罩
            //_excel.PrintMask = await CreatePrintMask(_excel);
            // 去掉excel默认选择
            Worksheet sheet = _excel.Sheets[_sheetIndex];
            sheet.ClearSelections();
            sheet.AddSelection(sheet.RowCount, sheet.ColumnCount, 1, 1);
            await PrintManager.ShowPrintUIAsync();
        }

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

                // 报表中包含图片,先将Excel导出,再导入。否则，图片打印不出来
                if (_hasPictures)
                {
                    _excel.ShowDecoration = false;
                    _cachedStream = new MemoryStream();
                    _excel.SaveXmlBackground(_cachedStream);
                    _cachedStream.Seek(0L, SeekOrigin.Begin);
                    _excel.OpenXmlOnBackground(_cachedStream);
                }

                Memento memento = new Memento();
                Init(memento);
                StretchContent();
                src.SetSource(_printDoc.DocumentSource);

                // 打印任务完成时释放资源
                printTask.Completed += (s, args) =>
                {
                    if (_hasPictures)
                    {
                        // 恢复分页线
                        if (_showDeco)
                        {
                            _excel.ShowDecoration = true;
                            _cachedStream.Seek(0L, SeekOrigin.Begin);
                            _excel.OpenXmlOnBackground(_cachedStream);
                        }

                        _cachedStream.Close();
                        _cachedStream = null;
                    }
                    Resume(memento);
                };
            });
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
            Size pageSize = new Size();
            if (_info.Orientation == PrintPageOrientation.Landscape)
            {
                // 横向
                pageSize.Width = _info.PaperSize.Height;
                pageSize.Height = _info.PaperSize.Width;
            }
            else
            {
                // 纵向
                pageSize.Width = _info.PaperSize.Width;
                pageSize.Height = _info.PaperSize.Height;
            }
            _pageSize = new Size(pageSize.Width, pageSize.Height);

            // 内容区域及边距
            double x = Math.Max(desc.ImageableRect.Left, _info.Margin.Left);
            double y = Math.Max(desc.ImageableRect.Top, _info.Margin.Top);
            double num3 = Math.Max(desc.PageSize.Width - desc.ImageableRect.Right, _info.Margin.Right);
            double num4 = Math.Max(desc.PageSize.Height - desc.ImageableRect.Bottom, _info.Margin.Bottom);
            _printArea = new Rect(x, y, (_pageSize.Width - x) - num3, (_pageSize.Height - y) - num4);
            _headerMargin = Math.Max(desc.ImageableRect.Top, _info.Margin.Header);
            _footerMargin = Math.Max(desc.ImageableRect.Top, _info.Margin.Footer);

            // 分页计算
            _paginator = new PrintPaginator(
                _excel,
                _sheetIndex,
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
            _excel.Clip = null;
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
                _printDoc.SetPreviewPage(e.PageNumber, _excel);
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
                _printDoc.AddPage(_excel);
            }
            _printDoc.AddPagesComplete();
        }

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
        /// 生成打印遮罩
        /// </summary>
        /// <param name="p_tgt"></param>
        /// <returns></returns>
        async Task<Grid> CreatePrintMask(FrameworkElement p_tgt)
        {
            Grid grid = new Grid();
            grid.Background = new SolidColorBrush(Colors.White);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Top;
            RenderTargetBitmap bmp = new RenderTargetBitmap();
            await bmp.RenderAsync(p_tgt);
            grid.Children.Add(new Image { Source = bmp, Stretch = Stretch.None });
            grid.Children.Add(new Rectangle { Fill = new SolidColorBrush(Colors.Transparent) });
            Border border = new Border();
            border.HorizontalAlignment = HorizontalAlignment.Center;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.Background = new SolidColorBrush(Colors.Green);
            border.BorderThickness = new Thickness();
            TextBlock tb = new TextBlock();
            tb.Text = "正在打印...";
            tb.FontWeight = FontWeights.Bold;
            tb.Margin = new Thickness(20, 10, 20, 10);
            tb.Foreground = new SolidColorBrush(Colors.White);
            border.Child = tb;
            grid.Children.Add(border);
            return grid;
        }

        /// <summary>
        /// 平铺打印内容
        /// </summary>
        void StretchContent()
        {
            Grid grid = VisualTreeHelper.GetParent(_excel) as Grid;
            Worksheet sheet = _excel.Sheets[_sheetIndex];
            int endRow = sheet.GetLastDirtyRow();
            int endCol = sheet.GetLastDirtyColumn();
            double height = 0.0;
            double width = 0.0;
            if (sheet.ColumnHeader.IsVisible)
                height += GetTotalHeight(sheet, sheet.ColumnHeader.RowCount, SheetArea.ColumnHeader);
            height += GetTotalHeight(sheet, endRow + 1, SheetArea.Cells);
            if (sheet.RowHeader.IsVisible)
                width += GetTotalWidth(sheet, sheet.RowHeader.ColumnCount, SheetArea.RowHeader);
            width += GetTotalWidth(sheet, endCol + 1, SheetArea.Cells);
            if (grid.ActualHeight < height)
                grid.Height = height;
            if (grid.ActualWidth < width)
                grid.Width = width;
        }

        /// <summary>
        /// 打印设置
        /// </summary>
        /// <param name="p_memento"></param>
        void Init(Memento p_memento)
        {
            Grid grid = VisualTreeHelper.GetParent(_excel) as Grid;
            p_memento.Height = grid.Height;
            p_memento.Width = grid.Width;
            p_memento.VerticalScrollBarVisibility = _excel.VerticalScrollBarVisibility;
            p_memento.HorizontalScrollBarVisibility = _excel.HorizontalScrollBarVisibility;
            p_memento.TabStripVisibility = _excel.TabStripVisibility;
            p_memento.ShowRowRangeGroup = _excel.ShowRowRangeGroup;
            p_memento.ShowColumnRangeGroup = _excel.ShowColumnRangeGroup;
            p_memento.ShowFreezeLine = _excel.ShowFreezeLine;
            p_memento.ShowSelection = _excel.ShowSelection;

            _excel.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _excel.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            _excel.TabStripVisibility = Visibility.Collapsed;
            _excel.ShowRowRangeGroup = false;
            _excel.ShowColumnRangeGroup = false;
            _excel.ShowSelection = false;
            _excel.HideDecorationWhenPrinting = true;
            _excel.ShowFreezeLine = false;

            Worksheet sheet = _excel.Sheets[_sheetIndex];
            sheet.ZoomFactor = 1f;
            ViewportInfo info = new ViewportInfo(sheet, 1, 1);
            sheet.SetViewportInfo(info);
        }

        /// <summary>
        /// 恢复打印前设置
        /// </summary>
        /// <param name="p_memento"></param>
        void Resume(Memento p_memento)
        {
            _excel.RenderTransform = null;
            _excel.Clip = null;

            Grid grid = VisualTreeHelper.GetParent(_excel) as Grid;
            grid.Height = p_memento.Height;
            grid.Width = p_memento.Width;
            _excel.VerticalScrollBarVisibility = p_memento.VerticalScrollBarVisibility;
            _excel.HorizontalScrollBarVisibility = p_memento.HorizontalScrollBarVisibility;
            _excel.TabStripVisibility = p_memento.TabStripVisibility;
            _excel.ShowRowRangeGroup = p_memento.ShowRowRangeGroup;
            _excel.ShowColumnRangeGroup = p_memento.ShowColumnRangeGroup;
            _excel.ShowFreezeLine = p_memento.ShowFreezeLine;
            _excel.ShowSelection = p_memento.ShowSelection;
            _excel.HideDecorationWhenPrinting = false;
        }

        /// <summary>
        /// 获得列头或Excel当前表总高
        /// </summary>
        /// <param name="p_sheet"></param>
        /// <param name="p_count"></param>
        /// <param name="p_area"></param>
        /// <returns></returns>
        double GetTotalHeight(Worksheet p_sheet, int p_count, SheetArea p_area)
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
        double GetTotalWidth(Worksheet p_sheet, int p_count, SheetArea p_area)
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
            Worksheet sheet = _excel.Sheets[_sheetIndex];
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
                    Width = info.ColumnPage.ContentSize + chw,
                    Height = info.RowPage.ContentSize + rhh
                });
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
            TranslateTransform trans = new TranslateTransform();
            trans.X = -page.Left + _printArea.Left;
            trans.Y = -page.Top + _printArea.Top;
            _excel.RenderTransform = trans;

            _excel.Clip = new RectangleGeometry { Rect = page };
        }

        /// <summary>
        /// 记录Excel打印前状态
        /// </summary>
        internal class Memento
        {
            public double Height;

            public double Width;

            public ScrollBarVisibility VerticalScrollBarVisibility;

            public ScrollBarVisibility HorizontalScrollBarVisibility;

            public Visibility TabStripVisibility;

            public Thickness BorderThickness = new Thickness(0);

            public bool ShowRowRangeGroup;

            public bool ShowColumnRangeGroup;

            public bool ShowFreezeLine;

            public bool ShowSelection;
        }
    }
}

