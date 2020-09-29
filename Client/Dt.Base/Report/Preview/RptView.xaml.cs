#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2020-09-18 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Core;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;
#endregion

namespace Dt.Base.Report
{
    public sealed partial class RptView : UserControl
    {
        #region 成员变量
        BaseCommand _cmdExport;
        BaseCommand _cmdPrint;
        BaseCommand _cmdSearch;
        BaseCommand _cmdGridLine;
        BaseCommand _cmdColHeader;
        BaseCommand _cmdRowHeader;
        BaseCommand _cmdClearTable;
        BaseCommand _cmdClearChart;
        #endregion

        public RptView()
        {
            InitializeComponent();

            _excel.Sheets.Clear();
            _excel.CellClick += OnCellClick;
            _excel.SelectionChanged += OnSelectionChanged;
        }

        /// <summary>
        /// 对应的报表描述信息
        /// </summary>
        public RptInfo Info { get; private set; }

        /// <summary>
        /// 内部Excel
        /// </summary>
        public Excel Excel
        {
            get { return _excel; }
        }

        /// <summary>
        /// 加载报表内容，前提条件:
        /// <para>报表模板名称</para>
        /// <para>确保查询参数完备</para>
        /// </summary>
        /// <param name="p_info">报表描述信息</param>
        public async void LoadReport(RptInfo p_info)
        {
            // 确保正确加载模板，参数完备
            if (p_info == null
                || !await p_info.Init()
                || !p_info.IsParamsValid())
                return;

            _excel.IsBusy = true;
            try
            {
                Info = p_info;

                // 绘制报表内容
                if (Info.Sheet == null)
                {
                    RptRootInst inst = new RptRootInst(Info);
                    await inst.Draw();
                }

                using (_excel.Defer())
                {
                    _excel.Sheets.Clear();

                    // 设置分页线
                    _excel.PaperSize = new Size(Info.Root.Setting.ValidWidth, Info.Root.Setting.ValidHeight);

                    // 添加报表页
                    var ws = Info.Sheet;
                    if (ws != null)
                    {
                        // 应用外部可控制属性
                        ws.ColumnHeader.IsVisible = Info.Root.ViewSetting.ShowColHeader;
                        ws.RowHeader.IsVisible = Info.Root.ViewSetting.ShowRowHeader;
                        ws.ShowGridLine = Info.Root.ViewSetting.ShowGridLine;
                        _excel.Sheets.Add(ws);
                    }
                }
            }
            catch
            {
                AtKit.Warn("报表绘制异常！");
            }
            finally
            {
                _excel.IsBusy = false;
            }
        }

        /// <summary>
        /// 点击单元格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnCellClick(object sender, CellClickEventArgs e)
        {
            RptTextInst inst;
            if (Info.ScriptObj != null
                && (inst = _excel.ActiveSheet[e.Row, e.Column].Tag as RptTextInst) != null
                && inst.Item is RptText txt
                && !string.IsNullOrEmpty(txt.ScriptID))
            {
                inst.Row = e.Row;
                inst.Col = e.Column;
                Info.ScriptObj.OnCellClick(txt.ScriptID, inst);
            }
        }

        /// <summary>
        /// 选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSelectionChanged(object sender, EventArgs e)
        {
            //object selectedItem = null;
            //if (_previewMenu != null && _previewMenu.IsOpened)
            //    _previewMenu.Close();

            //Worksheet sheet = _excel.ActiveSheet;
            //if (sheet.Selections.Count == 0)
            //    return;

            //CellRange range = sheet.Selections[0];
            //SheetTable[] st = sheet.GetTables();
            //foreach (SheetTable tbl in st)
            //{
            //    if (tbl.Range.Intersects(range.Row, range.Column, range.RowCount, range.ColumnCount))
            //    {
            //        selectedItem = tbl;
            //        break;
            //    }
            //}
            //if (selectedItem == null && range.RowCount == 1 && range.ColumnCount == 1)
            //    return;

            //if (_previewMenu == null)
            //    _previewMenu = new PreviewMenu(this);
            //_previewMenu.SelectedItem = selectedItem;
            //_previewMenu.Show(this, range);
        }

        #region 命令
        /// <summary>
        /// 获取导出命令对象
        /// </summary>
        public BaseCommand CmdExport
        {
            get
            {
                if (_cmdExport == null)
                    _cmdExport = new BaseCommand((p_param) => { DoExport(); });
                return _cmdExport;
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
                    _cmdPrint = new BaseCommand((p_param) => { DoPrint(); });
                return _cmdPrint;
            }
        }

        /// <summary>
        /// 获取查询命令对象
        /// </summary>
        public BaseCommand CmdSearch
        {
            get
            {
                if (_cmdSearch == null)
                    _cmdSearch = new BaseCommand((p_param) => { DoSearch(); });
                return _cmdSearch;
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
                    _cmdGridLine = new BaseCommand((p_param) =>
                    {
                        if (_excel.ActiveSheet != null)
                            DoShowGridLine(!_excel.ActiveSheet.ShowGridLine);
                    });
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
                    _cmdClearTable = new BaseCommand((p_param) => { ClearTable(); });
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
                    _cmdClearChart = new BaseCommand((p_param) => { ClearChart(); });
                return _cmdClearChart;
            }
        }

        /// <summary>
        /// 导出
        /// </summary>
        public async void DoExport()
        {
            if (_excel.ActiveSheet == null)
                return;

            var filePicker = new FileSavePicker();
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
                AtKit.Msg("导出成功！");
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        public void DoPrint()
        {
            if (_excel.ActiveSheet != null)
            {
                PrintInfo info = new PrintInfo();
                RptSetting setting = Info.Root.Setting;
                info.PaperSize = setting.PaperName == "PrinterCustom" ? new PaperSize(setting.Width, setting.Height) : new PaperSize((PrintMediaSize)Enum.Parse(typeof(PrintMediaSize), setting.PaperName));
                info.Margin = new Margins((int)setting.TopMargin, (int)setting.BottomMargin, (int)setting.LeftMargin, (int)setting.RightMargin);
                info.Orientation = setting.Landscape ? PrintPageOrientation.Landscape : PrintPageOrientation.Portrait;
                info.ShowBorder = false;
                info.PageOrder = PrintPageOrder.OverThenDown;
                Worksheet sheet = _excel.ActiveSheet;
                info.ShowRowHeader = sheet.RowHeader.IsVisible ? VisibilityType.Show : VisibilityType.Hide;
                info.ShowColumnHeader = sheet.ColumnHeader.IsVisible ? VisibilityType.Show : VisibilityType.Hide;
                info.ShowGridLine = sheet.ShowGridLine;

                _excel.Print(info, -1, Info.Name);
            }
        }

        /// <summary>
        /// 查询
        /// </summary>
        public void DoSearch()
        {
            //if (_info == null || _info.Root == null)
            //    return;

            //// 可能存在报表链接，始终构造
            //bool has = (from c in _info.Root.Params.Data
            //            where !c.Bool("hide")
            //            select c).Any();
            //if (has)
            //{
            //    var fm = new RptParamsForm();
            //    fm.LoadParams(_info);
            //    Dialog dlg = new Dialog();
            //    dlg.Content = fm;
            //    dlg.PopInit += (sender, pop) => pop.StartPosition = PopStartPosition.CenterScreen;
            //    fm.Query += (sender, e) =>
            //    {
            //        LoadReport(_info);
            //        dlg.Close();
            //    };
            //    dlg.Show(null, "查询");
            //}
            //else
            //{
            //    // 无参数或都隐藏时刷新
            //    _info.Sheet = null;
            //    _info.DataSet = null;
            //    LoadReport(_info);
            //}
        }

        /// <summary>
        /// 显示网格
        /// </summary>
        /// <param name="p_show"></param>
        public void DoShowGridLine(bool p_show)
        {
            var st = _excel.ActiveSheet;
            if (st != null)
                st.ShowGridLine = p_show;
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
        /// 清除表格。
        /// </summary>
        private void ClearTable()
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet == null)
                return;

            List<string> recText = new List<string>();
            List<object> recInst = new List<object>();
            int idx = 0;
            RptTextInst inst;
            _excel.AutoRefresh = false;
            _excel.SuspendEvent();
            Dt.Cells.Data.Cell cell = null;
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
                            AtKit.RunSync(() => { (inst.Item as RptText).ApplyStyle(sheet[i, j]); });
                        }
                        idx++;
                    }
                }
                recText.Clear();
                recInst.Clear();
            }
            _excel.ResumeEvent();
            _excel.AutoRefresh = true;
        }

        /// <summary>
        /// 清除图表
        /// </summary>
        private void ClearChart()
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet != null)
            {
                sheet.Charts.Clear();
                _excel.DecorationRange = null;
            }
        }
        #endregion
    }
}
