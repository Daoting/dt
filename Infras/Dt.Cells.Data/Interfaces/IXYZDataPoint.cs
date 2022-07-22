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
    /// Defines the interface for XYZDataPoint.
    /// </summary>
    public interface IXYZDataPoint : IXYDataPoint, IDataPoint
    {
        /// <summary>
        /// Gets the z value.
        /// </summary>
        /// <value>
        /// The z value.
        /// </value>
        double? ZValue { get; }
    }
}

