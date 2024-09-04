using Dt.Base;
using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Infras.Demo
{
    public sealed partial class ExportPdfPage : Page
    {
        public ExportPdfPage()
        {
            this.InitializeComponent();
            // 设置分页线
            ExcelKit.SnapBorder = _snap;
            _excel.ShowDecoration = true;
            LoadData();
        }

        void OnBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        async void OpenFile(object sender, RoutedEventArgs e)
        {
            var picker = GetFileOpenPicker();
            picker.FileTypeFilter.Add(".xls");
            picker.FileTypeFilter.Add(".xlsx");
            picker.FileTypeFilter.Add(".xml");
            StorageFile storageFile = await picker.PickSingleFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForReadAsync();
                if (storageFile.FileType.ToLower() == ".xml")
                    await _excel.OpenXml(stream);
                else
                    await _excel.OpenExcel(stream);
                stream.Dispose();
            }
            _excel.ShowDecoration = true;
        }

        async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = GetFileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file == null)
                return;

            int startRow = 0;
            int startColumn = 0;
            Worksheet sheet = _excel.ActiveSheet;
            int selectCount = sheet.Selections.Count;
            if (selectCount >= 1)
            {
                CellRange cellRange = sheet.Selections[selectCount - 1];
                startRow = cellRange.Row;
                startColumn = cellRange.Column;
            }
            try
            {
                _excel.SuspendEvent();
                var stream = await file.OpenStreamForReadAsync();
                sheet.AddPicture(CreatePictureName(), stream);
                stream.Dispose();
            }
            finally
            {
                _excel.ResumeEvent();
                _excel.RefreshPictures();
            }
        }

        string CreatePictureName()
        {
            SpreadPictures picutres = _excel.ActiveSheet.Pictures;
            return "Picture" + (picutres.Count > 0 ? Int32.Parse(picutres[picutres.Count - 1].Name.Substring(7)) + 1 : 1);
        }

        async void OnExport(object sender, RoutedEventArgs e)
        {
            var picker = GetFileSavePicker();
            picker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            picker.SuggestedFileName = "新文件";
            StorageFile storageFile = await picker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SavePdf(stream);
                stream.Dispose();
            }
        }

        async void OnExportArea(object sender, RoutedEventArgs e)
        {
            var ws = _excel.ActiveSheet;
            if (ws.Selections.Count == 0)
                return;
            
            var picker = GetFileSavePicker();
            picker.FileTypeChoices.Add("PDF文件", new List<string>(new string[] { ".pdf" }));
            picker.SuggestedFileName = "新文件";
            StorageFile storageFile = await picker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SavePdf(stream, ws.Selections[0]);
                stream.Dispose();
            }
        }
        
        async void OnSave(object sender, RoutedEventArgs e)
        {
            var picker = GetFileSavePicker();
            picker.FileTypeChoices.Add("Excel Files", new List<string>(new string[] { ".xlsx" }));
            picker.FileTypeChoices.Add("Xml文件", new List<string>(new string[] { ".xml" }));
            picker.FileTypeChoices.Add("Excel 97-2003 Files", new List<string>(new string[] { ".xls" }));
            picker.SuggestedFileName = "新文件";
            StorageFile storageFile = await picker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                var fileName = storageFile.FileType.ToUpperInvariant();
                if (fileName.EndsWith(".XML"))
                {
                    await _excel.SaveXmlAsync(stream);
                }
                else
                {
                    var fileFormat = ExcelFileFormat.XLS;
                    if (fileName.EndsWith(".XLSX"))
                        fileFormat = ExcelFileFormat.XLSX;
                    else
                        fileFormat = ExcelFileFormat.XLS;
                    await _excel.SaveExcel(stream, fileFormat);
                }
                stream.Dispose();
            }
        }

        void OnPrint(object sender, RoutedEventArgs e)
        {
            _excel.Print();
        }

        void OnPrint0(object sender, RoutedEventArgs e)
        {
            _excel.Print(new PrintInfo { Margin = new Margins(0, 0, 0, 0) });
        }

        void OnPrintArea(object sender, RoutedEventArgs e)
        {
            var ws = _excel.ActiveSheet;
            if (ws.Selections.Count > 0)
            {
                _excel.Print(ws.Selections[0]);
            }
        }

        void LoadData()
        {
            var ff = new Microsoft.UI.Xaml.Media.FontFamily("ms-appx:///icon.ttf#DtIcon");
            var ws = _excel.Sheets[0];
            var cell = ws[0, 0];
            cell.Value = "\uE001";
            cell.FontFamily = ff;
            cell.FontSize = 20;

            cell = ws[0, 1];
            cell.Value = "\uE002";
            cell.FontFamily = ff;

            cell = ws[2, 0];
            cell.Value = "中文";

            cell = ws[2, 2];
            cell.Value = "abc";

            ws.SetValue(4, 1, "NC");
            ws.SetValue(4, 2, "Raleigh");
            ws.SetValue(7, 2, "Charlotte");
            ws.SetValue(4, 3, "001");
            ws.SetValue(5, 3, "002");
            ws.SetValue(6, 3, "003");
            ws.SetValue(7, 3, "004");
            ws.SetValue(8, 3, "005");
            ws.SetValue(9, 3, "006");
            ws.SetValue(11, 1, "PA");
            ws.SetValue(11, 2, "Philadelphia");
            ws.SetValue(14, 2, "Pittsburgh");
            ws.SetValue(11, 3, "007");
            ws.SetValue(12, 3, "008");
            ws.SetValue(13, 3, "009");
            ws.SetValue(14, 3, "010");
            ws.SetValue(15, 3, "011");
            ws.SetValue(16, 3, "012");
            //var assembly = typeof(FilePage).Assembly;
            //using (var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Res.img.xlsx"))
            //{
            //    await _excel.OpenExcel(stream);
            //}
            //var _cachedStream = new MemoryStream();
            //await _excel.SaveXmlAsync(_cachedStream);
            //_cachedStream.Seek(0L, SeekOrigin.Begin);
            //await _excel.OpenXml(_cachedStream);

            //object[,] values = {{"","North","South","East","West"},
            //                    {"s1",50,25,55,30},
            //                    {"s2",92,24,15,24},
            //                    {"s3",65,26,70,60},
            //                    {"s4",24,80,26,20} };
            //_excel.Sheets[0].SetArray(0, 0, values);

            //_excel.Sheets[0].Name = "Column";
            //_excel.Sheets[0].AddChart("Chart1", SpreadChartType.ColumnClustered, "Column!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[0].AddChart("Chart2", SpreadChartType.ColumnStacked, "Column!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[0].AddChart("Chart3", SpreadChartType.ColumnStacked100pc, "Column!$A$1:$E$5", 30, 440, 400, 290);
        }

        FileOpenPicker GetFileOpenPicker()
        {
            var picker = new FileOpenPicker();

#if WIN
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Dt.Base.ExcelKit.MainWin);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
#endif
            return picker;
        }

        FileSavePicker GetFileSavePicker()
        {
            var picker = new FileSavePicker();

#if WIN
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(Dt.Base.ExcelKit.MainWin);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
#endif
            return picker;
        }

        void AddBase64Pic(object sender, RoutedEventArgs e)
        {
            _excel.SuspendEvent();
            var ws = _excel.Sheets[0];
            string data = "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G5bPXMqOn76Yn/rPJwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACAwAYDABnUvwLnL07/DrEsO7pvNZmfyw4Si/8zK3lo8tHsBBhWAof3jlHXq2HzpN/TiiVfzZ/F4j7kEgNAEAAITAAhMACAwAYDABAACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACAwAYDABAACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACAwAYDABAACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACAwAYDABAACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACAwAYDABAACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAKbWVg+u5I/75Slxb3Z8pH9+WkyZ965kZ1641p+imdpcU+2ODebnyazfOZS/iyexbmdq++zMtLP6/jpi/mp/wYVAGjb0ALgEgACEwAITAAgMAGAwAQAAhMACEwAIDABgMAEAAITAAhMACCwzt4LkJS9UQPaduHqrezlC9fzU/91OgBAu1wCQGACAIEJAAQmABCYAEBgAgCBCQAEJgAQmABAYAIAgQkABCYAEJgAQGACAIEJAAQmABCYAEBgAgCBCQAEJgAQmABAYAIAgQkABCYAEJgAQGACAIEJAAQmABCYAEBgAgCBCQAEJgAQmABAYAIAgQkABCYAEJgAQFhZ9n+zPA2MtLkRWQAAAABJRU5ErkJggg==";
            var ImgData = Convert.FromBase64String(data);
            var pic = ws.AddPicture(
                    Guid.NewGuid().ToString().Substring(0, 6),
                    new MemoryStream(ImgData),
                    0,
                    0,
                    0,
                    0,
                    10,
                    0,
                    5,
                    0);
            _excel.ResumeEvent();
            _excel.RefreshPictures();
        }
    }
}
