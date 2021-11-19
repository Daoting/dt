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

namespace Dt.Xls
{
    /// <summary>
    /// Specifies properties of a Excel sparkline group.
    /// </summary>
    public interface IExcelSparklineGroup
    {
        /// <summary>
        /// Specifies the color of the horizontal axis for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor AxisColor { get; set; }

        /// <summary>
        /// Date Axis Range
        /// </summary>
        IExternalRange DateAxisRange { get; set; }

        /// <summary>
        /// Specifies how empty cells are plotted for all sparklines in the sparkline group
        /// </summary>
        ExcelSparklineEmptyCellDisplayAs DisplayEmptyCellAs { get; set; }

        /// <summary>
        /// Specifies the color of the first data point for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor FirstColor { get; set; }

        /// <summary>
        /// Specifies the color of the highest data point for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor HighColor { get; set; }

        /// <summary>
        /// A flag specifies whether this sparkline group uses a data axis.
        /// </summary>
        bool IsDateAxis { get; set; }

        /// <summary>
        /// Specifies the color of the last data point for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor LastColor { get; set; }

        /// <summary>
        /// Specifies the line weight for each sparkline in the sparkline group, where the weight
        /// is measured in points.
        /// </summary>
        double LineWeight { get; set; }

        /// <summary>
        /// Specifies the color of the lowest data point for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor LowColor { get; set; }

        /// <summary>
        /// Specifies the maximums for the vertical axis that is shared across all sparklines in this sparkline group
        /// </summary>
        /// <remarks>It should be only valid when the MaxAxisType is Custom.</remarks>
        double ManualMaxValue { get; set; }

        /// <summary>
        /// Specifies the minimum for the vertical axis that is shared across all sparklines in this sparkline group
        /// </summary>
        /// <remarks>It should be only valid when the MinAxisType is Custom.</remarks>
        double ManualMinValue { get; set; }

        /// <summary>
        /// Specifies the color of the data markers for each sparkline in this sparkline group.
        /// </summary>
        IExcelColor MarkersColor { get; set; }

        /// <summary>
        /// Specifies how the vertical axis maximums for the sparklines in this sparkline group are calculated.
        /// </summary>
        ExcelSparklineAxisMinMax MaxAxisType { get; set; }

        /// <summary>
        /// Specifies how the vertical axis minimums for the sparklines in this sparkline group are calculated.
        /// </summary>
        ExcelSparklineAxisMinMax MinAxisType { get; set; }

        /// <summary>
        /// Specifies the color of the negative data points for each sparkline in this sparkline group
        /// </summary>
        IExcelColor NegativeColor { get; set; }

        /// <summary>
        /// Specifies whether each sparkline in the sparkline group is displayed in a right-to-left manner.
        /// </summary>
        bool RightToLeft { get; set; }

        /// <summary>
        /// Specifies the color for each sparkline in this sparkline group
        /// </summary>
        IExcelColor SeriesColor { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowFirstDifferently { get; set; }

        /// <summary>
        /// Specifies whether data in hidden cells are plotted for the sparklines in this sparkline group.
        /// </summary>
        bool ShowHidden { get; set; }

        /// <summary>
        /// Specifies whether the data point with the highest value are formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowHighestDifferently { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowLastDifferently { get; set; }

        /// <summary>
        /// Specifies whether the data points with the lowest value are formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowLowestDifferently { get; set; }

        /// <summary>
        /// Specifies whether data markers are displayed for each sparkline in this sparkline group.
        /// </summary>
        bool ShowMarkers { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowNegativeDifferently { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        bool ShowXAxis { get; set; }

        /// <summary>
        /// Specifies properties for individual sparklines
        /// </summary>
        List<IExcelSparkline> Sparklines { get; set; }

        /// <summary>
        /// Specifies the type of the sparkline.
        /// </summary>
        ExcelSparklineType SparklineType { get; set; }
    }
}

