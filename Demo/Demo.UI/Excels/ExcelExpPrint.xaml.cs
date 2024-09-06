#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2024-03-19 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage;
#endregion

namespace Demo.UI
{
    public partial class ExcelExpPrint : Tab
    {
        Excel _excel;
        PrintInfo _printInfo = new PrintInfo();

        public ExcelExpPrint()
        {
            InitializeComponent();
            _fv.Data = _printInfo;
        }

        protected override void OnFirstLoaded()
        {
            _excel = OwnWin.FindChildByType<Excel>();
        }

        async void OpenFile(object sender, RoutedEventArgs e)
        {
            var filePicker = Kit.GetFileOpenPicker();
            filePicker.FileTypeFilter.Add(".xls");
            filePicker.FileTypeFilter.Add(".xlsx");
            filePicker.FileTypeFilter.Add(".xml");
            StorageFile storageFile = await filePicker.PickSingleFileAsync();
            if (storageFile != null)
            {
                using (var stream = await storageFile.OpenStreamForReadAsync())
                {
                    if (storageFile.FileType.ToLower() == ".xml")
                        await _excel.OpenXml(stream);
                    else
                        await _excel.OpenExcel(stream);
                }
            }
        }

        async void SaveExcelFile(object sender, RoutedEventArgs e)
        {
            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
            filePicker.FileTypeChoices.Add("Excel 97-2003 Files", new List<string>(new string[] { ".xls" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                using (var stream = await storageFile.OpenStreamForWriteAsync())
                {
                    var fileName = storageFile.FileType.ToUpperInvariant();
                    var fileFormat = ExcelFileFormat.XLS;
                    if (fileName.EndsWith(".XLSX"))
                        fileFormat = ExcelFileFormat.XLSX;
                    else
                        fileFormat = ExcelFileFormat.XLS;
                    await _excel.SaveExcel(stream, fileFormat);
                    Kit.Msg("导出成功！");
                }
            }
        }

        async void SaveCsvFile(object sender, RoutedEventArgs e)
        {
            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("CSV文件", new List<string>(new string[] { ".csv" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                using (var stream = await storageFile.OpenStreamForWriteAsync())
                {
                    await _excel.SaveCSV(_excel.ActiveSheetIndex, stream, TextFileSaveFlags.AsViewed);
                    Kit.Msg("导出成功！");
                }
            }
        }

        async void SaveXmlFile(object sender, RoutedEventArgs e)
        {
            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("Xml文件", new List<string>(new string[] { ".xml" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                using (var stream = await storageFile.OpenStreamForWriteAsync())
                {
                    await _excel.SaveXmlAsync(stream);
                    Kit.Msg("导出成功！");
                }
            }
        }

        Dlg _dlg;
        PdfView _pdfView;
        
        async void OnPreviewPdf(object sender, RoutedEventArgs e)
        {
            ShowPreviewDlg();

            ClearArea();
            foreach (var ws in _excel.Sheets)
            {
                ws.PrintInfo = _printInfo;
            }
            MemoryStream ms = new();
            await _excel.SavePdf(ms);
            _pdfView.Open(ms.ToArray());
            ms.Close();
        }

        async void OnPreviewArea(object sender, RoutedEventArgs e)
        {
            var ws = _excel.ActiveSheet;
            if (ws.Selections.Count == 0)
            {
                Kit.Warn("无选择区域");
                return;
            }

            ShowPreviewDlg();
            ws.PrintInfo = _printInfo;

            // 有冻结行列时需要先取消冻结
            int fColCnt = ws.FrozenColumnCount;
            int fRowCnt = ws.FrozenRowCount;
            int fColTail = ws.FrozenTrailingColumnCount;
            int fRowTail = ws.FrozenTrailingRowCount;
            ws.FrozenColumnCount = 0;
            ws.FrozenRowCount = 0;
            ws.FrozenTrailingColumnCount = 0;
            ws.FrozenTrailingRowCount = 0;

            MemoryStream ms = new();
            await _excel.SavePdf(ms, ws.Selections[0]);
            _pdfView.Open(ms.ToArray());
            ms.Close();
            
            ws.FrozenColumnCount = fColCnt;
            ws.FrozenRowCount = fRowCnt;
            ws.FrozenTrailingColumnCount = fColTail;
            ws.FrozenTrailingRowCount = fRowTail;
        }

        void ShowPreviewDlg()
        {
            if (_dlg == null)
            {
                _dlg = new Dlg();
                if (!Kit.IsPhoneUI)
                {
                    _dlg.Width = 1000;
                    _dlg.Height = Kit.ViewHeight - 200;
                }
                _pdfView = new PdfView();
                _dlg.Content = _pdfView;
            }
            _dlg.Show();
        }
        
        async void OnSavePdf(object sender, RoutedEventArgs e)
        {
            ClearArea();
            foreach (var ws in _excel.Sheets)
            {
                ws.PrintInfo = _printInfo;
            }

            var stream = await GetSavePdfFile();
            if (stream != null)
            {
                await _excel.SavePdf(stream);
                stream.Close();
                Kit.Msg("导出成功！");
            }
        }

        async void OnSavePdfArea(object sender, RoutedEventArgs e)
        {
            var ws = _excel.ActiveSheet;
            if (ws.Selections.Count == 0)
            {
                Kit.Warn("无选择区域");
                return;
            }

            var stream = await GetSavePdfFile();
            if (stream != null)
            {
                ws.PrintInfo = _printInfo;
                
                // 有冻结行列时需要先取消冻结
                int fColCnt = ws.FrozenColumnCount;
                int fRowCnt = ws.FrozenRowCount;
                int fColTail = ws.FrozenTrailingColumnCount;
                int fRowTail = ws.FrozenTrailingRowCount;
                ws.FrozenColumnCount = 0;
                ws.FrozenRowCount = 0;
                ws.FrozenTrailingColumnCount = 0;
                ws.FrozenTrailingRowCount = 0;
                
                await _excel.SavePdf(stream, ws.Selections[0]);
                stream.Close();

                ws.FrozenColumnCount = fColCnt;
                ws.FrozenRowCount = fRowCnt;
                ws.FrozenTrailingColumnCount = fColTail;
                ws.FrozenTrailingRowCount = fRowTail;
                Kit.Msg("导出成功！");
            }
        }

        void OnPrintExcel(object sender, RoutedEventArgs e)
        {
            ClearArea();
            _excel.Print(_printInfo);
        }

        void OnPrintArea(object sender, RoutedEventArgs e)
        {
            if (SetArea())
                _excel.Print(_printInfo);
        }
        
        bool SetArea()
        {
            var ws = _excel.ActiveSheet;
            if (ws.Selections.Count == 0)
            {
                Kit.Warn("无选择区域");
                return false;
            }

            CellRange range = ws.Selections[0];
            _printInfo.RowStart = range.Row;
            _printInfo.RowEnd = range.Row + range.RowCount - 1;
            _printInfo.ColumnStart = range.Column;
            _printInfo.ColumnEnd = range.Column + range.ColumnCount - 1;
            return true;
        }

        void ClearArea()
        {
            _printInfo.RowStart = -1;
            _printInfo.RowEnd = -1;
            _printInfo.ColumnStart = -1;
            _printInfo.ColumnEnd = -1;
        }

        async Task<Stream> GetSavePdfFile()
        {
            var filePicker = Kit.GetFileSavePicker();
            filePicker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
                return await storageFile.OpenStreamForWriteAsync();
            return null;
        }
    }
}