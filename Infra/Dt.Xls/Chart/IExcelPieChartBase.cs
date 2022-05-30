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
    /// Represents the common settings for all pie kind charts
    /// </summary>
    public interface IExcelPieChartBase : IExcelChartBase
    {
        /// <summary>
        /// Specifies the series collection on the pie chart.
        /// </summary>
        List<IExcelPieSeries> PieSeries { get; set; }
    }
}

