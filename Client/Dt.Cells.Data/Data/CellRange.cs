#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.Data
{
    /// <summary>
    /// Represents a selected range of cells for a sheet. 
    /// </summary>
    public class CellRange : ICellRange, ICloneable
    {
        /// <summary>
        /// column index of this.sheet.
        /// </summary>
        int column;
        /// <summary>
        /// column count of this.sheet.
        /// </summary>
        int columnCount;
        /// <summary>
        /// row index of this.sheet.
        /// </summary>
        int row;
        /// <summary>
        /// row count of this.sheet.
        /// </summary>
        int rowCount;

        /// <summary>
        /// Creates a range of cells.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        public CellRange(int row, int column, int rowCount, int columnCount)
        {
            if (row == -1)
            {
                rowCount = -1;
            }
            if (column == -1)
            {
                columnCount = -1;
            }
            this.row = row;
            this.column = column;
            this.rowCount = rowCount;
            this.columnCount = columnCount;
        }

        /// <summary>
        /// Clones a cell range of the current cell range.
        /// </summary>
        /// <returns></returns>
        public virtual object Clone()
        {
            return new CellRange(this.Row, this.Column, this.RowCount, this.ColumnCount);
        }

        /// <summary>
        /// Determines whether the range of cells contains a specified range of cells.
        /// </summary>
        /// <param name="range">The CellRange object that contains the other range of cells.</param>
        /// <returns>
        /// <c>true</c> if the cell range contains the specified cell range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(CellRange range)
        {
            return this.Contains(range.Row, range.Column, range.RowCount, range.ColumnCount);
        }

        /// <summary>
        /// Determines whether the range of cells contains the specified cell.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="column">The column index.</param>
        /// <returns>
        /// <c>true</c> if the cell range contains the specified row and column; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int row, int column)
        {
            return this.Contains(row, column, 1, 1);
        }

        /// <summary>
        /// Determines whether the range of cells contains a specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        /// <returns>
        /// <c>true</c> if the cell range contains the specified cell range; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(int row, int column, int rowCount, int columnCount)
        {
            if ((this.row != -1) && ((this.row > row) || ((row + rowCount) > (this.row + this.rowCount))))
            {
                return false;
            }
            return ((this.column == -1) || ((this.column <= column) && ((column + columnCount) <= (this.column + this.columnCount))));
        }

        /// <summary>
        /// Determines whether the range of cells is the same as another specified range of cells.
        /// </summary>
        /// <param name="item">The object that contains the range of cells to compare.</param>
        /// <returns><c>true</c> if the ranges are the same; otherwise, <c>false</c>.</returns>
        public override bool Equals(object item)
        {
            if (!(item is CellRange))
            {
                return false;
            }
            CellRange range = (CellRange) item;
            return ((((this.row == range.row) && (this.column == range.column)) && (this.rowCount == range.rowCount)) && (this.columnCount == range.columnCount));
        }

        /// <summary>
        /// Determines whether the range of cells is the same as another specified range of cells.
        /// </summary>
        /// <param name="row">The row index of the first cell in the range.</param>
        /// <param name="column">The column index of the first cell in the range.</param>
        /// <param name="rowCount">The number of rows in the range.</param>
        /// <param name="columnCount">The number of columns in the range.</param>
        /// <returns><c>true</c> if the ranges are the same; otherwise, <c>false</c>.</returns>
        public bool Equals(int row, int column, int rowCount, int columnCount)
        {
            return ((((this.row == row) && (this.column == column)) && (this.rowCount == rowCount)) && (this.columnCount == columnCount));
        }

        /// <summary>
        /// Gets the fixed cell range.
        /// </summary>
        /// <param name="source">The source</param>
        /// <param name="maxRowCount">The maximum row count</param>
        /// <param name="maxColumnCount">The maximum column count</param>
        /// <returns>Returns the cell range.</returns>
        internal static CellRange FixedCellRange(CellRange source, int maxRowCount, int maxColumnCount)
        {
            CellRange range = null;
            if ((source.row != -1) && (source.column != -1))
            {
                return new CellRange(source.row, source.column, source.rowCount, source.columnCount);
            }
            if ((source.row == -1) && (source.column != -1))
            {
                return new CellRange(0, source.column, maxRowCount, source.columnCount);
            }
            if ((source.row != -1) && (source.column == -1))
            {
                return new CellRange(source.row, 0, source.rowCount, maxColumnCount);
            }
            if ((source.row == -1) && (source.column == -1))
            {
                range = new CellRange(0, 0, maxRowCount, maxColumnCount);
            }
            return range;
        }

        /// <summary>
        /// Gets the hash code of this object.
        /// </summary>
        /// <returns>
        /// Returns a hash code for the current <see cref="T:System.Object" />.
        /// </returns>
        public override int GetHashCode()
        {
            return (((this.row ^ this.column) ^ this.rowCount) ^ this.columnCount);
        }

        /// <summary>
        /// Gets the intersection of two cell ranges.
        /// </summary>
        /// <param name="cellRange1">The first cell range.</param>
        /// <param name="cellRange2">The second cell range.</param>
        /// <param name="maxRowCount">The maximum row count.</param>
        /// <param name="maxColumnCount">The maximum column count.</param>
        /// <returns>Returns null if there is no intersection, or the cell range of the intersection.</returns>
        public static CellRange GetIntersect(CellRange cellRange1, CellRange cellRange2, int maxRowCount, int maxColumnCount)
        {
            if ((cellRange1 == null) || (cellRange2 == null))
            {
                return null;
            }
            if (!cellRange1.Intersects(cellRange2.row, cellRange2.column, cellRange2.rowCount, cellRange2.columnCount))
            {
                return null;
            }
            int num = (cellRange1.Column == -1) ? maxColumnCount : ((cellRange1.Column + cellRange1.ColumnCount) - 1);
            int num2 = (cellRange2.Column == -1) ? maxColumnCount : ((cellRange2.Column + cellRange2.ColumnCount) - 1);
            int num3 = (cellRange1.Row == -1) ? maxRowCount : ((cellRange1.Row + cellRange1.RowCount) - 1);
            int num4 = (cellRange2.Row == -1) ? maxRowCount : ((cellRange2.Row + cellRange2.RowCount) - 1);
            int column = Math.Max(cellRange1.Column, cellRange2.Column);
            int num6 = Math.Min(num, num2);
            int row = Math.Max(cellRange1.Row, cellRange2.Row);
            int num8 = Math.Min(num3, num4);
            int columnCount = (num6 - column) + 1;
            return new CellRange(row, column, (num8 - row) + 1, columnCount);
        }

        /// <summary>
        /// Determines whether the range of cells intersects the specified column.
        /// </summary>
        /// <param name="column">The column index.</param>
        /// <returns><c>true</c> if the range intersects the column; otherwise, <c>false</c>.</returns>
        public bool IntersectColumn(int column)
        {
            return this.Intersects(-1, column, -1, 1);
        }

        /// <summary>
        /// Determines whether the range of cells intersects the specified row.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <returns><c>true</c> if the range intersects the row; otherwise, <c>false</c>.</returns>
        public bool IntersectRow(int row)
        {
            return this.Intersects(row, -1, 1, -1);
        }

        /// <summary>
        /// Determines whether the range of cells intersects the specified range.
        /// </summary>
        /// <param name="row">The row index at the start of the range.</param>
        /// <param name="column">The column index at the start of the range.</param>
        /// <param name="rowCount">The number of rows.</param>
        /// <param name="columnCount">The number of columns.</param>
        /// <returns><c>true</c> if the range intersects the specified range; otherwise, <c>false</c>.</returns>
        public bool Intersects(int row, int column, int rowCount, int columnCount)
        {
            if (((row != -1) && (this.row != -1)) && ((this.row >= (row + rowCount)) || (row >= (this.row + this.rowCount))))
            {
                return false;
            }
            return (((column == -1) || (this.column == -1)) || ((this.column < (column + columnCount)) && (column < (this.column + this.columnCount))));
        }

        /// <summary>
        /// Determines whether this cell range is valid in the specified worksheet.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</param>
        /// <returns>
        /// <c>true</c> if the cell range is a valid range in the specified worksheet; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidRange(Worksheet worksheet)
        {
            return this.IsValidRange(worksheet, false);
        }

        /// <summary>
        /// Determines whether this cell range is valid in the specified worksheet and
        /// whether to treat it as a span.
        /// </summary>
        /// <param name="worksheet">The <see cref="T:Dt.Cells.Data.Worksheet" /> object.</param>
        /// <param name="isSpan">Whether to treat the cell range as a span or a selection.</param>
        /// <returns>
        /// <c>true</c> if the cell range is a valid range in the specified sheet; otherwise, <c>false</c>.
        /// </returns>
        public bool IsValidRange(Worksheet worksheet, bool isSpan)
        {
            bool flag = false;
            if (worksheet == null)
            {
                return flag;
            }
            int rowCount = worksheet.RowCount;
            int columnCount = worksheet.ColumnCount;
            if (isSpan)
            {
                if (((0 <= this.row) && (this.row < rowCount)) && ((0 <= this.column) && (this.column < columnCount)))
                {
                    flag = true;
                }
                return flag;
            }
            if ((((-1 > this.row) || (this.row >= rowCount)) || ((-1 > this.column) || (this.column >= columnCount))) || ((this.rowCount != -1) && ((this.column + this.columnCount) > columnCount)))
            {
                return flag;
            }
            if ((this.columnCount != -1) && ((this.row + this.rowCount) > rowCount))
            {
                return flag;
            }
            return true;
        }

        /// <summary>
        /// Offsets the specified cell range along the specified axes.
        /// </summary>
        /// <param name="cellRange">The cell range.</param>
        /// <param name="x">The <i>x</i> axis offset.</param>
        /// <param name="y">The <i>y</i> axis offset.</param>
        /// <returns>Returns the cell range after the offset.</returns>
        public static CellRange Offset(CellRange cellRange, int x, int y)
        {
            if (cellRange == null)
            {
                return null;
            }
            int column = cellRange.Column;
            int row = cellRange.Row;
            if (cellRange.Column != -1)
            {
                column += x;
            }
            if (cellRange.Row != -1)
            {
                row += y;
            }
            return new CellRange(row, column, cellRange.rowCount, cellRange.columnCount);
        }

        /// <summary>
        /// Converts a string to a cell range.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <returns>Returns the cell range that was parsed from the string.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="s" /> is a null reference (Nothing in Visual Basic).</exception>
        /// <exception cref="T:System.FormatException"><paramref name="s" /> does not represent a cell range.</exception>
        public static CellRange Parse(string s)
        {
            CellRange range;
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            if (!TryParse(s, out range))
            {
                throw new FormatException();
            }
            return range;
        }

        /// <summary>
        /// Removes the specified cell range and splits the cell range to some small cell ranges.
        /// </summary>
        /// <param name="cellRanges">The cell ranges</param>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <param name="rowCount">The row count</param>
        /// <param name="columnCount">The column count</param>
        /// <param name="maxRowCount">The maximum row count</param>
        /// <param name="maxColumnCount">The maximum column count</param>
        /// <returns>Returns a new cell ranges array.</returns>
        internal static CellRange[] Remove(CellRange[] cellRanges, int row, int column, int rowCount, int columnCount, int maxRowCount, int maxColumnCount)
        {
            List<CellRange> list = new List<CellRange>();
            foreach (CellRange range in cellRanges)
            {
                CellRange[] rangeArray = Remove(range, row, column, rowCount, columnCount, maxRowCount, maxColumnCount);
                if (rangeArray != null)
                {
                    list.AddRange(rangeArray);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// Removes the specified cell range and splits the cell range to some small cell ranges.
        /// </summary>
        /// <param name="cellRange">The cell range</param>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <param name="rowCount">The row count</param>
        /// <param name="columnCount">The column count</param>
        /// <param name="maxRowCount">The maximum row count</param>
        /// <param name="maxColumnCount">The maximum column count</param>
        /// <returns>Returns a new cell ranges array.</returns>
        internal static CellRange[] Remove(CellRange cellRange, int row, int column, int rowCount, int columnCount, int maxRowCount, int maxColumnCount)
        {
            if (cellRange != null)
            {
                if (!cellRange.Intersects(row, column, rowCount, columnCount))
                {
                    return new CellRange[] { cellRange };
                }
                int num = cellRange.Row;
                int num2 = (cellRange.Row + cellRange.RowCount) - 1;
                int num3 = cellRange.Column;
                int num4 = (cellRange.Column + cellRange.ColumnCount) - 1;
                int num5 = row;
                int num6 = (row + rowCount) - 1;
                int num7 = column;
                int num8 = (column + columnCount) - 1;
                List<CellRange> list = new List<CellRange>();
                if ((num == -1) && (num3 != -1))
                {
                    if ((num7 != -1) && (num5 == -1))
                    {
                        if ((num7 - num3) > 0)
                        {
                            CellRange range = new CellRange(Math.Max(num5, num), num3, (Math.Min(num6, num2) - Math.Max(num5, num)) + 1, num7 - num3);
                            list.Add(range);
                        }
                        if ((num4 - num8) > 0)
                        {
                            CellRange range2 = new CellRange(Math.Max(num5, num), num8 + 1, (Math.Min(num6, num2) - Math.Max(num5, num)) + 1, num4 - num8);
                            list.Add(range2);
                        }
                    }
                    else if ((num7 == -1) && (num5 != -1))
                    {
                        num = 0;
                        num2 = maxRowCount - 1;
                        num7 = 0;
                        num8 = maxColumnCount - 1;
                    }
                    else if ((num7 != -1) && (num5 != -1))
                    {
                        num = 0;
                        num2 = maxRowCount - 1;
                    }
                }
                if ((num != -1) && (num3 == -1))
                {
                    if ((num7 != -1) && (num5 == -1))
                    {
                        num3 = 0;
                        num4 = maxColumnCount - 1;
                        num5 = 0;
                        num6 = maxRowCount - 1;
                    }
                    else if ((num7 == -1) && (num5 != -1))
                    {
                        if ((num5 - num) > 0)
                        {
                            CellRange range3 = new CellRange(num, Math.Max(num7, num3), num5 - num, (Math.Min(num8, num4) - Math.Max(num7, num3)) + 1);
                            list.Add(range3);
                        }
                        if ((num2 - num6) > 0)
                        {
                            CellRange range4 = new CellRange(num6 + 1, Math.Max(num7, num3), num2 - num6, (Math.Min(num8, num4) - Math.Max(num7, num3)) + 1);
                            list.Add(range4);
                        }
                    }
                    else if ((num7 != -1) && (num5 != -1))
                    {
                        num3 = 0;
                        num4 = maxColumnCount - 1;
                    }
                }
                if ((num != -1) && (num3 != -1))
                {
                    if ((num5 == -1) && (num7 != -1))
                    {
                        num5 = 0;
                        num6 = maxRowCount;
                    }
                    else if ((num5 != -1) && (num7 == -1))
                    {
                        num7 = 0;
                        num8 = maxColumnCount;
                    }
                    if ((num5 != -1) && (num7 != -1))
                    {
                        if (((num7 - num3) > 0) && ((num5 - num) > 0))
                        {
                            CellRange range5 = new CellRange(num, num3, num5 - num, num7 - num3);
                            list.Add(range5);
                        }
                        if (((Math.Min(num8, num4) - Math.Max(num7, num3)) >= 0) && ((num5 - num) > 0))
                        {
                            CellRange range6 = new CellRange(num, Math.Max(num7, num3), num5 - num, (Math.Min(num8, num4) - Math.Max(num7, num3)) + 1);
                            list.Add(range6);
                        }
                        if (((num4 - num8) > 0) && ((num5 - num) > 0))
                        {
                            CellRange range7 = new CellRange(num, num8 + 1, num5 - num, num4 - num8);
                            list.Add(range7);
                        }
                        if (((num7 - num3) > 0) && ((Math.Min(num6, num2) - Math.Max(num5, num)) >= 0))
                        {
                            CellRange range8 = new CellRange(Math.Max(num5, num), num3, (Math.Min(num6, num2) - Math.Max(num5, num)) + 1, num7 - num3);
                            list.Add(range8);
                        }
                        if (((num4 - num8) > 0) && ((Math.Min(num6, num2) - Math.Max(num5, num)) >= 0))
                        {
                            CellRange range9 = new CellRange(Math.Max(num5, num), num8 + 1, (Math.Min(num6, num2) - Math.Max(num5, num)) + 1, num4 - num8);
                            list.Add(range9);
                        }
                        if (((num7 - num3) > 0) && ((num2 - num6) > 0))
                        {
                            CellRange range10 = new CellRange(num6 + 1, num3, num2 - num6, num7 - num3);
                            list.Add(range10);
                        }
                        if (((Math.Min(num8, num4) - Math.Max(num7, num3)) >= 0) && ((num2 - num6) > 0))
                        {
                            CellRange range11 = new CellRange(num6 + 1, Math.Max(num7, num3), num2 - num6, (Math.Min(num8, num4) - Math.Max(num7, num3)) + 1);
                            list.Add(range11);
                        }
                        if (((num4 - num8) > 0) && ((num2 - num6) > 0))
                        {
                            CellRange range12 = new CellRange(num6 + 1, num8 + 1, num2 - num6, num4 - num8);
                            list.Add(range12);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    return list.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the string representation of the cell range.
        /// </summary>
        /// <returns>Returns the string that represents the cell range.</returns>
        public override string ToString()
        {
            return string.Format("CellRange: {0},{1},{2},{3}", (object[]) new object[] { ((int) this.Row), ((int) this.Column), ((int) this.RowCount), ((int) this.ColumnCount) });
        }

        /// <summary>
        /// Converts a string to a cell range.
        /// </summary>
        /// <param name="s">The string to be parsed.</param>
        /// <param name="result">The cell range represented by the string.</param>
        /// <remarks>If the return value is <c>false</c>, then the <paramref name="result" /> is set to null.</remarks>
        /// <returns><c>true</c> if the string was parsed successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string s, out CellRange result)
        {
            CellRange range;
            CellRange range2;
            if (s == null)
            {
                result = null;
                return false;
            }
            if (s.StartsWith("CellRange: "))
            {
                s = s.Replace("CellRange: ", "");
                string[] strArray = s.Split(new char[] { ',' });
                if (strArray.Length == 4)
                {
                    try
                    {
                        int row = int.Parse(strArray[0].Trim());
                        int column = int.Parse(strArray[1].Trim());
                        int rowCount = int.Parse(strArray[2].Trim());
                        int columnCount = int.Parse(strArray[3].Trim());
                        result = new CellRange(row, column, rowCount, columnCount);
                        return true;
                    }
                    catch
                    {
                        result = null;
                        return false;
                    }
                }
            }
            int index = s.IndexOf(':');
            if (index < 0)
            {
                return TryParseCell(s, out result);
            }
            string str = s.Substring(0, index);
            string str2 = s.Substring(index + 1, (s.Length - index) - 1);
            if (TryParseCell(str, out range) && TryParseCell(str2, out range2))
            {
                int num6 = Math.Min(range.Row, range2.Row);
                int num7 = Math.Min(range.Column, range2.Column);
                int num8 = (Math.Max(range.Row, range2.Row) - num6) + 1;
                int num9 = (Math.Max(range.Column, range2.Column) - num7) + 1;
                result = new CellRange(num6, num7, num8, num9);
                return true;
            }
            result = null;
            return false;
        }

        /// <summary>
        /// Converts a string to a cell range that consists of a single cell.
        /// </summary>
        /// <param name="s">String to be parsed</param>
        /// <param name="result">Cell represented by the string</param>
        /// <returns><c>true</c> if the string is converted successfully; otherwise, <c>false</c>.</returns>
        static bool TryParseCell(string s, out CellRange result)
        {
            int num = 0;
            int row = 0;
            int column = 0;
            bool flag = true;
            flag &= ((num < s.Length) && ('A' <= char.ToUpperInvariant(s[num]))) && (char.ToUpperInvariant(s[num]) <= 'Z');
            while (((num < s.Length) && ('A' <= char.ToUpperInvariant(s[num]))) && (char.ToUpperInvariant(s[num]) <= 'Z'))
            {
                flag &= column <= 0x4ec4ec4;
                column = (((0x1a * column) + char.ToUpperInvariant(s[num])) - 0x41) + 1;
                flag &= (0 <= column) && (column <= 0x7fffffff);
                num++;
            }
            flag &= column > 0;
            column--;
            flag &= ((num < s.Length) && ('1' <= s[num])) && (s[num] <= '9');
            while (((num < s.Length) && ('0' <= s[num])) && (s[num] <= '9'))
            {
                flag &= row <= 0xccccccc;
                row = (10 * row) + (s[num] - '0');
                flag &= (0 <= row) && (row <= 0x7fffffff);
                num++;
            }
            flag &= row > 0;
            row--;
            flag &= num == s.Length;
            result = flag ? new CellRange(row, column, 1, 1) : null;
            return flag;
        }

        /// <summary>
        /// Gets the column index of the first cell in the range.
        /// </summary>
        /// <value>The column index of the first cell in the range.</value>
        public int Column
        {
            get { return  this.column; }
        }

        /// <summary>
        /// Gets the number of columns in the range.
        /// </summary>
        /// <value>The number of columns in the range.</value>
        public int ColumnCount
        {
            get { return  this.columnCount; }
        }

        /// <summary>
        /// Gets the row index of the first cell in the range.
        /// </summary>
        /// <value>The row index of the first cell in the range.</value>
        public int Row
        {
            get { return  this.row; }
        }

        /// <summary>
        /// Gets the number of rows in the range.
        /// </summary>
        /// <value>The number of rows in the range.</value>
        public int RowCount
        {
            get { return  this.rowCount; }
        }
    }
}

