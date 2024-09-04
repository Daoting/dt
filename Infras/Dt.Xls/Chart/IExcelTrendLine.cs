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
    /// A trendline is a straight or curved line that graphically represents the general trend of the data points of a series.
    /// </summary>
    public interface IExcelTrendLine
    {
        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends before the data for the series that is being trended.
        /// On no-scatter charts, the value shall be 0 or 0.5.
        /// </summary>
        double Backward { get; set; }

        /// <summary>
        /// Specifies that the equation for the trendline is displayed on the chart.
        /// </summary>
        bool DisplayEquation { get; set; }

        /// <summary>
        /// Specifies that the R-squared value of the trendline is displayed on the chart.
        /// </summary>
        bool DisplayRSquaredValue { get; set; }

        /// <summary>
        /// Specifies the formatting for the Trendline
        /// </summary>
        IExcelChartFormat Foramt { get; set; }

        /// <summary>
        /// Specifies the number of categories (or units on a scatter chart) that the trend line extends after the data for the series that is being trended.
        /// </summary>
        double Forward { get; set; }

        /// <summary>
        /// Specifies the value where the trendline shall cross the y axis.
        /// </summary>
        double? Intercept { get; set; }

        /// <summary>
        /// Specifies the name of the trendline.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Specifies the order of the polynomial trend line. 
        /// </summary>
        int Order { get; set; }

        /// <summary>
        /// Specified the period of the trend line for a moving average trend line.
        /// </summary>
        /// <remarks>It should be ignored for other trend line types.</remarks>
        int Period { get; set; }

        /// <summary>
        /// Specifies the label for the trendline
        /// </summary>
        ExcelTrendLineLabel TrendLineLabel { get; set; }

        /// <summary>
        /// Specifies the style of the trendline.
        /// </summary>
        ExcelTrendLineType TrendlineType { get; set; }
    }
}

