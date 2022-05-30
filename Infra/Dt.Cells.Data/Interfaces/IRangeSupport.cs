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
    /// Interface that supports operations on ranges of cells 
    /// in the data model.
    /// </summary>
    internal interface IRangeSupport
    {
        /// <summary>
        /// Adds columns of cells after the specified column.
        /// </summary>
        /// <param name="column">The column index of the column after which to add columns.</param>
        /// <param name="count">The number of columns to add.</param>
        void AddColumns(int column, int count);
        /// <summary>
        /// Adds rows of cells after the specified row.
        /// </summary>
        /// <param name="row">The row index of the row after which to add rows.</param>
        /// <param name="count">The number of rows to add.</param>
        void AddRows(int row, int count);
        /// <summary>
        /// Removes all of the contents from the cells in the specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first row in the selected range.</param>
        /// <param name="column">The column index of the first column in the selected range.</param>
        /// <param name="rowCount">The number of rows in the selected range.</param>
        /// <param name="columnCount">The number of columns in the selected range.</param>
        void Clear(int row, int column, int rowCount, int columnCount);
        /// <summary>
        /// Copies a range of cells and pastes it into a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin copying.</param>
        /// <param name="fromColumn">The column index from which to begin copying.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to copy.</param>
        /// <param name="columnCount">The number of columns to copy.</param>
        void Copy(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Moves a range of cells to a range of cells at the specified location.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the move.</param>
        /// <param name="fromColumn">The column index from which to begin the move.</param>
        /// <param name="toRow">The row index at which to paste the cell range.</param>
        /// <param name="toColumn">The column index at which to paste the cell range.</param>
        /// <param name="rowCount">The number of rows to move.</param>
        /// <param name="columnCount">The number of columns to move.</param>
        void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Removes columns from the specified starting position.
        /// </summary>
        /// <param name="column">The column index at which to start removing columns.</param>
        /// <param name="count">The number of columns to remove.</param>
        void RemoveColumns(int column, int count);
        /// <summary>
        /// Removes rows from the specified starting position.
        /// </summary>
        /// <param name="row">The row index at which to start removing rows.</param>
        /// <param name="count">The number of rows to remove.</param>
        void RemoveRows(int row, int count);
        /// <summary>
        /// Swaps a range of cells from one location to another.
        /// </summary>
        /// <param name="fromRow">The row index from which to begin the swap.</param>
        /// <param name="fromColumn">The column index from which to begin the swap.</param>
        /// <param name="toRow">The row index of the destination range.</param>
        /// <param name="toColumn">The column index of the destination range.</param>
        /// <param name="rowCount">The number of rows to swap.</param>
        /// <param name="columnCount">The number of columns to swap.</param>
        void Swap(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
    }
}

