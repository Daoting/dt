#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base.Report;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.ComponentModel;
using System.Text;
using Windows.Foundation;
using Windows.Storage;
#endregion

namespace Dt.Base
{
    /// <summary>
    /// 报表预览控件
    /// </summary>
    public sealed partial class RptTab : Tab
    {
        #region 静态内容
        public static readonly DependencyProperty IsPdfProperty = DependencyProperty.Register(
            "IsPdf",
            typeof(bool),
            typeof(RptTab),
            new PropertyMetadata(false, OnIsPdfChanged));

        static void OnIsPdfChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RptTab)d).OnIsPdfChanged();
        }
        #endregion

        #region 成员变量
        readonly Excel _excel;
        PdfView _pdf;
        RptInfo _info;

        SheetTable _selectedTable;
        Menu _contextMenu;
        Menu _selectionMenu;

        BaseCommand _cmdExportExcel;
        BaseCommand _cmdExportPdf;
        BaseCommand _cmdPrint;
        BaseCommand _cmdGridLine;
        BaseCommand _cmdColHeader;
        BaseCommand _cmdRowHeader;
        BaseCommand _cmdClearTable;
        BaseCommand _cmdClearChart;
        #endregion

        #region 构造方法
        public RptTab()
        {
            // 虽然未调用InitializeComponent()，xaml文件不能删除！
            // 否则，当所有项目中的xaml都未有 RptTab 对象时，代码中 new RptTab 出错闪退！操

            _excel = new Excel
            {
                TabStripVisibility = Visibility.Collapsed,
                ShowDecoration = true,
            };
            _excel.Sheets.Clear();
            _excel.CellClick += OnCellClick;
            _excel.SelectionChanged += OnSelectionChanged;
            _excel.RightTapped += OnExcelRightTapped;
            Content = _excel;
        }
        #endregion

        #region 属性
        /// <summary>
        /// 对应的报表描述信息
        /// </summary>
        public RptInfo Info => _info;

        /// <summary>
        /// 内部Excel
        /// </summary>
        public Excel Excel => _excel;

        /// <summary>
        /// 内部PdfView
        /// </summary>
        public PdfView PdfView => _pdf;

        /// <summary>
        /// 报表是否采用Pdf格式，默认false，Excel格式
        /// </summary>
        public bool IsPdf
        {
            get { return (bool)GetValue(IsPdfProperty); }
            set { SetValue(IsPdfProperty, value); }
        }
        #endregion

        #region 命令
        /// <summary>
        /// 获取导出Excel命令对象
        /// </summary>
        public BaseCommand CmdExportExcel
        {
            get
            {
                if (_cmdExportExcel == null)
                    _cmdExportExcel = new BaseCommand(p => ExportExcel());
                return _cmdExportExcel;
            }
        }

        /// <summary>
        /// 获取导出Pdf命令对象
        /// </summary>
        public BaseCommand CmdExportPdf
        {
            get
            {
                if (_cmdExportPdf == null)
                    _cmdExportPdf = new BaseCommand(p => ExportPdf());
                return _cmdExportPdf;
            }
        }

        /// <summary>
        /// 获取打印命令对象
        /// </summary>
        public BaseCommand CmdPrint
        {
            get
            {
                if (_cmdPrint == null)
                    _cmdPrint = new BaseCommand(p => DoPrint());
                return _cmdPrint;
            }
        }
        
        /// <summary>
        /// 获取网格命令对象
        /// </summary>
        public BaseCommand CmdGridLine
        {
            get
            {
                if (_cmdGridLine == null)
                {
                    _cmdGridLine = new BaseCommand(p => DoShowGridLine(!_excel.ShowGridLine));
                }

                return _cmdGridLine;
            }
        }

        /// <summary>
        /// 获取列头命令对象
        /// </summary>
        public BaseCommand CmdColHeader
        {
            get
            {
                if (_cmdColHeader == null)
                {
                    _cmdColHeader = new BaseCommand((p_param) =>
                    {
                        if (_excel.ActiveSheet != null)
                            DoShowColHeader(!_excel.ActiveSheet.ColumnHeader.IsVisible);
                    });
                }
                return _cmdColHeader;
            }
        }

        /// <summary>
        /// 获取行头命令对象
        /// </summary>
        public BaseCommand CmdRowHeader
        {
            get
            {
                if (_cmdRowHeader == null)
                {
                    _cmdRowHeader = new BaseCommand((p_param) =>
                    {
                        if (_excel.ActiveSheet != null)
                            DoShowRowHeader(!_excel.ActiveSheet.RowHeader.IsVisible);
                    });
                }
                return _cmdRowHeader;
            }
        }

        /// <summary>
        /// 获取清除表格命令对象
        /// </summary>
        public BaseCommand CmdClearTable
        {
            get
            {
                if (_cmdClearTable == null)
                    _cmdClearTable = new BaseCommand(p => ClearTable());
                return _cmdClearTable;
            }
        }

        /// <summary>
        /// 获取清除图表命令对象
        /// </summary>
        public BaseCommand CmdClearChart
        {
            get
            {
                if (_cmdClearChart == null)
                    _cmdClearChart = new BaseCommand(p => ClearChart());
                return _cmdClearChart;
            }
        }
        #endregion

        #region 加载报表
        /// <summary>
        /// 加载报表内容
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        public async void LoadReport(RptInfo p_info)
        {
            // 确保正确加载模板
            if (p_info == null || !await p_info.Init())
                return;

            if (_info != p_info)
            {
                // 切换报表时需要重新加载Menu
                _info = p_info;
                _contextMenu = null;
                _selectionMenu = null;
                LoadToolbarMenu();
            }

            // 清除旧数据
            _info.ClearData();

            var error = _info.IsParamsValid();
            if (error == null)
            {
                // 查询参数完备，绘制报表
                await DrawData();
            }
            else
            {
                // 查询参数不完备
                _excel.Sheets.Clear();
                Kit.Warn(error);
            }
        }

        /// <summary>
        /// 绘制报表
        /// </summary>
        /// <returns></returns>
        async Task DrawData()
        {
            var dlg = (Content as FrameworkElement).Busy("正在加载...");
            try
            {
                if (_info.ScriptObj != null && _info.ScriptObj.View != this)
                    _info.ScriptObj.View = this;

                // 绘制报表内容
                var inst = new RptRootInst(_info);
                await inst.Draw();

                using (_excel.Defer())
                {
                    _excel.Sheets.Clear();

                    // 外部可控制报表页属性
                    var ws = _info.Sheet;
                    if (ws != null)
                    {
                        ws.ColumnHeader.IsVisible = _info.Root.ViewSetting.ShowColHeader;
                        ws.RowHeader.IsVisible = _info.Root.ViewSetting.ShowRowHeader;
                        ws.ShowGridLine = _info.Root.ViewSetting.ShowGridLine;
                        _excel.Sheets.Add(ws);
                    }
                }

                if (IsPdf)
                    await LoadPdfContent();
            }
            finally
            {
                dlg.Close();
            }
        }

        async void OnIsPdfChanged()
        {
            bool isPdf = IsPdf;
            if (isPdf)
            {
                if (_pdf == null)
                    _pdf = new PdfView();
                Content = _pdf;
                await LoadPdfContent();
            }
            else
            {
                Content = _excel;
            }

            if (Menu != null)
            {
                if (Menu["Excel"] is Mi mi)
                    mi.Visibility = isPdf ? Visibility.Visible : Visibility.Collapsed;
                if (Menu["Pdf"] is Mi miPdf)
                    miPdf.Visibility = isPdf ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        async Task LoadPdfContent()
        {
            if (_info != null && _info.Sheet != null)
            {
                MemoryStream ms = new();
                await _excel.SavePdf(ms);
                if (_pdf == null)
                    _pdf = new PdfView();
                _pdf.Open(ms.ToArray(), _info.Name + ".pdf");
            }
        }
        #endregion

        #region 命令方法
        /// <summary>
        /// 导出Excel
        /// </summary>
        public async void ExportExcel()
        {
            if (_excel.ActiveSheet == null)
                return;

            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
            filePicker.FileTypeChoices.Add("Excel 97-2003 Files", new List<string>(new string[] { ".xls" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                var fileName = storageFile.FileType.ToUpperInvariant();
                var fileFormat = ExcelFileFormat.XLS;
                if (fileName.EndsWith(".XLSX"))
                    fileFormat = ExcelFileFormat.XLSX;
                else
                    fileFormat = ExcelFileFormat.XLS;
                await _excel.SaveExcel(stream, fileFormat, ExcelSaveFlags.NoFlagsSet);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 导出Pdf
        /// </summary>
        public async void ExportPdf()
        {
            if (_excel.ActiveSheet == null)
                return;

            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SavePdf(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        public void DoPrint()
        {
            if (_excel.ActiveSheet != null)
            {
                _excel.Print(_excel.ActiveSheet.PrintInfo, -1, _info.Name);
            }
        }
        
        /// <summary>
        /// 显示网格
        /// </summary>
        /// <param name="p_show"></param>
        public void DoShowGridLine(bool p_show)
        {
            _excel.ShowGridLine = p_show;
        }

        /// <summary>
        /// 显示列头
        /// </summary>
        /// <param name="p_show"></param>
        public void DoShowColHeader(bool p_show)
        {
            var st = _excel.ActiveSheet;
            if (st != null)
                st.ColumnHeader.IsVisible = p_show;
        }

        /// <summary>
        /// 显示行头
        /// </summary>
        /// <param name="p_show"></param>
        public void DoShowRowHeader(bool p_show)
        {
            var st = _excel.ActiveSheet;
            if (st != null)
                st.RowHeader.IsVisible = p_show;
        }
        
        /// <summary>
        /// 关闭Pdf的WebView2
        /// </summary>
        public void Close()
        {
            if (_pdf != null)
                _pdf.Close();
        }
        #endregion

        #region 工具栏菜单
        /// <summary>
        /// 加载报表工具栏菜单
        /// </summary>
        void LoadToolbarMenu()
        {
            if (_info.Root.ViewSetting.ShowMenu)
            {
                var menu = new Menu();
                AddMenuItems(menu);
                _info.ScriptObj?.InitMenu(menu);
                Menu = menu;
            }
            else if (Menu != null)
            {
                Menu = null;
            }
        }

        /// <summary>
        /// 添加公共菜单项
        /// </summary>
        /// <param name="p_menu"></param>
        void AddMenuItems(Menu p_menu)
        {
            var setting = _info.Root.ViewSetting;
            Mi mi;
            if (setting.ShowToggleView)
            {
                mi = new Mi { ID = "Excel", Icon = Icons.Excel };
                mi.Call += () => IsPdf = false;
                p_menu.Items.Add(mi);
                if (!IsPdf)
                    mi.Visibility = Visibility.Collapsed;

                mi = new Mi { ID = "Pdf", Icon = Icons.Pdf };
                mi.Call += () => IsPdf = true;
                p_menu.Items.Add(mi);
                if (IsPdf)
                    mi.Visibility = Visibility.Collapsed;
            }

            if (setting.ShowExport)
            {
                mi = new Mi { ID = "导出", Icon = Icons.导出 };
                mi.Items.Add(new Mi { ID = "Excel文件", Icon = Icons.Excel, Cmd = CmdExportExcel });
                mi.Items.Add(new Mi { ID = "Pdf文件", Icon = Icons.Pdf, Cmd = CmdExportPdf });
                p_menu.Items.Add(mi);
            }

            if (setting.ShowPrint)
            {
                p_menu.Items.Add(new Mi { ID = "打印", Icon = Icons.打印, Cmd = CmdPrint });
            }
        }
        #endregion

        #region Excel事件
        /// <summary>
        /// 点击单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCellClick(object sender, CellClickEventArgs e)
        {
            if (_info != null
                && _info.ScriptObj != null
                && _excel.ActiveSheet[e.Row, e.Column].Tag is RptTextInst inst
                && inst.Item is RptText txt
                && txt.HandleClick)
            {
                _selectedTable = null;
                _excel.DecorationRange = null;
                _info.ScriptObj.OnCellClick(new RptCellArgs(inst));
            }
        }

        /// <summary>
        /// 选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectionChanged(object sender, EventArgs e)
        {
            if (_info == null
                || !_info.Root.ViewSetting.ShowSelectionMenu)
                return;

            _selectedTable = null;
            _excel.DecorationRange = null;

            Worksheet sheet = _excel.ActiveSheet;
            if (sheet == null || sheet.Selections.Count != 1)
                return;

            CellRange range = sheet.Selections[0];
            SheetTable[] st = sheet.GetTables();
            foreach (SheetTable tbl in st)
            {
                if (tbl.Range.Intersects(range.Row, range.Column, range.RowCount, range.ColumnCount))
                {
                    _selectedTable = tbl;
                    _excel.DecorationRange = tbl.Range;
                    break;
                }
            }
            // 选择区包含表格 单行时不显示菜单
            if (_selectedTable != null || range.RowCount < 2)
                return;

            if (_selectionMenu == null)
            {
                var menu = new Menu { IsContextMenu = true };
                Mi mi = new Mi { ID = "转为表格", Icon = Icons.田字格 };
                mi.Click += (args) => AddSheetTable(_excel.ActiveSheet.Selections[0]);
                menu.Items.Add(mi);

                mi = new Mi { ID = "生成柱状图", Icon = Icons.对比图 };
                mi.Click += (args) => AddChart(_excel.ActiveSheet.Selections[0]);
                menu.Items.Add(mi);
                _selectionMenu = menu;
            }

            Point topLeft = _excel.GetAbsolutePosition();
            Rect rc = _excel.ActiveSheet.GetRangeBound(range);
            double x = topLeft.X + rc.X + rc.Width + 5 - (_excel.ActiveSheet.RowHeader.IsVisible ? 0 : _excel.ActiveSheet.RowHeader.DefaultColumnWidth);
            double y = topLeft.Y + rc.Y - (_excel.ActiveSheet.ColumnHeader.IsVisible ? 0 : _excel.ActiveSheet.ColumnHeader.DefaultRowHeight);
            _ = _selectionMenu.OpenContextMenu(new Point(x, y));
        }

        void OnExcelRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (_info == null
                || _excel.ActiveSheet == null
                || (!_info.Root.ViewSetting.ShowContextMenu && !_info.Root.ViewSetting.ShowSelectionMenu))
            {
                return;
            }

            if (_contextMenu == null)
            {
                var menu = new Menu { IsContextMenu = true };
                if (_info.Root.ViewSetting.ShowSelectionMenu)
                {
                    Mi mi = new Mi { ID = "删除表格", Icon = Icons.田字格 };
                    mi.Click += (args) => DelSheetTable();
                    menu.Items.Add(mi);

                    mi = new Mi { ID = "清除所有图表", Icon = Icons.对比图 };
                    mi.Click += (args) => ClearChart();
                    menu.Items.Add(mi);

                    mi = new Mi { ID = "清除所有表格", Icon = Icons.田字格 };
                    mi.Click += (args) => ClearTable();
                    menu.Items.Add(mi);
                }

                if (_info.Root.ViewSetting.ShowContextMenu)
                {
                    AddMenuItems(menu);
                    var setting = _info.Root.ViewSetting;
                    if (setting.ShowGridLineItem)
                        menu.Items.Add(new Mi { ID = "显示网格", IsCheckable = true, IsChecked = _excel.ShowGridLine, Cmd = CmdGridLine });
                    if (setting.ShowColHeaderItem)
                        menu.Items.Add(new Mi { ID = "显示列头", IsCheckable = true, IsChecked = _excel.ActiveSheet.ColumnHeader.IsVisible, Cmd = CmdColHeader });
                    if (setting.ShowRowHeaderItem)
                        menu.Items.Add(new Mi { ID = "显示行头", IsCheckable = true, IsChecked = _excel.ActiveSheet.RowHeader.IsVisible, Cmd = CmdRowHeader });
                }
                _contextMenu = menu;
            }

            if (_info.Root.ViewSetting.ShowSelectionMenu)
            {
                _contextMenu["删除表格"].Visibility = _selectedTable != null ? Visibility.Visible : Visibility.Collapsed;
                _contextMenu["清除所有图表"].Visibility = _excel.ActiveSheet.Charts.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                _contextMenu["清除所有表格"].Visibility = _excel.ActiveSheet.GetTables().Length > 0 ? Visibility.Visible : Visibility.Collapsed;
            }

            bool exist = (from mi in _contextMenu.Items
                          where mi.Visibility == Visibility.Visible
                          select mi).Any();
            if (exist)
            {
                // 脚本处理上下文菜单
                _info.ScriptObj?.OpenContextMenu(_contextMenu);
                _ = _contextMenu.OpenContextMenu(e.GetPosition(null));
            }
        }
        #endregion

        #region 增删表格图表
        /// <summary>
        /// 添加表格
        /// </summary>
        /// <param name="p_range"></param>
        SheetTable AddSheetTable(CellRange p_range)
        {
            Worksheet sheet = _excel.ActiveSheet;
            int maxRow = p_range.Row + p_range.RowCount;
            int maxCol = p_range.Column + p_range.ColumnCount;
            for (int i = p_range.Row; i < maxRow; i++)
            {
                for (int j = p_range.Column; j < maxCol; j++)
                {
                    sheet[i, j].StyleName = "";
                }
            }
            SheetTable table = sheet.AddTable("table" + Kit.NewGuid, p_range.Row, p_range.Column, p_range.RowCount, p_range.ColumnCount, TableStyles.Light21);
            sheet.SetActiveCell(0, 0, true);
            return table;
        }

        /// <summary>
        /// 添加图表
        /// </summary>
        /// <param name="p_range"></param>
        SpreadChart AddChart(CellRange p_range)
        {
            Worksheet sheet = _excel.ActiveSheet;
            Rect rc = sheet.GetRangeLocation(p_range);
            string rangeFormula = sheet.Cells[p_range.Row, p_range.Column, p_range.RowCount + p_range.Row - 1, p_range.ColumnCount + p_range.Column - 1].ToString(this.Excel.ActiveSheet.Cells[0, 0]);
            rangeFormula = "'" + sheet.Name + "'!" + rangeFormula;
            SpreadChart chart = sheet.AddChart("table" + Kit.NewGuid, SpreadChartType.ColumnClustered, rangeFormula, rc.Left, rc.Top + rc.Height + 8, 600, 300);
            chart.Legend.Orientation = Orientation.Vertical;
            StringBuilder builder = new StringBuilder("chart");
            builder.Append(sheet.Charts.Count.ToString()).Append("!");
            builder.Append(p_range.Row.ToString()).Append(",").Append(p_range.Column.ToString()).Append(",");
            builder.Append(p_range.RowCount.ToString()).Append(",").Append(p_range.ColumnCount.ToString());
            chart.Name = builder.ToString();
            chart.PropertyChanged += OnPropertyChanged;
            chart.IsSelected = true;
            sheet.SetActiveCell(0, 0, true);
            return chart;
        }

        /// <summary>
        /// 删除当前选择的表格
        /// </summary>
        void DelSheetTable()
        {
            if (_selectedTable == null)
                return;

            Worksheet sheet = _excel.ActiveSheet;
            List<string> recText = new List<string>();
            List<object> recInst = new List<object>();
            CellRange range = _selectedTable.Range;
            int idx = 0;
            RptTextInst inst;
            Dt.Cells.Data.Cell cell = null;
            int maxRow = range.Row + range.RowCount;
            int maxCol = range.Column + range.ColumnCount;
            for (int i = range.Row; i < maxRow; i++)
            {
                for (int j = range.Column; j < maxCol; j++)
                {
                    cell = sheet[i, j];
                    recText.Add((cell.Text.StartsWith("Column") && cell.Tag == null) ? null : cell.Text);
                    recInst.Add(cell.Tag);
                }
            }

            using (_excel.Defer())
            {
                _excel.DecorationRange = null;
                sheet.RemoveTable(_selectedTable);
                for (int i = range.Row; i < maxRow; i++)
                {
                    for (int j = range.Column; j < maxCol; j++)
                    {
                        sheet[i, j].Text = recText[idx];
                        sheet[i, j].Tag = recInst[idx];
                        inst = sheet[i, j].Tag as RptTextInst;
                        if (inst == null)
                        {
                            sheet[i, j].StyleName = "";
                        }
                        else
                        {
                            Kit.RunSync(() => { (inst.Item as RptText).ApplyStyle(sheet[i, j]); });
                        }
                        idx++;
                    }
                }
            }
        }

        /// <summary>
        /// 当选中图表的时候，显示图表数据范围，不选中不显示。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                SpreadChart chart = sender as SpreadChart;
                if (chart == null)
                    return;
                Worksheet sheet = _excel.ActiveSheet;
                string[] Position = chart.Name.Substring(chart.Name.IndexOf("!") + 1).Split(',');
                if (Position.Length != 4)
                    return;
                CellRange range = new CellRange(int.Parse(Position[0]), int.Parse(Position[1]), int.Parse(Position[2]), int.Parse(Position[3]));
                if (!range.IsValidRange(sheet))
                    return;
                _excel.DecorationRange = chart.IsSelected ? range : null;
            }
        }

        /// <summary>
        /// 清除表格
        /// </summary>
        void ClearTable()
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet == null)
                return;

            List<string> recText = new List<string>();
            List<object> recInst = new List<object>();
            int idx = 0;
            RptTextInst inst;
            Dt.Cells.Data.Cell cell = null;
            _selectedTable = null;

            using (_excel.Defer())
            {
                _excel.DecorationRange = null;
                foreach (SheetTable tbl in sheet.GetTables())
                {
                    CellRange range = tbl.Range;
                    int maxRow = range.Row + range.RowCount;
                    int maxCol = range.Column + range.ColumnCount;
                    for (int i = range.Row; i < maxRow; i++)
                    {
                        for (int j = range.Column; j < maxCol; j++)
                        {
                            cell = sheet[i, j];
                            recText.Add((cell.Text.StartsWith("Column") && cell.Tag == null) ? null : cell.Text);
                            recInst.Add(cell.Tag);
                        }
                    }
                    sheet.RemoveTable(tbl);
                    idx = 0;
                    for (int i = range.Row; i < maxRow; i++)
                    {
                        for (int j = range.Column; j < maxCol; j++)
                        {
                            sheet[i, j].Text = recText[idx];
                            sheet[i, j].Tag = recInst[idx];
                            inst = sheet[i, j].Tag as RptTextInst;
                            if (inst == null)
                            {
                                sheet[i, j].StyleName = "";
                            }
                            else
                            {
                                Kit.RunSync(() => { (inst.Item as RptText).ApplyStyle(sheet[i, j]); });
                            }
                            idx++;
                        }
                    }
                    recText.Clear();
                    recInst.Clear();
                }
            }
        }

        /// <summary>
        /// 清除图表
        /// </summary>
        void ClearChart()
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet != null)
            {
                using (_excel.Defer())
                {
                    sheet.Charts.Clear();
                    _excel.DecorationRange = null;
                }
            }
        }
        #endregion
    }
}
