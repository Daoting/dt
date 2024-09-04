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
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// An implementation used to represents a excel sparkline group setting.
    /// </summary>
    public class ExcelSparklineGroup : IExcelSparklineGroup
    {
        private List<IExcelSparkline> _sparklines;

        /// <summary>
        /// Specifies the color of the horizontal axis for each sparkline in this sparkline group.
        /// </summary>
        public IExcelColor AxisColor { get; set; }

        /// <summary>
        /// Date Axis Range
        /// </summary>
        public IExternalRange DateAxisRange { get; set; }

        /// <summary>
        /// Specifies how empty cells are plotted for all sparklines in the sparkline group
        /// </summary>
        public ExcelSparklineEmptyCellDisplayAs DisplayEmptyCellAs { get; set; }

        /// <summary>
        /// Specifies the color of the first data point for each sparkline in this sparkline group.
        /// </summary>
        public IExcelColor FirstColor { get; set; }

        /// <summary>
        /// Specifies the color of the highest data point for each sparkline in this sparkline group.
        /// </summary>
        public IExcelColor HighColor { get; set; }

        /// <summary>
        /// A flag specifies whether this sparkline group uses a data axis.
        /// </summary>
        /// <value></value>
        public bool IsDateAxis { get; set; }

        /// <summary>
        /// Specifies the color of the last data point for each sparkline in this sparkline group.
        /// </summary>
        public IExcelColor LastColor { get; set; }

        /// <summary>
        /// Specifies the line weight for each sparkline in the sparkline group, where the weight
        /// is measured in points.
        /// </summary>
        /// <value></value>
        public double LineWeight { get; set; }

        /// <summary>
        /// Specifies the color of the lowest data point for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public IExcelColor LowColor { get; set; }

        /// <summary>
        /// Specifies the maximums for the vertical axis that is shared across all sparklines in this sparkline group
        /// </summary>
        /// <value></value>
        /// <remarks>It should be only valid when the MaxAxisType is Custom.</remarks>
        public double ManualMaxValue { get; set; }

        /// <summary>
        /// Specifies the minimum for the vertical axis that is shared across all sparklines in this sparkline group
        /// </summary>
        /// <value></value>
        /// <remarks>It should be only valid when the MinAxisType is Custom.</remarks>
        public double ManualMinValue { get; set; }

        /// <summary>
        /// Specifies the color of the data markers for each sparkline in this sparkline group.
        /// </summary>
        public IExcelColor MarkersColor { get; set; }

        /// <summary>
        /// Specifies how the vertical axis maximums for the sparklines in this sparkline group are calculated.
        /// </summary>
        /// <value></value>
        public ExcelSparklineAxisMinMax MaxAxisType { get; set; }

        /// <summary>
        /// Specifies how the vertical axis minimums for the sparklines in this sparkline group are calculated.
        /// </summary>
        /// <value></value>
        public ExcelSparklineAxisMinMax MinAxisType { get; set; }

        /// <summary>
        /// Specifies the color of the negative data points for each sparkline in this sparkline group
        /// </summary>
        public IExcelColor NegativeColor { get; set; }

        /// <summary>
        /// Specifies whether each sparkline in the sparkline group is displayed in a right-to-left manner.
        /// </summary>
        /// <value></value>
        public bool RightToLeft { get; set; }

        /// <summary>
        /// Specifies the color for each sparkline in this sparkline group
        /// </summary>
        /// <value>An IExcelColor instance used to represents SeriesColor</value>
        public IExcelColor SeriesColor { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowFirstDifferently { get; set; }

        /// <summary>
        /// Specifies whether data in hidden cells are plotted for the sparklines in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowHidden { get; set; }

        /// <summary>
        /// Specifies whether the data point with the highest value are formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowHighestDifferently { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowLastDifferently { get; set; }

        /// <summary>
        /// Specifies whether the data points with the lowest value are formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowLowestDifferently { get; set; }

        /// <summary>
        /// Specifies whether data markers are displayed for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowMarkers { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowNegativeDifferently { get; set; }

        /// <summary>
        /// Specifies whether the first data point is formatted differently for each sparkline in this sparkline group.
        /// </summary>
        /// <value></value>
        public bool ShowXAxis { get; set; }

        /// <summary>
        /// Specifies properties for individual sparklines
        /// </summary>
        /// <value></value>
        public List<IExcelSparkline> Sparklines
        {
            get
            {
                if (this._sparklines == null)
                {
                    this._sparklines = new List<IExcelSparkline>();
                }
                return this._sparklines;
            }
            set { this._sparklines = value; }
        }

        /// <summary>
        /// Specifies the type of the sparkline.
        /// </summary>
        /// <value></value>
        public ExcelSparklineType SparklineType { get; set; }
    }
}

