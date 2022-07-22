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
    /// Provides data for the ChartChanged event.
    /// </summary>
    public class ChartChangedEventArgs : ChartChangedBaseEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartChangedEventArgs" /> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="property">The property.</param>
        public ChartChangedEventArgs(SpreadChart chart, ChartArea chartArea, string property) : base(chart, chartArea, property)
        {
        }
    }
}

