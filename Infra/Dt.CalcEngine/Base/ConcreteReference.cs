#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Runtime.InteropServices;
#endregion

namespace Dt.CalcEngine
{
    internal class ConcreteReference : CalcReference
    {
        private readonly Range[] _ranges;
        private readonly CalcReference _source;

        public ConcreteReference(CalcReference source, Range[] areas)
        {
            this._source = source;
            this._ranges = areas;
        }

        public ConcreteReference(CalcReference source, int row, int column, int rowCount, int columnCount)
        {
            this._source = source;
            this._ranges = new Range[] { new Range(row, column, rowCount, columnCount) };
        }

        protected object GetActualValue(int area, int rowOffset, int columnOffset)
        {
            return this._source.GetValue(area, (this.GetRow(area) - this._source.GetRow(area)) + rowOffset, (this.GetColumn(area) - this._source.GetColumn(area)) + columnOffset);
        }

        public override int GetColumn(int area)
        {
            return this._ranges[area].Column;
        }

        public override int GetColumnCount(int area)
        {
            return this._ranges[area].ColumnCount;
        }

        public override int GetRow(int area)
        {
            return this._ranges[area].Row;
        }

        public override int GetRowCount(int area)
        {
            return this._ranges[area].RowCount;
        }

        public override CalcReference GetSource()
        {
            return this._source;
        }

        public override object GetValue(int area, int rowOffset, int columnOffset)
        {
            return this.GetActualValue(area, rowOffset, columnOffset);
        }

        public override bool IsSubtotal(int area, int rowOffset, int columnOffset)
        {
            return this._source.IsSubtotal(area, (this.GetRow(area) - this._source.GetRow(area)) + rowOffset, (this.GetColumn(area) - this._source.GetColumn(area)) + columnOffset);
        }

        public override int RangeCount
        {
            get
            {
                return this._ranges.Length;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Range
        {
            private readonly int _row;
            private readonly int _column;
            private readonly int _rowCount;
            private readonly int _columnCount;
            public Range(int row, int column, int rowCount, int columnCount)
            {
                this._row = row;
                this._column = column;
                this._rowCount = rowCount;
                this._columnCount = columnCount;
            }

            public int Row
            {
                get
                {
                    return this._row;
                }
            }
            public int Column
            {
                get
                {
                    return this._column;
                }
            }
            public int RowCount
            {
                get
                {
                    return this._rowCount;
                }
            }
            public int ColumnCount
            {
                get
                {
                    return this._columnCount;
                }
            }
        }
    }
}

