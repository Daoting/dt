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
    /// Specifies the legend
    /// </summary>
    public interface IExcelChartLegend
    {
        /// <summary>
        /// Sepcifies the Legend layout.
        /// </summary>
        Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies the legend entries.
        /// </summary>
        List<IExcelLegendEntry> LegendEntries { get; set; }

        /// <summary>
        /// Specifies that other chart elements shall be allowed to overlap this chart element.
        /// </summary>
        bool Overlay { get; set; }

        /// <summary>
        /// Specifies the position of the legend.
        /// </summary>
        ExcelLegendPositon Position { get; set; }

        /// <summary>
        /// Specifies the format info.
        /// </summary>
        IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the axis text format 
        /// </summary>
        IExcelTextFormat TextFormat { get; set; }
    }
}

