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
    /// Represents a table for table supporting.
    /// </summary>
    internal interface ITableSheet
    {
        /// <summary>
        /// Gets the actual column width of the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>       
        /// <returns>The width of the specified column.</returns>
        double GetActualColumnWidth(int column);
        /// <summary>
        /// Gets the actual row height of the specified row.
        /// </summary>
        /// <param name="row">The row index.</param>       
        /// <returns>The height of the specified row.</returns>
        double GetActualRowHeight(int row);
        /// <summary>
        /// Gets a formula from the specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The formula of the specified cell in the sheet.</returns>
        string GetCellFormula(int row, int column);
        /// <summary>
        /// Gets the value from the specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>The value of the specified cell in the sheet.</returns>
        string GetCellText(int row, int column);
        /// <summary>
        /// Moves values.
        /// </summary>
        /// <param name="fromRow">The from row index.</param>
        /// <param name="fromColumn">The from column index.</param>
        /// <param name="toRow">The to row index.</param>
        /// <param name="toColumn">The to column index.</param>
        /// <param name="rowCount">The row count.</param>
        /// <param name="columnCount">The column count.</param>
        void Move(int fromRow, int fromColumn, int toRow, int toColumn, int rowCount, int columnCount);
        /// <summary>
        /// Sets a formula in a specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="formula">The formula to place in the specified cell.</param>
        void SetCellFormula(int row, int column, string formula);
        /// <summary>
        /// Sets a value to a specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="value">The value.</param>
        void SetCellText(int row, int column, string value);
        /// <summary>
        /// Sets a value to a specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <param name="value">The value.</param>
        void SetValue(int row, int column, object value);

        /// <summary>
        /// Gets the column count of the sheet.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Gets the row count of the sheet.
        /// </summary>
        int RowCount { get; }
    }
}

