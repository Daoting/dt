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
    /// Represent workbook properties.
    /// </summary>
    public class ExcelWorkbookPropery : IExcelWorkbookPropery
    {
        /// <summary>
        /// Flag indicates whether the workbook use 1904 date system or not.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if this instance is using 1904 date system; otherwise, <see langword="false" />.
        /// </value>
        /// <remarks>
        /// Microsoft windows use 1900 date system by default. Apple Macintosh use 1904 date system by default.
        /// </remarks>
        public bool IsDate1904 { get; set; }

        /// <summary>
        /// Get or set a value indicating whether save external links when save the file.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if save external links; otherwise, <see langword="false" />.
        /// </value>
        public bool SaveExternalLinks { get; set; }
    }
}

