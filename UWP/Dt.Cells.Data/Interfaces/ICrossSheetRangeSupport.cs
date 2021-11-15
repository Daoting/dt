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
    internal interface ICrossSheetRangeSupport
    {
        /// <summary>
        /// Copies a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="src">The source sheet.</param>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        void Copy(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Moves a range of cells to a range of cells at the specified location.
        /// </summary>
        /// <param name="src">The source sheet.</param>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        void Move(Worksheet src, int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
    }
}

