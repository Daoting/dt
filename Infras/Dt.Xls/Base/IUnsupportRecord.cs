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
    /// Represents a unsupport record in the excel.
    /// </summary>
    public interface IUnsupportRecord
    {
        /// <summary>
        /// Unsupport item category
        /// </summary>
        RecordCategory Category { get; }

        /// <summary>
        /// Unsupport item file type
        /// </summary>
        ExcelFileType FileType { get; }

        /// <summary>
        /// Unsupport item content
        /// </summary>
        object Value { get; }
    }
}

