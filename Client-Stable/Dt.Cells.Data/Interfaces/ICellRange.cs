#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a CellRange object.
    /// </summary>
    public interface ICellRange : ICloneable
    {
        /// <summary>
        /// Gets the column.
        /// </summary>
        int Column { get; }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Gets the row.
        /// </summary>
        int Row { get; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        int RowCount { get; }
    }
}

