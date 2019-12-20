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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#endregion

namespace Dt.Cells.UI
{
    internal class CollectionBase<T> : ICollection<T>, IEnumerable<T>, IEnumerable where T: class
    {
        private List<T> _collectionBaseInternal;
        private Dictionary<int, T> _itemsMap;
        private PropertyInfo _keyProperty;

        public CollectionBase(string _keyPropertyName)
        {
            this._itemsMap = new Dictionary<int, T>();
            this._collectionBaseInternal = new List<T>();
            this._keyProperty = typeof(T).GetRuntimeProperty(_keyPropertyName);
        }

        public CollectionBase(IEnumerable<T> collection, string _keyPropertyName)
        {
            this._itemsMap = new Dictionary<int, T>();
            this._collectionBaseInternal = new List<T>(collection);
            this._keyProperty = typeof(T).GetRuntimeProperty(_keyPropertyName);
        }

        public virtual void Add(T item)
        {
            this._collectionBaseInternal.Add(item);
            this._itemsMap.Add((int) ((int) this._keyProperty.GetValue(item, null)), item);
        }

        public void Clear()
        {
            this._collectionBaseInternal.Clear();
            this._itemsMap.Clear();
        }

        public T Find(int index)
        {
            if (this._itemsMap.ContainsKey(index))
            {
                return this._itemsMap[index];
            }
            return default(T);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (Enumerable.ElementAt<T>((IEnumerable<T>) this, i) == item)
                {
                    return i;
                }
            }
            return -1;
        }

        public virtual bool Remove(T item)
        {
            this._itemsMap.Remove((int) ((int) this._keyProperty.GetValue(item, null)));
            return this._collectionBaseInternal.Remove(item);
        }

        bool ICollection<T>.Contains(T item)
        {
            return this._collectionBaseInternal.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            this._collectionBaseInternal.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>) this._collectionBaseInternal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) this._collectionBaseInternal.GetEnumerator();
        }

        public int Count
        {
            get { return  this._collectionBaseInternal.Count; }
        }

        public T this[int index]
        {
            get { return  this._collectionBaseInternal[index]; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return  false; }
        }
    }
}

