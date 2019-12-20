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
    /// Specifies the category axis of the chart.
    /// </summary>
    public interface IExcelChartCategoryAxis : IExcelChartAxis
    {
        /// <summary>
        /// Specifies this axis is a date or text axis based on the data that is used for the axis lables. not a specific choice.
        /// </summary>
        bool IsAutomaticCategoryAxis { get; set; }

        /// <summary>
        /// Specifies the text alignment for the tick lables on the aixs.
        /// </summary>
        Dt.Xls.Chart.LabelAlignment LabelAlignment { get; set; }

        /// <summary>
        /// Specifies the distance of lables from the axis.
        /// </summary>
        /// <remarks>
        /// The value should be between 0 and 1000.
        /// </remarks>
        int LabelOffset { get; set; }

        /// <summary>
        /// Specifies the labels shall be shown as flat text.
        /// </summary>
        bool NoMultiLevelLables { get; set; }

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

