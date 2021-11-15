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
    public partial class RangeGroup : Win
    {
        public RangeGroup()
        {
            InitializeComponent();
            using (_excel.Defer())
            {
                InitializeSample();
            }
        }
        void InitializeSample()
        {
            // Range group
            Worksheet sheet = _excel.Sheets[0];

            sheet.ColumnCount = 7;
            sheet.RowCount = 34;
            sheet.Columns[0].Width = sheet.Columns[1].Width = 80;
            sheet.Columns[2].Width = 110;
            sheet.AddSelection(0, 0, 1, 1);
            // set value
            var t = new object[,]
                             {
                               {"= Eastern ==========", "", "", 0, 0, "", 0},
                               {"Eastern", "Atlantic", "Celtics", 57, 19, "-", 0.750},
                               {"Eastern", "Atlantic", "76ers", 38, 35, 17.5, 0.521},
                               {"Eastern", "Atlantic", "Nets", 31, 44, 25.5, 0.413},
                               {"Eastern", "Atlantic", "Raptors", 29, 45, 27, 0.392},
                               {"Eastern", "Atlantic", "Knicks", 29, 46, 27.5, 0.387},
                               {"Eastern", "Central", "Cavaliers", 61, 13, "-", 0.824},
                               {"Eastern", "Central", "Pistons", 36, 39, 25.5, 0.480},
                               {"Eastern", "Central", "Bulls", 36, 40, 26, 0.474},
                               {"Eastern", "Central", "Pacers", 32, 43, 29.5, 0.427},
                               {"Eastern", "Central", "Bucks", 32, 44, 30, 0.421},
                               {"Eastern", "Southeast", "Magic", 55, 19, "-", 0.743},
                               {"Eastern", "Southeast", "Hawks", 43, 32, 12.5, 0.573},
                               {"Eastern", "Southeast", "Heat", 39, 36, 16.5, 0.520},
                               {"Eastern", "Southeast", "Bobcats", 34, 41, 21.5, 0.453},
                               {"Eastern", "Southeast", "Wizards", 17, 59, 39, 0.224},
                               {"= Total ==========", "", "", 0, 0, "", 0},
                               {"= Western ==========", "", "", 0, 0, "", 0},
                               {"Western", "Northwest", "Nuggets", 49, 26, "-", 0.653},
                               {"Western", "Northwest", "Trail Blazers", 47, 27, 1.5, 0.635},
                               {"Western", "Northwest", "Jazz", 46, 28, 2.5, 0.622},
                               {"Western", "Northwest", "Thunder", 21, 53, 27.5, 0.284},
                               {"Western", "Northwest", "Timberwolves", 21, 54, 28, 0.280},
                               {"Western", "Pacific", "Lakers", 59, 16, "-", 0.787},
                               {"Western", "Pacific", "Suns", 41, 34, 18, 0.547},
                               {"Western", "Pacific", "Warriors", 26, 49, 33, 0.347},
                               {"Western", "Pacific", "Clippers", 18, 57, 41, 0.240},
                               {"Western", "Pacific", "Kings", 16, 58, 42.5, 0.216},
                               {"Western", "Southwest", "Spurs", 48, 29, "-", 0.649},
                               {"Western", "Southwest", "Rockets", 48, 27, 0.5, 0.640},
                               {"Western", "Southwest", "Hornets", 47, 27, 1, 0.635},
                               {"Western", "Southwest", "Mavericks", 45, 30, 3.5, 0.600},
                               {"Western", "Southwest", "Grizzlies", 20, 54, 28, 0.270},
                               {"= Total ==========", "", "", 0, 0, "", 0},
                             };

            for (int r = 0; r <= t.GetUpperBound(0); r++)
            {
                for (int c = 0; c <= t.GetUpperBound(1); c++)
                {
                    sheet.SetValue(r, c, t[r, c]);
                }
            }
            sheet.Cells[0, 0].ColumnSpan = 7;
            sheet.Cells[16, 0].ColumnSpan = 3;
            sheet.Cells[17, 0].ColumnSpan = 7;
            sheet.Cells[33, 0].ColumnSpan = 3;
            sheet.ColumnHeader.RowCount = 2;
            sheet.ColumnHeader.AutoTextIndex = 1;
            sheet.ColumnHeader.Cells[0, 0].Value = "2008-09 NBA Regular Season Standings";
            sheet.ColumnHeader.Cells[0, 0].ColumnSpan = 7;
            sheet.ColumnHeader.Cells[0, 0].FontFamily = new FontFamily("Arial");
            sheet.ColumnHeader.Cells[0, 0].FontSize = 14;
            sheet.ColumnHeader.Cells[0, 0].HorizontalAlignment = CellHorizontalAlignment.Center;
            sheet.ColumnHeader.Cells[0, 0].VerticalAlignment = CellVerticalAlignment.Center;
            sheet.ColumnHeader.Cells[0, 0].Foreground = new SolidColorBrush(Colors.Gray);
            sheet.ColumnHeader.Rows[0].Height = 30;

            sheet.Columns[2].Foreground = new SolidColorBrush(Colors.Blue);
            sheet.Cells[19, 2].Foreground = new SolidColorBrush(Colors.Blue);
            sheet.Columns[0].Label = "Conference";
            sheet.Columns[1].Label = "Standing";
            sheet.Columns[2].Label = "Team";
            sheet.Columns[3].Label = "W";
            sheet.Columns[4].Label = "L";
            sheet.Columns[5].Label = "GB";
            sheet.Columns[6].Label = "PCT";

            // set row range group
            sheet.RowRangeGroup.Group(1, 15); // eastern
            sheet.RowRangeGroup.Group(1, 4);
            sheet.RowRangeGroup.Group(6, 4);
            sheet.RowRangeGroup.Group(11, 4);
            sheet.RowRangeGroup.Group(18, 15); // western
            sheet.RowRangeGroup.Group(18, 4);
            sheet.RowRangeGroup.Group(23, 4);
            sheet.RowRangeGroup.Group(28, 4);
            // sheet.RowRangeGroup.Expand(1, false);

            _cbRow.IsChecked = true;
            _cbColumn.IsChecked = true;
        }

        void groupButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            CellRange cr = sheet.Selections[0];
            if (cr.Column == -1 && cr.Row == -1) // sheet selection
            {

            }
            else if (cr.Column == -1) // row selection
            {
                sheet.RowRangeGroup.Group(cr.Row, cr.RowCount);
            }
            else if (cr.Row == -1) // column selection
            {
                sheet.ColumnRangeGroup.Group(cr.Column, cr.ColumnCount);
            }
            else // cell range selection
            {
                Kit.Msg("请选择行或列！");
            }
        }

        void ungroupButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            CellRange cr = sheet.Selections[0];
            if (cr.Column == -1 && cr.Row == -1) // sheet selection
            {

            }
            else if (cr.Column == -1) // row selection
            {
                sheet.RowRangeGroup.Ungroup(cr.Row, cr.RowCount);
            }
            else if (cr.Row == -1) // column selection
            {
                sheet.ColumnRangeGroup.Ungroup(cr.Column, cr.ColumnCount);
            }
            else // cell range selection
            {
                Kit.Msg("请选择分组中的行或列！");
            }
        }

        void showDetailButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.Sheets[0];
            CellRange cr = sheet.Selections[0];
            if (cr.Column == -1 && cr.Row == -1) // sheet selection
            {

            }
            else if (cr.Column == -1) // row selection
            {
                for (int i = 0; i < cr.RowCount; i++)
                {
                    var rgi = sheet.RowRangeGroup.Find(i + cr.Row, 0);
                    if (rgi != null)
                    {
                        sheet.RowRangeGroup.Expand(rgi, true);
                    }
                }
            }
            else if (cr.Row == -1) // column selection
            {
                for (int i = 0; i < cr.ColumnCount; i++)
                {
                    var rgi = sheet.ColumnRangeGroup.Find(i + cr.Column, 0);
                    if (rgi != null)
                    {
                        sheet.ColumnRangeGroup.Expand(rgi, true);
                    }
                }
            }
            else // cell range selection
            {
                Kit.Msg("请选择分组中的行或列！");
            }
        }

        void hideDetailButton_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.Sheets[0];
            CellRange cr = sheet.Selections[0];
            if (cr.Column == -1 && cr.Row == -1) // sheet selection
            {

            }
            else if (cr.Column == -1) // row selection
            {
                for (int i = 0; i < cr.RowCount; i++)
                {
                    var rgi = sheet.RowRangeGroup.Find(i + cr.Row, 0);
                    if (rgi != null)
                    {
                        sheet.RowRangeGroup.Expand(rgi, false);
                    }
                }
            }
            else if (cr.Row == -1) // column selection
            {
                for (int i = 0; i < cr.ColumnCount; i++)
                {
                    var rgi = sheet.ColumnRangeGroup.Find(i + cr.Column, 0);
                    if (rgi != null)
                    {
                        sheet.ColumnRangeGroup.Expand(rgi, false);
                    }
                }
            }
            else // cell range selection
            {
                Kit.Msg("请选择分组中的行或列！");
            }
        }

        void summaryRow_Checked(object sender, RoutedEventArgs e)
        {
            _excel.ActiveSheet.RowRangeGroup.Direction = _cbRow.IsChecked == true ? RangeGroupDirection.Forward : RangeGroupDirection.Backward;
        }

        void summaryColumn_Checked(object sender, RoutedEventArgs e)
        {
            _excel.ActiveSheet.ColumnRangeGroup.Direction = _cbColumn.IsChecked == true ? RangeGroupDirection.Forward : RangeGroupDirection.Backward;
        }

        void asc_Click(object sender, RoutedEventArgs e)
        {
            // sort a to z
            Worksheet sheet = _excel.ActiveSheet;
            CellRange cr = sheet.Selections[0];
            if ((bool)_cbSort.IsChecked)
            {
                sheet.SortRange(cr.Row, cr.Column, cr.RowCount, cr.ColumnCount, false, new SortInfo[] { new SortInfo(sheet.ActiveRowIndex, true) });
            }
            else
            {
                sheet.SortRange(cr.Row, cr.Column, cr.RowCount, cr.ColumnCount, true, new SortInfo[] { new SortInfo(sheet.ActiveColumnIndex, true) });
            }
        }

        void desc_Click(object sender, RoutedEventArgs e)
        {
            // sort z to a
            Worksheet sheet = _excel.ActiveSheet;
            CellRange cr = sheet.Selections[0];
            if ((bool)_cbSort.IsChecked)
            {
                sheet.SortRange(cr.Row, cr.Column, cr.RowCount, cr.ColumnCount, false, new SortInfo[] { new SortInfo(sheet.ActiveRowIndex, false) });
            }
            else
            {
                sheet.SortRange(cr.Row, cr.Column, cr.RowCount, cr.ColumnCount, true, new SortInfo[] { new SortInfo(sheet.ActiveColumnIndex, false) });
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