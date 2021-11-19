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
    /// Specifies an excel Line 3D chart.
    /// </summary>
    public interface IExcelLine3DChart : IExcelLineChartBase, IExcelChartBase
    {
        /// <summary>
        /// Represeents the series axis info.
        /// </summary>
        IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// Specifies the space between bar or column clusters, as a percentage of the bar or column width.
        /// </summary>
        int GapDepth { get; set; }
    }
}

