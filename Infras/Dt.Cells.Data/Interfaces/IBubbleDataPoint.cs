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
    /// Defines the interface for BubbleDataPoint.
    /// </summary>
    public interface IBubbleDataPoint : IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Gets the size value.
        /// </summary>
        /// <value>
        /// The size value.
        /// </value>
        double? SizeValue { get; }
    }
}

