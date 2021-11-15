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
    public partial class Formatter : Win
    {
        public Formatter()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                Worksheet sheet = _excel.Sheets[0];
                sheet.Name = "Standard Formatter";
                initStandardFormatter(sheet);
                sheet = new Worksheet("Date Formatter");
                _excel.Sheets.Insert(0, sheet);
                initFormatterUnitTest(sheet);
                startFormatterUnitTest(sheet, dateFormatterArray);
                sheet = new Worksheet("Number Formatter");
                _excel.Sheets.Insert(0, sheet);
                initFormatterUnitTest(sheet);
                startFormatterUnitTest(sheet, numberFormatterArray);
            }
        }

        void initStandardFormatter(Worksheet sheet)
        {
            // Formatter
            sheet.Columns[1].Width = sheet.Columns[2].Width = 150;

            double dvalue = 12345.6789;
            int nvalue = 12345;

            // c C
            sheet.Cells[0, 0].Value = "c C";
            sheet.Cells[0, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "c");
            sheet.Cells[0, 1].Value = dvalue;
            sheet.Cells[0, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "C");
            sheet.Cells[0, 2].Value = dvalue;

            // c3 C3
            sheet.Cells[1, 0].Value = "c3 C3";
            sheet.Cells[1, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "c3");
            sheet.Cells[1, 1].Value = dvalue;
            sheet.Cells[1, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "C3");
            sheet.Cells[1, 2].Value = dvalue;

            // d D
            sheet.Cells[2, 0].Value = "d D";
            sheet.Cells[2, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "d");
            sheet.Cells[2, 1].Value = nvalue;
            sheet.Cells[2, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "D");
            sheet.Cells[2, 2].Value = nvalue;

            // d8 D8
            sheet.Cells[3, 0].Value = "d8 D8";
            sheet.Cells[3, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "d8");
            sheet.Cells[3, 1].Value = nvalue;
            sheet.Cells[3, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "D8");
            sheet.Cells[3, 2].Value = nvalue;

            // e E
            sheet.Cells[4, 0].Value = "e E";
            sheet.Cells[4, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "e");
            sheet.Cells[4, 1].Value = dvalue;
            sheet.Cells[4, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "E");
            sheet.Cells[4, 2].Value = dvalue;

            // e4 E4
            sheet.Cells[4, 0].Value = "e4 E4";
            sheet.Cells[4, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "e4");
            sheet.Cells[4, 1].Value = dvalue;
            sheet.Cells[4, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "E4");
            sheet.Cells[4, 2].Value = dvalue;

            // f F
            sheet.Cells[5, 0].Value = "f F";
            sheet.Cells[5, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "f");
            sheet.Cells[5, 1].Value = dvalue;
            sheet.Cells[5, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "F");
            sheet.Cells[5, 2].Value = dvalue;

            // f3 F3
            sheet.Cells[6, 0].Value = "f3 F3";
            sheet.Cells[6, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "f3");
            sheet.Cells[6, 1].Value = dvalue;
            sheet.Cells[6, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "F3");
            sheet.Cells[6, 2].Value = dvalue;

            // g G
            sheet.Cells[7, 0].Value = "g G";
            sheet.Cells[7, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "g");
            sheet.Cells[7, 1].Value = dvalue;
            sheet.Cells[7, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "G");
            sheet.Cells[7, 2].Value = dvalue;

            // g2 G2 Check Excel
            sheet.Cells[8, 0].Value = "g2 G2";
            sheet.Cells[8, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "g2");
            sheet.Cells[8, 1].Value = dvalue;
            sheet.Cells[8, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "G2");
            sheet.Cells[8, 2].Value = dvalue;

            // n N
            sheet.Cells[9, 0].Value = "n N";
            sheet.Cells[9, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "n");
            sheet.Cells[9, 1].Value = dvalue;
            sheet.Cells[9, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "N");
            sheet.Cells[9, 2].Value = dvalue;

            // n1 N1
            sheet.Cells[10, 0].Value = "n1 N1";
            sheet.Cells[10, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "n1");
            sheet.Cells[10, 1].Value = dvalue;
            sheet.Cells[10, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "N1");
            sheet.Cells[10, 2].Value = dvalue;

            // p P
            sheet.Cells[11, 0].Value = "p P";
            sheet.Cells[11, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "p");
            sheet.Cells[11, 1].Value = dvalue;
            sheet.Cells[11, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "P");
            sheet.Cells[11, 2].Value = dvalue;

            // p1 P1
            sheet.Cells[12, 0].Value = "p1 P1";
            sheet.Cells[12, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "p1");
            sheet.Cells[12, 1].Value = dvalue;
            sheet.Cells[12, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "P1");
            sheet.Cells[12, 2].Value = dvalue;

            // r R
            sheet.Cells[13, 0].Value = "r R";
            sheet.Cells[13, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "r");
            sheet.Cells[13, 1].Value = Math.PI;
            sheet.Cells[13, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "R");
            sheet.Cells[13, 2].Value = Math.PI;

            // x X, Check Excel
            sheet.Cells[14, 0].Value = "x X";
            sheet.Cells[14, 1].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "x");
            sheet.Cells[14, 1].Value = nvalue;
            sheet.Cells[14, 2].Formatter = new GeneralFormatter(FormatMode.StandardNumericMode, "X");
            sheet.Cells[14, 2].Value = nvalue;
        }

        static string numValue = "1234.0123456789012345678901234567899";
        static string dateValue = "2011/9/2";
        static string timeValue = "10:08:25";
        static string perValue = "1234.0123456789012345678901234567899%";
        static string sciValue = "1.012345678901234567890123456789e2";
        static string cusValue = "1001.1";
        static string cusDateValue = "2011/08/29";
        string[] numberFormatterArray = new string[]{
            //Number Formatter
            numValue, "0", "1234",
            numValue, "0.0", "1234.0",
            numValue, "0.00", "1234.01",
            numValue, "0.0000000000", "1234.0123456789",
            numValue, "0.00000000000000000000", "1234.01234567890000000000",
            numValue, "0.000000000000000000000000000000", "1234.012345678900000000000000000000",
            numValue, "#,##0", "1,234",
            numValue, "#,##0.00", "1,234.01",
            numValue, "#,##0.0000000000", "1,234.0123456789",
            numValue, "#,##0.00000000000000000000", "1,234.01234567890000000000",
            numValue, "#,##0.000000000000000000000000000000", "1,234.012345678900000000000000000000",
            numValue, "$#,##0", "$1,234",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "$#,##0.0000000000", "$1,234.0123456789",
            numValue, "$#,##0.00000000000000000000", "$1,234.01234567890000000000",
            numValue, "$#,##0.000000000000000000000000000000", "$1,234.012345678900000000000000000000",
            numValue, "#,##0.00", "1,234.01",
            numValue, "￥#,##0.00", "￥1,234.01",
            numValue, "Lek#,##0.00", "Lek1,234.01",
            numValue, "դր#,##0.00000", "դր1,234.01235",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "€#,##0.00", "€1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "ман#,##0.00", "ман1,234.01",
            numValue, "€#,##0.00", "€1,234.01",
            numValue, "лв#,##0.000000", "лв1,234.012346",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "$b#,##0.00", "$b1,234.01",
            numValue, "R$#,##0.00", "R$1,234.01",
            numValue, "#,##0.00р.", "1,234.01р.",
            numValue, "BZ$#,##0.00", "BZ$1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "CB#,##0.00", "CB1,234.01",
            numValue, "fr#,##0.00", "fr1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "¥#,##0.00", "¥1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "₡#,##0.00", "₡1,234.01",
            numValue, "Kč#,##0.00", "Kč1,234.01",
            numValue, "€#,##0.00", "€1,234.01",
            numValue, "kr#,##0.00", "kr1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "kr#,##0.00", "kr1,234.01",
            numValue, "€#,##0.00", "€1,234.01",
            numValue, "£#,##0.00", "£1,234.01",
            numValue, "Lari#,##0.00", "Lari1,234.01",
            numValue, "Q#,##0.00", "Q1,234.01",
            numValue, "L#,##0.00", "L1,234.01",
            numValue, "kn#,##0.00", "kn1,234.01",
            numValue, "Ft#,##0.00", "Ft1,234.01",
            numValue, "Rp#,##0.00", "Rp1,234.01",
            numValue, "₪#,##0.00", "₪1,234.01",
            numValue, "रु#,##0.00", "रु1,234.01",
            numValue, "J$#,##0.00", "J$1,234.01",
            numValue, "¥#,##0.00", "¥1,234.01",
            numValue, "сом#,##0.00", "сом1,234.01",
            numValue, "₩#,##0.00", "₩1,234.01",
            numValue, "Т#,##0.00", "Т1,234.01",
            numValue, "Lt#,##0.00", "Lt1,234.01",
            numValue, "ден#,##0.00", "ден1,234.01",
            numValue, "₮#,##0.00", "₮1,234.01",
            numValue, "$#,##0.00", "$1,234.01",
            numValue, "C$#,##0.00", "C$1,234.01",
            numValue, "kr#,##0.00", "kr1,234.01",
            numValue, "zł#,##0.00", "zł1,234.01",
            numValue, "lei#,##0.00", "lei1,234.01",
            numValue, "р#,##0.00", "р1,234.01",
            numValue, "TA#,##0.00", "TA1,234.01",
            numValue, "฿#,##0.00", "฿1,234.01",
            numValue, "TL#,##0.00", "TL1,234.01",
            numValue, "TT$#,##0.00", "TT$1,234.01",
            numValue, "NT$#,##0.00", "NT$1,234.01",
            numValue, "₴#,##0.00", "₴1,234.01",
            numValue, "$U#,##0.00", "$U1,234.01",
            numValue, "сўм#,##0.00", "сўм1,234.01",
            numValue, "₫#,##0.00", "₫1,234.01",
            numValue, "R#,##0.00", "R1,234.01",
            numValue, "Z$#,##0.00", "Z$1,234.01",
            //Percentage Formatter
            perValue, "0%", "1234%",
            perValue, "0.00%", "1234.01%",
            perValue, "0.000000000000000%", "1234.012345678900000%",
            perValue, "0.000000000000000000000000000000%", "1234.012345678900000000000000000000%",
            //Scientific Formatter
            sciValue, "0E+00", "1E+02",
            sciValue, "0.00E+00", "1.01E+02",
            sciValue, "0.000000000000000E+00", "1.012345678901230E+02",
            sciValue, "0.000000000000000000000000000000E+00", "1.012345678901230000000000000000E+02",
            //Custom Number Formatter
            cusValue, "General", "1001.1",
            cusValue, "0", "1001",
            cusValue, "0.00", "1001.10",
            cusValue, "#,##0", "1,001",
            cusValue, "#,##0.00", "1,001.10",
            cusValue, "0%", "100110%",
            cusValue, "0.00%", "100110.00%",
            cusValue, "0.00E+00", "1.00E+03",
            cusValue, "##0.0E+0", "1.0E+3",
            cusValue, "# ?/?", "1001 1/9",
            cusValue, "# ??/??", "1001  1/10"
        };

        string[] dateFormatterArray = new string[]{
            //Date Formatter
            dateValue, "M/d/yyyy", "9/2/2011",
            dateValue, "[$-F800]dddd, mmmm dd, yyyy", "Friday, September 02, 2011",
            dateValue, "M/d", "9/2",
            dateValue, "M/d/yy", "9/2/11",
            dateValue, "MM/dd/yy", "09/02/11",
            dateValue, "d-MMM", "2-Sep",
            dateValue, "d-MMM-yy", "2-Sep-11",
            dateValue, "dd-MMM-yy", "02-Sep-11",
            dateValue, "MMM-yy", "Sep-11",
            dateValue, "MMMM-yy", "September-11",
            dateValue, "MMMM d, yyyy", "September 2, 2011",
            dateValue, "M/d/yy HH:mm tt", "9/2/11 12:00 AM",
            dateValue, "M/d/yy H:mm", "9/2/11 0:00",
            dateValue, "[$-409]mmmmm;@", "S",
            dateValue, "[$-409]mmmmm-yy;@", "S-11",
            dateValue, "d-MMM-yyyy", "2-Sep-2011",
            //Time Formatter
            timeValue, "HH:mm:ss tt", "10:08:25 AM",
            timeValue, "HH:mm", "10:08",
            timeValue, "HH:mm tt", "10:08 AM",
            timeValue, "HH:mm:ss", "10:08:25",
            timeValue, "mm:ss.0", "08:25.0",
            timeValue, "m/d/yy HH:mm tt", "12/30/99 10:08 AM",
            timeValue, "d/m/yy HH:mm", "30/12/99 10:08",
            //Custom DateTime Formatter
            cusDateValue, "General", "40784",
            cusDateValue, "m/d/yyyy", "8/29/2011",
            cusDateValue, "d-mmm-yy", "29-Aug-11",
            cusDateValue, "d-mmm", "29-Aug",
            cusDateValue, "mmm-yy", "Aug-11",
            cusDateValue, "h:mm AM/PM", "0:00 AM",
            cusDateValue, "h:mm:ss AM/PM", "0:00:00 AM",
            cusDateValue, "h:mm", "0:00",
            cusDateValue, "h:mm:ss", "0:00:00",
            cusDateValue, "m/d/yyyy h:mm", "8/29/2011 0:00",
            cusDateValue, "mm:ss", "00:00",
            cusDateValue, "mm:ss.0", "00:00.0",
            cusDateValue, "@", "8/29/2011 12:00:00 AM",
            cusDateValue, "[h]:mm:ss", "978816:00:00"
        };
        string[] textFormatterArray = new string[]{
            "The quick brown fox jumps over the lazy dog.", "@", "The quick brown fox jumps over the lazy dog."
        };

        void initFormatterUnitTest(Worksheet sheet)
        {
            sheet.ColumnCount = 3;
            sheet.ColumnHeader.Cells[0, 0].Value = "Value";
            sheet.ColumnHeader.Cells[0, 1].Value = "Formatter";
            sheet.ColumnHeader.Cells[0, 2].Value = "Display";

            sheet.Columns[0].Width = 200;
            sheet.Columns[1].Width = 300;
            sheet.Columns[2].Width = 300;
        }

        void startFormatterUnitTest(Worksheet sheet, string[] cellFormatterArray)
        {
            for (var i = 0; i < cellFormatterArray.Length; i += 3)
            {
                sheet.SetValue(i / 3, 0, cellFormatterArray[i]);
                sheet.SetValue(i / 3, 1, cellFormatterArray[i + 1]);
                sheet.SetText(i / 3, 2, cellFormatterArray[i]);
                sheet[i / 3, 2].Formatter = new GeneralFormatter(cellFormatterArray[i + 1]);
            }
        }

        void setFormatButton_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].Formatter = new GeneralFormatter(_tbFormat.Text);
                    }
                }
            }
        }

        void clearFormatButton_Click(object sender, RoutedEventArgs e)
        {
            var sheet = _excel.ActiveSheet;
            foreach (CellRange cellRange in sheet.Selections)
            {
                CellRange range = GetActualCellRange(cellRange, sheet.RowCount, sheet.ColumnCount);
                for (int i = 0; i < range.RowCount; i++)
                {
                    for (int j = 0; j < range.ColumnCount; j++)
                    {
                        sheet.Cells[range.Row + i, range.Column + j].ResetFormatter();
                    }
                }
            }
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