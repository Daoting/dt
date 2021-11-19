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
    /// Specifies a series on a bar chart.
    /// </summary>
    public interface IExcelBarSeries : IExcelChartSeriesBase
    {
        /// <summary>
        /// Represents the error bars.
        /// </summary>
        IExcelErrorBars ErrorBars { get; set; }

        /// <summary>
        /// Specifies the parent element shall invert its colors if the value is negative.
        /// </summary>
        bool InvertIfNegative { get; set; }

        /// <summary>
        /// Represents the negative solid fill format.
        /// </summary>
        SolidFillFormat NegativeSolidFillFormat { get; set; }

        /// <summary>
        /// Represent the picture options settings when the fill is blip fill.
        /// </summary>
        Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        /// <summary>
        /// Represents collection of trend lines.
        /// </summary>
        List<IExcelTrendLine> Trendlines { get; set; }
    }
}

