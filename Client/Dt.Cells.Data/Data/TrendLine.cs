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
    /// 
    /// </summary>
    internal class TrendLine : SpreadChartElement
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Dt.Cells.Data.TrendLine" /> class.
        /// </summary>
        internal TrendLine()
        {
            this.Order = 2;
            this.Period = 2;
            this.DisplayEquation = true;
            this.DisplayRSquaredValue = true;
            this.TrendlineType = TrendLineType.Linear;
        }

        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends before the data for the series that is being trended.
        /// On no-scatter charts, the value shall be 0 or 0.5.
        /// </summary>
        internal double Backward { get; set; }

        /// <summary>
        /// Specifies that the equation for the trendline is displayed on the chart.
        /// </summary>
        internal bool DisplayEquation { get; set; }

        /// <summary>
        /// Specifies that the R-squared value of the trendline is displayed on the chart.
        /// </summary>
        internal bool DisplayRSquaredValue { get; set; }

        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends after the data for the series that is being trended.
        /// </summary>
        internal double Forward { get; set; }

        /// <summary>
        /// Specifies the value where the trendline shall cross the y axis.
        /// </summary>
        internal double? Intercept { get; set; }

        /// <summary>
        /// Specifies the name of the trendline.
        /// </summary>
        internal string Name { get; set; }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        internal int Order { get; set; }

        /// <summary>
        /// Specified the period of the trend line for a moving average trend line.
        /// </summary>
        /// <remarks>
        /// It should be ignored for other trend line types.
        /// </remarks>
        internal int Period { get; set; }

        /// <summary>
        /// Specifies the label for the trendline
        /// </summary>
        internal Dt.Cells.Data.TrendLineLabel TrendLineLabel { get; set; }

        /// <summary>
        /// Specifies the style of the trendline.
        /// </summary>
        internal TrendLineType TrendlineType { get; set; }
    }
}

