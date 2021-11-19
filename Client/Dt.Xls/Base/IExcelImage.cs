#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents an excle image item
    /// </summary>
    public interface IExcelImage
    {
        /// <summary>
        /// Gets or sets the anchor.
        /// </summary>
        /// <value>
        /// The anchor.
        /// </value>
        IAnchor Anchor { get; set; }

        /// <summary>
        /// Represents whether the picture is visible or hidden
        /// </summary>
        bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the type of the image.
        /// </summary>
        /// <value>
        /// The type of the image.
        /// </value>
        Dt.Xls.ImageType ImageType { get; set; }

        /// <summary>
        /// Represents whether the chart is locked
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Represents the picture format
        /// </summary>
        IExcelChartFormat PictureFormat { get; set; }

        /// <summary>
        /// Gets or sets the source array.
        /// </summary>
        /// <value>
        /// The source array.
        /// </value>
        byte[] SourceArray { get; set; }
    }
}

