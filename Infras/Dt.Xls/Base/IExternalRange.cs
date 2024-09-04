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
    /// Defines properties used to represents a external range
    /// </summary>
    public interface IExternalRange : IRange
    {
        /// <summary>
        /// External References workbook name.
        /// </summary>
        string WorkbookName { get; set; }

        /// <summary>
        /// External References workbook name
        /// </summary>
        string WorksheetName { get; set; }
    }
}

