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
    /// Represents methods used to support excel lossless editing features.
    /// </summary>
    public interface IExcelLosslessWriter
    {
        /// <summary>
        /// Get the codename for the specify sheet.
        /// </summary>
        string GetCodeName(int sheetIndex);
        /// <summary>
        /// Get the excel sheet type 
        /// </summary>
        ExcelSheetType GetSheetType(int sheetIndex);
        /// <summary>
        /// Get the collection of unsupport records
        /// </summary>
        List<IUnsupportRecord> GetUnsupportItems(int sheetIndex);
    }
}

