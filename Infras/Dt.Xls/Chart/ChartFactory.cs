#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// 
    /// </summary>
    internal class ChartFactory
    {
        /// <summary>
        /// Gets the chart.
        /// </summary>
        /// <param name="chartType">Type of the chart.</param>
        /// <returns></returns>
        internal static IExcelChart GetChart(ExcelChartType chartType)
        {
            ExcelChart chart = new ExcelChart();
            switch (chartType)
            {
                case ExcelChartType.ColumnClustered:
                case ExcelChartType.ColumnStacked:
                case ExcelChartType.ColumnStacked100Percent:
                case ExcelChartType.BarClustered:
                case ExcelChartType.BarStacked:
                case ExcelChartType.BarStacked100Percent:
                    chart.BarChart = new ExcelBarChart(chartType);
                    return chart;

                case ExcelChartType.ColumnClustered3D:
                case ExcelChartType.ColumnStacked3D:
                case ExcelChartType.ColumnStacked100Percent3D:
                case ExcelChartType.Column3D:
                case ExcelChartType.BarClustered3D:
                case ExcelChartType.BarStacked3D:
                case ExcelChartType.BarStacked100Percent3D:
                    chart.Bar3DChart = new ExcelBar3DChart(chartType);
                    return chart;

                case ExcelChartType.Line:
                case ExcelChartType.LineWithMarkers:
                case ExcelChartType.LineStacked:
                case ExcelChartType.LineStackedWithMarkers:
                case ExcelChartType.LineStacked100Percent:
                case ExcelChartType.LineStacked100PercentWithMarkers:
                    chart.LineChart = new ExcelLineChart(chartType);
                    return chart;

                case ExcelChartType.Line3D:
                    chart.Line3DChart = new ExcelLine3DChart();
                    return chart;

                case ExcelChartType.Pie:
                case ExcelChartType.ExplodedPie:
                    chart.PieChart = new ExcelPieChart(chartType);
                    return chart;

                case ExcelChartType.Pie3D:
                case ExcelChartType.ExplodedPie3D:
                    chart.Pie3DChart = new ExcelPie3DChart(chartType);
                    return chart;

                case ExcelChartType.PieOfPie:
                case ExcelChartType.BarOfPie:
                    chart.OfPieChart = new ExcelOfPieChart(chartType);
                    return chart;

                case ExcelChartType.Scatter:
                case ExcelChartType.ScatterLines:
                case ExcelChartType.ScatterLinesWithMarkers:
                case ExcelChartType.ScatterLinesSmooth:
                case ExcelChartType.ScatterLinesSmoothWithMarkers:
                    chart.ScatterChart = new ExcelScatterChart(chartType);
                    return chart;

                case ExcelChartType.Area:
                case ExcelChartType.AreaStacked:
                case ExcelChartType.AreaStacked100Percent:
                    chart.AreaChart = new ExcelAreaChart(chartType);
                    return chart;

                case ExcelChartType.Area3D:
                case ExcelChartType.AreaStacked3D:
                case ExcelChartType.AreaStacked100Percent3D:
                    chart.Area3DChart = new ExcelArea3DChart(chartType);
                    return chart;

                case ExcelChartType.Doughunt:
                case ExcelChartType.DoughuntExploded:
                    chart.DoughnutChart = new ExcelDoughnutChart(chartType);
                    return chart;

                case ExcelChartType.Radar:
                case ExcelChartType.RadarWithMarkers:
                case ExcelChartType.FilledRadar:
                    chart.RadarChart = new ExcelRadarChart(chartType);
                    return chart;

                case ExcelChartType.Surface:
                case ExcelChartType.SurfaceWireFrame:
                    chart.Surface3DChart = new ExcelSurface3DChart(chartType);
                    return chart;

                case ExcelChartType.SurfaceViewedAbove:
                case ExcelChartType.SurfaceViewedAboveWireFrame:
                    chart.SurfaceChart = new ExcelSurfaceChart(chartType);
                    return chart;

                case ExcelChartType.Bubble:
                case ExcelChartType.Bubble3D:
                    chart.BubbleChart = new ExcelBubbleChart(chartType);
                    return chart;

                case ExcelChartType.StockHighLowClose:
                case ExcelChartType.StockOpenHighLowClose:
                    chart.StockChart = new ExcelStockChart(chartType);
                    return chart;

                case ExcelChartType.CylinderColumnClustered:
                case ExcelChartType.CylinderColumnStacked:
                case ExcelChartType.CylinderColumnStacked100Percent:
                case ExcelChartType.CylinderBarClustered:
                case ExcelChartType.CylinderBarStacked:
                case ExcelChartType.CylinderBarStacked100Percent:
                case ExcelChartType.CylinderColumn3D:
                case ExcelChartType.ConeColumnClustered:
                case ExcelChartType.ConeColumnStacked:
                case ExcelChartType.ConeColumnStacked100Percent:
                case ExcelChartType.ConeBarClustered:
                case ExcelChartType.ConeBarStacked:
                case ExcelChartType.ConeBarStacked100Percent:
                case ExcelChartType.ConeColumn3D:
                case ExcelChartType.PyramidColumnClustered:
                case ExcelChartType.PyramidColumnStacked:
                case ExcelChartType.PyramidColumnStacked100Percent:
                case ExcelChartType.PyramidBarClustered:
                case ExcelChartType.PyramidBarStacked:
                case ExcelChartType.PyramidBarStacked100Percent:
                case ExcelChartType.PyramidColumn3D:
                    chart.Bar3DChart = new ExcelBar3DChart(chartType);
                    return chart;
            }
            return chart;
        }
    }
}

