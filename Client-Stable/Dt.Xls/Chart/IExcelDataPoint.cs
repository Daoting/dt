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
    /// Specifies a single data point
    /// </summary>
    public interface IExcelDataPoint
    {
        /// <summary>
        /// Gets or sets the data point format.
        /// </summary>
        /// <value>
        /// The data point format.
        /// </value>
        IExcelChartFormat DataPointFormat { get; set; }

        /// <summary>
        /// Specifies the amount the data points shall be moved from the center of the pie.
        /// </summary>
        int? Explosion { get; set; }

        /// <summary>
        /// The index of the datapoint
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 
        /// </summary>
        bool InvertIfNegative { get; set; }

        /// <summary>
        /// Specifies that the bubbles have a 3-D effect applied to them.
        /// </summary>
        bool IsBubble3D { get; set; }

        /// <summary>
        /// Specifies the Marker used in the DataPoint
        /// </summary>
        ExcelDataMarker Marker { get; set; }

        /// <summary>
        /// Represents the picture option settings when the fill is blip fill.
        /// </summary>
        Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }
    }
}

