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

namespace Dt.Xls
{
    /// <summary>
    /// Represents a collection of generalized method which be used to read Excel Sparkline settings.
    /// </summary>
    /// <remarks>Excel 2010 add sparkline feature. A Sparkline is basically a little chart displayed in a cell
    /// representing your selected data set that allows you to quickly and easily spot trends at a glance.
    /// </remarks>
    public interface IExcelSparklineReader
    {
        /// <summary>
        /// Set the groups of sparklines on the sheet.
        /// </summary>
        /// <param name="sheet">The zero based sheet index.</param>
        /// <param name="excelSparklineGroups">The excel sparkline groups.</param>
        void SetExcelSparklineGroups(int sheet, List<IExcelSparklineGroup> excelSparklineGroups);
    }
}

