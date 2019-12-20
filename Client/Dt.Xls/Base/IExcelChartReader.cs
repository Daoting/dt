#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Xls.Chart;
using System;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents a collection of generalized method which be used to read excel charts.
    /// </summary>
    public interface IExcelChartReader
    {
        /// <summary>
        /// Set a chart to the sheet.
        /// </summary>
        /// <param name="sheetIndex">The sheet.</param>
        /// <param name="chart">The chart.</param>
        void SetExcelChart(int sheetIndex, IExcelChart chart);
        /// <summary>
        /// Set an excel image object to the sheet.
        /// </summary>
        /// <param name="sheetIndex">Index of the sheet.</param>
        /// <param name="excelImage">The excel image.</param>
        void SetExcelImage(int sheetIndex, IExcelImage excelImage);
    }
}

