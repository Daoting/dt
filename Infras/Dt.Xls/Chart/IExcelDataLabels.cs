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
    /// Specifies the settings for the data labels for an entire series or the entire chart.
    /// </summary>
    public interface IExcelDataLabels
    {
        /// <summary>
        /// The data label items in this IDataLabel collection
        /// </summary>
        List<IExcelDataLabel> DataLabelList { get; }

        /// <summary>
        /// Specifies that the chart element specifies by its containing element shall be deleted from the chart.
        /// </summary>
        bool Delete { get; set; }

        /// <summary>
        /// Specifies the leader lines format.
        /// </summary>
        IExcelChartFormat LeaderLineFormat { get; set; }

        /// <summary>
        /// Specifies the number format for the data label.
        /// </summary>
        string NumberFormat { get; set; }

        /// <summary>
        /// Specifies whethere the data label use the same number formats as the cells that contain the data for the associated data point.
        /// </summary>
        bool NumberFormatLinked { get; set; }

        /// <summary>
        /// Specifies the text orientation of the data lable. 
        /// </summary>
        /// <remarks>
        /// The value should be between -90 and 90.
        /// </remarks>
        int Orientation { get; set; }

        /// <summary>
        /// Specifies the position of the data label.
        /// </summary>
        DataLabelPosition Position { get; set; }

        /// <summary>
        /// Specifies text that shall be used to separate the parts of the data label, the default is comma.
        /// </summary>
        string Separator { get; set; }

        /// <summary>
        /// Specifies the formatting options for the data label.
        /// </summary>
        IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies whether the data label show the bubble size.
        /// </summary>
        bool ShowBubbleSize { get; set; }

        /// <summary>
        /// Specifies whether the data label show the category name.
        /// </summary>
        bool ShowCategoryName { get; set; }

        /// <summary>
        /// Specifies leader lines shall be shown for data label
        /// </summary>
        bool ShowLeaderLines { get; set; }

        /// <summary>
        /// Specifies whether the data label show the legend key.
        /// </summary>
        bool ShowLegendKey { get; set; }

        /// <summary>
        /// Specifies whether the data label show the percentage.
        /// </summary>
        bool ShowPercentage { get; set; }

        /// <summary>
        /// Specifies whether the data label show the series name.
        /// </summary>
        bool ShowSeriesName { get; set; }

        /// <summary>
        /// Specifies whether the data label show the value.
        /// </summary>
        bool ShowValue { get; set; }

        /// <summary>
        /// Specifies the text of the data label.
        /// </summary>
        string Text { get; set; }

        /// <summary>
        /// Specifies the text format for the data label.
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }
    }
}

