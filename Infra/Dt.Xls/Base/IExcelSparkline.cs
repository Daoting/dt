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
    /// Specifies information for a single sparkline.
    /// </summary>
    public interface IExcelSparkline
    {
        /// <summary>
        /// Specifies the date range for the sparkline group
        /// </summary>
        /// <remarks>
        /// If a single-sheet-area is specified, that single-sheet-area must contain cells from either a single row or a single column
        /// </remarks>
        IExternalRange DataRange { get; set; }

        /// <summary>
        /// Specifies the cell range in which the sparkline is located.
        /// </summary>
        /// <remarks>
        /// It should specify exactly one cell
        /// </remarks>
        IRange Location { get; set; }
    }
}

