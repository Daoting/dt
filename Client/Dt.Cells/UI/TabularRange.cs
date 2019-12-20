#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using Dt.Cells.Data;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    internal class TabularRange : IEquatable<TabularRange>
    {
        private readonly SheetArea _area;
        private readonly int _column;
        private readonly int _columnCount;
        private readonly int _row;
        private readonly int _rowCount;
        public static readonly TabularRange WholeSheet = new TabularRange(SheetArea.Cells, -1, -1, -1, -1);

        public TabularRange(TabularPosition position, int rowCount = 1, int columnCount = 1) : this(position.Area, position.Row, position.Column, rowCount, columnCount)
        {
        }

        public TabularRange(SheetArea area, int row, int column, int rowCount = 1, int columnCount = 1)
        {
            this._area = area;
            if (row == -1)
            {
                rowCount = -1;
            }
            if (column == -1)
            {
                columnCount = -1;
            }
            this._row = row;
            this._column = column;
            this._rowCount = rowCount;
            this._columnCount = columnCount;
        }

        public bool Equals(TabularRange other)
        {
            if (other == null)
            {
                return false;
            }
            return ((((this._area == other._area) && (this._row == other._row)) && ((this._column == other._column) && (this._rowCount == other._rowCount))) && (this._columnCount == other._columnCount));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as TabularRange);
        }

        internal TabularRange GetFixedRange(int rowCount, int columnCount)
        {
            TabularRange range = null;
            if ((this._row != -1) && (this._column != -1))
            {
                return new TabularRange(this._area, this._row, this._column, Math.Min(this._rowCount, rowCount), Math.Min(this._columnCount, columnCount));
            }
            if ((this._row == -1) && (this._column != -1))
            {
                return new TabularRange(this._area, 0, this._column, rowCount, Math.Min(this._columnCount, columnCount));
            }
            if ((this._row != -1) && (this._column == -1))
            {
                return new TabularRange(this._area, this._row, 0, Math.Min(this._rowCount, rowCount), columnCount);
            }
            if ((this._row == -1) && (this._column == -1))
            {
                range = new TabularRange(this._area, 0, 0, rowCount, columnCount);
            }
            return range;
        }

        public override int GetHashCode()
        {
            return (int) (((((((byte) this._area) << 0x1c) ^ (this.Row << 0x15)) ^ (this.RowCount << 14)) ^ (this.Column << 7)) ^ this.ColumnCount);
        }

        internal bool IsContains(TabularRange tabularRange)
        {
            if (this.Area != tabularRange.Area)
            {
                return false;
            }
            return (((tabularRange.Row >= this.Row) && (tabularRange.Column >= this.Column)) && ((tabularRange.LastRow <= this.LastRow) && (tabularRange.LastColumn <= this.LastColumn)));
        }

        internal bool IsIntersectsWith(TabularRange tabularRange)
        {
            if (this.Area != tabularRange.Area)
            {
                return false;
            }
            return (this.IsIntersectsWithColumnRange(tabularRange.Column, tabularRange.ColumnCount) && this.IsIntersectsWithRowRange(tabularRange.Row, tabularRange.RowCount));
        }

        internal bool IsIntersectsWithColumnRange(int columnIndex, int count)
        {
            int num = (columnIndex + count) - 1;
            return (((columnIndex >= this.Column) && (columnIndex <= this.LastColumn)) || (((num >= this.Column) && (num <= this.LastColumn)) || (((this.Column >= columnIndex) && (this.Column <= num)) || ((this.LastColumn >= columnIndex) && (this.LastColumn <= num)))));
        }

        internal bool IsIntersectsWithRowRange(int rowIndex, int count)
        {
            int num = (rowIndex + count) - 1;
            return (((rowIndex >= this.Row) && (rowIndex <= this.LastRow)) || (((num >= this.Row) && (num <= this.LastRow)) || (((this.Row >= rowIndex) && (this.Row <= num)) || ((this.LastRow >= rowIndex) && (this.LastRow <= num)))));
        }

        public static bool operator ==(TabularRange x, TabularRange y)
        {
            return object.Equals(x, y);
        }

        public static bool operator !=(TabularRange x, TabularRange y)
        {
            return !object.Equals(x, y);
        }

        public override string ToString()
        {
            return string.Concat((object[]) new object[] { this._area, ", ", ((int) this._row), ", ", ((int) this._column), ", ", ((int) this._rowCount), ", ", ((int) this._columnCount) });
        }

        public SheetArea Area
        {
            get { return  this._area; }
        }

        public int Column
        {
            get { return  this._column; }
        }

        public int ColumnCount
        {
            get { return  this._columnCount; }
        }

        public TabularPosition this[int row, int column]
        {
            get
            {
                if ((row < 0) || ((this._row != -1) && (row > this._rowCount)))
                {
                    throw new ArgumentOutOfRangeException("row");
                }
                if ((column < 0) || ((this._column != -1) && (column > this._columnCount)))
                {
                    throw new ArgumentOutOfRangeException("column");
                }
                if (this._row != -1)
                {
                    row += this._row;
                }
                if (this._column != -1)
                {
                    column += this._column;
                }
                return new TabularPosition(this._area, row, column);
            }
        }

        internal int LastColumn
        {
            get
            {
                if (this._column == -1)
                {
                    return -1;
                }
                return ((this._column + this._columnCount) - 1);
            }
        }

        internal int LastRow
        {
            get
            {
                if (this._row == -1)
                {
                    return -1;
                }
                return ((this._row + this._rowCount) - 1);
            }
        }

        public CellRangeType RangeType
        {
            get
            {
                if ((this._row == -1) && (this._column == -1))
                {
                    return CellRangeType.WholeSheet;
                }
                if (this._row == -1)
                {
                    return CellRangeType.Columns;
                }
                if (this._column == -1)
                {
                    return CellRangeType.Rows;
                }
                return CellRangeType.Cells;
            }
        }

        public int Row
        {
            get { return  this._row; }
        }

        public int RowCount
        {
            get { return  this._rowCount; }
        }

        internal TabularPosition TopLeft
        {
            get { return  new TabularPosition(this._area, this._row, this._column); }
        }
    }
}

