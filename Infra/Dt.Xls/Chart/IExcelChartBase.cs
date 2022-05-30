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
    /// Represetns the common chart settings for all chart type
    /// </summary>
    public interface IExcelChartBase
    {
        /// <summary>
        /// Speicifies the bar chart type.
        /// </summary>
        ExcelChartType ChartType { get; }

        /// <summary>
        /// Specifies the settings for the data labels.
        /// </summary>
        IExcelDataLabels DataLabels { get; set; }

        /// <summary>
        /// Vary colors by point
        /// </summary>
        bool VaryColors { get; set; }
    }
}

