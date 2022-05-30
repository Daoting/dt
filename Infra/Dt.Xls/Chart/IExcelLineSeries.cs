#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Specifies a series on a line chart.
    /// </summary>
    public interface IExcelLineSeries : IExcelChartSeriesBase
    {
        /// <summary>
        /// Specifies the  error bars.
        /// </summary>
        ExcelErrorBars ErrorBars { get; set; }

        /// <summary>
        /// Represents the data marker settings;
        /// </summary>
        ExcelDataMarker Marker { get; set; }

        /// <summary>
        /// Specifies the line connection the points on the chart shall be smoothed using Catmull-Rom splines.
        /// </summary>
        bool Smoothing { get; set; }

        /// <summary>
        /// Represents collection of trend lines.
        /// </summary>
        List<IExcelTrendLine> Trendlines { get; set; }
    }
}

