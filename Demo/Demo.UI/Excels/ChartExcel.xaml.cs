﻿#region 文件描述
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
using Dt.Cells.UndoRedo;
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
using Microsoft.UI.Xaml.Shapes;
using Cell = Dt.Cells.Data.Cell;
#endregion

namespace Demo.UI
{
    public partial class ChartExcel : Win
    {
        SpreadChart _selectedChart;
        int customChartIndex = 0;

        public ChartExcel()
        {
            InitializeComponent();

            using (_excel.Defer())
            {
                InitChart();
                InitChartTitle();
            }
            InitProperty();
        }

        void InitProperty()
        {
            _cbTypes.ItemsSource = GetEnumValues(typeof(SpreadChartType));
            foreach (SpreadTheme st in _excel.Themes)
            {
                _cbTheme.Items.Add(st.Name);
            }

            _cbTheme.SelectedItem = _excel.CurrentThemeName;

            _cbTypes.SelectionChanged += _cbTypes_SelectionChanged;
            _cbTheme.SelectionChanged += Theme_SelectionChanged;

            _cbTypes.SelectedItem = _selectedChart.ChartType.ToString();
        }

        Worksheet Addsheet()
        {
            Worksheet ws = new Worksheet(50, 15);
            _excel.Sheets.Add(ws);
            return ws;
        }
        
        void InitChart()
        {
            _excel.Sheets.Clear();
            object[,] values = {{"","North","South","East","West"},
                                {"s1",50,25,55,30},
                                {"s2",92,24,15,24},
                                {"s3",65,26,70,60},
                                {"s4",24,80,26,20} };
            Worksheet ws = Addsheet();
            ws.Name = "Column";
            ws.SetArray(0, 0, values);
            ws.AddChart("Chart1", SpreadChartType.ColumnClustered, "Column!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("Chart2", SpreadChartType.ColumnStacked, "Column!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("Chart3", SpreadChartType.ColumnStacked100pc, "Column!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);

            ws = Addsheet();
            ws.Name = "Line";
            ws.SetArray(0, 0, values);
            ws.AddChart("chart1", SpreadChartType.Line, "Line!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.LineSmoothed, "Line!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.LineStacked, "Line!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);
            ws.AddChart("chart4", SpreadChartType.LineStacked100pc, "Line!$A$1:$E$5", 17, 0, 7, 0, 27, 0, 12, 0);
            ws.AddChart("chart5", SpreadChartType.LineStacked100pcWithMarkers, "Line!$A$1:$E$5", 28, 0, 1, 0, 38, 0, 6, 0);
            ws.AddChart("chart6", SpreadChartType.LineStackedWithMarkers, "Line!$A$1:$E$5", 28, 0, 7, 0, 38, 0, 12, 0);
            ws.AddChart("chart7", SpreadChartType.LineWithMarkers, "Line!$A$1:$F$5", 39, 0, 1, 0, 49, 0, 6, 0);
            ws.AddChart("chart8", SpreadChartType.LineWithMarkersSmoothed, "Line!$A$1:$E$5", 39, 0, 7, 0, 49, 0, 12, 0);

            ws = Addsheet();
            ws.Name = "Pie";
            ws.SetArray(0, 0, values);
            ws.AddChart("chart1", SpreadChartType.Pie, "Pie!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.PieDoughnut, "Pie!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.PieExploded, "Pie!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);
            ws.AddChart("chart4", SpreadChartType.PieExplodedDoughnut, "Line!$A$1:$E$5", 17, 0, 7, 0, 27, 0, 12, 0);

            ws = Addsheet();
            ws.Name = "Bar";
            ws.SetArray(0, 0, values);
            ws.AddChart("chart1", SpreadChartType.BarClustered, "Bar!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.BarStacked, "Bar!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.BarStacked100pc, "Bar!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);

            ws = Addsheet();
            ws.Name = "Area";
            ws.SetArray(0, 0, values);
            ws.AddChart("chart1", SpreadChartType.Area, "Area!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.AreaStacked, "Area!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.AreaStacked100pc, "Area!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);

            object[,] values1 = { { "", "North", "South", "East", "West", "Northeast" },
                                { "s1", 384, 246, 549, 260, 260 },
                                { "s2", 926, 146, 1501, 240, 650 },
                                { "s3", 650, 260, 700, 600, 428 },
                                { "s4", 240, 80, 260, 1100, 268 } };
            ws = Addsheet();
            ws.Name = "Scatter";
            ws.SetArray(0, 0, values1);
            ws.AddChart("chart1", SpreadChartType.Scatter, "Scatter!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.ScatterLines, "Scatter!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.ScatterLinesSmoothed, "Scatter!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);
            ws.AddChart("chart4", SpreadChartType.ScatterLinesSmoothedWithMarkers, "Scatter!$A$1:$E$5", 17, 0, 7, 0, 27, 0, 12, 0);
            ws.AddChart("chart5", SpreadChartType.ScatterLinesWithMarkers, "Scatter!$A$1:$E$5", 28, 0, 1, 0, 38, 0, 6, 0);

            ws = Addsheet();
            ws.Name = "Bubble";
            ws.SetArray(0, 0, values1);
            ws.AddChart("chart1", SpreadChartType.Bubble, "Bubble!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);

            object[,] values2 = { { "", new DateTime(2013, 8, 1), new DateTime(2013, 8, 2), new DateTime(2013, 8, 3),
                                      new DateTime(2013, 8, 4), new DateTime(2013, 8, 5) },
                                { "Open", 864, 279, 825, 360, 384 },
                                { "High", 926, 612, 865, 562, 650 },
                                { "Low", 380, 146, 501, 310, 260 },
                                { "Close", 650, 560, 786, 486, 428 } };
            ws = Addsheet();
            ws.Name = "Stock";
            ws.SetArray(0, 0, values2);
            ws.Rows[0].Formatter = new GeneralFormatter("MM/DD");
            ws.AddChart("chart1", SpreadChartType.StockHighLowOpenClose, "Stock!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);

            ws = Addsheet();
            ws.Name = "Radar";
            ws.SetArray(0, 0, values);
            ws.AddChart("chart1", SpreadChartType.Radar, "Radar!$A$1:$E$5", 6, 0, 1, 0, 16, 0, 6, 0);
            ws.AddChart("chart2", SpreadChartType.RadarFilled, "Radar!$A$1:$E$5", 6, 0, 7, 0, 16, 0, 12, 0);
            ws.AddChart("chart3", SpreadChartType.RadarWithMarkers, "Radar!$A$1:$E$5", 17, 0, 1, 0, 27, 0, 6, 0);
            
            _excel.ActiveSheetChanged += _excel_ActiveSheetChanged;
            _selectedChart = _excel.ActiveSheet.Charts[0];
        }

        void InitChartTitle()
        {
            int sheetCount = _excel.SheetCount;
            SpreadChart chart = null;

            for (int i = 0; i < sheetCount; i++)
            {
                var sheet = _excel.Sheets[i];
                sheet.ChartSelectionChanged += ChartSelectionChanged;

                int chartCount = sheet.Charts.Count;
                for (int j = 0; j < chartCount; j++)
                {
                    chart = sheet.Charts[j];
                    chart.ChartTitle = new ChartTitle();
                    chart.ChartTitle.Text = chart.ChartType.ToString();
                }
            }
        }

        void _excel_ActiveSheetChanged(object sender, EventArgs e)
        {
            if (_excel.ActiveSheet.Charts == null || _excel.ActiveSheet.Charts.Count == 0)
            {
                return;
            }
            foreach (var item in _excel.ActiveSheet.Charts)
            {
                if (item.IsSelected)
                {
                    _selectedChart = item;
                    ChangeChartTypeSilent();
                    return;
                }
            }
            _selectedChart = _excel.ActiveSheet.Charts[0];
            ChangeChartTypeSilent();
        }

        void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _excel.CurrentThemeName = _cbTheme.SelectedItem as string;
        }

        void _cbTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_selectedChart != null)
            {
                SpreadChartType chartType = (SpreadChartType)Enum.Parse(typeof(SpreadChartType), (string)_cbTypes.SelectedItem, false);
                var changeChartType = new ChangeChartTypeAction(_selectedChart, chartType);
                _excel.UndoManager.Do(changeChartType);
                _selectedChart.ChartTitle.Text = _selectedChart.ChartType.ToString();
                _excel.RefreshCharts(_selectedChart);
            }
        }

        void AddNewChart(object sender, RoutedEventArgs e)
        {
            if (_excel.ActiveSheet.Selections.Count == 0)
            {
                Kit.Msg("请选择单元格区域！");
                return;
            }

            CellRange range = _excel.ActiveSheet.Selections[0];
            if (range.Row < 0 || range.Column < 0)
            {
                Kit.Msg("单元格区域无效！");
                return;
            }

            string rangeFormula = _excel.ActiveSheet.Cells[range.Row, range.Column, range.RowCount + range.Row - 1, range.ColumnCount + range.Column - 1].ToString(_excel.ActiveSheet.Cells[0, 0]);
            rangeFormula = "'" + _excel.ActiveSheet.Name + "'!" + rangeFormula;

            SpreadChartType chartType = (SpreadChartType)Enum.Parse(typeof(SpreadChartType), (string)_cbTypes.SelectedItem, false);
            //SpreadChart chart = _excel.ActiveSheet.AddChart("CustomChart" + customChartIndex.ToString(), chartType, rangeFormula, 10, 10, 400, 290);
            SpreadChart chart = _excel.ActiveSheet.AddChart("CustomChart" + customChartIndex.ToString(), chartType, rangeFormula, 10, 0, 10, 0, 400, 290);
            _excel.ActiveSheet.ChartSelectionChanged += ChartSelectionChanged;

            chart.ChartTitle = new ChartTitle();
            chart.ChartTitle.Text = chartType.ToString();
            chart.IsSelected = true;

            customChartIndex++;
        }

        void ChartSelectionChanged(object sender, ChartSelectionChangedEventArgs e)
        {
            _selectedChart = e.Chart;
            ChangeChartTypeSilent();
        }

        void ChangeChartTypeSilent()
        {
            _cbTypes.SelectionChanged -= _cbTypes_SelectionChanged;
            _cbTypes.SelectedItem = _selectedChart.ChartType.ToString();
            _cbTypes.SelectionChanged += _cbTypes_SelectionChanged;
        }

        object[] GetEnumValues(Type type)
        {
            List<object> list = new List<object>();
            for (int i = 0; ; i++)
            {
                string val = Enum.GetName(type, i);
                if (!string.IsNullOrEmpty(val))
                    list.Add(val);
                else
                    break;
            }
            return list.ToArray();
        }
    }
}