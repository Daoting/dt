#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Globalization;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Interface that defines date-time and number format information for the formatter.
    /// </summary>
    internal interface IFormatProviderSupport
    {
        /// <summary>
        /// Gets or sets the date-time format information.
        /// </summary>
        /// <value>The date-time format information.</value>
        System.Globalization.DateTimeFormatInfo DateTimeFormatInfo { get; set; }

        /// <summary>
        /// Gets or sets the number format information.
        /// </summary>
        /// <value>The number format information.</value>
        System.Globalization.NumberFormatInfo NumberFormatInfo { get; set; }
    }
}

