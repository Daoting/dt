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
    /// Specifies a series axis for the chart.
    /// </summary>
    public interface IExcelChartSeriesAxis : IExcelChartAxis
    {
        /// <summary>
        /// Specifies how many tick lables to skip between label that is drawn.
        /// </summary>
        int TickLalelInterval { get; set; }

        /// <summary>
        /// Specifies how many tick marks shall be skipped before the next noe shall be drawn.
        /// </summary>
        int TickMarkInterval { get; set; }
    }
}

