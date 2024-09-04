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
    /// Represents an object that provides data to a sparkline.
    /// </summary>
    public interface ISparklineData
    {
        /// <summary>
        /// Occurs when data has changed.
        /// </summary>
        event EventHandler DataChanged;

        /// <summary>
        /// Gets the value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The value.</returns>
        object GetValue(int index);

        /// <summary>
        /// Gets the data count.
        /// </summary>
        int Count { get; }
    }
}

