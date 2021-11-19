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

namespace Dt.Xls
{
    /// <summary>
    /// Represents a collection of methods which be used to write customized data module to Excel file or stream.
    /// </summary>
    public interface IExcelWriter2
    {
        /// <summary>
        /// Gets the sheet tab color of the specified sheet.
        /// </summary>
        /// <param name="sheet">the zero based sheet index.</param>
        /// <returns>return null if the the sheet tab color is not set.</returns>
        IExcelColor GetSheetTabColor(int sheet);
    }
}

