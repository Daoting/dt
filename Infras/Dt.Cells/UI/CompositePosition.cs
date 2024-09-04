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
        DataSheetElementType _type;
        int _row;
        int _column;
        public static readonly CompositePosition Empty;
        public static readonly CompositePosition Sheet;
        public CompositePosition(DataSheetElementType type, int row, int column)
        {
            _type = type;
            _row = row;
            _column = column;
        }

        public DataSheetElementType Type
        {
            get { return  _type; }
        }
        public int Row
        {
            get { return  _row; }
        }
        public int Column
        {
            get { return  _column; }
        }
        public bool IsEmpty
        {
            get { return  (_type == DataSheetElementType.Empty); }
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
            return ((obj is CompositePosition) && Equals((CompositePosition) obj));
        }

        public override int GetHashCode()
        {
            return PositionHelper.GetHashCode(_type, _row, _column);
        }

        static CompositePosition()
        {
            Empty = new CompositePosition(DataSheetElementType.Empty, 0, 0);
            Sheet = new CompositePosition(DataSheetElementType.Sheet, -1, -1);
        }
    }
}

