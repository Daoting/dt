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
    /// Defines a format, such as cell shading or font color, that excel can automatically
    /// apply to cells if a specified condition is true.
    /// </summary>
    public interface IExcelConditionalFormat
    {
        /// <summary>
        /// This collection expresses conditional formatting rules applied to a particular cell or range.
        /// </summary>
        /// <value>The conditional formatting rules.</value>
        List<IExcelConditionalFormatRule> ConditionalFormattingRules { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        short Identifier { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether it's a office 2007 new added conditional format type.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is office 2007 new added conditional format; otherwise, <see langword="false" />.
        /// </value>
        bool IsOffice2007ConditionalFormat { get; set; }

        /// <summary>
        /// Gets or sets range over which these conditional formatting rules apply.
        /// </summary>
        /// <value>The conditional format scope.</value>
        List<IRange> Ranges { get; set; }
    }
}

