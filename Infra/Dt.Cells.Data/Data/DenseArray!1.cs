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
using System.Reflection;
#endregion

namespace Dt.Cells.Data
{
    internal class DenseArray<T>
    {
        List<T> _items;
        int _length;
        const int INITLENGTH = 0x3e8;

        public DenseArray() : this(0)
        {
        }

        public DenseArray(int length)
        {
            this._items = new List<T>();
            this._items.InsertRange(0, new T[Math.Min(length, 0x3e8)]);
            this._length = length;
        }

        void CheckBound(int lastIndex)
        {
            if (this._items.Count <= lastIndex)
            {
                this._items.InsertRange(this._items.Count, new T[(lastIndex - this._items.Count) + 1]);
            }
        }

        public void Clear()
        {
            this._items.Clear();
        }

        public void Clear(int index, int count)
        {
            count = Math.Min(count, this._items.Count - index);
            for (int i = 0; i < count; i++)
            {
                T local = default(T);
                this._items[index + i] = local;
            }
        }

        public int FirstNonEmptyIndex()
        {
            return this.NextNonEmptyIndex(-1);
        }

        public List<int> GetNonEmptyIndexes()
        {
            List<int> list = new List<int>();
            for (int i = 0; i < this._items.Count; i++)
            {
                if (this._items[i] != null)
                {
                    list.Add(i);
                }
            }
            return list;
        }

        public void InsertRange(int index, int count)
        {
            if ((index >= 0) && (count >= 1))
            {
                this.CheckBound(index - 1);
                this._items.InsertRange(index, new T[count]);
                this.Length += count;
            }
        }

        public int NextNonEmptyIndex(int row)
        {
            if (row >= -1)
            {
                for (int i = row + 1; i < this._items.Count; i++)
                {
                    if (this._items[i] != null)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public void RemoveRange(int index, int count)
        {
            if (((index >= 0) && (index < this.Length)) && (count >= 0))
            {
                if ((index + count) > this.Length)
                {
                    count = this.Length - index;
                }
                if (index < this._items.Count)
                {
                    this._items.RemoveRange(index, Math.Min(count, this._items.Count - index));
                }
                this.Length -= count;
            }
        }

        public T this[int index]
        {
            get
            {
                if ((index >= 0) && (index < this._items.Count))
                {
                    return this._items[index];
                }
                return default(T);
            }
            set
            {
                this.CheckBound(index);
                this._items[index] = value;
            }
        }

        public int Length
        {
            get { return  this._length; }
            set
            {
                if ((value >= 0) && (value != this._length))
                {
                    this._length = value;
                    if (value < this._items.Count)
                    {
                        this._items.RemoveRange(value, this._items.Count - value);
                    }
                }
            }
        }
    }
}

