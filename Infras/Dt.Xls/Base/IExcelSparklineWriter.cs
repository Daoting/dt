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
    /// Represents a collection of method which be used to write customized sparkline to excel.
    /// </summary>
    /// <remarks>
    /// Excel support sparkline since Excel 2010.
    /// </remarks>
    public interface IExcelSparklineWriter
    {
        /// <summary>
        /// Get the sparkline group settings of the specified sheet.
        /// </summary>
        /// <param name="sheet">A zero based sheet index</param>
        /// <returns>A collection of IExcelSparklineGroup represents properties of excel sparkline group</returns>
        List<IExcelSparklineGroup> GetExcelSparkLineGroups(int sheet);
    }
}

