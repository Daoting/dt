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
    /// Represents a chart Axis.
    /// </summary>
    public interface IExcelChartAxis
    {
        /// <summary>
        /// Specifies the position for an axis.
        /// </summary>
        Dt.Xls.Chart.AxisPosition AxisPosition { get; set; }

        /// <summary>
        /// Specifies a title for the Axis
        /// </summary>
        IExcelChartTitle AxisTitle { get; set; }

        /// <summary>
        /// Specifies the ID of axis that this axis cross.
        /// </summary>
        int CrossAx { get; set; }

        /// <summary>
        /// Specifies how this axis crosses the perpendicular axis.
        /// </summary>
        AxisCrosses Crosses { get; set; }

        /// <summary>
        /// Specifies where on the axis the perpendicular axis crosses. The units are dependent on the type of axis. 
        /// </summary>
        /// <remarks>
        /// When  the AxisType is Value, the value is a decimal number on the value axis.
        /// When the AxisType is Date, the date is defined as a integer number of days relative to the base data of the current date base.
        /// When the AxisType is Category, the value is an integer category number, starting with 1 as the first category.
        /// </remarks>
        double CrosssAt { get; set; }

        /// <summary>
        /// Specifies that chart element specifies by its containing element shall be deleted from the chart.
        /// </summary>
        bool Delete { get; set; }

        /// <summary>
        /// Identify used to mark this axis.
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// Specifies major gridlines.
        /// </summary>
        IExcelGridLines MajorGridlines { get; set; }

        /// <summary>
        /// Specifies the major tick marks for the axis.
        /// </summary>
        TickMark MajorTickMark { get; set; }

        /// <summary>
        /// Specifies minor gridlines.
        /// </summary>
        IExcelGridLines MinorGridlines { get; set; }

        /// <summary>
        /// Specifies the minor tick marks for the axis.
        /// </summary>
        TickMark MinorTickMark { get; set; }

        /// <summary>
        /// Specifies the number format for the data label.
        /// </summary>
        string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whethere the data label use the same number formats as the cells that contain the data for the associated data point.
        /// </summary>
        bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Specifies additional axis scale settings.
        /// </summary>
        IScaling Scaling { get; set; }

        /// <summary>
        /// Specifies the axis format
        /// </summary>
        IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }

        /// <summary>
        /// Specifies the possible positions for the tick labels.
        /// </summary>
        Dt.Xls.Chart.TickLabelPosition TickLabelPosition { get; set; }

        /// <summary>
        /// Specifies the axis type.
        /// </summary>
        ExcelChartAxisType Type { get; }
    }
}

