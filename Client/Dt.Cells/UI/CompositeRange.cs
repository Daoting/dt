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
        private DataSheetElementType _type;
        private int _row;
        private int _rowCount;
        private int _column;
        private int _columnCount;
        public static readonly CompositeRange Empty;
        public static readonly CompositeRange Sheet;
        public CompositeRange(DataSheetElementType type, int row, int column, int rowCount, int columnCount)
        {
            this._type = type;
            this._row = row;
            this._rowCount = rowCount;
            this._column = column;
            this._columnCount = columnCount;
        }

        public static CompositeRange FromStartEnd(DataSheetElementType type, int startRow, int endRow, int startColumn, int endColumn)
        {
            return new CompositeRange(type, startRow, startColumn, (endRow - startRow) + 1, (endColumn - startColumn) + 1);
        }

        public DataSheetElementType Type
        {
            get { return  this._type; }
        }
        public int Row
        {
            get { return  this._row; }
        }
        public int RowCount
        {
            get { return  this._rowCount; }
        }
        public int Column
        {
            get { return  this._column; }
        }
        public int ColumnCount
        {
            get { return  this._columnCount; }
        }
        public int StartRow
        {
            get { return  this._row; }
        }
        public int EndRow
        {
            get
            {
                if (this._row != -1)
                {
                    return ((this._row + this._rowCount) - 1);
                }
                return -1;
            }
        }
        public int StartColumn
        {
            get { return  this._column; }
        }
        public int EndColumn
        {
            get
            {
                if (this._column != -1)
                {
                    return ((this._column + this._columnCount) - 1);
                }
                return -1;
            }
        }
        public bool IsEmpty
        {
            get { return  (this._type == DataSheetElementType.Empty); }
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
            return ((obj is CompositeRange) && this.Equals((CompositeRange) obj));
        }

        public override int GetHashCode()
        {
            return RangeHelper.GetHashCode(this._type, this._row, this._rowCount, this._column, this._columnCount);
        }

        static CompositeRange()
        {
            Empty = new CompositeRange();
            Sheet = new CompositeRange(DataSheetElementType.Sheet, -1, -1, -1, -1);
        }
    }
}

