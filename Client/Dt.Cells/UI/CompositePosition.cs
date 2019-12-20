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
    internal struct CompositePosition : IEquatable<CompositePosition>
    {
        private DataSheetElementType _type;
        private int _row;
        private int _column;
        public static readonly CompositePosition Empty;
        public static readonly CompositePosition Sheet;
        public CompositePosition(DataSheetElementType type, int row, int column)
        {
            this._type = type;
            this._row = row;
            this._column = column;
        }

        public DataSheetElementType Type
        {
            get { return  this._type; }
        }
        public int Row
        {
            get { return  this._row; }
        }
        public int Column
        {
            get { return  this._column; }
        }
        public bool IsEmpty
        {
            get { return  (this._type == DataSheetElementType.Empty); }
        }
        public static bool operator ==(CompositePosition _this, CompositePosition other)
        {
            return (((_this._type == other._type) && (_this._row == other._row)) && (_this._column == other._column));
        }

        public static bool operator !=(CompositePosition _this, CompositePosition other)
        {
            return !(_this == other);
        }

        public bool Equals(CompositePosition other)
        {
            return (this == other);
        }

        public override bool Equals(object obj)
        {
            return ((obj is CompositePosition) && this.Equals((CompositePosition) obj));
        }

        public override int GetHashCode()
        {
            return PositionHelper.GetHashCode(this._type, this._row, this._column);
        }

        static CompositePosition()
        {
            Empty = new CompositePosition(DataSheetElementType.Empty, 0, 0);
            Sheet = new CompositePosition(DataSheetElementType.Sheet, -1, -1);
        }
    }
}

