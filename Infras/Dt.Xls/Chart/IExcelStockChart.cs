#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Xls.Chart
{
    /// <summary>
    /// Represents an excel stock chart.
    /// </summary>
    public interface IExcelStockChart : IExcelChartBase
    {
        /// <summary>
        /// Represents the category axis info.
        /// </summary>
        IExcelChartAxis AxisX { get; set; }

        /// <summary>
        /// Represents the value axis info.
        /// </summary>
        IExcelChartAxis AxisY { get; set; }

        /// <summary>
        /// specifies drop lines.
        /// </summary>
        ExcelChartLines DropLine { get; set; }

        /// <summary>
        /// Represents the high/low line format 
        /// </summary>
        ExcelChartLines HighLowLine { get; set; }

        /// <summary>
        /// Specifies the series collection on the line chart.
        /// </summary>
        List<IExcelLineSeries> LineSeries { get; set; }

        /// <summary>
        /// Specifies the Up/Down bars for the line chart.
        /// </summary>
        ExcelUpDownBars UpDownBars { get; set; }
    }
}

