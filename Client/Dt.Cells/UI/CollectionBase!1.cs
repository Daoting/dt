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
        List<T> _collectionBaseInternal;
        Dictionary<int, T> _itemsMap;
        PropertyInfo _keyProperty;

        public CollectionBase(string _keyPropertyName)
        {
            _itemsMap = new Dictionary<int, T>();
            _collectionBaseInternal = new List<T>();
            _keyProperty = typeof(T).GetRuntimeProperty(_keyPropertyName);
        }

        public CollectionBase(IEnumerable<T> collection, string _keyPropertyName)
        {
            _itemsMap = new Dictionary<int, T>();
            _collectionBaseInternal = new List<T>(collection);
            _keyProperty = typeof(T).GetRuntimeProperty(_keyPropertyName);
        }

        public virtual void Add(T item)
        {
            _collectionBaseInternal.Add(item);
            _itemsMap.Add((int) ((int) _keyProperty.GetValue(item, null)), item);
        }

        public void Clear()
        {
            _collectionBaseInternal.Clear();
            _itemsMap.Clear();
        }

        public T Find(int index)
        {
            if (_itemsMap.ContainsKey(index))
            {
                return _itemsMap[index];
            }
            return default(T);
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Count; i++)
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
            _itemsMap.Remove((int) ((int) _keyProperty.GetValue(item, null)));
            return _collectionBaseInternal.Remove(item);
        }

        bool ICollection<T>.Contains(T item)
        {
            return _collectionBaseInternal.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            _collectionBaseInternal.CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>) _collectionBaseInternal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) _collectionBaseInternal.GetEnumerator();
        }

        public int Count
        {
            get { return  _collectionBaseInternal.Count; }
        }

        public T this[int index]
        {
            get { return  _collectionBaseInternal[index]; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return  false; }
        }
    }
}

