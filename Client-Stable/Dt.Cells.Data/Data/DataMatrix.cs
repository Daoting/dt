#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-02 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections.Generic;
#endregion

namespace Dt.Cells.Data
{
    internal class DataMatrix<T> where T: class
    {
        SparseArray<DenseArray<T>> _table;
        int columnCount;
        int lastDirtyColumn;
        int lastDirtyRow;
        int rowCount;

        public DataMatrix()
        {
            this.lastDirtyRow = -1;
            this.lastDirtyColumn = -1;
            this._table = new SparseArray<DenseArray<T>>();
        }

        public DataMatrix(int row, int column)
        {
            this.lastDirtyRow = -1;
            this.lastDirtyColumn = -1;
            this._table = new SparseArray<DenseArray<T>>();
            this.RowCount = row;
            this.ColumnCount = column;
        }

        public void AddColumns(int column, int count)
        {
            if (((column >= 0) && (column <= this.columnCount)) && (count >= 0))
            {
                using (IEnumerator<DenseArray<T>> enumerator = this._table.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.InsertRange(column, count);
                    }
                }
                this.ColumnCount += count;
                if (column <= this.lastDirtyColumn)
                {
                    this.lastDirtyColumn += count;
                }
            }
        }

        public void AddRows(int row, int count)
        {
            if (((row >= 0) && (row <= this.rowCount)) && (count >= 0))
            {
                this._table.InsertRange(row, count);
                this.RowCount += count;
                if (row <= this.lastDirtyRow)
                {
                    this.lastDirtyRow += count;
                }
            }
        }

        public void Clear()
        {
            this._table.Clear();
        }

        public void Clear(int row, int column, int rowCount, int columnCount)
        {
            for (int i = 0; i < rowCount; i++)
            {
                DenseArray<T> array = this.GetRow(i + row, false);
                if (array != null)
                {
                    array.Clear(column, columnCount);
                }
            }
        }

        public void ClearRows(int row, int count)
        {
            for (int i = 0; i < count; i++)
            {
                DenseArray<T> array = this._table[row + i];
                if (array != null)
                {
                    array.Clear();
                }
            }
        }

        public int FirstNonEmptyRow()
        {
            return this._table.FirstNonEmptyIndex();
        }

        public List<int> GetNonEmptyColumns()
        {
            List<int> list = new List<int>();
            for (int i = 0; i <= this.lastDirtyColumn; i++)
            {
                list.Add(i);
            }
            return list;
        }

        public List<int> GetNonEmptyRows()
        {
            return this._table.GetNonEmptyIndexes();
        }

        DenseArray<T> GetRow(int row, bool create)
        {
            DenseArray<T> array = this._table[row];
            if (create && (array == null))
            {
                array = new DenseArray<T>(this.columnCount);
                this._table[row] = array;
            }
            return array;
        }

        public T GetValue(int row, int column)
        {
            DenseArray<T> array = this.GetRow(row, false);
            if (array != null)
            {
                return array[column];
            }
            return default(T);
        }

        public int NextNonEmptyRow(int row)
        {
            return this._table.NextNonEmptyIndex(row);
        }

        public void RemoveColumns(int column, int count)
        {
            if (((column >= 0) && (column < this.columnCount)) && (count >= 0))
            {
                if ((column + count) > this.columnCount)
                {
                    count = this.columnCount - column;
                }
                using (IEnumerator<DenseArray<T>> enumerator = this._table.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        enumerator.Current.RemoveRange(column, count);
                    }
                }
                this.columnCount -= count;
                if (column <= this.lastDirtyColumn)
                {
                    this.lastDirtyColumn = ((this.lastDirtyColumn - column) >= count) ? (this.lastDirtyColumn -= count) : (column - 1);
                }
            }
        }

        public void RemoveRows(int row, int count)
        {
            if (((row >= 0) && (row < this.rowCount)) && (count >= 0))
            {
                if ((row + count) > this.rowCount)
                {
                    count = this.rowCount - row;
                }
                this._table.RemoveRange(row, count);
                this.rowCount -= count;
                if (row <= this.lastDirtyRow)
                {
                    this.lastDirtyRow = ((this.lastDirtyRow - row) >= count) ? (this.lastDirtyRow -= count) : (row - 1);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        public void SetValue(int row, int column, T value)
        {
            // hdt
            this.GetRow(row, true)[column] = value;
            if (row > this.lastDirtyRow)
            {
                this.lastDirtyRow = row;
            }
            if (column > this.lastDirtyColumn)
            {
                this.lastDirtyColumn = column;
            }
        }

        public int ColumnCount
        {
            get { return  this.columnCount; }
            set
            {
                if ((0 <= value) && (value != this.columnCount))
                {
                    this.columnCount = value;
                    using (IEnumerator<DenseArray<T>> enumerator = this._table.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.Length = this.columnCount;
                        }
                    }
                }
                if (value <= this.lastDirtyColumn)
                {
                    this.lastDirtyColumn = value - 1;
                }
            }
        }

        public int LastDirtyColumn
        {
            get { return  this.lastDirtyColumn; }
        }

        public int LastDirtyRow
        {
            get { return  this.lastDirtyRow; }
        }

        public int RowCount
        {
            get { return  this.rowCount; }
            set
            {
                this.rowCount = value;
                this._table.Length = this.rowCount;
                if (value <= this.lastDirtyRow)
                {
                    this.lastDirtyRow = value - 1;
                }
            }
        }
    }
}

