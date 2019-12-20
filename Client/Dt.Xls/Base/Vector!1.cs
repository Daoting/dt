#region 文件描述
/******************************************************************************
* 创建: Daoting
* 摘要: 
* 日志: 2014-07-01 创建
******************************************************************************/
#endregion

#region 引用命名
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
#endregion

namespace Dt.Xls
{
    internal class Vector<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T: class
    {
        private int _capacity;
        private List<T> _internalList;
        private int _length;

        public Vector()
        {
            this._capacity = 1;
            this._internalList = new List<T>();
            this._internalList.Add(default(T));
        }

        public void Add(T item)
        {
            if (this._length == this._capacity)
            {
                this.ExpandList(this._capacity * 2);
            }
            this._internalList[this._length] = item;
            this._length++;
        }

        public void Clear()
        {
            this._internalList.Clear();
            this._length = 0;
        }

        public bool Contains(T item)
        {
            return this._internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        private void ExpandList(int capacity)
        {
            if (capacity == 0)
            {
                capacity = 1;
            }
            for (int i = this._internalList.Count; i < capacity; i++)
            {
                T local = default(T);
                this._internalList.Add(local);
            }
            this._capacity = capacity;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T iteratorVariable0 in this._internalList)
            {
                yield return iteratorVariable0;
            }
        }

        internal T GetItem(int index)
        {
            return this._internalList[index];
        }

        public int IndexOf(T item)
        {
            return this._internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (this._length == this._capacity)
            {
                this.ExpandList(this._capacity * 2);
            }
            this._internalList.Insert(index, item);
        }

        public bool Remove(T item)
        {
            bool flag = false;
            if (this._internalList.Remove(item))
            {
                flag = true;
                this._length--;
            }
            return flag;
        }

        public void RemoveAt(int index)
        {
            if (((index >= 0) && (index < this._length)) && (this._internalList.Count > 0))
            {
                this._internalList.RemoveAt(index);
                this._length--;
                this._capacity--;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this.GetEnumerator();
        }

        internal int Capacity
        {
            get { return  this._capacity; }
        }

        public int Count
        {
            get { return  this._length; }
        }

        public bool IsReadOnly
        {
            get { return  false; }
        }

        public T this[int index]
        {
            get
            {
                if ((index < this._capacity) && (index >= 0))
                {
                    return this._internalList[index];
                }
                return default(T);
            }
            set
            {
                if (index >= this._capacity)
                {
                    this.ExpandList(index * 2);
                }
                this._internalList[index] = value;
                if (this._length <= index)
                {
                    this._length = index + 1;
                }
            }
        }
    }
}

