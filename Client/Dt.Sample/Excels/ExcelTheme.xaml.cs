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
    public partial class ExcelTheme : Win
    {
        public ExcelTheme()
        {
            InitializeComponent();
            Worksheet sheet = _excel.ActiveSheet;

            using (_excel.Defer())
            {
                InitializeSample();
            }

            foreach (SpreadTheme st in _excel.Themes)
            {
                comboThemes.Items.Add(st.Name);
            }

            _excel.ValueChanged += gcSpreadSheet1_ValueChanged;

            comboThemes.SelectedItem = _excel.CurrentThemeName;

            ThemeColorType.SelectionChanged += new SelectionChangedEventHandler(ThemeColorType_SelectionChanged);
            ThemeColorType.Items.Add("Background");
            ThemeColorType.Items.Add("Text");
            ThemeColorType.Items.Add("Accent");
            ThemeColorType.SelectedIndex = 2;

            ThemeColorBrightness.ValueChanged += delegate { ThemeColorBrightness.Value = (int)ThemeColorBrightness.Value; };
        }

        void gcSpreadSheet1_ValueChanged(object sender, CellEventArgs e)
        {
            if (e.Row == 1 && e.Column == 3)
            {
                try
                {
                    _excel.CurrentThemeName = _excel.ActiveSheet.GetText(1, 3);
                    comboThemes.SelectedItem = _excel.CurrentThemeName;
                }
                catch { _excel.ActiveSheet.SetValue(1, 3, _excel.CurrentThemeName); }
            }
        }

        void InitializeSample()
        {
            var sheet = _excel.ActiveSheet;

            string themes = "";
            foreach (SpreadTheme st in _excel.Themes)
            {
                themes += "," + st.Name;
            }
            var dv = DataValidator.CreateListValidator(themes.Substring(1));
            sheet[1, 3].DataValidator = dv;
            sheet[1, 3].Value = _excel.CurrentThemeName;
            sheet[1, 3].ColumnSpan = 2;
            sheet.SetBorder(new CellRange(1, 3, 1, 2), new BorderLine(Colors.Red, BorderLineStyle.Dashed), SetBorderOptions.OutLine);

            sheet[1, 5].Value = "<- Change Document Theme";
            sheet[1, 5].ColumnSpan = 4;

            sheet.Columns[0].Width = 22;
            sheet.Columns[1].Width = 30;
            sheet.Columns[2].Width = 22;
            sheet[3, 3].Value = "Theme Colors (Headinds Font)";
            sheet[3, 3].FontSize = 24;
            sheet[3, 3].FontTheme = "Headings";
            sheet[3, 3].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet[3, 3].ColumnSpan = 10;
            _excel.AutoFitRow(3);

            sheet[4, 3].Value = "Background 1";
            sheet[4, 4].Value = "Text 1";
            sheet[4, 5].Value = "Background 2";
            sheet[4, 6].Value = "Text 2";
            sheet[4, 7].Value = "Accent 1";
            sheet[4, 8].Value = "Accent 2";
            sheet[4, 9].Value = "Accent 3";
            sheet[4, 10].Value = "Accent 4";
            sheet[4, 11].Value = "Accent 5";
            sheet[4, 12].Value = "Accent 6";
            sheet[4, 4, 4, 12].FontTheme = "Body";

            sheet[5, 1].Value = "100";
            sheet[5, 1].FontTheme = "Body";

            sheet[5, 3].BackgroundThemeColor = "Background 1";
            sheet[5, 4].BackgroundThemeColor = "Text 1";
            sheet[5, 5].BackgroundThemeColor = "Background 2";
            sheet[5, 6].BackgroundThemeColor = "Text 2";
            sheet[5, 7].BackgroundThemeColor = "Accent 1";
            sheet[5, 8].BackgroundThemeColor = "Accent 2";
            sheet[5, 9].BackgroundThemeColor = "Accent 3";
            sheet[5, 10].BackgroundThemeColor = "Accent 4";
            sheet[5, 11].BackgroundThemeColor = "Accent 5";
            sheet[5, 12].BackgroundThemeColor = "Accent 6";

            sheet[7, 1].Value = "80";
            sheet[7, 1].FontTheme = "Body";

            sheet[7, 3].BackgroundThemeColor = "Background 1 -5";
            sheet[7, 4].BackgroundThemeColor = "Text 1 50";
            sheet[7, 5].BackgroundThemeColor = "Background 2 -10";
            sheet[7, 6].BackgroundThemeColor = "Text 2 80";
            sheet[7, 7].BackgroundThemeColor = "Accent 1 80";
            sheet[7, 8].BackgroundThemeColor = "Accent 2 80";
            sheet[7, 9].BackgroundThemeColor = "Accent 3 80";
            sheet[7, 10].BackgroundThemeColor = "Accent 4 80";
            sheet[7, 11].BackgroundThemeColor = "Accent 5 80";
            sheet[7, 12].BackgroundThemeColor = "Accent 6 80";

            sheet[8, 1].Value = "60";
            sheet[8, 1].FontTheme = "Body";

            sheet[8, 3].BackgroundThemeColor = "Background 1 -15";
            sheet[8, 4].BackgroundThemeColor = "Text 1 35";
            sheet[8, 5].BackgroundThemeColor = "Background 2 -25";
            sheet[8, 6].BackgroundThemeColor = "Text 2 60";
            sheet[8, 7].BackgroundThemeColor = "Accent 1 60";
            sheet[8, 8].BackgroundThemeColor = "Accent 2 60";
            sheet[8, 9].BackgroundThemeColor = "Accent 3 60";
            sheet[8, 10].BackgroundThemeColor = "Accent 4 60";
            sheet[8, 11].BackgroundThemeColor = "Accent 5 60";
            sheet[8, 12].BackgroundThemeColor = "Accent 6 60";

            sheet[9, 1].Value = "40";
            sheet[9, 1].FontTheme = "Body";

            sheet[9, 3].BackgroundThemeColor = "Background 1 -25";
            sheet[9, 4].BackgroundThemeColor = "Text 1 25";
            sheet[9, 5].BackgroundThemeColor = "Background 2 -50";
            sheet[9, 6].BackgroundThemeColor = "Text 2 40";
            sheet[9, 7].BackgroundThemeColor = "Accent 1 40";
            sheet[9, 8].BackgroundThemeColor = "Accent 2 40";
            sheet[9, 9].BackgroundThemeColor = "Accent 3 40";
            sheet[9, 10].BackgroundThemeColor = "Accent 4 40";
            sheet[9, 11].BackgroundThemeColor = "Accent 5 40";
            sheet[9, 12].BackgroundThemeColor = "Accent 6 40";

            sheet[10, 1].Value = "-25";
            sheet[10, 1].FontTheme = "Body";

            sheet[10, 3].BackgroundThemeColor = "Background 1 -35";
            sheet[10, 4].BackgroundThemeColor = "Text 1 15";
            sheet[10, 5].BackgroundThemeColor = "Background 2 -75";
            sheet[10, 6].BackgroundThemeColor = "Text 2 -25";
            sheet[10, 7].BackgroundThemeColor = "Accent 1 -25";
            sheet[10, 8].BackgroundThemeColor = "Accent 2 -25";
            sheet[10, 9].BackgroundThemeColor = "Accent 3 -25";
            sheet[10, 10].BackgroundThemeColor = "Accent 4 -25";
            sheet[10, 11].BackgroundThemeColor = "Accent 5 -25";
            sheet[10, 12].BackgroundThemeColor = "Accent 6 -25";

            sheet[11, 1].Value = "-50";
            sheet[11, 1].FontTheme = "Body";

            sheet[11, 3].BackgroundThemeColor = "Background 1 -50";
            sheet[11, 4].BackgroundThemeColor = "Text 1 5";
            sheet[11, 5].BackgroundThemeColor = "Background 2 -90";
            sheet[11, 6].BackgroundThemeColor = "Text 2 -50";
            sheet[11, 7].BackgroundThemeColor = "Accent 1 -50";
            sheet[11, 8].BackgroundThemeColor = "Accent 2 -50";
            sheet[11, 9].BackgroundThemeColor = "Accent 3 -50";
            sheet[11, 10].BackgroundThemeColor = "Accent 4 -50";
            sheet[11, 11].BackgroundThemeColor = "Accent 5 -50";
            sheet[11, 12].BackgroundThemeColor = "Accent 6 -50";

            sheet.AddTable("sampleTable2", 14, 3, 6, 4, TableStyles.Medium4);
            sheet.AddTable("sampleTable3", 14, 9, 6, 4, TableStyles.Medium5);
        }

        void ThemeColorType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemeColorIndex.Items.Clear();

            ThemeColorIndex.Items.Add("1");
            ThemeColorIndex.Items.Add("2");

            if (ThemeColorType.SelectedIndex == 2)
            {
                ThemeColorIndex.Items.Add("3");
                ThemeColorIndex.Items.Add("4");
                ThemeColorIndex.Items.Add("5");
                ThemeColorIndex.Items.Add("6");
            }
            ThemeColorIndex.SelectedIndex = 0;
        }

        CellRange GetActualCellRange(CellRange cellRange, int rowCount, int columnCount)
        {
            if (cellRange.Row == -1 && cellRange.Column == -1)
            {
                return new CellRange(0, 0, rowCount, columnCount);
            }
            else if (cellRange.Row == -1)
            {
                return new CellRange(0, cellRange.Column, rowCount, cellRange.ColumnCount);
            }
            else if (cellRange.Column == -1)
            {
                return new CellRange(cellRange.Row, 0, cellRange.RowCount, columnCount);
            }

            return cellRange;
        }
        void setBackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].BackgroundThemeColor = string.Format("{0} {1} {2}", ThemeColorType.SelectedItem, ThemeColorIndex.SelectedItem, (int)ThemeColorBrightness.Value);
                    }
                }
            }
        }

        void setForegroundButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].ForegroundThemeColor = string.Format("{0} {1} {2}", ThemeColorType.SelectedItem, ThemeColorIndex.SelectedItem, (int)ThemeColorBrightness.Value);
                    }
                }
            }
        }

        void comboThemes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _excel.CurrentThemeName = comboThemes.SelectedItem as string;
            _excel.ActiveSheet[1, 3].Value = _excel.CurrentThemeName;
        }

        void setFontButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].FontTheme = "Headings";
                    }
                }
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