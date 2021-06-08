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
    public partial class UseCase : Win
    {
        public UseCase()
        {
            InitializeComponent();
        }

        async void OnLoadFile(object sender, RoutedEventArgs e)
        {
            var file = ((Button)sender).Tag.ToString();
            using(var stream = ResKit.GetResource("Excel." + file))
            {
                if (file.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
                    await _excel.OpenExcel(stream);
                else
                    await _excel.OpenXml(stream);
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

        async void SaveCsvFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileSavePicker();
            filePicker.FileTypeChoices.Add("Csv文件", new List<string>(new string[] { ".csv" }));
            filePicker.SuggestedFileName = "新文件";
            StorageFile storageFile = await filePicker.PickSaveFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForWriteAsync();
                await _excel.SaveCSV(_excel.ActiveSheetIndex, stream);
                stream.Dispose();
                Kit.Msg("导出成功！");
            }
        }

        async void OpenFile(object sender, RoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.FileTypeFilter.Add(".xlsx");
            filePicker.FileTypeFilter.Add(".xml");
            filePicker.FileTypeFilter.Add(".csv");
            filePicker.FileTypeFilter.Add(".xls");
            StorageFile storageFile = await filePicker.PickSingleFileAsync();
            if (storageFile != null)
            {
                using (var stream = await storageFile.OpenStreamForReadAsync())
                {
                    switch (storageFile.FileType.ToLower())
                    {
                        case ".xml":
                        case ".ssxml":
                            await _excel.OpenXml(stream);
                            break;

                        case ".xlsx":
                        case ".xls":
                            await _excel.OpenExcel(stream);
                            break;

                        case ".csv":
                            await _excel.OpenCSV(_excel.ActiveSheetIndex, stream);
                            break;
                    }
                }
            }
        }

        void OnConditionalFormat(object sender, RoutedEventArgs e)
        {
            DetachEvent();

            using (_excel.Defer())
            {
                Worksheet sheet = _excel.ActiveSheet;
                sheet.Columns[0].Width = 5;
                sheet.Columns[7].Width = 5;

                // sample title
                sheet.AddSpanCell(1, 1, 1, 13);
                sheet.SetValue(1, 1, "Conditional Format Samples");
                sheet[1, 1].FontSize = 24;
                sheet[1, 1].HorizontalAlignment = CellHorizontalAlignment.Center;
                _excel.AutoFitRow(1);

                // cell value rule
                int r = 3; int c = 1; int w = 6; int h = 4;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays green background if cell value is greater than 100:");
                int increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + 4, col + 1, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                var cvRule = new CellValueRule();
                cvRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                cvRule.Operator = ComparisonOperator.GreaterThan;
                cvRule.Value1 = 100;
                cvRule.Style = new StyleInfo() { Background = new SolidColorBrush(Color.FromArgb(50, 0, 255, 0)) };
                sheet.ConditionalFormats.AddRule(cvRule);

                r = 3; c = 8;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays red background if cell value is between 60 to 120:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                cvRule = new CellValueRule();
                cvRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                cvRule.Operator = ComparisonOperator.Between;
                cvRule.Value1 = 60;
                cvRule.Value2 = 120;
                cvRule.Style = new StyleInfo() { Background = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)) };
                sheet.ConditionalFormats.AddRule(cvRule);

                // 2 color scale rule
                r = 8; c = 1;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays two color gradient represents cell value:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                var tcsRule = TwoColorScaleRule.Create(ScaleValueType.LowestValue, null, Color.FromArgb(100, 255, 0, 0), ScaleValueType.HighestValue, null, Color.FromArgb(100, 0, 0, 255));
                tcsRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                sheet.ConditionalFormats.AddRule(tcsRule);

                // 3 color scale rule
                r = 8; c = 8;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays three color gradient represents cell value:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                var threecsRule = ThreeColorScaleRule.Create(ScaleValueType.LowestValue, null, Color.FromArgb(100, 255, 0, 0),
                    ScaleValueType.Number, 100, Color.FromArgb(100, 0, 255, 0),
                    ScaleValueType.HighestValue, null, Color.FromArgb(100, 0, 0, 255));
                threecsRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                sheet.ConditionalFormats.AddRule(threecsRule);

                // date occurring rule
                r = 13; c = 1;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Display blue background if cell value is in next week:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, DateTime.Now.AddDays(increase));
                        increase += 1;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                sheet[r, c, r + h, c + w].Formatter = new GeneralFormatter("mm/dd");
                var doRule = new DateOccurringRule();
                doRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                doRule.Operator = DateOccurringType.NextWeek;
                doRule.Style = new StyleInfo() { Background = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255)) };
                sheet.ConditionalFormats.AddRule(doRule);

                // specific text rule
                r = 13; c = 8;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Display red foreground if cell value contains \"o\":");
                var data = new string[] { "The", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog", "The", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog" };
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, data[increase]);
                        increase += 1;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                var stRule = new SpecificTextRule();
                stRule.Ranges = new CellRange[] { new CellRange(r + 1, c, h - 1, w) };
                stRule.Operator = TextComparisonOperator.Contains;
                stRule.Text = "o";
                stRule.Style = new StyleInfo() { Foreground = new SolidColorBrush(Colors.Red), FontWeight = FontWeights.Bold };
                sheet.ConditionalFormats.AddRule(stRule);

                // data bar rule
                r = 18; c = 1;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays a colored data bar represents cell value:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                sheet.ConditionalFormats.AddDataBarRule(ScaleValueType.LowestValue, null, ScaleValueType.HighestValue, null, Colors.Green, new CellRange(r + 1, c, h - 1, w)); ;

                // icon set rule
                r = 18; c = 8;
                sheet.AddSpanCell(r, c, 1, w);
                sheet.SetValue(r, c, "Displays an icon represents cell value:");
                increase = 0;
                for (int row = 0; row < h - 1; row++)
                {
                    for (int col = 0; col < w; col++)
                    {
                        sheet.SetValue(row + r + 1, col + c, increase);
                        increase += 10;
                    }
                }
                sheet.SetBorder(new CellRange(r, c, h, w), new BorderLine(Colors.Black, BorderLineStyle.Dashed), SetBorderOptions.All);
                sheet.ConditionalFormats.AddIconSetRule(IconSetType.FiveArrowsColored, new CellRange(r + 1, c, h - 1, w));
            }
        }

        void DetachEvent()
        {
            _excel.ActiveSheetChanged -= new System.EventHandler(gcSpreadSheet1_ActiveSheetChanged);
            _excel.CellClick -= new EventHandler<CellClickEventArgs>(gcSpreadSheet1_CellClick);
            _excel.KeyDown -= new KeyEventHandler(gcSpreadSheet1_KeyDown);
        }

        void gcSpreadSheet1_ActiveSheetChanged(object sender, System.EventArgs e)
        {
            _excel.CurrentThemeName = _excel.Themes[_excel.ActiveSheetIndex].Name;
        }

        void gcSpreadSheet1_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Space)
            {
                if (_excel.ActiveSheet.ActiveCell.Text == "☐")
                {
                    _excel.ActiveSheet.ActiveCell.Text = "☑";
                    e.Handled = true;
                }
                else if (_excel.ActiveSheet.ActiveCell.Text == "☑")
                {
                    _excel.ActiveSheet.ActiveCell.Text = "☐";
                    e.Handled = true;
                }
            }

        }

        void gcSpreadSheet1_CellClick(object sender, CellClickEventArgs e)
        {
            var sheet = _excel.ActiveSheet;

            if (sheet[e.Row, e.Column].Text == "☐")
                sheet[e.Row, e.Column].Text = "☑";
            else if (sheet[e.Row, e.Column].Text == "☑")
                sheet[e.Row, e.Column].Text = "☐";
        }

    }
}