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
    /// Defines number format setting
    /// </summary>
    public interface IExcelNumberFormat : IEquatable<IExcelNumberFormat>
    {
        /// <summary>
        /// Gets the number format code.
        /// </summary>
        /// <value>The number format code.</value>
        string NumberFormatCode { get; }

        /// <summary>
        /// Gets the number format id.
        /// </summary>
        /// <value>The number format id.</value>
        int NumberFormatId { get; }
    }
}

