#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls;
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents a excel chart
    /// </summary>
    public interface IExcelChart
    {
        /// <summary>
        /// Represents alternate enhanced functionality content the chart will use if the application understand it.
        /// </summary>
        /// <remarks>
        /// There may be multiple choices in the lest and the first understandable one will be used.
        /// </remarks>
        List<int> AlternateContentChoiceStyleList { get; }

        /// <summary>
        /// Represents fall back choice if all alternate choice are not recognized by cosuming application.
        /// </summary>
        /// <remarks>
        /// There may be multiple choices in the lest and the first understandable one will be used.
        /// </remarks>
        List<int> AlternateFallbackStyleList { get; }

        /// <summary>
        /// Specifies the chart location.
        /// </summary>
        IAnchor Anchor { get; set; }

        /// <summary>
        /// Specifies there is a area 3D chart.
        /// </summary>
        IExcelArea3DChart Area3DChart { get; set; }

        /// <summary>
        /// Specifies there is a area chart
        /// </summary>
        IExcelAreaChart AreaChart { get; set; }

        /// <summary>
        /// Specifies the back wall of the chart
        /// </summary>
        IExcelWall BackWall { get; set; }

        /// <summary>
        /// Specifies there is a 3D Bar or column series on the chart.
        /// </summary>
        IExcelBar3DChart Bar3DChart { get; set; }

        /// <summary>
        /// Specifies there is a Bar or column series on the chart.
        /// </summary>
        IExcelBarChart BarChart { get; set; }

        /// <summary>
        /// Specifies there is a bubble chart.
        /// </summary>
        IExcelBubbleChart BubbleChart { get; set; }

        /// <summary>
        /// Represents the format settings for the chart area.
        /// </summary>
        IExcelChartFormat ChartFormat { get; set; }

        /// <summary>
        /// Specifies a title.
        /// </summary>
        IExcelChartTitle ChartTitle { get; set; }

        /// <summary>
        /// Represents chart table settings 
        /// </summary>
        IExcelChartDataTable DataTable { get; set; }

        /// <summary>
        /// Specifies the default formatting for all chart elements.
        /// </summary>
        /// <remarks>
        /// The valid value should be between 1 to 48 and 0 means not set.
        /// </remarks>
        int DefaultStyleIndex { get; set; }

        /// <summary>
        /// Specifies how blank cells shall be plotted on a chart.
        /// </summary>
        DisplayBlankAs DisplayBlanksAs { get; set; }

        /// <summary>
        /// Specifies there is a doughnut chart.
        /// </summary>
        IExcelDoughnutChart DoughnutChart { get; set; }

        /// <summary>
        /// Specifies the floor of the chart.
        /// </summary>
        IExcelWall FloorWall { get; set; }

        /// <summary>
        /// Represents whether the chart is visible or hidden
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        /// Specifies that the chart use the 1904 date system.
        /// </summary>
        bool IsDate1904 { get; set; }

        /// <summary>
        /// Specifies the legend.
        /// </summary>
        IExcelChartLegend Legend { get; set; }

        /// <summary>
        /// Specifies there is a surface 3D chart.
        /// </summary>
        IExcelLine3DChart Line3DChart { get; set; }

        /// <summary>
        /// Specifies there is a surface chart
        /// </summary>
        IExcelLineChart LineChart { get; set; }

        /// <summary>
        /// Represents whether the chart is locked
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Represents the chart name
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Specifies there is a bar of pie or pie of pie chart
        /// </summary>
        IExcelOfPieChart OfPieChart { get; set; }

        /// <summary>
        /// Specifies there is a pie 3d chart.
        /// </summary>
        IExcelPie3DChart Pie3DChart { get; set; }

        /// <summary>
        /// Specifies there is a pie chart
        /// </summary>
        IExcelPieChart PieChart { get; set; }

        /// <summary>
        /// Represents the format settings for the plot area.
        /// </summary>
        IExcelChartFormat PlotAreaFormat { get; set; }

        /// <summary>
        /// Represents the layout of the  plot area
        /// </summary>
        Layout PlotAreaLayout { get; set; }

        /// <summary>
        /// Specifies that only visible cells shall be plotted on the chart
        /// </summary>
        /// <remarks>
        /// The default value is true
        /// </remarks>
        bool PlotVisibleOnly { get; set; }

        /// <summary>
        /// Specifies there is a radar chart.
        /// </summary>
        IExcelRadarChart RadarChart { get; set; }

        /// <summary>
        /// Specifies the chart area shll have rounded corners.
        /// </summary>
        bool RoundedCorners { get; set; }

        /// <summary>
        /// Specifies there is a scatter chart.
        /// </summary>
        IExcelScatterChart ScatterChart { get; set; }

        /// <summary>
        /// Represents the second chart in the same plot area.
        /// </summary>
        IExcelChart SecondaryChart { get; set; }

        /// <summary>
        /// Specifies whether shown chart title or not.
        /// </summary>
        bool ShowAutoTitle { get; set; }

        /// <summary>
        /// Specifies data labels over the maximun of the chart shall be shown.
        /// </summary>
        bool ShowDataLabelsOverMaximun { get; set; }

        /// <summary>
        /// Specifies the side wall of the chart.
        /// </summary>
        IExcelWall SideWall { get; set; }

        /// <summary>
        /// Specifies there is a stock chart.
        /// </summary>
        IExcelStockChart StockChart { get; set; }

        /// <summary>
        /// Specifies there is a surface 3D chart.
        /// </summary>
        IExcelSurface3DChart Surface3DChart { get; set; }

        /// <summary>
        /// Specifies there is a surface chart
        /// </summary>
        IExcelSurfaceChart SurfaceChart { get; set; }

        /// <summary>
        /// Represents the text format settings for the chart area
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }

        /// <summary>
        /// Specifies the 3-D view of the chart.
        /// </summary>
        Dt.Xls.Chart.ViewIn3D ViewIn3D { get; set; }
    }
}

