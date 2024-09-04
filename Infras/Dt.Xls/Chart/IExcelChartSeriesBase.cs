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
    /// Specifies a series
    /// </summary>
    public interface IExcelChartSeriesBase
    {
        /// <summary>
        /// Specifies the data used for the category axis
        /// </summary>
        IExcelChartCategoryAxisData CategoryAxisData { get; set; }

        /// <summary>
        /// Represents the data labels of this series.
        /// </summary>
        IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Specifies collection od data points
        /// </summary>
        List<IExcelDataPoint> DataPoints { get; set; }

        /// <summary>
        /// Represents the formatting options of this series.
        /// </summary>
        IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Specifies the index of the containing element. It shall determine which of the parent's children
        /// collection this elemeht applies to.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Specifies the order of the series on the collection. It's 0 based
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Specifies text for a series name.
        /// </summary>
        IExcelSeriesName SeriesName { get; set; }

        /// <summary>
        /// Specifies the data values which shall be used to define the locatin of the data markers on a charts
        /// </summary>
        IExcelSeriesValue SeriesValue { get; set; }
    }
}

