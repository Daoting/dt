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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Dt.Sample
{
    public partial class ChartPicture : Win
    {
        ChartSampleData _data;

        public ChartPicture()
        {
            InitializeComponent();

            _data = new ChartSampleData();
            _chart.Data = _data.GetData(ChartType.Column);
        }

        void OnChartTypeChanged(object sender, object e)
        {
            _chart.Data = _data.GetData((ChartType)e);
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

        ExcelSaveFlags GetSaveFlag()
        {
            var flagText = (_saveFlags.SelectedItem as ComboBoxItem).Content.ToString();
            var result = ExcelSaveFlags.NoFlagsSet;
            Enum.TryParse<ExcelSaveFlags>(flagText, true, out result);
            return result;
        }

        async void OnAddChart(object sender, RoutedEventArgs e)
        {
            Worksheet sheet = _excel.ActiveSheet;
            if (sheet.Selections.Count == 0)
                return;

            CellRange range = sheet.Selections[0];
            Rect rc = sheet.GetRangeLocation(range);

            Chart ct = new Chart();
            ct.Width = rc.Width;
            ct.Height = rc.Height;
            ct.ChartType = _chart.ChartType;
            ct.Palette = _chart.Palette;
            ct.Header = _chart.Header;
            ChartLegend legend = _chart.Children[0] as ChartLegend;
            if (legend.Visibility == Visibility.Visible)
            {
                ChartLegend lg = new ChartLegend();
                lg.Title = legend.Title;
                lg.Position = legend.Position;
                lg.Orientation = legend.Orientation;
                lg.OverlapChart = legend.OverlapChart;
                ct.Children.Add(lg);
            }
            ct.View.AxisX.Title = _chart.View.AxisX.Title;
            ct.View.AxisY.Title = _chart.View.AxisY.Title;
            ct.View.Inverted = _chart.View.Inverted;
            ct.Data = _data.GetData(ct.ChartType);

            RenderTargetBitmap bmp = new RenderTargetBitmap();
            await bmp.RenderAsync(_chart);
            sheet.AddPicture("pic" + sheet.Pictures.Count.ToString(), bmp, rc.Left, rc.Top, rc.Width, rc.Height);
        }
    }
}