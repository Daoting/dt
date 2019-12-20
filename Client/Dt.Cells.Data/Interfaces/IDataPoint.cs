#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Defines the interface for DataPoint.
    /// </summary>
    public interface IDataPoint
    {
        /// <summary>
        /// Gets the data series.
        /// </summary>
        /// <value>
        /// The data series.
        /// </value>
        SpreadDataSeries DataSeries { get; }

        /// <summary>
        /// Gets the index of the point.
        /// </summary>
        /// <value>
        /// The index of the point.
        /// </value>
        int PointIndex { get; }

        /// <summary>
        /// Gets the index of the series.
        /// </summary>
        /// <value>
        /// The index of the series.
        /// </value>
        int SeriesIndex { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        double? Value { get; }
    }
}

