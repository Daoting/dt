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
    /// Represents the excel of pie chart, like BarOfPie or PieOfPie.
    /// </summary>
    public interface IExcelOfPieChart : IExcelPieChartBase, IExcelChartBase
    {
        /// <summary>
        /// Specifies the custom split information for a pie of pie or bar of pie chart with a custom splits.
        /// </summary>
        List<int> CustomSplitPoints { get; set; }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        /// <remarks>
        /// the value should be between 0 to 500
        /// </remarks>
        int GapWidth { get; set; }

        /// <summary>
        /// Specifes the size of the second pie or bar of a pie chart, as a percentage of the size of the first pie.
        /// </summary>
        int SecondPieSize { get; set; }

        /// <summary>
        /// Specifies series lines for the chart
        /// </summary>
        ExcelChartLines SeriesLines { get; set; }

        /// <summary>
        /// Specifies a value that shall be used to determine which data points are in the second pie or bar on a
        /// pie of pie or bar of pie chart.
        /// </summary>
        double SplitPosition { get; set; }

        /// <summary>
        /// Specifies the possible ways to split a pie of pie or bar of pie chart.
        /// </summary>
        OfPieChartSplitType SplitType { get; set; }
    }
}

