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
    /// Defines properties used to represents a range.
    /// </summary>
    public interface IRange
    {
        /// <summary>
        /// Gets the zero-based index of the start column of the range.
        /// </summary>
        /// <value>The start column index.</value>
        int Column { get; }

        /// <summary>
        /// Gets the column span of the range.
        /// </summary>
        /// <value>The column span.</value>
        int ColumnSpan { get; }

        /// <summary>
        /// Gets the zero-based index of the start row of the range.
        /// </summary>
        /// <value>The start row index.</value>
        int Row { get; }

        /// <summary>
        /// Gets the row span of the range.
        /// </summary>
        /// <value>The row span.</value>
        int RowSpan { get; }
    }
}

