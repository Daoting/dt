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
    /// Represents the common settings for all surface kind chart
    /// </summary>
    public interface IExcelSurfaceChartBase : IExcelChartBase
    {
        /// <summary>
        /// Represents the category axis info.
        /// </summary>
        IExcelChartAxis AxisX { get; set; }

        /// <summary>
        /// Represents the value axis info.
        /// </summary>
        IExcelChartAxis AxisY { get; set; }

        /// <summary>
        /// Represeents the series axis info.
        /// </summary>
        IExcelChartAxis AxisZ { get; set; }

        /// <summary>
        /// A collection of formatting bands for a surface chart indexed from low to high.
        /// </summary>
        List<int> BandFormats { get; set; }

        /// <summary>
        /// Specifies the data series for the surface chart.
        /// </summary>
        List<ExcelSurfaceSeries> SurfaceSeries { get; set; }
    }
}

