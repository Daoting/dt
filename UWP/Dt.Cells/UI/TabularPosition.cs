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
using System.Runtime.InteropServices;
#endregion

namespace Dt.Cells.UI
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TabularPosition
    {
        public static readonly TabularPosition Empty;
        readonly bool _isNotEmpty;
        readonly SheetArea _area;
        readonly int _row;
        readonly int _column;
        public TabularPosition(SheetArea area, int row, int column)
        {
            this = new TabularPosition();
            _isNotEmpty = true;
            _area = area;
            _row = row;
            _column = column;
        }

        public bool IsEmpty
        {
            get { return  !_isNotEmpty; }
        }
        public SheetArea Area
        {
            get { return  _area; }
        }
        public int Row
        {
            get
            {
                if (IsEmpty)
                {
                    return -1;
                }
                return _row;
            }
        }
        public int Column
        {
            get
            {
                if (IsEmpty)
                {
                    return -1;
                }
                return _column;
            }
        }
        public bool Equals(TabularPosition other)
        {
            return ((((_isNotEmpty == other._isNotEmpty) && (_area == other._area)) && (_row == other._row)) && (_column == other._column));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TabularPosition))
            {
                return false;
            }
            TabularPosition other = (TabularPosition) obj;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            if (!_isNotEmpty)
            {
                return -1;
            }
            return (int) (((((byte) _area) << 0x1c) ^ (_row << 14)) ^ _column);
        }

        public static bool operator ==(TabularPosition left, TabularPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(TabularPosition left, TabularPosition right)
        {
            return !left.Equals(right);
        }

        static TabularPosition()
        {
            Empty = new TabularPosition();
        }
    }
}

