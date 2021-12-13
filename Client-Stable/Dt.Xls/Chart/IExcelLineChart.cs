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
    /// Specifies an excel line chart.
    /// </summary>
    public interface IExcelLineChart : IExcelLineChartBase, IExcelChartBase
    {
        /// <summary>
        /// Represetns the high low lines
        /// </summary>
        ExcelChartLines HighLowLine { get; set; }

        /// <summary>
        /// Specifies the line connecting the points on the chart shall be smoonthed using Catmull-Rom splines.
        /// </summary>
        bool Smoothing { get; set; }

        /// <summary>
        /// Specifies the Up/Down bars for the line chart.
        /// </summary>
        ExcelUpDownBars UpDownBars { get; set; }
    }
}

