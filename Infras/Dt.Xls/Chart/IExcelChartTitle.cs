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
    /// Specifies a title
    /// </summary>
    public interface IExcelChartTitle
    {
        /// <summary>
        /// Specifies a title.
        /// </summary>
        Dt.Xls.Chart.Layout Layout { get; set; }

        /// <summary>
        /// Specifies that other chart elements shall be allowed to overlap this chart elements
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        bool Overlay { get; set; }

        /// <summary>
        /// Specifies the text of title in RichText mode
        /// </summary>
        RichText RichTextTitle { get; set; }

        /// <summary>
        /// Specifies the chart title formatting.
        /// </summary>
        IExcelChartFormat ShapeFormat { get; set; }

        /// <summary>
        /// Specifies the text of the chart title formatting
        /// </summary>
        IExcelTextFormat TextForamt { get; set; }

        /// <summary>
        /// Specifies text to use on a chart.
        /// </summary>
        string TitleFormula { get; set; }
    }
}

