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
    /// Represents the commom settrings for all bar kind charts
    /// </summary>
    public interface IExcelBarChartBase : IExcelChartBase
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
        /// Specifies the series collection on the bar chart.
        /// </summary>
        List<IExcelBarSeries> BarSeries { get; set; }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        /// <remarks>
        /// the value should be between 0 to 500
        /// </remarks>
        int GapWidth { get; set; }
    }
}

