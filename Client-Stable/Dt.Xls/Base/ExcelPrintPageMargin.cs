#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    /// <summary>
    /// Represent the page margin settings for the entire workbook.
    /// </summary>
    public class ExcelPrintPageMargin : IExcelPrintPageMargin
    {
        /// <summary>
        /// Get or set the bottom margin
        /// </summary>
        /// <value>The value of the bottom margin</value>
        public double Bottom { get; set; }

        /// <summary>
        /// Get or set the footer margin
        /// </summary>
        /// <value>The value of the footer margin</value>
        public double Footer { get; set; }

        /// <summary>
        /// Get or set the header margin
        /// </summary>
        /// <value>The value of the header margin</value>
        public double Header { get; set; }

        /// <summary>
        /// Get or set the left margin
        /// </summary>
        /// <value>The value of the left margin</value>
        public double Left { get; set; }

        /// <summary>
        /// Get or set the right margin
        /// </summary>
        /// <value>The value of the right margin</value>
        public double Right { get; set; }

        /// <summary>
        /// Get or set the top margin
        /// </summary>
        /// <value>The value of the top margin</value>
        public double Top { get; set; }
    }
}

