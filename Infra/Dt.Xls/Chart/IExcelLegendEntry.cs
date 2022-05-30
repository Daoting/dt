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
    /// Specifies a lengend entry
    /// </summary>
    public interface IExcelLegendEntry
    {
        /// <summary>
        /// Specifies that the chart element specified by its containing element shall be deleted from the chart.
        /// </summary>
        bool Delete { get; set; }

        /// <summary>
        /// Specifies the legend entry index.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }
    }
}

