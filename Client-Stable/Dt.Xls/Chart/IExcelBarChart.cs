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
    /// Specifies an excel bar chart
    /// </summary>
    public interface IExcelBarChart : IExcelBarChartBase, IExcelChartBase
    {
        /// <summary>
        /// Specifies how much bars and columns shall overlap on 2-D charts.
        /// </summary>
        /// <remarks>
        /// the value should be between -100 and 100
        /// </remarks>
        int Overlap { get; set; }

        /// <summary>
        /// Specifies series lines for the chart
        /// </summary>
        ExcelChartLines SeriesLines { get; set; }
    }
}

