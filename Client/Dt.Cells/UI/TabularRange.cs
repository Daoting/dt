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
        readonly SheetArea _area;
        readonly int _column;
        readonly int _columnCount;
        readonly int _row;
        readonly int _rowCount;
        public static readonly TabularRange WholeSheet = new TabularRange(SheetArea.Cells, -1, -1, -1, -1);

        public TabularRange(TabularPosition position, int rowCount = 1, int columnCount = 1) : this(position.Area, position.Row, position.Column, rowCount, columnCount)
        {
        }

        public TabularRange(SheetArea area, int row, int column, int rowCount = 1, int columnCount = 1)
        {
            _area = area;
            if (row == -1)
            {
                rowCount = -1;
            }
            if (column == -1)
            {
                columnCount = -1;
            }
            _row = row;
            _column = column;
            _rowCount = rowCount;
            _columnCount = columnCount;
        }

        public bool Equals(TabularRange other)
        {
            if (other == null)
            {
                return false;
            }
            return ((((_area == other._area) && (_row == other._row)) && ((_column == other._column) && (_rowCount == other._rowCount))) && (_columnCount == other._columnCount));
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TabularRange);
        }

        internal TabularRange GetFixedRange(int rowCount, int columnCount)
        {
            TabularRange range = null;
            if ((_row != -1) && (_column != -1))
            {
                return new TabularRange(_area, _row, _column, Math.Min(_rowCount, rowCount), Math.Min(_columnCount, columnCount));
            }
            if ((_row == -1) && (_column != -1))
            {
                return new TabularRange(_area, 0, _column, rowCount, Math.Min(_columnCount, columnCount));
            }
            if ((_row != -1) && (_column == -1))
            {
                return new TabularRange(_area, _row, 0, Math.Min(_rowCount, rowCount), columnCount);
            }
            if ((_row == -1) && (_column == -1))
            {
                range = new TabularRange(_area, 0, 0, rowCount, columnCount);
            }
            return range;
        }

        public override int GetHashCode()
        {
            return (int) (((((((byte) _area) << 0x1c) ^ (Row << 0x15)) ^ (RowCount << 14)) ^ (Column << 7)) ^ ColumnCount);
        }

        internal bool IsContains(TabularRange tabularRange)
        {
            if (Area != tabularRange.Area)
            {
                return false;
            }
            return (((tabularRange.Row >= Row) && (tabularRange.Column >= Column)) && ((tabularRange.LastRow <= LastRow) && (tabularRange.LastColumn <= LastColumn)));
        }

        internal bool IsIntersectsWith(TabularRange tabularRange)
        {
            if (Area != tabularRange.Area)
            {
                return false;
            }
            return (IsIntersectsWithColumnRange(tabularRange.Column, tabularRange.ColumnCount) && IsIntersectsWithRowRange(tabularRange.Row, tabularRange.RowCount));
        }

        internal bool IsIntersectsWithColumnRange(int columnIndex, int count)
        {
            int num = (columnIndex + count) - 1;
            return (((columnIndex >= Column) && (columnIndex <= LastColumn)) || (((num >= Column) && (num <= LastColumn)) || (((Column >= columnIndex) && (Column <= num)) || ((LastColumn >= columnIndex) && (LastColumn <= num)))));
        }

        internal bool IsIntersectsWithRowRange(int rowIndex, int count)
        {
            int num = (rowIndex + count) - 1;
            return (((rowIndex >= Row) && (rowIndex <= LastRow)) || (((num >= Row) && (num <= LastRow)) || (((Row >= rowIndex) && (Row <= num)) || ((LastRow >= rowIndex) && (LastRow <= num)))));
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
            return string.Concat((object[]) new object[] { _area, ", ", ((int) _row), ", ", ((int) _column), ", ", ((int) _rowCount), ", ", ((int) _columnCount) });
        }

        public SheetArea Area
        {
            get { return  _area; }
        }

        public int Column
        {
            get { return  _column; }
        }

        public int ColumnCount
        {
            get { return  _columnCount; }
        }

        public TabularPosition this[int row, int column]
        {
            get
            {
                if ((row < 0) || ((_row != -1) && (row > _rowCount)))
                {
                    throw new ArgumentOutOfRangeException("row");
                }
                if ((column < 0) || ((_column != -1) && (column > _columnCount)))
                {
                    throw new ArgumentOutOfRangeException("column");
                }
                if (_row != -1)
                {
                    row += _row;
                }
                if (_column != -1)
                {
                    column += _column;
                }
                return new TabularPosition(_area, row, column);
            }
        }

        internal int LastColumn
        {
            get
            {
                if (_column == -1)
                {
                    return -1;
                }
                return ((_column + _columnCount) - 1);
            }
        }

        internal int LastRow
        {
            get
            {
                if (_row == -1)
                {
                    return -1;
                }
                return ((_row + _rowCount) - 1);
            }
        }

        public CellRangeType RangeType
        {
            get
            {
                if ((_row == -1) && (_column == -1))
                {
                    return CellRangeType.WholeSheet;
                }
                if (_row == -1)
                {
                    return CellRangeType.Columns;
                }
                if (_column == -1)
                {
                    return CellRangeType.Rows;
                }
                return CellRangeType.Cells;
            }
        }

        public int Row
        {
            get { return  _row; }
        }

        public int RowCount
        {
            get { return  _rowCount; }
        }

        internal TabularPosition TopLeft
        {
            get { return  new TabularPosition(_area, _row, _column); }
        }
    }
}

