using Dt.Cells.Data;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Dt.Shell
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : UserControl
    {
        public MainWindow()
        {
            this.InitializeComponent();
            _grid.Padding = new Thickness(0, (int)UIKit.UIApplication.SharedApplication.StatusBarFrame.Height, 0, 0);
        }

        void OnTest1(object sender, RoutedEventArgs e)
        {
            using (_excel.Defer())
            {
                InitChart();
                InitChartTitle();
            }
        }
        void InitChart()
        {
            _excel.SheetCount = 9;

            object[,] values = {{"","North","South","East","West"},
                                {"s1",50,25,55,30},
                                {"s2",92,24,15,24},
                                {"s3",65,26,70,60},
                                {"s4",24,80,26,20} };
            _excel.Sheets[0].SetArray(0, 0, values);

            _excel.Sheets[0].Name = "Column";
            _excel.Sheets[0].AddChart("Chart1", SpreadChartType.ColumnClustered, "Column!$A$1:$E$5", 30, 120, 400, 290);
            _excel.Sheets[0].AddChart("Chart2", SpreadChartType.ColumnStacked, "Column!$A$1:$E$5", 480, 120, 400, 290);
            _excel.Sheets[0].AddChart("Chart3", SpreadChartType.ColumnStacked100pc, "Column!$A$1:$E$5", 30, 440, 400, 290);


            //_excel.Sheets[1].Name = "Line";
            //_excel.Sheets[1].SetArray(0, 0, values);
            //_excel.Sheets[1].AddChart("chart1", SpreadChartType.Line, "Line!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[1].AddChart("chart2", SpreadChartType.LineSmoothed, "Line!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[1].AddChart("chart3", SpreadChartType.LineStacked, "Line!$A$1:$E$5", 30, 440, 400, 290);
            //_excel.Sheets[1].AddChart("chart4", SpreadChartType.LineStacked100pc, "Line!$A$1:$E$5", 480, 440, 400, 290);
            //_excel.Sheets[1].AddChart("chart5", SpreadChartType.LineStacked100pcWithMarkers, "Line!$A$1:$E$5", 30, 760, 400, 290);
            //_excel.Sheets[1].AddChart("chart6", SpreadChartType.LineStackedWithMarkers, "Line!$A$1:$E$5", 480, 760, 400, 290);
            //_excel.Sheets[1].AddChart("chart7", SpreadChartType.LineWithMarkers, "Line!$A$1:$F$5", 30, 1080, 400, 290);
            //_excel.Sheets[1].AddChart("chart8", SpreadChartType.LineWithMarkersSmoothed, "Line!$A$1:$E$5", 480, 1080, 400, 290);


            //_excel.Sheets[2].Name = "Pie";
            //_excel.Sheets[2].SetArray(0, 0, values);
            //_excel.Sheets[2].AddChart("chart1", SpreadChartType.Pie, "Pie!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[2].AddChart("chart2", SpreadChartType.PieDoughnut, "Pie!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[2].AddChart("chart3", SpreadChartType.PieExploded, "Pie!$A$1:$E$5", 30, 440, 400, 290);
            //_excel.Sheets[2].AddChart("chart4", SpreadChartType.PieExplodedDoughnut, "Line!$A$1:$E$5", 480, 760, 400, 290);


            //_excel.Sheets[3].Name = "Bar";
            //_excel.Sheets[3].SetArray(0, 0, values);
            //_excel.Sheets[3].AddChart("chart1", SpreadChartType.BarClustered, "Bar!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[3].AddChart("chart2", SpreadChartType.BarStacked, "Bar!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[3].AddChart("chart3", SpreadChartType.BarStacked100pc, "Bar!$A$1:$E$5", 30, 440, 400, 290);


            //_excel.Sheets[4].Name = "Area";
            //_excel.Sheets[4].SetArray(0, 0, values);
            //_excel.Sheets[4].AddChart("chart1", SpreadChartType.Area, "Area!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[4].AddChart("chart2", SpreadChartType.AreaStacked, "Area!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[4].AddChart("chart3", SpreadChartType.AreaStacked100pc, "Area!$A$1:$E$5", 30, 440, 400, 290);


            //object[,] values1 = { { "", "North", "South", "East", "West", "Northeast" },
            //                    { "s1", 384, 246, 549, 260, 260 },
            //                    { "s2", 926, 146, 1501, 240, 650 },
            //                    { "s3", 650, 260, 700, 600, 428 },
            //                    { "s4", 240, 80, 260, 1100, 268 } };

            //_excel.Sheets[5].Name = "Scatter";
            //_excel.Sheets[5].SetArray(0, 0, values1);
            //_excel.Sheets[5].AddChart("chart1", SpreadChartType.Scatter, "Scatter!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[5].AddChart("chart2", SpreadChartType.ScatterLines, "Scatter!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[5].AddChart("chart3", SpreadChartType.ScatterLinesSmoothed, "Scatter!$A$1:$E$5", 30, 440, 400, 290);
            //_excel.Sheets[5].AddChart("chart4", SpreadChartType.ScatterLinesSmoothedWithMarkers, "Scatter!$A$1:$E$5", 480, 440, 400, 290);
            //_excel.Sheets[5].AddChart("chart5", SpreadChartType.ScatterLinesWithMarkers, "Scatter!$A$1:$E$5", 30, 760, 400, 290);


            //_excel.Sheets[6].Name = "Bubble";
            //_excel.Sheets[6].SetArray(0, 0, values1);
            //_excel.Sheets[6].AddChart("chart1", SpreadChartType.Bubble, "Bubble!$A$1:$E$5", 30, 120, 380, 260);


            //object[,] values2 = { { "", new DateTime(2013, 8, 1), new DateTime(2013, 8, 2), new DateTime(2013, 8, 3),
            //                          new DateTime(2013, 8, 4), new DateTime(2013, 8, 5) },
            //                    { "Open", 864, 279, 825, 360, 384 },
            //                    { "High", 926, 612, 865, 562, 650 },
            //                    { "Low", 380, 146, 501, 310, 260 },
            //                    { "Close", 650, 560, 786, 486, 428 } };

            //_excel.Sheets[7].Name = "Stock";
            //_excel.Sheets[7].SetArray(0, 0, values2);
            //_excel.Sheets[7].Rows[0].Formatter = new GeneralFormatter("MM/DD");
            //_excel.Sheets[7].AddChart("chart1", SpreadChartType.StockHighLowOpenClose, "Stock!$A$1:$E$5", 30, 120, 400, 290);


            //_excel.Sheets[8].Name = "Radar";
            //_excel.Sheets[8].SetArray(0, 0, values);
            //_excel.Sheets[8].AddChart("chart1", SpreadChartType.Radar, "Radar!$A$1:$E$5", 30, 120, 400, 290);
            //_excel.Sheets[8].AddChart("chart2", SpreadChartType.RadarFilled, "Radar!$A$1:$E$5", 480, 120, 400, 290);
            //_excel.Sheets[8].AddChart("chart3", SpreadChartType.RadarWithMarkers, "Radar!$A$1:$E$5", 30, 440, 400, 290);

        }

        void InitChartTitle()
        {
            int sheetCount = _excel.SheetCount;
            SpreadChart chart = null;

            for (int i = 0; i < sheetCount; i++)
            {
                var sheet = _excel.Sheets[i];

                int chartCount = sheet.Charts.Count;
                for (int j = 0; j < chartCount; j++)
                {
                    chart = sheet.Charts[j];
                    chart.ChartTitle = new ChartTitle();
                    chart.ChartTitle.Text = chart.ChartType.ToString();
                }
            }
        }
    }
}
