#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-12-16 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Base;
using Dt.Cells.Data;
using Dt.Cells.UI;
using Dt.Charts;
using Dt.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class ImgExcel : Win
    {
        public ImgExcel()
        {
            InitializeComponent();
        }

        async void AddPictureButton_Click(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".jpg");
            filePicker.FileTypeFilter.Add(".jpeg");
            filePicker.FileTypeFilter.Add(".png");
            filePicker.FileTypeFilter.Add(".bmp");
            filePicker.FileTypeFilter.Add(".gif");
            StorageFile file = await filePicker.PickSingleFileAsync();
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

        async void SaveExcelFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
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
                await _excel.SaveExcel(stream, fileFormat, GetSaveFlag());
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SavePDFFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
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

        async void SaveCsvFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("CSV文件", new List<string>(new string[] { ".csv" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveCSV(_excel.ActiveSheetIndex, stream, TextFileSaveFlags.AsViewed);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void SaveXmlFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Xml文件", new List<string>(new string[] { ".xml" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveXmlAsync(stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        ExcelSaveFlags GetSaveFlag()
        {
            var flagText = (_saveFlags.SelectedItem as ComboBoxItem).Content.ToString();
            var result = ExcelSaveFlags.NoFlagsSet;
            Enum.TryParse<ExcelSaveFlags>(flagText, true, out result);
            return result;
        }

        void OnPrintExcel(object sender, RoutedEventArgs e)
        {
            _excel.Print();
        }
    }
}