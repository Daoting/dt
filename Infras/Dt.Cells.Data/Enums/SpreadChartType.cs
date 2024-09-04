#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Specifies the chart type.
    /// </summary>
    public enum SpreadChartType
    {
        None,
        BarClustered,
        BarStacked,
        BarStacked100pc,
        ColumnClustered,
        ColumnStacked,
        ColumnStacked100pc,
        Line,
        LineSmoothed,
        LineStacked,
        LineStacked100pc,
        LineWithMarkers,
        LineWithMarkersSmoothed,
        LineStackedWithMarkers,
        LineStacked100pcWithMarkers,
        Pie,
        PieExploded,
        PieDoughnut,
        PieExplodedDoughnut,
        Area,
        AreaStacked,
        AreaStacked100pc,
        Radar,
        RadarWithMarkers,
        RadarFilled,
        Scatter,
        ScatterLines,
        ScatterLinesWithMarkers,
        ScatterLinesSmoothed,
        ScatterLinesSmoothedWithMarkers,
        Bubble,
        StockHighLowOpenClose
    }
}

