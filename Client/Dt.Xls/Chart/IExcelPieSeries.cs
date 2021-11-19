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
    /// Specifies a series on a pie chart.
    /// </summary>
    public interface IExcelPieSeries : IExcelChartSeriesBase
    {
        /// <summary>
        /// Specifies the amount the data point shall be moved from the center of the pie.
        /// </summary>
        int Explosion { get; set; }
    }
}

