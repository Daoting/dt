#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a cells enumerator for any area of the sheet.
    /// </summary>
    public class CellsEnumerator : IEnumerator<Cell>, IEnumerator, IDisposable
    {
        /// <summary>
        /// the column end index.
        /// </summary>
        int actualEndColumn;
        /// <summary>
        /// the row end index.
        /// </summary>
        int actualEndRow;
        /// <summary>
        /// data model
        /// </summary>
        StorageBlock block;
        /// <summary>
        /// the column end index.
        /// </summary>
        int columnEnd;
        /// <summary>
        /// the column start index.
        /// </summary>
        int columnStart;
        /// <summary>
        /// the high priority values
        /// </summary>
        int currentColumn;
        /// <summary>
        /// the low priority values.
        /// </summary>
        int currentRow;
        /// <summary>
        /// Indicate  whether actual end column is set 
        /// </summary>
        bool isActualEndColumnSet;
        /// <summary>
        /// Indicate  whether actual end row is set 
        /// </summary>
        bool isActualEndRowSet;
        /// <summary>
        /// the option for enumerator
        /// </summary>
        EnumeratorOption options;
        /// <summary>
        /// the row end index.
        /// </summary>
        int rowEnd;
        /// <summary>
        /// the row start index.
        /// </summary>
        int rowStart;
        /// <summary>
        /// the search order.
        /// </summary>
        Dt.Cells.Data.SearchOrder searchOrder;
        /// <summary>
        /// the sheet area.
        /// </summary>
        Dt.Cells.Data.SheetArea sheetArea;
        /// <summary>
        /// the sheet.
        /// </summary>
        Worksheet worksheet;

        /// <summary>
        /// Creates a new cells enumerator.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        public CellsEnumerator(Worksheet worksheet) : this(worksheet, Dt.Cells.Data.SheetArea.Cells)
        {
        }

        /// <summary>
        /// Creates a new cells enumerator for the specified sheet area.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="sheetArea">The sheet area.</param>
        public CellsEnumerator(Worksheet worksheet, Dt.Cells.Data.SheetArea sheetArea) : this(worksheet, sheetArea, Dt.Cells.Data.SearchOrder.ZOrder)
        {
        }

        /// <summary>
        /// Creates a new cells enumerator for the specified sheet area with the specified search order.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="sheetArea">The sheet area.</param>
        /// <param name="searchOrder">The search order for this enumerator.</param>
        public CellsEnumerator(Worksheet worksheet, Dt.Cells.Data.SheetArea sheetArea, Dt.Cells.Data.SearchOrder searchOrder) : this(worksheet, sheetArea, searchOrder, -1, -1, -1, -1)
        {
        }

        /// <summary>
        /// Creates a new cells enumerator for the specified sheet area with the specified search order for the specified search range.
        /// </summary>
        /// <param name="worksheet">The sheet.</param>
        /// <param name="sheetArea">The sheet area.</param>
        /// <param name="searchOrder">The search order for this enumerator.</param>
        /// <param name="rowStart">The starting row to enumerate.</param>
        /// <param name="columnStart">The starting column to enumerate.</param>
        /// <param name="rowEnd">The ending row to enumerate.</param>
        /// <param name="columnEnd">The ending column to enumerate.</param>
        public CellsEnumerator(Worksheet worksheet, Dt.Cells.Data.SheetArea sheetArea, Dt.Cells.Data.SearchOrder searchOrder, int rowStart, int columnStart, int rowEnd, int columnEnd)
        {
            this.sheetArea = Dt.Cells.Data.SheetArea.Cells;
            this.rowStart = -1;
            this.rowEnd = -1;
            this.actualEndRow = -1;
            this.columnStart = -1;
            this.columnEnd = -1;
            this.actualEndColumn = -1;
            this.currentColumn = -1;
            this.currentRow = -1;
            this.options = EnumeratorOption.HasStyle | EnumeratorOption.HasValue;
            if (worksheet == null)
            {
                throw new ArgumentOutOfRangeException("sheet");
            }
            this.options = EnumeratorOption.HasStyle | EnumeratorOption.HasValue;
            this.worksheet = worksheet;
            this.sheetArea = sheetArea;
            this.searchOrder = searchOrder;
            this.rowStart = rowStart;
            this.columnStart = columnStart;
            this.rowEnd = rowEnd;
            this.columnEnd = columnEnd;
            this.Init();
            this.block = this.worksheet.GetStorage(this.sheetArea);
        }

        void Before(int r1, int c1, int r2, int c2, out int row, out int column)
        {
            row = -1;
            column = -1;
            if (this.SearchOrder == Dt.Cells.Data.SearchOrder.ZOrder)
            {
                if (r1 < r2)
                {
                    row = r1;
                    column = c1;
                }
                else if (r1 == r2)
                {
                    if (c1 < c2)
                    {
                        row = r1;
                        column = c1;
                    }
                    else
                    {
                        row = r2;
                        column = c2;
                    }
                }
                else
                {
                    row = r2;
                    column = c2;
                }
            }
            else if (this.SearchOrder == Dt.Cells.Data.SearchOrder.NOrder)
            {
                if (c1 < c2)
                {
                    row = r1;
                    column = c1;
                }
                else if (c1 == c2)
                {
                    if (r1 < r2)
                    {
                        row = r1;
                        column = c1;
                    }
                    else
                    {
                        row = r2;
                        column = c2;
                    }
                }
                else
                {
                    row = r2;
                    column = c2;
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Reset();
        }

        int GetActualEndColumnZOrder(int row)
        {
            if ((row < this.RowStart) || (row > this.RowEnd))
            {
                return -1;
            }
            int num = -1;
            bool flag = false;
            if ((((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All) || ((this.options & EnumeratorOption.HasStyle) > EnumeratorOption.All)) && (this.block != null))
            {
                num = Math.Max(num, this.block.GetLastDirtyColumn(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data));
                flag = true;
            }
            if ((row == this.RowEnd) || this.IsBlockRange)
            {
                return (flag ? Math.Min(num, this.ColumnEnd) : this.ColumnEnd);
            }
            return (flag ? Math.Min(num, this.worksheet.GetColumnCount(this.sheetArea) - 1) : (this.worksheet.GetColumnCount(this.sheetArea) - 1));
        }

        int GetActualEndRowZOrder(int column)
        {
            if ((column < this.ColumnStart) || (column > this.ColumnEnd))
            {
                return -1;
            }
            int num = -1;
            bool flag = false;
            if ((((this.options & EnumeratorOption.HasStyle) > EnumeratorOption.All) || ((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All)) && (this.block != null))
            {
                num = Math.Max(num, this.block.GetLastDirtyRow(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data));
                flag = true;
            }
            if ((column == this.ColumnEnd) || this.IsBlockRange)
            {
                return (flag ? Math.Min(num, this.RowEnd) : this.RowEnd);
            }
            return (flag ? Math.Min(num, this.worksheet.GetRowCount(this.sheetArea) - 1) : (this.worksheet.GetRowCount(this.sheetArea) - 1));
        }

        int GetNextNonEmptyColumnInRow(StorageBlock model, int row, int column)
        {
            int num = (this.SearchOrder == Dt.Cells.Data.SearchOrder.ZOrder) ? this.GetActualEndColumnZOrder(row) : this.ActualEndColumn;
            for (int i = column; i <= num; i++)
            {
                if (((model.GetValue(row, i) != null) || (model.GetTag(row, i) != null)) || (model.GetStyle(row, i) != null))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Init this instance.
        /// </summary>
        void Init()
        {
            this.currentRow = -1;
            this.currentColumn = -1;
        }

        bool IsIndexAcceptable(int row, int column)
        {
            return (((((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All) && (this.block != null)) && ((this.block.GetValue(row, column) != null) || (this.block.GetTag(row, column) != null))) || (((((this.options & EnumeratorOption.HasStyle) > EnumeratorOption.All) && (this.block != null)) && (this.block.GetStyle(row, column) != null)) || (this.options == EnumeratorOption.All)));
        }

        bool IsNOrderOver(int row, int column)
        {
            if (this.IsBlockRange)
            {
                return ((((row >= this.RowStart) && (row <= this.ActualEndRow)) && (column >= this.ColumnStart)) && (column <= this.ActualEndColumn));
            }
            if (column > this.ActualEndColumn)
            {
                return false;
            }
            if ((column == this.ActualEndColumn) && ((row < 0) || (row > this.ActualEndRow)))
            {
                return false;
            }
            if (column < this.ColumnStart)
            {
                return false;
            }
            if ((column == this.ColumnStart) && (row < this.RowStart))
            {
                return false;
            }
            return true;
        }

        bool IsZOrderOver(int row, int column)
        {
            if (this.IsBlockRange)
            {
                return ((((row >= this.RowStart) && (row <= this.ActualEndRow)) && (column >= this.ColumnStart)) && (column <= this.ActualEndColumn));
            }
            if (row > this.ActualEndRow)
            {
                return false;
            }
            if ((row == this.ActualEndRow) && ((column < 0) || (column > this.ActualEndColumn)))
            {
                return false;
            }
            if (row < this.RowStart)
            {
                return false;
            }
            if ((row == this.RowStart) && (column < this.ColumnStart))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the enumerator successfully advanced to the next element; <c>false</c> if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public bool MoveNext()
        {
            if (((this.currentRow == -1) && (this.currentColumn == -1)) && ((this.RowStart <= this.RowEnd) && (this.ColumnStart <= this.ColumnEnd)))
            {
                this.currentRow = this.RowStart;
                this.currentColumn = this.ColumnStart;
                if (this.IsIndexAcceptable(this.currentRow, this.currentColumn) && !this.SkipCurrent())
                {
                    return true;
                }
            }
            while (this.TryMoveNext())
            {
                if (!this.SkipCurrent())
                {
                    return true;
                }
            }
            this.currentRow = -1;
            this.currentColumn = -1;
            return false;
        }

        bool Next(ref int row, ref int column)
        {
            if (this.SearchOrder == Dt.Cells.Data.SearchOrder.ZOrder)
            {
                int actualEndColumnZOrder = this.GetActualEndColumnZOrder(row);
                if ((column + 1) <= actualEndColumnZOrder)
                {
                    column++;
                    return this.IsZOrderOver(row, column);
                }
                if ((row + 1) > this.ActualEndRow)
                {
                    return false;
                }
                row++;
                if (this.IsBlockRange)
                {
                    column = this.ColumnStart;
                }
                else
                {
                    column = 0;
                }
                return this.IsZOrderOver(row, column);
            }
            if (this.SearchOrder != Dt.Cells.Data.SearchOrder.NOrder)
            {
                return false;
            }
            int actualEndRowZOrder = this.GetActualEndRowZOrder(column);
            if ((row + 1) <= actualEndRowZOrder)
            {
                row++;
                return this.IsNOrderOver(row, column);
            }
            if ((column + 1) > this.ActualEndColumn)
            {
                return false;
            }
            column++;
            if (this.IsBlockRange)
            {
                row = this.RowStart;
            }
            else
            {
                row = 0;
            }
            return this.IsNOrderOver(row, column);
        }

        bool NextNOrder(StorageBlock model, ref int row, ref int column)
        {
            if (model != null)
            {
                int num = this.GetNextNonEmptyColumnInRow(model, row, column + 1);
                if (num != -1)
                {
                    column = num;
                    if (this.IsNOrderOver(row, column))
                    {
                        return true;
                    }
                }
                do
                {
                    int num2 = model.NextNonEmptyRow(row);
                    if ((num2 == -1) || (num2 > this.RowEnd))
                    {
                        row = -1;
                    }
                    else
                    {
                        if (num2 != -1)
                        {
                            row = num2;
                        }
                        if ((num2 != -1) && (num2 < row))
                        {
                            row = num2;
                        }
                    }
                    if (row != -1)
                    {
                        if ((row == this.RowStart) || this.IsBlockRange)
                        {
                            column = this.ColumnStart - 1;
                        }
                        else
                        {
                            column = -1;
                        }
                        do
                        {
                            int num3 = this.GetNextNonEmptyColumnInRow(model, row, column + 1);
                            if ((num3 == -1) || (num3 > this.ColumnEnd))
                            {
                                column = -1;
                            }
                            else
                            {
                                if (num3 != -1)
                                {
                                    column = num3;
                                }
                                if ((num3 != -1) && (num3 < column))
                                {
                                    column = num3;
                                }
                            }
                            if (column != -1)
                            {
                                return this.IsZOrderOver(row, column);
                            }
                        }
                        while (column != -1);
                    }
                }
                while (row != -1);
            }
            return false;
        }

        bool NextStyle(ref int row, ref int column)
        {
            if ((this.block != null) && (this.SearchOrder == Dt.Cells.Data.SearchOrder.ZOrder))
            {
                return this.NextZOrder(this.block, ref row, ref column);
            }
            int num = row;
            int num2 = column;
            while (this.Next(ref num, ref num2))
            {
                if (this.IsIndexAcceptable(num, num2))
                {
                    row = num;
                    column = num2;
                    return true;
                }
            }
            return false;
        }

        bool NextValue(ref int row, ref int column)
        {
            if ((this.block != null) && (this.searchOrder == Dt.Cells.Data.SearchOrder.ZOrder))
            {
                return this.NextZOrder(this.block, ref row, ref column);
            }
            int num = row;
            int num2 = column;
            while (this.Next(ref num, ref num2))
            {
                if (this.IsIndexAcceptable(num, num2))
                {
                    row = num;
                    column = num2;
                    return true;
                }
            }
            return false;
        }

        bool NextZOrder(StorageBlock model, ref int row, ref int column)
        {
            if (model != null)
            {
                int num = this.GetNextNonEmptyColumnInRow(model, row, column + 1);
                if (num != -1)
                {
                    column = num;
                    if (this.IsZOrderOver(row, column))
                    {
                        return true;
                    }
                }
                do
                {
                    int num2 = model.NextNonEmptyRow(row);
                    if ((num2 == -1) || (num2 > this.RowEnd))
                    {
                        row = -1;
                    }
                    else
                    {
                        if (num2 != -1)
                        {
                            row = num2;
                        }
                        if ((num2 != -1) && (num2 < row))
                        {
                            row = num2;
                        }
                    }
                    if (row != -1)
                    {
                        if ((row == this.RowStart) || this.IsBlockRange)
                        {
                            column = this.ColumnStart - 1;
                        }
                        else
                        {
                            column = -1;
                        }
                        do
                        {
                            int num3 = this.GetNextNonEmptyColumnInRow(model, row, column + 1);
                            if ((num3 == -1) || ((num3 > this.ColumnEnd) && this.IsBlockRange))
                            {
                                column = -1;
                            }
                            else
                            {
                                if (num3 != -1)
                                {
                                    column = num3;
                                }
                                if ((num3 != -1) && (num3 < column))
                                {
                                    column = num3;
                                }
                            }
                            if (column != -1)
                            {
                                return this.IsZOrderOver(row, column);
                            }
                        }
                        while (column != -1);
                    }
                }
                while (row != -1);
            }
            return false;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">
        /// The collection was modified after the enumerator was created.
        /// </exception>
        public void Reset()
        {
            this.currentRow = -1;
            this.currentColumn = -1;
        }

        /// <summary>
        /// Determines whether this instance is empty.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool SkipCurrent()
        {
            return ((this.Skipper != null) && this.Skipper.Skip(this.worksheet, this.CurrentRowIndex, this.CurrentColumnIndex));
        }

        /// <summary>
        /// Tries the move next.
        /// </summary>
        /// <returns></returns>
        bool TryMoveNext()
        {
            int currentRow = this.currentRow;
            int currentColumn = this.currentColumn;
            bool flag = false;
            if ((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All)
            {
                int row = currentRow;
                int column = currentColumn;
                if (this.NextValue(ref row, ref column))
                {
                    currentRow = row;
                    currentColumn = column;
                    flag = true;
                }
            }
            int num5 = this.currentRow;
            int num6 = this.currentColumn;
            bool flag2 = false;
            if ((this.options & EnumeratorOption.HasStyle) > EnumeratorOption.All)
            {
                int num7 = num5;
                int num8 = num6;
                if (this.NextStyle(ref num7, ref num8))
                {
                    num5 = num7;
                    num6 = num8;
                    flag2 = true;
                }
            }
            switch (this.options)
            {
                case EnumeratorOption.All:
                {
                    int num9 = this.currentRow;
                    int num10 = this.currentColumn;
                    if (!this.Next(ref num9, ref num10))
                    {
                        this.currentRow = -1;
                        this.currentColumn = -1;
                        break;
                    }
                    this.currentRow = num9;
                    this.currentColumn = num10;
                    return true;
                }
                case EnumeratorOption.HasValue:
                    if (!flag)
                    {
                        this.currentRow = -1;
                        this.currentColumn = -1;
                        break;
                    }
                    this.currentRow = currentRow;
                    this.currentColumn = currentColumn;
                    break;

                case EnumeratorOption.HasStyle:
                    if (!flag2)
                    {
                        this.currentRow = -1;
                        this.currentColumn = -1;
                        break;
                    }
                    this.currentRow = num5;
                    this.currentColumn = num6;
                    break;

                case (EnumeratorOption.HasStyle | EnumeratorOption.HasValue):
                    if (!flag || !flag2)
                    {
                        if (flag)
                        {
                            this.currentRow = currentRow;
                            this.currentColumn = currentColumn;
                        }
                        else if (flag2)
                        {
                            this.currentRow = num5;
                            this.currentColumn = num6;
                        }
                        else
                        {
                            this.currentRow = -1;
                            this.currentColumn = -1;
                        }
                        break;
                    }
                    this.Before(currentRow, currentColumn, num5, num6, out this.currentRow, out this.currentColumn);
                    break;
            }
            if ((this.currentRow == -1) && (this.currentColumn == -1))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the actual end column.
        /// </summary>
        /// <value>The actual end column.</value>
        int ActualEndColumn
        {
            get
            {
                if (!this.isActualEndColumnSet)
                {
                    int num = -1;
                    bool flag = false;
                    if (((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All) && (this.block != null))
                    {
                        int lastDirtyColumn = this.block.GetLastDirtyColumn(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data);
                        num = Math.Max(num, lastDirtyColumn);
                        flag = true;
                    }
                    num = flag ? Math.Min(num, this.ColumnEnd) : this.ColumnEnd;
                    this.actualEndColumn = num;
                    this.isActualEndColumnSet = true;
                }
                return this.actualEndColumn;
            }
        }

        /// <summary>
        /// Gets the actual end row.
        /// </summary>
        /// <value>The actual end row.</value>
        int ActualEndRow
        {
            get
            {
                if (!this.isActualEndRowSet)
                {
                    int num = -1;
                    bool flag = false;
                    if (((this.options & EnumeratorOption.HasValue) > EnumeratorOption.All) && (this.block != null))
                    {
                        int lastDirtyRow = this.block.GetLastDirtyRow(StorageType.Axis | StorageType.Sparkline | StorageType.Tag | StorageType.Style | StorageType.Data);
                        num = Math.Max(num, lastDirtyRow);
                        flag = true;
                    }
                    num = flag ? Math.Min(num, this.RowEnd) : this.RowEnd;
                    this.actualEndRow = num;
                    this.isActualEndRowSet = true;
                }
                return this.actualEndRow;
            }
        }

        /// <summary>
        /// Gets the ending column to enumerate.
        /// </summary>
        /// <value>The ending column index.</value>
        [DefaultValue(-1)]
        public int ColumnEnd
        {
            get
            {
                if (this.columnEnd == -1)
                {
                    return (this.worksheet.GetColumnCount(this.sheetArea) - 1);
                }
                return this.columnEnd;
            }
        }

        /// <summary>
        /// Gets the starting column to enumerate.
        /// </summary>
        /// <value>The starting column index.</value>
        [DefaultValue(-1)]
        public int ColumnStart
        {
            get
            {
                if (this.columnStart == -1)
                {
                    return 0;
                }
                return this.columnStart;
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The element in the collection at the current position of the enumerator.</value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public Cell Current
        {
            get { return  (this.Current as Cell); }
        }

        /// <summary>
        /// Gets the index of the current column.
        /// </summary>
        /// <value>The index of the current column.</value>
        [DefaultValue(-1)]
        public int CurrentColumnIndex
        {
            get { return  this.currentColumn; }
        }

        /// <summary>
        /// Gets the index of the current row.
        /// </summary>
        /// <value>The index of the current row.</value>
        [DefaultValue(-1)]
        public int CurrentRowIndex
        {
            get { return  this.currentRow; }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether this instance is a block range.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is a block range; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool IsBlockRange { get; set; }

        /// <summary>
        /// Gets or sets the options that indicate whether to enumerate the cells with value or style.
        /// </summary>
        /// <value>The <see cref="T:Dt.Cells.Data.EnumeratorOption" /> enumeration.</value>
        [DefaultValue(3)]
        public EnumeratorOption Options
        {
            get { return  this.options; }
            set { this.options = value; }
        }

        /// <summary>
        /// Gets the ending row to enumerate.
        /// </summary>
        /// <value>The ending row index.</value>
        [DefaultValue(-1)]
        public int RowEnd
        {
            get
            {
                if (this.rowEnd == -1)
                {
                    return (this.worksheet.GetRowCount(this.sheetArea) - 1);
                }
                return this.rowEnd;
            }
        }

        /// <summary>
        /// Gets the starting row to enumerate.
        /// </summary>
        /// <value>The starting row index.</value>
        [DefaultValue(-1)]
        public int RowStart
        {
            get
            {
                if (this.rowStart == -1)
                {
                    return 0;
                }
                return this.rowStart;
            }
        }

        /// <summary>
        /// Gets the search direction.
        /// </summary>
        /// <value>The <see cref="P:Dt.Cells.Data.CellsEnumerator.SearchOrder" /> enumeration that indicates the search order.</value>
        [DefaultValue(0)]
        public Dt.Cells.Data.SearchOrder SearchOrder
        {
            get { return  this.searchOrder; }
        }

        /// <summary>
        /// Gets the sheet for the shape.
        /// </summary>
        /// <value>The sheet for the shape.</value>
        public Worksheet Sheet
        {
            get { return  this.worksheet; }
        }

        /// <summary>
        /// Gets the sheet area.
        /// </summary>
        /// <value>The sheet area.</value>
        [DefaultValue(1)]
        public Dt.Cells.Data.SheetArea SheetArea
        {
            get { return  this.sheetArea; }
        }

        /// <summary>
        /// Gets or sets the skipped cell.
        /// </summary>
        /// <value>The skip value.</value>
        public CellSkipper Skipper { get; set; }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        object IEnumerator.Current
        {
            get
            {
                if (((0 <= this.CurrentRowIndex) && (this.CurrentRowIndex < this.Sheet.GetRowCount(this.sheetArea))) && ((0 <= this.CurrentColumnIndex) && (this.CurrentColumnIndex < this.Sheet.GetColumnCount(this.sheetArea))))
                {
                    ICellsSupport support = this.Sheet.FindCellsArea(this.sheetArea);
                    if (support != null)
                    {
                        return support.Cells[this.CurrentRowIndex, this.CurrentColumnIndex];
                    }
                }
                return null;
            }
        }
    }
}

