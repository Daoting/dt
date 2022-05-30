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
    /// Represents the commom settings for all Area 
    /// </summary>
    public interface IExcelAreaChartBase : IExcelChartBase
    {
        /// <summary>
        /// Specifies the series collection on the area chart.
        /// </summary>
        List<IExcelAreaSeries> AreaSeries { get; set; }

        /// <summary>
        /// Represents the category axis info.
        /// </summary>
        IExcelChartAxis AxisX { get; set; }

        /// <summary>
        /// Represents the value axis info.
        /// </summary>
        IExcelChartAxis AxisY { get; set; }

        /// <summary>
        /// specifies drop lines.
        /// </summary>
        ExcelChartLines DropLine { get; set; }
    }
}

