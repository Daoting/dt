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
    /// Specifies the wall of the chart.
    /// </summary>
    public interface IExcelWall
    {
        /// <summary>
        /// Specifies the wall format settings.
        /// </summary>
        IExcelChartFormat Format { get; set; }

        /// <summary>
        /// Represents the picture options when the fill is a blip fill.
        /// </summary>
        Dt.Xls.Chart.PictureOptions PictureOptions { get; set; }

        /// <summary>
        /// Specifies the thickness of the walls
        /// </summary>
        int Thickness { get; set; }
    }
}

