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
    /// Specifies the Excel Doughnut chart
    /// </summary>
    public interface IExcelDoughnutChart : IExcelPieChartBase, IExcelChartBase
    {
        /// <summary>
        /// Specifies the angle of the first pie char slice, in degrees (clockwise from up)
        /// </summary>
        int FirstSliceAngle { get; set; }

        /// <summary>
        /// Specifies the size of the hole in a doughnut chart group
        /// </summary>
        int HoleSize { get; set; }
    }
}

