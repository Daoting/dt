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
    /// Defines properties used to represents excel print options
    /// </summary>
    public interface IExcelPrintOptions
    {
        /// <summary>
        /// Get or set whether print horizontal centered or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if it should horizontal centered; otherwise, <see langword="false" />.
        /// </value>
        bool HorizontalCentered { get; set; }

        /// <summary>
        /// Get or set whether print the gridline or not.
        /// </summary>
        /// /// <value>
        /// <see langword="true" /> if it should print gridline; otherwise, <see langword="false" />.
        /// </value>
        bool PrintGridLine { get; set; }

        /// <summary>
        /// Get or set whether print headings or not..
        /// </summary>
        /// /// <value>
        /// <see langword="true" /> if it should print row column headers; otherwise, <see langword="false" />.
        /// </value>
        bool PrintRowColumnsHeaders { get; set; }

        /// <summary>
        /// Get or set whether print vertical centered or not.
        /// </summary>
        /// /// <value>
        /// <see langword="true" /> if it should vertical centered; otherwise, <see langword="false" />.
        /// </value>
        bool VerticalCentered { get; set; }
    }
}

