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
    /// Represents a sheet that hosts the sparkline.
    /// </summary>
    public interface ISparklineSheet
    {
        /// <summary>
        /// Occurs when the cell has changed.
        /// </summary>
        event EventHandler<CellChangedEventArgs> CellChanged;

        /// <summary>
        /// Gets the sparkline.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <returns>The sparkline.</returns>
        Sparkline GetSparkline(int row, int column);
        /// <summary>
        /// Sets the sparkline.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="column">The column.</param>
        /// <param name="sparkline">The sparkline.</param>
        void SetSparkline(int row, int column, Sparkline sparkline);
    }
}

