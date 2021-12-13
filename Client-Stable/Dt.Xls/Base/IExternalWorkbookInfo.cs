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
    /// An interface used to represents external referenced workbook info
    /// </summary>
    public interface IExternalWorkbookInfo
    {
        /// <summary>
        /// Get the collection of named cell ranges of the external workbook
        /// </summary>
        List<IName> DefinedNames { get; }

        /// <summary>
        /// Get the external workbook name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the collection of sheet names of the external workbook
        /// </summary>
        List<string> SheetNames { get; }
    }
}

