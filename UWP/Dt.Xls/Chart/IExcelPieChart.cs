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
    /// Represents the excel pie chart.
    /// </summary>
    public interface IExcelPieChart : IExcelPieChartBase, IExcelChartBase
    {
        /// <summary>
        /// Specifies the angle of the first pie char slice, in degrees (clockwise from up)
        /// </summary>
        int FirstSliceAngle { get; set; }
    }
}

