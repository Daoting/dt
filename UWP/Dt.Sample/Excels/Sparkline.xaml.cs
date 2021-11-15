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
    public partial class Sparkline : Win
    {
        public Sparkline()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitializeSample();
            }
            AddEnumToComboBox(typeof(SparklineType), _cbSparklineType);
            AddEnumToComboBox(typeof(DataOrientation), _cbOrientation);
        }

        void InitializeSample()
        {
            Worksheet sheet = _excel.ActiveSheet;
            _excel.CanCellOverflow = true;

            sheet.SetValue(0, 0, "Data Range is A2-A9");
            sheet.SetValue(1, 0, 1);
            sheet.SetValue(2, 0, -2);
            sheet.SetValue(3, 0, -1);
            sheet.SetValue(4, 0, 6);
            sheet.SetValue(5, 0, 4);
            sheet.SetValue(6, 0, -4);
            sheet.SetValue(7, 0, 3);
            sheet.SetValue(8, 0, 8);


            sheet.SetValue(0, 2, "Date axis range is C2-C9");
            sheet.SetValue(1, 2, new DateTime(2011, 1, 5));
            sheet.SetValue(2, 2, new DateTime(2011, 1, 1));
            sheet.SetValue(3, 2, new DateTime(2011, 2, 11));
            sheet.SetValue(4, 2, new DateTime(2011, 3, 1));
            sheet.SetValue(5, 2, new DateTime(2011, 2, 1));
            sheet.SetValue(6, 2, new DateTime(2011, 2, 3));
            sheet.SetValue(7, 2, new DateTime(2011, 3, 6));
            sheet.SetValue(8, 2, new DateTime(2011, 2, 19));

            CellRange data = new CellRange(1, 0, 8, 1);
            CellRange dateAxis = new CellRange(1, 2, 8, 1);


            sheet.Cells["A12"].Text = "Sparkline without dateAxis:";

            sheet.Cells["A13"].Text = "(1) Line";
            sheet.Cells["d13"].Text = "(2)Column";
            sheet.Cells["g13"].Text = "(3)Winloss";

            //line
            sheet.Cells["A14"].ColumnSpan = 3;
            sheet.Cells["A14"].RowSpan = 4;
            sheet.SetSparkline(
                13,
                0,
                data,
                DataOrientation.Vertical
                , SparklineType.Line
                , new SparklineSetting()
                {
                    ShowMarkers = true,
                    LineWeight = 3,
                    DisplayXAxis = true
                ,
                    ShowFirst = true
                ,
                    ShowLast = true
                ,
                    ShowLow = true
                ,
                    ShowHigh = true
                ,
                    ShowNegative = true
                }
                );

            //column
            sheet.Cells["d14"].ColumnSpan = 3;
            sheet.Cells["d14"].RowSpan = 4;
            sheet.SetSparkline(13, 3, data
                , DataOrientation.Vertical
                , SparklineType.Column
                , new SparklineSetting()
                {
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //winloss
            sheet.Cells["g14"].ColumnSpan = 3;
            sheet.Cells["g14"].RowSpan = 4;
            sheet.SetSparkline(13, 6, data
                , DataOrientation.Vertical
                , SparklineType.Winloss
                , new SparklineSetting()
                {
                    DisplayXAxis = true,
                    ShowNegative = true
                    //,ShowFirst= true
                    //,ShowLast = true
                    //,ShowLow= true
                    //,ShowHigh = true
                }
                );



            //////////////////////////////////////////////
            sheet.Cells["A18"].Text = "Sparkline with dateAxis:";

            sheet.Cells["A19"].Text = "(1) Line";
            sheet.Cells["d19"].Text = "(2)Column";
            sheet.Cells["g19"].Text = "(3)Winloss";

            //line
            sheet.Cells["A20"].ColumnSpan = 3;
            sheet.Cells["A20"].RowSpan = 4;
            sheet.SetSparkline(19, 0, data
                , DataOrientation.Vertical
                , SparklineType.Line
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    ShowMarkers = true,
                    LineWeight = 3,
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //column
            sheet.Cells["d20"].ColumnSpan = 3;
            sheet.Cells["d20"].RowSpan = 4;
            sheet.SetSparkline(19, 3, data
                , DataOrientation.Vertical
                , SparklineType.Column
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    DisplayXAxis = true
                    ,
                    ShowFirst = true
                    ,
                    ShowLast = true
                    ,
                    ShowLow = true
                    ,
                    ShowHigh = true
                    ,
                    ShowNegative = true
                }
                );

            //winloss
            sheet.Cells["g20"].ColumnSpan = 3;
            sheet.Cells["g20"].RowSpan = 4;
            sheet.SetSparkline(19, 6, data
                , DataOrientation.Vertical
                , SparklineType.Winloss
                , dateAxis
                , DataOrientation.Vertical
                , new SparklineSetting()
                {
                    ShowNegative = true,
                    DisplayXAxis = true
                    //,ShowFirst = true
                    //,ShowLast = true
                    //,ShowLow = true
                    //,ShowHigh = true
                }
                );

            _excel.ActiveSheet.AddSelection(0, 0, 1, 1);
        }

        void btnAddSparkline_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            CellRange range = GetActualCellRange(sheet.Selections[0], sheet.RowCount, sheet.ColumnCount);
            string[] rc = _tbPosition.Text.Split(',');
            int r = int.Parse(rc[0]);
            int c = int.Parse(rc[1]);
            sheet.SetSparkline(
                r,
                c,
                range,
                (DataOrientation)Enum.Parse(typeof(DataOrientation), _cbOrientation.SelectedItem.ToString(), true),
                (SparklineType)Enum.Parse(typeof(SparklineType), _cbSparklineType.SelectedItem.ToString(), true),
                null);
        }

        void btnClearSparkline_Click(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            CellRange range = GetActualCellRange(sheet.Selections[0], sheet.RowCount, sheet.ColumnCount);
            for (int r = 0; r < range.RowCount; r++)
            {
                for (int c = 0; c < range.ColumnCount; c++)
                {
                    sheet.RemoveSparkline(r + range.Row, c + range.Column);
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

        void AddEnumToComboBox(Type enumType, ComboBox comboBox)
        {
            var values = from f in enumType.GetTypeInfo().DeclaredFields
                         where f.IsLiteral
                         select f.GetValue(null);
            foreach (var property in values)
            {
                comboBox.Items.Add(property.ToString());
            }
            comboBox.SelectedIndex = 0;
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