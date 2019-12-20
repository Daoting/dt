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
    /// Specifies a series on a Bulle chart.
    /// </summary>
    public interface IExcelBubbleSeries : IExcelChartSeriesBase
    {
        /// <summary>
        /// Specifies that the bubbles have a 3-D effect applied to them.
        /// </summary>
        bool Bubble3D { get; set; }

        /// <summary>
        /// Specifies the data for the sizes of the bubbles on the bubble chart.
        /// </summary>
        IExcelSeriesValue BubbleSize { get; set; }

        /// <summary>
        /// Specifies the first error bars.
        /// </summary>
        ExcelErrorBars FirstErrorBars { get; set; }

        /// <summary>
        /// Invert if negative.
        /// </summary>
        bool InvertIfNegative { get; set; }

        /// <summary>
        /// Specifies the second error bars.
        /// </summary>
        ExcelErrorBars SecondErrorBars { get; set; }

        /// <summary>
        /// Represents collection of trend lines.
        /// </summary>
        List<IExcelTrendLine> Trendlines { get; set; }
    }
}

