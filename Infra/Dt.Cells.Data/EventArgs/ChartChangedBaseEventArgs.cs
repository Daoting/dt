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
    /// Provides a base class for event data of chart changed event.
    /// </summary>
    public class ChartChangedBaseEventArgs : EventArgs
    {
        Dt.Cells.Data.ChartArea _chartArea;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.ChartChangedBaseEventArgs" /> class.
        /// </summary>
        /// <param name="chart">The chart.</param>
        /// <param name="chartArea">The chart area.</param>
        /// <param name="property">The property.</param>
        public ChartChangedBaseEventArgs(SpreadChartBase chart, Dt.Cells.Data.ChartArea chartArea, string property)
        {
            this.Chart = chart;
            this.Property = property;
            this._chartArea = chartArea;
        }

        /// <summary>
        /// Gets the chart.
        /// </summary>
        /// <value>
        /// The chart.
        /// </value>
        public SpreadChartBase Chart { get; private set; }

        /// <summary>
        /// Gets the chart area.
        /// </summary>
        /// <value>
        /// The chart area.
        /// </value>
        public Dt.Cells.Data.ChartArea ChartArea
        {
            get { return  this._chartArea; }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <value>
        /// The property.
        /// </value>
        public string Property { get; private set; }
    }
}

