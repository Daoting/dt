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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
using Windows.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
#endregion

namespace Demo.UI
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

        void OnChartTypeChanged(FvCell arg1, object e)
        {
            _chart.Data = _data.GetData((ChartType)e);
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

            var pixelBuffer = await bmp.GetPixelsAsync();
            var ms = new MemoryStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, ms.AsRandomAccessStream());

            // 换算成物理像素
            float dpi = (float)XamlRoot.RasterizationScale * 96;
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Ignore,
                (uint)bmp.PixelWidth,
                (uint)bmp.PixelHeight,
                dpi,
                dpi,
                pixelBuffer.ToArray());
            await encoder.FlushAsync();
            
            sheet.AddPicture("pic" + sheet.Pictures.Count.ToString(), ms, rc.Left, rc.Top, rc.Width, rc.Height);
        }
    }
}