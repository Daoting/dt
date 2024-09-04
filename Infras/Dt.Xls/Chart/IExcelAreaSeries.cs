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
    /// Specifies a series on an area chart.
    /// </summary>
    public interface IExcelAreaSeries : IExcelChartSeriesBase
    {
        /// <summary>
        /// Represents the first error bars.
        /// </summary>
        IExcelErrorBars FirstErrorBars { get; set; }

        /// <summary>
        /// Represents the picture options when the fill is blip fill.
        /// </summary>
        Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        /// <summary>
        /// Represents the second error bars.
        /// </summary>
        IExcelErrorBars SecondErrorBars { get; set; }

        /// <summary>
        /// Represents collection of trend lines.
        /// </summary>
        List<IExcelTrendLine> Trendlines { get; set; }
    }
}

