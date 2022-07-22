#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2013-10-07 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
#endregion

namespace Dt.CalcEngine
{
    internal class SliceArray : CalcArray
    {
        private object _array;
        private int _column;
        private int _columnCount;
        private int _row;
        private int _rowCount;

        public SliceArray(object array, int row, int column, int rowCount, int columnCount)
        {
            this._array = array;
            this._row = row;
            this._column = column;
            this._rowCount = rowCount;
            this._columnCount = columnCount;
        }

        public override object GetValue(int row, int column)
        {
            return ArrayHelper.GetValue(this._array, this._row + row, this._column + column, 0);
        }

        public override int ColumnCount
        {
            get
            {
                return this._columnCount;
            }
        }

        public override int RowCount
        {
            get
            {
                return this._rowCount;
            }
        }
    }
}

