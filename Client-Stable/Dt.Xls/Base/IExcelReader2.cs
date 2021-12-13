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
    /// Represents a collection of generalized methods which be used to read Excel file or stream.
    /// </summary>
    public interface IExcelReader2
    {
        /// <summary>
        /// Set the tab color to a specific sheet.
        /// </summary>
        /// <param name="sheet">The zero base sheet index</param>
        /// <param name="color"></param>
        void SetExcelSheetTabColor(int sheet, IExcelColor color);
    }
}

