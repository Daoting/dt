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
    /// An implementation of IExcelSparkline used to specifies information for a single sparkline
    /// </summary>
    public class ExcelSparkline : IExcelSparkline
    {
        /// <summary>
        /// Specifies the date range for the sparkline group
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// If a single-sheet-area is specified, that single-sheet-area must contain cells from either a single row or a single column
        /// </remarks>
        public IExternalRange DataRange { get; set; }

        /// <summary>
        /// Specifies the cell range in which the sparkline is located.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// It should specify exactly one cell
        /// </remarks>
        public IRange Location { get; set; }
    }
}

