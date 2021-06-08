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
    public partial class FilterExcel : Win
    {
        HideRowFilter _filter = null;

        public FilterExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitializeSample();
            }
        }
        void InitializeSample()
        {
            Worksheet sheet = _excel.Sheets[0];
            sheet.RowCount = 50;
            sheet.ColumnCount = 8;
            int rc = sheet.RowCount; int cc = sheet.ColumnCount;

            for (int r = 0; r < rc; r++)
            {
                for (int c = 0; c < cc; c++)
                {
                    if (c == 0) sheet.SetValue(r, c, "Value is Number");
                    else sheet.SetValue(r, c, r + c);
                }
            }


            sheet.SetValue(0, 0, SheetArea.ColumnHeader, "Conditions");
            sheet.SetValue(0, 1, SheetArea.ColumnHeader, "Cell Value");

            sheet.SetValue(1, 0, "Text contains e");
            sheet.SetValue(10, 0, "Text doesn't contains e");
            sheet.SetValue(21, 0, "Text contains e");
            sheet.SetValue(1, 1, "begin");
            sheet.SetValue(10, 1, "during");
            sheet.SetValue(21, 1, "end");

            sheet.SetValue(2, 0, "Background is Cyan");
            sheet.SetValue(6, 0, "Background is Purple");
            sheet.SetValue(12, 0, "Background is Cyan");
            sheet.Cells[2, 1].Background = new SolidColorBrush(Colors.Cyan);
            sheet.Cells[6, 1].Background = new SolidColorBrush(Colors.Purple);
            sheet.Cells[12, 1].Background = new SolidColorBrush(Colors.Cyan);

            sheet.SetValue(3, 0, "Value is Thursday");
            sheet.SetValue(8, 0, "Value is Friday");
            sheet.SetValue(14, 0, "Value is Thursday");
            sheet.SetValue(3, 1, new DateTime(2011, 6, 30));
            sheet.SetValue(8, 1, new DateTime(2011, 7, 1));
            sheet.SetValue(14, 1, new DateTime(2011, 6, 30));

            sheet.SetValue(4, 0, "Value is null");
            sheet.SetValue(9, 0, "Value is null");
            sheet.SetValue(18, 0, "Value is null");
            sheet.SetValue(4, 1, null);
            sheet.SetValue(9, 1, null);
            sheet.SetValue(18, 1, null);

            sheet.Columns[0].Width = 150;
            sheet.Columns[1].Width = 150;

            _filter = new HideRowFilter(new CellRange(-1, 0, -1, 2));
            _filter.ShowFilterButton = false;

            sheet.RowFilter = _filter;
        }


        void AddCondition(HideRowFilter drf)
        {
            if (_cbText.IsChecked == true)
            {
                TextCondition tc = TextCondition.FromString(TextCompareType.Contains, "*e*");
                drf.AddFilterItem(1, tc);
            }
            if (_cbStyle.IsChecked == true)
            {
                ColorCondition sc = ColorCondition.FromColor(ColorCompareType.BackgroundColor, Colors.Cyan);
                drf.AddFilterItem(1, sc);
            }
            if (_cbNum.IsChecked == true)
            {
                NumberCondition nc = NumberCondition.FromDouble(GeneralCompareType.LessThan, 20);
                drf.AddFilterItem(1, nc);
            }
            if (_cbDate.IsChecked == true)
            {
                DateExCondition dc = DateExCondition.FromWeek(DayOfWeek.Thursday);
                drf.AddFilterItem(1, dc);
            }
            if (_cbNull.IsChecked == true)
            {
                FormulaCondition cx = FormulaCondition.FromType(CustomValueType.Empty);
                drf.AddFilterItem(1, cx);
            }
        }

        void Condition_Checked(object sender, RoutedEventArgs e)
        {
            if (_filter != null)
            {
                _filter.ClearFilterItems();
                AddCondition(_filter);
                _filter.Filter(1);
            }
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
                await _excel.SaveExcel(stream, fileFormat, ExcelSaveFlags.NoFlagsSet);
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
    }
}