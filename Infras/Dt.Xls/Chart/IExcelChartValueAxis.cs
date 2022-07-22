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
    /// Specifies a value axis
    /// </summary>
    public interface IExcelChartValueAxis : IExcelChartAxis
    {
        /// <summary>
        /// Gets or sets the cross between.
        /// </summary>
        /// <value>
        /// The cross between.
        /// </value>
        Dt.Xls.Chart.CrossBetween CrossBetween { get; set; }

        /// <summary>
        /// Gets or sets the dislay units.
        /// </summary>
        /// <value>
        /// The dislay units.
        /// </value>
        DisplayUnits DislayUnits { get; set; }

        /// <summary>
        /// Specifies the distance between major ticks.
        /// </summary>
        double MajorUnit { get; set; }

        /// <summary>
        /// Specifies the distance betwwen minor ticks.
        /// </summary>
        double MinorUnit { get; set; }
    }
}

