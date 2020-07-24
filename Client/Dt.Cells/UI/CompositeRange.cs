#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-03 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CompositeRange : IEquatable<CompositeRange>
    {
        DataSheetElementType _type;
        int _row;
        int _rowCount;
        int _column;
        int _columnCount;
        public static readonly CompositeRange Empty;
        public static readonly CompositeRange Sheet;
        public CompositeRange(DataSheetElementType type, int row, int column, int rowCount, int columnCount)
        {
            _type = type;
            _row = row;
            _rowCount = rowCount;
            _column = column;
            _columnCount = columnCount;
        }

        public static CompositeRange FromStartEnd(DataSheetElementType type, int startRow, int endRow, int startColumn, int endColumn)
        {
            return new CompositeRange(type, startRow, startColumn, (endRow - startRow) + 1, (endColumn - startColumn) + 1);
        }

        public DataSheetElementType Type
        {
            get { return  _type; }
        }
        public int Row
        {
            get { return  _row; }
        }
        public int RowCount
        {
            get { return  _rowCount; }
        }
        public int Column
        {
            get { return  _column; }
        }
        public int ColumnCount
        {
            get { return  _columnCount; }
        }
        public int StartRow
        {
            get { return  _row; }
        }
        public int EndRow
        {
            get
            {
                if (_row != -1)
                {
                    return ((_row + _rowCount) - 1);
                }
                return -1;
            }
        }
        public int StartColumn
        {
            get { return  _column; }
        }
        public int EndColumn
        {
            get
            {
                if (_column != -1)
                {
                    return ((_column + _columnCount) - 1);
                }
                return -1;
            }
        }
        public bool IsEmpty
        {
            get { return  (_type == DataSheetElementType.Empty); }
        }
        public static bool operator ==(CompositeRange _this, CompositeRange other)
        {
            return ((((_this._type == other._type) && (_this._row == other._row)) && ((_this._rowCount == other._rowCount) && (_this._column == other._column))) && (_this._columnCount == other._columnCount));
        }

        public static bool operator !=(CompositeRange _this, CompositeRange other)
        {
            return !(_this == other);
        }

        public bool Equals(CompositeRange other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            return ((obj is CompositeRange) && Equals((CompositeRange) obj));
        }

        public override int GetHashCode()
        {
            return RangeHelper.GetHashCode(_type, _row, _rowCount, _column, _columnCount);
        }

        static CompositeRange()
        {
            Empty = new CompositeRange();
            Sheet = new CompositeRange(DataSheetElementType.Sheet, -1, -1, -1, -1);
        }
    }
}

