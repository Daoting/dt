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
        private readonly bool _isNotEmpty;
        private readonly SheetArea _area;
        private readonly int _row;
        private readonly int _column;
        public TabularPosition(SheetArea area, int row, int column)
        {
            this = new TabularPosition();
            this._isNotEmpty = true;
            this._area = area;
            this._row = row;
            this._column = column;
        }

        public bool IsEmpty
        {
            get { return  !this._isNotEmpty; }
        }
        public SheetArea Area
        {
            get { return  this._area; }
        }
        public int Row
        {
            get
            {
                if (this.IsEmpty)
                {
                    return -1;
                }
                return this._row;
            }
        }
        public int Column
        {
            get
            {
                if (this.IsEmpty)
                {
                    return -1;
                }
                return this._column;
            }
        }
        public bool Equals(TabularPosition other)
        {
            return ((((this._isNotEmpty == other._isNotEmpty) && (this._area == other._area)) && (this._row == other._row)) && (this._column == other._column));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TabularPosition))
            {
                return false;
            }
            TabularPosition other = (TabularPosition) obj;
            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            if (!this._isNotEmpty)
            {
                return -1;
            }
            return (int) (((((byte) this._area) << 0x1c) ^ (this._row << 14)) ^ this._column);
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

