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
using System.Collections.Generic;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represents a collection of generalized method which be used to read excel charts.
    /// </summary>
    public interface IExcelChartWriter
    {
        /// <summary>
        /// get all the charts in the specified worksheet
        /// </summary>
        List<IExcelChart> GetWorksheetCharts(int sheetIndex);
        /// <summary>
        /// Get all images in the specified worksheet
        /// </summary>
        List<IExcelImage> GetWorkSheetImages(int sheetIndex);
    }
}

