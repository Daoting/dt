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
    /// Lossless reader used to read the records which not support in the current vesion.
    /// </summary>
    public interface IExcelLosslessReader
    {
        /// <summary>
        /// Add the unsupport records to the specified worksheet.
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet</param>
        /// <param name="unsupportRecord">The unsupport record</param>
        void AddUnsupportItem(int sheetIndex, IUnsupportRecord unsupportRecord);
        /// <summary>
        /// Set the codename to the specified worksheet.
        /// </summary>
        /// <param name="sheetIndex">The index of the sheet</param>
        /// <param name="codeName">The codename for the specified worksheet</param>
        void SetCodeName(int sheetIndex, string codeName);
    }
}

