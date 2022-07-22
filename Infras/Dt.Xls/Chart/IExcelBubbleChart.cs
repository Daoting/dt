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
    /// Specifies an excel bubble chart.
    /// </summary>
    public interface IExcelBubbleChart : IExcelChartBase
    {
        /// <summary>
        /// Represents the category axis info.
        /// </summary>
        IExcelChartAxis AxisX { get; set; }

        /// <summary>
        /// Represents the value axis info.
        /// </summary>
        IExcelChartAxis AxisY { get; set; }

        /// <summary>
        /// Specifies the scale factor for the bubble chart.
        /// </summary>
        int BubbleScale { get; set; }

        /// <summary>
        /// Specifies the series of the bubble chart.
        /// </summary>
        List<ExcelBubbleSeries> BubbleSeries { get; set; }

        /// <summary>
        /// Specifies negative sized bubble shall be shown on a bubble chart.
        /// </summary>
        bool ShowNegativeBubbles { get; set; }

        /// <summary>
        /// Specifies how the bubble size values are represented on the chart.
        /// </summary>
        BubbleSizeRepresents SizeRepresents { get; set; }
    }
}

