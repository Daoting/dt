#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名

using System;
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a range of Cell objects.
    /// </summary>
    public sealed class Cells
    {
        /// <summary>
        /// Indicates which area contains this cell.
        /// </summary>
        Dt.Cells.Data.SheetArea sheetArea;
        /// <summary>
        /// sheet view containing Cell
        /// </summary>
        Dt.Cells.Data.Worksheet worksheet;

        /// <summary>
        /// Creates a collection of cells.
        /// </summary>
        /// <param name="worksheet">The sheet</param>
        /// <param name="sheetArea">A <see cref="P:Dt.Cells.Data.Cells.SheetArea" /> enumeration that specifies the sheet area type.</param>
        internal Cells(Dt.Cells.Data.Worksheet worksheet, Dt.Cells.Data.SheetArea sheetArea)
        {
            this.worksheet = worksheet;
            this.sheetArea = sheetArea;
        }

        /// <summary>
        /// Checks the index bounds.
        /// </summary>
        /// <param name="column">Column index</param>
        /// <param name="sheetArea">The sheet area</param>
        void CheckColumnIndexBounds(int column, Dt.Cells.Data.SheetArea sheetArea)
        {
            int columnCount = this.worksheet.GetColumnCount(sheetArea);
            if ((column < -1) || (column >= columnCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvalidColumnIndexWithAllowedRangeBehind, column, columnCount - 1));
            }
        }

        /// <summary>
        /// Checks the index bounds.
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="sheetArea">The sheet area</param>
        void CheckRowIndexBounds(int row, Dt.Cells.Data.SheetArea sheetArea)
        {
            int rowCount = this.worksheet.GetRowCount(sheetArea);
            if ((row < -1) || (row >= rowCount))
            {
                throw new IndexOutOfRangeException(string.Format(ResourceStrings.InvaildRowIndexWithAllowedRangeBehind, row, rowCount - 1));
            }
        }

        /// <summary>
        /// Gets a new cell with a specified tag, or returns null if there is no cell with the specified tag.
        /// </summary>
        /// <param name="tag">A specified tag that can be used to find a cell.</param>
        /// <value>A <see cref="T:Dt.Cells.Data.Cell" /> object.</value>
        public Cell this[string tag]
        {
            get
            {
                if ((this.worksheet != null) && (tag != null))
                {
                    CellRange range;
                    int rowCount = this.worksheet.RowCount;
                    int columnCount = this.worksheet.ColumnCount;
                    if ((this.sheetArea == Dt.Cells.Data.SheetArea.Cells) && CellRange.TryParse(tag, out range))
                    {
                        return new Cell(this.worksheet, range.Row, range.Column, (range.Row + range.RowCount) - 1, (range.Column + range.ColumnCount) - 1, this.sheetArea);
                    }
                    for (int i = 0; i < rowCount; i++)
                    {
                        for (int j = 0; j < columnCount; j++)
                        {
                            if (tag.Equals(this.worksheet.GetTag(i, j, this.sheetArea)))
                            {
                                return new Cell(this.worksheet, i, j, this.sheetArea);
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a new cell for the specified row and column.
        /// </summary>
        /// <value>A <see cref="T:Dt.Cells.Data.Cell" /> object.</value>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified row index is not valid; must be between zero and the total number of rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified column index is not valid; must be between zero and the total number of columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified column header row index is not valid; must be between zero and the total number of column header rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified row header column index is not valid; must be between zero and the total number of row header columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified sheet corner row index is not valid; must be between zero and the total number of sheet corner rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified sheet corner column index is not valid; must be between zero and the total number of sheet corner columns.
        /// </exception>
        public Cell this[int row, int column]
        {
            get
            {
                this.CheckRowIndexBounds(row, this.sheetArea);
                this.CheckColumnIndexBounds(column, this.sheetArea);
                return new Cell(this.worksheet, row, column, this.sheetArea);
            }
        }

        /// <summary>
        /// Gets a new cell for the range of cells with the specified rows and columns.
        /// </summary>
        /// <param name="row">The starting row index.</param>
        /// <param name="column">The starting column index.</param>
        /// <param name="row2">The ending row index.</param>
        /// <param name="column2">The ending column index.</param>
        /// <value>A <see cref="T:Dt.Cells.Data.Cell" /> object.</value>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting row index is not valid; must be between zero and the total number of rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting column index is not valid; must be between zero and the total number of columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting column header row index is not valid; must be between zero and the total number of column header rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting row header column index is not valid; must be between zero and the total number of row header columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting sheet corner row index is not valid; must be between zero and the total number of sheet corner rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified starting sheet corner column index is not valid; must be between zero and the total number of sheet corner columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending row index is not valid; must be between zero and the total number of rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending column index is not valid; must be between zero and the total number of columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending column header row index is not valid; must be between zero and the total number of column header rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending row header column index is not valid; must be between zero and the total number of row header columns.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending sheet corner row index is not valid; must be between zero and the total number of sheet corner rows.
        /// </exception>
        /// <exception cref="T:System.IndexOutOfRangeException">
        /// Specified ending sheet corner column index is not valid; must be between zero and the total number of sheet corner columns.
        /// </exception>
        public Cell this[int row, int column, int row2, int column2]
        {
            get
            {
                this.CheckRowIndexBounds(row, this.sheetArea);
                this.CheckColumnIndexBounds(column, this.sheetArea);
                this.CheckRowIndexBounds(row2, this.sheetArea);
                this.CheckColumnIndexBounds(column2, this.sheetArea);
                return new Cell(this.worksheet, row, column, row2, column2, this.sheetArea);
            }
        }

        /// <summary>
        /// Gets the parent object containing this collection of cells.
        /// </summary>
        /// <value>The parent object that contains the collection of cells.</value>
        internal object Parent
        {
            get
            {
                if (this.worksheet != null)
                {
                    switch (this.sheetArea)
                    {
                        case Dt.Cells.Data.SheetArea.Cells:
                            return this.worksheet;

                        case (Dt.Cells.Data.SheetArea.CornerHeader | Dt.Cells.Data.SheetArea.RowHeader):
                            return this.worksheet.RowHeader;

                        case Dt.Cells.Data.SheetArea.ColumnHeader:
                            return this.worksheet.ColumnHeader;
                    }
                }
                return null;
            }
        }

        internal Dt.Cells.Data.SheetArea SheetArea
        {
            get { return  this.sheetArea; }
        }

        internal Dt.Cells.Data.Worksheet Worksheet
        {
            get { return  this.worksheet; }
        }
    }
}

