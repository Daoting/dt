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
    /// Represents a data table.
    /// </summary>
    public interface IExcelChartDataTable
    {
        /// <summary>
        /// Represent the data tale format settings
        /// </summary>
        IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Indicating whether show Horizontal border in data table.
        /// </summary>
        bool ShowHorizontalBorder { get; set; }

        /// <summary>
        /// Indicating whether show legend keys in data tale
        /// </summary>
        bool ShowLegendKeys { get; set; }

        /// <summary>
        /// Indicating whether show outline border in data table.
        /// </summary>
        bool ShowOutlineBorder { get; set; }

        /// <summary>
        /// Indicating whether show vertical border in data table
        /// </summary>
        bool ShowVerticalBorder { get; set; }

        /// <summary>
        /// Represents the data table text formattings
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }
    }
}

