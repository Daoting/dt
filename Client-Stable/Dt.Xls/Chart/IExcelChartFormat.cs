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
    /// Specifies the formatting for the chart element.
    /// </summary>
    public interface IExcelChartFormat
    {
        /// <summary>
        /// Gets or sets the fill format.
        /// </summary>
        /// <value>
        /// The fill format.
        /// </value>
        IFillFormat FillFormat { get; set; }

        /// <summary>
        /// Gets or sets the line format.
        /// </summary>
        /// <value>
        /// The line format.
        /// </value>
        ILineFormat LineFormat { get; set; }
    }
}

