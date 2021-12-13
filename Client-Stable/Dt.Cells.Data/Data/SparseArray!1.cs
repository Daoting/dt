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
    internal class SparseArray<T>
    {
        Dictionary<int, T> _items;
        int _length;
        int lastDirtyIndex;

        public SparseArray() : this(0)
        {
        }

        public SparseArray(int length)
        {
            this.lastDirtyIndex = -1;
            this._items = new Dictionary<int, T>();
            this.Length = length;
        }

        public void Clear()
        {
            this._items.Clear();
        }

        public void Clear(int index)
        {
            this.Clear(index, 1);
        }

        public void Clear(int index, int count)
        {
            for (int i = 0; i < count; i++)
            {
                this._items.Remove(index + i);
            }
        }

        public bool ContainsIndex(int index)
        {
            return this._items.ContainsKey(index);
        }

        public int FirstNonEmptyIndex()
        {
            return this.NextNonEmptyIndex(-1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>) this._items.Values.GetEnumerator();
        }

        public List<int> GetNonEmptyIndexes()
        {
            int[] numArray = new int[this._items.Count];
            this._items.Keys.CopyTo(numArray, 0);
            return new List<int>(numArray);
        }

        public void InsertRange(int index, int count)
        {
            if ((index >= 0) && (count >= 1))
            {
                Dictionary<int, T> dictionary = new Dictionary<int, T>();
                foreach (int num in this._items.Keys)
                {
                    if (num >= index)
                    {
                        dictionary.Add(num, this._items[num]);
                    }
                }
                foreach (int num2 in dictionary.Keys)
                {
                    this._items.Remove(num2);
                }
                foreach (int num3 in dictionary.Keys)
                {
                    this._items.Add(num3 + count, dictionary[num3]);
                }
                this.Length += count;
                if (index <= this.lastDirtyIndex)
                {
                    this.lastDirtyIndex += count;
                }
            }
        }

        public int NextNonEmptyIndex(int index)
        {
            if (this._items.Count != 0)
            {
                for (int i = index + 1; i < this.Length; i++)
                {
                    if (this._items.ContainsKey(i))
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
                Dictionary<int, T> dictionary = new Dictionary<int, T>();
                for (int i = 0; i < count; i++)
                {
                    if (this._items.ContainsKey(index + i))
                    {
                        this._items.Remove(index + i);
                    }
                }
                foreach (int num2 in this._items.Keys)
                {
                    if (num2 > index)
                    {
                        dictionary.Add(num2, this._items[num2]);
                    }
                }
                foreach (int num3 in dictionary.Keys)
                {
                    this._items.Remove(num3);
                }
                foreach (int num4 in dictionary.Keys)
                {
                    this._items.Add(num4 - count, dictionary[num4]);
                }
                this.Length -= count;
                if (index <= this.lastDirtyIndex)
                {
                    this.lastDirtyIndex = ((this.lastDirtyIndex - index) >= count) ? (this.lastDirtyIndex -= count) : (index - 1);
                }
            }
        }

        public int DataLength
        {
            get { return  this._items.Count; }
        }

        public T this[int index]
        {
            get
            {
                if (this._items.ContainsKey(index))
                {
                    return this._items[index];
                }
                return default(T);
            }
            set
            {
                this._items[index] = value;
                if (index > this.lastDirtyIndex)
                {
                    this.lastDirtyIndex = index;
                }
            }
        }

        public int LastDirtyIndex
        {
            get { return  this.lastDirtyIndex; }
        }

        public int Length
        {
            get { return  this._length; }
            set
            {
                if ((value >= 0) && (value != this._length))
                {
                    this._length = value;
                    Dictionary<int, T> dictionary = new Dictionary<int, T>();
                    foreach (int num in this._items.Keys)
                    {
                        if (num >= value)
                        {
                            dictionary.Add(num, this._items[num]);
                        }
                    }
                    foreach (int num2 in dictionary.Keys)
                    {
                        this._items.Remove(num2);
                    }
                    if (value <= this.lastDirtyIndex)
                    {
                        this.lastDirtyIndex = value - 1;
                    }
                }
            }
        }
    }
}

