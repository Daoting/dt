#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Provides data for the ChartSelectionChanged event.
    /// </summary>
    public class ChartSelectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartSelectionChangedEventArgs" /> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        public ChartSelectionChangedEventArgs(SpreadChart chart)
        {
            this.Chart = chart;
        }

        /// <summary>
        /// Gets the chart.
        /// </summary>
        /// <value>
        /// The chart.
        /// </value>
        public SpreadChart Chart { get; private set; }
    }
}

