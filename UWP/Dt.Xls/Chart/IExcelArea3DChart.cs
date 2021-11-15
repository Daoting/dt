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
    /// Specifies an excel area 3D chart.
    /// </summary>
    public interface IExcelArea3DChart : IExcelAreaChartBase, IExcelChartBase
    {
        /// <summary>
        /// Represeents the series axis info.
        /// </summary>
        IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// Specifies the space between areas.
        /// </summary>
        int GapDepth { get; set; }
    }
}

