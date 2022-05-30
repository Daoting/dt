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
    /// Specifies a date axis for the chart.
    /// </summary>
    public interface IExcelChartDateAxis : IExcelChartAxis
    {
        /// <summary>
        /// Specifies the base time unit
        /// </summary>
        AxisTimeUnit BaseTimeUnit { get; set; }

        /// <summary>
        /// Specifies this axis is a date or text axis based on the data that is used for the axis lables. not a specific choice.
        /// </summary>
        bool IsAutomaticCategoryAxis { get; set; }

        /// <summary>
        /// Specifies the distance of lables from the axis.
        /// </summary>
        int LabelOffset { get; set; }

        /// <summary>
        /// Specifies the major time unit
        /// </summary>
        AxisTimeUnit MajorTimeUnit { get; set; }

        /// <summary>
        /// Specifies the distance between major ticks.
        /// </summary>
        int MajorUnit { get; set; }

        /// <summary>
        /// Specifies the minor time unit
        /// </summary>
        AxisTimeUnit MinorTimeUnit { get; set; }

        /// <summary>
        /// Specifies the distance betwwen minor ticks.
        /// </summary>
        int MinorUnit { get; set; }
    }
}

